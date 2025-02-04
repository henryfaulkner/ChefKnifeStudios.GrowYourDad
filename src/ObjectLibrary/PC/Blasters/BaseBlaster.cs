using Godot;
using System;

public partial class BaseBlaster : Node2D
{
	public virtual void Shoot()
	{
		GD.Print("Base Shot");
	}
}
