using Godot;
using System;

public interface IBlasterFactory
{
    public SingleShotBlaster SpawnSingleShotBlaster(Node parent, Vector2 globalPosition); 
}

public partial class BlasterFactory : Node, IBlasterFactory
{
    #region SingleShot
    private readonly StringName SINGLESHOT_SCENE_PATH = "res://ObjectLibrary/PC/Blasters/SingleShot/SingleShotBlaster.tscn";
    private PackedScene _singleShotScene;
    #endregion

    ILoggerService _logger;

	public override void _Ready()
	{
		_singleShotScene = (PackedScene)ResourceLoader.Load(SINGLESHOT_SCENE_PATH);

		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
	}

    public SingleShotBlaster SpawnSingleShotBlaster(Node parent, Vector2 globalPosition)
    {
        var result = _singleShotScene.Instantiate<SingleShotBlaster>();
		parent.AddChild(result);
		result.GlobalPosition = globalPosition;
		return result;
    }
}