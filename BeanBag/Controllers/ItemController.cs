using Microsoft.AspNetCore.Mvc;
namespace BeanBag.Controllers
{
    public class ItemController : Controller
    {
        
       public IActionResult Index()
        {
            //using (var Context = new BeanBag.Database.BeanBagContext())
            {
              /*  var item = new BeanBag.Models.ItemModel
                {
                    itemID = "001",
                    inventoryID = "001",
                    itemName = "Chair",
                    itemType = "Furniture",
                    scanDate = DateTime.Parse(DateTime.Today.ToString())

                }; */
            }
            return View();
        }
    }
}
      