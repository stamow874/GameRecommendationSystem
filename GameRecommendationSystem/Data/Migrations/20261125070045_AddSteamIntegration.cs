using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameRecommendationSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSteamIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SteamAvatarUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SteamConnectedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SteamDisplayName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SteamId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SteamLastSyncedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SteamProfileUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserSteamGames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AppId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IconUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlaytimeForeverMinutes = table.Column<int>(type: "int", nullable: false),
                    PlaytimeTwoWeeksMinutes = table.Column<int>(type: "int", nullable: true),
                    LastPlayedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSyncedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSteamGames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSteamGames_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSteamGames_UserId_AppId",
                table: "UserSteamGames",
                columns: new[] { "UserId", "AppId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSteamGames");

            migrationBuilder.DropColumn(
                name: "SteamAvatarUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SteamConnectedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SteamDisplayName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SteamId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SteamLastSyncedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SteamProfileUrl",
                table: "AspNetUsers");
        }
    }
}
