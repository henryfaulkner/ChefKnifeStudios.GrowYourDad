using Godot;
using System;

public partial class TargetArea2D : Area2D
{
	[Signal]
	public delegate void TargetHitEventHandler(int hitType);

	public void EmitSignalTargetHit(Enumerations.HitTypes hitType)
	{
		EmitSignal(SignalName.TargetHit, (int)hitType);
	}
}
