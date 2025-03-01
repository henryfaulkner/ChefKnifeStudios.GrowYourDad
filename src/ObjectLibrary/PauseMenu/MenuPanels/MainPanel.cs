using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Godot;

public partial class MainPanel : BaseMenuPanel
{
	[Export]
	private BaseButton ResumeBtn;

	[Export]
	private BaseButton AudioSettingsBtn;

	[Export]
	private BaseButton GameplaySettingsBtn;

	[Export]
	private BaseButton MainMenuBtn;

	public Enumerations.PauseMenuPanels Id => Enumerations.PauseMenuPanels.Main;
	private PauseMenuService _pauseMenuService;

	public override void _Ready()
	{
		FocusIndex = 0;
		Buttons = new List<BaseButton>();
		Buttons.Add(ResumeBtn);
		//Buttons.Add(AudioSettingsBtn);
		//Buttons.Add(GameplaySettingsBtn);
		//Buttons.Add(MainMenuBtn);
		SubscribeToButtonEvents();
		_pauseMenuService = GetNode<PauseMenuService>(Constants.SingletonNodes.PauseMenuService);
	}

	private void SubscribeToButtonEvents()
	{
		ResumeBtn.Pressed += HandleResume;
		AudioSettingsBtn.Pressed += HandleAudioSettings;
		GameplaySettingsBtn.Pressed += HandleGameplaySettings;
		MainMenuBtn.Pressed += HandleMainMenu;
	}

	private void HandleResume()
	{
		_pauseMenuService.EmitCloseMenu();
	}

	private void HandleAudioSettings()
	{
		_pauseMenuService.PushPanel(this);
		EmitSignal(SignalName.Open, (int)Enumerations.PauseMenuPanels.AudioSettings);
	}

	private void HandleGameplaySettings()
	{
		_pauseMenuService.PushPanel(this);
		EmitSignal(SignalName.Open, (int)Enumerations.PauseMenuPanels.GameplaySettings);
	}

	private void HandleMainMenu()
	{
		throw new NotImplementedException();
	}
}
