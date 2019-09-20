using System.Threading.Tasks;

namespace AspNetCore.EventLog.Abstractions.Persistence
{
    public interface IDbMigrator
    {
        Task MigrateAsync();

    }
}
