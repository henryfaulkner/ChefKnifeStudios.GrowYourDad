using Godot;
using System;

public partial class Protein : RigidBody2D
{
	[Export]
	private Area2D HitBox { get; set; } = null!;

	ILoggerService _logger;
	IPcWalletService _pcWalletService;

	public override void _Ready() 
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_pcWalletService = GetNode<IPcWalletService>(Constants.SingletonNodes.PcWalletService);

		HitBox.AreaEntered += HandlePcHit;
		HitBox.BodyEntered += HandleTileHit;
	}

	public override void _ExitTree()
	{
		if (HitBox != null)
		{
			HitBox.AreaEntered -= HandlePcHit;
			HitBox.BodyEntered -= HandleTileHit;
		}
	}

	void HandlePcHit(Area2D target)
	{
		if (target is PcHurtBoxArea targetArea2D)
		{
			_pcWalletService.ProteinInWallet += 1;
			QueueFree();
		} 
	}

	void HandleTileHit(Node2D target)
	{
		if (target is TileMapLayer tileMapLayer)
		{
			QueueFree();
		}
	}
}
