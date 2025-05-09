namespace BASE.DAL.Contracts;

public interface IBaseUow
{
    public Task<int> SaveChangesAsync();
}