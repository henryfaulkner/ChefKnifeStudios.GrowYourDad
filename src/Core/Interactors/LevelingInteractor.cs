using Godot;
using System;
using System.Linq;

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

public interface ILevelingInteractor
{
    PcLevelModel GetPcLevel(int gameSaveId);
    LevelRequirementModel GetLevelRequirement(int level);
}

public partial class LevelingInteractor : Node, ILevelingInteractor
{
    IUnitOfWork _unitOfWork = null!;

    int[] _xpLevelTable = [];

    public override void _Ready()
    {
        _unitOfWork = GetNode<IUnitOfWork>(Constants.SingletonNodes.UnitOfWork);

        _xpLevelTable = GenerateXpTable();
    }

    public PcLevelModel GetPcLevel(int gameSaveId)
    {
        PcLevelModel result = new();

        result.TotalProteinBanked  = _unitOfWork.CrawlStatsRepository
            .QueryScalar(q => 
                q.Where(x => x.GameSaveId == gameSaveId)
                    .Sum(x => x.ProteinsBanked)
            );
        result.Level = GetLevelByXp(result.TotalProteinBanked);

        return result;
    }

    public LevelRequirementModel GetLevelRequirement(int level)
    {
        return new()
        {
            Level = level + 1,
            TotalProteinNeededForNextLevel = _xpLevelTable[level],
        };         
    }

    int GetLevelByXp(int totalXp)
	{
		for (int i = 0; i < _xpLevelTable.Length; i += 1)
        {
            if (totalXp < _xpLevelTable[i])
                return i + 1;
        }
        return -1;
	}	

    int[] GenerateXpTable(int maxLevel = 20, int baseXP = 100, float multiplier = 1.5f)
	{
		int[] result = new int[maxLevel];  // Array to store XP required for each level

		// Generate the XP table
		for (int level = 0; level < maxLevel; level++)
		{
			if (level == 0)
			{
				result[level] = baseXP;
			}
			else
			{
				// Calculate XP for the next level based on the previous level
				result[level] = (int)(result[level - 1] * multiplier);
			}
		}

		return result;
	}
}
