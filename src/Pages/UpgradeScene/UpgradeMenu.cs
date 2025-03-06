using Godot;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class UpgradeMenu : MarginContainer
{
	[Export]
	UpgradeMenuOption[] UpgradeMenuOptions { get; set; } = { };
	[Export]
	RichTextLabel ItemNameLabel { get; set; } = null!;
	[Export]
	RichTextLabel ItemDescriptionLabel { get; set; } = null!;
	
	int SelectedOptionIndex { get; set; } = -1;
	List<ItemBase> ItemList { get; set; } = new();

	ILoggerService _logger;
	IPcInventoryService _pcInventoryService;

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_pcInventoryService = GetNode<IPcInventoryService>(Constants.SingletonNodes.PcInventoryService);

		ProcessMode = ProcessModeEnum.WhenPaused;

		ItemList = ItemHelper.GetRandomNItems(UpgradeMenuOptions.Count()).ToList();

		var len = UpgradeMenuOptions.Count();
		_logger.LogInfo($"len {len}");
		if (len != 0)
		{
			SelectedOptionIndex = len / 2;
			_logger.LogInfo($"selectedOptionIndex {SelectedOptionIndex}");
			UpgradeMenuOptions[SelectedOptionIndex].GrabFocus();
			MatchTextWithItem(ItemList[SelectedOptionIndex]);
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
			MatchTextWithItem(ItemList[SelectedOptionIndex]);
		}
		
		if (Input.IsActionJustPressed("right"))
		{
			if (SelectedOptionIndex == UpgradeMenuOptions.Count() - 1) SelectedOptionIndex = 0;
			else SelectedOptionIndex += 1;
			foreach (var x in UpgradeMenuOptions) x.LoseFocus();
			UpgradeMenuOptions[SelectedOptionIndex].GrabFocus();
			MatchTextWithItem(ItemList[SelectedOptionIndex]);
		}

		if (Input.IsActionJustPressed("shoot"))
		{
			_pcInventoryService.AddToInventory(ItemList[SelectedOptionIndex]);
			GetTree().Paused = false;
		}
	}

	void MatchTextWithItem(ItemBase item)
	{
		ItemNameLabel.Text = $"[center]{item.Name}[/center]";
		ItemDescriptionLabel.Text = $"[center]{item.Description}[/center]";
	}
}
