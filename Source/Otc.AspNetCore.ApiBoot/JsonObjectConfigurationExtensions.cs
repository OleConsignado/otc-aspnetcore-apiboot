using Otc.AspNetCore.ApiBoot;

namespace Microsoft.Extensions.Configuration
{
    public static class JsonObjectConfigurationExtensions
    {
        public static IConfigurationBuilder AddJsonObject<T>(this IConfigurationBuilder builder, T configurationObject)
            where T : class
        {
            return builder.Add(new JsonObjectConfigurationSource(configurationObject));
        }
    }
}