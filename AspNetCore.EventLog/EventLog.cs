﻿using System;
using Newtonsoft.Json;

namespace AspNetCore.EventLog
{
    public class EventLog
    {

        public Guid Id { get; set; }


        public Guid TransactionId { get; set; }


        public string EventAssemblyName { get; set; }


        public string EventTypeName { get; set; }


        public string Content { get; set; }


        public DateTime CreationTime { get; set; }


        public EventState EventState { get; set; }


        public EventLog CreateEventLog(object @event, Guid transactionId, JsonSerializerSettings settings)
        {
            return new EventLog
            {
                Id = Guid.NewGuid(),
                TransactionId = transactionId,
                EventTypeName = @event.GetType().FullName,
                EventAssemblyName = @event.GetType().Assembly.FullName,
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
