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
	[Signal]
	public delegate void RefreshUIEventHandler();

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
			EmitSignal(SignalName.RefreshUI);
		}
	}

	CrawlStats _crawlStats = new();
	public CrawlStats CrawlStats
	{
		get => _crawlStats;
		set 
		{
			_crawlStats = value;
			EmitSignal(SignalName.RefreshUI);
		}
	}

	public void PersistCrawlStats()
	{
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
	}

	void ClearCrawlStats()
	{
		_crawlStats = new();
	}
}
