using System;
using System.Threading.Tasks;

public interface IUnitOfWork : IDisposable
{
	//IRepository<Log> LogRepository { get; }

	Task<bool> SaveChangesAsync();
}

public class UnitOfWork : IUnitOfWork
{
	private readonly AppDbContext _context;

	//public IRepository<Log> LogRepository { get; }

	public UnitOfWork(AppDbContext context)
	{
		_context = context;

		//LogRepository = new Repository<Log>(_context);
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
}
