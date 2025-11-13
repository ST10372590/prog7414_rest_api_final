using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniVerse.Migrations
{
    /// <inheritdoc />
    public partial class CorrectModuleIdInAssessment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModuleID",
                table: "Assessments",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModuleID",
                table: "Assessments");
        }
    }
}
