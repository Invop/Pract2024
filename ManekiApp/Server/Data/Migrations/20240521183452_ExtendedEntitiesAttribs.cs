using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManekiApp.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExtendedEntitiesAttribs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_AuthorPage_AuthorId",
                schema: "public",
                table: "Subscription");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                schema: "public",
                table: "Subscription",
                newName: "AuthorPageId");

            migrationBuilder.RenameIndex(
                name: "IX_Subscription_AuthorId",
                schema: "public",
                table: "Subscription",
                newName: "IX_Subscription_AuthorPageId");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                schema: "public",
                table: "Post",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "About",
                table: "ApplicationUser",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "ApplicationUser",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "ApplicationUser",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ProfilePicture",
                table: "ApplicationUser",
                type: "bytea",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuthorPage_UserId",
                schema: "public",
                table: "AuthorPage",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorPage_ApplicationUser_UserId",
                schema: "public",
                table: "AuthorPage",
                column: "UserId",
                principalTable: "ApplicationUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_AuthorPage_AuthorPageId",
                schema: "public",
                table: "Subscription",
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
                name: "FK_AuthorPage_ApplicationUser_UserId",
                schema: "public",
                table: "AuthorPage");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_AuthorPage_AuthorPageId",
                schema: "public",
                table: "Subscription");

            migrationBuilder.DropIndex(
                name: "IX_AuthorPage_UserId",
                schema: "public",
                table: "AuthorPage");

            migrationBuilder.DropColumn(
                name: "Title",
                schema: "public",
                table: "Post");

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

            migrationBuilder.RenameColumn(
                name: "AuthorPageId",
                schema: "public",
                table: "Subscription",
                newName: "AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_Subscription_AuthorPageId",
                schema: "public",
                table: "Subscription",
                newName: "IX_Subscription_AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_AuthorPage_AuthorId",
                schema: "public",
                table: "Subscription",
                column: "AuthorId",
                principalSchema: "public",
                principalTable: "AuthorPage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
