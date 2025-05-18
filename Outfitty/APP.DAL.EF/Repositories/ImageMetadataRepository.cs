using APP.DAL.Contracts;
using APP.DAL.DTO;
using APP.DAL.EF.Mappers;
using BASE.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace APP.DAL.EF.Repositories;

public class ImageMetadataRepository : BaseRepository<ImageMetadata, Domain.ImageMetadata>, IImageMetadataRepository
{
    public ImageMetadataRepository(AppDbContext repositoryDbContext) : base(repositoryDbContext, new ImageMetadataUowMapper())
    {
    }
    
    public override async Task<IEnumerable<ImageMetadata>> AllAsync(Guid userId = default)
    {
        var query = GetQuery(userId);
        query = query.Include(i => i.ClothingItem).Include(i => i.User);
        return (await query.ToListAsync()).Select(e => Mapper.Map(e)!);
    }

    public override async Task<ImageMetadata?> FindAsync(Guid id, Guid userId = default)
    {
        return Mapper.Map(await RepositoryDbSet
            .Include(i => i.ClothingItem)
            .Include(i => i.User)
            .Where(i => i.Id == id && i.UserId == userId)
            .FirstOrDefaultAsync());
    }

    public async Task<ImageMetadata?> GetImageByClothingItemAsync(Guid clothingItemId, Guid userId)
    {
        return Mapper.Map(await RepositoryDbSet
            .Where(i => i.ClothingItemId == clothingItemId && i.UserId == userId)
            .FirstOrDefaultAsync());
    }

    public async Task<ImageMetadata?> GetImageByUserProfileAsync(Guid userId)
    {
        return Mapper.Map(await RepositoryDbSet
            .Where(i => i.UserId == userId && i.ClothingItemId == null)
            .FirstOrDefaultAsync());
    }

    public async Task<IEnumerable<ImageMetadata>> GetImagesByUserAsync(Guid userId)
    {
        var query = GetQuery(userId);
        return (await query.ToListAsync()).Select(e => Mapper.Map(e)!);
    }

    public async Task<bool> DeleteImageAsync(Guid imageId, Guid userId)
    {
        var image = await RepositoryDbSet
            .Where(i => i.Id == imageId && i.UserId == userId)
            .FirstOrDefaultAsync();

        if (image == null) return false;
        RepositoryDbSet.Remove(image);
        await RepositoryDbContext.SaveChangesAsync();
        return true;
    }
}