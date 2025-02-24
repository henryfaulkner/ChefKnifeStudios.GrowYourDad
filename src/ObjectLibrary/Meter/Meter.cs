using Godot;
using System;

public partial class Meter : MarginContainer
{
	[Export]
	TextureRect Icon { get; set; } = null!;

	[Export]
	Texture2D IconTexture { get; set; } = null!;

	[Export]
	ProgressBar ProgressBar { get; set; } = null!;

	[Export]
	Label ValueLabel { get; set; } = null!;

	ILoggerService _logger;
	ProgressBarBusiness _progressBarBusiness;

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		if (Icon != null && IconTexture != null) Icon.Texture = IconTexture;
		_progressBarBusiness = new ProgressBarBusiness(0, 5, ProgressBar);
	}

	public void UpdateMaxAndValue(int max, int value)
	{
		_progressBarBusiness.UpdateMax(max);
		_progressBarBusiness.UpdateValue(value);
		ValueLabel.Text = _progressBarBusiness.ToString();
	}

	public void UpdateMax(int max)
	{
		_logger.LogInfo($"Start Meter UpdateMax, {max}");
		_progressBarBusiness.UpdateMax(max);
		ValueLabel.Text = _progressBarBusiness.ToString();
		_logger.LogInfo("End Meter UpdateMax");
	}

	public void UpdateValue(int value)
	{
		_logger.LogInfo($"Start Meter UpdateValue, {value}");
		_progressBarBusiness.UpdateValue(value);
		ValueLabel.Text = _progressBarBusiness.ToString();
		_logger.LogInfo("End Meter UpdateValue");
	}
}
