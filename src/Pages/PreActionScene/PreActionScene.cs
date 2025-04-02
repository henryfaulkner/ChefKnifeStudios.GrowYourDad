using Godot;
using Microsoft.EntityFrameworkCore.Metadata;
using System;

public partial class PreActionScene : Node2D
{
	[Export]
	Area2D DoorArea { get; set; } = null!;

	ILoggerService _logger = null!;
	IPcMeterService _pcMeterService = null!;
	IPcInventoryService _pcInventoryService = null!;
	NavigationAuthority _navigationAuthority = null!;

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_pcMeterService = GetNode<IPcMeterService>(Constants.SingletonNodes.PcMeterService);
		_pcInventoryService = GetNode<IPcInventoryService>(Constants.SingletonNodes.PcInventoryService);
		_navigationAuthority = GetNode<NavigationAuthority>(Constants.SingletonNodes.NavigationAuthority);
		
		_pcMeterService.HpValue = _pcInventoryService.GetPcHpMax();
		_pcMeterService.HpMax = _pcInventoryService.GetPcHpMax();
		_pcMeterService.SpValue = _pcInventoryService.GetPcHpMax();
		_pcMeterService.SpMax = _pcInventoryService.GetPcSpMax();
		
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
			RarityTier = "common",
		});

		// Use call_deferred to safely change the scene
		_navigationAuthority.CallDeferred("ChangeToActionLevel");
	}
}
