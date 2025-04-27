using Godot;
using System;

public partial class IntroScene : Node2D
{
	[Export]
	IntroTextBox IntroTextBox { get; set; } = null!;

	// Apostrophe's have a weirdly negatice offset, which makes for stange line-spacing while rendering.
	// Works fine with Godoto default font. Bug occurs with: Helveti Pixel & Born2bSporty
	// I have removed apostrophe's from the dialogue for this reason.
	const string EXPOSITION_1 = "Imagine, you are a kid and you do kid stuff. You think you have it good. A pond in your backyard, and a mom and dad who love you very much.";
	const string EXPOSITION_2 = "Until you look a little closer at your dad, and you recognize tragedy!!! Your dad is not jacked at all. He is small as fuck. No bicep veins, no lat spread, no big ol bitties. From now on, you have a mission. [wave amp=25.0 freq=4.0 connected=1][b][color=yellow]To grow your dad...[/color][b][wave]";
	
	ILoggerService _logger = null!;
	NavigationAuthority _navigationAuthority = null!;

	public override void _EnterTree()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_navigationAuthority = GetNode<NavigationAuthority>(Constants.SingletonNodes.NavigationAuthority);

		base._EnterTree();
	}
	
	public override void _Ready()
	{
		IntroTextBox.HideCreateProfile();

		IntroTextBox.AddDialogue(EXPOSITION_1);
		IntroTextBox.AddDialogue(EXPOSITION_2);
		IntroTextBox.ExecuteProcess();
		
		IntroTextBox.TextBoxCompleted += HandleTextBoxCompleted;
		IntroTextBox.Submitted += HandleTextBoxSubmitted;
	}
	
	void HandleTextBoxCompleted()
	{
		IntroTextBox.HideTextBox();
		IntroTextBox.ShowCreateProfile();
	}

	void HandleTextBoxSubmitted()
	{
		// Use call_deferred to safely change the scene
		_navigationAuthority.CallDeferred("ChangeToPreActionLevel");
	}
}
