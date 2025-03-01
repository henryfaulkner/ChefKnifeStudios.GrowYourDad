using System;
using System.Collections.Generic;
using Godot;

public partial class PlayerControlsPanel : BaseMenuPanel
{
	[Export]
	private BaseButton BackBtn;

	public Enumerations.PauseMenuPanels Id => Enumerations.PauseMenuPanels.PlayerControls;
	private PauseMenuService PauseMenuService;

	public override void _Ready()
	{
		FocusIndex = 0;
		Buttons = new List<BaseButton>();
		Buttons.Add(BackBtn);
		SubscribeToButtonEvents();
		PauseMenuService = GetNode<PauseMenuService>("/root/PauseMenuService");
	}

	private void SubscribeToButtonEvents()
	{
		BackBtn.Pressed += HandleBack;
	}

	private void HandleBack()
	{
		var resultPanel = PauseMenuService.PopPanel();
		EmitSignal(SignalName.Open, (int)resultPanel.Id);
	}
}
