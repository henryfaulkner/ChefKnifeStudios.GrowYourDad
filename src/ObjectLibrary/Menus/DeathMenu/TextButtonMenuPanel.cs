using Godot;
using System;
using System.Collections.Generic;

public partial class TextButtonMenuPanel : Panel, IMenuPanel
{
	[Signal]
	public delegate void OpenEventHandler(int openPanelId);

	static readonly StringName _SELECT_INPUT = new StringName("shoot");

	public virtual int Id { get; } = -1;
	public List<TextButton> Controls { get; set; } = null!;

	int FocusIndex { get; set; } = 0;

	public override void _Ready()
	{
		MouseEntered += HandleMouseEntered;
		MouseExited += HandleMouseExited;
	}

	public override void _ExitTree()
	{
		MouseEntered -= HandleMouseEntered;
		MouseExited -= HandleMouseExited;
	}

	public override void _PhysicsProcess(double _delta)
	{
		if (!Visible) return;
		if (Input.IsActionJustPressed(_SELECT_INPUT))
 		{
			if (Controls[FocusIndex]?.HandleSelectCallback == null)
				GD.Print($"Controls[FocusIndex]?.HandleSelectCallback is null at {FocusIndex}");
			Controls[FocusIndex].HandleSelectCallback.Invoke();	
 		}
	}

	public override void _GuiInput(InputEvent @event)
	{
		if (!Visible) return;
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			if (mouseEvent.ButtonIndex == MouseButton.Left)
			{
				if (FocusIndex < 0 || FocusIndex >= Controls.Count) return;
				
				Controls[FocusIndex].HandleSelectCallback.Invoke();
			}
		}
	}

	public void MoveFocusBackward()
 	{
 		if (!Visible) return;
 		int len = Controls.Count;
		
		if (FocusIndex == -1)
		{
 			Controls[len - 1].GrabFocus();
 			Controls[len - 1].HandleFocus();
 			FocusIndex = len - 1;
			return;
 		}
		
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
		
		if (FocusIndex == -1)
		{
			Controls[0].GrabFocus();
 			Controls[0].HandleFocus();
 			FocusIndex = 0;
			return;
		}
		
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

	public void MoveFocusTarget(int focusIndex)
	{
		if (!Visible) return;
		int len = Controls.Count;
		if (focusIndex >= len) return;
		if (FocusIndex < len && FocusIndex >= 0) Controls[FocusIndex].HandleLoseFocus();
		FocusIndex = focusIndex;
		if (focusIndex == -1) return;
		Controls[focusIndex].GrabFocus();
		Controls[focusIndex].HandleFocus();
	}

	void HandleMouseEntered()
	{
	}

	void HandleMouseExited()
	{
		MoveFocusTarget(-1);
	}
}
