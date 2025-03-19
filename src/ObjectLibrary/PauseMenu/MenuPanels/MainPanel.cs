using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Godot;

public partial class MainPanel : BaseMenuPanel
{
	[Export]
	BaseButton ResumeBtn { get; set; } = null!;
	[Export]
	BaseButton ShopKeeperBtn { get; set; } = null!;
	[Export]
	BaseButton GameSaveBtn { get; set; } = null!;
	[Export]
	BaseButton MainMenuBtn { get; set; } = null!;
	
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

		Controls = new List<Control>();
		Controls.Add(ResumeBtn);
		Controls.Add(ShopKeeperBtn);
		Controls.Add(GameSaveBtn);
		Controls.Add(MainMenuBtn);
		
		ResumeBtn.Pressed += HandleResumeBtnClick;
		ShopKeeperBtn.Pressed += HandleShopKeeperBtnClick;
		GameSaveBtn.Pressed += HandleGameSaveBtnClick;
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
		
		if (GameSaveBtn != null)
		{
			GameSaveBtn.Pressed -= HandleGameSaveBtnClick;
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
	
	void HandleGameSaveBtnClick()
	{
		_pauseMenuService.PushPanel(this);
		EmitSignal(SignalName.Open, (int)Enumerations.PauseMenuPanels.GameSave);
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
