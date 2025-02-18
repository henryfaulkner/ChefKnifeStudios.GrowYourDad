using Godot;
using System;

public partial class TripleShotBlaster : BaseBlaster
{
	IBlastFactory _blastFactory;

	public override void _Ready()
	{
		_blastFactory = GetNode<IBlastFactory>(Constants.SingletonNodes.BlastFactory);
	}

	public override void Shoot()
	{
		_blastFactory.SpawnBlast(GetTree().GetRoot(), GlobalPosition, new Vector2(-0.5f, 0.5f), 20f);
		_blastFactory.SpawnBlast(GetTree().GetRoot(), GlobalPosition, new Vector2(0.0f, 1.0f), 20f);
		_blastFactory.SpawnBlast(GetTree().GetRoot(), GlobalPosition, new Vector2(0.5f, 0.5f), 20f);
	}
}
