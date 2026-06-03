using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Futelo.Server.Migrations
{
    /// <inheritdoc />
    public partial class FlexibleTiebreakers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TiebreakerRule",
                table: "Leagues",
                newName: "FinalTiebreaker");

            migrationBuilder.AddColumn<string>(
                name: "TiebreakerCriteria",
                table: "Leagues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("UPDATE [Leagues] SET [TiebreakerCriteria] = '0,1,2,3,4,5,6', [FinalTiebreaker] = 1 WHERE [TiebreakerCriteria] = ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TiebreakerCriteria",
                table: "Leagues");

            migrationBuilder.RenameColumn(
                name: "FinalTiebreaker",
                table: "Leagues",
                newName: "TiebreakerRule");
        }
    }
}
