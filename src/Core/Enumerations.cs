using System.ComponentModel;

public partial class Enumerations
{
	public enum LogLevels
	{
		[Description("Debug")]
		Debug = 0,
		[Description("Info")]
		Info = 1,
		[Description("Error")]
		Error = 2,
	}

	public enum BlasterTypes
	{
		[Description("Single-Shot Blaster")]
		SingleShotBlaster = 1,
		[Description("Triple-Shot Blaster")]
		TripleShotBlaster = 2,
	}
	
	public enum PcAreas
	{
		Body,
		Blast,
		Boots,
	}

	public enum PauseMenuPanels
	{
		Main,
		ShopKeeper,
		GameSave,
		CrawlStatsHistory,
	}

	public enum DeathMenuPanels
	{
		Root,
		GameSaves,
	}
}
