using Godot;
using System;
using System.Threading.Tasks;

public partial class LineFish : Path2D, IEnemy
{
	[Export]
	PathFollow2D _pathFollow = null!;
	[Export]
	EnemyHurtBoxArea _hurtBox = null!;
	[Export]
	EnemyHitBoxArea _hitBox = null!;

	public Spike? Spike { get; set; } = null!; 

	public float Speed = 0.2f;
	int _directionSign = 1;

	[Export]
	int _hp = 2;

	ILoggerService _logger = null!;
	IPcInventoryService _pcInventoryService = null!;
	IProteinFactory _proteinFactory = null!;
	Observables _observables = null!;
	ICrawlStatsService _crawlStatsService = null!;

	bool _isFlashing = false;

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_pcInventoryService = GetNode<IPcInventoryService>(Constants.SingletonNodes.PcInventoryService);
		_proteinFactory = GetNode<IProteinFactory>(Constants.SingletonNodes.ProteinFactory);
		_observables = GetNode<Observables>(Constants.SingletonNodes.Observables);
		_crawlStatsService = GetNode<ICrawlStatsService>(Constants.SingletonNodes.CrawlStatsService);
		
		_hurtBox.AreaHurt += HandleHurt;
		_hitBox.AreaHit += HandleHit;
	}

	public override void _PhysicsProcess(double delta)
	{
		ProcessPathFollow(_pathFollow, Speed, delta);
		SyncChildPositions(_pathFollow);
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
				_logger.LogError("LineFish HandleHit did not map properly.");
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
				_logger.LogError("LineFish HandleHurt did not map properly.");
				break;
		}
	}

	public void TakeDamage()
	{
		_hp -= _pcInventoryService.GetPcDamage();
		if (_hp <= 0)
		{
			HandleDeath();
		}
	}

	public async Task StartFlashing()
	{
		_isFlashing = true;
		var originalColor = Modulate;

		float elapsed = 0.0f;

		while (elapsed < Constants.Invulnerability.Duration)
		{
			Modulate = (Modulate == originalColor) ? new Color(1.0f, 0.0f, 0.0f, 1.0f) : originalColor;
			await ToSignal(GetTree().CreateTimer(Constants.Invulnerability.Interval), "timeout");
			elapsed += Constants.Invulnerability.Interval;
		}

		// Ensure color is reset to the original after flashing
		Modulate = originalColor;
		_isFlashing = false;
	}

	void ReactToPcHit()
	{
		int damageConstant = 1;
		_observables.EmitPcHit(damageConstant);
	}
	
	void ReactToBlastHurt()
	{
		if (!_isFlashing) 
		{
			_ = StartFlashing();
			if (Spike is not null) Spike.StartFlashing();
		}
		TakeDamage();
	}

	void ReactToBootsHurt()
	{
		HandleDeath();
	}

	void HandleDeath()
	{
		_crawlStatsService.CrawlStats.FoesDefeated += 1;
		_proteinFactory.SpawnMultiProtein(GetNode(".."), _pathFollow.GlobalPosition);
		if (Spike != null) Spike.QueueFree();
		QueueFree();
	}

	bool ProcessPathFollow(PathFollow2D pathFollow, float speed, double delta)
	{
		var result = false;
		if (pathFollow == null)
		{
			_logger.LogError("PathFollow2D is not assigned.");
			return result;
		}

		if (pathFollow.ProgressRatio + (speed * _directionSign) * (float)delta > 1)
		{
			pathFollow.ProgressRatio = 1.0f;
			_directionSign *= -1;
		}
		else if (pathFollow.ProgressRatio + (speed * _directionSign) * (float)delta < 0)
		{
			pathFollow.ProgressRatio = 0.0f;
			_directionSign *= -1;
			result = true;
		}
		else
		{
			pathFollow.ProgressRatio += (speed * _directionSign) * (float)delta;
		}
		return result;
	}

	void SyncChildPositions(PathFollow2D pathFollow)
	{
		Vector2 position = pathFollow.GlobalPosition;
		if (Spike is not null) 
		{
			Spike.GlobalPosition = new Vector2(
				position.X, 
				position.Y - 15
			);
		}
	}
}
