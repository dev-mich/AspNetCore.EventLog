using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AspNetCore.EventLog.PostgreSQL.Migrations
{
    public partial class PublishedRefactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventLog_Published_PublisherName",
                table: "EventLog_Published");

            migrationBuilder.DropColumn(
                name: "PublisherName",
                table: "EventLog_Published");

            migrationBuilder.AddColumn<Guid>(
                name: "TransactionId",
                table: "EventLog_Published",
                maxLength: 40,
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_EventLog_Published_TransactionId",
                table: "EventLog_Published",
                column: "TransactionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventLog_Published_TransactionId",
                table: "EventLog_Published");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "EventLog_Published");

            migrationBuilder.AddColumn<string>(
                name: "PublisherName",
                table: "EventLog_Published",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_EventLog_Published_PublisherName",
                table: "EventLog_Published",
                column: "PublisherName");
        }
    }
}
