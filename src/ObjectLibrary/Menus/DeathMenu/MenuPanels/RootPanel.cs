using Godot;
using System;

public partial class RootPanel : BaseMenuPanel
{
    const string OPTION_TEMPLATE = """
		[dropcap font_size=64 margins={0},0,0,0]
			{1}
		[/dropcap]
	""";
	const int MARGIN_INTERVAL = 18;

	static string[] Options = [ "RESUME", "GAME SAVES", "Return To Surface" ];
	Action[] SelectHandlers = null!;

	public RootPanel()
	{
		SelectHandlers = [ HandleResumeSelect, HandleGameSaveSelect, HandleReturnSelect ];
	}

    public override void _Ready()
    {
        for (int i = 0; i < Options.Length; i += 1)
		{
			Control control = new();
			RichTextLabel label = new();
			AddChild(control);
			control.AddChild(label);
			label.Text = string.Format(OPTION_TEMPLATE, MARGIN_INTERVAL * i, Options[i]);
		}
    }

    void HandleResumeSelect() {}
	void HandleGameSaveSelect() {}
	void HandleReturnSelect() {}


}