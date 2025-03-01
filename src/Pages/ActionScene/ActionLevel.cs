using Godot;
using System;
using System.Reflection.Metadata;

public partial class ActionLevel : Node2D
{
	ILoggerService _logger;
	IGameStateService _gameStateService;
	PauseMenuService _pauseMenuService;

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_gameStateService = GetNode<IGameStateService>(Constants.SingletonNodes.GameStateService);
		_pauseMenuService = GetNode<PauseMenuService>(Constants.SingletonNodes.PauseMenuService);

		_gameStateService.HpValue = 3;
		_gameStateService.HpMax = 3;
		_gameStateService.SpValue = 3;
		_gameStateService.SpMax = 3;

		_pauseMenuService.EmitCloseMenu();
	}
}
