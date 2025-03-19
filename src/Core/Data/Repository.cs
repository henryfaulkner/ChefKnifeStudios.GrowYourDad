using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

public interface IRepository<T> where T : class
{
    T GetById(int id);
    T GetByIdIncludes(int id, params Func<IQueryable<T>, IQueryable<T>>[] includes);
    IEnumerable<T> GetAll();
    IEnumerable<T> GetAllIncludes(params Func<IQueryable<T>, IQueryable<T>>[] includes);
    void Add(T entity);
    void AddRange(IEnumerable<T> entities);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
    bool SaveRepository();
    bool Any();
    TResult QueryScalar<TResult>(Func<IQueryable<T>, TResult> queryFunc);
    IEnumerable<T> Query(Func<IQueryable<T>, IQueryable<T>> queryFunc);
}


public class Repository<T> : IRepository<T>, IDisposable where T : class
{
    private AppDbContext _context;
    private bool _disposed = false; // To detect redundant calls
    
    public Repository(AppDbContext appDBContext)
    {
        _context = appDBContext;
    }

    public void Add(T entity)
    {
        _context.Set<T>().Add(entity);
    }

    public void AddRange(IEnumerable<T> entities)
    {
        _context.Set<T>().AddRange(entities);
    }

    public IEnumerable<T> GetAll()
    {
        return _context.Set<T>().AsEnumerable();
    }

    public IEnumerable<T> GetAllIncludes(params Func<IQueryable<T>, IQueryable<T>>[] includes)
    {
        IQueryable<T> query = _context.Set<T>();

        // Apply each include function to the query
        foreach (var include in includes)
        {
            query = include(query);
        }

        return query.AsEnumerable();
    }

    public T GetById(int id)
    {
        return _context.Set<T>().Find(id);
    }

    public T GetByIdIncludes(int id, params Func<IQueryable<T>, IQueryable<T>>[] includes)
    {
        IQueryable<T> query = _context.Set<T>();

        // Apply each include function to the query
        foreach (var include in includes)
        {
            query = include(query);
        }

        return query.FirstOrDefault(x => EF.Property<int>(x, "Id") == id);
    }

    public void Remove(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        _context.Set<T>().RemoveRange(entities);
    }

    public bool SaveRepository()
    {
        var rows = _context.SaveChanges();
        return rows > 0;
    }

    public bool Any()
    {
        return _context.Set<T>().Any();
    }
    
    public TResult QueryScalar<TResult>(Func<IQueryable<T>, TResult> queryFunc)
    {
        return queryFunc(_context.Set<T>());
    }

    public IEnumerable<T> Query(Func<IQueryable<T>, IQueryable<T>> queryFunc)
    {
        var query = queryFunc(_context.Set<T>());
        return query.AsEnumerable();
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