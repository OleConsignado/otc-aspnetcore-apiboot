using System;
using System.Collections.Generic;

namespace Otc.AspNetCore.ApiBoot
{
    [Serializable]
    public class SerilogConfigurationWriteTo
    {
        public string Name { get; set; }
        public IDictionary<string, object> Args { get; set; }
    }
}