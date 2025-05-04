using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureNotes.Migrations
{
    public partial class AddHmacToNotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Hmac",
                table: "Notes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hmac",
                table: "Notes");
        }
    }
}
