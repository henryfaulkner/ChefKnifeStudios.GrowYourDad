public class PcLevel
{
    public int GameSaveId { get; set; }
    public required string GameSaveUsername { get; set; }
    public int Level { get; set; }
    public int TotalProteinBanked { get; set; }
    public int TotalProteinNeededForNextLevel { get; set; }
}