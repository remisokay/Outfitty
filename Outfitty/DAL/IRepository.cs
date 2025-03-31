using Domain;

namespace DAL; //contracts repo

public interface IRepository<TEntity> : IRepository<TEntity, Guid> where TEntity : BaseEntity
{
    
}

public interface IRepository<TEntity, in TKey> where TEntity : BaseEntity
{
    
}