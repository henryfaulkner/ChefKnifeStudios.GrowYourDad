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
	private int _hpValue;
	private int _hpMax;
	private int _spValue;
	private int _spMax;

	public int HpValue
	{
		get => _hpValue;
		set
		{
			if (_hpValue == value) return;
			if (value < 0) value = 0;
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

	ILoggerService _logger;
	Observables _observables;

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_observables = GetNode<Observables>(Constants.SingletonNodes.Observables);
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
}
