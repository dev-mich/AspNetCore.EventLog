using System;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using AspNetCore.EventLog.Interfaces;

namespace AspNetCore.EventLog.PostgreSQL.Infrastructure
{
    class PostgreSQLMigrator : IDbMigrator
    {
        private readonly DbContextFactory _contextFactory;

        public PostgreSQLMigrator(DbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task MigrateAsync()
        {
            try
            {
                await _contextFactory.Context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                throw;
            }
        }
    }
}
