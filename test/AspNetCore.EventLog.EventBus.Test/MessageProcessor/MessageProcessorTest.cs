using System;
using System.Threading.Tasks;
using AspNetCore.EventLog.Entities;
using AspNetCore.EventLog.Exceptions;
using AspNetCore.EventLog.Interfaces;
using Moq;
using Xunit;

namespace AspNetCore.EventLog.EventBus.Test.MessageProcessor
{
    public abstract class MessageProcessorTest
    {
        //protected Mock<IReceivedStore> ReceivedStoreMock;

        //protected MessageProcessorTest()
        //{
        //    ReceivedStoreMock = new Mock<IReceivedStore>();
        //}

        //protected abstract IMessageProcessor InitMessageProcessor();



        //[Fact]
        //public Task TestNotPersistedException()
        //{

        //    ReceivedStoreMock.Setup(s => s.AddAsync(It.IsAny<Received>())).Throws(It.IsAny<Exception>());

        //    return Assert.ThrowsAsync<ReceivedEventNotPersistedException>(() =>
        //        InitMessageProcessor().PersistEvent(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()));

        //}


        //[Fact]
        //public Task TestAlreadyPersistedException()
        //{
        //    var eventId = Guid.NewGuid();

        //    ReceivedStoreMock.Setup(s => s.FindAsync(eventId)).ReturnsAsync(new Received(eventId, "name", "content"));

        //    return Assert.ThrowsAsync<ReceivedEventAlreadyPersistedException>(() =>
        //        InitMessageProcessor().PersistEvent(eventId, It.IsAny<string>(), It.IsAny<string>()));

        //}


        //[Fact]
        //public Task TestPersistenceException()
        //{

        //    ReceivedStoreMock.Setup(s => s.UpdateAsync(It.IsAny<Received>())).Throws(It.IsAny<Exception>());

        //    return Assert.ThrowsAsync<PersistenceException>(() =>
        //        InitMessageProcessor().Process("eventName", "content"));

        //}


        //[Fact]
        //public async Task TestMissingEventName()
        //{
        //    await Assert.ThrowsAsync<ArgumentNullException>(() => InitMessageProcessor().Process(null, "content"));
        //    await Assert.ThrowsAsync<ArgumentNullException>(() => InitMessageProcessor().Process("", "content"));
        //}


        //[Fact]
        //public async Task TestMissingContent()
        //{
        //    await Assert.ThrowsAsync<ArgumentNullException>(() => InitMessageProcessor().Process("eventName", null));
        //    await Assert.ThrowsAsync<ArgumentNullException>(() => InitMessageProcessor().Process("eventName", ""));
        //}

        //public void Dispose()
        //{
        //    ReceivedStoreMock = new Mock<IReceivedStore>();
        //}
    }
}
