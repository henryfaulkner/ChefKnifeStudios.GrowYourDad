using Godot;
using System;

public partial class NavigationAuthority : Node
{
	static readonly StringName INTRO_PATH = new StringName("res://Pages/IntroScene/IntroScene.tscn");
	readonly PackedScene _introScene;

	static readonly StringName PREACTION_LEVEL_PATH = new StringName("res://Pages/PreActionScene/PreActionScene.tscn");
	readonly PackedScene _preactionLevelScene;

	static readonly StringName ACTION_LEVEL_PATH = new StringName("res://Pages/ActionScene/ActionLevel.tscn");
	readonly PackedScene _actionLevelScene;

	static readonly StringName UPGRADE_LEVEL_PATH = new StringName("res://Pages/UpgradeScene/UpgradeScene.tscn");
	readonly PackedScene _upgradeLevelScene;

	static readonly StringName DEATH_MENU_PATH = new StringName("res://Pages/DeathMenuScene/DeathMenuScene.tscn");
	readonly PackedScene _deathMenuScene;

	public NavigationAuthority()
	{
		_introScene = (PackedScene)ResourceLoader.Load(INTRO_PATH);
		_preactionLevelScene = (PackedScene)ResourceLoader.Load(PREACTION_LEVEL_PATH);
		_actionLevelScene = (PackedScene)ResourceLoader.Load(ACTION_LEVEL_PATH);
		_upgradeLevelScene = (PackedScene)ResourceLoader.Load(UPGRADE_LEVEL_PATH);
		_deathMenuScene = (PackedScene)ResourceLoader.Load(DEATH_MENU_PATH);
	}

	public void ChangeToIntro()
	{
		GetTree().ChangeSceneToPacked(_introScene);
	}

	public void ChangeToPreActionLevel()
	{
		GetTree().ChangeSceneToPacked(_preactionLevelScene);
	}

	public void ChangeToActionLevel()
	{
		GetTree().ChangeSceneToPacked(_actionLevelScene);
	}

	public void ChangeToUpgradeLevel()
	{
		GetTree().ChangeSceneToPacked(_upgradeLevelScene);
	}

	public void ChangeToDeathMenu()
	{
		GetTree().ChangeSceneToPacked(_deathMenuScene);
	}
}
