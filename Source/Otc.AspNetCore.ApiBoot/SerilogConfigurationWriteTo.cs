using Otc.ComponentModel.DataAnnotations;

namespace Otc.AspNetCore.ApiBoot
{
    public class SerilogConfigurationWriteTo
    {
        [Required]
        public string Name { get; set; }
    }
}