using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;

namespace Otc.AspNetCore.ApiBoot
{
    internal class JsonObjectConfigurationSource : JsonConfigurationSource
    {
        private readonly object configurationObject;

        public JsonObjectConfigurationSource(object configurationObject)
        {
            this.configurationObject = configurationObject ?? throw new ArgumentNullException(nameof(configurationObject));
        }

        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new JsonObjectConfigurationProvider(this, configurationObject);
        }
    }
}