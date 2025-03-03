using System;
using System.Collections.Generic;
using Godot;

public partial class GameplaySettingsPanel : BaseMenuPanel
{
	[Export]
	private BaseButton PlayerControlsBtn;

	[Export]
	private BaseButton BackBtn;

	public Enumerations.PauseMenuPanels Id => Enumerations.PauseMenuPanels.GameplaySettings;
	private PauseMenuService PauseMenuService;

	public override void _Ready()
	{
		FocusIndex = 0;
		Buttons = new List<BaseButton>();
		Buttons.Add(PlayerControlsBtn);
		Buttons.Add(BackBtn);
		SubscribeToButtonEvents();
		PauseMenuService = GetNode<PauseMenuService>("/root/PauseMenuService");
	}

	public override void _ExitTree()
	{
		if (PlayerControlsBtn != null)
		{
			PlayerControlsBtn.Pressed -= HandlePlayerControls;
		}

		if (BackBtn != null)
		{
			BackBtn.Pressed -= HandleBack;
		}
	}

	private void SubscribeToButtonEvents()
	{
		PlayerControlsBtn.Pressed += HandlePlayerControls;
		BackBtn.Pressed += HandleBack;
	}

	private void HandlePlayerControls()
	{
		PauseMenuService.PushPanel(this);
		EmitSignal(SignalName.Open, (int)Enumerations.PauseMenuPanels.PlayerControls);
	}

	private void HandleBack()
	{
		var resultPanel = PauseMenuService.PopPanel();
		EmitSignal(SignalName.Open, (int)resultPanel.Id);
	}
}

