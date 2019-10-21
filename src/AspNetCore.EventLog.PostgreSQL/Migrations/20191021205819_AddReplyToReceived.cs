using Microsoft.EntityFrameworkCore.Migrations;

namespace AspNetCore.EventLog.PostgreSQL.Migrations
{
    public partial class AddReplyToReceived : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CorrelationId",
                table: "EventLog_Received",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReplyTo",
                table: "EventLog_Received",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrelationId",
                table: "EventLog_Received");

            migrationBuilder.DropColumn(
                name: "ReplyTo",
                table: "EventLog_Received");
        }
    }
}
