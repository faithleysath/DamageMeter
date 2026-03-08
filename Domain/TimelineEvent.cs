namespace DamageMeterRebuilt.Domain;

internal sealed record TimelineEvent(
    TimelineEventType Type,
    int Turn,
    string ActorKey,
    string Label,
    int Value);
