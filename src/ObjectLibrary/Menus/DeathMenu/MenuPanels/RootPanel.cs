using Godot;
using System;

public partial class RootPanel : TextButtonMenuPanel
{
	[Export]
	TextButton NewCrawlBtn { get; set; } = null!;
	[Export]
	TextButton GameSavesBtn { get; set; } = null!;
	[Export]
	TextButton ReturnToSurfaceBtn { get; set; } = null!;

	public MenuBusiness MenuBusiness { get; set; } = null!;
	public override int Id => (int)Enumerations.DeathMenuPanels.Root;

	const string OPTION_TEMPLATE = """
		[dropcap font_size=64 margins={0},0,0,0]
			{1}
		[/dropcap]
	""";
	const int MARGIN_INTERVAL = 18;

	string[] Texts = null!;
	Action[] SelectHandlers = null!;

	NavigationAuthority _navigationAuthority = null!;

	public override void _Ready()
	{
		_navigationAuthority = GetNode<NavigationAuthority>(Constants.SingletonNodes.NavigationAuthority);

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

		base._Ready();
	}

	public void Init(MenuBusiness menuBusiness)
	{
		MenuBusiness = menuBusiness;
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
