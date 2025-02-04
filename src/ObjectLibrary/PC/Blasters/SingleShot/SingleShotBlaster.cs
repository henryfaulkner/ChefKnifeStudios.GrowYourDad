using Godot;
using System;

public partial class SingleShotBlaster : BaseBlaster
{
	public override void Shoot()
	{
		GD.Print("Single Shot");
	}
}
