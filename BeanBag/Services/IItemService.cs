using BeanBag.Models;
using System;
using System.Collections.Generic;

namespace BeanBag.Services
{
    public interface IItemService
    {
        public List<Item> GetItems(Guid InventoryId);
        public void CreateItem(Item newItem);
        public void EditItem(Item item);
        public bool DeleteItem(Guid ItemId);
        public Item FindItem(Guid ItemId);
        public Item AddQRItem(Item item);
        public Guid GetInventoryIdFromItem(Guid ItemId);
    }
}
