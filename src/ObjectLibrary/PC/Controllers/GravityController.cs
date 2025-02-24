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
	
	public override void _Ready() 
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_observables = GetNode<Observables>(Constants.SingletonNodes.Observables);

		_observables.BootsBounce += HandleBootsBounce;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += (GetGravity() * _gravityRatio) * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("shoot"))
		{
			velocity.Y = _shotVelocity * _gravityRatio;
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

		Velocity = velocity;
		MoveAndSlide();
	}

	void HandleBootsBounce()
	{
		Vector2 velocity = Velocity;
		velocity.Y = _shotVelocity * _gravityRatio;
		Velocity = velocity;
	}
}
