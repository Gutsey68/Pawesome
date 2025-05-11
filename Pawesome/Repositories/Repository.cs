using Pawesome.Interfaces;
using Microsoft.EntityFrameworkCore;
using Pawesome.Data;

namespace Pawesome.Repositories;

/// <summary>
/// Generic repository implementing basic CRUD operations
/// </summary>
/// <typeparam name="T">The type of entity managed by the repository</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    /// <summary>
    /// The database context
    /// </summary>
    protected readonly AppDbContext _context;
    
    /// <summary>
    /// Initializes a new instance of the Repository
    /// </summary>
    /// <param name="context">The database context</param>
    public Repository(AppDbContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Retrieves all entities of type T
    /// </summary>
    /// <returns>A collection of all entities</returns>
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }
    
    /// <summary>
    /// Retrieves an entity by its identifier
    /// </summary>
    /// <param name="id">The ID of the entity to retrieve</param>
    /// <returns>The entity if found, null otherwise</returns>
    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }
    
    /// <summary>
    /// Adds a new entity to the database
    /// </summary>
    /// <param name="entity">The entity to add</param>
    /// <returns>The added entity</returns>
    public virtual async Task<T> AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        return entity;
    }
    
    /// <summary>
    /// Updates an existing entity in the database
    /// </summary>
    /// <param name="entity">The entity to update</param>
    public virtual Task UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Deletes an entity from the database
    /// </summary>
    /// <param name="entity">The entity to delete</param>
    public virtual Task DeleteAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Saves all changes made in this context to the database
    /// </summary>
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}