using AppModelv2_WebApp_OpenIDConnect_DotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppModelv2_WebApp_OpenIDConnect_DotNet.Controllers
{
    public class DashboardController : BaseController
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        private const string ClientId = "clientId";
        private const string ClientSecret = "clientSecret";
        private const string AntiforgeryToken = "state";
        // GET: Dashboard
        public ActionResult Index(string filterWeek, string filterMonth, string filterYear, string filterMode)
        {

            filterDates fd;

            switch (filterMode)
            {
                case "week":
                    fd = getDatesFromFilter(filterWeek, filterMode);
                    ViewData["filterMode"] = "week";
                    break;

                case "month":
                    fd = getDatesFromFilter(filterMonth, filterMode);
                    ViewData["filterMode"] = "month";
                    break;

                case "year":
                    fd = getDatesFromFilter(filterYear, filterMode);
                    ViewData["filterMode"] = "year";
                    break;

                default:
                    fd = getDatesFromFilter(DateTime.Now.ToString(), "week");
                    ViewData["filterMode"] = "week";
                    filterMode = "week";
                    break;
            }

            StartDate = fd.StartDate;
            EndDate = fd.EndDate;

            ViewData["Title"] = "Dashboard";//YSA-DO Add Resources Resources.Resources.Dashboard;

            //On récupère et on applique les paramètres utilisateurs
            UserParameters parameters = UserParametersDataContext.GetUserParameters(true);
            ViewData["SerieColor1"] = parameters.Color_Serie_1;
            ViewData["SerieColor2"] = parameters.Color_Serie_2;
            ViewData["SerieColor3"] = parameters.Color_Serie_3;
            ViewData["SerieColor4"] = parameters.Color_Serie_4;
            ViewData["SerieColor5"] = parameters.Color_Serie_5;
            ViewData["SerieColor6"] = parameters.Color_Serie_6;
            //ViewData["NavisionUserName"] = parameters.NavisionUserName;//Add 
            ViewData["UsernameLogin"] = System.Web.HttpContext.Current.Session["Username"];  //YSA Nav User
            ViewData["ShowWebPortalSetup"] = parameters.Can_Read_Web_Portal_Setup;
            ViewData["ShowExpenseSheet"] = parameters.Can_Read_ExpenseSheet;
            ViewData["ShowTimeSheet"] = parameters.Can_Read_TimeSheet;
            ViewData["CanReadGaugeChart"] = parameters.Can_Read_Gauge_Chart; //Audrey
            ViewData["CanReadGaugeChartHour"] = parameters.Can_Read_Gauge_Chart_Hour; // YSA Gauge Chart
            ViewData["CanReadHoursDistribution"] = parameters.Can_Read_Hours_Distribution;
            ViewData["CanReadTotalHours"] = parameters.Can_Read_Total_Hours_Chart;
            ViewData["CanReadSynthesis"] = parameters.Can_Read_TimeSheet_Synt;
            ViewData["TimerValidation"] = parameters.Timer_Validation;

            //On fournit via le viewBag les dates de filtres par défaut (semaine en cours)
            //C'est moche à cause des américains qui savent pas écrire les dates correctement
            ViewBag.startDate = StartDate.ToShortDateString();
            ViewBag.endDate = EndDate.ToShortDateString();
            ViewBag.start = StartDate;
            return View();
        }
        public filterDates getDatesFromFilter(string filterDate, string filterMode)
        {
            filterDates dates = new filterDates();

            switch (filterMode)
            {
                case "week":
                    int daysToRemove = (int)(Convert.ToDateTime(filterDate).DayOfWeek) == 0 ? 7 : (int)(Convert.ToDateTime(filterDate).DayOfWeek);
                    dates.StartDate = Convert.ToDateTime(filterDate).AddDays(1 - daysToRemove).Date;
                    dates.EndDate = dates.StartDate.AddDays(6);
                    break;
                case "month":
                    dates.StartDate = Convert.ToDateTime(filterDate);
                    dates.EndDate = dates.StartDate.AddDays(DateTime.DaysInMonth(dates.StartDate.Year, dates.StartDate.Month) - 1);
                    break;
                case "year":
                    dates.StartDate = new DateTime().AddYears(Convert.ToInt32(filterDate) - 1);
                    dates.EndDate = dates.StartDate.AddYears(1).AddDays(-1);
                    break;
            }

            return dates;
        }
        //private void ClearSession()
        //{
        //    Session[ClientId] = null;
        //    Session[ClientSecret] = null;
        //    Session[AntiforgeryToken] = null;
        //}
        //private ViewResult Error(string error, string description)
        //{
        //    ClearSession();
        //    return View("Error", new ErrorModel { Message = error, Description = description ?? "(none)" });
        //}
    }
    public struct filterDates
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}