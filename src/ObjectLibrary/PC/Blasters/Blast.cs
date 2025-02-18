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
	}

	void HandleBlastHit(Area2D target)
	{
		if (target is BlasterTargetArea2D blasterTargetArea2D)
		{
			blasterTargetArea2D.EmitSignalBlasterTargetHit();
			QueueFree();
		} 
	}
}
