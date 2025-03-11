using Godot;
using System;

public partial class Observables : Node
{
	[Signal]
	public delegate void BootsBounceEventHandler();
	public void EmitBootsBounce()
	{
		EmitSignal(SignalName.BootsBounce);
	}

	#region Meter Actions
	[Signal]
	public delegate void UpdateHpMeterValueEventHandler(int value);
	public void EmitUpdateHpMeterValue(int value)
	{
		EmitSignal(SignalName.UpdateHpMeterValue, value);
	}

	[Signal]
	public delegate void UpdateHpMeterMaxEventHandler(int max);
	public void EmitUpdateHpMeterMax(int max)
	{
		EmitSignal(SignalName.UpdateHpMeterMax, max);
	}

	[Signal]
	public delegate void UpdateSpMeterValueEventHandler(int value);
	public void EmitUpdateSpMeterValue(int value)
	{
		EmitSignal(SignalName.UpdateSpMeterValue, value);
	}

	[Signal]
	public delegate void UpdateSpMeterMaxEventHandler(int max);
	public void EmitUpdateSpMeterMax(int max)
	{
		EmitSignal(SignalName.UpdateSpMeterMax, max);
	}
	#endregion
} 
