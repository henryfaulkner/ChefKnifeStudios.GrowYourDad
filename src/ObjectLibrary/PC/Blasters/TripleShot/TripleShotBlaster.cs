using Godot;
using System;

public partial class TripleShotBlaster : BaseBlaster
{
	const float BLAST_SPEED = 150f;
	
	IBlastFactory _blastFactory;

	public override void _Ready()
	{
		_blastFactory = GetNode<IBlastFactory>(Constants.SingletonNodes.BlastFactory);
	}

	public override void Shoot()
	{
		_blastFactory.SpawnBlast(GetTree().GetRoot(), GlobalPosition, new Vector2(-0.5f, 0.5f), BLAST_SPEED);
		_blastFactory.SpawnBlast(GetTree().GetRoot(), GlobalPosition, new Vector2(0.0f, 1.0f), BLAST_SPEED);
		_blastFactory.SpawnBlast(GetTree().GetRoot(), GlobalPosition, new Vector2(0.5f, 0.5f), BLAST_SPEED);
	}
}
