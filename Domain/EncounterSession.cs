namespace DamageMeterRebuilt.Domain;

internal sealed class EncounterSession
{
    public required string EncounterKey { get; init; }
    public int CurrentTurn { get; set; }
    public int LastSeenHistoryIndex { get; set; }
    public Dictionary<string, PlayerStats> Players { get; } = new(StringComparer.Ordinal);
    public List<TimelineEvent> Timeline { get; } = new();
    public Dictionary<string, List<TimelineEvent>> DeathSnapshots { get; } = new(StringComparer.Ordinal);

    public bool HasMeaningfulData()
    {
        return Players.Values.Any(
            player => player.DamageDealt > 0
                || player.DamageTaken > 0
                || player.TotalBlockGained > 0
                || player.CardsPlayed > 0
                || player.CardsDrawn > 0
                || player.PotionsUsed > 0
                || player.DebuffsApplied > 0
                || player.OrbsChanneled > 0
                || player.CardsGenerated > 0
                || player.AfflictionsApplied > 0
                || player.SummonsCreated > 0
                || player.StarsModified != 0);
    }

    public EncounterSegment Archive()
    {
        var segment = new EncounterSegment
        {
            EncounterKey = EncounterKey,
            TurnCount = CurrentTurn
        };

        foreach (var (key, value) in Players)
        {
            segment.Players[key] = value.Clone();
        }

        segment.Timeline.AddRange(Timeline);
        foreach (var (key, value) in DeathSnapshots)
        {
            segment.DeathSnapshots[key] = value.ToList();
        }
        return segment;
    }
}
