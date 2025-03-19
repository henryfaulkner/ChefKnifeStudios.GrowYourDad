using Godot;
using System;

public interface ICrawlStatsService
{
	CrawlDepth CrawlDepth { get; }
} 

public partial class CrawlStatsService : Node, ICrawlStatsService
{
	CrawlDepth _crawlDepth = new();

	public CrawlDepth CrawlDepth 
	{
		get => _crawlDepth;
		set => _crawlDepth = value;
	}
}
