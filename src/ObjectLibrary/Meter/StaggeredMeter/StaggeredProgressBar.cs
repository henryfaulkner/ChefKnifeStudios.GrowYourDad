using Godot;
using System;

public partial class StaggeredProgressBar : ProgressBar
{
	const int LINE_INTERVAL = 1;

	public bool IsAtMax()
	{
		return Value >= MaxValue;
	}

	public void AddToMeter(float amount)
	{
		if (Value + amount < 0)
		{
			Value = 0;
		}
		else if (Value + amount < MaxValue)
		{
			Value += amount;
		}
		else
		{
			Value = MaxValue;
		}

		QueueRedraw(); // Trigger redraw
	}

	public void UpdateValue(float value)
	{
		Value = Math.Clamp(value, 0, MaxValue);
		QueueRedraw();
	}

	public void UpdateMax(float max)
	{
		MaxValue = max;
		QueueRedraw();
	}

	public override void _Draw()
	{
		base._Draw();

		// Custom drawing logic for interval lines
		var barWidth = GetRect().Size.X;
		var lineSpacing = barWidth / MaxValue * LINE_INTERVAL; 

		for (int i = 0; i <= MaxValue; i += LINE_INTERVAL)
		{
			float xPos = (float)(i * barWidth / MaxValue);

			// Draw vertical lines for intervals
			DrawLine(
				new Vector2(xPos, 0),
				new Vector2(xPos, GetRect().Size.Y),
				Colors.White,
				1
			);
		}
	}

	public override string ToString()
	{
		return $"{Value}/{MaxValue}";
	}
}
