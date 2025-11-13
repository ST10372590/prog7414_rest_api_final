using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniVerse.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "integer", nullable: false),
                    PreferredLanguage = table.Column<string>(type: "text", nullable: false),
                    TimeZone = table.Column<string>(type: "text", nullable: false),
                    EmailNotifications = table.Column<bool>(type: "boolean", nullable: false),
                    PushNotifications = table.Column<bool>(type: "boolean", nullable: false),
                    InAppNotifications = table.Column<bool>(type: "boolean", nullable: false),
                    ProfilePublic = table.Column<bool>(type: "boolean", nullable: false),
                    ShareUsageData = table.Column<bool>(type: "boolean", nullable: false),
                    Theme = table.Column<string>(type: "text", nullable: false),
                    ItemsPerPage = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.UserID);
                    table.ForeignKey(
                        name: "FK_UserSettings_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSettings");
        }
    }
}
