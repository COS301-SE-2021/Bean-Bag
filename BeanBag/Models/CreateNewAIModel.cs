using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace BeanBag.Models
{
    public class CreateNewAIModel
    {
        public string name { get; set; }

        public IFormFile image { get; set; }
    }
}
