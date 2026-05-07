using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Futelo.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddLeagueChampion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChampionId",
                table: "Leagues",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Leagues_ChampionId",
                table: "Leagues",
                column: "ChampionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Leagues_AspNetUsers_ChampionId",
                table: "Leagues",
                column: "ChampionId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leagues_AspNetUsers_ChampionId",
                table: "Leagues");

            migrationBuilder.DropIndex(
                name: "IX_Leagues_ChampionId",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "ChampionId",
                table: "Leagues");
        }
    }
}
