using Godot;
using System;

public class CliffTileSetCoords
{
	public Vector2I TopLeft { get; set; } = new Vector2I(0, 0); 
	public Vector2I TopCenter { get; set; } = new Vector2I(1, 0); 
	public Vector2I TopRight { get; set; } = new Vector2I(2, 0); 
	public Vector2I CenterLeft { get; set; } = new Vector2I(0, 1); 
	public Vector2I CenterCenter { get; set; } = new Vector2I(1, 1); 
	public Vector2I CenterRight { get; set; } = new Vector2I(2, 1); 
	public Vector2I BottomLeft { get; set; } = new Vector2I(0, 2); 
	public Vector2I BottomCenter { get; set; } = new Vector2I(1, 2); 
	public Vector2I BottomRight { get; set; } = new Vector2I(2, 2); 
}