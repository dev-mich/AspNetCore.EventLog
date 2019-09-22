using System;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace AspNetCore.EventLog.EventBus.Test.Utils
{
    public static class ServiceProviderUtils
    {

        public static void ResolveService<TService>(this Mock<IServiceProvider> serviceProviderMock, Mock<TService> result) where TService : class
        {
            serviceProviderMock
                .Setup(x => x.GetService(typeof(TService)))
                .Returns(result);

            var serviceScope = new Mock<IServiceScope>();
            serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);

            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            serviceScopeFactory
                .Setup(x => x.CreateScope())
                .Returns(serviceScope.Object);

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactory.Object);

        }

    }
}
