drop view if exists PcLevel;

create view PcLevel as
with Banked as (
    select 
        ifnull(GameSaveId, -1) as GameSaveId,
        sum(ProteinsBanked) as TotalProteinsBanked
    from CrawlStats
    group by ifnull(GameSaveId, -1)
),
Leveled as (
    select 
        b.GameSaveId,
        b.TotalProteinsBanked,
        max(x.Level) as Level
    from Banked b
    join XpLevel x on x.XP <= b.TotalProteinsBanked
    group by b.GameSaveId, b.TotalProteinsBanked
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
        l.TotalProteinsBanked,
        xNext.XP - l.TotalProteinsBanked as TotalProteinNeededForNextLevel
    from Leveled l
    left join GameSaves gs on l.GameSaveId = gs.Id
    left join XpLevel xNext on xNext.Level = l.Level + 1
)
select 
    GameSaveId,
    GameSaveUsername,
    Level,
    TotalProteinsBanked,
    TotalProteinNeededForNextLevel
from Result;
