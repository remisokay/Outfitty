// using APP.DAL.Contracts;
// using BASE.DAL.Contracts;
// using BASE.DAL.EF;
// using Microsoft.EntityFrameworkCore;
//
// namespace APP.DAL.EF;
//
// public class AppUow : BaseUow<AppDbContext>, IAppUow
// {
//     public AppUow(AppDbContext uowDbContext) : base(uowDbContext)
//     {
//     }
//     
//     private IWardrobeRepository? _wardrobeRepository;
//     public IWardrobeRepository WardrobeRepository =>
//     _wardrobeRepository ??= new WardrobeRepository(UowDbContext);
//     public IClothingItemRepository ClothingItemRepository { get; }
//     public IOutfitRepository OutfitRepository { get; }
//     public IOutfitItemRepository OutfitItemRepository { get; }
//     public IFavouriteRepository FavouriteRepository { get; }
//     public IPlannerEntryRepository PlannerEntryRepository { get; }
//     public IImageMetadataRepository ImageMetadataRepository { get; }
// }
