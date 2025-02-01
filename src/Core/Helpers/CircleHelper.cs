using Godot;
using System;
using System.Collections.Generic;

public class CircleHelper
{
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

	public static void TranslateListOfVectors(ref List<Vector2> points, Vector2 translate)
	{
		for (int i = 0; i < points.Count; i += 1)
		{
			points[i] += translate;
		}
	}
}