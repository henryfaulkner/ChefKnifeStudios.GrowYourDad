using Godot;
using System;

public partial class IntroScene : Node2D
{
	[Export]
	TextBox TextBox { get; set; } = null!;
	
	const string EXPOSITION_1 = "Imagine, you’re a kid and you do kid stuff. You think you’ve got it good. A pond in your backyard and a mom and dad who love you very much.";
	const string EXPOSITION_2 = "Until you look a little closer at your dad, and you recognize tragedy!!! Your dad is not jacked at all. He’s small as fuck. No bicep veins, no lat spread, no big ol’ bitties. From now on, you’ve got a mission. To grow your dad…";
	
	ILoggerService _logger = null!;

	public override void _EnterTree()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);

		base._EnterTree();
	}
	
	public override void _Ready()
	{
		TextBox.AddDialogue(EXPOSITION_1);
		TextBox.AddDialogue(EXPOSITION_2);
		TextBox.ExecuteProcess();
		
		TextBox.TextBoxCompleted += HandleTextBoxCompleted;
	}
	
	void HandleTextBoxCompleted()
	{
		TextBox.HideTextBox();
	}
}
