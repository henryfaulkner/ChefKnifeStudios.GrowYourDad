using Godot;
using System;
using System.Threading.Tasks;

public interface ICrawlStatsService
{
	GameSave GameSave { get; set; }
	CrawlStats CrawlStats { get; }

	void PersistCrawlStats();
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
	
	GameSave? _gameSave = null!;
	public GameSave? GameSave 
	{ 
		get => _gameSave; 
		set
		{
			_gameSave = value;
			CrawlStats.GameSaveId = _gameSave?.Id;
		}
	}

	CrawlStats _crawlStats = new();
	public CrawlStats CrawlStats
	{
		get => _crawlStats;
		set => _crawlStats = value;
	}

	public void PersistCrawlStats()
	{
		_logger.LogInfo("Start PersistCrawlStats");
		try
		{
			_unitOfWork.CrawlStatsRepository.Add(_crawlStats);
			_unitOfWork.SaveChanges();
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
		_logger.LogInfo("End PersistCrawlStats");
	}

	void ClearCrawlStats()
	{
		_crawlStats = new();
	}
}
