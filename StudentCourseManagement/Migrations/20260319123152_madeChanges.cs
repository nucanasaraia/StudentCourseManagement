using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentCourseManagement.Migrations
{
    /// <inheritdoc />
    public partial class madeChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PasswordResetToken",
                table: "Students",
                newName: "PasswordResetTokenHash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PasswordResetTokenHash",
                table: "Students",
                newName: "PasswordResetToken");
        }
    }
}
