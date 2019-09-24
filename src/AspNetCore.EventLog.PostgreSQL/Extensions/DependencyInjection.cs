using System;
using AspNetCore.EventLog.Configuration;
using AspNetCore.EventLog.PostgreSQL.Configuration;
using AspNetCore.EventLog.PostgreSQL.Infrastructure;

namespace AspNetCore.EventLog.PostgreSQL.Extensions
{
    public static class DependencyInjection
    {

        public static void UsePostgres(this EventLogOptions options, Action<PostgreSqlOptions> setupOptions)
        {
            options.RegisterExtension(new PostgreSqlExtension(setupOptions));
        }


    }
}
