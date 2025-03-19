using System;
using Godot;

public interface IEnemyFactory
{
	CircleFish SpawnCircleFish(Node parent, Vector2 globalPosition, float? speed = null, float? radius = null);
	LineFish SpawnLineFish(Node parent, Vector2 globalPosition, Vector2 pointOnePos, Vector2 pointTwoPos, float? speed = null);
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

	ILoggerService _logger = null!;

	public EnemyFactory()
	{
		_circleFishScene = (PackedScene)ResourceLoader.Load(CIRCLE_FISH_PATH);
		_lineFishScene = (PackedScene)ResourceLoader.Load(LINE_FISH_PATH);
		_pathFindingFishScene = (PackedScene)ResourceLoader.Load(PATH_FINDING_FISH_PATH);
	} 

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
	}

	public CircleFish SpawnCircleFish(Node parent, Vector2 globalPosition, float? speed = null, float? radius = null)
	{
		var result = _circleFishScene.Instantiate<CircleFish>();
		parent.AddChild(result);
		result.GlobalPosition = globalPosition;
		if (speed.HasValue) result.Speed = speed.Value;
		if (radius.HasValue) result.Radius = radius.Value;
		return result;
	}

	public LineFish SpawnLineFish(Node parent, Vector2 globalPosition, Vector2 pointOnePos, Vector2 pointTwoPos, float? speed = null)
	{
		_logger.LogInfo("Call SpawnLineFish");
		try
		{
			var result = _lineFishScene.Instantiate<LineFish>();
			result.GlobalPosition = globalPosition;
			result.Curve.ClearPoints();
			result.Curve.AddPoint(pointOnePos);
			result.Curve.AddPoint(pointTwoPos);
			if (speed.HasValue) result.Speed = speed.Value;
			parent.AddChild(result);
			return result;
		} 
		catch (Exception ex)
		{
			var innerException = ex;
			while (innerException.InnerException != null)
			{
				innerException = innerException.InnerException;
			}
			_logger.LogError($"Inner exception: {innerException.Message}");
			throw;
		}
	}
	
	public PathFindingFish SpawnPathFindingFish(Node parent, Vector2 globalPosition)
	{
		var result = _pathFindingFishScene.Instantiate<PathFindingFish>();
		parent.AddChild(result);
		result.GlobalPosition = globalPosition;
		return result;
	}
}
