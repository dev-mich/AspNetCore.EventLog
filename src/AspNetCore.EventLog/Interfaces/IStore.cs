using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace AspNetCore.EventLog.Interfaces
{
    public interface IStore<T>
    {
        void UseTransaction(DbTransaction transaction);

        Task<bool> AddAsync(T entity);

        Task<bool> AddAsync(IEnumerable<T> entity);

        Task<bool> UpdateAsync(T entity);

        Task<T> FindAsync(object id);

    }
}
