using Godot;
using System;
using System.Threading.Tasks;

public interface IUnitOfWork : IDisposable
{
	IRepository<GameSave> GameSaveRepository { get; }
	IRepository<CrawlStats> CrawlStatsRepository { get; }

	bool SaveChanges();
}

public partial class UnitOfWork : Node, IUnitOfWork
{
	private readonly AppDbContext _context;

	public IRepository<GameSave> GameSaveRepository { get; private set; } = null!;
	public IRepository<CrawlStats> CrawlStatsRepository { get; private set; } = null!;

	public UnitOfWork()
	{
		var context = new AppDbContext();
		
		_context = context;
		RegisterRepositories(context);
	}

	public UnitOfWork(AppDbContext context)
	{
		_context = context;
		RegisterRepositories(context);
	}

	public bool SaveChanges()
	{
		var numberOfEntries = _context.SaveChanges();
		return numberOfEntries > 0;
	}

	public void Dispose()
	{
		_context.Dispose();
	}

	void RegisterRepositories(AppDbContext context)
	{
		GameSaveRepository = new Repository<GameSave>(_context);
		CrawlStatsRepository = new Repository<CrawlStats>(_context);
	}
}
