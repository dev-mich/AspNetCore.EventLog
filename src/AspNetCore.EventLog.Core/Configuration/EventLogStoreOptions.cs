using Newtonsoft.Json;

namespace AspNetCore.EventLog.DependencyInjection
{
    public class EventLogStoreOptions
    {

        public JsonSerializerSettings JsonSettings { get; set; }


    }
}
