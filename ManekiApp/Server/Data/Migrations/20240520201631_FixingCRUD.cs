using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManekiApp.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixingCRUD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorPage_ApplicationUser_UserId",
                schema: "public",
                table: "AuthorPage");

            migrationBuilder.DropIndex(
                name: "IX_UserVerificationCode_UserId",
                schema: "public",
                table: "UserVerificationCode");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSubscription",
                schema: "public",
                table: "UserSubscription");

            migrationBuilder.DropIndex(
                name: "IX_AuthorPage_UserId",
                schema: "public",
                table: "AuthorPage");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSubscription",
                schema: "public",
                table: "UserSubscription",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscription_SubscriptionId",
                schema: "public",
                table: "UserSubscription",
                column: "SubscriptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSubscription",
                schema: "public",
                table: "UserSubscription");

            migrationBuilder.DropIndex(
                name: "IX_UserSubscription_SubscriptionId",
                schema: "public",
                table: "UserSubscription");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSubscription",
                schema: "public",
                table: "UserSubscription",
                columns: new[] { "SubscriptionId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserVerificationCode_UserId",
                schema: "public",
                table: "UserVerificationCode",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuthorPage_UserId",
                schema: "public",
                table: "AuthorPage",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorPage_ApplicationUser_UserId",
                schema: "public",
                table: "AuthorPage",
                column: "UserId",
                principalTable: "ApplicationUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
