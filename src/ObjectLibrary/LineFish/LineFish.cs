using Godot;
using System;

public partial class LineFish : Path2D
{
	[Export]
	PathFollow2D _pathFollow;

	float _speed = 0.2f;
	int _directionSign = 1;

	ILoggerService _logger;

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
	}

	public override void _Process(double delta)
	{
		ProcessPathFollow(_pathFollow, _speed, delta);
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
