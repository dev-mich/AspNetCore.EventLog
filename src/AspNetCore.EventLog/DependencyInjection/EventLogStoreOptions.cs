using System;
using System.Data.Common;
using AspNetCore.EventLog.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AspNetCore.EventLog.DependencyInjection
{
    public class EventLogStoreOptions
    {

        public string DefaultSchema { get; set; }


        public Func<DbConnection, DbContextOptionsBuilder<EventLogDbContext>> ContextFactory { get; set; }


        public JsonSerializerSettings JsonSettings { get; set; }


    }
}
