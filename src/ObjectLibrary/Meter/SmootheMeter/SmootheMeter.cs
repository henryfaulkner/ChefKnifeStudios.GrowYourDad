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

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		if (Icon != null && IconTexture != null) Icon.Texture = IconTexture;
	}

	public void UpdateMaxAndValue(int max, int value)
	{
		ProgressBar.UpdateMax(max);
		ProgressBar.UpdateValue(value);
		RightLabel.Text = ProgressBar.ToString();
	}

	public void UpdateMax(int max)
	{
		ProgressBar.UpdateMax(max);
		RightLabel.Text = ProgressBar.ToString();
	}

	public void UpdateValue(int value)
	{
		ProgressBar.UpdateValue(value);
		RightLabel.Text = ProgressBar.ToString();
	}
	
	public void SetLeftLabel(string text)
	{
		LeftLabel.Text = text;
	}
}
