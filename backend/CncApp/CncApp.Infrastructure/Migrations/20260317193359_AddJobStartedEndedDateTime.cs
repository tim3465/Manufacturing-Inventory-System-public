using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CncApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJobStartedEndedDateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "EndedDateTime",
                table: "Jobs",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "StartedDateTime",
                table: "Jobs",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndedDateTime",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "StartedDateTime",
                table: "Jobs");
        }
    }
}
