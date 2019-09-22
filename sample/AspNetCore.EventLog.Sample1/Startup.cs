using AspNetCore.EventLog.DependencyInjection;
using AspNetCore.EventLog.PostgreSQL.Extensions;
using AspNetCore.EventLog.RabbitMQ.Config;
using AspNetCore.EventLog.RabbitMQ.Extensions;
using AspNetCore.EventLog.Sample1.EventBus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            services.AddMvc();

            services.AddEventLog(bld =>
            {
                bld.UsePostgres(pg =>
                {
                    pg.ConnectionString =
                        "Server=db;port=5432;Database=test;UserId=postgres;Password=postgres;";
                    pg.DefaultSchema = "test_event_log";
                });
                bld.AddRabbitmq(new RabbitMqConfiguration
                {
                    HostName = "localhost",
                    Port = 5672,
                    Username = "rabbit",
                    Password = "rabbit",
                    ExchangeResolver = typeof(RabbitMQExchangeResolver)
                });
            });
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

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
