using BASE.Contracts;
using BASE.DAL.Contracts;

namespace BASE.BLL.Contracts;

public interface IBaseService<TEntity> : IBaseService<TEntity, Guid>, IBaseRepository<TEntity>
    where TEntity : IDomainId
{
    
}

public interface IBaseService<TEntity, TKey> : IBaseRepository<TEntity, TKey>
    where TEntity : IDomainId<TKey>
    where TKey : IEquatable<TKey>
{
    
}