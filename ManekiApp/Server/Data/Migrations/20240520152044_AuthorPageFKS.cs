using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManekiApp.Server.Migrations
{
    /// <inheritdoc />
    public partial class AuthorPageFKS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AuthorPageId",
                schema: "public",
                table: "Post",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ApplicationRole",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    NormalizedName = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUser",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    TelegramId = table.Column<string>(type: "text", nullable: true),
                    TelegramConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "text", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationRoleApplicationUser",
                columns: table => new
                {
                    RolesId = table.Column<string>(type: "text", nullable: false),
                    UsersId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationRoleApplicationUser", x => new { x.RolesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_ApplicationRoleApplicationUser_ApplicationRole_RolesId",
                        column: x => x.RolesId,
                        principalTable: "ApplicationRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationRoleApplicationUser_ApplicationUser_UsersId",
                        column: x => x.UsersId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuthorPage",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ProfileImage = table.Column<byte[]>(type: "bytea", nullable: true),
                    SocialLinks = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorPage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthorPage_ApplicationUser_UserId",
                        column: x => x.UserId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Post_AuthorPageId",
                schema: "public",
                table: "Post",
                column: "AuthorPageId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationRoleApplicationUser_UsersId",
                table: "ApplicationRoleApplicationUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorPage_UserId",
                schema: "public",
                table: "AuthorPage",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_AuthorPage_AuthorPageId",
                schema: "public",
                table: "Post",
                column: "AuthorPageId",
                principalSchema: "public",
                principalTable: "AuthorPage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_AuthorPage_AuthorPageId",
                schema: "public",
                table: "Post");

            migrationBuilder.DropTable(
                name: "ApplicationRoleApplicationUser");

            migrationBuilder.DropTable(
                name: "AuthorPage",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ApplicationRole");

            migrationBuilder.DropTable(
                name: "ApplicationUser");

            migrationBuilder.DropIndex(
                name: "IX_Post_AuthorPageId",
                schema: "public",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "AuthorPageId",
                schema: "public",
                table: "Post");
        }
    }
}
