using Godot;
using System;

public partial class PathFindingFish : Agent
{
	ILoggerService _logger;

	#region State Machine
	States _state;
	enum States 
	{
		Searching,
		Approaching,
	}
	#endregion

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);

		ReadyAgent();

		_state = States.Searching;

		// Draw a circle of raycasts around the origin of the fish
		// Use as detection mechanism 
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_state == States.Searching)
		{
			// Check if raycast detect PC
			// If raycasts detects PC, target PC with NavAgent and set state to Approaching
		}

		if (_state == States.Approaching)
		{
			// Check if raycast detect PC
			// If raycasts do not detect PC, remove target from NavAgent and set state to Searching
		}
	}

	public override void HandleNavTargetArrival()
	{
	}
}
