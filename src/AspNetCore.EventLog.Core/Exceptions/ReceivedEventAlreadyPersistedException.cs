﻿using System;

namespace AspNetCore.EventLog.Core.Exceptions
{
    public class ReceivedEventAlreadyPersistedException : Exception
    {

        public ReceivedEventAlreadyPersistedException(Guid id, string eventName): base($"event with name {eventName} already stored with id {id}") { }

    }
}
