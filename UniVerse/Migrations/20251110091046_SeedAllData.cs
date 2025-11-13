using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniVerse.Migrations
{
    /// <inheritdoc />
    public partial class SeedAllData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Users_LecturerID",
                table: "Courses");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Users_LecturerID",
                table: "Courses",
                column: "LecturerID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Users_LecturerID",
                table: "Courses");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Users_LecturerID",
                table: "Courses",
                column: "LecturerID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
