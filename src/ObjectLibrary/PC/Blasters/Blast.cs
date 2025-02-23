using Godot;
using System;

public partial class Blast : RigidBody2D
{
	[Export]
	private Area2D HitBox { get; set; } = null!;

	ILoggerService _logger;

	public override void _Ready() 
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);

		HitBox.AreaEntered += HandleBlastHit;
		HitBox.BodyEntered += HandleTileHit;
	}

	void HandleBlastHit(Area2D target)
	{
		if (target is TargetArea2D targetArea2D)
		{
			targetArea2D.EmitSignalTargetHit(Enumerations.HitTypes.Blast);
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
