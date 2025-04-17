using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public partial class CrawlStats
{
	public CrawlStats() {}

	[Key]
	public int Id { get; set; }
	public int? GameSaveId { get; set; } = null!;
	public int FloorNumber { get; set; } = 1;
	public int DepthOnFloor { get; set; } = 1;
	public int ProteinsCollected { get; set; } = 0;
	public int ProteinsBanked { get; set; } = 0;
	public int FoesDefeated { get; set; } = 0;
	public int ItemsCollected { get; set; } = 0;
	public int ItemsPurchased { get; set; } = 0;

	[ForeignKey(nameof(GameSaveId))]
	public GameSave? GameSave { get; set; }
}
