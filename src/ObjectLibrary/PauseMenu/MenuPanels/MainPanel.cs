using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Godot;

public partial class MainPanel : BaseMenuPanel
{
	[Export]
	private BaseButton ResumeBtn;
	[Export]
	private BaseButton ShopKeeperBtn;
	[Export]
	private BaseButton MainMenuBtn;
	
	static readonly StringName PREACTION_LEVEL_PATH = new StringName("res://Pages/PreActionScene/PreActionScene.tscn");
	readonly PackedScene _preactionLevelScene;

	public override Enumerations.PauseMenuPanels Id => Enumerations.PauseMenuPanels.Main;
	
	PauseMenuService _pauseMenuService;

	public MainPanel()
	{
		_preactionLevelScene = (PackedScene)ResourceLoader.Load(PREACTION_LEVEL_PATH);
	}

	public override void _Ready()
	{
		_pauseMenuService = GetNode<PauseMenuService>(Constants.SingletonNodes.PauseMenuService);

		FocusIndex = 0;
		Buttons = new List<BaseButton>();
		Buttons.Add(ResumeBtn);
		Buttons.Add(ShopKeeperBtn);
		Buttons.Add(MainMenuBtn);
		
		ResumeBtn.Pressed += HandleResumeBtnClick;
		ShopKeeperBtn.Pressed += HandleShopKeeperBtnClick;
		MainMenuBtn.Pressed += HandleMainMenuBtnClick;
	}

	public override void _ExitTree()
	{
		if (ResumeBtn != null)
		{
			ResumeBtn.Pressed -= HandleResumeBtnClick;
		}

		if (ShopKeeperBtn != null)
		{
			ShopKeeperBtn.Pressed -= HandleShopKeeperBtnClick;
		}

		if (MainMenuBtn != null)
		{
			MainMenuBtn.Pressed -= HandleMainMenuBtnClick;
		}
	}

	private void HandleResumeBtnClick()
	{
		_pauseMenuService.EmitCloseMenu();
	}

	private void HandleShopKeeperBtnClick()
	{
		_pauseMenuService.PushPanel(this);
		EmitSignal(SignalName.Open, (int)Enumerations.PauseMenuPanels.ShopKeeper);
	}

	private void HandleMainMenuBtnClick()
	{
		// Restarting the game state is gonna be a whole thing... sheesh

		// Use call_deferred to safely change the scene
		CallDeferred(nameof(ChangeToActionLevel));
	}

	void ChangeToActionLevel()
	{
		GetTree().ChangeSceneToPacked(_preactionLevelScene);
	}
}
