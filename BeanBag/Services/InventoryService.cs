using BeanBag.Database;
using BeanBag.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BeanBag.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly DBContext _db;

        public InventoryService(DBContext db)
        {
            _db = db;
        }

   
        public List<Inventory> GetInventories(string id)
        {
            var inventories = (from i in _db.Inventories where i.userId.Equals(id) select i).ToList();
            return inventories;
        }

        public void CreateInventory(Inventory newInventory)
        {
            newInventory.createdDate = DateTime.Now;
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
