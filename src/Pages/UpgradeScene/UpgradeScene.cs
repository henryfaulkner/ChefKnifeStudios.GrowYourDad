using Godot;
using System;

public partial class UpgradeScene : CanvasLayer
{
	[Export]
	Area2D FreezeArea { get; set; } = null!;
	[Export]
	Area2D DoorArea { get; set; } = null!;

	ILoggerService _logger = null!;
	ICrawlStatsService _crawlStatsService = null!;
	NavigationAuthority _navigationAuthority = null!;

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_crawlStatsService = GetNode<ICrawlStatsService>(Constants.SingletonNodes.CrawlStatsService);
		_navigationAuthority = GetNode<NavigationAuthority>(Constants.SingletonNodes.NavigationAuthority);
		
		FreezeArea.AreaEntered += HandleFreezeAreaEntered;
		DoorArea.AreaEntered += HandleDoorAreaEntered;

		_crawlStatsService.CrawlStats.IncrementCrawlDepth();
	}

	public override void _ExitTree()
	{
		if (FreezeArea != null)
		{
			FreezeArea.AreaEntered -= HandleFreezeAreaEntered;
		}

		if (DoorArea != null)
		{
			DoorArea.AreaEntered -= HandleDoorAreaEntered;
		}
	}

	void HandleFreezeAreaEntered(Area2D target)
	{
		GetTree().Paused = true;
	}

	void HandleDoorAreaEntered(Area2D target)
	{
		// Use call_deferred to safely change the scene
		_navigationAuthority.CallDeferred("ChangeToActionLevel");
	}
}
