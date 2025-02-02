using Godot;
using System;

public partial class PC : Node2D
{
	[ExportGroup("Nodes")]
	[Export]
	CharacterBody2D _controller { get; set; }
	[Export]
	Node2D _blastSpawner { get; set; }
	
	ILoggerService _logger;
	
	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
	}

	public override void _PhysicsProcess(double delta)
	{
		SyncChildPositionsToController();
	}

	void SyncChildPositionsToController()
	{
		Vector2 position = _controller.GlobalPosition;
		_blastSpawner.GlobalPosition = position;
	}
}
