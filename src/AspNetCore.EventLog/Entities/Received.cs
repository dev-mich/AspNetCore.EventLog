using System;

namespace AspNetCore.EventLog.Entities
{
    public class Received
    {

        public Guid Id { get; set; }


        public string CorrelationId { get; set; }


        public string ReplyTo { get; set; }


        public string EventName { get; set; }


        public string Content { get; set; }


        public DateTime ReceivedTime { get; set; }


        public ReceivedState EventState { get; set; }


        public int FailCount { get; set; }


        public uint ConcurrencyToken { get; set; }


        public Received(Guid id, string eventName, string content, string replyTo, string correlationId)
        {
            Id = id;
            ReplyTo = replyTo;
            CorrelationId = correlationId;
            EventName = eventName;
            Content = content;
            ReceivedTime = DateTime.UtcNow;
            EventState = ReceivedState.Received;
            FailCount = 0;
        }


    }


    public enum ReceivedState
    {
        Received = 1,
        InProgress,
        Consumed,
        ConsumeFailed,
        Rejected
    }
}
