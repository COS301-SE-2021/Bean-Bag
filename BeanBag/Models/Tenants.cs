using System.ComponentModel.DataAnnotations;

namespace BeanBag.Models
{
    public class Tenants
    {
        [Key]
        public string TenantId { get; set; }
        
        [Required]
        public string TenantName { get; set; }
    }
}