using System.Threading.Tasks;
using BeanBag.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace BeanBag.Controllers
{
    public class AppearanceController : Controller
    {
        private readonly TenantService _tenantService;
        private readonly TenantBlobStorageService _tenantBlobStorageService;

        public AppearanceController(TenantService tenantService, TenantBlobStorageService tenantBlobStorageService)
        {
            _tenantService = tenantService;
            _tenantBlobStorageService = tenantBlobStorageService;
        }

        // This function sends a response to the Home Index page.
        public IActionResult Index()
        {
            return View();
        }
        
        
        [HttpPost]
        public IActionResult ChangeThemeColour()
        {
            //Instantiated a variable that will hold the selected theme 
            string theme = Request.Form["theme"];
            
            //Pass the theme into a function that will save it into the DB
            _tenantService.SetTenantTheme(User.GetObjectId(), theme);

            return RedirectToAction("Index", "Appearance");
        }
        
        
        [HttpPost]
        // Allows user to upload tenant logo
        public async Task<IActionResult> UploadLogo([FromForm(Name = "file")] IFormFile file)
        {
            var image = await _tenantBlobStorageService.UploadLogoImage(file);

            _tenantService.SetLogo(User.GetObjectId(),image);

            return RedirectToAction("Index", "Appearance");

        }
    }
}