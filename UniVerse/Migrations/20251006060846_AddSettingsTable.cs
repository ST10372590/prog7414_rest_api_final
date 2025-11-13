using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniVerse.Migrations
{
    /// <inheritdoc />
    public partial class AddSettingsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSettings_Users_UserID",
                table: "UserSettings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSettings",
                table: "UserSettings");

            migrationBuilder.RenameTable(
                name: "UserSettings",
                newName: "UserSetting");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSetting",
                table: "UserSetting",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSetting_Users_UserID",
                table: "UserSetting",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSetting_Users_UserID",
                table: "UserSetting");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSetting",
                table: "UserSetting");

            migrationBuilder.RenameTable(
                name: "UserSetting",
                newName: "UserSettings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSettings",
                table: "UserSettings",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSettings_Users_UserID",
                table: "UserSettings",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
