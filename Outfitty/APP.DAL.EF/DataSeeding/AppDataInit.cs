using System.Security.Claims;
using Domain.Enums;
using Domain.identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace APP.DAL.EF.DataSeeding;

public class AppDataInit
{
    public static void SeedAppData(AppDbContext context)
    {
        // seed sample only if db is empty
        if (!context.Wardrobes.Any())
        {
            var adminId = context.Users.FirstOrDefault(u => u.UserName == "admin@outfitty.com")?.Id;

            if (adminId != null)
            {   // seeding wardrobe
                var wardrobe = new Domain.Wardrobe
                {
                    Id = Guid.NewGuid(),
                    Name = "My Wardrobe",
                    Description = "Default wardrobe with sample clothing items",
                    UserId = adminId.Value
                };
                context.Wardrobes.Add(wardrobe);
                context.SaveChanges();

                // seeding clothes items
                // photos
                var shirtImage = new Domain.ImageMetadata
                {
                    Id = Guid.NewGuid(),
                    OriginalFileName = "shirt.jpg",
                    ContentType = "image/jpeg",
                    FileSize = 1024,
                    Width = 800,
                    Height = 600,
                    StoragePath = "uploads/sample/shirt.jpg",
                    PublicUrl = "/images/sample/shirt.jpg",
                    UserId = adminId.Value
                };
                context.ImageData.Add(shirtImage);
                
                var jeansImage = new Domain.ImageMetadata
                {
                    Id = Guid.NewGuid(),
                    OriginalFileName = "jeans.jpg",
                    ContentType = "image/jpeg",
                    FileSize = 1024,
                    Width = 800,
                    Height = 600,
                    StoragePath = "uploads/sample/jeans.jpg",
                    PublicUrl = "/images/sample/jeans.jpg",
                    UserId = adminId.Value
                };
                context.ImageData.Add(jeansImage);
                
                context.SaveChanges();
                
                // items
                var shirt = new Domain.ClothingItem
                {
                    Id = Guid.NewGuid(),
                    Name = "Cute Shirt",
                    Type = ClothingType.TopInner,
                    Style = ClothingStyle.Casual,
                    Season = ClothingSeason.AllSeason,
                    PrimaryColor = ClothingColor.Black,
                    SecondaryColor = ClothingColor.Pink,
                    WardrobeId = wardrobe.Id,
                    ImageMetadataId = shirtImage.Id
                };
                context.ClothingItems.Add(shirt);
                
                var jeans = new Domain.ClothingItem
                {
                    Id = Guid.NewGuid(),
                    Name = "Cute Jeans",
                    Type = ClothingType.BottomOuter,
                    Style = ClothingStyle.Casual,
                    Season = ClothingSeason.AllSeason,
                    PrimaryColor = ClothingColor.Blue,
                    WardrobeId = wardrobe.Id,
                    ImageMetadataId = jeansImage.Id
                };
                context.ClothingItems.Add(jeans);
                
                context.SaveChanges();
                
                // seeding sample outfit
                var sampleOutfit = new Domain.Outfit
                {
                    Id = Guid.NewGuid(),
                    Name = "Cute Outfit",
                    Description = "Cutesy comfortable outfit for everyday wear",
                    Season = ClothingSeason.AllSeason,
                    Style = ClothingStyle.Casual,
                    UserId = adminId.Value
                };
                context.Outfits.Add(sampleOutfit);
                context.SaveChanges();
                
                // seeding outfit items
                var outfitItems = new List<Domain.OutfitItem>
                {
                    new Domain.OutfitItem
                    {
                        Id = Guid.NewGuid(),
                        OutfitId = sampleOutfit.Id,
                        ClothingItemId = shirt.Id,
                        DisplayOrder = 1
                    },
                    new Domain.OutfitItem
                    {
                        Id = Guid.NewGuid(),
                        OutfitId = sampleOutfit.Id,
                        ClothingItemId = jeans.Id,
                        DisplayOrder = 2
                    }
                };
                context.OutfitItems.AddRange(outfitItems);
                
                // seeding planner entry
                var entry = new Domain.PlannerEntry
                {
                    Id = Guid.NewGuid(),
                    Title = "Walk with friends",
                    Date = DateTime.Today.AddDays(2),
                    Comment = "Its gonna be quite warm, maybe gonna need a jacket",
                    UserId = adminId.Value,
                    OutfitId = sampleOutfit.Id
                };
                context.PlannerEntries.Add(entry);
                
                // seeding favourite
                var fav = new Domain.Favourite
                {
                    Id = Guid.NewGuid(),
                    UserId = adminId.Value,
                    OutfitId = sampleOutfit.Id
                };
                context.Favourites.Add(fav);
                
                context.SaveChanges();
            }
        }
    }

    public static void MigrateDatabase(AppDbContext context)
    {
        context.Database.Migrate();
    }

    public static void DeleteDatabase(AppDbContext context)
    {
        context.Database.EnsureDeleted();
    }

    public static void SeedIdentity(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        // creating roles
        foreach (var (roleName, id) in InitialData.Roles)
        {
            var role = roleManager.FindByNameAsync(roleName).Result;
            
            if (role != null) continue;

            role = new AppRole()
            {
                Id = id ?? Guid.NewGuid(),
                Name = roleName
            };
            
            var result = roleManager.CreateAsync(role).Result;
            if (!result.Succeeded)
            {
                throw new ApplicationException("Role creation failed in data seeding!");
            }
        }
        
        // creating users
        foreach (var userInfo in InitialData.Users)
        {
            var user = userManager.FindByEmailAsync(userInfo.email).Result;
            if (user == null)
            {
                user = new AppUser()
                {
                    Id = userInfo.id ?? Guid.NewGuid(),
                    Email = userInfo.email,
                    Username = userInfo.username,
                    EmailConfirmed = true,
                    // TODO: ProfileImage = userInfo.profileImage,
                    Bio = userInfo.bio,
                };
                
                var result = userManager.CreateAsync(user, userInfo.password).Result;
                
                if (!result.Succeeded) throw new ApplicationException("User creation failed in data seeding!");
                
                result = userManager.AddClaimAsync(user, new Claim(ClaimTypes.Name, user.Username)).Result;
                
                if (!result.Succeeded) throw new ApplicationException("Claim adding failed in data seeding!");
            }
            
            // assigning roles
            foreach (var role in userInfo.roles)
            {
                if (userManager.IsInRoleAsync(user, role).Result)
                {
                    Console.WriteLine($"User {user.UserName} already in role {role}");
                    continue;
                }

                var roleResult = userManager.AddToRoleAsync(user, role).Result;
                if (!roleResult.Succeeded)
                {
                    foreach (var error in roleResult.Errors)
                    {
                        Console.WriteLine(error.Description);
                    }
                }
                else
                {
                    Console.WriteLine($"User {user.UserName} added to role {role}");
                }
            }
        }
    }
}