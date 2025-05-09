using BASE.DAL.Contracts;
using Microsoft.EntityFrameworkCore;

namespace BASE.DAL.EF;

public class BaseUow<TDbContext> : IBaseUow
where TDbContext : DbContext
{
    protected readonly TDbContext UowDbContext;

    public BaseUow(TDbContext uowDbContext)
    {
        UowDbContext = uowDbContext;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await UowDbContext.SaveChangesAsync();
    }
}