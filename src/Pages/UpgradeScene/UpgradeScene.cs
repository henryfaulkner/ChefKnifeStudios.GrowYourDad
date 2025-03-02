using Godot;
using System;

public partial class UpgradeScene : CanvasLayer
{
	[Export]
	Area2D FreezeArea { get; set; }

	public override void _Ready()
	{
		FreezeArea.AreaEntered += HandleFreezeAreaEntered;
	}

	void HandleFreezeAreaEntered(Area2D target)
	{
		GetTree().Paused = true;
	}
}
