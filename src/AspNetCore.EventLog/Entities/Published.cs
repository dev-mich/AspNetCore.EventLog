using System;

namespace AspNetCore.EventLog.Entities
{
    public class Published
    {

        public Guid Id { get; set; }


        public string CorrelationId { get; set; }


        public string ReplyTo { get; set; }


        public string EventName { get; set; }


        public string Content { get; set; }


        public DateTime CreationTime { get; set; }


        public PublishedState EventState { get; set; }


        public uint ConcurrencyToken { get; set; }


        public static Published CreateEventLog(Guid id, string eventName, string content, string correlationId = null, string replyTo = null)
        {
            return new Published
            {
                Id = id,
                CorrelationId = correlationId,
                ReplyTo = replyTo,
                EventName = eventName,
                Content = content,
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
