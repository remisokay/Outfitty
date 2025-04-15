using Domain;
using Domain.identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace APP.DAL.EF;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid, IdentityUserClaim<Guid>, AppUserRole,
    IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    public DbSet<Outfit> Outfits { get; set; } = default!;
    public DbSet<Wardrobe> Wardrobes { get; set; } = default!;
    public DbSet<ClothesItem> ClothesItems { get; set; } = default!;
    // TODO: add other entities

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
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

        builder.Entity<AppUserRole>().HasKey(a => a.Id);
        builder.Entity<AppUserRole>().HasIndex(a => new { a.UserId, a.RoleId }).IsUnique();
        builder.Entity<AppUserRole>()
            .HasOne(a => a.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(a => a.UserId);
        builder.Entity<AppUserRole>()
            .HasOne(a => a.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(a => a.RoleId);

    }

}