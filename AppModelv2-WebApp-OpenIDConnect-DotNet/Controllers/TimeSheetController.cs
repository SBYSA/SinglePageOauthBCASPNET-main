using AppModelv2_WebApp_OpenIDConnect_DotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppModelv2_WebApp_OpenIDConnect_DotNet.Controllers
{
    public class TimeSheetController : BaseController
    {
        DateTime sDate;
        DateTime eDate;
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Permet d'envoyer le context dans la nouvelle classe qui sérialise le JsonResult
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected JsonResult JsonUnspecified(object data)
        {
            return new JsonUnspecifiedResult
            {
                Data = data
            };
        }

        public ActionResult Index(string startDate, string endDate)
        {
            ViewData["Title"] = Resources.Resources.TimeSheet;


            //On récupère et on applique les paramètres utilisateurs
            ForeignKeyValues FKValues = ForeignKeyValuesDataContext.getTimeForeignKeyValues();

            //On récupère la taille des colonnes dans les cookies
            HttpCookie columnTimePlanning_DateSizeCookie = Request.Cookies["_columnTimePlanning_DateSize"];
            HttpCookie columnTimePartner_LabelSizeCookie = Request.Cookies["_columnTimePartner_LabelSize"];
            HttpCookie columnTimeSBLClient_LabelSizeCookie = Request.Cookies["_columnTimeSBLClient_LabelSize"];
            HttpCookie columnTimeMatter_LabelSizeCookie = Request.Cookies["_columnTimeMatter_LabelSize"];
            HttpCookie columnTimeMatter_Line_LabelSizeCookie = Request.Cookies["_columnTimeMatter_Line_LabelSize"];
            HttpCookie columnTimeAction_CodeSizeCookie = Request.Cookies["_columnTimeAction_CodeSize"];
            HttpCookie columnTimeDescriptionSizeCookie = Request.Cookies["_columnTimeDescriptionSize"];
            HttpCookie columnTimeTimeSizeCookie = Request.Cookies["_columnTimeTimeSize"];
            HttpCookie columnTimeQuantity_BaseSizeCookie = Request.Cookies["_columnTimeQuantity_BaseSize"];
            HttpCookie columnTimeUnit_of_Measure_CodeSizeCookie = Request.Cookies["_columnTimeUnit_of_Measure_CodeSize"];
            HttpCookie columnTimeLine_AmountSizeCookie = Request.Cookies["_columnTimeLine_AmountSize"];
            HttpCookie ColumnTimeUnitPriceSizeCookie = Request.Cookies["_columnClientQuantity_size"];

            FKValues.Parameters.ColumnTimePlanning_DateSize = columnTimePlanning_DateSizeCookie != null ? Convert.ToInt32(columnTimePlanning_DateSizeCookie.Value) : 120;
            if (columnTimePartner_LabelSizeCookie != null) { FKValues.Parameters.ColumnTimePartner_LabelSize = Convert.ToInt32(columnTimePartner_LabelSizeCookie.Value); }
            if (columnTimeSBLClient_LabelSizeCookie != null) { FKValues.Parameters.ColumnTimeSBLClient_LabelSize = Convert.ToInt32(columnTimeSBLClient_LabelSizeCookie.Value); }
            if (columnTimeMatter_LabelSizeCookie != null) { FKValues.Parameters.ColumnTimeMatter_LabelSize = Convert.ToInt32(columnTimeMatter_LabelSizeCookie.Value); }
            FKValues.Parameters.ColumnTimeMatter_Line_LabelSize = columnTimeMatter_Line_LabelSizeCookie != null ? Convert.ToInt32(columnTimeMatter_Line_LabelSizeCookie.Value) : 135;
            FKValues.Parameters.ColumnTimeAction_CodeSize = columnTimeAction_CodeSizeCookie != null ? Convert.ToInt32(columnTimeAction_CodeSizeCookie.Value) : 135;
            if (columnTimeDescriptionSizeCookie != null) { FKValues.Parameters.ColumnTimeDescriptionSize = Convert.ToInt32(columnTimeDescriptionSizeCookie.Value); }
            FKValues.Parameters.ColumnTimeTimeSize = columnTimeTimeSizeCookie != null ? Convert.ToInt32(columnTimeTimeSizeCookie.Value) : 90;
            FKValues.Parameters.ColumnTimeQuantity_BaseSize = columnTimeQuantity_BaseSizeCookie != null ? Convert.ToInt32(columnTimeQuantity_BaseSizeCookie.Value) : 80;
            FKValues.Parameters.ColumnTimeLine_AmountSize = columnTimeLine_AmountSizeCookie != null ? Convert.ToInt32(columnTimeLine_AmountSizeCookie.Value) : 120;
            FKValues.Parameters.ColumnTimeUnit_of_Measure_CodeSize = columnTimeUnit_of_Measure_CodeSizeCookie != null ? Convert.ToInt32(columnTimeUnit_of_Measure_CodeSizeCookie.Value) : 80;
            FKValues.Parameters.ColumnUnitPriceSize = ColumnTimeUnitPriceSizeCookie != null ? Convert.ToInt32(ColumnTimeUnitPriceSizeCookie.Value) : 80;

            ViewData["NavisionUserName"] = FKValues.Parameters.NavisionUserName;
            ViewData["UsernameLogin"] = System.Web.HttpContext.Current.Session["Username"];
            ViewData["ShowWebPortalSetup"] = FKValues.Parameters.Can_Read_Web_Portal_Setup;
            ViewData["ShowExpenseSheet"] = FKValues.Parameters.Can_Read_ExpenseSheet;
            ViewData["ShowTimeSheet"] = FKValues.Parameters.Can_Read_TimeSheet;
            ViewData["CanExportExcel"] = FKValues.Parameters.Can_Export_Excel;
            ViewData["TimerValidation"] = FKValues.Parameters.Timer_Validation;
            //On récupère l'unité de mesure par défaut
            ViewBag.baseUnitOfMeasure = ResourceDataContext.GetBaseUnitOfMeasure(User.Identity.Name.ToUpper(), true);

            //On convertit les filtres recus en date qu'on met au valeurs par défaut à l'ouverture de la page
            sDate = !String.IsNullOrEmpty(startDate) ? Convert.ToDateTime(startDate) : DateTime.Now.AddDays(1 - ((int)(DateTime.Now.DayOfWeek))).Date;
            eDate = !String.IsNullOrEmpty(endDate) ? Convert.ToDateTime(endDate) : DateTime.Now.AddDays(7 - ((int)(DateTime.Now.DayOfWeek))).Date;

            //On fournit via le viewBag les dates de filtres par défaut (semaine en cours)
            ViewBag.startDate = sDate.ToShortDateString();
            ViewBag.endDate = eDate.ToShortDateString();

            //On ouvre la vue avec les lignes de temps demandées
            return View(FKValues);
        }

        /// <summary>
        /// Valide les lignes de temps
        /// </summary>
        /// <returns></returns>
        public ActionResult ValidateTimeJournalLines()
        {
            try
            {
                MixedLedgerEntryJnlLineDataContext.ValidateTimeJournalLines();
                return Json(new ControllerReturn() { ObjectToReturn = "OK" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new ControllerReturn() { Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #region Create
        [HttpPost]
        public JsonResult Create([DataSourceRequest] DataSourceRequest request, MixedLedgerEntryJnlLine mixedLedgerEntryJnlLine)
        {
            if (mixedLedgerEntryJnlLine != null && ModelState.IsValid)
            {
                try
                {
                    MixedLedgerEntryJnlLineDataContext.createTimeLine(mixedLedgerEntryJnlLine);
                }
                catch (Exception ex)
                {
                    mixedLedgerEntryJnlLine.LineId = 0;
                    ModelState.AddModelError(ex.Source, ex.Message);
                }
            }
            //Replace Json to JsonUnspecified
            return JsonUnspecified(new[] { mixedLedgerEntryJnlLine }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Read
        [HttpPost]
        public JsonResult Read([DataSourceRequest] DataSourceRequest request, DateTime startDate, DateTime endDate, bool over_write)
        {
            //Replace Json to JsonUnspecified
            return JsonUnspecified(MixedLedgerEntryJnlLineDataContext.GetMixedLedgerEntryJnlLines(startDate, endDate, "time", over_write, User.Identity.Name.ToUpper()).ToDataSourceResult(request));
        }

        #region récupération des listes
        public JsonResult GetWorkTypes(string Matter_No)
        {
            try
            {
                return Json(WorkTypeDataContext.GetWorkTypesFromMatter(Matter_No), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        #region récupération du worktype par default 
        public JsonResult GetWorkTypesByDefaut(string Matter_No)
        {
            try
            {
                return Json(WorkTypeDataContext.GetWorkTypeByDefaut(Matter_No), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }




        public JsonResult GetDesignationActionFromAction_Code(string Action_Code)
        {
            try
            {
                return Json(new ControllerReturn() { ObjectToReturn = WorkTypeDataContext.GetWorkTypes(false).Where(w => w.Code == Action_Code).FirstOrDefault() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new ControllerReturn() { Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #endregion
        #endregion

        #region Update
        [HttpPost]
        public JsonResult Update([DataSourceRequest] DataSourceRequest request, MixedLedgerEntryJnlLine mixedLedgerEntryJnlLine)
        {
            if (mixedLedgerEntryJnlLine != null && ModelState.IsValid)
            {
                try
                {
                    MixedLedgerEntryJnlLineDataContext.updateTimeLine(mixedLedgerEntryJnlLine);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(ex.Source, ex.Message);
                }
            }
            //Replace Json to JsonUnspecified
            return JsonUnspecified(new[] { mixedLedgerEntryJnlLine }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Delete
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete([DataSourceRequest] DataSourceRequest request, MixedLedgerEntryJnlLine mixedLedgerEntryJnlLine)
        {
            try
            {
                MixedLedgerEntryJnlLineDataContext.deleteTimeLine(mixedLedgerEntryJnlLine);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(ex.Source, ex.Message);
            }

            return Json(new[] { mixedLedgerEntryJnlLine }.ToDataSourceResult(request, ModelState));
        }
        #endregion
    }
}