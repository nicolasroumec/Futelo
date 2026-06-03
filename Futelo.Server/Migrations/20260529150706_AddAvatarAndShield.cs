using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Futelo.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddAvatarAndShield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Shield",
                table: "Teams",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Avatar",
                table: "AspNetUsers",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Shield",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "AspNetUsers");
        }
    }
}
