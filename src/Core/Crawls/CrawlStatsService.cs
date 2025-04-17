using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public interface ICrawlStatsService
{
	GameSave GameSave { get; set; }
	CrawlStats CrawlStats { get; }
	void EmitRefreshUI();

	void PersistCrawlStats();
	IEnumerable<CrawlStatsHistory> GetCrawlStatsHistory();
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
			CrawlStats.GameSaveId = value?.Id;
			CrawlStats.GameSave = value;
			EmitSignal(SignalName.RefreshUI);
		}
	}

	CrawlStats _crawlStats = new(null);
	public CrawlStats CrawlStats
	{
		get => _crawlStats;
		set 
		{
			_crawlStats = value;
			EmitSignal(SignalName.RefreshUI);
		}
	}

	public void EmitRefreshUI()
	{
		EmitSignal(SignalName.RefreshUI);
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

	public IEnumerable<CrawlStatsHistory> GetCrawlStatsHistory()
	{
		var includeFunc = new Func<IQueryable<CrawlStats>, IQueryable<CrawlStats>>[]
		{
			query => query.Include(e => e.GameSave)
		};
		return _unitOfWork.CrawlStatsRepository
			.GetAllIncludes(includeFunc)
			.OrderByDescending(x => x.Id)
			.Select(x => new CrawlStatsHistory()
			{
				Gamer = x.GameSave?.Username ?? "Anonymous",
				FloorDepth = x.CrawlDepth_ToString(),
				ProteinCollected = x.ProteinsCollected.ToString(),
				FoesDefeated = x.FoesDefeated.ToString(),	
			});
	}

	void ClearCrawlStats()
	{
		CrawlStats = new(GameSave);
	}
}
