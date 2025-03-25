using Godot;
using System;
using System.Collections.Generic;

public partial class TextButtonMenuPanel : Panel
{
	[Signal]
	public delegate void OpenEventHandler(int openPanelId);

	static readonly StringName _SELECT_INPUT = new StringName("shoot");

	public virtual int Id { get; } = -1;
	public List<TextButton> Controls { get; set; } = null!;

	int FocusIndex { get; set; } = 0;

    public override void _PhysicsProcess(double _delta)
    {
		if (Input.IsActionJustPressed(_SELECT_INPUT))
 		{
 			Controls[FocusIndex].HandleSelectCallback.Invoke();
 		}
    }

    public void MoveFocusBackward()
 	{
 		if (!Visible) return;
 		int len = Controls.Count;
		Controls[FocusIndex].HandleLoseFocus();
 		if (FocusIndex == 0)
 		{
 			Controls[len - 1].GrabFocus();
 			Controls[len - 1].HandleFocus();
 			FocusIndex = len - 1;
 		}
 		else
 		{
 			Controls[FocusIndex - 1].GrabFocus();
 			Controls[FocusIndex - 1].HandleFocus();
 			FocusIndex -= 1;
 		}
 	}
 
 	public void MoveFocusForward()
 	{
 		if (!Visible) return;
 		int len = Controls.Count;
		Controls[FocusIndex].HandleLoseFocus();
 		if (FocusIndex == len - 1)
 		{
 			Controls[0].GrabFocus();
 			Controls[0].HandleFocus();
 			FocusIndex = 0;
 		}
 		else
 		{
 			Controls[FocusIndex + 1].GrabFocus();
 			Controls[FocusIndex + 1].HandleFocus();
 			FocusIndex += 1;
 		}
 	}
	
	public void OpenPanel()
	{
		FocusIndex = 0;
		Visible = true;
		Controls[0].GrabFocus();
		Controls[0].HandleFocus();
	}
}
