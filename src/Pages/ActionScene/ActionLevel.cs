using Godot;
using System;
using System.Linq;
using System.Reflection.Metadata;

public partial class ActionLevel : Node2D
{
	ILoggerService _logger;
	IPcMeterService _pcMeterService;
	IPcInventoryService _pcInventoryService;
	PauseMenuService _pauseMenuService;
	Observables _observables;

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_pcMeterService = GetNode<IPcMeterService>(Constants.SingletonNodes.PcMeterService);
		_pauseMenuService = GetNode<PauseMenuService>(Constants.SingletonNodes.PauseMenuService);
		_pcInventoryService = GetNode<IPcInventoryService>(Constants.SingletonNodes.PcInventoryService);
		_observables = GetNode<Observables>(Constants.SingletonNodes.Observables);

		_pcMeterService.HpValue = _pcMeterService.HpValue;
		_pcMeterService.HpMax = _pcInventoryService.GetPcHpMax();
		_pcMeterService.SpValue = _pcInventoryService.GetPcSpMax();
		_pcMeterService.SpMax = _pcInventoryService.GetPcSpMax();

		// force change meters
		_observables.EmitUpdateHpMeterValue(_pcMeterService.HpValue);
		_observables.EmitUpdateHpMeterMax(_pcMeterService.HpMax);
		_observables.EmitUpdateSpMeterValue(_pcMeterService.SpValue);
		_observables.EmitUpdateSpMeterMax(_pcMeterService.SpMax);

		_pauseMenuService.EmitCloseMenu();
	}
}
