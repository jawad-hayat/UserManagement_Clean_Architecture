using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public static class Seed
    {
        public static void SeedData(this ModelBuilder builder)
        {
            SeedUserAndRoles(builder);
        }

        private static void SeedUserAndRoles(ModelBuilder builder)
        {
            string AdminRoleId = Guid.NewGuid().ToString();
            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = AdminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            });


            var userId = Guid.NewGuid().ToString();

            var hasher = new PasswordHasher<User>();
            //insert admin record
            builder.Entity<User>().HasData(new User
            {
                Id = userId,
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@pacsquare.com",
                NormalizedEmail = "ADMIN@PACSQUARE.COM",
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "admin@123"),
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                PhoneNumber = "",
                PhoneNumberConfirmed = true,
                IsDeleted = false
            });

            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = AdminRoleId,
                UserId = userId,
            });
        }
    }
}
