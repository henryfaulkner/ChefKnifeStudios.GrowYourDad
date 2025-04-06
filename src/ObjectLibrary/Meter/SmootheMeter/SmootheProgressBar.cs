using Godot;
using System;

public partial class SmootheProgressBar : ProgressBar
{
	public bool IsAtMax()
	{
		return Value >= MaxValue;
	}

	public void AddToMeter(float amount, bool withTween = true)
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
		if (withTween) TweenCurrentValue();
	}

	public void UpdateValue(float value, bool withTween = true)
	{
		Value = Math.Clamp(value, 0, MaxValue);
		if (withTween) TweenCurrentValue();
	}

	public void UpdateMax(float max)
	{
		MaxValue = max;
	}

	public override string ToString()
	{
		return $"{Value}/{MaxValue} protein";
	}

	// https://www.youtube.com/watch?v=fpBOEJXZeYs&t=5s
	private void TweenCurrentValue()
	{
		var progressBar = this;
		var tween = progressBar.GetTree().CreateTween();
		tween.TweenProperty(this, "value", progressBar.Value, 1).SetTrans(Tween.TransitionType.Linear);
	}
}
