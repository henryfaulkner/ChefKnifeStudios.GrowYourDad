using Godot;
using System;
using System.Collections.Generic;

public partial class Boots : Node2D
{
	[Export]
	public BootsHitBox[]? HitBoxes { get; set; }
}
