using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UniVerse.Migrations
{
    /// <inheritdoc />
    public partial class SeedModulesCoursesLecturers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserID", "Email", "FirstName", "LastLogin", "LastName", "PasswordHash", "PhoneNumber", "PreferredLanguage", "ProfilePhoto", "Role", "Username" },
                values: new object[,]
                {
                    { 1, "alice@universe.edu", "Alice", null, "Johnson", "hashed_pw_1", "0123456789", "English", null, "Lecturer", "alice" },
                    { 2, "brian@universe.edu", "Brian", null, "Smith", "hashed_pw_2", "0123456790", "English", null, "Lecturer", "brian" },
                    { 3, "chloe@universe.edu", "Chloe", null, "Adams", "hashed_pw_3", "0123456791", "English", null, "Lecturer", "chloe" },
                    { 4, "david@universe.edu", "David", null, "Nguyen", "hashed_pw_4", "0123456792", "English", null, "Lecturer", "david" }
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "CourseID", "CourseDescription", "CourseTitle", "Credits", "EndDate", "LecturerID", "StartDate" },
                values: new object[,]
                {
                    { "C001", "Learn the fundamentals of C# programming.", "Introduction to Programming", 12, new DateTime(2025, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "C002", "Introduction to HTML, CSS, and JavaScript.", "Web Development Basics", 10, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, new DateTime(2025, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "C003", "Learn SQL and relational databases.", "Database Management Systems", 15, new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "C004", "Develop Android apps using Kotlin.", "Mobile App Development", 14, new DateTime(2025, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Modules",
                columns: new[] { "ModuleID", "CompletionStatus", "ContentLink", "ContentType", "CourseID", "ModuleTitle" },
                values: new object[,]
                {
                    { "M001", "Incomplete", "https://example.com/csharp-basics", "Video", "C001", "C# Basics" },
                    { "M002", "Incomplete", "https://example.com/data-types", "Article", "C001", "Data Types and Variables" },
                    { "M003", "Incomplete", "https://example.com/control-structures", "Quiz", "C001", "Control Structures" },
                    { "M004", "Incomplete", "https://example.com/html", "Video", "C002", "HTML Fundamentals" },
                    { "M005", "Incomplete", "https://example.com/css", "Article", "C002", "CSS Styling" },
                    { "M006", "Incomplete", "https://example.com/js", "Video", "C002", "JavaScript Basics" },
                    { "M007", "Incomplete", "https://example.com/db-intro", "Video", "C003", "Introduction to Databases" },
                    { "M008", "Incomplete", "https://example.com/sql-queries", "Article", "C003", "SQL Queries" },
                    { "M009", "Incomplete", "https://example.com/db-design", "Quiz", "C003", "Database Design" },
                    { "M010", "Incomplete", "https://example.com/kotlin-basics", "Video", "C004", "Kotlin Basics" },
                    { "M011", "Incomplete", "https://example.com/ui-design", "Article", "C004", "UI Design with XML" },
                    { "M012", "Incomplete", "https://example.com/firebase", "Video", "C004", "Integrating Firebase" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "ModuleID",
                keyValue: "M001");

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "ModuleID",
                keyValue: "M002");

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "ModuleID",
                keyValue: "M003");

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "ModuleID",
                keyValue: "M004");

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "ModuleID",
                keyValue: "M005");

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "ModuleID",
                keyValue: "M006");

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "ModuleID",
                keyValue: "M007");

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "ModuleID",
                keyValue: "M008");

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "ModuleID",
                keyValue: "M009");

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "ModuleID",
                keyValue: "M010");

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "ModuleID",
                keyValue: "M011");

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "ModuleID",
                keyValue: "M012");

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseID",
                keyValue: "C001");

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseID",
                keyValue: "C002");

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseID",
                keyValue: "C003");

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseID",
                keyValue: "C004");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 4);
        }
    }
}
