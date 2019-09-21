﻿using System;
using System.Collections.Generic;
using AspNetCore.EventLog.Abstractions.DependencyInjection;
using Newtonsoft.Json;

namespace AspNetCore.EventLog.Core.Configuration
{
    public class EventLogOptions
    {

        public JsonSerializerSettings JsonSettings { get; set; }

        public IList<IExtension> Extensions { get; }

        public EventLogOptions()
        {
            Extensions = new List<IExtension>();
        }

        public void RegisterExtension(IExtension extension)
        {
            if (extension == null)
                throw new ArgumentNullException(nameof(extension));

            Extensions.Add(extension);
        }

    }
}
