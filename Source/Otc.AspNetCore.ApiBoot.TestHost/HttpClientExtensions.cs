using Otc.AuthorizationContext.Abstractions;
using Otc.AuthorizationContext.AspNetCore.Jwt;
using System;
using System.Net.Http;

namespace Otc.AspNetCore.ApiBoot.TestHost
{
    public static class HttpClientExtensions
    {
        public static HttpClient AddAuthorization(this HttpClient httpClient, 
            IAuthorizationData authorizationData)
        {
            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }

            var token = new AuthorizationDataSerializer<IAuthorizationData>(
                StaticConfiguration.jwtConfiguration).Serialize(authorizationData);

            if (httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                httpClient.DefaultRequestHeaders.Remove("Authorization");
            }

            httpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");

            return httpClient;
        }
    }
}
