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
		var result = _blastScene.Instantiate<Blast>();
		parent.AddChild(result);
		result.GlobalPosition = new Vector2(initGlobalPosition.X + dirVector.X * speed, initGlobalPosition.Y + dirVector.Y * speed);
		result.GravityScale = 0.0f;
		result.ConstantForce = dirVector * speed;
		result.ApplyImpulse(dirVector * speed * 10, Vector2.Zero);
		return result;
	}
}
