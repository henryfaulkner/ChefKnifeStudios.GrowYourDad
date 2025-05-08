using System.Text.Json;

namespace ChefKnifeStudios.GrowYourDad.GoalDesigner.Models;

public class VotingModel
{
    public int RankingIndex { get; set; }
    public int FirstIndex { get; set; }
    public int SecondIndex { get; set; }
    public int NextIndex { get; set; }

    public required List<Goal> Goals { get; set; }

    // Hidden field for serialized goals
    public string GoalsJson { get; set; }

    // Deserialize GoalsJson into the Goals list
    public void DeserializeGoals()
    {
        if (!string.IsNullOrEmpty(GoalsJson))
        {
            Goals = JsonSerializer.Deserialize<List<Goal>>(GoalsJson) ?? new List<Goal>();
        }
    }
}
