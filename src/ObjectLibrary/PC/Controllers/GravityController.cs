using System.Linq;
using Godot;

public partial class GravityController : CharacterBody2D, IController
{
	[ExportGroup("Nodes")]
	[Export]
	public PcHurtBoxArea HurtBox { get; set; } = null!;
	
	[ExportGroup("Variables")]
	[Export]
	float _maxSpeed = 400.0f;
	[Export]
	float _shotVelocity = -200.0f;
	[Export]
	float _gravityRatio = 1.00f;

	[Export]
	float _maxAcceleration = 600.00f;
	[Export]
	float _maxDeceleration = 2400.00f;
	[Export]
	float _maxTurnSpeed = 250.00f;
	[Export]
	float _maxAirAcceleration = 600.00f;
	[Export]
	float _maxAirDeceleration = 1200.00f;
	[Export]
	float _maxAirTurnSpeed = 250.00f;

	[Export]
	float _jumpHeight = 200.00f;
	[Export]
	float _gravMultiplyer = 1.00f;

	const float DOWNWARD_MOVEMENT_MULTIPLIER = 2.00F;

	ILoggerService _logger = null!;
	Observables _observables = null!;
	IPcMeterService _pcMeterService = null!;
	IPcInventoryService _pcInventoryService = null!;

	bool _isJumpDesired = false;
	
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
	bool _isJumping = false;
	public override void _PhysicsProcess(double delta)
	{
		_logger.LogInfo($"Before applying physics - Velocity: {Velocity}, IsOnFloor: {IsOnFloor()}");

// Log gravity
_logger.LogInfo($"Gravity: {GetGravity()}, Gravity Ratio: {_gravityRatio}");

// Log input
_logger.LogInfo($"Input Direction: {Input.GetVector("left", "right", "up", "down")}");

		
		
		Vector2 velocity = Velocity;
		_logger.LogInfo($"Initial velocity: {velocity}");

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += (GetGravity() * _gravityRatio) * (float)delta;
			_logger.LogInfo($"Applied gravity: {Velocity}");
		}
		
		_isJumping = _isJumping && Input.IsActionPressed("shoot") && !IsOnFloor();

		if (Input.IsActionJustPressed("shoot"))
		{
			_isJumpDesired = true;
			if (IsOnFloor())
			{
				_isJumping = true;
				HandleJump(ref velocity, delta);
			}
			else if ((_pcMeterService.SpValue > 0 || recentSpZero)
				&& _pcInventoryService.GetInvItemsWithBlastingEffect().Any())
			{
				_isJumping = false;
				HandleBlast(ref velocity, delta);
			}
		} 

		if (Velocity.Y > 0.01f)
		{
			_gravMultiplyer = _isJumping ? 1.00f : 3f; // jumpCutoff
		}
		else if (Velocity.Y < -0.01f)  
		{
			_gravMultiplyer = 1f; 
		}
		else 
		{
			_gravMultiplyer = DOWNWARD_MOVEMENT_MULTIPLIER; 
		}

		// Log gravity multiplier
		_logger.LogInfo($"Gravity multiplier: {_gravMultiplyer}");

		// Get the input direction and handle the movement/deceleration.
		Vector2 direction = Input.GetVector("left", "right", "up", "down");
		_logger.LogInfo($"Velocity before movement: {velocity}");
		HandleMovement(ref velocity, direction, delta);
		_logger.LogInfo($"Velocity after movement: {velocity}");

		if (velocity.Y >= (GetGravity() * _gravityRatio * _gravMultiplyer).Y)
			velocity.Y = (GetGravity() * _gravityRatio * _gravMultiplyer).Y;

		_logger.LogInfo($"Velocity before setting: {velocity}");
		Velocity = velocity;
		MoveAndSlide();

		_logger.LogInfo($"Velocity after MoveAndSlide: {Velocity}");

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
		_logger.LogInfo($"Boots bounce - Velocity: {velocity}");
		Velocity = velocity;
	}

	void HandleHurtBoxEntered(Area2D target)
	{
		if (target is EnemyHitBoxArea targetArea2D)
		{
			targetArea2D.EmitSignalAreaHit(Enumerations.PcAreas.Body);
			_logger.LogInfo($"Hurt box entered - Target: {target}");
		}
	}

	void HandleJump(ref Vector2 velocity, double delta)
	{
		_isJumpDesired = false;
		float gravityForce = 2f * GetGravity().Y * _gravityRatio * _jumpHeight;
		if (gravityForce < 0)
		{
			_logger.LogInfo($"Invalid gravityForce for jump: {gravityForce}");
			gravityForce = 0; // Prevent NaN
		}
		float jumpSpeed = Mathf.Sqrt(gravityForce);

		if (velocity.Y > 0) 
			jumpSpeed = Mathf.Max(jumpSpeed - velocity.Y, 0f);
		else if (velocity.Y < 0) 
			jumpSpeed += Mathf.Abs(Velocity.Y);

		velocity.Y += jumpSpeed;
		_logger.LogInfo($"HandleJump - Velocity: {velocity}");
	}

	void HandleBlast(ref Vector2 velocity, double delta)
	{
		velocity.Y = _shotVelocity * _gravityRatio;
		if (_pcMeterService.SpValue <= 0) recentSpZero = false;
		_logger.LogInfo($"HandleBlast - Velocity: {velocity}");
	}

	void HandleMovement(ref Vector2 velocity, Vector2 movementDir, double delta)
	{
		bool isOnGround = IsOnFloor();
		var acceleration = isOnGround ? _maxAcceleration : _maxAirAcceleration;
		var deceleration = isOnGround ? _maxDeceleration : _maxAirDeceleration;
		var turnSpeed = isOnGround ? _maxTurnSpeed : _maxAirTurnSpeed; 
		
		double speedChange;
		float xSpeed = movementDir.X * _maxSpeed;

		if (movementDir.X != 0)
		{
			if (Mathf.Sign(movementDir.X) != Mathf.Sign(Velocity.X))
			{
				speedChange = turnSpeed * delta;
			}
			else 
			{
				speedChange = acceleration * delta;
			}
		}
		else
		{
			speedChange = deceleration * delta;
		}

		velocity.X = Mathf.MoveToward(velocity.X, xSpeed, (float)speedChange);
		_logger.LogInfo($"HandleMovement - xSpeed: {xSpeed}");
		_logger.LogInfo($"HandleMovement - Velocity: {velocity}");
	}
}
