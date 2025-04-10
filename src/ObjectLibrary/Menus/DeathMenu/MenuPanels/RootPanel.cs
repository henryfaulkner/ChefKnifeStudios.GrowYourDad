using Godot;
using System;

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
	IPcRepo _pcRepo = new PcRepo();

	public override void _Ready()
	{
		_navigationAuthority = GetNode<NavigationAuthority>(Constants.SingletonNodes.NavigationAuthority);
		_crawlStatsService = GetNode<ICrawlStatsService>(Constants.SingletonNodes.CrawlStatsService);
		_observables = GetNode<Observables>(Constants.SingletonNodes.Observables);
		_pcInventoryService = GetNode<IPcInventoryService>(Constants.SingletonNodes.PcInventoryService);
		_pcWalletService = GetNode<IPcWalletService>(Constants.SingletonNodes.PcWalletService);

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
		PcLevel level = _pcRepo.GetLevelData(gameSaveId);
		LevelXpProgressMarker previousLevelMarker = new(level);

		_crawlStatsService.CrawlStats.ProteinsBanked = _pcWalletService.ProteinInWallet;
		_crawlStatsService.CrawlStats.ItemsCollected = _pcInventoryService.CountInventory();
		_crawlStatsService.PersistCrawlStats();
		_observables.EmitRestartCrawl();

		level = _pcRepo.GetLevelData(gameSaveId);
		LevelXpProgressMarker nextLevelMarker = new(level);

		


		base._Ready();
	}

	public void Init(MenuBusiness menuBusiness)
	{
		MenuBusiness = menuBusiness;
	}

	void TweenGamerBetweenLevels(LevelXpProgressMarker previousLevelMarker, LevelXpProgressMarker nextLevelMarker)
	{
		// pass previousLevelMarker and nextLevelMarker to GamerLevelProgress
	}

	//TODO: make this tween between old xp level and new xp level
	// bool _firstRender = true;
	// public override void _PhysicsProcess(double delta)
	// {
	// 	if (_firstRender)
	// 	{
	// 		RefreshGamerLevelUi(true);
	// 	}
	// 	_firstRender = false;

	// 	base._PhysicsProcess(delta);
	// }
	
	// public void RefreshGamerLevelUi(bool withTween)
	// {
	// 	int gameSaveId = _crawlStatsService.GameSave?.Id ?? -1;
	// 	PcLevel pcLevel = _pcRepo.GetLevelData(gameSaveId);
	// 	GamerLevelProgress.SetLeftLabel(string.Format(GAMER_LEVEL_TEMPLATE, pcLevel.Level.ToString()));
	// 	GamerLevelProgress.UpdateMaxAndValue(
	// 		pcLevel.TotalProteinNeededForNextLevel - pcLevel.TotalProteinNeededForCurrentLevel, 
	// 		withTween ? pcLevel.TotalProteinBanked - pcLevel.TotalProteinNeededForCurrentLevel : 0, 
	// 		withTween: withTween
	// 	);
	// }

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
}

public class LevelXpProgressMarker
{
	public LevelXpProgressMarker(PcLevel pcLevel)
	{
		// int level, int proteinNeededForNextLevel, int proteinNeededForCurrentLevel, int proteinBanked
		Level = pcLevel.Level;
		XpRatio = (pcLevel.TotalProteinBanked-pcLevel.TotalProteinNeededForCurrentLevel) 
			/ (pcLevel.TotalProteinNeededForNextLevel-pcLevel.TotalProteinNeededForCurrentLevel);
	}

	// Full levels
	public int Level { get; init; }
	// Amount 0-100 accomplished to reaching next Level
	public int XpRatio { get; init; }
}
