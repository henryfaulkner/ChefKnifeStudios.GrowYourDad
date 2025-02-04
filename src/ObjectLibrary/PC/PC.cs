using Godot;
using Microsoft.EntityFrameworkCore.Update;
using System;

public partial class PC : Node2D
{
	[ExportGroup("Nodes")]
	[Export]
	CharacterBody2D _controller { get; set; }
	
	BaseBlaster _blaster { get; set; }
	
	ILoggerService _logger;
	IBlasterFactory _blasterFactory;
	
	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_blasterFactory = GetNode<IBlasterFactory>(Constants.SingletonNodes.BlasterFactory);

		_blaster = CreateBlaster(_blasterFactory, DecideBlasterType());
	}

	public override void _PhysicsProcess(double delta)
	{
		SyncChildPositionsToController();
		
		if (Input.IsActionJustPressed("shoot"))
		{
			_blaster.Shoot();
		}
	}

	void SyncChildPositionsToController()
	{
		Vector2 position = _controller.GlobalPosition;
		
		if (_blaster != null)
			_blaster.GlobalPosition = position;
	}

	public Enumerations.BlasterTypes DecideBlasterType()
	{
		return Enumerations.BlasterTypes.SingleShotBlaster;
	} 

	public BaseBlaster CreateBlaster(IBlasterFactory blasterFactory, Enumerations.BlasterTypes blasterType)
	{
		switch (blasterType)
		{
			case Enumerations.BlasterTypes.SingleShotBlaster:
				return _blasterFactory.SpawnSingleShotBlaster(GetNode("."), GlobalPosition);
			default:
				throw new Exception("BlasterType not found. Can not map BlasterType.");
		}
	}
}
