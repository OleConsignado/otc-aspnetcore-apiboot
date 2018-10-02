using Otc.AuthorizationContext.Abstractions;
using Otc.AuthorizationContext.AspNetCore.Jwt;
using System;
using System.Net.Http;

namespace Otc.AspNetCore.ApiBoot.TestHost
{
    public static class HttpClientExtensions
    {
        public static HttpClient AddAuthorization(this HttpClient httpClient, IAuthorizationData authorizationData)
        {
            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }

            var token = new AuthorizationDataSerializer<IAuthorizationData>(StaticConfiguration.JwtConfiguration).Serialize(authorizationData);

            if (httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                httpClient.DefaultRequestHeaders.Remove("Authorization");
            }

            httpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");

            return httpClient;
        }

        // TODO: Replace Otc.SessionContext by Otc.AuthorizationContext and remove this method
#pragma warning disable 618
        public static HttpClient AddAuthorization(this HttpClient httpClient, SessionContext.Abstractions.ISessionData sessionData)
#pragma warning restore 618
        {
            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }

#pragma warning disable 618
            var token = new SessionContext.AspNetCore.Jwt.SessionSerializer<SessionContext.Abstractions.ISessionData>(new SessionContext.AspNetCore.Jwt.JwtConfiguration()
#pragma warning restore 618
            {
                Audience = StaticConfiguration.JwtConfiguration.Audience,
                Issuer = StaticConfiguration.JwtConfiguration.Issuer,
                SecretKey = StaticConfiguration.JwtConfiguration.SecretKey
            }).Serialize(sessionData);

            if (httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                httpClient.DefaultRequestHeaders.Remove("Authorization");
            }

            httpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");

            return httpClient;
        }
    }
}
