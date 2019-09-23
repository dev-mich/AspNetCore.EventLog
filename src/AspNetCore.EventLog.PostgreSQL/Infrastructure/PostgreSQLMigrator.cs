using System;
using AspNetCore.EventLog.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AspNetCore.EventLog.PostgreSQL.Infrastructure
{
    class PostgreSQLMigrator : IDbMigrator
    {
        private readonly DbContextFactory _contextFactory;

        public PostgreSQLMigrator(DbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public Task MigrateAsync()
        {
            try
            {
                return _contextFactory.Context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                throw;
            }
        }
    }
}
