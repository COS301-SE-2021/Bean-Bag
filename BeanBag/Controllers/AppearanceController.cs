using System.Threading.Tasks;
using BeanBag.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace BeanBag.Controllers
{
    /* This controller is used to send and retrieve data to the dashboard
       view using the tenant service functions. */
    public class AppearanceController : Controller
    {
        // Global variables needed for calling the service classes.
        private readonly TenantService tenantService;
        private readonly TenantBlobStorageService tenantBlobStorageService;

        // Constructor.
        public AppearanceController(TenantService tenantService, TenantBlobStorageService tenantBlobStorageService)
        {
            this.tenantService = tenantService;
            this.tenantBlobStorageService = tenantBlobStorageService;
        }

        // This function sends a response to the Home Index page.
        public IActionResult Index()
        {
            return View();
        }
        
        // This function sets the tenant theme by calling the SetTenantTheme service function.
        [HttpPost]
        public IActionResult ChangeThemeColour()
        {
            //Instantiated a variable that will hold the selected theme 
            string theme = Request.Form["theme"];
            
            //Pass the theme into a function that will save it into the DB
            tenantService.SetTenantTheme(User.GetObjectId(), theme);

            return RedirectToAction("Index", "Appearance");
        }
        
        // This function allows a user to set the tenant logo by calling the SetLogo service function.
        [HttpPost]
        public async Task<IActionResult> UploadLogo([FromForm(Name = "file")] IFormFile file)
        {
            var image = await tenantBlobStorageService.UploadLogoImage(file);
            tenantService.SetLogo(User.GetObjectId(),image);

            return RedirectToAction("Index", "Appearance");

        }
    }
}