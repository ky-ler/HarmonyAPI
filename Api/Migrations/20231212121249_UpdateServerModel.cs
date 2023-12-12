using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateServerModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Channels_ServerList_ServerId",
                table: "Channels");

            migrationBuilder.DropForeignKey(
                name: "FK_Members_ServerList_ServerId",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_ServerList_Users_UserId",
                table: "ServerList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServerList",
                table: "ServerList");

            migrationBuilder.RenameTable(
                name: "ServerList",
                newName: "Servers");

            migrationBuilder.RenameIndex(
                name: "IX_ServerList_UserId",
                table: "Servers",
                newName: "IX_Servers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ServerList_InviteCode",
                table: "Servers",
                newName: "IX_Servers_InviteCode");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Servers",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Servers",
                table: "Servers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_Servers_ServerId",
                table: "Channels",
                column: "ServerId",
                principalTable: "Servers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Channels_Servers_ServerId",
                table: "Channels");

            migrationBuilder.DropForeignKey(
                name: "FK_Members_Servers_ServerId",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_Servers_Users_UserId",
                table: "Servers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Servers",
                table: "Servers");

            migrationBuilder.RenameTable(
                name: "Servers",
                newName: "ServerList");

            migrationBuilder.RenameIndex(
                name: "IX_Servers_UserId",
                table: "ServerList",
                newName: "IX_ServerList_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Servers_InviteCode",
                table: "ServerList",
                newName: "IX_ServerList_InviteCode");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "ServerList",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServerList",
                table: "ServerList",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_ServerList_ServerId",
                table: "Channels",
                column: "ServerId",
                principalTable: "ServerList",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Members_ServerList_ServerId",
                table: "Members",
                column: "ServerId",
                principalTable: "ServerList",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServerList_Users_UserId",
                table: "ServerList",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
