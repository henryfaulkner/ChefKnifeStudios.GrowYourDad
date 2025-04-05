using Godot;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

public class PcLevelModel
{
    public int Level { get; set; }
    public int TotalProteinBanked { get; set; }
}

public class LevelRequirementModel
{
    public int Level { get; set; }
    public int TotalProteinNeededForNextLevel { get; set; }
}

public interface IPcRepo
{
    PcLevel GetLevelData(int gameSaveId);
}

public class PcRepo : IPcRepo
{
    public PcLevel GetLevelData(int gameSaveId)
    {
        PcLevel? result = null;

        using (var dbContext = new AppDbContext())
        {
            result = dbContext.Set<PcLevel>()
                .Where(x => x.GameSaveId == gameSaveId)
                .FirstOrDefault() ?? new() { GameSaveUsername = string.Empty };
        }
        
        return result;
    }
}
