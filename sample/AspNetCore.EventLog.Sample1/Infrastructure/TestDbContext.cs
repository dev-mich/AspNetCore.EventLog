using AspNetCore.EventLog.Sample1.Entities;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.EventLog.Sample1.Infrastructure
{
    public class TestDbContext : DbContext
    {

        public DbSet<TestEntity> TestEntities { get; set; }

        public TestDbContext(DbContextOptions<TestDbContext> options): base(options) { }

    }
}
