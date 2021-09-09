using System.ComponentModel.DataAnnotations;

namespace BeanBag.Models
{
    public class UserRoles
    {
        [Key]
        public string userId { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "Role length can't be more than 100.")]
        public string role { get; set; }
    }
}
