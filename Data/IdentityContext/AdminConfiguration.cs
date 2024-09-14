using OrdersManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace OrdersManagement.Data.IdentityContext
{
    public class AdminConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        private const string adminId = "B22698B8-42A2-4115-9631-1C2D1E2AC5F7";

        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            ApplicationUser admin = new ApplicationUser
            {
                Id = adminId,
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@testing.com",
                NormalizedEmail = "admin@testing.com".ToUpper(),
                PhoneNumber = "0211231412",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
            };

            admin.PasswordHash = GeneratePassword(admin);

            builder.HasData(admin);
        }

        public string GeneratePassword(ApplicationUser user)
        {
            PasswordHasher<ApplicationUser> passHash = new PasswordHasher<ApplicationUser>();
            return passHash.HashPassword(user, "Password123!");
           
        }
    }
}
