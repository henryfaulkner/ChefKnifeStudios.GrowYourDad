using Godot;
using System;
using System.Collections.Generic;

public partial class CircleFish : Path2D, IBlasterTarget
{
	[ExportGroup("Nodes")]
	[Export]
	PathFollow2D _pathFollow;
	[Export]
	BlasterTargetArea2D _hitBox;
	
	[ExportGroup("Variables")]
	[Export]
	public float Speed { get; set; } = 0.2f;
	[Export]
	public float Radius { get; set; } = 125.0f;
	[Export]
	int _numPoints = 100;
	
	[Export]
	float _flashDuration = 1.0f; // Total duration of the flashing effect
	[Export]
	float _flashInterval = 0.2f; // Time between flashes

	[Export]
	int _hp = 1;

	ILoggerService _logger;
	bool _isFlashing = false;

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		
		var circlePoints = CircleHelper.GetCirclePoints(GlobalPosition, Radius, _numPoints);
		
		// Center the points
		CircleHelper.TranslateListOfVectors(ref circlePoints, -GlobalPosition);

		Curve = CreateCurve(circlePoints);

		_hitBox.BlasterTargetHit += ReactToBlastHit;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		ProcessPathFollow(_pathFollow, Speed, delta);
	}
	
	public void ReactToBlastHit()
	{
		if (!_isFlashing) StartFlashing();
		TakeDamage();
	}

	async void StartFlashing()
	{
		_isFlashing = true;
		var originalColor = Modulate;

		float elapsed = 0.0f;

		while (elapsed < _flashDuration)
		{
			Modulate = (Modulate == originalColor) ? new Color(1.0f, 0.0f, 0.0f, 1.0f) : originalColor;
			await ToSignal(GetTree().CreateTimer(_flashInterval), "timeout");
			elapsed += _flashInterval;
		}

		// Ensure color is reset to the original after flashing
		Modulate = originalColor;
		_isFlashing = false;
	}

	void TakeDamage()
	{
		_hp -= 1;
		if (_hp == 0)
		{
			QueueFree();
		}
	}

	static Curve2D CreateCurve(List<Vector2> points)
	{
		var result = new Curve2D();
		for (int i = 0; i < points.Count; i += 1)
		{
			result.AddPoint(points[i]);
		}
		return result;
	}
	
	static bool ProcessPathFollow(PathFollow2D pathFollow, float speed, double delta)
	{
		var result = false;
		if (pathFollow == null)
		{
			GD.PrintErr("PathFollow2D is not assigned.");
			return result;
		}
		
		pathFollow.ProgressRatio += speed * (float)delta;
		return result;
	}
}
