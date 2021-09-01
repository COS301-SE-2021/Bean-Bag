using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BeanBag.Services;
using Microsoft.AspNetCore.Mvc;

namespace BeanBag.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        
        //For testing purposes, an actual merchant would require the actual Pay-gate key and ID 
        private const string PayGateId = "10011072130";
        private const string PayGateKey = "secret";

        //Constructor
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<JsonResult> GetRequest()
        {
            HttpClient http = new HttpClient();
            Dictionary<string, string> request = new Dictionary<string, string>();
            
            // Add if statement here and choose the payment type based on what the type of subscription the
            // user chose.
            
            string paymentAmount = (50 * 100).ToString("00"); // amount int cents e.i 50 rands is 5000 cents

            request.Add("PAYGATE_ID", PayGateId);
            request.Add("REFERENCE", "Beanbag Standard Order"); // Payment ref e.g ORDER NUMBER
            request.Add("AMOUNT", paymentAmount);
            request.Add("CURRENCY", "ZAR"); // South Africa
            //return URL needs to be secure, determine this url once the 
            request.Add("RETURN_URL", "https://6cdc-102-250-1-245.ngrok.io");
            request.Add("TRANSACTION_DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            request.Add("LOCALE", "en-za");
            request.Add("COUNTRY", "ZAF");

            // get authenticated user's email
            // use a valid email, pay-gate will send a transaction confirmation to it
            if(User.Identity is {IsAuthenticated: true})
            {
                //how to get the users email?
               // request.Add("EMAIL", User.Identity.Name);
            } else
            {
                // put your own email address for the payment confirmation (dev only)
              //  request.Add("EMAIL", "chrafnadax@gmail.com");
            }
            
            //Using my email for testing purposes
            request.Add("EMAIL", "chrafnadax@gmail.com");
            request.Add("CHECKSUM", _paymentService.GetMd5Hash(request, PayGateKey));

            string requestString = _paymentService.ToUrlEncodedString(request);
            StringContent content = new StringContent(requestString, Encoding.UTF8, "application/x-www-form-urlencoded");
            HttpResponseMessage response = await http.PostAsync("https://secure.paygate.co.za/payweb3/initiate.trans", content);

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
                },  new Newtonsoft.Json.JsonSerializerSettings());
            }

            if (!_paymentService.VerifyMd5Hash(results, PayGateKey, results["CHECKSUM"]))
            {
                return Json(new
                {
                    success = false,
                    message = "MD5 verification failed"
                },  new Newtonsoft.Json.JsonSerializerSettings());
            }

        /*    bool isRecorded = _paymentService.AddTransaction(request, results["PAY_REQUEST_ID"]);
            if (isRecorded)
            {
                return Json(new
                {
                    success = true,
                    message = "Request completed successfully",
                    results
                },  new Newtonsoft.Json.JsonSerializerSettings());
            }*/
            
            
            return Json(new
            {
                success = false,
                message = "Failed to record a transaction"
            },  new Newtonsoft.Json.JsonSerializerSettings());
        }

        // This is your return url from Paygate
        // This will run when you complete payment
        [HttpPost]
        public async Task<ActionResult> CompletePayment()
        {
            string responseContent = Request.ToString();
            Dictionary<string, string> results = _paymentService.ToDictionary(responseContent);

           /* Transaction transaction = _paymentService.GetTransaction(results["PAY_REQUEST_ID"]);

            if (transaction == null)
            {
                // Unable to reconcile transaction
                return  RedirectToAction("Failed");
            }*/

            // Reorder attributes for MD5 check
            Dictionary<string, string> validationSet = new Dictionary<string, string>
            {
                {"PAYGATE_ID", PayGateId},
                {"PAY_REQUEST_ID", results["PAY_REQUEST_ID"]},
                {"TRANSACTION_STATUS", results["TRANSACTION_STATUS"]}
            };
            // validationSet.Add("REFERENCE", transaction.REFERENCE);

            if (!_paymentService.VerifyMd5Hash(validationSet, PayGateKey, results["CHECKSUM"]))
            {
                // checksum error
                return RedirectToAction("Failed");
            }
            
            /* Payment Status 
              -2 = Unable to reconcile transaction
              -1 = Checksum Error
              0 = Pending
              1 = Approved
              2 = Declined
              3 = Cancelled
              4 = User Cancelled
             */
            
            int paymentStatus = int.Parse(results["TRANSACTION_STATUS"]);
            if(paymentStatus == 1)
            {
                // Yey, payment approved
            }
            // Query paygate transaction details
            // And update user transaction on your database
          //  await VerifyTransaction(responseContent, transaction.REFERENCE);
            return RedirectToAction("Complete", new { id = results["TRANSACTION_STATUS"] });
        }

        private async Task VerifyTransaction(string responseContents, string reference)
        {
            HttpClient client = new HttpClient();
            Dictionary<string, string> response = _paymentService.ToDictionary(responseContents);
            Dictionary<string, string> request = new Dictionary<string, string>
            {
                {"PAYGATE_ID", PayGateId}, {"PAY_REQUEST_ID", response["PAY_REQUEST_ID"]}, {"REFERENCE", reference}
            };

            request.Add("CHECKSUM", _paymentService.GetMd5Hash(request, PayGateKey));

            string requestString = _paymentService.ToUrlEncodedString(request);

            StringContent content = new StringContent(requestString, Encoding.UTF8, "application/x-www-form-urlencoded");

            // ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            HttpResponseMessage res = await client.PostAsync("https://secure.paygate.co.za/payweb3/query.trans", content);
            res.EnsureSuccessStatusCode();

            string responseContent = await res.Content.ReadAsStringAsync();

            Dictionary<string, string> results = _paymentService.ToDictionary(responseContent);
            if (!results.Keys.Contains("ERROR"))
            {
             //   _paymentService.UpdateTransaction(results, results["PAY_REQUEST_ID"]);
            }

        }
        public ViewResult Complete(int? id)
        {
            string status = "Unknown";
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
                    status = "Approved";
                    break;
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

            return View();
        }

        public ActionResult Failed()
        {
            throw new NotImplementedException();
        }
        
        public IActionResult Billing()
        {
            return View();
        }
    }
    
}