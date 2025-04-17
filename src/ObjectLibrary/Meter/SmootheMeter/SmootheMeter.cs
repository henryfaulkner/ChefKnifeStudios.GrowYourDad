using Godot;
using System;

public partial class SmootheMeter : MarginContainer
{
	[Export]
	TextureRect Icon { get; set; } = null!;

	[Export]
	Texture2D IconTexture { get; set; } = null!;

	[Export]
	SmootheProgressBar ProgressBar { get; set; } = null!;

	[Export]
	Label LeftLabel { get; set; } = null!;

	[Export]
	Label RightLabel { get; set; } = null!;

	ILoggerService _logger = null!;

	public Action? TweenFinishedCallback;

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		if (Icon != null && IconTexture != null) Icon.Texture = IconTexture;
		ProgressBar.TweenFinishedCallback = HandleTweenFinshed;	
	}

	public void UpdateMaxAndValueLabels(int max, int value)
	{
		RightLabel.Text = $"{value}/{max}";
	}

	public void UpdateMaxAndValue(int max, int value, bool withTween = true, bool updateLabels = true)
	{
		ProgressBar.UpdateMax(max);
		ProgressBar.UpdateValue(value, withTween: withTween);
		if (updateLabels) RightLabel.Text = $"{value}/{max}";
	}

	public void UpdateMax(int max)
	{
		ProgressBar.UpdateMax(max);
		RightLabel.Text = $"{ProgressBar.Value}/{max}";
	}

	public void UpdateValue(int value, bool withTween = true)
	{
		ProgressBar.UpdateValue(value, withTween: withTween);
		RightLabel.Text = $"{value}/{ProgressBar.MaxValue}";
	}
	
	public void SetLeftLabel(string text)
	{
		LeftLabel.Text = text;
	}

	void HandleTweenFinshed()
	{
		if (TweenFinishedCallback != null)
		{
			TweenFinishedCallback.Invoke();
		}
	}
}
