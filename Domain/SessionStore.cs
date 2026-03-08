using System.Linq;

namespace DamageMeterRebuilt.Domain;

internal sealed class SessionStore
{
    private readonly PersonalRecords _records;
    private readonly Action<PersonalRecords>? _recordsChanged;
    private RunStats? _overallCache;
    private bool _overallDirty = true;

    public EncounterSession? CurrentEncounter { get; private set; }
    public List<EncounterSegment> ArchivedSegments { get; } = new();
    public bool IsTracking => CurrentEncounter is not null;

    public event Action? Changed;

    public SessionStore(PersonalRecords? records = null, Action<PersonalRecords>? recordsChanged = null)
    {
        _records = records?.Clone() ?? new PersonalRecords();
        _recordsChanged = recordsChanged;
    }

    public void ResetAll()
    {
        if (CurrentEncounter is not null)
        {
            CurrentEncounter = new EncounterSession
            {
                EncounterKey = CurrentEncounter.EncounterKey,
                CurrentTurn = Math.Max(CurrentEncounter.CurrentTurn, 1),
                LastSeenHistoryIndex = CurrentEncounter.LastSeenHistoryIndex
            };
        }
        else
        {
            CurrentEncounter = null;
        }

        ArchivedSegments.Clear();
        _overallCache = null;
        _overallDirty = true;
        Changed?.Invoke();
    }

    public void StartEncounter(string encounterKey)
    {
        ArchiveCurrentEncounterIfMeaningful();
        CurrentEncounter = new EncounterSession
        {
            EncounterKey = encounterKey,
            CurrentTurn = 0,
            LastSeenHistoryIndex = 0
        };
        MarkDirty();
    }

    public void ArchiveCurrentEncounterIfMeaningful()
    {
        if (CurrentEncounter is null || !CurrentEncounter.HasMeaningfulData())
        {
            CurrentEncounter = null;
            return;
        }

        var segment = CurrentEncounter.Archive();
        ArchivedSegments.Add(segment);
        UpdateRecords(segment);
        CurrentEncounter = null;
        MarkDirty();
    }

    public void MarkDirty()
    {
        _overallDirty = true;
        Changed?.Invoke();
    }

    public RunStats GetOverallView()
    {
        if (!_overallDirty && _overallCache is not null)
        {
            return _overallCache;
        }

        var overall = new RunStats();

        foreach (var segment in ArchivedSegments)
        {
            MergePlayersInto(segment.Players, overall.OverallPlayers);
        }

        if (CurrentEncounter is not null)
        {
            MergePlayersInto(CurrentEncounter.Players, overall.OverallPlayers);
        }

        overall.Records.CopyFrom(_records);
        _overallCache = overall;
        _overallDirty = false;
        return overall;
    }

    public PlayerStats GetOrCreatePlayer(string key, string displayName)
    {
        var encounter = CurrentEncounter ?? throw new InvalidOperationException("No active encounter.");
        if (!encounter.Players.TryGetValue(key, out var stats))
        {
            stats = new PlayerStats
            {
                Key = key,
                DisplayName = displayName
            };
            encounter.Players[key] = stats;
        }

        return stats;
    }

    public void CaptureDeathSnapshot(string playerKey, int maxEvents)
    {
        if (CurrentEncounter is null || CurrentEncounter.DeathSnapshots.ContainsKey(playerKey))
        {
            return;
        }

        var recent = CurrentEncounter.Timeline
            .Where(item => item.ActorKey == playerKey && IsLegacyCombatLogEvent(item.Type))
            .TakeLast(maxEvents)
            .ToList();

        CurrentEncounter.DeathSnapshots[playerKey] = recent;
    }

    private void MergePlayersInto(
        Dictionary<string, PlayerStats> source,
        Dictionary<string, PlayerStats> target)
    {
        foreach (var (key, player) in source)
        {
            if (!target.TryGetValue(key, out var merged))
            {
                target[key] = player.Clone();
                continue;
            }

            merged.MergeFrom(player);
        }
    }

    private void UpdateRecords(EncounterSegment segment)
    {
        var records = _records;
        records.TotalFights += 1;

        foreach (var player in segment.Players.Values)
        {
            records.TotalDamage += player.DamageDealt;
            records.MostFightDamage = Math.Max(records.MostFightDamage, player.DamageDealt);
            records.MostCardsPlayed = Math.Max(records.MostCardsPlayed, player.CardsPlayed);
            records.MostBlockGained = Math.Max(records.MostBlockGained, player.TotalBlockGained);

            if (player.MaxSingleHit > records.HighestHit)
            {
                records.HighestHit = player.MaxSingleHit;
                records.HighestHitCard = player.MaxSingleHitCard;
            }

            foreach (var turnDamage in player.DamagePerTurn.Values)
            {
                records.BestTurnDamage = Math.Max(records.BestTurnDamage, turnDamage);
            }
        }

        _recordsChanged?.Invoke(_records.Clone());
    }

    private static bool IsLegacyCombatLogEvent(TimelineEventType type)
    {
        return type is TimelineEventType.DamageDealt
            or TimelineEventType.DamageTaken
            or TimelineEventType.BlockGained
            or TimelineEventType.CardPlayed
            or TimelineEventType.PotionUsed
            or TimelineEventType.DebuffApplied;
    }
}
