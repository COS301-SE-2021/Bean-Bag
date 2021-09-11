using BeanBag.Database;
using BeanBag.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BeanBag.Services
{
    // This service class is used to handle any data that is related to a user's inventory.
    // This service class main focus is to bridge the the inventory controller to the DB
    public class InventoryService : IInventoryService
    {
        // Variable
        private readonly DBContext _db;

        // Constructor
        public InventoryService(DBContext db)
        {
            _db = db;
        }

        // This method is used to retrieve inventories that belong to a user using their userid
        public List<Inventory> GetInventories(string id)
        {
            var inventories = (from i in _db.Inventories where i.userId.Equals(id) select i).ToList();
            return inventories;
        }

        // This method is used to create and store an newly created inventory into the DB
        public void CreateInventory(Inventory newInventory)
        {
            newInventory.createdDate = DateTime.Now;
            _db.Inventories.Add(newInventory);
            _db.SaveChanges();
        }

        // This method is used to update an inventory information into the DB
        public bool EditInventory(string userId, Inventory inventory)
        {
            if (inventory.userId == userId)
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

        // This method is used to delete a specified inventory from the DB
        public bool DeleteInventory(Guid id, string userId)
        {
            var inventory = _db.Inventories.Find(id);

            if (inventory == null)
                return false;

            if (inventory.userId != userId)
                return false;

            //Delete items using item service
            var items = from i in _db.Items where i.inventoryId.Equals(id) select i;
            foreach(var i in items)
            {
                _db.Items.Remove(i);
            }

            _db.Inventories.Remove(inventory);
            _db.SaveChanges();
            return true;
        }

        // This method is used to retrieve a single inventory using the primary key inventoryId
        public Inventory FindInventory(Guid id)
        {
            Inventory inventory = _db.Inventories.Find(id);
            return inventory;
        }
    }
}
