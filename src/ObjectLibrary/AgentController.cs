using Godot;
using System;
using System.Collections.Generic;

public partial class AgentController : CharacterBody2D
{	
	[Signal]
	public delegate void WithinOneCardinalBlockFromNavTargetEventHandler();

	[ExportGroup("Form")]
	[Export]
	protected Area2D Area { get; set; }
	[Export]
	protected CollisionShape2D AreaCollision { get; set; }
	[Export]
	protected CollisionShape2D Collision { get; set; }

	// Good NavigationAgent2D tutorial
	// https://www.youtube.com/watch?v=Lt9YdQ6Ztm4&t=13s
	[ExportGroup("Movement")]
	[Export]
	protected NavigationAgent2D NavAgent;

	Node2D _navTarget;
	const float _seekWeight = 1.0f;
	const float _avoidWeight = 1.5f;

	ILoggerService _logger;

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);

		// disable automatic behavior
		NavAgent.Velocity = Vector2.Zero;
	}
	
	public void SetNavTarget(Node2D navTarget)
	{
		_navTarget = navTarget;
	}
	
	private void HandleTimerTimeout()
	{
		if (_navTarget == null) return;

		HandlePathFinding();
		MoveAndSlide();
	}

	private void HandlePathFinding()
	{
		NavAgent.TargetPosition = _navTarget.GlobalPosition;
		var nextPathPosition = NavAgent.GetNextPathPosition();

		// Calc Context-based steering
		var seek = SeekForce(nextPathPosition);
		var avoid = AvoidanceForce();
		var steering = (seek * _seekWeight + avoid * _avoidWeight);

		Velocity = steering;
	}
	
	#region Context-based steering

	// Calculate a force to move toward the next waypoint from NavigationAgent.
	private Vector2 SeekForce(Vector2 target)
	{
		return (target - GlobalPosition).Normalized();
	}

	// Add forces to steer away from nearby obstacles or agents.
	private Vector2 AvoidanceForce()
	{
		var result = Vector2.Zero;
		var obstacles = Area.GetOverlappingBodies();
		_logger.LogInfo($"obstacle count: {obstacles.Count}");
		foreach (var obstacle in obstacles)
		{
			var direction = (obstacle.GlobalPosition - GlobalPosition).Normalized();
			_logger.LogInfo($"obstacle direction: {direction}");
			var distance = GlobalPosition.DistanceTo(obstacle.GlobalPosition);
			_logger.LogInfo($"obstacle distance: {distance}");
			if (distance == 0) continue;

			// Stronger avoidance for closer obstacles
			var avoidStrength = 100f;
			result -= direction * (1.0f / distance) * avoidStrength;
		}
		return result;
	}

	#endregion 
}
