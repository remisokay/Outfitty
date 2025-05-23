using APP.BLL.Contracts;
using APP.BLL.Mappers;
using APP.BLL.Services;
using APP.DAL.Contracts;
using BASE.BLL;

namespace APP.BLL;

public class AppBll : BaseBll<IAppUow>, IAppBll
{
    public AppBll(IAppUow uow) : base(uow)
    {
        
    }

    
    private IClothingItemService? _clothingItemService;
    public IClothingItemService ClothingItems =>
        // Each service is created only when first accessed "??="
        _clothingItemService ??= new ClothingItemService(
            BllUow,
            new ClothingItemBllMapper(),
            new OutfitBllMapper()
        );
    
    
    private IOutfitService? _outfitService;
    public IOutfitService Outfits =>
        _outfitService ??= new OutfitService(
            BllUow,
            new OutfitBllMapper()
            // new OutfitItemBllMapper(),
            // new ClothingItemBllMapper()
        );
    
    
    private IWardrobeService? _wardrobeService;
    public IWardrobeService Wardrobes =>
        _wardrobeService ??= new WardrobeService(
            BllUow,
            new WardrobeBllMapper(),
            new ClothingItemBllMapper()
        );
    
    
    private IOutfitItemService? _outfitItemService;
    public IOutfitItemService OutfitItems =>
        _outfitItemService ??= new OutfitItemService(
            BllUow,
            new OutfitItemBllMapper(),
            new OutfitBllMapper()
        );
    
    
    private IPlannerEntryService? _plannerEntryService;
    public IPlannerEntryService PlannerEntries =>
        _plannerEntryService ??= new PlannerEntryService(
            BllUow,
            new PlannerEntryBllMapper(),
            new OutfitBllMapper()
        );
    
    
    private IFavouriteService? _favouriteService;
    public IFavouriteService Favourites =>
        _favouriteService ??= new FavouriteService(
            BllUow,
            new FavouriteBllMapper(),
            new OutfitBllMapper()
        );
    
    
    private IImageMetadataService? _imageMetadataService;
    public IImageMetadataService Images =>
        _imageMetadataService ??= new ImageMetadataService(
            BllUow,
            new ImageMetadataBllMapper()
        );
}