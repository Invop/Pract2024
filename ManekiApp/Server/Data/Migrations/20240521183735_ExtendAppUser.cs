using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManekiApp.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExtendAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "About",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ProfilePicture",
                table: "AspNetUsers",
                type: "bytea",
                nullable: true);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "About",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "AspNetUsers");
        }
    }
}
