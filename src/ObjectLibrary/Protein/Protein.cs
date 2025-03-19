using Godot;
using System;

public partial class Protein : RigidBody2D
{
	[Export]
	private Area2D HitBox { get; set; } = null!;

	ILoggerService _logger = null!;
	IPcWalletService _pcWalletService = null!;
	IPcPositionService _pcPositionService = null!;
	ICrawlStatsService _crawlStatsService = null!;

	const float MAGNET_SPEED = 20.0f;
	bool _isInPcMagnetArea = false; 

	public override void _Ready() 
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_pcWalletService = GetNode<IPcWalletService>(Constants.SingletonNodes.PcWalletService);
		_pcPositionService = GetNode<IPcPositionService>(Constants.SingletonNodes.PcPositionService);
		_crawlStatsService = GetNode<ICrawlStatsService>(Constants.SingletonNodes.CrawlStatsService);

		HitBox.AreaEntered += HandleAreaEntered;
		HitBox.AreaExited += HandleAreaExited;
		HitBox.BodyEntered += HandleTileHit;
	}

	public override void _ExitTree()
	{
		if (HitBox != null)
		{
			HitBox.AreaEntered -= HandleAreaEntered;
			HitBox.AreaExited -= HandleAreaExited;
			HitBox.BodyEntered -= HandleTileHit;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_isInPcMagnetArea) HandleMagnetMovement(delta);
	}

	void HandleAreaEntered(Area2D target)
	{
		switch (target)
		{
			case PcGrabArea grabArea:
				_pcWalletService.ProteinInWallet += 1;
				_crawlStatsService.CrawlStats.ProteinsCollected += 1;
				QueueFree();
				break;
			case PcMagnetArea magnetArea:
				_isInPcMagnetArea = true;
				break; 
			default:
				break;
		}
	}

	void HandleAreaExited(Area2D target)
	{
		switch (target)
		{
			case PcMagnetArea magnetArea:
				_isInPcMagnetArea = false;
				break;
			default:
				break;
		}
	}

	void HandleTileHit(Node2D target)
	{
		if (target is TileMapLayer tileMapLayer)
		{
			_logger.LogInfo("Protein hit Tile");
			QueueFree();
		}
	}

	void HandleMagnetMovement(double delta)
	{
		Vector2 normalProteinToPcDelta = (_pcPositionService.GlobalPosition-GlobalPosition).Normalized(); 
		ConstantForce = normalProteinToPcDelta * MAGNET_SPEED * 10;
		ApplyImpulse(normalProteinToPcDelta * MAGNET_SPEED, Vector2.Zero);
	}
}
