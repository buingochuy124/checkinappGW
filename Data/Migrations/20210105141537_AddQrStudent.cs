using Microsoft.EntityFrameworkCore.Migrations;

namespace CheckInGWDN.Data.Migrations
{
    public partial class AddQrStudent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Qr",
                table: "Students",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Qr",
                table: "Students");
        }
    }
}
