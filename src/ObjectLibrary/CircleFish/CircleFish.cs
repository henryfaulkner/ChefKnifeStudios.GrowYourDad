using Godot;
using System;
using System.Collections.Generic;

public partial class CircleFish : Path2D
{
	[ExportGroup("Nodes")]
	[Export]
	PathFollow2D _pathFollow;
	
	[ExportGroup("Variables")]
	[Export]
	float _speed = 0.2f;
	[Export]
	float _radius = 125.0f;
	[Export]
	int _numPoints = 100;
	
	ILoggerService _logger;

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		
		var circlePoints = CircleHelper.GetCirclePoints(GlobalPosition, _radius, _numPoints);
		
		// Center the points
		CircleHelper.TranslateListOfVectors(ref circlePoints, -GlobalPosition);

		Curve = CreateCurve(circlePoints);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		ProcessPathFollow(_pathFollow, _speed, delta);
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
