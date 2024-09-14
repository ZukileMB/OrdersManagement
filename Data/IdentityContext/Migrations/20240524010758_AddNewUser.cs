using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrdersManagement.Data.IdentityContext.Migrations
{
    /// <inheritdoc />
    public partial class AddNewUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
            table: "AspNetUsers",
            columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
            values: new object[] { "B22698B8-42A2-4115-9631-1C2D1E2AC5F7", 0, "1e4b3cc1-0eb8-4cb5-a5e4-fd3cfc0b935d", "ApplicationUser", "admin@testing.com", true, false, null, "ADMIN@TESTING.COM", "ADMIN", "AQAAAAIAAYagAAAAEAIBB8UbqpBhPaI7rFPrzbqT4k1M5w0PyrW2PBO3xtuoFaNaipoHECw2Av2IJZVRUg==", "0211231412", true, "13116adc-9643-4456-b8cf-0192402a6038", false, "Admin" });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "B22698B8-42A2-4115-9631-1C2D1E2AC5F7");

        }
    }
}
