using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace BeanBag.Controllers
{
    /*
     * This class is responsible for the data returned to the Item page
     */
    public class ItemController : Controller
    {
        /*
         * This function returns the page structure for the items page 
         */
        public IActionResult Index()
        {
            using (var Context = new BeanBag.Database.BeanBagContext())
            {
                var item = new BeanBag.Models.ItemModel
                {
                    itemID = "001",
                    inventoryID = "001",
                    itemName = "Chair",
                    itemType = "Furniture",
                    scanDate = DateTime.Parse(DateTime.Today.ToString())

                }; 
            }
            return View();
        }
    }
}
