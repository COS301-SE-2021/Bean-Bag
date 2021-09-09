using System.ComponentModel.DataAnnotations;

namespace BeanBag.Models
{
    public class Tenant
    {
        [Key]
        public string TenantId { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "Tenant name length can't be more than 20.")]
        public string TenantName { get; set; }
        public string TenantTheme { get; set; }
        public string TenantLogo { get; set; }
    }
}