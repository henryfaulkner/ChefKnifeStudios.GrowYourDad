public partial class CrawlStats
{
	const int DEPTH_PER_FLOOR = 15; 

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
