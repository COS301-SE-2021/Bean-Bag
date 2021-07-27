using BeanBag.Database;
using BeanBag.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Identity.Web;

namespace BeanBag.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly DBContext _db;

        public InventoryService(DBContext db)
        {
            _db = db;
        }

        public List<Inventory> GetInventories()
        {
            var inventories = (from i in _db.Inventories where i.userId.Equals("605ffb39-8901-440a-94c8-6c87d1b31825") select i).ToList();
            return inventories;
        }

        public void CreateInventory(Inventory newInventory)
        {
            _db.Inventories.Add(newInventory);
            _db.SaveChanges();
        }

        public bool EditInventory(string UserId, Inventory inventory)
        {
            if (inventory.userId == UserId)
            {
                _db.Inventories.Update(inventory);
                _db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteInventory(Guid Id, string UserId)
        {
            var inventory = _db.Inventories.Find(Id);

            if (inventory == null)
                return false;

            if (inventory.userId != UserId)
                return false;

            //Delete items using item service
            var items = from i in _db.Items where i.inventoryId.Equals(Id) select i;
            foreach(var i in items)
            {
                _db.Items.Remove(i);
            }

            _db.Inventories.Remove(inventory);
            _db.SaveChanges();
            return true;
        }

        public Inventory FindInventory(Guid id)
        {
            Inventory inventory = _db.Inventories.Find(id);
            return inventory;
        }
    }
}
