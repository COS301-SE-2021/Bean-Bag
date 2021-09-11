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
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace BeanBag.Controllers
{
    public class TenantSettingsController : Controller
    {
        
        // Global variables needed for calling the service classes.
        private readonly ITenantService _tenantService;
        private readonly TenantDbContext _tenantDbContext;
        private readonly string _from = "";
        private readonly string _pswd = "";

        // Constructor.
        public TenantSettingsController(ITenantService tenantService, TenantDbContext tenantDbContext)
        {
            _tenantService = tenantService;
            _tenantDbContext = tenantDbContext;
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
             @ViewBag.tenant =  _tenantService.GetCurrentTenant(User.GetObjectId());
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
        
        /* Delete Get method. Passes user to be deleted to the Post method. */
        public IActionResult Delete(string userObjectId)
        {
            if (User.Identity is {IsAuthenticated: true})
            {

                if (userObjectId == null)
                {
                    return BadRequest();
                }
                
                var user = _tenantDbContext.TenantUser.Find(userObjectId);
                return View(user);
            }

            return LocalRedirect("/");
        }
        
        
        /* Edit Get method. Passes user role and user id to Post method. */
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
        
        /* Post method for deleting a user. Receives user object id and deletes user from database. */
        [HttpPost]
        public IActionResult DeletePost(string userObjectId)
        {
            if (userObjectId == null)
            {
                return BadRequest();
            }
            
            //User cannot delete themselves
            if (userObjectId.Equals(User.GetObjectId()))
            {
                return BadRequest();
            }

            //Delete the user from database
            if (_tenantService.DeleteUser(userObjectId))
            {
                return RedirectToAction("Index");
            }

            return LocalRedirect("/");
        }
        
        /* Post method to edit the user role. This method receives the user id and the updated role from Edit. */
        [HttpPost]
        public IActionResult EditPost(string UserObjectId, string UserRole)
        {
            if (UserRole == null)
            {
                return BadRequest();
            }

            if (_tenantService.EditUserRole(UserObjectId,UserRole))
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }


        public IActionResult DeleteTenant()
        {
            throw new NotImplementedException();
        }

        /* This function allows an admin user to invite a new user to join the tenant.
         The user email will be entered - user receives a random code and link to complete sign up process. */
        [HttpPost]
        public IActionResult InviteNewUser(string userEmail)
        {
            if (userEmail == null)
            {
                return BadRequest();
            }
            
            //Get current tenant
            var tenant = _tenantService.GetTenantName(_tenantService.GetUserTenantId(User.GetObjectId()));

            MimeMessage mimeMessage = new MimeMessage();
            
            //add sender
            mimeMessage.From.Add(new MailboxAddress("Bean Bag", _from));

            //add receiver
            mimeMessage.To.Add(MailboxAddress.Parse(userEmail));
            
            //add message subject
            mimeMessage.Subject = "Invitation";
            
            //message
            mimeMessage.Body = new TextPart("plain")
            {
                Text = @"You have been invited to join " + tenant + ". Invitation code to join:" +
                       "Click on the link below to proceed to the sign up." 
                       
            };
            
            //create new SMTP client
            var client = new SmtpClient();
            
            try
            {
                //connect to gmail SMTP server (port: 465, ssl:true)
                client.Connect("smtp.gmail.com", 465, true);

                //authenticate
                client.Authenticate(_from,_pswd );
                
                //send message
                client.Send(mimeMessage);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception("email not sent");
            }
            finally
            {
                //disconnect from the smtp server
                client.Disconnect(true);
                
                //dispose client object
                client.Dispose();
                
            }

            return RedirectToAction("Index");
        }
    }
}