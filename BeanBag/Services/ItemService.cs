using BeanBag.Database;
using BeanBag.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BeanBag.Services
{
    // This service class is used to handle any data that is related to an inventory's items.
    // This service class main focus is to bridge the the item controller to the DB
    public class ItemService : IItemService
    {
        // Variables
        private readonly DBContext _db;

        // Constructor
        public ItemService(DBContext db)
        {
            _db = db;
        }

        // This method is used to add the QR code link to the to the item variable QRContents
        public Item AddQRItem(Item item)
        {
            if (item != null)
            {
                item.QRCodeLink = "https://bean-bag-function.azurewebsites.net/api/ItemQRCode?itemID=" + item.Id.ToString();
            }
            return item;
        }

        // This method is used to store a newly created item into the DB
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

        // This method is used to delete a specfied item from the DB using the primary key itemId
        public bool DeleteItem(Guid ItemId)
        {
            var item = FindItem(ItemId);

            if (item == null)
                return false;
            _db.Items.Remove(item);
            _db.SaveChanges();

            return true;
        }

        // This method is used to update an item's information in the DB
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

        // This method is used to find a specified item using the primary key itemId
        public Item FindItem(Guid ItemId)
        {
            Item item = _db.Items.Find(ItemId);
            return item;
        }

        // This method is used to retrieve all items belonging to an inventory using the inventoryId variable
        public List<Item> GetItems(Guid InventoryId)
        {
            var items = (from i in _db.Items where i.inventoryId.Equals(InventoryId) select i).ToList();
            return items;
        }

        // This method is used to return an inventoryId beloning to a specified item using the itemId
        public Guid GetInventoryIdFromItem(Guid ItemId)
        {
            var item = FindItem(ItemId);
            if (item == null)
                return Guid.Empty;

            return item.inventoryId;

        }
    }
}
