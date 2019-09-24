﻿using System;
using System.Threading.Tasks;
using AspNetCore.EventLog.Entities;

namespace AspNetCore.EventLog.Interfaces
{
    public interface IMessageProcessor
    {
        Task PersistEvent(Guid eventId, string eventName, string content);

        Task<bool> Process(string eventName, string content);

        Task<bool> Process(Received @event);

    }

}