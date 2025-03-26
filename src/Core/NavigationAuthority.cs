using Godot;
using System;

public partial class NavigationAuthority : Node
{
    static readonly StringName PREACTION_LEVEL_PATH = new StringName("res://Pages/PreActionScene/PreActionScene.tscn");
	readonly PackedScene _preactionLevelScene;

    static readonly StringName ACTION_LEVEL_PATH = new StringName("res://Pages/ActionScene/ActionLevel.tscn");
	readonly PackedScene _actionLevelScene = null!;

    static readonly StringName UPGRADE_LEVEL_PATH = new StringName("res://Pages/UpgradeScene/UpgradeScene.tscn");
	readonly PackedScene _upgradeLevelScene;

    public NavigationAuthority()
	{
        _preactionLevelScene = (PackedScene)ResourceLoader.Load(PREACTION_LEVEL_PATH);
		_actionLevelScene = (PackedScene)ResourceLoader.Load(ACTION_LEVEL_PATH);
        _upgradeLevelScene = (PackedScene)ResourceLoader.Load(UPGRADE_LEVEL_PATH);
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
}