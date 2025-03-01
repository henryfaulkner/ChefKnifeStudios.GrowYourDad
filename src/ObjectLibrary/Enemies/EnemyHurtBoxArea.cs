using Godot;
using System;

public partial class EnemyHurtBoxArea : Area2D
{
	[Signal]
	public delegate void AreaHurtEventHandler(int pcArea);

	public void EmitSignalAreaHurt(Enumerations.PcAreas pcArea)
	{
		EmitSignal(SignalName.AreaHurt, (int)pcArea);
	}
}
