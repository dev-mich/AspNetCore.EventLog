using System;
using AspNetCore.EventLog.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AspNetCore.EventLog.PostgreSQL.Infrastructure
{
    class PostgreSQLMigrator : IDbMigrator
    {
        private readonly PostgresDbContext _context;

        public PostgreSQLMigrator(PostgresDbContext context)
        {
            _context = context;
        }

        public Task MigrateAsync()
        {
            try
            {
                return _context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                throw;
            }
        }
    }
}
