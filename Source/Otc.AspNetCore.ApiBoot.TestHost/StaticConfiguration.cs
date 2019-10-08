﻿using Otc.AuthorizationContext.AspNetCore.Jwt;

namespace Otc.AspNetCore.ApiBoot.TestHost
{
    internal static class StaticConfiguration
    {
        internal static readonly JwtConfiguration jwtConfiguration = new JwtConfiguration()
        {
            Audience = "ole tecnologia",
            Issuer = "ole tecnologia",
            SecretKey = "abcdefghijklmnopqrs"
        };
    }
}
