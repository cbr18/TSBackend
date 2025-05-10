using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TSBackend.Migrations
{
    /// <inheritdoc />
    public partial class cleraly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tickets_meeting_id_seat_number",
                table: "tickets");

            migrationBuilder.DropColumn(
                name: "seat_number",
                table: "tickets");

            migrationBuilder.CreateIndex(
                name: "IX_tickets_meeting_id_user_id",
                table: "tickets",
                columns: new[] { "meeting_id", "user_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tickets_meeting_id_user_id",
                table: "tickets");

            migrationBuilder.AddColumn<int>(
                name: "seat_number",
                table: "tickets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_tickets_meeting_id_seat_number",
                table: "tickets",
                columns: new[] { "meeting_id", "seat_number" },
                unique: true);
        }
    }
}
