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

	public override void _ExitTree()
	{
		AreaEntered -= HandleAreaEntered;
		BodyEntered -= HandleBodyEntered;
	}

	void HandleAreaEntered(Area2D target)
	{
		if (target is EnemyHurtBoxArea targetArea2D)
		{
			targetArea2D.EmitSignalAreaHurt(Enumerations.PcAreas.Boots);
			_observables.EmitBootsBounce();
		}
	}

	void HandleBodyEntered(Node2D target)
	{
		// do nothing
	}
} 
