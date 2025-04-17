using Godot;
using System;

public partial class SmootheProgressBar : ProgressBar
{
	public Action? TweenFinishedCallback;

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
		if (withTween) TweenCurrentValue(Value);
		else Value = Value;
	}

	public void UpdateValue(float value, bool withTween = true)
	{
		if (withTween) TweenCurrentValue(Math.Clamp(value, 0, MaxValue));
		else Value = Math.Clamp(value, 0, MaxValue);
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
	void TweenCurrentValue(double value)
	{
		var progressBar = this;
		var tween = progressBar.GetTree().CreateTween();
		var propTweener = tween.TweenProperty(progressBar, "value", value, 1).SetTrans(Tween.TransitionType.Linear);
		propTweener.Finished += HandleTweenFinished;
	}

	void HandleTweenFinished()
	{
		if (TweenFinishedCallback != null)
		{
			TweenFinishedCallback.Invoke();
		}
	}
}
