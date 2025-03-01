using Godot;
using System;

public partial class EnemyHitBoxArea : Area2D
{
	[Signal]
	public delegate void AreaHitEventHandler(int pcArea);

	public void EmitSignalAreaHit(Enumerations.PcAreas pcArea)
	{
		EmitSignal(SignalName.AreaHit, (int)pcArea);
	}
}
