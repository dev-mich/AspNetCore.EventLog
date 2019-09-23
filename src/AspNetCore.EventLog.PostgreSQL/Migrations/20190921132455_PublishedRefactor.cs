using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AspNetCore.EventLog.PostgreSQL.Migrations
{
    public partial class PublishedRefactor : Migration
    {
        private readonly string _schema;

        public PublishedRefactor(string schema)
        {
            _schema = schema;
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventLog_Published_PublisherName",
                table: "EventLog_Published", schema: _schema);

            migrationBuilder.DropColumn(
                name: "PublisherName",
                table: "EventLog_Published", schema: _schema);

            migrationBuilder.AddColumn<Guid>(
                name: "TransactionId",
                table: "EventLog_Published",
                schema: _schema,
                maxLength: 40,
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_EventLog_Published_TransactionId",
                table: "EventLog_Published",
                column: "TransactionId", schema: _schema);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventLog_Published_TransactionId",
                table: "EventLog_Published", schema: _schema);

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "EventLog_Published", schema: _schema);

            migrationBuilder.AddColumn<string>(
                name: "PublisherName",
                table: "EventLog_Published",
                schema: _schema,
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_EventLog_Published_PublisherName",
                table: "EventLog_Published",
                column: "PublisherName", schema: _schema);
        }
    }
}
