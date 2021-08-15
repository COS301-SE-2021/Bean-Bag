using BeanBag.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeanBag.Services
{
    public interface IInventoryService
    {
        public List<Inventory> GetInventories(string id);
        public void CreateInventory(Inventory newInventory);
        public bool EditInventory(string UserId, Inventory inventory);
        public bool DeleteInventory(Guid Id, string UserId);
        public Inventory FindInventory(Guid Id);
    }
}
