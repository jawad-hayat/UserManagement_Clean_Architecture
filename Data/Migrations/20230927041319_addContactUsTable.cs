using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class addContactUsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "676d54c0-ba46-471e-af7b-5c241cc9a7b6", "90eb3d82-2e8d-4858-b69a-2d4730a085e9" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "676d54c0-ba46-471e-af7b-5c241cc9a7b6");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "90eb3d82-2e8d-4858-b69a-2d4730a085e9");

            migrationBuilder.CreateTable(
                name: "ContactUs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    IsDataStored = table.Column<bool>(type: "bit", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactUs", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "1bd23a26-876d-4dd3-82a4-a26c7ed6ad74", "2613c17d-458b-4dbc-b34d-b1a083acf476", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedDate", "Email", "EmailConfirmed", "FirstName", "IsActive", "IsDeleted", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "f80ac04a-836f-4ed6-bd37-83ecaa1a9672", 0, "745742b7-df45-40fd-973b-5ffbd753960a", new DateTime(2023, 9, 27, 4, 13, 18, 664, DateTimeKind.Utc).AddTicks(1907), "admin@pacsquare.com", true, "Admin", true, false, "User", false, null, "ADMIN@PACSQUARE.COM", "ADMIN", "AQAAAAEAACcQAAAAEJBoV050psCClTw4yTQvigAXf41ys55GITlxinhUQPOnsVtonOGkmu1ualuNeq9Ejw==", "", true, "4c3cdfc6-ea6a-44c3-b410-4911089b6c0d", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1bd23a26-876d-4dd3-82a4-a26c7ed6ad74", "f80ac04a-836f-4ed6-bd37-83ecaa1a9672" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactUs");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1bd23a26-876d-4dd3-82a4-a26c7ed6ad74", "f80ac04a-836f-4ed6-bd37-83ecaa1a9672" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1bd23a26-876d-4dd3-82a4-a26c7ed6ad74");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f80ac04a-836f-4ed6-bd37-83ecaa1a9672");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "676d54c0-ba46-471e-af7b-5c241cc9a7b6", "a863ce12-24b5-47ea-80ae-e24c7420f5d9", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedDate", "Email", "EmailConfirmed", "FirstName", "IsActive", "IsDeleted", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "90eb3d82-2e8d-4858-b69a-2d4730a085e9", 0, "d2663cd2-86bf-415e-ad68-9b27f0fad09d", new DateTime(2023, 9, 26, 4, 36, 45, 402, DateTimeKind.Utc).AddTicks(4374), "admin@pacsquare.com", true, "Admin", true, false, "User", false, null, "ADMIN@PACSQUARE.COM", "ADMIN", "AQAAAAEAACcQAAAAEGVYREMRJdSorEo/NKXvWekdrC68Qvh1SZQDH7rcLemCJFggemQc9AVfouvq+dv1VA==", "", true, "17df809a-9114-48ed-ad64-5b2ec2e5b0eb", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "676d54c0-ba46-471e-af7b-5c241cc9a7b6", "90eb3d82-2e8d-4858-b69a-2d4730a085e9" });
        }
    }
}
