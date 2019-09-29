using System;
using System.Data.Common;
using System.Reflection;
using AspNetCore.EventLog.PostgreSQL.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Options;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

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


        public void CompleteTransaction()
        {
            _currentTransaction = null;
            _context = null;
        }

        public PostgresDbContext Context
        {
            get
            {
                if (_context != null)
                    return _context;

                var optionBuilder = new DbContextOptionsBuilder<PostgresDbContext>();

                void NpgsqlOptions(NpgsqlDbContextOptionsBuilder opts)
                {
                    opts.MigrationsAssembly(Assembly.GetAssembly(typeof(PostgresDbContext)).FullName);
                    opts.MigrationsHistoryTable("__EventLogMigrationHistory", _setupOptions.Value.DefaultSchema);
                }

                if (_currentTransaction != null)
                {
                    optionBuilder.UseNpgsql(_currentTransaction.Connection, NpgsqlOptions);
                }
                else
                {
                    optionBuilder.UseNpgsql(_setupOptions.Value.ConnectionString, NpgsqlOptions);
                }

                optionBuilder.ReplaceService<IMigrationsAssembly, SchemaAwareMigrationAssembly>();

                _context = new PostgresDbContext(optionBuilder.Options, _setupOptions);

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
