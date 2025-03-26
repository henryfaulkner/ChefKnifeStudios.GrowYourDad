using Godot;
using System;
using System.Threading.Tasks;

public interface IPcMeterService
{
	int HpValue { get; set; }
	int HpMax { get; set; }
	int SpValue { get; set; }
	int SpMax { get; set; }
}

public partial class PcMeterService : GameStateSingletonBase, IPcMeterService 
{
	const int INITIAL_HP = 3;
	const int INITIAL_SP = 3;

	private int _hpValue = INITIAL_HP;
	private int _hpMax = INITIAL_HP;
	private int _spValue = INITIAL_SP;
	private int _spMax = INITIAL_SP;

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

	ILoggerService _logger = null!;
	Observables _observables = null!;
	IPcInventoryService _pcInventoryService = null!;
	IPcWalletService _pcWalletService = null!;
	ICrawlStatsService _crawlStatsService = null!;
	NavigationAuthority _navigationAuthority = null!;

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_observables = GetNode<Observables>(Constants.SingletonNodes.Observables);
		_pcInventoryService = GetNode<IPcInventoryService>(Constants.SingletonNodes.PcInventoryService);
		_pcWalletService = GetNode<IPcWalletService>(Constants.SingletonNodes.PcWalletService);
		_crawlStatsService = GetNode<ICrawlStatsService>(Constants.SingletonNodes.CrawlStatsService);
		_navigationAuthority = GetNode<NavigationAuthority>(Constants.SingletonNodes.NavigationAuthority);
	}

    public override void Clear()
    {
        HpValue = INITIAL_HP;
        HpMax = INITIAL_HP;
        SpValue = INITIAL_SP;
        SpMax = INITIAL_SP;
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

	void HandleDeath()
	{
		_crawlStatsService.CrawlStats.ProteinsBanked = _pcWalletService.ProteinInWallet;
		_crawlStatsService.CrawlStats.ItemsCollected = _pcInventoryService.CountInventory();
		_crawlStatsService.PersistCrawlStats();

		_observables.EmitRestartCrawl();

		// Use call_deferred to safely change the scene
		_navigationAuthority.CallDeferred("ChangeToPreActionLevel");
	}
}
