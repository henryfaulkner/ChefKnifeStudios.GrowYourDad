using Godot;
using System;

public partial class ActionLevel : Node2D
{
	ILoggerService _logger;
	IGameStateService _gameStateService;

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_gameStateService = GetNode<IGameStateService>(Constants.SingletonNodes.GameStateService);

		_gameStateService.HpValue = 3;
		_gameStateService.HpMax = 3;
		_gameStateService.SpValue = 3;
		_gameStateService.SpMax = 3;
	}
}
