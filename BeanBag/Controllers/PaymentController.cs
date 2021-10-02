using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BeanBag.Models;
using BeanBag.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using X.PagedList;

namespace BeanBag.Controllers
{
    // This controller sends and receives data to the paygate api and
    // calls functions in the payment service classes
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly ITenantService _tenantService;
        
        // the company will have their own details , this is for test purposes.
        readonly string PayGateID = "10011072130"; 
        readonly string _payGateKey = "secret";

        // Constructor.
        public PaymentController(IPaymentService payment, ITenantService tenant)
        {
            _paymentService = payment;
            _tenantService = tenant;
        }
        
        // This function is the get request for the payment gateway and will accept the payment amount.
        public async Task<IActionResult> GetRequest(string email, string amount)
        {
            Console.WriteLine("Checking the user id getReq: " + User.GetObjectId());


            HttpClient http = new HttpClient();
            string reference = Guid.NewGuid().ToString();
            Dictionary<string, string> request = new()
            {
                {"PAYGATE_ID", PayGateID},
                {"REFERENCE", reference},
                {"AMOUNT", amount},
                {"CURRENCY", "ZAR"},
                // Return url to original payment page -- run in ngrok
                // ngrok http -host-header="localhost:44352"
                {"RETURN_URL",
                    "https://beanbagpolaris.azurewebsites.net/Payment/CompletePayment?amounts=" +
                               ""+amount+"&references="+reference},
                {"TRANSACTION_DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},
                {"LOCALE", "en-za"},
                {"COUNTRY", "ZAF"},
                {"EMAIL", email}
            };
            
            request.Add("CHECKSUM", _paymentService.GetMd5Hash(request, _payGateKey));

            string requestString = _paymentService.ToUrlEncodedString(request);
            StringContent content = new StringContent(requestString,
                Encoding.UTF8, "application/x-www-form-urlencoded");
            HttpResponseMessage response
                = await http.PostAsync("https://secure.paygate.co.za/payweb3/initiate.trans", content);

            // if the request did not succeed, this line will make the program crash
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();

            Dictionary<string, string> results = _paymentService.ToDictionary(responseContent);

            if (results.Keys.Contains("ERROR"))
            {
                return Json(new
                {
                    success = false,
                    message = "An error occured while initiating your request"
                });
            }

            if (!_paymentService.VerifyMd5Hash(results, _payGateKey, results["CHECKSUM"]))
            {
                return Json(new
                {
                    success = false,
                    message = "MD5 verification failed"
                });
            }

            return Json(new
                {
                    success = true,
                    message = "Request completed successfully",
                    results
                });

        }

        // This function is used to complete the payment and return to the website.
        [HttpPost]
        public ActionResult CompletePayment(string amounts, string references)
        {
            Console.WriteLine("Checking the user id complete pay: " + User.GetObjectId());


            var responseContent = Request.Query.Concat(Request.Form);
            
            var results = responseContent.ToDictionary(x 
                => x.Key, x => x.Value);
    
            return  RedirectToAction("Complete", new { id = results["TRANSACTION_STATUS"] ,
                amount=amounts, payReqId=results["PAY_REQUEST_ID"] , reference=references});
        }

        //This function occurs after completing the payment, and presents a popup of the transaction status.
        public ActionResult Complete(int? id, string amount, string payReqId , string reference)
        {
            string status;
            switch (id.ToString())
            {
                case "-2":
                    status = "Unable to reconcile transaction";
                    break;
                case "-1":
                    status = "Checksum Error. The values have been altered";
                    break;
                case "0":
                    status = "Not Done";
                    break;
                case "1":
                    ViewBag.payReqId = payReqId;
                    ViewBag.reference = reference;

                    //Transaction Approved 
                    //Console.WriteLine("Checking the user id complete: " + User.GetObjectId());

                    //Determine the type of subscription
                    ViewBag.Subscription = amount.Equals("50000") ? "Standard" : "Premium";
                    ViewBag.UpdatedSubscription = false;

                    try
                    {
                        if(_tenantService.GetCurrentTenant(User.GetObjectId()).TenantId != null)
                        {
                            @ViewBag.UpdatedSubscription = true;
                        }
                    }
                    catch(Exception e)
                    {
                        if (e.ToString().Equals("User does not exist in the database.")) 
                        {
                            return View();
                        }
                    }

                    return View();

                case "2":
                    status = "Declined";
                    break;
                case "3":
                    status = "Cancelled";
                    break;
                case "4":
                    status = "User Cancelled";
                    break;
                default:
                    status = $"Unknown Status({ id })";
                    break;
            }

            TempData["Status"] = status;

            try 
            {
                if (_tenantService.GetCurrentTenant(User.GetObjectId()).TenantId != null)
                {
                    //Error popup
                    return RedirectToAction("Index", "Home");
                }
            }
            catch(Exception e)
            {
                if (e.ToString().Equals("User does not exist in the database."))
                {
                    //error popup
                    return RedirectToAction("Index", "Tenant");
                }
            }

            //error popup
            return RedirectToAction("Index", "Tenant");  
        }
        
        // This function returns the billing page where the tenant can view their transactions.
        public IActionResult Billing(string sortOrder, string currentFilter, string searchString,
            int? page,DateTime from, DateTime to, string subscription)
        {
            if(User.Identity is {IsAuthenticated: true})
            {
                
             //A ViewBag property provides the view with the current sort order, because this must be included in 
             //the paging links in order to keep the sort order the same while paging
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            List<Transactions> modelList;

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

            
            var currentTenantId = _tenantService.GetCurrentTenant(User.GetObjectId()).TenantId;
           
            //transaction
            var model = from s in _paymentService.GetTransactions(currentTenantId)
                select s;
                //Search and match data, if search string is not null or empty
                if (!String.IsNullOrEmpty(searchString))
                {
                    model = model.Where(s => s.Reference.ToString().Contains(searchString));
                }

                var inventories = model.ToList();
                switch (sortOrder)
                {
                    case "name_desc":
                        modelList = inventories.OrderByDescending(s => s.StartDate).ToList();
                        break;
                 
                    default:
                        modelList = inventories.OrderBy(s => s.StartDate).ToList();
                        break;
                }

                //Date sorting
                if (sortOrder == "date")
                {
                    modelList =( inventories.Where(t => t.StartDate > from && t.StartDate < to)).ToList();
                }
                
            //indicates the size of list
            int pageSize = 5;
            //set page to one is there is no value, ??  is called the null-coalescing operator.
            int pageNumber = (page ?? 1);
            //return the Model data with paged

          
            Pagination viewModel = new Pagination();
            IPagedList<Transactions> pagedList = modelList.ToPagedList(pageNumber, pageSize);
            viewModel.PagedListTenantTransactions = pagedList;

            
            
            // Get the total transactions
            @ViewBag.totalTransactions = _paymentService.GetTransactions(currentTenantId).Count();
            
            // Get tenant details
            @ViewBag.tenant = _tenantService.GetCurrentTenant(User.GetObjectId());

            //Update subscription
            @ViewBag.Sub = "P";
           //current subscription
           if (@ViewBag.tenant.TenantSubscription == "Free")
           {
               @ViewBag.subscription = "Free";
           }
           else
           {
                    ViewBag.transaction = _paymentService.GetPaidSubscription(currentTenantId);

               @ViewBag.subscription = _tenantService.GetCurrentTenant(User.GetObjectId()).TenantSubscription;
           }
           
            return View(viewModel);
            }
            else
            {
                return LocalRedirect("/");
            }
        }

        // This function allows the tenant Admin to update the tenants subscription plan.

        public ViewResult UpdateSubscription(string subscription, string tenantId)
        {  //Free - Automatic update
            if (subscription == "Free")
            {
                _tenantService.UpdateSubscription(subscription, tenantId);
                return View("_UpdateFreeSubscription");
            }
            else if(subscription == "Standard")
            {
                Console.WriteLine(subscription);
                return View("_UpdateStandardSubscription");
            }
            else if (subscription == "Premium")
            {
                //Premium
                Console.WriteLine(subscription);
                return View("_UpdatePremiumSubscription");
            }

            return (ViewResult) Billing("","","",1,DateTime.Now, DateTime.Now, "");
        }

    }
    
}