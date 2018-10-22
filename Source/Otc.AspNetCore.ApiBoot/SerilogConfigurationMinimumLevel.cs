using System;
using System.Collections.Generic;

namespace Otc.AspNetCore.ApiBoot
{
    [Serializable]
    public class SerilogConfigurationMinimumLevel
    {
        public string Default { get; set; }

        public IDictionary<string, string> Override { get; set; }
    }
}