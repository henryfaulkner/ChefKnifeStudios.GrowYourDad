using Godot;
using System;
using System.Collections.Generic;

public interface IProteinFactory
{
	Protein SpawnProtein(Node parent, Vector2 initGlobalPosition, Vector2 dirVector, float speed);
	IEnumerable<Protein> SpawnMultiProtein(Node parent, Vector2 initGlobalPosition);
}

public partial class ProteinFactory : Node, IProteinFactory
{
	#region Protein
	private readonly StringName PROTEIN_SCENE_PATH = "res://ObjectLibrary/Protein/Protein.tscn";
	private PackedScene _proteinScene = null!;
	#endregion

	ILoggerService _logger = null!;

	public override void _Ready()
	{
		_proteinScene = (PackedScene)ResourceLoader.Load(PROTEIN_SCENE_PATH);

		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
	}

	public Protein SpawnProtein(Node parent, Vector2 initGlobalPosition, Vector2 dirVector, float speed)
	{
		var result = _proteinScene.Instantiate<Protein>();
		result.GlobalPosition = new Vector2(
			initGlobalPosition.X + dirVector.X, 
			initGlobalPosition.Y + dirVector.Y
		);
		result.GravityScale = 0.0f;
		result.ConstantForce = dirVector * speed;
		result.ApplyImpulse(dirVector * speed * 10, Vector2.Zero);
		
		CallDeferred(nameof(AddProteinToScene), parent, result);
		
		return result;
	}

	public IEnumerable<Protein> SpawnMultiProtein(Node parent, Vector2 initGlobalPosition)
	{
		List<Protein> result = new();
		Random rand = new();

		int spawnNumber = rand.Next(2, 6);

		for (int i = 0; i < spawnNumber; i += 1)
		{
			int spawnSpeed = rand.Next(7, 13);
			int xBit = rand.Next(0, 2); // 0 or 1
			int yBit = rand.Next(0, 2);

			if (xBit == 0) xBit = -1;
			if (yBit == 0) yBit = -1;

			Vector2 spawnDir = new (
				rand.NextSingle() * xBit,
				rand.NextSingle() * yBit
			);
			spawnDir = spawnDir.Normalized(); 

			result.Add(SpawnProtein(parent, initGlobalPosition, spawnDir, spawnSpeed));
		}
		return result;
	}
	
	void AddProteinToScene(Node parent, Node child) 
	{ 
		parent.AddChild(child); 
		
		// Create a Timer node to despawn the Blast
		double lifeTime = 1.0;
		Timer despawnTimer = new Timer
		{
			WaitTime = lifeTime,
			OneShot = true
		};
		void OnTimeout() { child.QueueFree(); }
		despawnTimer.Timeout += OnTimeout;
		child.AddChild(despawnTimer);
		despawnTimer.Start();	
	}
}
