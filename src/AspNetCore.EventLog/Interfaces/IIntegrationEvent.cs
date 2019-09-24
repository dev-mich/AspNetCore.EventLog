using System;

namespace AspNetCore.EventLog.Interfaces
{
    public interface IIntegrationEvent
    {

        Guid Id { get; }

    }
}
