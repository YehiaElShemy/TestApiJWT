using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestApiJWT.Migrations
{
    public partial class RefreshTokenMM : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "2b7add82-e1d0-448e-886c-4ae26b97b47f", "3cf26a68-424f-4a5f-bc63-bb97dd6b10f5", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "d44f0e97-b5d9-47e6-9e35-e0e93dc4a756", "49579714-d8a8-473c-8619-57edfca27a80", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "c5bda407-c31d-4686-b7d7-b50ff97355a1", 0, "3f3f50b9-4301-411e-b5e0-f5b82062f441", "Admin123@gmail.com", false, false, null, "ADMIN123@GMAIL.COM", "ADMIN123", "AQAAAAIAAYagAAAAEH2XzyoLz+knSF5ueC5ART89pb8N4CR8g25c+L8w7w2ZQzQTlw+X551f87NqrjHqNg==", null, false, "cc99dc1c-58f5-416b-898b-c23b7d8158f1", false, "Admin123" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "d44f0e97-b5d9-47e6-9e35-e0e93dc4a756", "c5bda407-c31d-4686-b7d7-b50ff97355a1" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("AspNetUserRoles", "RoleId", "d44f0e97-b5d9-47e6-9e35-e0e93dc4a756", "UserId", "c5bda407-c31d-4686-b7d7-b50ff97355a1");
            migrationBuilder.DeleteData("AspNetRoles", "Id", "d44f0e97-b5d9-47e6-9e35-e0e93dc4a756");
            migrationBuilder.DeleteData("AspNetUsers", "Id", "c5bda407-c31d-4686-b7d7-b50ff97355a1");
        }

    }
}

