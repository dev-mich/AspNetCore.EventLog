using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using AspNetCore.EventLog.Interfaces;

namespace AspNetCore.EventLog.PostgreSQL.Infrastructure
{
    internal abstract class StoreBase<TEntity> : IStore<TEntity> where TEntity: class
    {
        private readonly DbContextFactory _contextFactory;

        protected StoreBase(DbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        protected PostgresDbContext Context => _contextFactory.Context;
        private DbSet<TEntity> _dbSet;
        protected DbSet<TEntity> DbSet => _dbSet ?? (_dbSet = Context.Set<TEntity>());


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

            return await SaveChanges();

        }

        public Task<TEntity> FindAsync(object id)
        {
            return DbSet.FindAsync(id);
        }

        public void UseTransaction(DbTransaction transaction)
        {
            _contextFactory.UseTransaction(transaction);
        }


        protected async Task<bool> SaveChanges()
        {
            return await Context.SaveChangesAsync() > 0;
        }

    }
}
