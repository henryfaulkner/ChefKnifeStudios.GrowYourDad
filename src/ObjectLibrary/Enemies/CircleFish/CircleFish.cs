using Godot;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

public partial class CircleFish : Path2D, IEnemy
{
	[ExportGroup("Nodes")]
	[Export]
	PathFollow2D _pathFollow = null!;
	[Export]
	EnemyHurtBoxArea _hurtBox = null!;
	[Export]
	EnemyHitBoxArea _hitBox = null!;
	
	[ExportGroup("Variables")]
	[Export]
	public float Speed { get; set; } = 0.2f;
	[Export]
	public float Radius { get; set; } = 125.0f;
	[Export]
	int _numPoints = 100;
	
	[Export]
	float _flashDuration = 1.0f; // Total duration of the flashing effect
	[Export]
	float _flashInterval = 0.2f; // Time between flashes

	[Export]
	int _hp = 1;

	ILoggerService _logger = null!;
	IPcInventoryService _pcInventoryService = null!;
	IProteinFactory _proteinFactory = null!;
	Observables _observables = null!;

	bool _isFlashing = false;

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_pcInventoryService = GetNode<IPcInventoryService>(Constants.SingletonNodes.PcInventoryService);
		_proteinFactory = GetNode<IProteinFactory>(Constants.SingletonNodes.ProteinFactory);
		_observables = GetNode<Observables>(Constants.SingletonNodes.Observables);
		
		var circlePoints = CircleHelper.GetCirclePoints(GlobalPosition, Radius, _numPoints);
		
		// Center the points
		CircleHelper.TranslateListOfVectors(ref circlePoints, -GlobalPosition);

		Curve = CreateCurve(circlePoints);

		_hurtBox.AreaHurt += HandleHurt;
		_hitBox.AreaHit += HandleHit;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		ProcessPathFollow(_pathFollow, Speed, delta);
	}

	public override void _ExitTree()
	{
		if (_hurtBox != null)
		{
			_hurtBox.AreaHurt -= HandleHurt;
		}

		if (_hitBox != null)
		{
			_hitBox.AreaHit -= HandleHit;
		}
	}

	public void HandleHit(int pcArea)
	{
		switch ((Enumerations.PcAreas)pcArea)
		{
			case Enumerations.PcAreas.Body:
				ReactToPcHit();
				break;
			case Enumerations.PcAreas.Blast:
			case Enumerations.PcAreas.Boots:
				break;
			default:
				_logger.LogError("CircleFish HandleHit did not map properly.");
				break;
		}
	}

	public void HandleHurt(int pcArea)
	{
		switch ((Enumerations.PcAreas)pcArea)
		{
			case Enumerations.PcAreas.Body:
				break;
			case Enumerations.PcAreas.Blast:
				ReactToBlastHurt();
				break;
			case Enumerations.PcAreas.Boots:
				ReactToBootsHurt();
				break;
			default:
				_logger.LogError("CircleFish HandleHurt did not map properly.");
				break;
		}
	}

	void ReactToPcHit()
	{
		int damageConstant = 1;
		_observables.EmitPcHit(damageConstant);
	}
	
	void ReactToBlastHurt()
	{
		if (!_isFlashing) StartFlashing();
		TakeDamage();
	}

	void ReactToBootsHurt()
	{
		HandleDeath();
	}

	async void StartFlashing()
	{
		_isFlashing = true;
		var originalColor = Modulate;

		float elapsed = 0.0f;

		while (elapsed < _flashDuration)
		{
			Modulate = (Modulate == originalColor) ? new Color(1.0f, 0.0f, 0.0f, 1.0f) : originalColor;
			await ToSignal(GetTree().CreateTimer(_flashInterval), "timeout");
			elapsed += _flashInterval;
		}

		// Ensure color is reset to the original after flashing
		Modulate = originalColor;
		_isFlashing = false;
	}

	void TakeDamage()
	{
		_hp -= _pcInventoryService.GetPcDamage();
		if (_hp <= 0)
		{
			HandleDeath();
		}
	}

	void HandleDeath()
	{
		_proteinFactory.SpawnMultiProtein(GetNode(".."), _pathFollow.GlobalPosition);
		QueueFree();
	}

	static Curve2D CreateCurve(List<Vector2> points)
	{
		var result = new Curve2D();
		for (int i = 0; i < points.Count; i += 1)
		{
			result.AddPoint(points[i]);
		}
		return result;
	}
	
	static bool ProcessPathFollow(PathFollow2D pathFollow, float speed, double delta)
	{
		var result = false;
		if (pathFollow == null)
		{
			GD.PrintErr("PathFollow2D is not assigned.");
			return result;
		}
		
		pathFollow.ProgressRatio += speed * (float)delta;
		return result;
	}
}
