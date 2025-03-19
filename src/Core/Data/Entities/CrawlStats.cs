using System.ComponentModel.DataAnnotations;

public partial class CrawlStats
{
	[Key]
	public int Id { get; set; }
	public string? GamerInitials { get; set; } = null!;
	public int FloorNumber { get; set; } = 1;
	public int DepthOnFloor { get; set; } = 1;
	public int ProteinsCollected { get; set; } = 0;
	public int ProteinsBanked { get; set; } = 0;
	public int FoesDefeated { get; set; } = 0;
	public int ItemsCollected { get; set; } = 0;
	public int ItemsPurchased { get; set; } = 0;
}
