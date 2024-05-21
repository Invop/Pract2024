using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManekiApp.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class TestUwU : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "About",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "ApplicationUser");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "public",
                table: "UserSubscription",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<Guid>(
                name: "SubscriptionId",
                schema: "public",
                table: "UserSubscription",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .OldAnnotation("Relational:ColumnOrder", 0);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "UserSubscription",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "UserSubscription");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "public",
                table: "UserSubscription",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<Guid>(
                name: "SubscriptionId",
                schema: "public",
                table: "UserSubscription",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AddColumn<string>(
                name: "About",
                table: "ApplicationUser",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "ApplicationUser",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "ApplicationUser",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "ProfilePicture",
                table: "ApplicationUser",
                type: "bytea",
                nullable: true);
        }
    }
}
