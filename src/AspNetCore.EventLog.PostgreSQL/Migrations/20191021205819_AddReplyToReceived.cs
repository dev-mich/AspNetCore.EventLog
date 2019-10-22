using Microsoft.EntityFrameworkCore.Migrations;

namespace AspNetCore.EventLog.PostgreSQL.Migrations
{
    public partial class AddReplyToReceived : Migration
    {

        private readonly string _schema;

        public AddReplyToReceived(string schema)
        {
            _schema = schema;
        }


        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                schema: _schema,
                name: "CorrelationId",
                table: "EventLog_Received",
                nullable: true); ;

            migrationBuilder.AddColumn<string>(
                schema: _schema,
                name: "ReplyTo",
                table: "EventLog_Received",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                schema: _schema,
                name: "CorrelationId",
                table: "EventLog_Received");

            migrationBuilder.DropColumn(
                schema: _schema,
                name: "ReplyTo",
                table: "EventLog_Received");
        }
    }
}
