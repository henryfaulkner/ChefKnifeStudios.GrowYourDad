using Godot;
using Microsoft.EntityFrameworkCore.Update;
using System;
using System.Linq;

public partial class PC : Node2D
{
	[ExportGroup("Nodes")]
	[Export]
	CharacterBody2D _controller = null!;
	
	BaseBlaster _blaster = null!;
	
	ILoggerService _logger;
	IBlasterFactory _blasterFactory;
	Observables _observables;
	IPcInventoryService _pcInventoryService;
	
	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_blasterFactory = GetNode<IBlasterFactory>(Constants.SingletonNodes.BlasterFactory);
		_observables = GetNode<Observables>(Constants.SingletonNodes.Observables);
		_pcInventoryService = GetNode<IPcInventoryService>(Constants.SingletonNodes.PcInventoryService);

		var blasterItem = _pcInventoryService.GetInvItemsWithBlastingEffect().FirstOrDefault();
		if (blasterItem != null)
			_blaster = CreateBlaster(_blasterFactory, blasterItem.BlasterType);
	}

	public override void _PhysicsProcess(double delta)
	{
		SyncChildPositionsToController();
		
		if (_blaster != null && Input.IsActionJustPressed("shoot") && !_controller.IsOnFloor())
		{
			_blaster.Shoot();
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (Input.IsActionJustPressed("single-shot"))
		{
			_blaster.QueueFree();
			_blaster = null;
			_blaster = CreateBlaster(_blasterFactory, Enumerations.BlasterTypes.SingleShotBlaster);
		}

		if (Input.IsActionJustPressed("triple-shot"))
		{
			_blaster.QueueFree();
			_blaster = null;
			_blaster = CreateBlaster(_blasterFactory, Enumerations.BlasterTypes.TripleShotBlaster);
		}
	}

	void SyncChildPositionsToController()
	{
		Vector2 position = _controller.GlobalPosition;
		
		if (_blaster != null)
			_blaster.GlobalPosition = position;
	}

	public BaseBlaster CreateBlaster(IBlasterFactory blasterFactory, Enumerations.BlasterTypes blasterType)
	{
		switch (blasterType)
		{
			case Enumerations.BlasterTypes.SingleShotBlaster:
				return _blasterFactory.SpawnSingleShotBlaster(GetNode("."), GlobalPosition);
			case Enumerations.BlasterTypes.TripleShotBlaster:
				return _blasterFactory.SpawnTripleShotBlaster(GetNode("."), GlobalPosition);
			default:
				throw new Exception("BlasterType not found. Can not map BlasterType.");
		}
	}
}
