using Godot;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class LevelGenerator : Node2D
{
	[ExportGroup("Nodes")]
	[Export]
	TileMapLayer TileMapLayer = null!;
	[Export]
	NoiseTexture2D _noiseTexturePlatform = null!;
	[Export]
	NoiseTexture2D _noiseTextureEnemy = null!;
	[Export]
	NavigationRegion2D _navRegion = null!;

	const int WIDTH = 30;
	const int HEIGHT = 200;
	const int TILE_SQUARE_SIZE = 16;
	const int CLIFF_TILE_SET_SOURCE_ID = 1;

	static readonly StringName UPGRADE_LEVEL_PATH = new StringName("res://Pages/UpgradeScene/UpgradeScene.tscn");
	readonly PackedScene _upgradeLevelScene;

	IEnemyFactory _enemyFactory = null!;
	ILoggerService _logger = null!;

	// List to store spawned obsticles
	List<Node2D> _spawnedObsticles = new();
	Noise _noisePlatform = null!;
	Noise _noiseEnemy = null!;
	Area2D _doorArea = null!;

	public LevelGenerator()
	{
		_upgradeLevelScene = (PackedScene)ResourceLoader.Load(UPGRADE_LEVEL_PATH);
	}

	public override void _Ready()
	{
		_enemyFactory = GetNode<IEnemyFactory>(Constants.SingletonNodes.EnemyFactory);
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);

		_noisePlatform = _noiseTexturePlatform.Noise;
		_noiseEnemy = _noiseTextureEnemy.Noise;
		GenerateLevel();
	}

	public override void _ExitTree()
	{
		if (_doorArea != null)
		{
			_doorArea.AreaExited -= HandleDoorAreaExited;
		}
	}

	void GenerateLevel()
	{
		var lowestWidth = -WIDTH/2;
		var highestWidth = WIDTH/2;

		var lowestHeight = 0;
		var highestHeight = HEIGHT;

		List<float> noiseValuePlatformList = new();
		List<float> noiseValueEnemyList = new();

		for (int x = lowestWidth; x < highestWidth; x += 1) 
		{
			for (int y = lowestHeight; y < highestHeight; y += 1)
			{
				var noiseValPlatform = _noisePlatform.GetNoise2D(x, y);
				noiseValuePlatformList.Add(noiseValPlatform);
				noiseValueEnemyList.Add(_noiseEnemy.GetNoise2D(x, y));
			}
		}

		_logger.LogInfo($"Highest: {noiseValuePlatformList.Max()}");
		_logger.LogInfo($"Lowest: {noiseValuePlatformList.Min()}");
		_logger.LogInfo($"Highest: {noiseValueEnemyList.Max()}");
		_logger.LogInfo($"Lowest: {noiseValueEnemyList.Min()}");

		GenerateWalls(lowestWidth, highestWidth, lowestHeight, highestHeight);
		GeneratePlatforms(lowestWidth, highestWidth, lowestHeight, highestHeight);
		GenerateEnemies(lowestWidth, highestWidth, lowestHeight, highestHeight);
		GenerateNavigationRegion(lowestWidth, highestWidth, lowestHeight, highestHeight);
	}

	void GenerateWalls(int lowestWidth, int highestWidth, int lowestHeight, int highestHeight)
	{
		var cliffTileCoords = new CliffTileSetCoords();
		int widthDelta = highestWidth - lowestWidth;

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
				else if (y == highestHeight - 2 && x == lowestWidth + (widthDelta/2))
				{
					GenerateUpgradeDoor(
						new Vector2(x*TILE_SQUARE_SIZE, y*TILE_SQUARE_SIZE), 
						new Vector2(widthDelta*TILE_SQUARE_SIZE, 3*TILE_SQUARE_SIZE)
					);
				}


				// THIS CREATED THE FLOOR OF THE ROOM DURING EARLY DEVELOPMENT
				// else if (y == highestHeight - 3)
				// {
				// 	atlasCoord = cliffTileCoords.TopCenter;
				// }
				// else if (y == highestHeight - 2)
				// {
				// 	atlasCoord = cliffTileCoords.CenterCenter;
				// }
				// else if (y == highestHeight - 1)
				// {
				// 	atlasCoord = cliffTileCoords.BottomCenter;
				// }

				if (atlasCoord.HasValue)
				{
					// Set Tile Cell
					TileMapLayer.SetCell(new Vector2I(x, y), CLIFF_TILE_SET_SOURCE_ID, atlasCoord);
				}
			}
		}
	}

	// Algorithm pseudocode:
	// Raster scan level
	// Every n and not m layers decide which side of layer to spawn platforms on: Left, Right, Left & Right, Center, or None
	// Use noise value to decide where to place tile on selected side
	// ^(replace) TODO: use noise value to decide where to place tile tetris 
	enum LayerSides 
	{
		Left, Right, LeftRight, Center, None,
	}
	LayerSides _layerSideState;

	void GeneratePlatforms(int lowestWidth, int highestWidth, int lowestHeight, int highestHeight)
	{
		var cliffTileCoords = new CliffTileSetCoords();
		int layerSideDecisionInterval = 5; // n layers
		List<int> layerSideNonDecisionIntervals = new() { 4, 6, 12 }; // not m layers
		int widthDelta = highestWidth - lowestWidth;

		for (int y = lowestHeight; y < highestHeight; y += 1)
		{ 
			if (y != lowestHeight 
				&& y % layerSideDecisionInterval == 0
				&& !layerSideNonDecisionIntervals.Where(x => y % x == 0).Any())
			{
				_layerSideState = default(LayerSides).Random();
			}

			switch (_layerSideState)
			{
				case LayerSides.Left:
					GeneratePlatformLayer(y, cliffTileCoords, lowestWidth, lowestWidth + (widthDelta/3));
					break;
				case LayerSides.Right:
					GeneratePlatformLayer(y, cliffTileCoords, highestWidth - (widthDelta/3), highestWidth);
					break;
				case LayerSides.LeftRight:
					GeneratePlatformLayer(y, cliffTileCoords, lowestWidth, lowestWidth + (widthDelta/3));
					GeneratePlatformLayer(y, cliffTileCoords, highestWidth - (widthDelta/3), highestWidth);
					break;
				case LayerSides.Center:
					GeneratePlatformLayer(y, cliffTileCoords, lowestWidth + (widthDelta/3), highestWidth - (widthDelta/3));
					break;
				case LayerSides.None:
					break;
				default:
					_logger.LogError("LevelGenerator GeneratePlatforms LayerSide did not map properly.");
					break;
			}

			
		}
	}

	void GeneratePlatformLayer(int y, CliffTileSetCoords cliffTileSetCoords, int lowestWidth, int highestWidth)
	{
		for (int x = lowestWidth; x < highestWidth; x += 1) 
		{
			var cellData = TileMapLayer.GetCellTileData(new Vector2I(x, y));
			if (cellData == null) // if cellData is NOT null, a cell already exists there
			{
				// Good video for procedural generation: https://www.youtube.com/watch?v=rlUzizExe2Q&t=356s and https://www.youtube.com/watch?v=dDihRqJZ_-M
				var noiseVal = _noisePlatform.GetNoise2D(x, y);
				if (noiseVal > 0.2 || noiseVal < -0.2) 
					TileMapLayer.SetCell(new Vector2I(x, y), CLIFF_TILE_SET_SOURCE_ID, cliffTileSetCoords.GetRandomPlatformCell());
			}
		}
	}

	enum Enemies 
	{
		CircleFish,
		//LineFish, uncomment when line draw logic is ready
		PathFindingFish,
	}

	void GenerateEnemies(int lowestWidth, int highestWidth, int lowestHeight, int highestHeight)
	{
		Random rand = new();

		float cfMinSpeed = 0.3f;
		float cfMaxSpeed = 1.2f;
		float cfMinRadius = 1.5f;
		float cfMaxRadius = 3.5f;

		for (int y = lowestHeight; y < highestHeight; y += 1)
		{ 
			for (int x = lowestWidth; x < highestWidth; x += 1) 
			{ 
				var cellData = TileMapLayer.GetCellTileData(new Vector2I(x, y));
				if (cellData == null) // if cellData is NOT null, a cell already exists there
				{
					double spawnChance = 0.002;
					//plus check for num enemies in area. Restrict to 3 max.
					if (spawnChance > rand.NextDouble())
					{
						Vector2 spawnPos = new Vector2(x*TILE_SQUARE_SIZE, y*TILE_SQUARE_SIZE);
						var enemyType = default(Enemies).Random();
						switch (enemyType)
						{
							case Enemies.CircleFish:
								var speed = (float)rand.NextDouble() * (cfMaxSpeed - cfMinSpeed) + cfMinSpeed;
								var radius = (float)rand.NextDouble() * (cfMaxRadius - cfMinRadius) + cfMinRadius;
								_ = _enemyFactory.SpawnCircleFish(this, spawnPos, speed, radius);
								break;
							case Enemies.PathFindingFish:
								_ = _enemyFactory.SpawnPathFindingFish(this, spawnPos);
								break;
							default:
								_logger.LogError("LevelGenerator GenerateEnemies Enemies did not map properly.");
								break;
						}
					}
				}
			}
		}
	}

	

	// Try redditor's comment's approach
	// https://www.reddit.com/r/godot/comments/1bnoxou/i_tried_adding_navigation_regions_to_my/
	void GenerateNavigationRegion(int lowestWidth, int highestWidth, int lowestHeight, int highestHeight)
	{
		// Create Nav Polygon with Vertices which surround the generated map
		// Add Nav Polygon to Nav Region
		// Bake the new nav poly with new vertices, into the nav region node
		// Note: TileMapLayer should be child of the Nav Region node

		var newNavigationMesh = new NavigationPolygon();
		var boundingOutline = new Vector2[]
		{
			new Vector2(lowestWidth*TILE_SQUARE_SIZE, lowestHeight*TILE_SQUARE_SIZE),
			new Vector2(lowestWidth*TILE_SQUARE_SIZE, highestHeight*TILE_SQUARE_SIZE),
			new Vector2(highestWidth*TILE_SQUARE_SIZE, highestHeight*TILE_SQUARE_SIZE),
			new Vector2(highestWidth*TILE_SQUARE_SIZE, lowestHeight*TILE_SQUARE_SIZE) 
		};
		newNavigationMesh.AddOutline(boundingOutline);
		NavigationServer2D.BakeFromSourceGeometryData(newNavigationMesh, new NavigationMeshSourceGeometryData2D());
		_navRegion.NavigationPolygon = newNavigationMesh;
	}

	// Recursive function to find a valid position for a tile of specific size
	Vector2 FindValidPosition(Vector2 position, Vector2 size)
	{
		foreach (Node2D obsticle in _spawnedObsticles)
		{
			// Get the size of the existing tile
			Vector2 existingTileSize = new Vector2(TILE_SQUARE_SIZE, TILE_SQUARE_SIZE); // Define this to retrieve size of existing tiles

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

	void GenerateUpgradeDoor(Vector2 position, Vector2 size)
	{
		_doorArea = new();
		CollisionShape2D collisionShape = new();
		RectangleShape2D shape = new();

		shape.Size = size;
		collisionShape.Shape = shape;
		_doorArea.AddChild(collisionShape);
		_doorArea.Position = position;
		_doorArea.AreaExited += HandleDoorAreaExited;
		this.AddChild(_doorArea);
	}

	void HandleDoorAreaExited(Area2D target)
	{ 
		_logger.LogInfo("Call HandleDoorAreaExited");
		if (target is PcHurtBoxArea pcHurtBoxArea)
		{
			_logger.LogInfo("Call HandleDoorAreaExited IS PcHurtBoxArea");
			// Use call_deferred to safely change the scene
			CallDeferred(nameof(ChangeToUpgradeLevel));
		}
	}

	void ChangeToUpgradeLevel()
	{
		GetTree().ChangeSceneToPacked(_upgradeLevelScene);
	}
}
