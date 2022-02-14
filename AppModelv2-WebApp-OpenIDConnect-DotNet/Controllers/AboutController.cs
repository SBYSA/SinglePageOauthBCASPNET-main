using AppModelv2_WebApp_OpenIDConnect_DotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppModelv2_WebApp_OpenIDConnect_DotNet.Controllers
{
    public class AboutController : BaseController
    {
        private UserParameters parameters;

        // GET: About
        public ActionResult Index()
        {
            parameters = UserParametersDataContext.GetUserParameters(true);
            //ViewData["NavisionUserName"] = parameters.user; YSA ==> Add UserName
            ViewData["UsernameLogin"] = System.Web.HttpContext.Current.Session["Username"];
            ViewData["ShowWebPortalSetup"] = parameters.Can_Read_Web_Portal_Setup;
            ViewData["ShowExpenseSheet"] = parameters.Can_Read_ExpenseSheet;
            ViewData["ShowTimeSheet"] = parameters.Can_Read_TimeSheet;
            return View();
        }
    }
}