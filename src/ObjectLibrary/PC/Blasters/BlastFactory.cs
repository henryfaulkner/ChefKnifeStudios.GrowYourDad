using Godot;
using System;

public interface IBlastFactory
{
	public Blast SpawnBlast(Node parent, Vector2 initGlobalPosition, Vector2 dirVector, float speed);
}

public partial class BlastFactory : Node, IBlastFactory
{
	#region Blast
	private readonly StringName BLAST_SCENE_PATH = "res://ObjectLibrary/PC/Blasters/Blast.tscn";
	private PackedScene _blastScene;
	#endregion

	ILoggerService _logger;

	public override void _Ready()
	{
		_blastScene = (PackedScene)ResourceLoader.Load(BLAST_SCENE_PATH);

		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
	}

	public Blast SpawnBlast(Node parent, Vector2 initGlobalPosition, Vector2 dirVector, float speed)
	{
		float initialOffsetAmount = 20.0f;
		double lifeTime = 1.0;
		
		var result = _blastScene.Instantiate<Blast>();
		parent.AddChild(result);
		result.GlobalPosition = new Vector2(
			initGlobalPosition.X + dirVector.X * initialOffsetAmount, 
			initGlobalPosition.Y + dirVector.Y * initialOffsetAmount
		);
		result.GravityScale = 0.0f;
		result.ConstantForce = dirVector * speed;
		result.ApplyImpulse(dirVector * speed * 10, Vector2.Zero);
		
		// Create a Timer node to despawn the Blast
		Timer despawnTimer = new Timer();
		despawnTimer.WaitTime = lifeTime;
		despawnTimer.OneShot = true;
		despawnTimer.Timeout += () => { result.QueueFree(); }; // Connect timeout to QueueFree
		result.AddChild(despawnTimer); // Add the Timer to the blast, so it will despawn as a result
		despawnTimer.Start();
		
		return result;
	}
}
