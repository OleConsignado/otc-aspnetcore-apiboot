using Otc.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Otc.AspNetCore.ApiBoot
{
    public class SerilogConfiguration
    {
        [Required]
        public IEnumerable<string> Using { get; set; }

        [Required]
        public SerilogConfigurationMinimumLevel MinimumLevel { get; set; }

        [Required]
        public IEnumerable<SerilogConfigurationWriteTo> WriteTo { get; set; }

        public IEnumerable<string> Enrich { get; set; }
    }
}