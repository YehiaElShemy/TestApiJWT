using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TestApiJWT.Helper
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {


        //string ADMIN_ID = "d44f0e97-b5d9-47e6-9e35-e0e93dc4a756";
        //    string ROLE_ID_USER = "2b7add82-e1d0-448e-886c-4ae26b97b47f";
        //    string USER_ID = "c5bda407-c31d-4686-b7d7-b50ff97355a1";
         
        //    builder.Entity<ApplicationUser>().HasData(
        //            new IdentityRole { Id = ADMIN_ID, Name = "Admin", NormalizedName = "ADMIN" },
        //            new IdentityRole { Id = ROLE_ID_USER, Name = "User", NormalizedName = "USER"}
        //            );
        //    builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
        //    {
        //        RoleId = ADMIN_ID,
        //        UserId = USER_ID,

        //    });

        //    builder.Entity<ApplicationUser>().HasData(
        //     new ApplicationUser
        //     {
        //         FirstName = "Yehia",
        //         LastName = "zakaria",
        //         PhoneNumber = "01064822587",
        //         LockoutEnabled = false,
        //         PhoneNumberConfirmed=true,
        //         TwoFactorEnabled= false,
        //         EmailConfirmed=false,


        //         Id = USER_ID,
        //         Email = "Admin123@gmail.com",
        //         UserName = "Admin123",
        //         NormalizedEmail = "ADMIN123@GMAIL.COM",
        //         NormalizedUserName = "ADMIN123",
        //         PasswordHash = "AQAAAAIAAYagAAAAEH2XzyoLz+knSF5ueC5ART89pb8N4CR8g25c+L8w7w2ZQzQTlw+X551f87NqrjHqNg==", // Admin@123
        //         AccessFailedCount =1
        //     }); ;



            base.OnModelCreating(builder);
        }
    }
}
