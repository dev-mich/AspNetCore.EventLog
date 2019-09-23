using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AspNetCore.EventLog.PostgreSQL.Migrations
{
    public partial class InitialMigration : Migration
    {
        private readonly string _schema;

        public InitialMigration(string schema)
        {
            _schema = schema;
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(_schema);

            migrationBuilder.CreateTable(
                schema: _schema,
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
                column: "CreationTime",
                schema: _schema);

            migrationBuilder.CreateIndex(
                name: "IX_EventLog_Published_PublisherName",
                table: "EventLog_Published",
                column: "PublisherName",
                schema: _schema);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventLog_Published", schema: _schema);
        }
    }
}
