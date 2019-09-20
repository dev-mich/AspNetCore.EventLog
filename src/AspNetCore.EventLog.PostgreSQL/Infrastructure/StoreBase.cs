using AspNetCore.EventLog.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace AspNetCore.EventLog.PostgreSQL.Infrastructure
{
    internal abstract class StoreBase<TEntity> : IStore<TEntity> where TEntity: class
    {
        private readonly PostgresDbContext _context;

        public StoreBase(PostgresDbContext context)
        {
            _context = context;
            DbSet = context.Set<TEntity>();
        }

        protected readonly DbSet<TEntity> DbSet;


        public async Task<bool> AddAsync(TEntity entity)
        {
            try
            {
                await DbSet.AddAsync(entity);

                await SaveChanges();

                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public async Task<bool> AddAsync(IEnumerable<TEntity> entity)
        {
            try
            {
                await DbSet.AddRangeAsync(entity);

                await SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(TEntity entity)
        {
            try
            {
                DbSet.Update(entity);
                _context.Entry(entity).State = EntityState.Modified;

                await SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }

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
