using Microsoft.EntityFrameworkCore.Migrations;

namespace AspNetCore.EventLog.PostgreSQL.Migrations
{
    public partial class AddRetryCount : Migration
    {
        private readonly string _schema;

        public AddRetryCount(string schema)
        {
            _schema = schema;
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                schema: _schema,
                name: "RetryCount",
                table: "EventLog_Received",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<uint>(
                schema: _schema,
                name: "xmin",
                table: "EventLog_Received",
                type: "xid",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                schema: _schema,
                name: "xmin",
                table: "EventLog_Published",
                type: "xid",
                nullable: false,
                defaultValue: 0u);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                schema: _schema,
                name: "RetryCount",
                table: "EventLog_Received");

            migrationBuilder.DropColumn(
                schema: _schema,
                name: "xmin",
                table: "EventLog_Received");

            migrationBuilder.DropColumn(
                schema: _schema,
                name: "xmin",
                table: "EventLog_Published");
        }
    }
}
