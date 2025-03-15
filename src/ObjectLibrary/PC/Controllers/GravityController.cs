using Godot;
using System;
using System.Linq;
using System.Runtime;

public partial class GravityController : CharacterBody2D, IController
{
	[ExportGroup("Nodes")]
	[Export]
	public PcHurtBoxArea HurtBox { get; set; }
	
	[ExportGroup("Variables")]
	[Export]
	float _airSpeed = 400.0f;
	[Export]
	float _jumpVelocity = -350.0f;
	[Export]
	float _shotVelocity = -200.0f;
	[Export]
	float _gravityRatio = 1.00f;

	ILoggerService _logger;
	Observables _observables;
	IPcMeterService _pcMeterService;
	IPcInventoryService _pcInventoryService;
	
	public override void _Ready() 
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_observables = GetNode<Observables>(Constants.SingletonNodes.Observables);
		_pcMeterService = GetNode<PcMeterService>(Constants.SingletonNodes.PcMeterService);
		_pcInventoryService = GetNode<PcInventoryService>(Constants.SingletonNodes.PcInventoryService);

		_observables.BootsBounce += HandleBootsBounce;
		HurtBox.AreaEntered += HandleHurtBoxEntered;
	}
	
	public override void _ExitTree()
	{
		if (_observables != null)
		{
			_observables.BootsBounce -= HandleBootsBounce;
		}
		
		if (HurtBox != null)
		{
			HurtBox.AreaEntered -= HandleHurtBoxEntered;
		}
	}

	bool recentSpZero = true;
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += (GetGravity() * _gravityRatio) * (float)delta;
		}
		
		if (Input.IsActionJustPressed("shoot"))
		{
			if (IsOnFloor())
			{
				// Handle Jump.
				velocity.Y = _jumpVelocity * _gravityRatio;
			}
			else if ((_pcMeterService.SpValue > 0 || recentSpZero) && _pcInventoryService.GetInvItemsWithBlastingEffect().Any())
			{
				// Handle Blast.
				velocity.Y = _shotVelocity * _gravityRatio;
				if (_pcMeterService.SpValue <= 0) recentSpZero = false;
			}
		} 

		// Get the input direction and handle the movement/deceleration.
		Vector2 direction = Input.GetVector("left", "right", "up", "down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * _airSpeed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, _airSpeed);
		}

		if (velocity.Y >= (GetGravity() * _gravityRatio).Y)
			velocity.Y = (GetGravity() * _gravityRatio).Y;
		
		Velocity = velocity;
		MoveAndSlide();
		
		if (IsOnFloor())
		{
			_pcMeterService.SpValue = _pcMeterService.SpMax;
			recentSpZero = true;
		}
	}

	void HandleBootsBounce()
	{
		Vector2 velocity = Velocity;
		velocity.Y = _shotVelocity * _gravityRatio;
		Velocity = velocity;
	}

	void HandleHurtBoxEntered(Area2D target)
	{
		if (target is EnemyHitBoxArea targetArea2D)
		{
			targetArea2D.EmitSignalAreaHit(Enumerations.PcAreas.Body);
		}
	}
}
