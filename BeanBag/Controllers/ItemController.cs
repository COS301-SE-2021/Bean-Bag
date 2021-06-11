using BeanBag.Database;
using BeanBag.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BeanBag.Controllers
{
    public class ItemController : Controller
    {
        // This variable is used to interact with the Database/DBContext class. Allows us to save, update and delete records 
        private readonly DBContext _db;
        public ItemController(DBContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return Ok("Item Index");
        }
    }
}
