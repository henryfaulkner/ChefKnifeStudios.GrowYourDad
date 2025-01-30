using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public interface IRepository<T> where T : class
{
	Task<T> GetByIdAsync(int id);
	Task<T> GetByIdIncludesAsync(int id, params Func<IQueryable<T>, IQueryable<T>>[] includes);
	Task<IEnumerable<T>> GetAllAsync();
	Task<IEnumerable<T>> GetAllIncludesAsync(params Func<IQueryable<T>, IQueryable<T>>[] includes);
	Task AddAsync(T entity);
	Task AddRangeAsync(IEnumerable<T> entities);
	void Remove(T entity);
	void RemoveRange(IEnumerable<T> entities);
	Task<bool> SaveRepositoryAsync();
	Task<bool> AnyAsync();
	Task<TResult> QueryScalarAsync<TResult>(Func<IQueryable<T>, TResult> queryFunc);
	Task<List<T>> QueryAsync(Func<IQueryable<T>, IQueryable<T>> queryFunc);
}


public class Repository<T> : IRepository<T>, IDisposable where T : class
{
	private AppDbContext _context;
	private bool _disposed = false; // To detect redundant calls
	
	public Repository(AppDbContext appDBContext)
	{
		_context = appDBContext;
	}

	public async Task AddAsync(T entity)
	{
		await _context.Set<T>().AddAsync(entity);
	}

	public async Task AddRangeAsync(IEnumerable<T> entities)
	{
		await _context.Set<T>().AddRangeAsync(entities);
	}

	public async Task<IEnumerable<T>> GetAllAsync()
	{
		return await _context.Set<T>().ToListAsync();
	}

	public async Task<IEnumerable<T>> GetAllIncludesAsync(params Func<IQueryable<T>, IQueryable<T>>[] includes)
	{
		IQueryable<T> query = _context.Set<T>();

		// Apply each include function to the query
		foreach (var include in includes)
		{
			query = include(query);
		}

		return await query.ToListAsync();
	}

	public async Task<T> GetByIdAsync(int id)
	{
		return await _context.Set<T>().FindAsync(id);
	}

	public async Task<T> GetByIdIncludesAsync(int id, params Func<IQueryable<T>, IQueryable<T>>[] includes)
	{
		IQueryable<T> query = _context.Set<T>();

		// Apply each include function to the query
		foreach (var include in includes)
		{
			query = include(query);
		}

		return await query.FirstOrDefaultAsync(x => EF.Property<int>(x, "Id") == id);
	}

	public void Remove(T entity)
	{
		_context.Set<T>().Remove(entity);
	}

	public void RemoveRange(IEnumerable<T> entities)
	{
		_context.Set<T>().RemoveRange(entities);
	}

	public async Task<bool> SaveRepositoryAsync()
	{
		var rows = await _context.SaveChangesAsync();
		return rows > 0;
	}

	public async Task<bool> AnyAsync()
	{
		return await _context.Set<T>().AnyAsync();
	}
	
	public async Task<TResult> QueryScalarAsync<TResult>(Func<IQueryable<T>, TResult> queryFunc)
	{
		return await Task.FromResult(queryFunc(_context.Set<T>()));
	}

	public async Task<List<T>> QueryAsync(Func<IQueryable<T>, IQueryable<T>> queryFunc)
	{
		var query = queryFunc(_context.Set<T>());
		return await query.ToListAsync();
	}
	
	// Implementation of Dispose pattern
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposed)
		{
			if (disposing)
			{
				// Dispose managed resources.
				_context?.Dispose();
			}
			// Free unmanaged resources (if any) here.

			_disposed = true;
		}
	}
}
