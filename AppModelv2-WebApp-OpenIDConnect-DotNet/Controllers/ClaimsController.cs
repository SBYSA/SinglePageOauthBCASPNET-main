using System.Web.Mvc;
using System;
using AppModelv2_WebApp_OpenIDConnect_DotNet.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;

namespace AppModelv2_WebApp_OpenIDConnect_DotNet.Controllers
{
    //[Authorize]
    public class ClaimsController : Controller
    {
        string TestCalim2;
        /// <summary>
        /// Add user's claims to viewbag
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var userClaims = User.Identity as System.Security.Claims.ClaimsIdentity;

            //var TestCalim = System.Web.HttpContext.Current.Response.Cookies.Add();
            TestCalim2 = System.Web.HttpContext.Current.Session["IdToken_Ticket"].ToString();
            var httpClient = new System.Net.Http.HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TestCalim2);

            const string environmentsUri = "https://api.businesscentral.dynamics.com/v2.0/d1176b80-8023-4b7a-adc8-99a22336517f/SBLawyer_TEST/api/sbconsulting/sblawyer/v1.0/companies(e34b1b2c-7f09-ec11-bb73-000d3a3926f4)/customers";

            var response = httpClient.GetAsync(environmentsUri).Result;
            var content = response.Content.ReadAsStringAsync().Result;

            var Object1 = JsonConvert.DeserializeObject<SBLClient>(content);
            
            //List<Company> custs = JsonConvert.DeserializeObject<List<Company>>(content);
            Console.WriteLine(Object1);

            //You get the user’s first and last name below:
            ViewBag.Name = userClaims?.FindFirst("name")?.Value;

            // The 'preferred_username' claim can be used for showing the username
            ViewBag.Username = userClaims?.FindFirst("preferred_username")?.Value;

            // The subject/ NameIdentifier claim can be used to uniquely identify the user across the web
            ViewBag.Subject = userClaims?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // TenantId is the unique Tenant Id - which represents an organization in Azure AD
            ViewBag.TenantId = userClaims?.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")?.Value;

            ViewBag.Test = userClaims?.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")?.Value;


            return View();
        }
        
        public ActionResult Create(SBLClient customer)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.businesscentral.dynamics.com/v2.0/d1176b80-8023-4b7a-adc8-99a22336517f/SBLawyer_TEST/api/sbconsulting/sblawyer/v1.0/companies(e34b1b2c-7f09-ec11-bb73-000d3a3926f4)/customers");
                var postJob = client.PostAsJsonAsync<SBLClient>("customer", customer);
                postJob.Wait();

                var postResult = postJob.Result;
                if (postResult.IsSuccessStatusCode)
                    return RedirectToAction("Index");
            }
            ModelState.AddModelError(string.Empty, "Server occured errors. Please check with admin!");

            return View();
        }
    }
}