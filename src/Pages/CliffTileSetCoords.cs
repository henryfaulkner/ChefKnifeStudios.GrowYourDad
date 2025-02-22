using Godot;
using System;
using System.Collections.Generic;

public class CliffTileSetCoords
{
	// Walls
	public Vector2I TopLeft { get; set; } = new Vector2I(0, 0); 
	public Vector2I TopCenter { get; set; } = new Vector2I(1, 0); 
	public Vector2I TopRight { get; set; } = new Vector2I(2, 0); 
	public Vector2I CenterLeft { get; set; } = new Vector2I(0, 1); 
	public Vector2I CenterCenter { get; set; } = new Vector2I(1, 1); 
	public Vector2I CenterRight { get; set; } = new Vector2I(2, 1); 
	public Vector2I BottomLeft { get; set; } = new Vector2I(0, 2); 
	public Vector2I BottomCenter { get; set; } = new Vector2I(1, 2); 
	public Vector2I BottomRight { get; set; } = new Vector2I(2, 2); 
	
	// Platforms
	// TODO: change simple tile selection to tetris cluster selection
	public List<Vector2I> PlatformOptions = new()
	{
		new Vector2I(7, 11),
		new Vector2I(0, 12),
		new Vector2I(1, 12),
		new Vector2I(2, 12),
		new Vector2I(3, 12),
	};
	
	public Vector2I GetRandomPlatformCell()
	{
		var rand = new Random();
		return PlatformOptions[rand.Next(0, PlatformOptions.Count)];
	}
}
