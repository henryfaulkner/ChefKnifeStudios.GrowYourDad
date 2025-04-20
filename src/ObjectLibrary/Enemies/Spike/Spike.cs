using Godot;
using System;

public partial class Spike : StaticBody2D
{
	[Export]
	EnemyHurtBoxArea _hurtBox = null!;
	[Export]
	EnemyHitBoxArea _hitBox = null!;

	public IEnemy? Enemy { get; set; } = null!;

	ILoggerService _logger = null!;
	Observables _observables = null!;

	bool _isFlashing = false;
	const int DAMAGE = 1;

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_observables = GetNode<Observables>(Constants.SingletonNodes.Observables);

		_hurtBox.AreaHurt += HandleHurt;
		_hitBox.AreaHit += HandleHit;

		base._Ready();
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

		base._ExitTree();
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

	public async void StartFlashing()
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
			StartFlashing();
			if (Enemy is not null) Enemy.StartFlashing();
		}
		TakeDamage();
	}

	void ReactToBootsHurt()
	{
		_observables.EmitPcHit(DAMAGE);
	}

	void TakeDamage()
	{
		// pass damage handling to associated enemy
		if (Enemy != null) Enemy.TakeDamage();
	}
}
