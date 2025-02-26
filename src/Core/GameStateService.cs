using Godot;
using System;

public interface IGameStateService
{
    int HpValue { get; set; }
    int HpMax { get; set; }
    int SpValue { get; set; }
    int SpMax { get; set; }
}

public partial class GameStateService : Node, IGameStateService 
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
            _hpValue = value;
            HandleHpValueChange(value);
        }
    }

    public int HpMax
    {
        get => _hpMax;
        set
        {
            _hpMax = value;
            HandleHpMaxChange(value);
        }
    }

    public int SpValue
    {
        get => _spValue;
        set
        {
            _spValue = value;
            HandleSpValueChange(value);
        }
    }

    public int SpMax
    {
        get => _spMax;
        set
        {
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

    public void HandleHpValueChange(int hpValue)
    {
        _observables.EmitUpdateHpMeterValue(hpValue);
    }

    public void HandleHpMaxChange(int hpMax)
    {
        _observables.EmitUpdateHpMeterMax(hpMax);
    }

    public void HandleSpValueChange(int spValue)
    {
        _observables.EmitUpdateSpMeterValue(spValue);
    }

    public void HandleSpMaxChange(int spMax)
    {
        _observables.EmitUpdateSpMeterMax(spMax);
    }
}