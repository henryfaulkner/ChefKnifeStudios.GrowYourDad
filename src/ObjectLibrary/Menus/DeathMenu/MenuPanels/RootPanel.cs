using Godot;
using System;

public partial class RootPanel : BaseMenuPanel
{
    [Export]
	Control RestartBtn { get; set; } = null!;
	[Export]
	Control GameSavesBtn { get; set; } = null!;
	[Export]
	Control ReturnToSurface { get; set; } = null!;


}