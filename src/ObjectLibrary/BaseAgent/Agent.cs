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
