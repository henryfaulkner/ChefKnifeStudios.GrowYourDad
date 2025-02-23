using Godot;
using System;

public partial class BootsHitBox : Area2D
{
	ILoggerService _logger;
	Observables _observables;

	public override void _Ready() 
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_observables = GetNode<Observables>(Constants.SingletonNodes.Observables);

		AreaEntered += HandleAreaEntered;
		BodyEntered += HandleBodyEntered;
	}

	void HandleAreaEntered(Area2D target)
	{
		if (target is TargetArea2D targetArea2D)
		{
			targetArea2D.EmitSignalTargetHit(Enumerations.HitTypes.Boots);
			_observables.EmitBootsBounce();
		}
	}

	void HandleBodyEntered(Node2D target)
	{
		// do nothing
	}
} 
