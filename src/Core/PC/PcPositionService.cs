using Godot;
using System;

public interface IPcPositionService
{
    Vector2 Position { get; set; }
    Vector2 GlobalPosition { get; set; }
}

public partial class PcPositionService : Node, IPcPositionService
{
    private Vector2 _position;

    public Vector2 Position 
    {
        get => _position;
        set
        {
            if (_position == value) return;
            _position = value;
        }
    }

    private Vector2 _globalPosition;

    public Vector2 GlobalPosition 
    {
        get => _globalPosition;
        set
        {
            if (_globalPosition == value) return;
            _globalPosition = value;
        }
    }
}