using System;

namespace AspNetCore.EventLog.Entities
{
    public class Received
    {

        public Guid Id { get; set; }


        public string EventName { get; set; }


        public string Content { get; set; }


        public DateTime ReceivedTime { get; set; }


        public ReceivedState EventState { get; set; }


        public int RetryCount { get; set; }


        public Received(Guid id, string eventName, string content)
        {
            Id = id;
            EventName = eventName;
            Content = content;
            ReceivedTime = DateTime.UtcNow;
            EventState = ReceivedState.Received;
            RetryCount = 0;
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
