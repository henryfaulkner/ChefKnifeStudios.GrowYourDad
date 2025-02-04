using Godot;
using System;

public partial class GravityController : CharacterBody2D
{
	[ExportGroup("Variables")]
	[Export]
	float _airSpeed = 300.0f;
	[Export]
	float _shotVelocity = -400.0f;
	[Export]
	float _gravityRatio = 0.25f;

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
			velocity.Y = (_shotVelocity * _gravityRatio);
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
}
