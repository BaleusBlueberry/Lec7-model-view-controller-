using Lec7.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Lec7.Data;

public class Lec7Context(DbContextOptions<Lec7Context> options)
    : IdentityDbContext<AppUser, IdentityRole, string>(options)
{
    public DbSet<Product> Product { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //seed roles:
        var hasher = new PasswordHasher<AppUser>();
        builder.Entity<IdentityRole>().HasData(
            new IdentityRole() { Id = "1", Name = "Admin" },
            new IdentityRole() { Id = "2", Name = "User" }
        );

        var user = new AppUser()
        {
            Id = "0FDAB284-6F8A-47B3-B6BF-57570AFB7280",
            UserName = "Baleus@gmail.com",
            Email = "Baleus@gmail.com",
            Language = "C#",
            ConcurrencyStamp = "0FDAB284-6F8A-47B3-B6BF-57570AFB7280"
        };

        user.PasswordHash = hasher.HashPassword(user, "123456");

        builder
            .Entity<AppUser>()
            .HasData(user);

        builder
            .Entity<IdentityUserRole<string>>()
            .HasData(
                new IdentityUserRole<string>() { RoleId = "1", UserId = user.Id });

    }
}
 