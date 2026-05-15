using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddAtmosphereSeatingStrengthFlavorBalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FlavorBalance",
                table: "Drinks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Strength",
                table: "Drinks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Atmosphere",
                table: "Bars",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Seating",
                table: "Bars",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FlavorBalance",
                table: "Drinks");

            migrationBuilder.DropColumn(
                name: "Strength",
                table: "Drinks");

            migrationBuilder.DropColumn(
                name: "Atmosphere",
                table: "Bars");

            migrationBuilder.DropColumn(
                name: "Seating",
                table: "Bars");
        }
    }
}
