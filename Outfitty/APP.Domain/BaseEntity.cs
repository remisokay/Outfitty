namespace Domain;

public abstract class BaseEntity
{
    public Guid Id { get; set; }
}

public abstract class BaseEntity<TKey>
{
    
}
