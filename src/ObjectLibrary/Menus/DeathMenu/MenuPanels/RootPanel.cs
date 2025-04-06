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
	IPcRepo _pcRepo = new PcRepo();

	public override void _Ready()
	{
		_navigationAuthority = GetNode<NavigationAuthority>(Constants.SingletonNodes.NavigationAuthority);
		_crawlStatsService = GetNode<ICrawlStatsService>(Constants.SingletonNodes.CrawlStatsService);

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
		RefreshGamerLevelUi(false);

		base._Ready();
	}

	public void Init(MenuBusiness menuBusiness)
	{
		MenuBusiness = menuBusiness;
	}

	// TODO: make this tween between old xp level and new xp level
	// bool _firstRender = true;
	// public override void _PhysicsProcess(double _delta)
	// {
	// 	if (_firstRender)
	// 	{
	// 		RefreshGamerLevelUi(true);
	// 	}
	// 	_firstRender = false;
	// }
	
	public void RefreshGamerLevelUi(bool withTween)
	{
		int gameSaveId = _crawlStatsService.GameSave?.Id ?? -1;
		PcLevel pcLevel = _pcRepo.GetLevelData(gameSaveId);
		GamerLevelProgress.SetLeftLabel(string.Format(GAMER_LEVEL_TEMPLATE, pcLevel.Level.ToString()));
		GamerLevelProgress.UpdateMaxAndValue(
			pcLevel.TotalProteinNeededForNextLevel - pcLevel.TotalProteinNeededForCurrentLevel, 
			pcLevel.TotalProteinBanked - pcLevel.TotalProteinNeededForCurrentLevel, 
			withTween: withTween
		);
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
}
