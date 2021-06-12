using BeanBag.Database;
using BeanBag.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        // Get method for create
        // Returns Create view
        public IActionResult Create()
        {
            IEnumerable<SelectListItem> InventoryDropDown = _db.Inventories.Select(i => new SelectListItem
            {
                Text = i.name,
                Value = i.Id.ToString()
            });

            ViewBag.InventoryDropDown = InventoryDropDown;

            return View();
        }

        // Post method for create
        // Takes in form from Create view to add a new item to the DB
        [HttpPost]
        public IActionResult Create(Item newItem)
        {
            if(ModelState.IsValid)
            {
                _db.Items.Add(newItem);
                _db.SaveChanges();
                
                return LocalRedirect("/Inventory/ViewItems?InventoryId="+newItem.inventoryId.ToString());
            }
            // Redirect to the inventory item view page
            return View();
        }

        public IActionResult Edit(Guid? Id)
        {
            if(Id == null)
            {
                return NotFound();
            }

            var item = _db.Items.Find(Id);
            if(item == null)
            {
                return NotFound();
            }

            IEnumerable<SelectListItem> InventoryDropDown = _db.Inventories.Select(i => new SelectListItem
            {
                Text = i.name,
                Value = i.Id.ToString()
            });

            ViewBag.InventoryDropDown = InventoryDropDown;

            return View(item);
        }

        [HttpPost]
        public IActionResult EditPost(Item item)
        {
            if(ModelState.IsValid)
            {
                _db.Items.Update(item);
                _db.SaveChanges();

                return LocalRedirect("/Inventory/ViewItems?InventoryId=" + item.inventoryId.ToString());
            }

            return View();
        }

        public IActionResult Delete(Guid? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var item = _db.Items.Find(Id);
            if (item == null)
            {
                return NotFound();
            }

            ViewBag.InventoryName = _db.Inventories.Find(item.inventoryId).name;
            return View(item);
        }

        [HttpPost]
        public IActionResult DeletePost(Guid? Id)
        {
            var item = _db.Items.Find(Id);

            if(item == null)
            {
                return NotFound();
            }

            _db.Items.Remove(item);
            _db.SaveChanges();
            return LocalRedirect("/Inventory/ViewItems?InventoryId=" + item.inventoryId.ToString());
        }
    }
}
