using BeanBag.Models;
using System;
using System.Collections.Generic;

namespace BeanBag.Services
{
    // This class is an interface for the Item service.
    public interface IItemService
    {
        public List<Item> GetItems(Guid inventoryId);
        public void CreateItem(Item newItem);
        public void EditItem(Item item);
        public bool DeleteItem(Guid itemId);
        public Item FindItem(Guid itemId);
        public Item AddQrItem(Item item);
        public Guid GetInventoryIdFromItem(Guid itemId);
    }
}
