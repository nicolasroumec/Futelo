using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Futelo.Server.Migrations
{
    /// <inheritdoc />
    public partial class EloPerVault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EloRating",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "EloRating",
                table: "VaultPlayers",
                type: "integer",
                nullable: false,
                defaultValue: 1500);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EloRating",
                table: "VaultPlayers");

            migrationBuilder.AddColumn<int>(
                name: "EloRating",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
