using Godot;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

public partial class DeathMenuHud : CanvasLayer
{
	[Export]
	public RichTextLabel CrawlDepthLabel { get; set; } = null!;
	[Export]
	public RichTextLabel ProteinLabel { get; set; } = null!;

	ILoggerService _logger = null!;
	IUnitOfWork _unitOfWork = null!;
	CrawlStatsService _crawlStatsService = null!;

	const string CRAWL_DEPTH_LABEL_TEXT = "[font_size=24][left]Level {0}[/left][/font_size]\n[font_size=20][i][left]{1}[/left][/i][/font_size]\n[font_size=20][i][left][color=red]{2}[/color][/left][/i][/font_size]";
	const string PROTEIN_LABEL_TEXT = "[font_size=24][right]{0} proteins[/right][/font_size]";

	CrawlStats? _mostRecentCrawlStats;

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_unitOfWork = GetNode<IUnitOfWork>(Constants.SingletonNodes.UnitOfWork);
		_crawlStatsService = GetNode<CrawlStatsService>(Constants.SingletonNodes.CrawlStatsService);
		
		_crawlStatsService.RefreshUI += HandleRefreshCrawlInfo;
	}

	bool _firstRender = true;
	public override void _PhysicsProcess(double delta)
	{
		if (_firstRender)
		{
			_mostRecentCrawlStats = _unitOfWork.CrawlStatsRepository
				.QueryScalar(dbSet => dbSet.Include(x => x.GameSave).OrderByDescending(record => record.Id).FirstOrDefault());
			
			if (CrawlDepthLabel != null && IsInstanceValid(CrawlDepthLabel)) 
			{
				CrawlDepthLabel.Text = string.Format(
					CRAWL_DEPTH_LABEL_TEXT, 
					_mostRecentCrawlStats?.CrawlDepth_ToString() ?? string.Empty, 
					_mostRecentCrawlStats?.GameSave?.Username ?? string.Empty,
					string.Empty
				);
			}

			if (ProteinLabel != null && IsInstanceValid(ProteinLabel)) 
			{
				ProteinLabel.Text = string.Format(
					PROTEIN_LABEL_TEXT, 
					_mostRecentCrawlStats?.ProteinsBanked.ToString() ?? string.Empty
				);
			}
		}
		_firstRender = false;

		base._PhysicsProcess(delta);
	}

	void HandleRefreshCrawlInfo()
	{
		if (CrawlDepthLabel == null || !IsInstanceValid(CrawlDepthLabel)) return;
		CrawlDepthLabel.Text = string.Format(
			CRAWL_DEPTH_LABEL_TEXT, 
			_mostRecentCrawlStats?.CrawlDepth_ToString() ?? string.Empty, 
			_mostRecentCrawlStats?.GameSave?.Username ?? string.Empty,
			_crawlStatsService.GameSave?.Username ?? string.Empty
		);
	}
}
