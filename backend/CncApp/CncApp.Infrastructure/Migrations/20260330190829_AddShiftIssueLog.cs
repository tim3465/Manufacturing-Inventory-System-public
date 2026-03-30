using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CncApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddShiftIssueLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShiftIssueLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShiftId = table.Column<int>(type: "int", nullable: false),
                    IssueType = table.Column<byte>(type: "tinyint", nullable: false),
                    ScrapQuantity = table.Column<int>(type: "int", nullable: false),
                    Downtime = table.Column<TimeSpan>(type: "time", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    UpdatedDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: true),
                    InactivatedDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    InactivatedByUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftIssueLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShiftIssueLogs_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShiftIssueLogs_ShiftId",
                table: "ShiftIssueLogs",
                column: "ShiftId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShiftIssueLogs");
        }
    }
}
