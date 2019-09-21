using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AspNetCore.EventLog.PostgreSQL.Migrations
{
    public partial class AddReceived : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventLog_Received",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    EventName = table.Column<string>(maxLength: 50, nullable: false),
                    Content = table.Column<string>(type: "json", nullable: false),
                    ReceivedTime = table.Column<DateTime>(nullable: false),
                    EventState = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventLog_Received", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventLog_Received_ReceivedTime",
                table: "EventLog_Received",
                column: "ReceivedTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventLog_Received");
        }
    }
}
