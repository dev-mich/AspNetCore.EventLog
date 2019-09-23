using System;
using System.Data.Common;
using AspNetCore.EventLog.PostgreSQL.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AspNetCore.EventLog.PostgreSQL.Infrastructure
{
    internal class DbContextFactory : IDisposable
    {

        private DbTransaction _currentTransaction;
        private readonly IOptions<PostgreSqlOptions> _setupOptions;
        private PostgresDbContext _context;

        public DbContextFactory(IOptions<PostgreSqlOptions> setupOptions)
        {
            _setupOptions = setupOptions;
        }

        public void UseTransaction(DbTransaction transaction)
        {
            if (_currentTransaction == null)
            {
                _currentTransaction = transaction;
                Context.Database.UseTransaction(transaction);
            }
        }

        public PostgresDbContext Context
        {
            get
            {
                if (_context != null)
                    return _context;

                var optionBuilder = new DbContextOptionsBuilder<PostgresDbContext>();

                if (_currentTransaction != null)
                {
                    optionBuilder.UseNpgsql(_currentTransaction.Connection);

                    _context = new PostgresDbContext(optionBuilder.Options, _setupOptions);
                }
                else
                {
                    optionBuilder.UseNpgsql(_setupOptions.Value.ConnectionString);

                    _context = new PostgresDbContext(optionBuilder.Options, _setupOptions);
                }

                return _context;

            }
        }

        public void Dispose()
        {
            _currentTransaction?.Dispose();
            _context?.Dispose();
        }
    }
}
