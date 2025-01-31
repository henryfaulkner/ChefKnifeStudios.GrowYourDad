using Godot;
using System;
using System.Collections.Generic;

public partial class CircleFish : Path2D
{
	[Export]
	PathFollow2D _pathFollow;
	
	float _speed = 0.2f;
	float _radius = 125.0f;
	int _numPoints = 100;
	
	ILoggerService _logger;

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		
		var circlePoints = GetCirclePoints(GlobalPosition, _radius, _numPoints);
		
		// Center the points
		TranslateListOfPoints(ref circlePoints, -GlobalPosition);

		Curve = GetCurveAlongListOfPoints(circlePoints);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		ProcessPathFollow(_pathFollow, _speed, delta);
	}

	public static List<Vector2> GetCirclePoints(Vector2 origin, float radius, int numPoints = 100)
	{
		List<Vector2> points = new List<Vector2>(numPoints); // Pre-allocate list for performance

		float angleStep = MathF.Tau / numPoints; // Tau is 2 * PI
		for (int i = 0; i < numPoints; i++)
		{
			float angle = i * angleStep;
			float x = origin.X + radius * MathF.Cos(angle);
			float y = origin.Y + radius * MathF.Sin(angle);

			points.Add(new Vector2(x, y));
		}

		return points;
	}

	static void TranslateListOfPoints(ref List<Vector2> points, Vector2 translate)
	{
		for (int i = 0; i < points.Count; i += 1)
		{
			points[i] += translate;
		}
	}

	static Curve2D GetCurveAlongListOfPoints(List<Vector2> points)
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
