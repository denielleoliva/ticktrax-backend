using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ticktraxbackend.Migrations
{
    /// <inheritdoc />
    public partial class ChangeToCoords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Submissions");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Submissions",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Submissions",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Submissions");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Submissions",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
