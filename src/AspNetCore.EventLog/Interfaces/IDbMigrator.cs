using System.Threading.Tasks;

namespace AspNetCore.EventLog.Interfaces
{
    public interface IDbMigrator
    {
        Task MigrateAsync();

    }
}
