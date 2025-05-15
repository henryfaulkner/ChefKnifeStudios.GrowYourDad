using System.Text.Json;

namespace ChefKnifeStudios.GrowYourDad.GoalDesigner.Models;

public class GoalCreationModel
{
    public required string Message { get; set; } = string.Empty;

    // A list of goals to preserve
    public List<Goal> Goals { get; set; } = new List<Goal>();

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

