using System;
using AspNetCore.EventLog.Abstractions.Event;
using Newtonsoft.Json;

namespace AspNetCore.EventLog
{
    public class EventLog
    {

        public Guid Id { get; set; }


        public string PublisherName { get; set; }


        public string EventName { get; set; }


        public string Content { get; set; }


        public DateTime CreationTime { get; set; }


        public EventState EventState { get; set; }


        public static EventLog CreateEventLog(IntegrationEvent @event, Guid transactionId, JsonSerializerSettings settings)
        {
            return new EventLog
            {
                Id = Guid.NewGuid(),
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
