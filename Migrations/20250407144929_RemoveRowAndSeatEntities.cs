using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TSBackend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRowAndSeatEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tickets_seats_seat_id",
                table: "tickets");

            migrationBuilder.DropTable(
                name: "seats");

            migrationBuilder.DropTable(
                name: "rows");

            migrationBuilder.DropIndex(
                name: "IX_tickets_seat_id",
                table: "tickets");

            migrationBuilder.RenameColumn(
                name: "seat_id",
                table: "tickets",
                newName: "seat_number");

            migrationBuilder.RenameIndex(
                name: "IX_tickets_meeting_id_seat_id",
                table: "tickets",
                newName: "IX_tickets_meeting_id_seat_number");

            migrationBuilder.AddColumn<int>(
                name: "total_seats",
                table: "places",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "total_seats",
                table: "places");

            migrationBuilder.RenameColumn(
                name: "seat_number",
                table: "tickets",
                newName: "seat_id");

            migrationBuilder.RenameIndex(
                name: "IX_tickets_meeting_id_seat_number",
                table: "tickets",
                newName: "IX_tickets_meeting_id_seat_id");

            migrationBuilder.CreateTable(
                name: "rows",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    place_id = table.Column<int>(type: "integer", nullable: false),
                    row_number = table.Column<int>(type: "integer", nullable: false),
                    seats_per_row = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rows", x => x.id);
                    table.ForeignKey(
                        name: "FK_rows_places_place_id",
                        column: x => x.place_id,
                        principalTable: "places",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "seats",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    row_id = table.Column<int>(type: "integer", nullable: false),
                    seat_number = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seats", x => x.id);
                    table.ForeignKey(
                        name: "FK_seats_rows_row_id",
                        column: x => x.row_id,
                        principalTable: "rows",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tickets_seat_id",
                table: "tickets",
                column: "seat_id");

            migrationBuilder.CreateIndex(
                name: "IX_rows_place_id_row_number",
                table: "rows",
                columns: new[] { "place_id", "row_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_seats_row_id_seat_number",
                table: "seats",
                columns: new[] { "row_id", "seat_number" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_tickets_seats_seat_id",
                table: "tickets",
                column: "seat_id",
                principalTable: "seats",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
