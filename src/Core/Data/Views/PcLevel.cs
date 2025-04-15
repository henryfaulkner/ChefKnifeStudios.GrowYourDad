public class PcLevel
{
    public int GameSaveId { get; set; }
    public required string GameSaveUsername { get; set; }
    public int Level { get; set; }
    public int TotalProteinBanked { get; set; }
    public int TotalProteinNeededForNextLevel { get; set; }
    public int TotalProteinNeededForCurrentLevel { get; set; }

    public override string ToString()
    {
        return $"PcLevel:\n" +
            $"  GameSaveId: {GameSaveId}\n" +
            $"  GameSaveUsername: {GameSaveUsername}\n" +
            $"  Level: {Level}\n" +
            $"  TotalProteinBanked: {TotalProteinBanked}\n" +
            $"  TotalProteinNeededForNextLevel: {TotalProteinNeededForNextLevel}\n" +
            $"  TotalProteinNeededForCurrentLevel: {TotalProteinNeededForCurrentLevel}";
    }

    public const string SqlScript = 
        """
        drop view if exists PcLevel;

        create view PcLevel as
        with Banked as (
            select 
                ifnull(GameSaveId, -1) as GameSaveId,
                sum(ProteinsBanked) as TotalProteinBanked
            from CrawlStats
            group by ifnull(GameSaveId, -1)
        ),
        Leveled as (
            select 
                b.GameSaveId,
                b.TotalProteinBanked,
                max(x.Level) as Level
            from Banked b
            join XpLevel x on x.XP <= b.TotalProteinBanked
            group by b.GameSaveId, b.TotalProteinBanked
        ),
        Result as (
            select 
                l.GameSaveId,
                gs.Username,
                case 
                    when l.GameSaveId = -1 or gs.Username is null then 'Anonymous'
                    else gs.Username
                end as GameSaveUsername,
                l.Level,
                l.TotalProteinBanked,
                xNext.XP as TotalProteinNeededForNextLevel,
                xCurrent.XP as TotalProteinNeededForCurrentLevel
            from Leveled l
            left join GameSaves gs on l.GameSaveId = gs.Id
            left join XpLevel xNext on xNext.Level = l.Level + 1
            left join XpLevel xCurrent on xCurrent.Level = l.Level
        )
        select 
            GameSaveId,
            GameSaveUsername,
            Level,
            TotalProteinBanked,
            TotalProteinNeededForNextLevel,
            TotalProteinNeededForCurrentLevel
        from Result;
        """;
}