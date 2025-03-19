public class CrawlDepth
{
	const int DEPTH_PER_FLOOR = 15; 

	public int FloorNumber { get; set; } = 1;
	public int DepthOnFloor { get; set; } = 1;

	public void IncrementCrawlDepth()
	{
		DepthOnFloor += 1;
		if (DepthOnFloor > DEPTH_PER_FLOOR) 
		{
			DepthOnFloor = 1;
			FloorNumber += 1;
		}
	}

	public override string ToString()
	{
		return $"{FloorNumber}-{DepthOnFloor}";
	}
}