using Godot;
using System;

public partial class BlasterTargetArea2D : Area2D
{
	[Signal]
	public delegate void BlasterTargetHitEventHandler();

	public void EmitSignalBlasterTargetHit()
	{
		EmitSignal(SignalName.BlasterTargetHit);
	}
}
