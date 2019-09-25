using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AspNetCore.EventLog.PostgreSQL.Migrations
{
    public partial class RemoveTransactionId : Migration
    {
        private readonly string _schema;

        public RemoveTransactionId(string schema)
        {
            _schema = schema;
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                schema: _schema,
                name: "IX_EventLog_Published_TransactionId",
                table: "EventLog_Published");

            migrationBuilder.DropColumn(
                schema: _schema,
                name: "TransactionId",
                table: "EventLog_Published");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                schema: _schema,
                name: "TransactionId",
                table: "EventLog_Published",
                maxLength: 40,
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                schema: _schema,
                name: "IX_EventLog_Published_TransactionId",
                table: "EventLog_Published",
                column: "TransactionId");
        }
    }
}
