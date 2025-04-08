using Godot;
using System;
using System.Collections.Generic;

public partial class PathFindingFish : Agent, IEnemy
{
	// Use this for tilemapping stuff
	// https://www.youtube.com/watch?v=qIqcp7xBGkw

	[ExportGroup("Nodes")]
	[Export]
	CharacterBody2D _controller = null!;
	[Export]
	Node2D _rayCastContainer = null!;
	[Export]
	EnemyHurtBoxArea _hurtBox = null!;
	[Export]
	EnemyHitBoxArea _hitBox = null!;

	[ExportGroup("Variables")]
	[Export]
	float _castRadius = 360.0f;
	[Export]
	int _numCasts = 75;

	List<RayCast2D> _rayCastList = new();
	Node2D? _navTarget;

	[Export]
	float _flashDuration = 1.0f; // Total duration of the flashing effect
	[Export]
	float _flashInterval = 0.2f; // Time between flashes

	[Export]
	int _hp = 2;

	ILoggerService _logger = null!;
	IPcInventoryService _pcInventoryService = null!;
	IProteinFactory _proteinFactory = null!;
	Observables _observables = null!;
	ICrawlStatsService _crawlStatsService = null!;

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
		_pcInventoryService = GetNode<IPcInventoryService>(Constants.SingletonNodes.PcInventoryService);
		_proteinFactory = GetNode<IProteinFactory>(Constants.SingletonNodes.ProteinFactory);
		_observables = GetNode<Observables>(Constants.SingletonNodes.Observables);
		_crawlStatsService = GetNode<ICrawlStatsService>(Constants.SingletonNodes.CrawlStatsService);
		
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

		_hurtBox.AreaHurt += HandleHurt;
		_hitBox.AreaHit += HandleHit;
	}

	public override void _PhysicsProcess(double delta)
	{
		SyncChildPositionsToController();
		
		_navTarget = DetectPlayerCharacter(_rayCastList);

		if (_state == States.Searching && _navTarget != null)
		{
			// If raycasts detects PC, target PC with NavAgent and set state to Approaching
			_logger.LogDebug("raycasts detects PC, target PC with NavAgent and set state to Approaching");
			SetNavTarget(_navTarget);
			_state = States.Approaching;
		} 

		if (_state == States.Approaching && _navTarget == null)
		{
			// If raycasts does not detect PC, remove target from NavAgent and set state to Searching
			_logger.LogDebug("raycasts does not detect PC, remove target from NavAgent and set state to Searching");
			SetNavTarget(_navTarget);
			_state = States.Searching;
		}
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

	public override void HandleNavTargetArrival()
	{
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
				_logger.LogError("PathFindingFish HandleHit did not map properly.");
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
				_logger.LogError("PathFindingFish HandleHurt did not map properly.");
				break;
		}
	}

	void ReactToPcHit()
	{
		int damageConstant = 1;
		_observables.EmitPcHit(damageConstant);
	}
	
	public void ReactToBlastHurt()
	{
		if (!_isFlashing) StartFlashing();
		TakeDamage();
	}

	public void ReactToBootsHurt()
	{
		HandleDeath();
	}

	async void StartFlashing()
	{
		_isFlashing = true;
		var originalColor = Modulate;
		var flashColor = new Color(1.0f, 0.0f, 0.0f, 1.0f); // solid red

		float elapsed = 0.0f;

		while (elapsed < _flashDuration)
		{
			Modulate = (Modulate == originalColor) ? flashColor : originalColor;
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
		_crawlStatsService.CrawlStats.FoesDefeated += 1;
		_proteinFactory.SpawnMultiProtein(GetNode(".."), _controller.GlobalPosition);
		QueueFree();
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
