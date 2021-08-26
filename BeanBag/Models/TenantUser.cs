using System.ComponentModel.DataAnnotations;

namespace BeanBag.Models
{
    public class TenantUser
    {
        [Key]
        public string UserObjectId { get; set; }
        [Required]
        public string UserTenantId { get; set; }
        
        public string UserName { get; set; }
    }
}