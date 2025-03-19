using Godot;
using System;

public partial class PreActionScene : Node2D
{
	[Export]
	Area2D DoorArea { get; set; } = null!;

	static readonly StringName ACTION_LEVEL_PATH = new StringName("res://Pages/ActionScene/ActionLevel.tscn");
	readonly PackedScene _actionLevelScene = null!;

	ILoggerService _logger = null!;
	PauseMenuService _pauseMenuService = null!;
	IPcMeterService _pcMeterService = null!;
	IPcInventoryService _pcInventoryService = null!;

	public PreActionScene()
	{
		_actionLevelScene = (PackedScene)ResourceLoader.Load(ACTION_LEVEL_PATH);
	}

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_pauseMenuService = GetNode<PauseMenuService>(Constants.SingletonNodes.PauseMenuService);
		_pcMeterService = GetNode<IPcMeterService>(Constants.SingletonNodes.PcMeterService);
		_pcInventoryService = GetNode<IPcInventoryService>(Constants.SingletonNodes.PcInventoryService);
		
		_pcMeterService.HpValue = _pcInventoryService.GetPcHpMax();
		_pcMeterService.HpMax = _pcInventoryService.GetPcHpMax();
		_pcMeterService.SpValue = _pcInventoryService.GetPcHpMax();
		_pcMeterService.SpMax = _pcInventoryService.GetPcSpMax();

		_pauseMenuService.EmitCloseMenu();
		
		DoorArea.AreaExited += HandleDoorAreaExited;
	}
	
	public override void _ExitTree()
	{
		if (DoorArea != null)
		{
			DoorArea.AreaExited -= HandleDoorAreaExited;
		}
	}

	void HandleDoorAreaExited(Area2D target)
	{ 
		_pcInventoryService.AddToInventory(new ItemWithBlastingEffect()
		{
			Id = "SINGLE_SHOT_BLASTER",
			Name = "Single Shot Blaster",
			Description = "A blaster that fires one shot at a time.",
			Price = 1,
			DamageBase = 1,
			AmmoConsumed = 1,
			BlasterType = Enumerations.BlasterTypes.SingleShotBlaster,
		});

		// Use call_deferred to safely change the scene
		CallDeferred(nameof(ChangeToActionLevel));
	}

	void ChangeToActionLevel()
	{
		GetTree().ChangeSceneToPacked(_actionLevelScene);
	}
}
