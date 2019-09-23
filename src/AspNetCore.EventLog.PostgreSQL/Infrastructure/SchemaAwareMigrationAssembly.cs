using AspNetCore.EventLog.PostgreSQL.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Reflection;

namespace AspNetCore.EventLog.PostgreSQL.Infrastructure
{
    public class SchemaAwareMigrationAssembly : MigrationsAssembly
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DbContext _context;

        public SchemaAwareMigrationAssembly(ICurrentDbContext currentContext, IDbContextOptions options, 
            IMigrationsIdGenerator idGenerator, IDiagnosticsLogger<DbLoggerCategory.Migrations> logger, IServiceProvider provider) : base(currentContext, options, idGenerator, logger)
        {
            _context = currentContext.Context;
            _serviceProvider = provider;
        }

        public override Migration CreateMigration(TypeInfo migrationClass, string activeProvider)
        {

            if (_context is PostgresDbContext context)
            {
                var instance = (Migration)Activator.CreateInstance(migrationClass.AsType(), context.Schema);
                instance.ActiveProvider = activeProvider;
                return instance;
            }

            return base.CreateMigration(migrationClass, activeProvider);
        }
    }
}
