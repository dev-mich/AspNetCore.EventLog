using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace AspNetCore.EventLog.Infrastructure
{
    public class EventLogTransaction : IDisposable
    {

        private readonly DbTransaction _transaction;

        public EventLogTransaction(DbTransaction transaction)
        {
            _transaction = transaction;
        }


        public delegate Task Committed();
        public event Committed OnCommit;

        public DbTransaction DbTransaction => _transaction;


        public void Commit()
        {
            _transaction.Commit();
            OnCommit?.Invoke();
        }


        public void Rollback()
        {
            _transaction.Rollback();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
        }
    }
}
