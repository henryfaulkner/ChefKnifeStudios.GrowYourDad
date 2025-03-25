using Godot;
using System;

public abstract partial class GameStateSingletonBase : Node
{
    Observables _observables = null!;

    public override void _Ready()
    {
        _observables = GetNode<Observables>(Constants.SingletonNodes.Observables);
        _observables.RestartCrawl += Clear; 
    }

    public override void _ExitTree()
    {
        _observables.RestartCrawl -= Clear;
    }

    public abstract void Clear();
} 