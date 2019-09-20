using System;
using Newtonsoft.Json;

namespace AspNetCore.EventLog.Entities
{
    public class Published
    {

        public Guid Id { get; set; }


        public string PublisherName { get; set; }


        public string EventName { get; set; }


        public string Content { get; set; }


        public DateTime CreationTime { get; set; }


        public EventState EventState { get; set; }


        public static Published CreateEventLog(string publisherName, string eventName, object @event, JsonSerializerSettings settings)
        {
            return new Published
            {
                Id = Guid.NewGuid(),
                PublisherName = publisherName,
                EventName = eventName,
                Content = JsonConvert.SerializeObject(@event, settings),
                CreationTime = DateTime.UtcNow,
                EventState = EventState.NotPublished
            };

        }

    }


    public enum EventState
    {
        NotPublished = 1,
        InProgress,
        Published,
        PublishedFailed
    }

}
