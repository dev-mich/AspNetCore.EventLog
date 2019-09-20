using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AspNetCore.EventLog.PostgreSQL.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventLog_Published",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PublisherName = table.Column<string>(maxLength: 150, nullable: false),
                    EventName = table.Column<string>(maxLength: 50, nullable: false),
                    Content = table.Column<string>(type: "json", nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    EventState = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventLog_Published", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventLog_Published_CreationTime",
                table: "EventLog_Published",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_EventLog_Published_PublisherName",
                table: "EventLog_Published",
                column: "PublisherName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventLog_Published");
        }
    }
}
