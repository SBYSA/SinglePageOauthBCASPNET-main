using SBLawyerWebApplication.Helpers;
using System;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace AppModelv2_WebApp_OpenIDConnect_DotNet.Controllers
{
    public class BaseController : Controller
    {
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string cultureName = null;

            // Attempt to read the culture cookie from Request
            HttpCookie cultureCookie = Request.Cookies["_culture"];
            if (cultureCookie != null)
                cultureName = cultureCookie.Value;
            else
                cultureName = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ?
                        Request.UserLanguages[0] :  // obtain it from HTTP header AcceptLanguages
                        null;
            // Validate culture name
            cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe

            // Modify current thread's cultures            
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            //SBLawyerWebApplication.Libs.SBLawyerCredential.init(@"SB\aroulic", "PassWord#");
            //Console.WriteLine(SBLawyerWebApplication.Libs.SBLawyerCredential.AuthenticationType.Value);

            return base.BeginExecuteCore(callback, state);
        }
        public struct ControllerReturn
        {
            public string Error { get; set; }
            public Object ObjectToReturn { get; set; }
        }
    }
}