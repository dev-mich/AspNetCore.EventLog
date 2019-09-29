using System.ComponentModel.DataAnnotations;

namespace AspNetCore.EventLog.Sample1.Entities
{
    public class TestEntity
    {

        [Key]
        public int Id { get; set; }

        public string Content { get; set; }

    }
}
