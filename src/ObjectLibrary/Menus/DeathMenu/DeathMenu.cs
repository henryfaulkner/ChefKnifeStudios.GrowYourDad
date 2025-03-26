using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class DeathMenu : CanvasLayer
{
	static readonly StringName _UP_INPUT = new StringName("up");
	static readonly StringName _RIGHT_INPUT = new StringName("right");
	static readonly StringName _DOWN_INPUT = new StringName("down");
	static readonly StringName _LEFT_INPUT = new StringName("left");
	
	[Export]
	public RootPanel RootPanel { get; set; } = null!;
	[Export]
	public GameSavesPanel GameSavesPanel { get; set; } = null!; 

	[Export]
	public MenuBusiness MenuBusiness { get; set; } = null!;

	List<TextButtonMenuPanel> _panelList = null!;

	public override void _Ready()
	{
		RootPanel.Init(MenuBusiness);
		GameSavesPanel.Init(MenuBusiness);

		_panelList = [RootPanel, GameSavesPanel];

		foreach (var panel in _panelList)
			panel.Open += OpenPanel;
	}

	public override void _PhysicsProcess(double delta)
 	{
 		if (Input.IsActionJustPressed(_UP_INPUT)
			|| Input.IsActionJustPressed(_LEFT_INPUT))
 		{
 			_panelList.ForEach(x =>
 			{
 				x.MoveFocusBackward();
 			});
 		}
 
 		if (Input.IsActionJustPressed(_DOWN_INPUT) 
			|| Input.IsActionJustPressed(_RIGHT_INPUT))
 		{
			_panelList.ForEach(x =>
 			{
 				x.MoveFocusForward();
 			});
 		}
	}

	public void OpenPanel(int openPanelId)
	{
		ResetAllPanels();
		var openPanel = _panelList.Where(x => x.Id == openPanelId).First();
		openPanel.OpenPanel();
	}

	void ResetAllPanels()
	{
		_panelList.ForEach(x =>
		{
			x.MoveFocusTarget(-1);
			x.Visible = false;
		});
	}
}
