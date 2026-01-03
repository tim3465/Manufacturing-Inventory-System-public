using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CncApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBarsAndPartsPerBarToJobAndShift : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BarsConsumed",
                table: "Shifts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PartsPerBar",
                table: "Shifts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BarsInJob",
                table: "Jobs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EstimatedPartsPerBar",
                table: "Jobs",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BarsConsumed",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "PartsPerBar",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "BarsInJob",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "EstimatedPartsPerBar",
                table: "Jobs");
        }
    }
}
