using Godot;
using System;
using System.Collections.Generic;

public partial class BaseMenuPanel : Panel
{
	[Signal]
	public delegate void OpenEventHandler(int openPanelId);

	static readonly StringName _SELECT_INPUT = new StringName("shoot");

	public virtual int Id { get; } = -1;
	public List<Control> Controls { get; set; } = null!;

	int FocusIndex { get; set; } = 0;

    public override void _PhysicsProcess(double _delta)
    {
		if (Input.IsActionJustPressed(_SELECT_INPUT))
 		{
 			HandleSelect(FocusIndex);
 		}
    }

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
		FocusIndex = 0;
		Visible = true;
		Controls[0].GrabFocus();
	}

	public virtual void HandleSelect(int selectedIndex)
	{
		GD.Print("BaseMenuPanel HandleSelect not implemented.");
	}
}
