using Godot;
using System;
using System.Collections.Generic;

public partial class BaseMenuPanel : Panel
{
	public virtual Enumerations.PauseMenuPanels Id { get; } = Enumerations.PauseMenuPanels.Main;
	public List<BaseButton> Buttons { get; set; } = null!;
	public int FocusIndex { get; set; } = -1;

	[Signal]
	public delegate void OpenEventHandler(int openPanelId);

	public void MoveFocusBackward()
	{
		if (!Visible) return;
		int len = Buttons.Count;
		if (FocusIndex == 0)
		{
			Buttons[len - 1].GrabFocus();
			FocusIndex = len - 1;
		}
		else
		{
			Buttons[FocusIndex - 1].GrabFocus();
			FocusIndex -= 1;
		}
	}

	public void MoveFocusForward()
	{
		if (!Visible) return;
		int len = Buttons.Count;
		if (FocusIndex == len - 1)
		{
			Buttons[0].GrabFocus();
			FocusIndex = 0;
		}
		else
		{
			Buttons[FocusIndex + 1].GrabFocus();
			FocusIndex += 1;
		}
	}
	
	public void OpenPanel()
	{
		Visible = true;
		Buttons[0].GrabFocus();
	}
}
