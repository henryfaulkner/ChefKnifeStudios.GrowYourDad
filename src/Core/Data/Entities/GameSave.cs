using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

public partial class GameSave
{
	[Key]
	public int Id { get; set; }
	public string Username { get; set; }

	public HashSet<CrawlStats> CrawlStatsCollection { get; set; }
}
