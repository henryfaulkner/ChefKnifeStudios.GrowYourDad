using Godot;
using System;
using System.Collections.Generic;

public partial class AgentController : CharacterBody2D
{	
	[Signal]
	public delegate void WithinOneCardinalBlockFromNavTargetEventHandler();

	[ExportGroup("Nodes")]
	[Export]
	protected Area2D Area { get; set; }
	[Export]
	protected CollisionShape2D AreaCollision { get; set; }
	[Export]
	protected CollisionShape2D Collision { get; set; }

	// Good NavigationAgent2D tutorial
	// https://www.youtube.com/watch?v=Lt9YdQ6Ztm4&t=13s
	[Export]
	protected NavigationAgent2D NavAgent { get; set; }
	[Export]
	Timer NavTimer { get; set; }

	[ExportGroup("Variables")]
	[Export]
	float _movementSpeed = 0.2f;

	Node2D? _navTarget;

	ILoggerService _logger;

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);

		NavAgent.VelocityComputed += HandleVelocityComputed;
		NavTimer.Timeout += HandleNavTimeout;
	}
	
	public override void _PhysicsProcess(double _delta)
	{
		if (_navTarget == null) 
		{
			//_logger.LogInfo("AgentController _navTarget == null");
			return;
		}
		if (NavAgent.IsNavigationFinished()) 
		{
			//_logger.LogInfo("AgentController NavAgent.IsNavigationFinished()");
			return;
		}

		Vector2 nextPathPosition = NavAgent.GetNextPathPosition();
		Vector2 newVelocity = GlobalPosition.DirectionTo(nextPathPosition) * _movementSpeed;
		
		// _logger.LogInfo($"AgentController nextPathPosition {nextPathPosition.ToString()}");
		// _logger.LogInfo($"AgentController newVelocity {newVelocity.ToString()}");
		// _logger.LogInfo($"AgentController NavAgent.AvoidanceEnabled {NavAgent.AvoidanceEnabled}");
		if (NavAgent.AvoidanceEnabled)
			NavAgent.SetVelocity(newVelocity);
		else
			HandleVelocityComputed(newVelocity); 
	}

	public void SetNavTarget(Node2D? navTarget)
	{
		_navTarget = navTarget;
	}

	void HandleVelocityComputed(Vector2 safeVelocity)
	{
		_logger.LogDebug("Call NavController HandleVelocityComputed");
		Velocity = safeVelocity;
		MoveAndSlide();
	}
	
	void HandleNavTimeout()
	{
		if (_navTarget != null)
		{ 
			NavAgent.SetTargetPosition(_navTarget.GlobalPosition);
			_logger.LogDebug($"NavTarget.GlobalPosition {_navTarget.GlobalPosition}");
		}
		else
		{
			//_logger.LogInfo("NavTarget is null");
		}
	}
}
