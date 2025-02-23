using Godot;
using System;
using System.Collections.Generic;

public partial class PathFindingFish : Agent, IBlasterTarget
{
	// Use this for tilemapping stuff
	// https://www.youtube.com/watch?v=qIqcp7xBGkw

	[ExportGroup("Nodes")]
	[Export]
	Node2D _rayCastContainer { get; set; }
	[Export]
	TargetArea2D _hitBox;

	[ExportGroup("Variables")]
	[Export]
	float _castRadius = 90.0f;
	[Export]
	int _numCasts = 25;

	List<RayCast2D> _rayCastList = new();
	Node2D? _navTarget;

	[Export]
	float _flashDuration = 1.0f; // Total duration of the flashing effect
	[Export]
	float _flashInterval = 0.2f; // Time between flashes

	[Export]
	int _hp = 2;

	ILoggerService _logger;
	bool _isFlashing = false;

	#region State Machine
	States _state;
	enum States 
	{
		Searching,
		Approaching,
	}
	#endregion

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);

		ReadyAgent();

		_state = States.Searching;

		// Draw a circle of raycasts around the origin of the fish
		// Use as detection mechanism
		var castTargets = CircleHelper.GetCirclePoints(GlobalPosition, _castRadius, _numCasts);

		// Center the points
		CircleHelper.TranslateListOfVectors(ref castTargets, -GlobalPosition);

		_rayCastList = CreateRayCastList(castTargets);
		foreach (var rayCast in _rayCastList)
		{
			_rayCastContainer.AddChild(rayCast);
		}

		_hitBox.TargetHit += HandleHit;
	}

	public override void _PhysicsProcess(double delta)
	{
		SyncChildPositionsToController();
		
		_navTarget = DetectPlayerCharacter(_rayCastList);

		if (_state == States.Searching && _navTarget != null)
		{
			// If raycasts detects PC, target PC with NavAgent and set state to Approaching
			SetNavTarget(_navTarget);
			_state = States.Approaching;
		} 

		if (_state == States.Approaching && _navTarget == null)
		{
			// If raycasts do not detect PC, remove target from NavAgent and set state to Searching
			SetNavTarget(_navTarget);
			_state = States.Searching;
		}
	}

	public override void HandleNavTargetArrival()
	{
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

	void SyncChildPositionsToController()
	{
		Vector2 position = Controller.GlobalPosition;
		_rayCastContainer.GlobalPosition = position;
	}

	static Node2D? DetectPlayerCharacter(List<RayCast2D> rayCastList)
	{
		// Check if raycasts collided with PC
		Node2D? result = null;
		foreach (var rayCast in rayCastList)
		{
			object? collider = rayCast.GetCollider();
			if (collider != null) // Is colliding, if not null
			{
				if (collider.GetType().BaseType == typeof(CharacterBody2D))
				{
					var colliderParent = ((CharacterBody2D)collider).GetParent();
					if (colliderParent.GetType() == typeof(PC))
					{
						result = (CharacterBody2D)collider;
						break;
					}
				}
			}
		}
		return result;
	}

	static List<RayCast2D> CreateRayCastList(List<Vector2> targetPoints)
	{
		List<RayCast2D> result = new();
		foreach (var targetPoint in targetPoints)
		{
			RayCast2D raycast = new();
			raycast.Position = Vector2.Zero;
			raycast.TargetPosition = targetPoint;
			result.Add(raycast);
		}
		return result;
	}
}
