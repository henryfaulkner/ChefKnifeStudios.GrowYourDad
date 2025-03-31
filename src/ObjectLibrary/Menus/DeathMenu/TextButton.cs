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
		
		//if (ForegroundLabel.HasThemeColorOverride("font_outline_color"))
		//{
			//GD.Print("HandleFocus Has outline color ovveride");
			//GD.Print(ForegroundLabel.GetThemeColor("font_outline_color"));
			//ForegroundLabel.RemoveThemeColorOverride("font_outline_color");
		//}
		//ForegroundLabel.AddThemeColorOverride("font_outline_color", new Color(0, 0, 1, 1));
		//ForegroundLabel.QueueRedraw();
	}
	
	public void HandleLoseFocus() 
	{
		BackgroundLabel.Visible = true;
		
		//if (ForegroundLabel.HasThemeColorOverride("font_outline_color"))
		//{
			//GD.Print("HandleLoseFocus Has outline color ovveride");
			//GD.Print(ForegroundLabel.GetThemeColor("font_outline_color"));
			//ForegroundLabel.RemoveThemeColorOverride("font_outline_color");
		//}
		//ForegroundLabel.AddThemeColorOverride("font_outline_color", new Color(0, 0, 0, 1));
		//ForegroundLabel.QueueRedraw();
	}

	void HandleMouseEnter()
	{
		EmitSignal(SignalName.RequestFocus);
	}
	
	void HandleMouseExited()
	{
	}
}
