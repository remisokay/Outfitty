namespace BASE.BLL.Contracts;

public interface IBaseBll
{
    public Task<int> SaveChangesAsync();
}