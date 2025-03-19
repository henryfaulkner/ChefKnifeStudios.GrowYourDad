using Godot;
using System;

public partial class OpenClosePauseMenuListener : Node2D
{
	private static readonly StringName _PAUSE_INPUT = new StringName("pause");

	PauseMenuService _pauseMenuService = null!;

	[Export]
	private Panel BaseMenuPanels { get; set; } = null!;

	public override void _Ready()
	{
		_pauseMenuService = GetNode<PauseMenuService>(Constants.SingletonNodes.PauseMenuService);

		ProcessMode = Node.ProcessModeEnum.Always;
		_pauseMenuService.OpenMenu += HandleOpenMenu;
		_pauseMenuService.CloseMenu += HandleCloseMenu;
	}

	public override void _Process(double _delta)
	{
		if (Input.IsActionJustPressed(_PAUSE_INPUT))
		{
			if (GetTree().Paused)
			{
				_pauseMenuService.EmitCloseMenu();
			}
			else
			{
				_pauseMenuService.EmitOpenMenu();
			}
		}
	}

	public override void _ExitTree()
	{
		if (_pauseMenuService != null)
		{
			_pauseMenuService.OpenMenu -= HandleOpenMenu;
			_pauseMenuService.CloseMenu -= HandleCloseMenu;
		}
	}

	void HandleOpenMenu()
	{
		GetTree().Paused = true;
	}

	void HandleCloseMenu()
	{
		GetTree().Paused = false;
	}
}
