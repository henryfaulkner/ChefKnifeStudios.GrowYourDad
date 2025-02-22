using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class LevelGenerator : Node2D
{
	[ExportGroup("Nodes")]
	[Export]
	TileMapLayer TileMapLayer = null!;
	[Export]
	NoiseTexture2D _noiseHeightText = null!;

	const int WIDTH = 50;
	const int HEIGHT = 200;
	const int CLIFF_TILE_SET_SOURCE_ID = 1;

	IEnemyFactory _enemyFactory = null!;
	ILoggerService _logger = null!;

	// List to store spawned obsticles
	List<Node2D> _spawnedObsticles = new();
	Noise _noise = null!;

	public override void _Ready()
	{
		_enemyFactory = GetNode<IEnemyFactory>(Constants.SingletonNodes.EnemyFactory);
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);

		_noise = _noiseHeightText.Noise;
		GenerateLevel();
	}

	void GenerateLevel()
	{
		var lowestWidth = -WIDTH/2;
		var highestWidth = WIDTH/2;

		var lowestHeight = 0;
		var highestHeight = HEIGHT;

		List<float> noiseValueList = new();

		for (int x = lowestWidth; x < highestWidth; x += 1) 
		{
			for (int y = lowestHeight; y < highestHeight; y += 1)
			{
				var noiseVal = _noise.GetNoise2D(x, y);
				noiseValueList.Add(noiseVal);
			}
		}

		_logger.LogInfo($"Highest: {noiseValueList.Max()}");
		_logger.LogInfo($"Lowest: {noiseValueList.Min()}");

		var cliffTileCoords = new CliffTileSetCoords();
		for (int x = lowestWidth; x < highestWidth; x += 1) 
		{
			for (int y = lowestHeight; y < highestHeight; y += 1)
			{  
				Vector2I? atlasCoord = null;
				
				// Generate right and left wall tiles at borders of the width and all the way down the height
				if (x == lowestWidth || x == highestWidth - 3)
				{
					// Left
					if (y == lowestHeight)
					{
						// Top
						atlasCoord = cliffTileCoords.TopLeft;
					}
					else if (y == highestHeight - 1)
					{
						// Bottom
						atlasCoord = cliffTileCoords.BottomLeft;
					}
					else 
					{
						// Center
						atlasCoord = cliffTileCoords.CenterLeft;
					}
				}
				else if (x == lowestWidth + 1 || x == highestWidth -2)
				{
					// Center
					if (y == lowestHeight)
					{
						// Top
						atlasCoord = cliffTileCoords.TopCenter;
					}
					else if (y == highestHeight - 1)
					{
						// Bottom
						atlasCoord = cliffTileCoords.BottomCenter;
					}
					else 
					{
						// Center
						atlasCoord = cliffTileCoords.CenterCenter;
					}
				}
				else if (x == lowestWidth + 2 || x == highestWidth - 1)
				{
					// Right
					if (y == lowestHeight)
					{
						// Top
						atlasCoord = cliffTileCoords.TopRight;
					}
					else if (y == highestHeight - 1)
					{
						// Bottom
						atlasCoord = cliffTileCoords.BottomRight;
					}
					else 
					{
						// Center
						atlasCoord = cliffTileCoords.CenterRight;
					}
				} 
				else if (y == highestHeight - 3)
				{
					atlasCoord = cliffTileCoords.TopCenter;
				}
				else if (y == highestHeight - 2)
				{
					atlasCoord = cliffTileCoords.CenterCenter;
				}
				else if (y == highestHeight - 1)
				{
					atlasCoord = cliffTileCoords.BottomCenter;
				}

				if (atlasCoord.HasValue)
				{
					// Set Tile Cell
					TileMapLayer.SetCell(new Vector2I(x, y), CLIFF_TILE_SET_SOURCE_ID, atlasCoord);
				}
			}
		}
				
		for (int x = lowestWidth; x < highestWidth; x += 1) 
		{
			for (int y = lowestHeight; y < highestHeight; y += 1)
			{  
				// Generate Platforms tiles
				// Good video for procedural generation: https://www.youtube.com/watch?v=rlUzizExe2Q&t=356s and https://www.youtube.com/watch?v=dDihRqJZ_-M
				
			}
		}
				
		for (int x = lowestWidth; x < highestWidth; x += 1) 
		{
			for (int y = lowestHeight; y < highestHeight; y += 1)
			{  
				// Generate Enemies nodes

			}
		}
	}

	// Recursive function to find a valid position for a tile of specific size
	Vector2 FindValidPosition(Vector2 position, Vector2 size)
	{
		foreach (Node2D obsticle in _spawnedObsticles)
		{
			// Get the size of the existing tile
			Vector2 existingTileSize = GetObsticleSize(obsticle); // Define this to retrieve size of existing tiles

			if (CheckOverlap(position, size, obsticle.Position, existingTileSize))
			{
				// Adjust position (e.g., move right by the current tile's width)
				position += new Vector2(size.X, 0);
				// Call recursively to check the new position
				return FindValidPosition(position, size);
			}
		}
		return position; // No overlap detected, return the position
	}

	// Check overlap between two tiles of differing sizes
	bool CheckOverlap(Vector2 pos1, Vector2 size1, Vector2 pos2, Vector2 size2)
	{
		Rect2 rect1 = new Rect2(pos1, size1);
		Rect2 rect2 = new Rect2(pos2, size2);
		return rect1.Intersects(rect2);
	}

	// Helper to get tile size; customize this for your game
	static Vector2 GetObsticleSize(Node2D tile)
	{
		// Example: Assume tile has a CollisionShape2D with a RectangleShape2D
		CollisionShape2D collisionShape = tile.GetNode<CollisionShape2D>("CollisionShape2D");
		RectangleShape2D rectShape = (RectangleShape2D)collisionShape.Shape;
		return rectShape.Size; 
	}
}
