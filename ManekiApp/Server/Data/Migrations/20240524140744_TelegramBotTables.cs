using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManekiApp.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class TelegramBotTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserChatPurchases",
                columns: table => new
                {
                    UserChatPurchaseId = table.Column<Guid>(type: "uuid", nullable: false),
                    TelegramChatId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserChatPurchases", x => x.UserChatPurchaseId);
                    table.ForeignKey(
                        name: "FK_UserChatPurchases_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserNotificationChats",
                columns: table => new
                {
                    UserNotificationChatId = table.Column<Guid>(type: "uuid", nullable: false),
                    TelegramChatId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotificationChats", x => x.UserNotificationChatId);
                    table.ForeignKey(
                        name: "FK_UserNotificationChats_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserChatPurchases_UserId",
                table: "UserChatPurchases",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserNotificationChats_UserId",
                table: "UserNotificationChats",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserChatPurchases");

            migrationBuilder.DropTable(
                name: "UserNotificationChats");
        }
    }
}
