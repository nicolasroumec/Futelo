using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Futelo.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddVideoGameToSeasonAndTeamToSeasonPlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VideoGameId",
                table: "Seasons",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "SeasonPlayers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Seasons_VideoGameId",
                table: "Seasons",
                column: "VideoGameId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonPlayers_TeamId",
                table: "SeasonPlayers",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonPlayers_Teams_TeamId",
                table: "SeasonPlayers",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Seasons_VideoGames_VideoGameId",
                table: "Seasons",
                column: "VideoGameId",
                principalTable: "VideoGames",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SeasonPlayers_Teams_TeamId",
                table: "SeasonPlayers");

            migrationBuilder.DropForeignKey(
                name: "FK_Seasons_VideoGames_VideoGameId",
                table: "Seasons");

            migrationBuilder.DropIndex(
                name: "IX_Seasons_VideoGameId",
                table: "Seasons");

            migrationBuilder.DropIndex(
                name: "IX_SeasonPlayers_TeamId",
                table: "SeasonPlayers");

            migrationBuilder.DropColumn(
                name: "VideoGameId",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "SeasonPlayers");
        }
    }
}
