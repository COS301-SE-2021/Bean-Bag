using System.ComponentModel.DataAnnotations;

namespace BeanBag.Models
{
    public class UserRoles
    {
        [Key]
        public string userId { get; set; }

        [Required]
        public string role { get; set; }
    }
}
