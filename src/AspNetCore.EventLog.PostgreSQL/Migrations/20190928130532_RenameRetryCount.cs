using Microsoft.EntityFrameworkCore.Migrations;

namespace AspNetCore.EventLog.PostgreSQL.Migrations
{
    public partial class RenameRetryCount : Migration
    {
        private readonly string _schema;

        public RenameRetryCount(string schema)
        {
            _schema = schema;
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                schema: _schema,
                name: "RetryCount",
                table: "EventLog_Received",
                newName: "FailCount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                schema: _schema,
                name: "FailCount",
                table: "EventLog_Received",
                newName: "RetryCount");
        }
    }
}
