using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

public partial class GameSave
{
	[SetsRequiredMembers]
	public GameSave()
	{
		Username = string.Empty;
	}

	[Key]
	public int Id { get; set; }
	public required string Username { get; set; } 
	public bool IsCurrent { get; set; }

	public HashSet<CrawlStats>? CrawlStatsCollection { get; set; }
}
