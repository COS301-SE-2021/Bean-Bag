using System;
using System.Collections.Generic;
using System.Linq;
using BeanBag.Database;
using BeanBag.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BeanBag.Controllers
{
    
    public class ItemController : Controller
    {
        
     // Mocked code for the item details on demo 1, to be fully implemented.    
     /*  public IActionResult Index()
        {
            using (var Context = new BeanBag.Database.BeanBagContext())
            {
               var item = new BeanBag.Models.ItemModel
                {
                    InventoryId = "001",
                    ItemName = "Chair",
                    ItemType = "Furniture",
                    ScanDate = DateTime.Parse(DateTime.Today.ToString())
                }; 
            }
            return View();
        }*/
     
     // Nada: create item and add. (using for unit testing , mocking DB)
     // Unit testing specifies mock data, intergration testing uses the database
     private readonly BeanBagContext _beanBagContext;
     public ItemController(BeanBagContext beanBagContext)
     {
         _beanBagContext = beanBagContext;
     }

     public EntityEntry<ItemModel> AddItem(string inventoryId, string itemName, string itemType, DateTime scanDate)
     {
         var newItem = _beanBagContext.Items.Add(
             new ItemModel
             {
                 InventoryId = inventoryId,
                 ItemName = itemName,
                 ItemType = itemType,
                 ScanDate = scanDate
             });

         _beanBagContext.SaveChanges();
         return newItem;
     }

     public List<ItemModel> GetItems()
     {
         var query = from b in _beanBagContext.Items
             orderby b.ItemName
             select b;

         return query.ToList();
     }
    }
}
      