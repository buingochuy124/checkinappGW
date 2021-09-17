using Microsoft.EntityFrameworkCore.Migrations;

namespace CheckInGWDN.Data.Migrations
{
    public partial class AddStatusEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "status",
                table: "Events",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "Events");
        }
    }
}
