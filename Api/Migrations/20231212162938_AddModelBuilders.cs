using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class AddModelBuilders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_Servers_ServerId",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_Servers_Users_UserId",
                table: "Servers");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Servers_ServerId",
                table: "Members",
                column: "ServerId",
                principalTable: "Servers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Servers_Users_UserId",
                table: "Servers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_Servers_ServerId",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_Servers_Users_UserId",
                table: "Servers");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Servers_ServerId",
                table: "Members",
                column: "ServerId",
                principalTable: "Servers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Servers_Users_UserId",
                table: "Servers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
