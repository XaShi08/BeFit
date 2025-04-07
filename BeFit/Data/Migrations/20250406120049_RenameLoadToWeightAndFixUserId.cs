using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeFit.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameLoadToWeightAndFixUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Load",
                table: "PerformedExercises",
                newName: "Weight");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "PerformedExercises",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PerformedExercises");

            migrationBuilder.RenameColumn(
                name: "Weight",
                table: "PerformedExercises",
                newName: "Load");
        }
    }
}
