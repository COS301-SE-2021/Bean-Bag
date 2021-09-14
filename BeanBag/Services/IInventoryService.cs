using BeanBag.Models;
using System;
using System.Collections.Generic;

namespace BeanBag.Services
{
    // This class is an interface for the Inventory service.
    public interface IInventoryService
    {
        public List<Inventory> GetInventories(string id);
        public void CreateInventory(Inventory newInventory);
        public bool EditInventory(string userId, Inventory inventory);
        public bool DeleteInventory(Guid id, string userId);
        public Inventory FindInventory(Guid id);
        public string GetUserRole(string id);

    }
}
