using Godot;
using System;
using System.Collections.Generic;

public partial class BaseMenuPanel : Panel
{
	public virtual Enumerations.PauseMenuPanels Id { get; } = Enumerations.PauseMenuPanels.Main;
	public List<Control> Controls { get; set; } = null!;

	[Signal]
	public delegate void OpenEventHandler(int openPanelId);
	
	public void OpenPanel()
	{
		Visible = true;
		Controls[0].GrabFocus();
	}
}
