﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManekiApp.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserChatsId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserChatNotification",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    TelegramChatId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserChatNotification", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserChatNotification_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserChatPayment",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    TelegramChatId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserChatPayment", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserChatPayment_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserChatNotification");

            migrationBuilder.DropTable(
                name: "UserChatPayment");
        }
    }
}
