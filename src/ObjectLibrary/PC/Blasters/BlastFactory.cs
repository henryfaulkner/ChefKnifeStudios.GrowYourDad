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
	private PackedScene _blastScene = null!;
	#endregion

	ILoggerService _logger = null!;

	public override void _Ready()
	{
		_blastScene = (PackedScene)ResourceLoader.Load(BLAST_SCENE_PATH);

		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
	}

	public Blast SpawnBlast(Node parent, Vector2 initGlobalPosition, Vector2 dirVector, float speed)
	{
		float initialOffsetAmount = 20.0f;
		double lifeTime = 1.0;

		// Instantiate a new Blast instance
		var result = _blastScene.Instantiate<Blast>();
		result.GlobalPosition = new Vector2(
			initGlobalPosition.X + dirVector.X * initialOffsetAmount, 
			initGlobalPosition.Y + dirVector.Y * initialOffsetAmount
		);
		result.GravityScale = 0.0f;
		result.ConstantForce = dirVector * speed;
		result.ApplyImpulse(dirVector * speed * 10, Vector2.Zero);
		parent.AddChild(result);

		// Create a Timer node and add it to the Blast instance
		Timer despawnTimer = new Timer
		{
			WaitTime = lifeTime,
			OneShot = true
		};
		void OnTimeout() { result.QueueFree(); }
		despawnTimer.Timeout += OnTimeout;
		result.AddChild(despawnTimer);
		despawnTimer.Start();

		return result;
	}
}
