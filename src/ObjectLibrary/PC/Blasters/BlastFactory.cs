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

	Timer DespawnTimer;
	Action DespawnTimeoutHandler;

	public override void _Ready()
	{
		_blastScene = (PackedScene)ResourceLoader.Load(BLAST_SCENE_PATH);

		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
	}

	public override void _ExitTree()
	{
		if (DespawnTimer != null && DespawnTimeoutHandler!= null)
		{
			DespawnTimer.Timeout -= DespawnTimeoutHandler;
		}
	}

	public Blast SpawnBlast(Node parent, Vector2 initGlobalPosition, Vector2 dirVector, float speed)
	{
		float initialOffsetAmount = 20.0f;
		double lifeTime = 1.0;
		
		var result = _blastScene.Instantiate<Blast>();
		result.GlobalPosition = new Vector2(
			initGlobalPosition.X + dirVector.X * initialOffsetAmount, 
			initGlobalPosition.Y + dirVector.Y * initialOffsetAmount
		);
		result.GravityScale = 0.0f;
		result.ConstantForce = dirVector * speed;
		result.ApplyImpulse(dirVector * speed * 10, Vector2.Zero);
		parent.AddChild(result);
		
		// Create a Timer node to despawn the Blast
		DespawnTimer = new Timer();
		DespawnTimer.WaitTime = lifeTime;
		DespawnTimer.OneShot = true;
		DespawnTimeoutHandler = () => { result.QueueFree(); };
		DespawnTimer.Timeout += DespawnTimeoutHandler; // Connect timeout to QueueFree
		result.AddChild(DespawnTimer); // Add the Timer to the blast, so it will despawn as a result
		DespawnTimer.Start();
		
		return result;
	}
}
