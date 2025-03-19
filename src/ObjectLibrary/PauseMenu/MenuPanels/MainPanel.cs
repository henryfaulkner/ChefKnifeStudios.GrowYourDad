using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Godot;

public partial class MainPanel : BaseMenuPanel
{
	[Export]
	private BaseButton ResumeBtn { get; set; } = null!;
	[Export]
	private BaseButton ShopKeeperBtn { get; set; } = null!;
	[Export]
	private BaseButton MainMenuBtn { get; set; } = null!;
	
	static readonly StringName PREACTION_LEVEL_PATH = new StringName("res://Pages/PreActionScene/PreActionScene.tscn");
	readonly PackedScene _preactionLevelScene;

	public override Enumerations.PauseMenuPanels Id => Enumerations.PauseMenuPanels.Main;
	
	PauseMenuService _pauseMenuService = null!;
	IPcInventoryService _pcInventoryService = null!;
	IPcWalletService _pcWalletService = null!;

	public MainPanel()
	{
		_preactionLevelScene = (PackedScene)ResourceLoader.Load(PREACTION_LEVEL_PATH);
	}

	public override void _Ready()
	{
		_pauseMenuService = GetNode<PauseMenuService>(Constants.SingletonNodes.PauseMenuService);
		_pcInventoryService = GetNode<IPcInventoryService>(Constants.SingletonNodes.PcInventoryService);
		_pcWalletService = GetNode<IPcWalletService>(Constants.SingletonNodes.PcWalletService);

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
		_pcInventoryService.Clear();
		_pcWalletService.ProteinInWallet = 0;

		// Use call_deferred to safely change the scene
		CallDeferred(nameof(ChangeToActionLevel));
	}

	void ChangeToActionLevel()
	{
		GetTree().ChangeSceneToPacked(_preactionLevelScene);
	}
}
