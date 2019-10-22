using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AspNetCore.EventLog.PostgreSQL.Migrations
{
    public partial class AddReplyContentReceived : Migration
    {
        private readonly string _schema;

        public AddReplyContentReceived(string schema)
        {
            _schema = schema;
        }


        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                schema: _schema,
                name: "Content",
                table: "EventLog_Received",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "json");

            migrationBuilder.AddColumn<string>(
                schema: _schema,
                name: "ReplyContent",
                table: "EventLog_Received",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                schema: _schema,
                name: "ReplySended",
                table: "EventLog_Received",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                schema: _schema,
                name: "ReplyState",
                table: "EventLog_Received",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                schema: _schema,
                name: "ReplyContent",
                table: "EventLog_Received");

            migrationBuilder.DropColumn(
                schema: _schema,
                name: "ReplySended",
                table: "EventLog_Received");

            migrationBuilder.DropColumn(
                schema: _schema,
                name: "ReplyState",
                table: "EventLog_Received");

            migrationBuilder.AlterColumn<string>(
                schema: _schema,
                name: "Content",
                table: "EventLog_Received",
                type: "json",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "jsonb");
        }
    }
}
