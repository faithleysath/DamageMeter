using MegaCrit.Sts2.Core.Entities.Cards;
using Godot;

namespace DamageMeterRebuilt.Domain;

internal sealed class PlayerStats
{
    public required string Key { get; init; }
    public required string DisplayName { get; init; }
    public string CharacterId { get; set; } = string.Empty;
    public Color CharacterColor { get; set; } = new(0.95f, 0.55f, 0.15f, 1f);

    public int DamageDealt { get; set; }
    public int DamageTaken { get; set; }
    public int BlockedByTarget { get; set; }
    public int OverkillDealt { get; set; }
    public int HitCount { get; set; }
    public int TotalBlockGained { get; set; }
    public int TotalEnergySpent { get; set; }
    public int TotalEnergyWasted { get; set; }
    public int PotionsUsed { get; set; }
    public int DebuffsApplied { get; set; }
    public int CardsPlayed { get; set; }
    public int CardsDrawn { get; set; }
    public int CardsDiscarded { get; set; }
    public int CardsExhausted { get; set; }
    public int OrbsChanneled { get; set; }
    public int CardsGenerated { get; set; }
    public int AfflictionsApplied { get; set; }
    public int SummonsCreated { get; set; }
    public int StarsModified { get; set; }
    public int MaxSingleHit { get; set; }
    public string MaxSingleHitCard { get; set; } = string.Empty;

    public Dictionary<int, int> DamagePerTurn { get; } = new();
    public Dictionary<int, int> EnergyWastedPerTurn { get; } = new();
    public Dictionary<string, int> DamageByCard { get; } = new(StringComparer.Ordinal);
    public Dictionary<string, int> DamageBySource { get; } = new(StringComparer.Ordinal);
    public Dictionary<string, int> CardPlayCount { get; } = new(StringComparer.Ordinal);
    public Dictionary<CardType, int> CardTypeCount { get; } = new();
    public Dictionary<string, int> BlockByCard { get; } = new(StringComparer.Ordinal);
    public Dictionary<string, int> EnergySpentByCard { get; } = new(StringComparer.Ordinal);
    public Dictionary<string, int> PotionUseCount { get; } = new(StringComparer.Ordinal);
    public Dictionary<string, int> PowerDebuffCount { get; } = new(StringComparer.Ordinal);
    public Dictionary<string, int> DrawCount { get; } = new(StringComparer.Ordinal);
    public Dictionary<string, int> DiscardCount { get; } = new(StringComparer.Ordinal);
    public Dictionary<string, int> ExhaustCount { get; } = new(StringComparer.Ordinal);
    public Dictionary<string, int> MonsterMoveTargets { get; } = new(StringComparer.Ordinal);
    public Dictionary<string, int> OrbCount { get; } = new(StringComparer.Ordinal);
    public Dictionary<string, int> GeneratedCardCount { get; } = new(StringComparer.Ordinal);
    public Dictionary<string, int> AfflictionCount { get; } = new(StringComparer.Ordinal);

    public PlayerStats Clone()
    {
        var clone = new PlayerStats
        {
            Key = Key,
            DisplayName = DisplayName,
            CharacterId = CharacterId,
            CharacterColor = CharacterColor,
            DamageDealt = DamageDealt,
            DamageTaken = DamageTaken,
            BlockedByTarget = BlockedByTarget,
            OverkillDealt = OverkillDealt,
            HitCount = HitCount,
            TotalBlockGained = TotalBlockGained,
            TotalEnergySpent = TotalEnergySpent,
            TotalEnergyWasted = TotalEnergyWasted,
            PotionsUsed = PotionsUsed,
            DebuffsApplied = DebuffsApplied,
            CardsPlayed = CardsPlayed,
            CardsDrawn = CardsDrawn,
            CardsDiscarded = CardsDiscarded,
            CardsExhausted = CardsExhausted,
            OrbsChanneled = OrbsChanneled,
            CardsGenerated = CardsGenerated,
            AfflictionsApplied = AfflictionsApplied,
            SummonsCreated = SummonsCreated,
            StarsModified = StarsModified,
            MaxSingleHit = MaxSingleHit,
            MaxSingleHitCard = MaxSingleHitCard
        };

        CopyMap(DamagePerTurn, clone.DamagePerTurn);
        CopyMap(EnergyWastedPerTurn, clone.EnergyWastedPerTurn);
        CopyMap(DamageByCard, clone.DamageByCard);
        CopyMap(DamageBySource, clone.DamageBySource);
        CopyMap(CardPlayCount, clone.CardPlayCount);
        CopyMap(CardTypeCount, clone.CardTypeCount);
        CopyMap(BlockByCard, clone.BlockByCard);
        CopyMap(EnergySpentByCard, clone.EnergySpentByCard);
        CopyMap(PotionUseCount, clone.PotionUseCount);
        CopyMap(PowerDebuffCount, clone.PowerDebuffCount);
        CopyMap(DrawCount, clone.DrawCount);
        CopyMap(DiscardCount, clone.DiscardCount);
        CopyMap(ExhaustCount, clone.ExhaustCount);
        CopyMap(MonsterMoveTargets, clone.MonsterMoveTargets);
        CopyMap(OrbCount, clone.OrbCount);
        CopyMap(GeneratedCardCount, clone.GeneratedCardCount);
        CopyMap(AfflictionCount, clone.AfflictionCount);

        return clone;
    }

    public void MergeFrom(PlayerStats other)
    {
        DamageDealt += other.DamageDealt;
        DamageTaken += other.DamageTaken;
        BlockedByTarget += other.BlockedByTarget;
        OverkillDealt += other.OverkillDealt;
        HitCount += other.HitCount;
        TotalBlockGained += other.TotalBlockGained;
        TotalEnergySpent += other.TotalEnergySpent;
        TotalEnergyWasted += other.TotalEnergyWasted;
        PotionsUsed += other.PotionsUsed;
        DebuffsApplied += other.DebuffsApplied;
        CardsPlayed += other.CardsPlayed;
        CardsDrawn += other.CardsDrawn;
        CardsDiscarded += other.CardsDiscarded;
        CardsExhausted += other.CardsExhausted;
        OrbsChanneled += other.OrbsChanneled;
        CardsGenerated += other.CardsGenerated;
        AfflictionsApplied += other.AfflictionsApplied;
        SummonsCreated += other.SummonsCreated;
        StarsModified += other.StarsModified;

        if (other.MaxSingleHit > MaxSingleHit)
        {
            MaxSingleHit = other.MaxSingleHit;
            MaxSingleHitCard = other.MaxSingleHitCard;
        }

        if (!string.IsNullOrWhiteSpace(other.CharacterId))
        {
            CharacterId = other.CharacterId;
        }

        if (other.CharacterColor.A > 0f)
        {
            CharacterColor = other.CharacterColor;
        }

        MergeMap(DamagePerTurn, other.DamagePerTurn);
        MergeMap(EnergyWastedPerTurn, other.EnergyWastedPerTurn);
        MergeMap(DamageByCard, other.DamageByCard);
        MergeMap(DamageBySource, other.DamageBySource);
        MergeMap(CardPlayCount, other.CardPlayCount);
        MergeMap(CardTypeCount, other.CardTypeCount);
        MergeMap(BlockByCard, other.BlockByCard);
        MergeMap(EnergySpentByCard, other.EnergySpentByCard);
        MergeMap(PotionUseCount, other.PotionUseCount);
        MergeMap(PowerDebuffCount, other.PowerDebuffCount);
        MergeMap(DrawCount, other.DrawCount);
        MergeMap(DiscardCount, other.DiscardCount);
        MergeMap(ExhaustCount, other.ExhaustCount);
        MergeMap(MonsterMoveTargets, other.MonsterMoveTargets);
        MergeMap(OrbCount, other.OrbCount);
        MergeMap(GeneratedCardCount, other.GeneratedCardCount);
        MergeMap(AfflictionCount, other.AfflictionCount);
    }

    private static void CopyMap<TKey>(Dictionary<TKey, int> source, Dictionary<TKey, int> target)
        where TKey : notnull
    {
        foreach (var (key, value) in source)
        {
            target[key] = value;
        }
    }

    private static void MergeMap<TKey>(Dictionary<TKey, int> target, Dictionary<TKey, int> source)
        where TKey : notnull
    {
        foreach (var (key, value) in source)
        {
            target[key] = target.GetValueOrDefault(key) + value;
        }
    }
}
