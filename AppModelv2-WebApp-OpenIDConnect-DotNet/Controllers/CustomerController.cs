using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using AppModelv2_WebApp_OpenIDConnect_DotNet.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace AppModelv2_WebApp_OpenIDConnect_DotNet.Controllers
{
    public class CustomerController : Controller
    {
        string IdToken;
        // GET: Customer
        public ActionResult Index()
        {
            var userClaims = User.Identity as System.Security.Claims.ClaimsIdentity;


            IdToken = System.Web.HttpContext.Current.Session["IdToken_Ticket"].ToString();
            var httpClient = new System.Net.Http.HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", IdToken);

            const string environmentsUri = "https://api.businesscentral.dynamics.com/v2.0/d1176b80-8023-4b7a-adc8-99a22336517f/SBLawyer_TEST/api/sbconsulting/sblawyer/v1.0/companies(e34b1b2c-7f09-ec11-bb73-000d3a3926f4)/customers";

            var response = httpClient.GetAsync(environmentsUri).Result;
            var content = response.Content.ReadAsStringAsync().Result;

            var result = JsonConvert.DeserializeObject<SBLClientRoot>(content);
            
            return View(result.Value);
        }

        public ActionResult Create()
        {
            var userClaims = User.Identity as System.Security.Claims.ClaimsIdentity;
            return View();
        }

        [HttpPost]
        public ActionResult Create(SBLClient customer)
        {
            IdToken = System.Web.HttpContext.Current.Session["IdToken_Ticket"].ToString();
            //Construction du lien API
            string BaseURL = ConfigurationManager.AppSettings["BaseURL"];
            string WorkTypeAPI = ConfigurationManager.AppSettings["worktypes"];
            string BCCompany = ConfigurationManager.AppSettings["BCCompany"];

            Uri u = new Uri(BaseURL + WorkTypeAPI + BCCompany);

            string payload = JsonConvert.SerializeObject(customer);

            HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
            var t = Task.Run(() => PostURI(u, c, IdToken));
            t.Wait();

            return View();
        }

        static async Task<string> PostURI(Uri u, HttpContent c, string IdToken)
        {
            var response = string.Empty;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", IdToken);
                HttpResponseMessage result = await client.PostAsync(u, c);
                if (result.IsSuccessStatusCode)
                {
                    response = result.StatusCode.ToString();
                }
            }
            return response;
        }

        public ActionResult ActionFamily()
        {
            List<ActionFamily> actionF = ActionFamilyDataContext.GetActionFamilies(false, "");
            return View();
        }
    }
}