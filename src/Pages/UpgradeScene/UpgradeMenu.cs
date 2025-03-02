using Godot;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class UpgradeMenu : MarginContainer
{
	[Export]
	UpgradeMenuOption[] UpgradeMenuOptions { get; set; } = { };

	int SelectedOptionIndex { get; set; } = -1;

	LoggerService _logger;

	public override void _Ready()
	{
		_logger = GetNode<LoggerService>(Constants.SingletonNodes.LoggerService);

		ProcessMode = ProcessModeEnum.WhenPaused;

		var len = UpgradeMenuOptions.Count();
		_logger.LogInfo($"len {len}");
		if (len != 0)
		{
			SelectedOptionIndex = len / 2;
			_logger.LogInfo($"selectedOptionIndex {SelectedOptionIndex}");
			UpgradeMenuOptions[SelectedOptionIndex].GrabFocus();
		}	
	}

	public override void _Input(InputEvent @event)
	{
		if (Input.IsActionJustPressed("left"))
		{
			if (SelectedOptionIndex == 0) SelectedOptionIndex = UpgradeMenuOptions.Count() - 1;
			else SelectedOptionIndex -= 1;
			foreach (var x in UpgradeMenuOptions) x.LoseFocus();
			UpgradeMenuOptions[SelectedOptionIndex].GrabFocus();
		}
		
		if (Input.IsActionJustPressed("right"))
		{
			if (SelectedOptionIndex == UpgradeMenuOptions.Count() - 1) SelectedOptionIndex = 0;
			else SelectedOptionIndex += 1;
			foreach (var x in UpgradeMenuOptions) x.LoseFocus();
			UpgradeMenuOptions[SelectedOptionIndex].GrabFocus();
		}

		if (Input.IsActionJustPressed("shoot"))
		{
			GetTree().Paused = false;
		}
	}
}
