using BASE.BLL.Contracts;

namespace APP.BLL.Contracts;

public interface IAppBll : IBaseBll
{
    IClothingItemService ClothingItems { get; }
    IOutfitService Outfits { get; }
    IWardrobeService Wardrobes { get; }
    IOutfitItemService OutfitItems { get; }
    IPlannerEntryService PlannerEntries { get; }
    IFavouriteService Favourites { get; }
    IImageMetadataService Images { get; }
}