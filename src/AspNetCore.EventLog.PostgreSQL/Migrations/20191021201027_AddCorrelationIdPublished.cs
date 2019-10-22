using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AspNetCore.EventLog.PostgreSQL.Migrations
{
    public partial class AddCorrelationIdPublished : Migration
    {
        private readonly string _schema;

        public AddCorrelationIdPublished(string schema)
        {
            _schema = schema;
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                schema: _schema,
                name: "Content",
                table: "EventLog_Published",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "json");

            migrationBuilder.AddColumn<Guid>(
                schema: _schema,
                name: "CorrelationId",
                table: "EventLog_Published",
                nullable: true); ;
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                schema: _schema,
                name: "CorrelationId",
                table: "EventLog_Published");

            migrationBuilder.AlterColumn<string>(
                schema: _schema,
                name: "Content",
                table: "EventLog_Published",
                type: "json",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "jsonb");
        }
    }
}
