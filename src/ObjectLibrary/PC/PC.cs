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
	
	ILoggerService _logger = null!;
	IBlasterFactory _blasterFactory= null!;
	Observables _observables = null!;
	IPcInventoryService _pcInventoryService = null!;
	IPcMeterService _pcMeterService = null!;
	IPcPositionService _pcPositionService = null!;

	bool _isInvulnerable = false;
	const float INVULNERABLE_DURATION = 1.0f;
	const float INVULNERABLE_FLASH_INTERVAL = 0.2f;
	
	const int ENEMY_COLLISION_LAYER_LAYER_NUMBER = 2;
	const int ENEMY_COLLISION_MASK_LAYER_NUMBER = 2; 
	
	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_blasterFactory = GetNode<IBlasterFactory>(Constants.SingletonNodes.BlasterFactory);
		_observables = GetNode<Observables>(Constants.SingletonNodes.Observables);
		_pcInventoryService = GetNode<IPcInventoryService>(Constants.SingletonNodes.PcInventoryService);
		_pcMeterService = GetNode<IPcMeterService>(Constants.SingletonNodes.PcMeterService);
		_pcPositionService = GetNode<IPcPositionService>(Constants.SingletonNodes.PcPositionService);

		_observables.PcHit += HandlePcHit;

		var blasterItem = _pcInventoryService.GetInvItemsWithBlastingEffect().FirstOrDefault();
		if (blasterItem != null)
			_blaster = CreateBlaster(_blasterFactory, blasterItem.BlasterType);
	}

	public override void _ExitTree()
	{
		if (_observables != null)
		{
			_observables.PcHit -= HandlePcHit;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		SyncChildPositionsToController();
		_pcPositionService.Position = Position;
		_pcPositionService.GlobalPosition = GlobalPosition;
		
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

	BaseBlaster CreateBlaster(IBlasterFactory blasterFactory, Enumerations.BlasterTypes blasterType)
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

	void HandlePcHit(int damage)
	{
		if (!_isInvulnerable)
		{
			_pcMeterService.HpValue -= damage;
			SetInvulnerabilityTimeout();
		}
	}

	async void SetInvulnerabilityTimeout()
	{
		_isInvulnerable = true;
		ToggleEnemyCollision(shouldCollide: false);
		var ogColor = _controller.Modulate;
		var flashColor = new Color(1.0f, 0.0f, 0.0f, 1.0f); // solid red
		float timeElapsed = 0.0f;

		while (timeElapsed < INVULNERABLE_DURATION)
		{
			_controller.Modulate = (_controller.Modulate == ogColor)
				? flashColor
				: ogColor; 
			await ToSignal(GetTree().CreateTimer(INVULNERABLE_FLASH_INTERVAL), "timeout");
			timeElapsed += INVULNERABLE_FLASH_INTERVAL;
		}

		// Ensure color is reset to the original after flashing
		_controller.Modulate = ogColor;
		_isInvulnerable = false;
		ToggleEnemyCollision(shouldCollide: true);
	}

	void ToggleEnemyCollision(bool shouldCollide)
	{
		_controller.SetCollisionLayerValue(ENEMY_COLLISION_LAYER_LAYER_NUMBER, shouldCollide);
		_controller.SetCollisionMaskValue(ENEMY_COLLISION_MASK_LAYER_NUMBER, shouldCollide);
	}
}
