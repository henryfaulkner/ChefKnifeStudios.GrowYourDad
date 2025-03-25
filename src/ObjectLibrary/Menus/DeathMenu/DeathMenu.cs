using Godot;
using System;

public partial class DeathMenu : Panel
{
	
	static readonly StringName _UP_INPUT = new StringName("up");
	static readonly StringName _DOWN_INPUT = new StringName("down");
	
	public MenuBusiness MenuBusiness { get; set; } = null!;

	public override void _PhysicsProcess(double delta)
 	{
 		if (Input.IsActionJustPressed(_UP_INPUT))
 		{
 			var x = MenuBusiness.WorkingDirectory.Peek();
			x.MoveFocusBackward();
 		}
 
 		if (Input.IsActionJustPressed(_DOWN_INPUT))
 		{
			var x = MenuBusiness.WorkingDirectory.Peek();
			x.MoveFocusForward();
 		}
	}
}
