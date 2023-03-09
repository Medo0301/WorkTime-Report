using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportWorkTime.Migrations
{
    /// <inheritdoc />
    public partial class Migration3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfDays",
                table: "GoingOut",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TypeOfDay",
                table: "GoingOut",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfDays",
                table: "GoingOut");

            migrationBuilder.DropColumn(
                name: "TypeOfDay",
                table: "GoingOut");
        }
    }
}
