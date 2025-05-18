using BASE.DAL.Contracts;

namespace APP.DAL.Contracts;

public interface IAppUow : IBaseUow
{
    IWardrobeRepository WardrobeRepository { get; }
    IClothingItemRepository ClothingItemRepository { get; }
    IOutfitRepository OutfitRepository { get; }
    IOutfitItemRepository OutfitItemRepository { get; }
    IFavouriteRepository FavouriteRepository { get; }
    IPlannerEntryRepository PlannerEntryRepository { get; }
    IImageMetadataRepository ImageMetadataRepository { get; }
}