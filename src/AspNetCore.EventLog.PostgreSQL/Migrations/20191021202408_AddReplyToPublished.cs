using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AspNetCore.EventLog.PostgreSQL.Migrations
{
    public partial class AddReplyToPublished : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CorrelationId",
                table: "EventLog_Published",
                nullable: true,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReplyTo",
                table: "EventLog_Published",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReplyTo",
                table: "EventLog_Published");

            migrationBuilder.AlterColumn<Guid>(
                name: "CorrelationId",
                table: "EventLog_Published",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
