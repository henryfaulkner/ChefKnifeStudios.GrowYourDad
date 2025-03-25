using Godot;
using System;

public partial class TextButton : Control
{
    [Export]
    public RichTextLabel Label { get; set; } = null!;

    public Action HandleSelectCallback { get; set; } = null!;

    public void HandleFocus() {}
    public void HandleLoseFocus() {}
}