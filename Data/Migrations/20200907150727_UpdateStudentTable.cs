using Microsoft.EntityFrameworkCore.Migrations;

namespace CheckInGWDN.Data.Migrations
{
    public partial class UpdateStudentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Students",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudentId",
                table: "Students",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "Students");
        }
    }
}
