using Otc.AuthorizationContext.Abstractions;

namespace Otc.AspNetCore.ApiBoot.Example.Models
{
    public class AuthorizationData : IAuthorizationData
    {
        public string UserId { get; set; }

        public string MyCustomAuthData { get; set; }
    }
}
