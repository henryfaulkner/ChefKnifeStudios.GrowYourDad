using Godot;
using System;

public partial class TripleShotBlaster : BaseBlaster
{
	const float BLAST_SPEED = 150f;
	
	IBlastFactory _blastFactory;
	IPcMeterService _pcMeterService;

	public override void _Ready()
	{
		_blastFactory = GetNode<IBlastFactory>(Constants.SingletonNodes.BlastFactory);
		_pcMeterService = GetNode<IPcMeterService>(Constants.SingletonNodes.PcMeterService);
	}

	public override void Shoot()
	{
		if (_pcMeterService.SpValue <= 0) return;
		_blastFactory.SpawnBlast(GetTree().GetRoot(), GlobalPosition, new Vector2(-0.25f, 0.75f), BLAST_SPEED);
		_blastFactory.SpawnBlast(GetTree().GetRoot(), GlobalPosition, new Vector2(0.0f, 1.0f), BLAST_SPEED);
		_blastFactory.SpawnBlast(GetTree().GetRoot(), GlobalPosition, new Vector2(0.25f, 0.75f), BLAST_SPEED);
		_pcMeterService.SpValue -= 3;
	}
}
