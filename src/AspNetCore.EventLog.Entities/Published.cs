using System;
using Newtonsoft.Json;

namespace AspNetCore.EventLog.Entities
{
    public class Published
    {

        public Guid Id { get; set; }


        public Guid TransactionId { get; set; }


        public string EventName { get; set; }


        public string Content { get; set; }


        public DateTime CreationTime { get; set; }


        public PublishedState EventState { get; set; }


        public static Published CreateEventLog(Guid transactionId, string eventName, object @event, JsonSerializerSettings settings)
        {
            return new Published
            {
                Id = Guid.NewGuid(),
                TransactionId = transactionId,
                EventName = eventName,
                Content = JsonConvert.SerializeObject(@event, settings),
                CreationTime = DateTime.UtcNow,
                EventState = PublishedState.NotPublished
            };

        }

    }


    public enum PublishedState
    {
        NotPublished = 1,
        InProgress,
        Published,
        PublishedFailed
    }

}
