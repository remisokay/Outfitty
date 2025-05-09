using BASE.Contracts;
using BASE.DAL.Contracts;
using Microsoft.EntityFrameworkCore;

namespace BASE.DAL.EF;

public class BaseRepository<TDalEntity, TDomainEntity> : BaseRepository<TDalEntity, TDomainEntity, Guid>,
    IBaseRepository<TDalEntity>
    where TDalEntity : class, IDomainId
    where TDomainEntity : class, IDomainId
{
    public BaseRepository(DbContext repositoryDbContext, IMapper<TDalEntity, TDomainEntity> mapper)
        : base(repositoryDbContext, mapper)
    {
        
    }
    
}

public class BaseRepository<TDalEntity, TDomainEntity, TKey> : IBaseRepository<TDalEntity, TKey>
    where TDalEntity : class, IDomainId<TKey>
    where TDomainEntity : class, IDomainId<TKey>
    where TKey : IEquatable<TKey>
{
    
    protected DbContext RepositoryDbContext;
    protected DbSet<TDomainEntity> RepositoryDbSet;
    protected IMapper<TDalEntity, TDomainEntity, TKey> Mapper;
    
    public BaseRepository(DbContext repositoryDbContext, IMapper<TDalEntity, TDomainEntity, TKey> mapper)
    {
        RepositoryDbContext = repositoryDbContext; // for db operations
        Mapper = mapper; // to translate between DAL and domain entities
        RepositoryDbSet = RepositoryDbContext.Set<TDomainEntity>(); // to set for the entity type
    }
    
    // user specific data filtering
    protected virtual IQueryable<TDomainEntity> GetQuery(TKey? userId = default!)
    {
        var query = RepositoryDbSet.AsQueryable();

        if (typeof(IDomainUserId<TKey>).IsAssignableFrom(typeof(TDomainEntity)) &&
            userId != null &&
            !EqualityComparer<TKey>.Default.Equals(userId, default))
        {
            query = query.Where(e => ((IDomainUserId<TKey>)e).UserId.Equals(userId));
        }
        // only return entities belonging to the specified user
        return query;
    }

    public async Task<int> SaveChangesAsync()
    {
        // commits all changes to the db
        return await RepositoryDbContext.SaveChangesAsync();
    }
    
    // Read operations
    public virtual IEnumerable<TDalEntity> All(TKey? userId = default)
    {
        return GetQuery(userId).ToList().Select(e => Mapper.Map(e)!);
    }

    public virtual async Task<IEnumerable<TDalEntity>> AllAsync(TKey? userId = default!)
    {
        return (await GetQuery(userId).ToListAsync()).Select(e => Mapper.Map(e)!);
    }

    public virtual TDalEntity? Find(TKey id, TKey? userId = default)
    {
        var query = GetQuery(userId);
        var res = query.FirstOrDefault(e => e.Id.Equals(id));
        return Mapper.Map(res);
    }

    public virtual async Task<TDalEntity?> FindAsync(TKey id, TKey? userId = default)
    {
        var query = GetQuery(userId);
        var res = await query.FirstOrDefaultAsync(e => e.Id.Equals(id));
        return Mapper.Map(res);
    }

    public virtual bool Exists(TKey id, TKey? userId = default)
    {
        var query = GetQuery(userId);
        return query.Any(e => e.Id.Equals(id));
    }

    public virtual async Task<bool> ExistsAsync(TKey id, TKey? userId = default)
    {
        var query = GetQuery(userId);
        return await query.AnyAsync(e => e.Id.Equals(id));
    }

    // Create operations
    public virtual void Add(TDalEntity entity, TKey? userId = default)
    {
        var dbEntity = Mapper.Map(entity);

        if (typeof(IDomainUserId<TKey>).IsAssignableFrom(typeof(TDomainEntity)) &&
            userId != null &&
            !EqualityComparer<TKey>.Default.Equals(userId, default))
        {
            ((IDomainUserId<TKey>) dbEntity!).UserId = userId;
        }
        
        RepositoryDbSet.Add(dbEntity!);
    }

    // Update operations
    public virtual TDalEntity? Update(TDalEntity entity, TKey? userId = default)
    {
        var query = GetQuery(userId);
        return query.Any(e => e.Id.Equals(entity.Id)) 
            ? Mapper.Map(RepositoryDbSet.Update(Mapper.Map(entity)!).Entity)! 
            : null;
    }

    public virtual async Task<TDalEntity?> UpdateAsync(TDalEntity entity, TKey? userId = default)
    {
        var query = GetQuery(userId);
        return await query.AnyAsync(e => e.Id.Equals(entity.Id)) 
            ? Mapper.Map(RepositoryDbSet.Update(Mapper.Map(entity)!).Entity)! 
            : null;
    }
    
    // Delete operations
    public virtual void Remove(TDalEntity entity, TKey? userId = default)
    {
        Remove(entity.Id, userId);
    }

    public virtual void Remove(TKey id, TKey? userId)
    {
        var query = GetQuery(userId);
        query = query.Where(e => e.Id.Equals(id));
        var dbEntity = query.FirstOrDefault();
        
        if (dbEntity != null) RepositoryDbSet.Remove(dbEntity);
    }

    public virtual async Task RemoveAsync(TKey id, TKey? userId = default)
    {
        var query = GetQuery(userId);
        query = query.Where(e => e.Id.Equals(id));
        var dbEntity = await query.FirstOrDefaultAsync();
        
        if (dbEntity != null) RepositoryDbSet.Remove(dbEntity);
    }
}