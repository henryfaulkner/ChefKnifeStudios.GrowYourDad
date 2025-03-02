using Godot;
using System;

public abstract partial class Agent : Node2D, IAgent
{
	[Export]
	protected AgentController Controller;

	ILoggerService _logger;

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
	}

	public override void _ExitTree()
	{
		if (Controller != null)
		{
			Controller.WithinOneCardinalBlockFromNavTarget -= HandleNavTargetArrival;
		}
	}

	public void ReadyAgent()
	{
		Controller.WithinOneCardinalBlockFromNavTarget += HandleNavTargetArrival;
	}
	
	public void SetNavTarget(Node2D? navTarget)
	{
		Controller.SetNavTarget(navTarget);
	}

	public abstract void HandleNavTargetArrival();
}
