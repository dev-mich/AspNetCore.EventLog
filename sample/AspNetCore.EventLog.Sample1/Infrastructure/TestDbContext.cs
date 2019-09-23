using Microsoft.EntityFrameworkCore;

namespace AspNetCore.EventLog.Sample1.Infrastructure
{
    public class TestDbContext : DbContext
    {

        public TestDbContext(DbContextOptions<TestDbContext> options): base(options) { }

    }
}
