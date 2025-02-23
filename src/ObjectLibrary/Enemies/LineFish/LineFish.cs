using Godot;
using System;

public partial class LineFish : Path2D, IBlasterTarget
{
	[Export]
	PathFollow2D _pathFollow;
	[Export]
	TargetArea2D _hitBox;

	float _speed = 0.2f;
	int _directionSign = 1;

	[Export]
	float _flashDuration = 1.0f; // Total duration of the flashing effect
	[Export]
	float _flashInterval = 0.2f; // Time between flashes
	

	[Export]
	int _hp = 2;

	ILoggerService _logger;
	bool _isFlashing = false;

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);

		_hitBox.TargetHit += HandleHit;
	}

	public override void _Process(double delta)
	{
		ProcessPathFollow(_pathFollow, _speed, delta);
	}
	
	public void HandleHit(int hitType)
	{
		switch ((Enumerations.HitTypes)hitType)
		{
			case Enumerations.HitTypes.Blast:
				ReactToBlastHit();
				break;
			case Enumerations.HitTypes.Boots:
				ReactToBootsHit();
				break;
			default:
				_logger.LogError("CircleFish HandleHit did not map properly.");
				break;
		}
	}
	
	public void ReactToBlastHit()
	{
		if (!_isFlashing) StartFlashing();
		TakeDamage();
	}

	public void ReactToBootsHit()
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
		_hp -= 1;
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
