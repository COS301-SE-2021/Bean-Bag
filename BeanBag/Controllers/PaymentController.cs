#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BeanBag.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace BeanBag.Controllers
{
    // This controller sends and receives data to the paygate api and
    // calls functions in the payment service classes
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
          
        // the company will have their own details , this is or test purposes.
        readonly string PayGateID = "10011072130"; 
        readonly string _payGateKey = "secret";

        // Constructor.
        public PaymentController(IPaymentService payment)
        {
            _paymentService = payment;
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
                // ngrok http https://localhost:44352 -host-header="localhost:44352"
                {"RETURN_URL", "https://b614-102-250-3-252.ngrok.io/Payment/CompletePayment?amount=" +
                               ""+amount+"&reference="+reference},
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
        public ActionResult CompletePayment(string a, string r)
        {
            Console.WriteLine("Checking the user id complete pay: " + User.GetObjectId());


            var responseContent = Request.Query.Concat(Request.Form);
            
            var results = responseContent.ToDictionary(x 
                => x.Key, x => x.Value);
    
            return  RedirectToAction("Complete", new { id = results["TRANSACTION_STATUS"] ,
                amount=a, payReqId=results["PAY_REQUEST_ID"] , reference=r});
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
                    @ViewBag.payReqId = payReqId;
                    @ViewBag.reference = reference;
                    
                    //Transaction Approved 
                    Console.WriteLine("Checking the user id complete: " + User.GetObjectId());
                    
                    //Determine the type of subscription
                    @ViewBag.Subscription = amount.Equals("50000") ? "Standard" : "Premium";
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

           return RedirectToAction("Index", "Tenant");
        }
        
        // This function returns the billing page where the tenant can view their transactions.
        public IActionResult Billing()
        {
            return View();
        }
    }
    
}