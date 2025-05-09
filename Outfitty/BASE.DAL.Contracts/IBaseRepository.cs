using BASE.Contracts;

namespace BASE.DAL.Contracts;

public interface IBaseRepository<TEntity> : IBaseRepository<TEntity, Guid> 
    where TEntity : IDomainId
{
}

public interface IBaseRepository<TEntity, TKey>
    where TEntity : IDomainId<TKey>
    where TKey : IEquatable<TKey>
{
    // read operations
    IEnumerable<TEntity> All(TKey? userId = default!);
    Task<IEnumerable<TEntity>> AllAsync(TKey? userId = default!);
    
    TEntity? Find(TKey id, TKey? userId = default!);
    Task<TEntity?> FindAsync(TKey id, TKey? userId = default!);
    
    bool Exists(TKey id, TKey? userId = default!);
    Task<bool> ExistsAsync(TKey id, TKey? userId = default!);
    
    // create operations
    void Add(TEntity entity, TKey? userId = default!);
    
    // update operations
    TEntity? Update(TEntity entity, TKey? userId = default!);
    Task<TEntity?> UpdateAsync(TEntity entity, TKey? userId = default!);
    
    // delete operations
    void Remove(TEntity entity, TKey? userId = default!);
    void Remove(TKey id, TKey? userId = default!);
    Task RemoveAsync(TKey id, TKey? userId = default!);
    
    
    
}

/*
 * Useful for:
 * 
 * Managing user-specific wardrobes
 * Ensuring users can only access their own clothes items
 * Maintaining separation between different users' outfits and favorites
 * Restricting calendar access to the appropriate user
 *
 */