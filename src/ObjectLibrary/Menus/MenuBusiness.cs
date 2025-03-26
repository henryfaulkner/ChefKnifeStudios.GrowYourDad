using System.Collections.Generic;
using Godot;

public partial class MenuBusiness : Node
{
	public Stack<IMenuPanel> WorkingDirectory { get; } = new();

	public void PushPanel(IMenuPanel priorPanel)
	{
		WorkingDirectory.Push(priorPanel);
		return;
	}

	public IMenuPanel PopPanel()
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
