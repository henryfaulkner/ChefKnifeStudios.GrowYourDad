using Godot;
using System;

public partial class OpenClosePauseMenuListener : Node2D
{
	private static readonly StringName _PAUSE_INPUT = new StringName("pause");

	public MenuBusiness MenuBusiness { get; set; } = null!;

	[Export]
	private Panel BaseMenuPanels { get; set; } = null!;

	public void Init(MenuBusiness menuBusiness)
	{
		MenuBusiness = menuBusiness;
		
		MenuBusiness.OpenMenu += HandleOpenMenu;
		MenuBusiness.CloseMenu += HandleCloseMenu;
	}

	public override void _Ready()
	{
		ProcessMode = Node.ProcessModeEnum.Always;
	}

	public override void _Process(double _delta)
	{
		if (Input.IsActionJustPressed(_PAUSE_INPUT))
		{
			if (GetTree().Paused)
			{
				MenuBusiness.EmitCloseMenu();
			}
			else
			{
				MenuBusiness.EmitOpenMenu();
			}
		}
	}

	public override void _ExitTree()
	{
		if (MenuBusiness != null)
		{
			MenuBusiness.OpenMenu -= HandleOpenMenu;
			MenuBusiness.CloseMenu -= HandleCloseMenu;
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
