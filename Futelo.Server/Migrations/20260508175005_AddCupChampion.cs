using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Futelo.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddCupChampion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChampionId",
                table: "Cups",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CupPosition",
                table: "CupPlayers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cups_ChampionId",
                table: "Cups",
                column: "ChampionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cups_AspNetUsers_ChampionId",
                table: "Cups",
                column: "ChampionId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cups_AspNetUsers_ChampionId",
                table: "Cups");

            migrationBuilder.DropIndex(
                name: "IX_Cups_ChampionId",
                table: "Cups");

            migrationBuilder.DropColumn(
                name: "ChampionId",
                table: "Cups");

            migrationBuilder.DropColumn(
                name: "CupPosition",
                table: "CupPlayers");
        }
    }
}
