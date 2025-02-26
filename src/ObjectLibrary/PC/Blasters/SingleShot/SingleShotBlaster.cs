using Godot;
using System;
using System.Collections.Generic;

public partial class SingleShotBlaster : BaseBlaster
{
	const float BLAST_SPEED = 150f;
	
	IBlastFactory _blastFactory;
	IGameStateService _gameStateService;

	public override void _Ready()
	{
		_blastFactory = GetNode<IBlastFactory>(Constants.SingletonNodes.BlastFactory);
		_gameStateService = GetNode<IGameStateService>(Constants.SingletonNodes.GameStateService);
	}

	public override void Shoot()
	{
		if (_gameStateService.SpValue <= 0) return;
		_blastFactory.SpawnBlast(GetTree().GetRoot(), GlobalPosition, new Vector2(0.0f, 1.0f), BLAST_SPEED);
		_gameStateService.SpValue -= 1;
	}
}
