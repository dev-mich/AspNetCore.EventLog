using AspNetCore.EventLog.Core.Configuration;
using AspNetCore.EventLog.PostgreSQL.Infrastructure;

namespace AspNetCore.EventLog.PostgreSQL.Extensions
{
    public static class DependencyInjection
    {

        public static void UsePostgres(this EventLogOptions options, string connectionString)
        {
            options.RegisterExtension(new PostgreSqlExtension(connectionString));
        }


    }
}
