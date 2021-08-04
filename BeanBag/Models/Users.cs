using System.ComponentModel.DataAnnotations;

namespace BeanBag.Models
{
    public class Users
    {
        [Key]
        public string UserObjectId { get; set; }
        
        [Required]
        public string UserTenantId { get; set; }
    }
}