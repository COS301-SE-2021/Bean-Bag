using System;
using BeanBag.Models;
using BeanBag.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace BeanBag.Controllers
{
    /* This controller is used to send and retrieve data to the account
    view using tenant service and inventory service functions. */
    public class AccountController : Controller
    {
        // Global variables needed for calling the service classes.
        private readonly TenantService tenantService;
        private readonly IInventoryService inventory;

        // Constructor.
        public AccountController(TenantService tenantService, IInventoryService inventory)
        {
            this.tenantService = tenantService;
            this.inventory = inventory;
        }
        
        // This function returns the view for the Account page.
        [AllowAnonymous]
        public IActionResult Index()
        {
            if (User.GetObjectId() == null)
            {
                throw new Exception("User ID is null");
            }
            else if (tenantService.SearchUser(User.GetObjectId()))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        
        
        // This function allows a user to create a new tenant.
        [HttpPost]
        public IActionResult CreateTenant(string tenantName)
        {
            if (tenantName == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                tenantService.CreateNewTenant(tenantName);
            }
            return RedirectToAction("Index");
        }

        /* This function allows a user to select a tenant and generates
         a new user inventory for demonstration purposes*/
        [HttpPost]
        public IActionResult SelectTenant(string tenant)
        {
            if (tenant == null)
            {
                return RedirectToAction("Index");
            }

            //Check if user is new
            var userId = User.GetObjectId();
            var currentTenantName = tenant;
            var currentTenantId = tenantService.GetTenantId(currentTenantName);

            if (tenantService.SearchUser(userId) == false)
            {
                //User is new - add user to database
                //Verify tenant
                if (tenantService.SearchTenant(currentTenantId))
                {
                    //Verified
                    tenantService.SignUserUp(userId, currentTenantId);
                }
                else
                {
                    throw new Exception("Tenant does not exist");
                }
            }

            //New inventory created for first time user.
            Inventory newInventory = new Inventory()
            {
                name = "My First Inventory",
                userId = userId,
                description = "My first ever inventory to add new items to",
                createdDate = DateTime.Now
            };
            inventory.CreateInventory(newInventory);

            return RedirectToAction("Index", "Home");
        }

    }
}