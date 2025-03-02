using Godot;
using System;

public partial class UpgradeScene : CanvasLayer
{
	[Export]
	Area2D FreezeArea { get; set; }
	[Export]
	Area2D DoorArea { get; set; }

	static readonly StringName ACTION_LEVEL_PATH = new StringName("res://Pages/ActionScene/ActionLevel.tscn");
	readonly PackedScene _actionLevelScene;

	ILoggerService _logger;
	PauseMenuService _pauseMenuService;

	public UpgradeScene()
	{
		_actionLevelScene = (PackedScene)ResourceLoader.Load(ACTION_LEVEL_PATH);
	}

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_pauseMenuService = GetNode<PauseMenuService>(Constants.SingletonNodes.PauseMenuService);

		_pauseMenuService.EmitCloseMenu();
		
		FreezeArea.AreaEntered += HandleFreezeAreaEntered;
		DoorArea.AreaEntered += HandleDoorAreaEntered;
	}

	public override void _ExitTree()
	{
		if (FreezeArea != null)
		{
			FreezeArea.AreaEntered -= HandleFreezeAreaEntered;
		}

		if (DoorArea != null)
		{
			DoorArea.AreaEntered -= HandleDoorAreaEntered;
		}
	}

	void HandleFreezeAreaEntered(Area2D target)
	{
		GetTree().Paused = true;
	}

	void HandleDoorAreaEntered(Area2D target)
	{
		// Use call_deferred to safely change the scene
		CallDeferred(nameof(ChangeToActionLevel));
	}

	void ChangeToActionLevel()
	{
		GetTree().ChangeSceneToPacked(_actionLevelScene);
	}
}
