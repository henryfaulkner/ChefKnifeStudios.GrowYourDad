using Godot;
using System;

public partial class TextureButton : Godot.TextureButton
{
	// private int FxBusIndex { get; set; }
	// private SaveStateService _serviceSaveState { get; set; }
	// private bool IsInitializePress { get; set; }

	// public override void _Ready()
	// {
	// 	FxBusIndex = AudioServer.GetBusIndex("Fx");
	// 	_serviceSaveState = GetNode<SaveStateService>("/root/SaveStateService");
	// 	IsInitializePress = true;
	// 	var context = _serviceSaveState.Load();
	// 	if (context.UserSettings.FxMuted) ButtonPressed = true;
	// 	EmitSignal(SignalName.Pressed);
	// }

	// public void _on_pressed()
	// {
	// 	//GD.Print("Mute button pressed.");
	// 	var context = _serviceSaveState.Load();
	// 	if (IsInitializePress)
	// 	{
	// 		//GD.Print("IsInitialized");
	// 		AudioServer.SetBusMute(FxBusIndex, context.UserSettings.FxMuted);
	// 		IsInitializePress = false;
	// 	}
	// 	else
	// 	{
	// 		//GD.Print("!IsInitialized");
	// 		AudioServer.SetBusMute(FxBusIndex, !AudioServer.IsBusMute(FxBusIndex));
	// 		context.UserSettings.FxMuted = AudioServer.IsBusMute(FxBusIndex);
	// 		_serviceSaveState.Commit(context);
	// 	}
	// 	//GD.Print($"context.UserSettings.FxMuted: {context.UserSettings.FxMuted}");
	// }
}
