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

	PauseMenuService _pauseMenuService = null!;
	
	List<BaseMenuPanel> _panelList { get; set; } = null!;

	
	public override void _Ready()
	{
		ProcessMode = Node.ProcessModeEnum.WhenPaused;
		_panelList = GetPauseMenuPanels();
		_pauseMenuService = GetNode<PauseMenuService>("/root/PauseMenuService");
		_pauseMenuService.SetPanelList(GetPauseMenuPanels());

		_pauseMenuService.OpenMenu += HandleOpenMenu;
		_pauseMenuService.CloseMenu += HandleCloseMenu;

		MainPanel.Open += OpenPanel;
		ShopKeeperPanel.Open += OpenPanel;
		GameSavePanel.Open += OpenPanel;
	}

	public override void _ExitTree()
	{
		if (_pauseMenuService != null)
		{
			_pauseMenuService.OpenMenu -= HandleOpenMenu;
			_pauseMenuService.CloseMenu -= HandleCloseMenu;
		}

		MainPanel.Open -= OpenPanel;
		ShopKeeperPanel.Open -= OpenPanel;
		GameSavePanel.Open -= OpenPanel;
	}

	private List<BaseMenuPanel> GetPauseMenuPanels()
	{
		var result = new List<BaseMenuPanel>();
		result.Add(MainPanel);
		result.Add(ShopKeeperPanel);
		result.Add(GameSavePanel);
		return result;
	}

	public void OpenPanel(int openPanelId)
	{
		ResetAllPanels();
		var openPanel = _panelList.Where(x => (int)x.Id == openPanelId).First();
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
