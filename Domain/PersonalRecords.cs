namespace DamageMeterRebuilt.Domain;

internal sealed class PersonalRecords
{
    public int HighestHit { get; set; }
    public string HighestHitCard { get; set; } = string.Empty;
    public int MostFightDamage { get; set; }
    public int BestTurnDamage { get; set; }
    public int MostCardsPlayed { get; set; }
    public int MostBlockGained { get; set; }
    public long TotalDamage { get; set; }
    public int TotalFights { get; set; }

    public PersonalRecords Clone()
    {
        return new PersonalRecords
        {
            HighestHit = HighestHit,
            HighestHitCard = HighestHitCard,
            MostFightDamage = MostFightDamage,
            BestTurnDamage = BestTurnDamage,
            MostCardsPlayed = MostCardsPlayed,
            MostBlockGained = MostBlockGained,
            TotalDamage = TotalDamage,
            TotalFights = TotalFights
        };
    }

    public void CopyFrom(PersonalRecords source)
    {
        HighestHit = source.HighestHit;
        HighestHitCard = source.HighestHitCard;
        MostFightDamage = source.MostFightDamage;
        BestTurnDamage = source.BestTurnDamage;
        MostCardsPlayed = source.MostCardsPlayed;
        MostBlockGained = source.MostBlockGained;
        TotalDamage = source.TotalDamage;
        TotalFights = source.TotalFights;
    }
}
