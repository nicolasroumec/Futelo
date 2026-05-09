using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Futelo.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddSuperCupChampion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChampionId",
                table: "SuperCups",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SuperCups_ChampionId",
                table: "SuperCups",
                column: "ChampionId");

            migrationBuilder.AddForeignKey(
                name: "FK_SuperCups_AspNetUsers_ChampionId",
                table: "SuperCups",
                column: "ChampionId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SuperCups_AspNetUsers_ChampionId",
                table: "SuperCups");

            migrationBuilder.DropIndex(
                name: "IX_SuperCups_ChampionId",
                table: "SuperCups");

            migrationBuilder.DropColumn(
                name: "ChampionId",
                table: "SuperCups");
        }
    }
}
