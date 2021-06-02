using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanBag.Models
{
    public class User 
    {
        //dummy
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; } = false;

        public List<Inventory> Inventories{ get; set; }

        public string activationCode;
        public DateTime activationDate;


    }
}
