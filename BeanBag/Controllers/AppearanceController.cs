using System;
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
        private readonly ITenantService _tenantService;
        private readonly ITenantBlobStorageService _tenantBlobStorageService;
        private readonly IPaymentService _paymentService;

        // Constructor.
        public AppearanceController(ITenantService tenantService,IPaymentService paymentService,
            ITenantBlobStorageService tenantBlobStorageService)
        {
            _tenantService = tenantService;
            _tenantBlobStorageService = tenantBlobStorageService;
            _paymentService = paymentService;
        }

        // This function sends a response to the Home Index page.
        public IActionResult Index()
        {
            
            //Subscription Expired 
            @ViewBag.SubscriptionExpired = false;
            if (_tenantService.GetCurrentTenant(User.GetObjectId()).TenantSubscription != "Free")
            {
                var transaction =
                    _paymentService.GetPaidSubscription(_tenantService.GetCurrentTenant(User.GetObjectId()).TenantId);
                if (transaction.EndDate >= DateTime.Now)
                {
                    @ViewBag.SubscriptionExpired = true;
                }
            }
            return View();
        }
        
        // This function sets the tenant theme by calling the SetTenantTheme service function.
        [HttpPost]
        public IActionResult ChangeThemeColour()
        {
            //Instantiated a variable that will hold the selected theme 
            string theme = Request.Form["theme"];
            
            //Pass the theme into a function that will save it into the DB
            _tenantService.SetTenantTheme(User.GetObjectId(), theme);

            return RedirectToAction("Index", "Appearance");
        }
        
        // This function allows a user to set the tenant logo by calling the SetLogo service function.
        [HttpPost]
        public async Task<IActionResult> UploadLogo([FromForm(Name = "file")] IFormFile file)
        {
            var image = await _tenantBlobStorageService.UploadLogoImage(file);
            _tenantService.SetLogo(User.GetObjectId(),image);

            return RedirectToAction("Index", "Appearance");

        }
    }
}