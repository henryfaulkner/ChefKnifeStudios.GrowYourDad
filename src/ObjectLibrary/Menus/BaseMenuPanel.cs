using Godot;
using System;
using System.Collections.Generic;

public partial class BaseMenuPanel : Panel
{
	public virtual Enumerations.PauseMenuPanels Id { get; } = Enumerations.PauseMenuPanels.Main;
	public List<Control> Controls { get; set; } = null!;
	public int FocusIndex { get; set; } = 0;

	[Signal]
	public delegate void OpenEventHandler(int openPanelId);

	public void MoveFocusBackward()
 	{
 		if (!Visible) return;
 		int len = Controls.Count;
 		if (FocusIndex == 0)
 		{
 			Controls[len - 1].GrabFocus();
 			FocusIndex = len - 1;
 		}
 		else
 		{
 			Controls[FocusIndex - 1].GrabFocus();
 			FocusIndex -= 1;
 		}
 	}
 
 	public void MoveFocusForward()
 	{
 		if (!Visible) return;
 		int len = Controls.Count;
 		if (FocusIndex == len - 1)
 		{
 			Controls[0].GrabFocus();
 			FocusIndex = 0;
 		}
 		else
 		{
 			Controls[FocusIndex + 1].GrabFocus();
 			FocusIndex += 1;
 		}
 	}
	
	public void OpenPanel()
	{
		Visible = true;
		Controls[0].GrabFocus();
	}
}
