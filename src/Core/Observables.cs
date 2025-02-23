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
} 