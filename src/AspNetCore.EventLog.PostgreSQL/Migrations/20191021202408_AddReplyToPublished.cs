using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AspNetCore.EventLog.PostgreSQL.Migrations
{
    public partial class AddReplyToPublished : Migration
    {
        private readonly string _schema;

        public AddReplyToPublished(string schema)
        {
            _schema = schema;
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                schema: _schema,
                name: "CorrelationId",
                table: "EventLog_Published",
                nullable: true,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                schema: _schema,
                name: "ReplyTo",
                table: "EventLog_Published",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                schema: _schema,
                name: "ReplyTo",
                table: "EventLog_Published");

            migrationBuilder.AlterColumn<Guid>(
                schema: _schema,
                name: "CorrelationId",
                table: "EventLog_Published",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
