using BASE.Contracts;
using Domain;
using Domain.identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace APP.DAL.EF;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid, IdentityUserClaim<Guid>, AppUserRole,
    IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    public DbSet<Wardrobe> Wardrobes { get; set; } = default!;
    public DbSet<ClothingItem> ClothingItems { get; set; } = default!;
    public DbSet<Outfit> Outfits { get; set; } = default!;
    public DbSet<OutfitItem> OutfitItems { get; set; } = default!;
    public DbSet<Favourite> Favourites { get; set; } = default!;
    public DbSet<PlannerEntry> PlannerEntries { get; set; } = default!;
    public DbSet<ImageMetadata> ImageData { get; set; } = default!;
    
    public DbSet<AppRefreshToken> RefreshTokens { get; set; } = default!;
    
    private readonly IUsernameResolver _usernameResolver;
    private readonly ILogger<AppDbContext> _logger;

    public AppDbContext(DbContextOptions<AppDbContext> options, IUsernameResolver usernameResolver,
        ILogger<AppDbContext> logger) : base(options)
    {
        _usernameResolver = usernameResolver;
        _logger = logger;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        foreach (var rel in builder.Model
                     .GetEntityTypes()
                     .SelectMany(e => e.GetForeignKeys()))
        {
            rel.DeleteBehavior = DeleteBehavior.Restrict; // remove cascade delete
        }
        
        // Identity relationships
        builder.Entity<AppUserRole>()
            .HasOne(a => a.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(a => a.UserId);
        
        builder.Entity<AppUserRole>()
            .HasOne(a => a.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(a => a.RoleId);
        
        builder.Entity<AppUser>()
            .HasOne(u => u.ProfileImage)
            .WithOne(i => i.User)
            .HasForeignKey<ImageMetadata>(i => i.UserId);
        
        builder.Entity<AppRefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId);
        
        // Enums configuration
        builder.Entity<ClothingItem>()
            .Property(c => c.Type)
            .HasConversion<string>();
        
        builder.Entity<ClothingItem>()
            .Property(c => c.Style)
            .HasConversion<string>();
        
        builder.Entity<ClothingItem>()
            .Property(c => c.Season)
            .HasConversion<string>();
        
        builder.Entity<ClothingItem>()
            .Property(c => c.PrimaryColor)
            .HasConversion<string>();
        
        builder.Entity<ClothingItem>()
            .Property(c => c.SecondaryColor)
            .HasConversion<string>();
        
        builder.Entity<Outfit>()
            .Property(c => c.Season)
            .HasConversion<string>();
        
        builder.Entity<Outfit>()
            .Property(c => c.Style)
            .HasConversion<string>();

        /* Outfitty relationships */
        
        // ClothingItem belongs to a Wardrobe
        builder.Entity<ClothingItem>()
            .HasOne(c => c.Wardrobe)
            .WithMany(w => w.ClothingItems)
            .HasForeignKey(c => c.WardrobeId);
        
        // ClothingItem has ImageMetadata
        builder.Entity<ClothingItem>()
            .HasOne(c => c.ImageMetadata)
            .WithOne(i => i.ClothingItem)
            .HasForeignKey<ImageMetadata>(i => i.ClothingItemId);
        
        // Outfit has ImageMetadata
        builder.Entity<Outfit>()
            .HasOne(c => c.ImageMetadata)
            .WithOne(i => i.Outfit)
            .HasForeignKey<ImageMetadata>(i => i.OutfitId);
        
        // OutfitItem connects Outfit and ClothingItem
        builder.Entity<OutfitItem>()
            .HasOne(oi => oi.Outfit)
            .WithMany(o => o.OutfitItems)
            .HasForeignKey(oi => oi.OutfitId);
            
        builder.Entity<OutfitItem>()
            .HasOne(oi => oi.ClothingItem)
            .WithMany(ci => ci.OutfitItems)
            .HasForeignKey(oi => oi.ClothingItemId);
        
        // Favourite references Outfit
        builder.Entity<Favourite>()
            .HasOne(f => f.Outfit)
            .WithMany(o => o.Favourites)
            .HasForeignKey(f => f.OutfitId);
        
        // PlannerEntry references Outfit
        builder.Entity<PlannerEntry>()
            .HasOne(pe => pe.Outfit)
            .WithMany(o => o.PlannerEntries)
            .HasForeignKey(pe => pe.OutfitId);
        
        // User relationships
        builder.Entity<Wardrobe>()
            .HasOne(w => w.User)
            .WithMany(u => u.Wardrobes)
            .HasForeignKey(w => w.UserId);
        
        builder.Entity<Favourite>()
            .HasOne(f => f.User)
            .WithMany(u => u.Favourites)
            .HasForeignKey(f => f.UserId);
            
        builder.Entity<PlannerEntry>()
            .HasOne(pe => pe.User)
            .WithMany(u => u.PlannerEntries)
            .HasForeignKey(pe => pe.UserId);
        
        builder.Entity<Outfit>()
            .HasOne(o => o.User)
            .WithMany(u => u.Outfits)
            .HasForeignKey(o => o.UserId);
        
        // indexing  for frequently queried columns
        builder.Entity<ClothingItem>()
            .HasIndex(c => c.WardrobeId);
    
        builder.Entity<Outfit>()
            .HasIndex(o => o.UserId);
    
        builder.Entity<PlannerEntry>()
            .HasIndex(pe => new { pe.UserId, pe.Date });
        
        builder.Entity<Favourite>()
            .HasIndex(f => new { f.UserId, f.OutfitId })
            .IsUnique();

    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var addedEntries = ChangeTracker.Entries();

        foreach (var entry in addedEntries)
        {
            if (entry is { Entity: IDomainMeta })
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        (entry.Entity as IDomainMeta)!.CreatedAt = DateTime.UtcNow;
                        (entry.Entity as IDomainMeta)!.ChangedBy = _usernameResolver.CurrentUserName;
                        break;
                    case EntityState.Modified:
                        entry.Property("ChangedAt").IsModified = true;
                        entry.Property("ChangedBy").IsModified = true;
                        (entry.Entity as IDomainMeta)!.CreatedAt = DateTime.UtcNow;
                        (entry.Entity as IDomainMeta)!.ChangedBy = _usernameResolver.CurrentUserName;
                        
                        // Prevent overwriting CreatedBy/CreatedAt on update
                        entry.Property("CreatedAt").IsModified = false;
                        entry.Property("CreatedBy").IsModified = false;
                        break;
                }
            }

            if (entry is { Entity: IDomainUserId, State: EntityState.Modified })
            {
                // do not allow user id modification
                entry.Property("UserId").IsModified = false;
                _logger.LogWarning("UserId modification attempt. Denied!");
            }
        }
        
        return base.SaveChangesAsync(cancellationToken);
    }

}