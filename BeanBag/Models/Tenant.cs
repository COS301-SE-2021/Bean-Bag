using System.ComponentModel.DataAnnotations;

namespace BeanBag.Models
{
    public class Tenant
    {
        [Key]
        public string TenantId { get; set; }
        [Required]
        public string TenantName { get; set; }
        
        public string TenantEmail { get; set; }
        public string TenantTheme { get; set; }
        public string TenantLogo { get; set; }
    }
}