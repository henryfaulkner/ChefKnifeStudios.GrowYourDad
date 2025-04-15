using Godot;
using System;

public partial class PreActionHud : CanvasLayer
{
	[Export]
	public RichTextLabel CrawlDepthLabel { get; set; } = null!;

	CrawlStatsService _crawlStatsService = null!;

	const string CRAWL_DEPTH_LABEL_TEXT = "[font_size=20][i][left]{0}[/left][/i][/font_size]";

	CrawlStats? _mostRecentCrawlStats;

	public override void _Ready()
	{
		_crawlStatsService = GetNode<CrawlStatsService>(Constants.SingletonNodes.CrawlStatsService);
		
		_crawlStatsService.RefreshUI += HandleRefreshCrawlInfo;
		HandleRefreshCrawlInfo();
	}

	void HandleRefreshCrawlInfo()
	{
		if (CrawlDepthLabel == null || !IsInstanceValid(CrawlDepthLabel)) return;
		CrawlDepthLabel.Text = string.Format(
			CRAWL_DEPTH_LABEL_TEXT, 
			_crawlStatsService.GameSave?.Username ?? string.Empty
		);
	}
}
