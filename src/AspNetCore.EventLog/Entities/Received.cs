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


        public ReplyState ReplyState { get; set; }


        public string ReplyContent { get; set; }


        public DateTime? ReplySended { get; set; }


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
            ReplyState = string.IsNullOrEmpty(replyTo) ? ReplyState.NotNeeded : ReplyState.Waiting;
            ReplySended = null;
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


    public enum ReplyState
    {
        NotNeeded = 1,
        Waiting,
        Forwarded
    }
}
