using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CheckInGWDN.Data.Migrations
{
    public partial class CreateDownloadTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Downloads",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    ZipPath = table.Column<string>(nullable: true),
                    CreateAt = table.Column<DateTime>(nullable: false),
                    UpdateAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Downloads", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Downloads");
        }
    }
}
