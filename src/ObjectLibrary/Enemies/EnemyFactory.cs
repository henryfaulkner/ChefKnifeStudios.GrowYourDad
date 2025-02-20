using System;
using Godot;

public interface IEnemyFactory
{
    CircleFish SpawnCircleFish(Node parent, Vector2 globalPosition);
    LineFish SpawnLineFish(Node parent, Vector2 globalPosition);
    PathFindingFish SpawnPathFindingFish(Node parent, Vector2 globalPosition);
}

public partial class EnemyFactory : Node, IEnemyFactory
{
    readonly StringName CIRCLE_FISH_PATH = new StringName("res://ObjectLibrary/Enemies/CircleFish/CircleFish.tscn");
    readonly PackedScene _circleFishScene;
    
    readonly StringName LINE_FISH_PATH = new StringName("res://ObjectLibrary/Enemies/LineFish/LineFish.tscn");
    readonly PackedScene _lineFishScene;
    
    readonly StringName PATH_FINDING_FISH_PATH = new StringName("res://ObjectLibrary/Enemies/PathFindingFish/PathFindingFish.tscn");
    readonly PackedScene _pathFindingFishScene;

    public EnemyFactory()
    {
        _circleFishScene = (PackedScene)ResourceLoader.Load(CIRCLE_FISH_PATH);
        _lineFishScene = (PackedScene)ResourceLoader.Load(LINE_FISH_PATH);
        _pathFindingFishScene = (PackedScene)ResourceLoader.Load(PATH_FINDING_FISH_PATH);
    } 

    public CircleFish SpawnCircleFish(Node parent, Vector2 globalPosition)
	{
		var result = _circleFishScene.Instantiate<CircleFish>();
		parent.AddChild(result);
		result.GlobalPosition = globalPosition;
		return result;
	}

    public LineFish SpawnLineFish(Node parent, Vector2 globalPosition)
	{
		var result = _lineFishScene.Instantiate<LineFish>();
		parent.AddChild(result);
		result.GlobalPosition = globalPosition;
		return result;
	}
    
    public PathFindingFish SpawnPathFindingFish(Node parent, Vector2 globalPosition)
	{
		var result = _pathFindingFishScene.Instantiate<PathFindingFish>();
		parent.AddChild(result);
		result.GlobalPosition = globalPosition;
		return result;
	}
}
