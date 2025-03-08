using Godot;
using System;

public partial class LineFish : Path2D, IEnemy
{
	[Export]
	PathFollow2D _pathFollow;
	[Export]
	EnemyHurtBoxArea _hurtBox;
	[Export]
	EnemyHitBoxArea _hitBox;

	float _speed = 0.2f;
	int _directionSign = 1;

	[Export]
	float _flashDuration = 1.0f; // Total duration of the flashing effect
	[Export]
	float _flashInterval = 0.2f; // Time between flashes
	

	[Export]
	int _hp = 2;

	ILoggerService _logger;
	IPcMeterService _pcMeterService;
	IPcInventoryService _pcInventoryService;

	bool _isFlashing = false;

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_pcMeterService = GetNode<IPcMeterService>(Constants.SingletonNodes.PcMeterService);
		_pcInventoryService = GetNode<IPcInventoryService>(Constants.SingletonNodes.PcInventoryService);
		
		_hurtBox.AreaHurt += HandleHurt;
		_hitBox.AreaHit += HandleHit;
	}

	public override void _Process(double delta)
	{
		ProcessPathFollow(_pathFollow, _speed, delta);
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

	void ReactToPcHit()
	{
		int damageConstant = 1;
		_pcMeterService.HpValue -= damageConstant;
	}
	
	public void ReactToBlastHurt()
	{
		if (!_isFlashing) StartFlashing();
		TakeDamage();
	}

	public void ReactToBootsHurt()
	{
		QueueFree();
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
		if (_hp == 0)
		{
			QueueFree();
		}
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
}
