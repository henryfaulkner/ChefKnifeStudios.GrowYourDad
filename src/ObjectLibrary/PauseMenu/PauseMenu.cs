using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PauseMenu : CanvasLayer
{
	private static readonly StringName _SCENE_MAIN_MENU = new StringName("res://Main.tscn");

	#region Input Constants

	private static readonly StringName _PAUSE_INPUT = new StringName("pause");
	private static readonly StringName _UP_INPUT = new StringName("up");
	private static readonly StringName _DOWN_INPUT = new StringName("down");

	#endregion

	#region Panels

	[Export]
	private MainPanel MainPanel;

	[Export]
	private AudioSettingsPanel AudioSettingsPanel;

	[Export]
	private GameplaySettingsPanel GameplaySettingsPanel;

	[Export]
	private PlayerControlsPanel PlayerControlsPanel;

	#endregion

	#region Audio Exports

	[Export]
	private AudioStreamPlayer SelectAudio;

	[Export]
	private AudioStreamPlayer SwitchAudio;

	#endregion

	private PauseMenuService _pauseMenuService;
	public List<BaseMenuPanel> PanelList { get; set; }

	private BaseMenuPanel.OpenEventHandler MainPanelOpenHandler;
	private BaseMenuPanel.OpenEventHandler AudioSettingsPanelOpenHandler;
	private BaseMenuPanel.OpenEventHandler GameplaySettingsPanelOpenHandler;
	private BaseMenuPanel.OpenEventHandler PlayerControlsPanelOpenHandler;

	public override void _Ready()
	{
		ProcessMode = Node.ProcessModeEnum.WhenPaused;
		PanelList = GetPauseMenuPanels();
		_pauseMenuService = GetNode<PauseMenuService>("/root/PauseMenuService");
		_pauseMenuService.SetPanelList(GetPauseMenuPanels());

		_pauseMenuService.OpenMenu += HandleOpenMenu;
		_pauseMenuService.CloseMenu += HandleCloseMenu;
		SubscribeToPanelEvents();
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed(_UP_INPUT))
		{
			PanelList.ForEach(x =>
			{
				x.MoveFocusBackward(SwitchAudio);
			});
		}

		if (Input.IsActionJustPressed(_DOWN_INPUT))
		{
			PanelList.ForEach(x =>
			{
				x.MoveFocusForward(SwitchAudio);
			});
		}
	}

	public override void _ExitTree()
	{
		if (_pauseMenuService != null)
		{
			_pauseMenuService.OpenMenu -= HandleOpenMenu;
			_pauseMenuService.CloseMenu -= HandleCloseMenu;
		}

		if (MainPanelOpenHandler != null)
		{
			MainPanel.Open -= MainPanelOpenHandler;
			MainPanelOpenHandler = null;
		}

		if (AudioSettingsPanelOpenHandler != null)
		{
			AudioSettingsPanel.Open -= AudioSettingsPanelOpenHandler;
			AudioSettingsPanelOpenHandler = null;
		}

		if (GameplaySettingsPanelOpenHandler != null)
		{
			GameplaySettingsPanel.Open -= GameplaySettingsPanelOpenHandler;
			GameplaySettingsPanelOpenHandler = null;
		}

		if (PlayerControlsPanelOpenHandler != null)
		{
			PlayerControlsPanel.Open -= PlayerControlsPanelOpenHandler;
			PlayerControlsPanelOpenHandler = null;
		}
	}

	private List<BaseMenuPanel> GetPauseMenuPanels()
	{
		var result = new List<BaseMenuPanel>();
		result.Add(MainPanel);
		result.Add(AudioSettingsPanel);
		result.Add(GameplaySettingsPanel);
		result.Add(PlayerControlsPanel);
		return result;
	}

	private void SubscribeToPanelEvents()
	{
		SubscribeToMainPanelEvents();
		SubscribeToAudioSettingsPanelEvents();
		SubscribeToGameplaySettingsPanelEvents();
		SubscribeToPlayerControlsPanelEvents();
	}

	private void SubscribeToMainPanelEvents()
	{
		MainPanelOpenHandler = OpenPanel;
		MainPanel.Open += MainPanelOpenHandler;
	}

	private void SubscribeToAudioSettingsPanelEvents()
	{
		AudioSettingsPanelOpenHandler = OpenPanel;
		AudioSettingsPanel.Open += AudioSettingsPanelOpenHandler;
	}

	private void SubscribeToGameplaySettingsPanelEvents()
	{
		GameplaySettingsPanelOpenHandler = OpenPanel;
		GameplaySettingsPanel.Open += GameplaySettingsPanelOpenHandler;
	}

	private void SubscribeToPlayerControlsPanelEvents()
	{
		PlayerControlsPanelOpenHandler = OpenPanel;
		PlayerControlsPanel.Open += PlayerControlsPanelOpenHandler;
	}

	public void OpenPanel(int openPanelId)
	{
		HideAllPanels();
		var openPanel = PanelList.Where(x => (int)x.Id == openPanelId).First();
		openPanel.Visible = true;
	}

	public void HideAllPanels()
	{
		PanelList.ForEach(x =>
		{
			x.Visible = false;
		});
	}
	
	void HandleOpenMenu()
	{
		GD.Print("Open Menu");
		Visible = true;
	}
	
	void HandleCloseMenu()
	{
		GD.Print("Close Menu");
		Visible = false;
	}
}
