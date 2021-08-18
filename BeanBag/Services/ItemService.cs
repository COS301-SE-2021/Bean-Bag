using BeanBag.Database;
using BeanBag.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BeanBag.Services
{
    public class ItemService : IItemService
    {
        private readonly DBContext _db;

        public ItemService(DBContext db)
        {
            _db = db;
        }

        public Item AddQRItem(Item item)
        {
            if (item != null)
            {
                item.QRContents = "https://bean-bag.azurewebsites.net/api/QRCodeScan?itemID=" + item.Id.ToString();
            }
            return item;
        }

        public void CreateItem(Item newItem)
        {
            if (newItem != null)
            {
                newItem.entryDate = DateTime.Now;
                _db.Items.Add(newItem);
                _db.SaveChanges();
                newItem = AddQRItem(newItem);
                _db.Items.Update(newItem);
                _db.SaveChanges(); 
            }
            
        }

        public bool DeleteItem(Guid ItemId)
        {
            var item = FindItem(ItemId);

            if (item == null)
                return false;
            _db.Items.Remove(item);
            _db.SaveChanges();

            return true;
        }

        public void EditItem(Item item)
        {
            if (item != null)
            {
                if (!item.isSold)
                    item.soldDate = DateTime.MinValue;
                _db.Items.Update(item);
                _db.SaveChanges(); 
            }
            
        }

        public Item FindItem(Guid ItemId)
        {
            Item item = _db.Items.Find(ItemId);
            return item;
        }

        public List<Item> GetItems(Guid InventoryId)
        {
            var items = (from i in _db.Items where i.inventoryId.Equals(InventoryId) select i).ToList();
            return items;
        }

        public Guid GetInventoryIdFromItem(Guid ItemId)
        {
            var item = FindItem(ItemId);
            if (item == null)
                return Guid.Empty;

            return item.inventoryId;

        }
    }
}
