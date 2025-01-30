using Godot;
using System;
using System.Collections.Generic;

public partial class CircleFish : Path2D
{
	[Export]
	PathFollow2D _pathFollow;
	
	float _speed = 2.0f;
	float _radius = 200.0f;
	int _numPoints = 100;

	public override void _Ready()
	{
		var circlePoints = GetCirclePoints(Position, _radius, _numPoints);

		// Place CircleFish along the circle's rim at the top
		TranslateListOfPoints(ref circlePoints, new Vector2(0, -_radius));

		Curve = GetCurveAlongListOfPoints(circlePoints);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		ProcessPathFollow(_pathFollow, _speed, delta);
	}

	public static List<Vector2> GetCirclePoints(Vector2 origin, float radius, int numPoints = 100)
	{
		List<Vector2> points = new List<Vector2>();

		for (int i = 0; i < numPoints; i++)
		{
			// Calculate the angle for this point
			float angle = 2*MathF.PI * i / numPoints;

			// Calculate the x and y coordinates for the point
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
