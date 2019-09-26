using AspNetCore.EventLog.DependencyInjection;
using AspNetCore.EventLog.Interfaces;
using AspNetCore.EventLog.PostgreSQL.Extensions;
using AspNetCore.EventLog.RabbitMQ.Config;
using AspNetCore.EventLog.RabbitMQ.Extensions;
using AspNetCore.EventLog.Sample1.EventBus;
using AspNetCore.EventLog.Sample1.Infrastructure;
using AspNetCore.EventLog.Sample1.IntegrationEvents;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.EventLog.Sample1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connString = "Server=db;port=5432;Database=test;UserId=postgres;Password=postgres;";

            services.AddMvc();

            services.AddEntityFrameworkNpgsql().AddDbContext<TestDbContext>(opts => opts.UseNpgsql(connString));

            services.AddEventLog(bld =>
            {
                bld.UsePostgres(pg =>
                {
                    pg.ConnectionString = connString;
                    pg.DefaultSchema = "test_event_log";
                });
                bld.AddRabbitmq(new RabbitMqConfiguration
                {
                    HostName = "eventbus",
                    Port = 5672,
                    Username = "rabbit",
                    Password = "rabbit",
                    ExchangeResolver = typeof(RabbitMQExchangeResolver),
                    QueueResolver = typeof(RabbitMQQueueResolver)
                });
            });

            // register integration event handlers
            services.AddScoped<IEventHandler<TestIntegrationEvent>, TestIntegrationEventHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseMvc();

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var receiverService = scope.ServiceProvider.GetService<IReceiverService>();

                receiverService.Subscribe<TestIntegrationEvent>("test.event");
            }
        }
    }
}
