using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace AspNetCore.EventLog.Interfaces
{
    public interface IStore<T>
    {
        void UseTransaction(DbTransaction transaction);

        Task<bool> AddAsync(T entity);

        bool Add(T entity);

        Task<bool> AddAsync(IEnumerable<T> entity);

        Task<bool> UpdateAsync(T entity);

        bool Update(T entity);

        Task<T> FindAsync(object id);

        T Find(object id);

    }
}
