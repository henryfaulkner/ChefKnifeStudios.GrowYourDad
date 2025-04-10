using System.ComponentModel.DataAnnotations;

public class XpLevel
{
    [Key]
    public int Level { get; set; }
    public int Xp { get; set; }

    public const string SqlScript = 
        """
        drop table if exists XpLevel;

        create table XpLevel (
            level integer primary key,
            xp integer
        );

        with recursive xp(level, xp) as (
            select 1, 0
            union all
            select level + 1, cast(xp * 1.5 + 10 as integer)
            from xp
            where level + 1 <= 20
        )
        insert into XpLevel(level, xp)
        select level, xp from xp;
        """;
}