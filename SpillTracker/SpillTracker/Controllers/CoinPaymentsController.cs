using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using SpillTracker.Models;
using SpillTracker.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpillTracker.Controllers
{   
    public class CoinPaymentsController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;
        private readonly ISpillTrackerUserRepository _userRepo;
        private readonly IHttpContextAccessor _HttpContextAccessor;
        public CoinPaymentsController(IConfiguration config, UserManager<IdentityUser> userManager, ISpillTrackerUserRepository userRepo, IHttpContextAccessor httpContextAccessor)
        {
            _config = config;
            _userManager = userManager;
            _userRepo = userRepo;
            _HttpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ProcessCheckout()
        {
            string userId = _userManager.GetUserId(User);
            IdentityUser identityUser = await _userManager.GetUserAsync(User);
            Stuser theBuyer = null;
            if (userId != null)
            {
                theBuyer = _userRepo.GetStuserByIdentityId(userId);
            }

            var model = new OrderModel
            {
                OrderId = Guid.NewGuid().ToString(),
                OrderTotal = (decimal)0.12,
                ProductName = "Generate PDF Report",
                FirstName = theBuyer.FirstName,
                LastName = theBuyer.LastName,
                Email = identityUser.Email
            };

            return View(model);
        }

        [HttpPost]
        public void ProcessCheckout(OrderModel orderModel)
        {
            var queryParameters = CreateQueryParameters(orderModel);

            var baseUrl = _config.GetSection("CoinPayments")["BaseUrl"];
            var redirectUrl = QueryHelpers.AddQueryString(baseUrl, queryParameters);

            _HttpContextAccessor.HttpContext.Response.Redirect(redirectUrl);
        }

        public IActionResult SuccessHandler(string orderNumber)
        {
            ViewBag.OrderNumber = orderNumber;

            return View("PaymentResponse");
        }

        public IActionResult CancelOrder()
        {
            return View("PaymentResponse");
        }

        private IDictionary<string, string> CreateQueryParameters(OrderModel orderModel)
        {
            //get store location  
            var storeLocation = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            var queryParameters = new Dictionary<string, string>()
            {
                //IPN settings  
                ["ipn_version"] = "1.0",
                ["cmd"] = "_pay_auto",
                ["ipn_type"] = "simple",
                ["ipn_mode"] = "hmac",
                ["merchant"] = _config["MerchantId"],
                ["allow_extra"] = "0",
                ["currency"] = "USD",
                ["amountf"] = orderModel.OrderTotal.ToString("N2"),
                ["item_name"] = orderModel.ProductName,

                //IPN, success and cancel URL  
                ["success_url"] = $"{storeLocation}/CoinPayments/SuccessHandler?orderNumber={orderModel.OrderId}",
                ["ipn_url"] = $"{storeLocation}/CoinPayments/IPNHandler",
                ["cancel_url"] = $"{storeLocation}/CoinPayments/CancelOrder",

                //order identifier                  
                ["custom"] = orderModel.OrderId,

                //billing info  
                ["first_name"] = orderModel.FirstName,
                ["last_name"] = orderModel.LastName,
                ["email"] = orderModel.Email,

            };
            return queryParameters;
        }

        [HttpPost]
        public IActionResult IPNHandler()
        {
            byte[] parameters;
            using (var stream = new MemoryStream())
            {
                Request.Body.CopyTo(stream);
                parameters = stream.ToArray();
            }
            var strRequest = Encoding.ASCII.GetString(parameters);
            var ipnSecret = _config["IpnSecret"];

            if (Helper.VerifyIpnResponse(strRequest, Request.Headers["hmac"], ipnSecret,
                out Dictionary<string, string> values))
            {
                values.TryGetValue("first_name", out string firstName);
                values.TryGetValue("last_name", out string lastName);
                values.TryGetValue("email", out string email);
                values.TryGetValue("amount1", out string amount1);
                values.TryGetValue("subtotal", out string subtotal);
                values.TryGetValue("status", out string status);
                values.TryGetValue("status_text", out string statusText);

                var newPaymentStatus = Helper.GetPaymentStatus(status, statusText);

                switch (newPaymentStatus)
                {
                    case PaymentStatus.Pending:
                        {
                            //TODO: update order status and use logging mechanism
                            //order is pending
                            Debug.WriteLine($"Order Status {newPaymentStatus}");
                        }
                        break;
                    case PaymentStatus.Authorized:
                        {
                            //order is authorized
                            Debug.WriteLine("Order is Authorized");
                        }
                        break;
                    case PaymentStatus.Paid:
                        {
                            //order is paid
                            Debug.WriteLine("Order is Paid");
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                //Issue Occurred with CoinPayments IPN  
                Debug.WriteLine("Issue Occurred with CoinPayments IPN");
            }

            //nothing should be rendered to visitor  
            return Content("");
        }
    }   
}
