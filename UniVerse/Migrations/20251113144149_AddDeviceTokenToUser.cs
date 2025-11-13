using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniVerse.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceTokenToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModuleId",
                table: "Announcements",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ModuleTitle",
                table: "Announcements",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModuleId",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "ModuleTitle",
                table: "Announcements");
        }
    }
}
