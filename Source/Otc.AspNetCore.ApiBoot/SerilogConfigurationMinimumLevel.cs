using Otc.ComponentModel.DataAnnotations;

namespace Otc.AspNetCore.ApiBoot
{
    public class SerilogConfigurationMinimumLevel
    {
        [Required]
        public string Default { get; set; }
    }
}