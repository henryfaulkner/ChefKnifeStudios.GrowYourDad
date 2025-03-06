using Godot;
using System;
using System.Reflection.Metadata;

public partial class ActionLevel : Node2D
{
	ILoggerService _logger;
	IPcMeterService _pcMeterService;
	PauseMenuService _pauseMenuService;

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_pcMeterService = GetNode<IPcMeterService>(Constants.SingletonNodes.PcMeterService);
		_pauseMenuService = GetNode<PauseMenuService>(Constants.SingletonNodes.PauseMenuService);

		_pcMeterService.HpValue = 3;
		_pcMeterService.HpMax = 3;
		_pcMeterService.SpValue = 3;
		_pcMeterService.SpMax = 3;

		_pauseMenuService.EmitCloseMenu();
	}
}
