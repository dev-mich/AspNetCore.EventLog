using AspNetCore.EventLog.Abstractions.Persistence;
using AspNetCore.EventLog.PostgreSQL.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace AspNetCore.EventLog.PostgreSQL.Infrastructure
{
    internal abstract class StoreBase<TEntity> : IStore<TEntity> where TEntity: class
    {
        private readonly PostgreSqlOptions _options;
        private PostgresDbContext _context;

        protected StoreBase(IOptions<PostgreSqlOptions> options)
        {
            _options = options.Value;
        }

        protected DbSet<TEntity> DbSet { get; private set; }


        public async Task<bool> AddAsync(TEntity entity)
        {
            await DbSet.AddAsync(entity);

            return await SaveChanges();
        }

        public async Task<bool> AddAsync(IEnumerable<TEntity> entity)
        {
            await DbSet.AddRangeAsync(entity);

            return await SaveChanges();
        }

        public async Task<bool> UpdateAsync(TEntity entity)
        {
            DbSet.Update(entity);
            _context.Entry(entity).State = EntityState.Modified;

            return await SaveChanges();

        }

        public Task<TEntity> FindAsync(object id)
        {
            return DbSet.FindAsync(id);
        }

        public void UseTransaction(DbTransaction transaction)
        {
            _context = new PostgresDbContext(transaction.Connection, _options);
            DbSet = _context.Set<TEntity>();
            _context.Database.UseTransaction(transaction);
        }


        protected async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }

    }
}
