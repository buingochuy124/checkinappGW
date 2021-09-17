using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CheckInGWDN.Data.Migrations
{
    public partial class AddTimeStudent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAt",
                table: "Students",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateAt",
                table: "Students",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "Students");
        }
    }
}
