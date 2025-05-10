using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TSBackend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCategoryEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rows_categories_category_id",
                table: "rows");

            migrationBuilder.DropForeignKey(
                name: "FK_tickets_categories_category_id",
                table: "tickets");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropIndex(
                name: "IX_tickets_category_id",
                table: "tickets");

            migrationBuilder.DropColumn(
                name: "category_id",
                table: "tickets");

            migrationBuilder.RenameColumn(
                name: "category_id",
                table: "rows",
                newName: "place_id");

            migrationBuilder.RenameIndex(
                name: "IX_rows_category_id_row_number",
                table: "rows",
                newName: "IX_rows_place_id_row_number");

            migrationBuilder.AddForeignKey(
                name: "FK_rows_places_place_id",
                table: "rows",
                column: "place_id",
                principalTable: "places",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rows_places_place_id",
                table: "rows");

            migrationBuilder.RenameColumn(
                name: "place_id",
                table: "rows",
                newName: "category_id");

            migrationBuilder.RenameIndex(
                name: "IX_rows_place_id_row_number",
                table: "rows",
                newName: "IX_rows_category_id_row_number");

            migrationBuilder.AddColumn<int>(
                name: "category_id",
                table: "tickets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    place_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    rows_per_category = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.id);
                    table.ForeignKey(
                        name: "FK_categories_places_place_id",
                        column: x => x.place_id,
                        principalTable: "places",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tickets_category_id",
                table: "tickets",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_categories_place_id_name",
                table: "categories",
                columns: new[] { "place_id", "name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_rows_categories_category_id",
                table: "rows",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tickets_categories_category_id",
                table: "tickets",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
