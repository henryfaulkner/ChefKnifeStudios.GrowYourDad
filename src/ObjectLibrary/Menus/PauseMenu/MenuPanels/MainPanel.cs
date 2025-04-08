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
	BaseButton CrawlStatsHistoryBtn { get; set; } = null!;
	[Export]
	BaseButton MainMenuBtn { get; set; } = null!;

	public override int Id => (int)Enumerations.PauseMenuPanels.Main;
	
	Observables _observables = null!;
	NavigationAuthority _navigationAuthority = null!;

	public MenuBusiness MenuBusiness { get; set; } = null!;

	public void Init(MenuBusiness menuBusiness)
	{
		MenuBusiness = menuBusiness;
	}

	public override void _Ready()
	{
		_observables = GetNode<Observables>(Constants.SingletonNodes.Observables);
		_navigationAuthority = GetNode<NavigationAuthority>(Constants.SingletonNodes.NavigationAuthority);

		Controls = [ResumeBtn, ShopKeeperBtn, GameSaveBtn, MainMenuBtn];
		
		ResumeBtn.Pressed += HandleResumeBtnClick;
		ShopKeeperBtn.Pressed += HandleShopKeeperBtnClick;
		GameSaveBtn.Pressed += HandleGameSaveBtnClick;
		CrawlStatsHistoryBtn.Pressed += HandleCrawlStatsHistoryBtnClick;
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
		
		if (CrawlStatsHistoryBtn != null)
		{
			CrawlStatsHistoryBtn.Pressed -= HandleCrawlStatsHistoryBtnClick;
		}

		if (MainMenuBtn != null)
		{
			MainMenuBtn.Pressed -= HandleMainMenuBtnClick;
		}
	}

	private void HandleResumeBtnClick()
	{
		MenuBusiness.EmitCloseMenu();
	}

	private void HandleShopKeeperBtnClick()
	{
		MenuBusiness.PushPanel(this);
		EmitSignal(SignalName.Open, (int)Enumerations.PauseMenuPanels.ShopKeeper);
	}
	
	void HandleGameSaveBtnClick()
	{
		MenuBusiness.PushPanel(this);
		EmitSignal(SignalName.Open, (int)Enumerations.PauseMenuPanels.GameSave);
	}

	void HandleCrawlStatsHistoryBtnClick()
	{
		MenuBusiness.PushPanel(this);
		EmitSignal(SignalName.Open, (int)Enumerations.PauseMenuPanels.CrawlStatsHistory);
	}

	void HandleMainMenuBtnClick()
	{
		_observables.EmitRestartCrawl();

		// Use call_deferred to safely change the scene
		_navigationAuthority.CallDeferred("ChangeToPreActionLevel");
	}
}
