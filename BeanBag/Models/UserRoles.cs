using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
