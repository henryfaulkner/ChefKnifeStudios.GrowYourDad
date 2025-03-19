using Godot;
using System;
using System.Threading.Tasks;

public interface ICrawlStatsService
{
	CrawlStats CrawlStats { get; }

	Task PersistCrawlStatsAsync();
} 

public partial class CrawlStatsService : Node, ICrawlStatsService
{
	IUnitOfWork _unitOfWork = null!;
	ILoggerService _logger = null!;

	public override void _Ready()
	{
		_unitOfWork = GetNode<IUnitOfWork>(Constants.SingletonNodes.UnitOfWork);
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
	}

	CrawlStats _crawlStats = new();
	public CrawlStats CrawlStats
	{
		get => _crawlStats;
		set => _crawlStats = value;
	}

	public async Task PersistCrawlStatsAsync()
	{
		_logger.LogInfo("Start PersistCrawlStatsAsync");
		try
		{
			await _unitOfWork.CrawlStatsRepository.AddAsync(_crawlStats);
			await _unitOfWork.SaveChangesAsync();
			ClearCrawlStats();
		} 
		catch (Exception ex)
		{
			var innerException = ex;
			while (innerException.InnerException != null)
			{
				innerException = innerException.InnerException;
			}
			_logger.LogError($"Inner exception: {innerException.Message}");
			throw;
		}
		_logger.LogInfo("End PersistCrawlStatsAsync");
	}

	void ClearCrawlStats()
	{
		_crawlStats = new();
	}
}
