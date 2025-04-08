using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PauseMenu : CanvasLayer
{
	static readonly StringName _SCENE_MAIN_MENU = new StringName("res://Main.tscn");
	static readonly StringName _PAUSE_INPUT = new StringName("pause");
	static readonly StringName _UP_INPUT = new StringName("up");
	static readonly StringName _DOWN_INPUT = new StringName("down");

	[ExportGroup("Nodes")]
	[Export]
	MainPanel MainPanel { get; set; } = null!;
	[Export]
	ShopKeeperPanel ShopKeeperPanel { get; set; } = null!;
	[Export]
	GameSavePanel GameSavePanel { get; set; } = null!;
	[Export]
	CrawlStatsHistoryPanel CrawlStatsHistoryPanel { get; set; } = null!;
	[Export]
	OpenClosePauseMenuListener PauseListener { get; set; } = null!;
	[Export]
	public MenuBusiness MenuBusiness { get; set; } = null!;
	
	List<BaseMenuPanel> _panelList { get; set; } = null!;
	
	public override void _Ready()
	{
		ProcessMode = Node.ProcessModeEnum.WhenPaused;
		MainPanel.Init(MenuBusiness);
		ShopKeeperPanel.Init(MenuBusiness);
		GameSavePanel.Init(MenuBusiness);
		CrawlStatsHistoryPanel.Init(MenuBusiness);
		PauseListener.Init(MenuBusiness);

		_panelList = new List<BaseMenuPanel>
		{
			MainPanel,
			ShopKeeperPanel,
			GameSavePanel,
			CrawlStatsHistoryPanel,
		};

		MenuBusiness.OpenMenu += HandleOpenMenu;
		MenuBusiness.CloseMenu += HandleCloseMenu;

		MainPanel.Open += OpenPanel;
		ShopKeeperPanel.Open += OpenPanel;
		GameSavePanel.Open += OpenPanel;
		CrawlStatsHistoryPanel.Open += OpenPanel;

		MenuBusiness.EmitCloseMenu();
	}

	public override void _ExitTree()
	{
		if (MenuBusiness != null)
		{
			MenuBusiness.OpenMenu -= HandleOpenMenu;
			MenuBusiness.CloseMenu -= HandleCloseMenu;
		}

		MainPanel.Open -= OpenPanel;
		ShopKeeperPanel.Open -= OpenPanel;
		GameSavePanel.Open -= OpenPanel;
		CrawlStatsHistoryPanel.Open -= OpenPanel;
	}

	public void OpenPanel(int openPanelId)
	{
		ResetAllPanels();
		var openPanel = _panelList.Where(x => x.Id == openPanelId).First();
		openPanel.OpenPanel();
	}

	public void ResetAllPanels()
	{
		_panelList.ForEach(x =>
		{
			x.Visible = false;
		});
	}
	
	void HandleOpenMenu()
	{
		ResetAllPanels();
		Visible = true;
		MainPanel.OpenPanel();
	}
	
	void HandleCloseMenu()
	{
		Visible = false;
	}
}
