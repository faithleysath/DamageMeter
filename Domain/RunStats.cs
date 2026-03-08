namespace DamageMeterRebuilt.Domain;

internal sealed class RunStats
{
    public Dictionary<string, PlayerStats> OverallPlayers { get; } = new(StringComparer.Ordinal);
    public PersonalRecords Records { get; } = new();
}
