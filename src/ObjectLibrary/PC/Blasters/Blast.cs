using Godot;
using System;

public partial class Blast : RigidBody2D
{
	[Export]
	private Area2D HitBox { get; set; } = null!;

	

	public override void _Ready() 
	{
		HitBox.AreaEntered += HandleBlastHit;
	}

	public void HandleBlastHit(Node2D target)
	{
		if (target is IBlasterTarget blasterTarget)
		{

			blasterTarget.ReactToBlastHit();
			QueueFree();
		} 
	}
}
