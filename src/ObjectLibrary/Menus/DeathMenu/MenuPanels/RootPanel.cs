using Godot;
using System;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public partial class RootPanel : TextButtonMenuPanel
{
	[Export]
	Label GamerLevelPanel { get; set; } = null!;
	[Export]
	SmootheMeter GamerLevelProgress { get; set; } = null!;

	[Export]
	TextButton NewCrawlBtn { get; set; } = null!;
	[Export]
	TextButton GameSavesBtn { get; set; } = null!;
	[Export]
	TextButton ReturnToSurfaceBtn { get; set; } = null!;

	public MenuBusiness MenuBusiness { get; set; } = null!;
	public override int Id => (int)Enumerations.DeathMenuPanels.Root;

	const string GAMER_LEVEL_TEMPLATE = """
		Level {0}
	""";

	const string OPTION_TEMPLATE = """
		[dropcap font_size=64 margins={0},0,0,0]
			{1}
		[/dropcap]
	""";
	const int MARGIN_INTERVAL = 18;

	string[] Texts = null!;
	Action[] SelectHandlers = null!;

	NavigationAuthority _navigationAuthority = null!;
	ICrawlStatsService _crawlStatsService = null!;
	Observables _observables = null!;
	IPcInventoryService _pcInventoryService = null!;
	IPcWalletService _pcWalletService = null!;
	ILoggerService _logger = null!;
	IPcRepo _pcRepo = new PcRepo();

	Queue<Action> _meterActionQueue = [];

	LevelXpProgressMarker? _previousLevelMarker;
	LevelXpProgressMarker? _nextLevelMarker;

	public override void _Ready()
	{
		_navigationAuthority = GetNode<NavigationAuthority>(Constants.SingletonNodes.NavigationAuthority);
		_crawlStatsService = GetNode<ICrawlStatsService>(Constants.SingletonNodes.CrawlStatsService);
		_observables = GetNode<Observables>(Constants.SingletonNodes.Observables);
		_pcInventoryService = GetNode<IPcInventoryService>(Constants.SingletonNodes.PcInventoryService);
		_pcWalletService = GetNode<IPcWalletService>(Constants.SingletonNodes.PcWalletService);
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);

		Controls = [ NewCrawlBtn, GameSavesBtn, ReturnToSurfaceBtn ];
		SelectHandlers = [ HandleNewCrawlSelect, HandleGameSaveSelect, HandleReturnToSurfaceSelect ];
		Texts = [ "TAKE ANOTHER DIVE", "SAVE GAME", "RETURN TO SURFACE" ];

		for (int i = 0; i < Controls.Count; i += 1)
		{
			var focusIndex = i; 
			var textBtn = Controls[i];
			textBtn.HandleSelectCallback = SelectHandlers[i];
			textBtn.RequestFocus += () => MoveFocusTarget(focusIndex); 
			textBtn.ForegroundLabel.Text = string.Format(OPTION_TEMPLATE, MARGIN_INTERVAL * i, Texts[i]);
			textBtn.BackgroundLabel.Text = string.Format(OPTION_TEMPLATE, MARGIN_INTERVAL * i, Texts[i]);
		}

		MoveFocusTarget(0);
		
		int gameSaveId = _crawlStatsService.GameSave?.Id ?? -1;
		PcLevel pcLevel = _pcRepo.GetLevelData(gameSaveId);
		_previousLevelMarker = new(pcLevel);

		_crawlStatsService.CrawlStats.ProteinsBanked = _pcWalletService.ProteinInWallet;
		_crawlStatsService.CrawlStats.ItemsCollected = _pcInventoryService.CountInventory();
		_crawlStatsService.PersistCrawlStats();
		_observables.EmitRestartCrawl();

		pcLevel = _pcRepo.GetLevelData(gameSaveId);
		_nextLevelMarker = new(pcLevel);

		GamerLevelProgress.TweenFinishedCallback = HandleTweenFinished;
		GamerLevelProgress.UpdateMaxAndValueLabels(
			pcLevel.TotalProteinNeededForNextLevel, 
			pcLevel.TotalProteinBanked
		);

		base._Ready();
	}

	public void Init(MenuBusiness menuBusiness)
	{
		MenuBusiness = menuBusiness;
	}

	bool _firstRender = true;
	public override void _PhysicsProcess(double delta)
	{
		if (_firstRender)
		{
			if (_previousLevelMarker == null ) _logger.LogError("_previousLevelMarker is null");
			if (_nextLevelMarker == null ) _logger.LogError("_nextLevelMarker is null");
			TweenGamerBetweenLevels(_previousLevelMarker!, _nextLevelMarker!);
		}
		_firstRender = false;

		base._PhysicsProcess(delta);
	}

	void TweenGamerBetweenLevels(LevelXpProgressMarker previousLevelMarker, LevelXpProgressMarker nextLevelMarker)
	{
		// pass previousLevelMarker and nextLevelMarker to GamerLevelProgress
		for (int i = previousLevelMarker.Level; i <= nextLevelMarker.Level; i += 1)
		{
			if (i == 0) continue;
			int index = i;
			_meterActionQueue.Enqueue(() => 
				{
	 				GamerLevelProgress.SetLeftLabel(string.Format(GAMER_LEVEL_TEMPLATE, index.ToString()));
					GamerLevelProgress.UpdateMaxAndValue(100, 0, withTween: false, updateLabels: false);
					if (index == nextLevelMarker.Level)
					{
						GamerLevelProgress.UpdateMaxAndValue(100, nextLevelMarker.XpRatio, withTween: true, updateLabels: false);
					}
					else if (i == previousLevelMarker.Level)
					{
						GamerLevelProgress.UpdateMaxAndValue(100, previousLevelMarker.XpRatio, withTween: true, updateLabels: false);
					}
					else
					{
						GamerLevelProgress.UpdateMaxAndValue(100, 100, withTween: true, updateLabels: false);
					}
				}
			);
		}
		if (_meterActionQueue.TryDequeue(out var action))
		{
			action.Invoke(); 
		} 
	}

	void HandleNewCrawlSelect() 
	{
		_navigationAuthority.CallDeferred("ChangeToActionLevel");
	}

	void HandleGameSaveSelect() 
	{
		MenuBusiness.PushPanel(this);
		EmitSignal(SignalName.Open, (int)Enumerations.DeathMenuPanels.GameSaves);
	}
	
	void HandleReturnToSurfaceSelect() 
	{
		_navigationAuthority.CallDeferred("ChangeToPreActionLevel");
	}

	void HandleTweenFinished()
	{
		if (_meterActionQueue.TryDequeue(out var action))
		{
			action.Invoke(); 
		} 
	}
}

public class LevelXpProgressMarker
{
	public LevelXpProgressMarker(PcLevel pcLevel)
	{
		Level = pcLevel.Level;
		if (pcLevel.Level > 0)
			XpRatio = (int)((double)pcLevel.TotalProteinBanked / (double)pcLevel.TotalProteinNeededForNextLevel * 100);
		else 
			XpRatio = 0;
	}

	// Full levels
	public int Level { get; init; }
	// Amount 0-100 accomplished to reaching next Level
	public int XpRatio { get; init; }

	public override string ToString()
	{
		return $"Level: {Level}; XpRatio: {XpRatio};";
	}

}
