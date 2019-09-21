using System;

namespace AspNetCore.EventLog.Abstractions.EventHandling
{
    public interface IIntegrationEvent
    {

        Guid Id { get; }

    }
}
