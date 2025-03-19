using Godot;
using System;

public interface IPcMeterService
{
	int HpValue { get; set; }
	int HpMax { get; set; }
	int SpValue { get; set; }
	int SpMax { get; set; }
}

public partial class PcMeterService : Node, IPcMeterService 
{
	private int _hpValue = -1;
	private int _hpMax = -1;
	private int _spValue = -1;
	private int _spMax = -1;

	public int HpValue
	{
		get => _hpValue;
		set
		{
			if (_hpValue == value) return;
			if (value < 0) value = 0;
			if (value == 0) HandleDeath();
			_hpValue = value;
			HandleHpValueChange(value);
		}
	}

	public int HpMax
	{
		get => _hpMax;
		set
		{
			if (_hpMax == value) return;
			if (value < 0) value = 0;
			_hpMax = value;
			HandleHpMaxChange(value);
		}
	}

	public int SpValue
	{
		get => _spValue;
		set
		{
			if (_spValue == value) return;
			if (value < 0) value = 0;
			_spValue = value;
			HandleSpValueChange(value);
		}
	}

	public int SpMax
	{
		get => _spMax;
		set
		{
			if (_spMax == value) return;
			if (value < 0) value = 0;
			_spMax = value;
			HandleSpMaxChange(_spMax);
		}
	}

	static readonly StringName PREACTION_LEVEL_PATH = new StringName("res://Pages/PreActionScene/PreActionScene.tscn");
	readonly PackedScene _preactionLevelScene;

	ILoggerService _logger = null!;
	Observables _observables = null!;
	IPcInventoryService _pcInventoryService = null!;
	IPcWalletService _pcWalletService = null!;

	public PcMeterService()
	{
		_preactionLevelScene = (PackedScene)ResourceLoader.Load(PREACTION_LEVEL_PATH);
	}

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_observables = GetNode<Observables>(Constants.SingletonNodes.Observables);
		_pcInventoryService = GetNode<IPcInventoryService>(Constants.SingletonNodes.PcInventoryService);
		_pcWalletService = GetNode<IPcWalletService>(Constants.SingletonNodes.PcWalletService);
	}

	void HandleHpValueChange(int hpValue)
	{
		_observables.EmitUpdateHpMeterValue(hpValue);
	}

	void HandleHpMaxChange(int hpMax)
	{
		_observables.EmitUpdateHpMeterMax(hpMax);
	}

	void HandleSpValueChange(int spValue)
	{
		_observables.EmitUpdateSpMeterValue(spValue);
	}

	void HandleSpMaxChange(int spMax)
	{
		_observables.EmitUpdateSpMeterMax(spMax);
	}

	private void HandleDeath()
	{
		_pcInventoryService.Clear();
		_pcWalletService.ProteinInWallet = 0;

		// Use call_deferred to safely change the scene
		CallDeferred(nameof(ChangeToActionLevel));
	}

	void ChangeToActionLevel()
	{
		GetTree().ChangeSceneToPacked(_preactionLevelScene);
	}
}
