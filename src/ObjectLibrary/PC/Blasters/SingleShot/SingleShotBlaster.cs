using Godot;
using System;
using System.Collections.Generic;

public partial class SingleShotBlaster : BaseBlaster
{
	IBlastFactory _blastFactory;

	public override void _Ready()
	{
		_blastFactory = GetNode<IBlastFactory>(Constants.SingletonNodes.BlastFactory);
	}

	public override void Shoot()
	{
		_blastFactory.SpawnBlast(GetTree().GetRoot(), GlobalPosition, new Vector2(0.0f, 1.0f), 20f);
	}
}
