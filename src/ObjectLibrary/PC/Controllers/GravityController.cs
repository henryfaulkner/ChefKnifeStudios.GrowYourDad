using Godot;
using System;

public partial class GravityController : CharacterBody2D
{
	[ExportGroup("Variables")]
	[Export]
	float _airSpeed = 400.0f;
	[Export]
	float _shotVelocity = -200.0f;
	[Export]
	float _gravityRatio = 1.00f;

	ILoggerService _logger;
	Observables _observables;
	IGameStateService _gameStateService;
	
	public override void _Ready() 
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_observables = GetNode<Observables>(Constants.SingletonNodes.Observables);
		_gameStateService = GetNode<GameStateService>(Constants.SingletonNodes.GameStateService);

		_observables.BootsBounce += HandleBootsBounce;
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

		// Handle Jump.
		if (Input.IsActionJustPressed("shoot") && (_gameStateService.SpValue > 0 || recentSpZero))
		{
			velocity.Y = _shotVelocity * _gravityRatio;
			if (_gameStateService.SpValue <= 0) recentSpZero = false;
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
			_gameStateService.SpValue = _gameStateService.SpMax;
			recentSpZero = true;
		}
	}

	void HandleBootsBounce()
	{
		Vector2 velocity = Velocity;
		velocity.Y = _shotVelocity * _gravityRatio;
		Velocity = velocity;
	}
}
