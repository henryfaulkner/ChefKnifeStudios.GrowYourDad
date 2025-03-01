using Godot;
using System;
using System.Collections.Generic;

public partial class BaseMenuPanel : Panel
{
	public Enumerations.PauseMenuPanels Id { get; }
	public List<BaseButton> Buttons { get; set; }
	public int FocusIndex { get; set; }

	[Signal]
	public delegate void OpenEventHandler(int openPanelId);

	public void MoveFocusBackward(AudioStreamPlayer switchAudio)
	{
		if (!Visible) return;
		switchAudio.Play();
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

	public void MoveFocusForward(AudioStreamPlayer switchAudio)
	{
		if (!Visible) return;
		switchAudio.Play();
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
}
