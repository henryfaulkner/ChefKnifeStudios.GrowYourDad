using Godot;
using System;
using System.Threading.Tasks;

public interface IUnitOfWork : IDisposable
{
	IRepository<CrawlStats> CrawlStatsRepository { get; }

	Task<bool> SaveChangesAsync();
}

public partial class UnitOfWork : Node, IUnitOfWork
{
	private readonly AppDbContext _context;

	public IRepository<CrawlStats> CrawlStatsRepository { get; private set; }

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

	public async Task<bool> SaveChangesAsync()
	{
		var numberOfEntries = await _context.SaveChangesAsync();
		return numberOfEntries > 0;
	}

	public void Dispose()
	{
		_context.Dispose();
	}

	void RegisterRepositories(AppDbContext context)
	{
		CrawlStatsRepository = new Repository<CrawlStats>(_context);
	}
}
