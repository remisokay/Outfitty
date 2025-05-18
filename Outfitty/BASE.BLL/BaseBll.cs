using BASE.BLL.Contracts;
using BASE.DAL.Contracts;

namespace BASE.BLL;

public class BaseBll<TUow> : IBaseBll
    where TUow : IBaseUow
{
    protected readonly TUow BllUow;

    public BaseBll(TUow uow)
    {
        BllUow = uow;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await BllUow.SaveChangesAsync();
    }
}