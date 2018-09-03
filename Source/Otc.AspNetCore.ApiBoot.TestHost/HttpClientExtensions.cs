using Otc.SessionContext.Abstractions;
using Otc.SessionContext.AspNetCore.Jwt;
using System;
using System.Net.Http;

namespace Otc.AspNetCore.ApiBoot.TestHost
{
    public static class HttpClientExtensions
    {
        public static HttpClient AddAuthorization(this HttpClient httpClient, ISessionData sessionData)
        {
            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }

            var token = new SessionSerializer<ISessionData>(StaticConfiguration.JwtConfiguration).Serialize(sessionData);

            if (httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                httpClient.DefaultRequestHeaders.Remove("Authorization");
            }

            httpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");

            return httpClient;
        }
    }
}
