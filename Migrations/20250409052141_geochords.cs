using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TSBackend.Migrations
{
    /// <inheritdoc />
    public partial class geochords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "latitude",
                table: "places",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "longitude",
                table: "places",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "latitude",
                table: "places");

            migrationBuilder.DropColumn(
                name: "longitude",
                table: "places");
        }
    }
}
