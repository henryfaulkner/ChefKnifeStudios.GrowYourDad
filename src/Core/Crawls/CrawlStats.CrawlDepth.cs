public partial class CrawlStats
{
	public CrawlStats(GameSave? gameSave)
	{
		GameSave = gameSave;
		GameSaveId = gameSave?.Id;
	}

	const int DEPTH_PER_FLOOR = 1; 

	public void IncrementCrawlDepth()
	{
		DepthOnFloor += 1;
		if (DepthOnFloor > DEPTH_PER_FLOOR) 
		{
			DepthOnFloor = 1;
			FloorNumber += 1;
		}
	}

	public string CrawlDepth_ToString()
	{
		return $"{FloorNumber}-{DepthOnFloor}";
	}
}
