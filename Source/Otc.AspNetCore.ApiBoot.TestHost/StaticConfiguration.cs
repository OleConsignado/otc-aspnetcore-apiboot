using Otc.SessionContext.AspNetCore.Jwt;

namespace Otc.AspNetCore.ApiBoot.TestHost
{
    internal static class StaticConfiguration
    {
        internal static readonly JwtConfiguration JwtConfiguration = new JwtConfiguration()
        {
            Audience = "ole tecnologia",
            Issuer = "ole tecnologia",
            SecretKey = "abcdefghijklmnopqrs"
        };
    }
}
