using System.Collections.Generic;
using Godot;

public partial class MenuBusiness : Node
{
	public Stack<BaseMenuPanel> WorkingDirectory { get; set; } = new Stack<BaseMenuPanel>();
	public List<BaseMenuPanel> PanelList { get; set; } = new List<BaseMenuPanel>();

	public void SetPanelList(List<BaseMenuPanel> panelList)
	{
		PanelList = panelList;
	}

	public void PushPanel(BaseMenuPanel priorPanel)
	{
		WorkingDirectory.Push(priorPanel);
		return;
	}

	public BaseMenuPanel PopPanel()
	{
		return WorkingDirectory.Pop();
	}

	#region Signals
	[Signal]
	public delegate void OpenMenuEventHandler();
	public void EmitOpenMenu()
	{
		EmitSignal(SignalName.OpenMenu);
	}

	[Signal]
	public delegate void CloseMenuEventHandler();
	public void EmitCloseMenu()
	{
		EmitSignal(SignalName.CloseMenu);
	}
	#endregion
}
