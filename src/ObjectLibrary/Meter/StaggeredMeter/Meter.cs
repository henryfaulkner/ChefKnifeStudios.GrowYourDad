using Godot;
using System;

public partial class Meter : MarginContainer
{
	[Export]
	TextureRect Icon { get; set; } = null!;

	[Export]
	Texture2D IconTexture { get; set; } = null!;

	[Export]
	StaggeredProgressBar ProgressBar { get; set; } = null!;

	[Export]
	Label ValueLabel { get; set; } = null!;

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
		ValueLabel.Text = ProgressBar.ToString();
	}

	public void UpdateMax(int max)
	{
		ProgressBar.UpdateMax(max);
		ValueLabel.Text = ProgressBar.ToString();
	}

	public void UpdateValue(int value)
	{
		ProgressBar.UpdateValue(value);
		ValueLabel.Text = ProgressBar.ToString();
	}
}
