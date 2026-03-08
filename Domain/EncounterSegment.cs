namespace DamageMeterRebuilt.Domain;

internal sealed class EncounterSegment
{
    public required string EncounterKey { get; init; }
    public int TurnCount { get; set; }
    public Dictionary<string, PlayerStats> Players { get; } = new(StringComparer.Ordinal);
    public List<TimelineEvent> Timeline { get; } = new();
    public Dictionary<string, List<TimelineEvent>> DeathSnapshots { get; } = new(StringComparer.Ordinal);

    public EncounterSegment Clone()
    {
        var clone = new EncounterSegment
        {
            EncounterKey = EncounterKey,
            TurnCount = TurnCount
        };

        foreach (var (key, value) in Players)
        {
            clone.Players[key] = value.Clone();
        }

        clone.Timeline.AddRange(Timeline);
        foreach (var (key, value) in DeathSnapshots)
        {
            clone.DeathSnapshots[key] = value.ToList();
        }
        return clone;
    }
}
