using System;
using Godot;

public class ProgressBarBusiness
{
	public ProgressBar ProgressBar { get; set; }

	public ProgressBarBusiness(
		float value,
		float maxValue,
		ProgressBar progressBar)
	{
		ProgressBar = progressBar;
		ProgressBar.MaxValue = maxValue;
		ProgressBar.Value = value;
	}

	public bool IsAtMax()
	{
		return ProgressBar.Value >= ProgressBar.MaxValue;
	}

	public void AddToMeter(float amount)
	{
		if (ProgressBar.Value + amount < 0)
		{
			ProgressBar.Value = 0;
		}
		else if (ProgressBar.Value + amount < ProgressBar.MaxValue)
		{
			ProgressBar.Value += amount;
		}
		else
		{
			ProgressBar.Value = ProgressBar.MaxValue;
		}
		TweenCurrentValue();
	}

	public void UpdateValue(float value)
	{
		ProgressBar.Value = value;
		TweenCurrentValue();
	}

	public void UpdateMax(float max)
	{
		ProgressBar.MaxValue = max;
	}

	// https://www.youtube.com/watch?v=fpBOEJXZeYs&t=5s
	private void TweenCurrentValue()
	{
		var tween = ProgressBar.GetTree().CreateTween();
		tween.TweenProperty(ProgressBar, "value", ProgressBar.Value, 1).SetTrans(Tween.TransitionType.Linear);
	}

	public string ToString()
	{
		return $"{ProgressBar.Value}/{ProgressBar.MaxValue}";
	}
}
