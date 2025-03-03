using System;
using System.Collections.Generic;
using Godot;

public partial class AudioSettingsPanel : BaseMenuPanel
{
	[Export]
	private BaseButton FxSoundBtn;

	[Export]
	private BaseButton BackBtn;

	public Enumerations.PauseMenuPanels Id => Enumerations.PauseMenuPanels.AudioSettings;
	private PauseMenuService PauseMenuService;

	public override void _Ready()
	{
		FocusIndex = 0;
		Buttons = new List<BaseButton>();
		Buttons.Add(FxSoundBtn);
		Buttons.Add(BackBtn);
		SubscribeToButtonEvents();
		PauseMenuService = GetNode<PauseMenuService>("/root/PauseMenuService");
	}

	public override void _ExitTree()
	{
		if (FxSoundBtn != null)
		{
			FxSoundBtn.Pressed -= HandleFxSound;
		}

		if (BackBtn != null)
		{
			BackBtn.Pressed -= HandleBack;
		}
	}

	private void SubscribeToButtonEvents()
	{
		FxSoundBtn.Pressed += HandleFxSound;
		BackBtn.Pressed += HandleBack;
	}

	private void HandleFxSound()
	{
		throw new NotImplementedException();
	}

	private void HandleBack()
	{
		var resultPanel = PauseMenuService.PopPanel();
		EmitSignal(SignalName.Open, (int)resultPanel.Id);
	}
}

