using Godot;
using System;

public partial class TextButton : Control
{
	[Signal]
	public delegate void RequestFocusEventHandler();
	
	[Export]
	public RichTextLabel ForegroundLabel { get; set; } = null!;
	[Export]
	public RichTextLabel BackgroundLabel { get; set; } = null!;

	public Action HandleSelectCallback { get; set; } = null!;

	public override void _Ready()
	{
		MouseEntered += HandleMouseEnter;
		MouseExited += HandleMouseExited;
	}

	public override void _ExitTree()
	{
		MouseEntered -= HandleMouseEnter;
		MouseExited -= HandleMouseExited;
	}

	public void HandleFocus() 
	{
		BackgroundLabel.Visible = false;
	}
	
	public void HandleLoseFocus() 
	{
		BackgroundLabel.Visible = true;
	}

	void HandleMouseEnter()
	{
		EmitSignal(SignalName.RequestFocus);
	}
	
	void HandleMouseExited()
	{
	}
}
