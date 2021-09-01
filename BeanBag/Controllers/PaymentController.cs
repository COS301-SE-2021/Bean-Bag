using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BeanBag.Models;
using BeanBag.Services;
using Microsoft.AspNetCore.Mvc;

namespace BeanBag.Controllers
{
    public class PaymentController : Controller
    {
          private readonly IPaymentService _payment;
          readonly string PayGateID = "10011072130"; 
          readonly string _payGateKey = "secret";

        public PaymentController(IPaymentService payment)
        {
            _payment = payment;
        }

        public async Task<IActionResult> GetRequest()
        {
            HttpClient http = new HttpClient();
            Dictionary<string, string> request = new Dictionary<string, string>();
            string paymentAmount = (50 * 100).ToString("00"); // amount int cents e.i 50 rands is 5000 cents

            request.Add("PAYGATE_ID", "10011072130");
            request.Add("REFERENCE", "pgtest_"); // Payment ref e.g ORDER NUMBER
            request.Add("AMOUNT", "5000");
            request.Add("CURRENCY", "ZAR"); // South Africa
            request.Add("RETURN_URL", "https://6cdc-102-250-1-245.ngrok.io");
            request.Add("TRANSACTION_DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            request.Add("LOCALE", "en-za");
            request.Add("COUNTRY", "ZAF");

            // get authenticated user's email
            // use a valid email, pay=gate will send a transaction confirmation to it
          
                // put your own email address for the payment confirmation (dev only)
                request.Add("EMAIL", "chrafnadax@gmail.com");
            
            request.Add("CHECKSUM", _payment.GetMd5Hash(request, _payGateKey));

            string requestString = _payment.ToUrlEncodedString(request);
            StringContent content = new StringContent(requestString, Encoding.UTF8, "application/x-www-form-urlencoded");
            HttpResponseMessage response = await http.PostAsync("https://secure.paygate.co.za/payweb3/initiate.trans", content);

            // if the request did not succeed, this line will make the program crash
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();

            Dictionary<string, string> results = _payment.ToDictionary(responseContent);

            if (results.Keys.Contains("ERROR"))
            {
                return Json(new
                {
                    success = false,
                    message = "An error occured while initiating your request"
                });
            }

            if (!_payment.VerifyMd5Hash(results, _payGateKey, results["CHECKSUM"]))
            {
                return Json(new
                {
                    success = false,
                    message = "MD5 verification failed"
                });
            }

            //NEED THE DB TO ENSURE THE TRANSACTION IS SAVED 
          //  bool IsRecorded = _payment.AddTransaction(request, results["PAY_REQUEST_ID"]);
            if (true)
            {
                return Json(new
                {
                    success = true,
                    message = "Request completed successfully",
                    results
                });
            }
          /*  return Json(new
            {
                success = false,
                message = "Failed to record a transaction"
            });*/
        }

        // This is your return url from Paygate
        // This will run when you complete payment
        [HttpPost]
        public async Task<ActionResult> CompletePayment()
        {
            string responseContent = Request.RouteValues.ToString();
            Dictionary<string, string> results = _payment.ToDictionary(responseContent);

            Transaction transaction = _payment.GetTransaction(results["PAY_REQUEST_ID"]);

            if (transaction == null)
            {
                // Unable to reconsile transaction
                return RedirectToAction("Failed");
            }

            // Reorder attributes for MD5 check
            Dictionary<string, string> validationSet = new Dictionary<string, string>();
            validationSet.Add("PAYGATE_ID", PayGateID);
            validationSet.Add("PAY_REQUEST_ID", results["PAY_REQUEST_ID"]);
            validationSet.Add("TRANSACTION_STATUS", results["TRANSACTION_STATUS"]);
            validationSet.Add("REFERENCE", transaction.REFERENCE);

            if (!_payment.VerifyMd5Hash(validationSet, _payGateKey, results["CHECKSUM"]))
            {
                // checksum error
                return RedirectToAction("Failed");
            }
            
            /** Payment Status 
             * -2 = Unable to reconsile transaction
             * -1 = Checksum Error
             * 0 = Pending
             * 1 = Approved
             * 2 = Declined
             * 3 = Cancelled
             * 4 = User Cancelled
             */
            
            int paymentStatus = int.Parse(results["TRANSACTION_STATUS"]);
            if(paymentStatus == 1)
            {
                // Yey, payment approved
                // Do something useful
            }
            // Query paygate transaction details
            // And update user transaction on your database
            await VerifyTransaction(responseContent, transaction.REFERENCE);
            return RedirectToAction("Complete", new { id = results["TRANSACTION_STATUS"] });
        }

        private async Task VerifyTransaction(string responseContent, string Referrence)
        {
            HttpClient client = new HttpClient();
            Dictionary<string, string> response = _payment.ToDictionary(responseContent);
            Dictionary<string, string> request = new Dictionary<string, string>();

            request.Add("PAYGATE_ID", PayGateID);
            request.Add("PAY_REQUEST_ID", response["PAY_REQUEST_ID"]);
            request.Add("REFERENCE", Referrence);
            request.Add("CHECKSUM", _payment.GetMd5Hash(request, _payGateKey));

            string requestString = _payment.ToUrlEncodedString(request);

            StringContent content = new StringContent(requestString, Encoding.UTF8, "application/x-www-form-urlencoded");

            // ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            HttpResponseMessage res = await client.PostAsync("https://secure.paygate.co.za/payweb3/query.trans", content);
            res.EnsureSuccessStatusCode();

            string _responseContent = await res.Content.ReadAsStringAsync();

            Dictionary<string, string> results = _payment.ToDictionary(_responseContent);
            if (!results.Keys.Contains("ERROR"))
            {
                _payment.UpdateTransaction(results, results["PAY_REQUEST_ID"]);
            }

        }

        public ViewResult Complete(int? id)
        {
            string status = "Unknown";
            switch (id.ToString())
            {
                case "-2":
                    status = "Unable to reconsile transaction";
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