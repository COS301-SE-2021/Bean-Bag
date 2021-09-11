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
        public string TenantEmail { get; set; }
        public string TenantNumber { get; set; }
        public string TenantAddress { get; set; }
        public string TenantTheme { get; set; }
        public string TenantLogo { get; set; }
        public string TenantSubscription { get; set; }
        
        public string InviteCode { get; set; }
    }
}