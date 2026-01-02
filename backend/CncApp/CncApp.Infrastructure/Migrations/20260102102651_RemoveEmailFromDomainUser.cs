using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CncApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEmailFromDomainUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(320)",
                maxLength: 320,
                nullable: true);
        }
    }
}
