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
using SBLawyerWebApplication.Helpers;

namespace AppModelv2_WebApp_OpenIDConnect_DotNet.Controllers
{
    public class UserParametersController : Controller
    {
        string IdToken;
        // GET: UserParameters
        public ActionResult Index()
        {
            //var userClaims = User.Identity as System.Security.Claims.ClaimsIdentity;


            //IdToken = System.Web.HttpContext.Current.Session["IdToken_Ticket"].ToString();
            //var httpClient = new System.Net.Http.HttpClient();
            //httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", IdToken);

            UserParameters parameters = UserParametersDataContext.GetUserParameters(true);

            //const string environmentsUri = "https://api.businesscentral.dynamics.com/v2.0/d1176b80-8023-4b7a-adc8-99a22336517f/SBLawyer_TEST/api/sbconsulting/sblawyer/v1.0/companies(e34b1b2c-7f09-ec11-bb73-000d3a3926f4)/webportalsetups";

            //var response = httpClient.GetAsync(environmentsUri).Result;
            //var content = response.Content.ReadAsStringAsync().Result;

            //var result = JsonConvert.DeserializeObject<UserParametersRoot>(content);

            //List<UserParameters> Parameters = new List<UserParameters>();
            //Parameters = result.Value;

            //foreach (UserParameters p in parameters)
            //{
            ViewData["NavisionUserName"] = parameters.User_ID;
            ViewData["UsernameLogin"] = System.Web.HttpContext.Current.Session["Username"];
            ViewData["ShowWebPortalSetup"] = parameters.Can_Read_Web_Portal_Setup;
            ViewData["ShowExpenseSheet"] = parameters.Can_Read_ExpenseSheet;
            ViewData["ShowTimeSheet"] = parameters.Can_Read_TimeSheet;
            ViewData["CanReadGaugeChart"] = parameters.Can_Read_Gauge_Chart;
            ViewData["CanReadGaugeChartHour"] = parameters.Can_Read_Gauge_Chart_Hour;
            ViewData["CanReadHoursDistribution"] = parameters.Can_Read_Hours_Distribution;
            ViewData["CanReadTotalHours"] = parameters.Can_Read_Total_Hours_Chart;
            ViewData["CanReadSynthesis"] = parameters.Can_Read_TimeSheet_Synt;
            ViewData["CanExportExcel"] = parameters.Can_Export_Excel;
            ViewData["HideUnitPrice"] = parameters.Time_Hide_Unit_Price;
            ViewData["CanUploadAttachedFile"] = parameters.Can_Upload_Attached_File;
            ViewData["PathUploasAttachedFile"] = parameters.Path_Folder_Uploaded_Files;
            ViewData["HideActionCode"] = parameters.Time_Hide_Action_Code;
            ViewData["TimerValidation"] = parameters.Timer_Validation;
            ViewData["CanFilterByDate"] = parameters.Can_Filter_By_Date;
            ViewData["CanAddRow"] = parameters.Can_Add_Row;
            ViewData["HideKilometer"] = parameters.Exp_Hide_Column_Kilometer;
            ViewData["HideTimeWriteOff"] = parameters.Time_Hide_Write_Off;
            ViewData["HideExpWriteOff"] = parameters.Exp_Hide_Write_off;


            ViewData["Serie1"] = parameters.Color_Serie_1.ToString();
            ViewData["Serie2"] = parameters.Color_Serie_2.ToString();
            ViewData["Serie3"] = parameters.Color_Serie_3.ToString();
            ViewData["Serie4"] = parameters.Color_Serie_4.ToString();
            ViewData["Serie5"] = parameters.Color_Serie_5.ToString();
            ViewData["Serie6"] = parameters.Color_Serie_6.ToString();
            //}

            return View(parameters);
        }

        /// <summary>
        /// Définit la culture de l'utilisateur
        /// </summary>
        /// <param name="culture"> culture de l'utilisateur ex:"FR-fr"</param>
        /// <returns></returns>
        public ActionResult SetCulture(string culture)
        {
            // Validate input
            culture = CultureHelper.GetImplementedCulture(culture);
            // Save culture in a cookie
            HttpCookie cookie = Request.Cookies["_culture"];
            if (cookie != null)
                cookie.Value = culture;   // update cookie value
            else
            {
                cookie = new HttpCookie("_culture");
                cookie.Value = culture;
                cookie.Expires = DateTime.Now.AddYears(1);
            }
            Response.Cookies.Add(cookie);
            return RedirectToAction("Index");
        }


        /// <summary>
        /// Lecture des affaires de l'utilisateur
        /// </summary>
        /// <param name="request"></param>
        /// <param name="over_write"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ReadUserMatters([DataSourceRequest] DataSourceRequest request, string Matter_No, string Description, string Sell_to_Customer_No, string Partner_Code)
        {
            return Json(MatterDataContext.GetUserMatters(Matter_No, Description, Sell_to_Customer_No, Partner_Code).ToDataSourceResult(request));
        }

        /// <summary>
        /// Lecture des affaires disponibles
        /// </summary>
        /// <param name="request"></param>
        /// <param name="Matter_No"></param>
        /// <param name="Description"></param>
        /// <param name="Sell_to_Customer_No"></param>
        /// <param name="Partner_Code"></param>
        /// <param name="over_write"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ReadAvailableMatters([DataSourceRequest] DataSourceRequest request, string Matter_No, string Description, string Sell_to_Customer_No, string Partner_Code)
        {
            List<Matter> userMatters = MatterDataContext.GetUserMatters("", "", "", "").ToList();

            if (!String.IsNullOrEmpty(Matter_No) || !String.IsNullOrEmpty(Description) || !String.IsNullOrEmpty(Sell_to_Customer_No) || !String.IsNullOrEmpty(Partner_Code))
                return Json(MatterDataContext.GetAvailableMatters(userMatters, Matter_No, Description, Sell_to_Customer_No, Partner_Code).ToDataSourceResult(request));
            else
                return Json(new List<Matter>().ToDataSourceResult(request));
        }

        /// <summary>
        /// Retire une affaire de la liste des affaires d'un utilisateur
        /// </summary>
        /// <param name="request">Requete Update</param>
        /// <param name="userMatter">Affaire à retirer</param>
        /// <returns>affaire modifiée</returns>
        [HttpPost]
        public JsonResult UpdateUserMatters([DataSourceRequest] DataSourceRequest request, Matter userMatter)
        {
            if (userMatter != null && ModelState.IsValid)
            {
                try
                {
                    MatterDataContext.updateUserMatter(userMatter);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(ex.Source, ex.Message);
                }
            }

            return Json(new[] { userMatter }.ToDataSourceResult(request, ModelState));
        }

        /// <summary>
        /// Ajoute une affaire à la liste des affaires favorites d'un utilisateur
        /// </summary>
        /// <param name="request">requete CRUD</param>
        /// <param name="matter">affaire à ajouter</param>
        /// <returns>affaire modifiée</returns>
        [HttpPost]
        public JsonResult UpdateAvailableMatters([DataSourceRequest] DataSourceRequest request, Matter matter)
        {
            if (matter != null && ModelState.IsValid)
            {
                try
                {
                    MatterDataContext.updateAvailableMatter(matter);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(ex.Source, ex.Message);
                }
            }

            return Json(new[] { matter }.ToDataSourceResult(request, ModelState));
        }

        /// <summary>
        /// Met à jour l'état de favoris d'une affaire
        /// </summary>
        /// <param name="add">true si c'est un ajout aux favoris, sinon false</param>
        /// <param name="matter_No">numéro de l'affaire</param>
        /// <returns>numéro de l'affaire</returns>
        public ActionResult updateOneUserMatter(bool add, string matter_No)
        {
            Matter userMatter = new Matter() { Selected = true, Matter_No = matter_No };

            if (add)
            {
                try
                {
                    MatterDataContext.updateAvailableMatter(userMatter);
                }
                catch (Exception ex)
                {
                    matter_No = "ERROR-" + matter_No;
                }
            }
            else
            {
                try
                {
                    MatterDataContext.updateUserMatter(userMatter);
                }
                catch (Exception ex)
                {
                    matter_No = "ERROR-" + matter_No;
                }
            }

            return Json(matter_No, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Met à jour les paramètres de couleur
        /// </summary>
        /// <param name="SerieColor1"></param>
        /// <param name="SerieColor2"></param>
        /// <param name="SerieColor3"></param>
        /// <param name="SerieColor4"></param>
        /// <param name="SerieColor5"></param>
        /// <param name="SerieColor6"></param>
        /// <param name="SelectedLineBackgroundColor"></param>
        /// <param name="SelectedLineForegroundColor"></param>
        /// <param name="JnlBackgroundColor"></param>
        /// <param name="JnlForegroundColor"></param>
        /// <param name="MleBackgroundColor"></param>
        /// <param name="MleForegroundColor"></param>
        /// <param name="CallBackBackgroundColor"></param>
        /// <param name="CallBackForegroundColor"></param>
        /// <param name="NotEditableBackgroundColor"></param>
        /// <param name="NotEditableForegroundColor"></param>
        /// <returns></returns>
        public ActionResult updateColors(
            string UserId,
            string NavisionUserName,
            string SerieColor1,
            string SerieColor2,
            string SerieColor3,
            string SerieColor4,
            string SerieColor5,
            string SerieColor6,
            string SelectedLineBackgroundColor,
            string SelectedLineForegroundColor,
            string JnlBackgroundColor,
            string JnlForegroundColor,
            string MleBackgroundColor,
            string MleForegroundColor,
            string CallBackBackgroundColor,
            string CallBackForegroundColor,
            string NotEditableBackgroundColor,
            string NotEditableForegroundColor
          )
        {
            //Si le paramétrage a un UserId, on peut faire une mise à jour, sinon, il faut crééer le paramétrage de l'utilisateur actuel
            if (UserId != "")
                UserParametersDataContext.updateColor(
                    SerieColor1,
                    SerieColor2,
                    SerieColor3,
                    SerieColor4,
                    SerieColor5,
                    SerieColor6,
                    SelectedLineBackgroundColor,
                    SelectedLineForegroundColor,
                    JnlBackgroundColor,
                    JnlForegroundColor,
                    MleBackgroundColor,
                    MleForegroundColor,
                    CallBackBackgroundColor,
                    CallBackForegroundColor,
                    NotEditableBackgroundColor,
                    NotEditableForegroundColor);
            else
                UserParametersDataContext.createUserParametersWithColors(
                    SerieColor1,
                    SerieColor2,
                    SerieColor3,
                    SerieColor4,
                    SerieColor5,
                    SerieColor6,
                    SelectedLineBackgroundColor,
                    SelectedLineForegroundColor,
                    JnlBackgroundColor,
                    JnlForegroundColor,
                    MleBackgroundColor,
                    MleForegroundColor,
                    CallBackBackgroundColor,
                    CallBackForegroundColor,
                    NotEditableBackgroundColor,
                    NotEditableForegroundColor
                    );

            return Json(System.Web.HttpContext.Current.Session["Username"].ToString().ToUpper());
        }

        /// <summary>
        /// Met à jour les paramètres de visibilité des éléments
        /// </summary>
        /// <param name="HideQuantityTime"></param>
        /// <param name="HideLineAmountTime"></param>
        /// <param name="HideMatterLineColumnTime"></param>
        /// <param name="HideUnitOfMeasureTime"></param>
        /// <param name="HidePostingConfirmMessageTime"></param>
        /// <param name="HideQuantityExpense"></param>
        /// <param name="HideLineAmountExpense"></param>
        /// <param name="HideUnitOfMeasureExpense"></param>
        /// <param name="HidePostingConfirmMessageExpense"></param>
        /// <param name="HideUnitPrice"></param>
        /// <returns></returns>
        public ActionResult updateVisibility(
        string UserId,
        bool HideQuantityTime,
        bool HideLineAmountTime,
        bool HideMatterLineColumnTime,
        bool HideUnitOfMeasureTime,
        bool HideAssociateTime,
        bool HidePostingConfirmMessageTime,
        bool HideQuantityExpense,
        bool HideLineAmountExpense,
        bool HideUnitOfMeasureExpense,
        bool HideAssociateExpense,
        bool HidePostingConfirmMessageExpense,
        bool CanReadWebPortalSetup,
        bool CanReadTimeSheet,
        bool CanReadExpenseSheet,
        bool CanReadGaugeChart,
        bool CanReadGaugeChartHour, // YSA Gauge Chart
        bool CanReadHoursDistribution,
        bool CanReadTotalHours,
        bool CanReadSynthesis,
        bool CanExportExcel,
        bool CanExportExcelExpenseSheet,
        bool HideUnitPrice,
        bool CanUploadAttachedFile,
        string PathUploadAttachedFile,
        bool HideActionCode,
        bool TimerValidation,
        bool CanFilterByDate,
        bool CanAddRow,
        bool HideKilometer,
        bool HideTimeWriteOff,
        bool HideExpWriteOff
        )
        {

            //Si le paramétrage a un UserId, on peut faire une mise à jour, sinon, il faut crééer le paramétrage de l'utilisateur actuel
            if (UserId != "")
                UserParametersDataContext.updateVisibility(
                    HideQuantityTime,
                    HideLineAmountTime,
                    HideMatterLineColumnTime,
                    HideUnitOfMeasureTime,
                    HideAssociateTime,
                    HideUnitPrice,
                    HidePostingConfirmMessageTime,
                    HideQuantityExpense,
                    HideLineAmountExpense,
                    HideUnitOfMeasureExpense,
                    HideAssociateExpense,
                    HidePostingConfirmMessageExpense,
                    CanReadWebPortalSetup,
                    CanReadTimeSheet,
                    CanReadExpenseSheet,
                    CanReadGaugeChart,
                    CanReadGaugeChartHour, // YSA Gauge Chart
                    CanReadHoursDistribution,
                    CanReadTotalHours,
                    CanReadSynthesis,
                    CanExportExcel,
                    CanExportExcelExpenseSheet,
                    CanUploadAttachedFile,
                    PathUploadAttachedFile,
                    HideActionCode,
                    TimerValidation,
                    CanFilterByDate,
                    CanAddRow,
                    HideKilometer,
                    HideTimeWriteOff,
                    HideExpWriteOff
                    );
            else
                UserParametersDataContext.createUserParametersWithVisibilities(
                    HideQuantityTime,
                    HideLineAmountTime,
                    HideMatterLineColumnTime,
                    HideUnitOfMeasureTime,
                    HideAssociateTime,
                    HideUnitPrice,
                    HidePostingConfirmMessageTime,
                    HideQuantityExpense,
                    HideLineAmountExpense,
                    HideUnitOfMeasureExpense,
                    HideAssociateExpense,
                    HidePostingConfirmMessageExpense,
                    CanReadWebPortalSetup,
                    CanReadTimeSheet,
                    CanReadExpenseSheet,
                    CanReadGaugeChart,
                    CanReadGaugeChartHour, // YSA Gauge Chart
                    CanReadHoursDistribution,
                    CanReadTotalHours,
                    CanReadSynthesis,
                    CanExportExcel,
                    CanExportExcelExpenseSheet,
                    CanUploadAttachedFile,
                    PathUploadAttachedFile,
                    HideActionCode,
                    TimerValidation,
                    CanFilterByDate,
                    CanAddRow,
                    HideKilometer,
                    HideTimeWriteOff,
                    HideExpWriteOff
                   );

            return Json(System.Web.HttpContext.Current.Session["Username"].ToString().ToUpper(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckWorkTypesByDefaut(string Matter_No)
        {
            try
            {
                return Json(WorkTypeDataContext.checkDefautWorktype(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        #region Delete
        public ActionResult resetParameters()
        {
            var deletedParameters = UserParametersDataContext.deleteUserParameters();

            return Json(System.Web.HttpContext.Current.Session["Username"].ToString().ToUpper(), JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}