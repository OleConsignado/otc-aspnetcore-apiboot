using Microsoft.Extensions.Logging;

namespace Otc.AspNetCore.ApiBoot.Tests
{
    internal class TestLoggerFactory : ILoggerFactory
    {
        public void AddProvider(ILoggerProvider provider)
        {

        }

        public ILogger CreateLogger(string categoryName)
        {
            return null;
        }

        public void Dispose()
        {

        }
    }
}