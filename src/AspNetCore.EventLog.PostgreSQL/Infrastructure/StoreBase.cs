using AspNetCore.EventLog.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace AspNetCore.EventLog.PostgreSQL.Infrastructure
{
    internal abstract class StoreBase<TEntity> : IStore<TEntity> where TEntity: class
    {
        private readonly PostgresDbContext _context;

        protected StoreBase(PostgresDbContext context)
        {
            _context = context;
            DbSet = context.Set<TEntity>();
        }

        protected readonly DbSet<TEntity> DbSet;


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
            _context.Database.UseTransaction(transaction);
        }


        protected async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }

    }
}
