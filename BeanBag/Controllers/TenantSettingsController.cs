using System;
using System.Collections.Generic;
using System.Linq;
using BeanBag.Database;
using BeanBag.Models;
using BeanBag.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using X.PagedList;

namespace BeanBag.Controllers
{
    public class TenantSettingsController : Controller
    {
        
        // Global variables needed for calling the service classes.
        private readonly TenantService _tenantService;
        private readonly IInventoryService _inventoryService;
        private readonly TenantDbContext _tenantDbContext;

        // Constructor.
        public TenantSettingsController(TenantService tenantService, IInventoryService inventoryService, TenantDbContext dbContext)
        {
            _tenantService = tenantService;
            _inventoryService = inventoryService;
            _tenantDbContext = dbContext;
        }

        /* Provides a list of users under the currently signed in tenant */
        [AllowAnonymous]
        public IActionResult Index(string sortOrder, string currentFilter, string searchString,
            int? page)
        {
            if(User.Identity is {IsAuthenticated: true})
            {
                
             //A ViewBag property provides the view with the current sort order, because this must be included in 
             //the paging links in order to keep the sort order the same while paging
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            IEnumerable<TenantUser> modelList;

            //ViewBag.CurrentFilter, provides the view with the current filter string.
            //he search string is changed when a value is entered in the text box and the submit button is pressed. In that case, the searchString parameter is not null.
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;


            var model = from s in _tenantService.GetUserList(User.GetObjectId())
                select s;
            
                //Search and match data, if search string is not null or empty
                if (!String.IsNullOrEmpty(searchString))
                {
                    model = model.Where(s => s.UserName.Contains(searchString));
                }

                var users = model.ToList();
                switch (sortOrder)
                {
                    case "name_desc":
                        modelList = users.OrderByDescending(s => s.UserName).ToList();
                        break;
                 
                    default:
                        modelList = users.OrderBy(s => s.UserName).ToList();
                        break;
                }
                
            //indicates the size of list
            int pageSize = 5;
            //set page to one is there is no value, ??  is called the null-coalescing operator.
            int pageNumber = (page ?? 1);
            //return the Model data with paged

            TenantUser tenantUser = new TenantUser();
            Pagination viewModel = new Pagination();
            IPagedList<TenantUser> pagedList = modelList.ToPagedList(pageNumber, pageSize);

            viewModel.TenantUser = tenantUser;
            viewModel.PagedListTenantUsers = pagedList;
           // @ViewBag.totalInventories = _inventoryService.GetInventories(User.GetObjectId()).Count;
            
            //Checking user role is in DB
            //CheckUserRole();
            return View(viewModel);
            }
            else
            {
                return LocalRedirect("/");
            }
        }
        
        
        /* Allows admin user to edit the tenant details through pop-up form. Changes get saved to database. */
        [HttpPost]
        public IActionResult EditDetails(string tenantName, string tenantAddress, string tenantEmail, string tenantNumber)
        {
            if(User.Identity is {IsAuthenticated: true})
            {

                var tenant = _tenantService.GetCurrentTenant(User.GetObjectId());
                
                if(tenant!=null)
                {
                    _tenantService.EditTenantDetails(tenant.TenantId,tenantName,tenantAddress,tenantEmail,tenantNumber);
                    return RedirectToAction("Index");
                }
                else 
                {
                    return BadRequest();
                }
            }
            else
            {
                return LocalRedirect("/");
            }
        }
        
        /* Allows admin user to delete a user from the database */
        public IActionResult Delete(string userId)
        {
            if (User.Identity is {IsAuthenticated: true})
            {

                if (userId == null)
                {
                    return BadRequest();
                }

                if (User.GetObjectId()!=null && User.GetObjectId().Equals(userId))
                {
                    return BadRequest();
                }

                var user = _tenantDbContext.TenantUser.Find(userId);

                return View(user);
            }

            return LocalRedirect("/");
        }
        
        /* Allows admin user to change the role of the user */
        public IActionResult Edit(string userId)
        {
            if (userId == null)
            {
                return BadRequest();
            }
            
            var user = _tenantDbContext.TenantUser.Find(userId);
            var role = _tenantService.GetUserRole(userId);
                
            return View(role,userId);
                
        }
        
        [HttpPost]
        public IActionResult EditPost(string userId, UserRoles userRole)
        {
            if (userRole == null)
            {
                return BadRequest();
            }

            if (_tenantService.EditUserRole(userId,userRole.role))
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
        
        
        
        [HttpPost]
        public IActionResult DeletePost(string userId)
        {
            if (userId == null)
            {
                return BadRequest();
            }

            if (_tenantService.DeleteUser(userId))
            {
                return RedirectToAction("Index");
            }

            return LocalRedirect("/");
        }
        
        
    }
}