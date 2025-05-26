using APP.DAL.Contracts;
using APP.DAL.EF.Repositories;
using BASE.DAL.Contracts;
using BASE.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace APP.DAL.EF;

public class AppUow : BaseUow<AppDbContext>, IAppUow
{
    public AppUow(AppDbContext uowDbContext) : base(uowDbContext)
    {
    }
    
    private IWardrobeRepository? _wardrobeRepository;
    public IWardrobeRepository WardrobeRepository =>
    _wardrobeRepository ??= new WardrobeRepository(UowDbContext);
    
    
    private IClothingItemRepository? _clothingItemRepository;
    public IClothingItemRepository ClothingItemRepository =>
        _clothingItemRepository ??= new ClothingItemRepository(UowDbContext);
    
    
    private IOutfitRepository? _outfitRepository;
    public IOutfitRepository OutfitRepository =>
        _outfitRepository ??= new OutfitRepository(UowDbContext);
    
    
    private IOutfitItemRepository? _outfitItemRepository;
    public IOutfitItemRepository OutfitItemRepository =>
        _outfitItemRepository ??= new OutfitItemRepository(UowDbContext);
    
    
    private IFavouriteRepository? _favouriteRepository;
    public IFavouriteRepository FavouriteRepository =>
        _favouriteRepository ??= new FavouriteRepository(UowDbContext);
    
    
    private IPlannerEntryRepository? _plannerEntryRepository;
    public IPlannerEntryRepository PlannerEntryRepository =>
        _plannerEntryRepository ??= new PlannerEntryRepository(UowDbContext);
    
    
    private IImageMetadataRepository? _imageMetadataRepository;
    public IImageMetadataRepository ImageMetadataRepository =>
        _imageMetadataRepository ??= new ImageMetadataRepository(UowDbContext);
}
