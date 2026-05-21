using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Futelo.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddCupSeedingModeAndAwayGoalRule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BracketMode",
                table: "Cups",
                newName: "SeedingMode");

            migrationBuilder.AddColumn<bool>(
                name: "AwayGoalRule",
                table: "Cups",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AwayGoalRule",
                table: "Cups");

            migrationBuilder.RenameColumn(
                name: "SeedingMode",
                table: "Cups",
                newName: "BracketMode");
        }
    }
}
