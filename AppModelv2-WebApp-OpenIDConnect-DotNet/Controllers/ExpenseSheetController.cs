using AppModelv2_WebApp_OpenIDConnect_DotNet.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppModelv2_WebApp_OpenIDConnect_DotNet.Controllers
{
    public class ExpenseSheetController : BaseController
    {
        DateTime sDate;
        DateTime eDate;

        protected JsonResult JsonUnspecified(object data)
        {
            return new JsonUnspecifiedResult
            {
                Data = data
            };
        }

        public ActionResult Index(string startDate, string endDate)
        {
            ViewData["Title"] = Resources.Resources.ExpenseSheet;

            //On récupère et on applique les paramètres utilisateurs
            ForeignKeyValues FKValues = ForeignKeyValuesDataContext.getExpenseForeignKeyValues();

            //On récupère la taille des colonnes dans les cookies
            HttpCookie columnExpensePlanning_DateSizeCookie = Request.Cookies["_columnExpensePlanning_DateSize"];
            HttpCookie columnExpensePartner_LabelSizeCookie = Request.Cookies["_columnExpensePartner_LabelSize"];
            HttpCookie columnExpenseSBLClient_LabelSizeCookie = Request.Cookies["_columnExpenseSBLClient_LabelSize"];
            HttpCookie columnExpenseMatter_LabelSizeCookie = Request.Cookies["_columnExpenseMatter_LabelSize"];
            HttpCookie columnExpenseMatter_Line_LabelSizeCookie = Request.Cookies["_columnExpenseMatter_Line_LabelSize"];
            HttpCookie columnExpenseAction_Family_CodeSizeCookie = Request.Cookies["_columnExpenseAction_Family_CodeSize"];
            HttpCookie columnExpenseDescriptionSizeCookie = Request.Cookies["_columnExpenseDescriptionSize"];
            HttpCookie columnExpenseQuantity_BaseSizeCookie = Request.Cookies["_columnExpenseQuantity_BaseSize"];
            HttpCookie columnExpenseUnit_PriceSizeCookie = Request.Cookies["_columnExpenseUnit_PriceSize"];
            HttpCookie columnExpenseLine_AmountSizeCookie = Request.Cookies["_columnExpenseLine_AmountSize"];

            FKValues.Parameters.ColumnExpensePlanning_DateSize = columnExpensePlanning_DateSizeCookie != null ? Convert.ToInt32(columnExpensePlanning_DateSizeCookie.Value) : 120;
            if (columnExpensePartner_LabelSizeCookie != null) { FKValues.Parameters.ColumnExpensePartner_LabelSize = Convert.ToInt32(columnExpensePartner_LabelSizeCookie.Value); }
            if (columnExpenseSBLClient_LabelSizeCookie != null) { FKValues.Parameters.ColumnExpenseSBLClient_LabelSize = Convert.ToInt32(columnExpenseSBLClient_LabelSizeCookie.Value); }
            if (columnExpenseMatter_LabelSizeCookie != null) { FKValues.Parameters.ColumnExpenseMatter_LabelSize = Convert.ToInt32(columnExpenseMatter_LabelSizeCookie.Value); }
            FKValues.Parameters.ColumnExpenseMatter_Line_LabelSize = columnExpenseMatter_Line_LabelSizeCookie != null ? Convert.ToInt32(columnExpenseMatter_Line_LabelSizeCookie.Value) : 135;
            if (columnExpenseAction_Family_CodeSizeCookie != null) { FKValues.Parameters.ColumnExpenseAction_Family_CodeSize = Convert.ToInt32(columnExpenseAction_Family_CodeSizeCookie.Value); }
            if (columnExpenseDescriptionSizeCookie != null) { FKValues.Parameters.ColumnExpenseDescriptionSize = Convert.ToInt32(columnExpenseDescriptionSizeCookie.Value); }
            FKValues.Parameters.ColumnExpenseQuantity_BaseSize = columnExpenseQuantity_BaseSizeCookie != null ? Convert.ToInt32(columnExpenseQuantity_BaseSizeCookie.Value) : 120;
            FKValues.Parameters.ColumnExpenseUnit_PriceSize = columnExpenseUnit_PriceSizeCookie != null ? Convert.ToInt32(columnExpenseUnit_PriceSizeCookie.Value) : 100;
            FKValues.Parameters.ColumnExpenseLine_AmountSize = columnExpenseLine_AmountSizeCookie != null ? Convert.ToInt32(columnExpenseLine_AmountSizeCookie.Value) : 120;

            ViewData["NavisionUserName"] = FKValues.Parameters.NavisionUserName;
            ViewData["UsernameLogin"] = System.Web.HttpContext.Current.Session["Username"];
            ViewData["ShowWebPortalSetup"] = FKValues.Parameters.Can_Read_Web_Portal_Setup;
            ViewData["ShowExpenseSheet"] = FKValues.Parameters.Can_Read_ExpenseSheet;
            ViewData["ShowTimeSheet"] = FKValues.Parameters.Can_Read_TimeSheet;
            ViewData["CanExportExcelExpenseSheet"] = FKValues.Parameters.Can_Export_Excel_Expense;
            ViewData["PatheUploadAttachedFile"] = FKValues.Parameters.Path_Folder_Uploaded_Files;
            ViewData["TimerValidation"] = FKValues.Parameters.Timer_Validation;
            //On récupère l'unité de mesure par défaut
            ViewBag.baseUnitOfMeasure = ResourceDataContext.GetBaseUnitOfMeasure(User.Identity.Name.ToUpper(), true);


            //On convertit les filtres recus en date qu'on met au valeurs par défaut à l'ouverture de la page
            sDate = !String.IsNullOrEmpty(startDate) ? Convert.ToDateTime(startDate) : DateTime.Now.AddDays(1 - ((int)(DateTime.Now.DayOfWeek))).Date;
            eDate = !String.IsNullOrEmpty(endDate) ? Convert.ToDateTime(endDate) : DateTime.Now.AddDays(7 - ((int)(DateTime.Now.DayOfWeek))).Date;

            //On fournit via le viewBag les dates de filtres par défaut (semaine en cours)
            ViewBag.startDate = sDate.ToShortDateString();
            ViewBag.endDate = eDate.ToShortDateString();



            //On ouvre la vue avec les lignes de frais demandées
            return View(FKValues);
        }



        /// <summary>
        /// Valide les lignes de frais
        /// </summary>
        /// <returns></returns>
        public ActionResult ValidateExpenseJournalLines()
        {
            try
            {
                MixedLedgerEntryJnlLineDataContext.ValidateExpenseJournalLines();
                return Json(new ControllerReturn() { ObjectToReturn = "OK" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new ControllerReturn() { Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Sauvegarde Fichier Piéce jointe 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetFile()
        {
            //upload file 
            try
            {
                //On récupère et on applique les paramètres utilisateurs
                ForeignKeyValues FKValues = ForeignKeyValuesDataContext.getExpenseForeignKeyValues();
                string filename = "";
                string pathValue = "";
                int LineNo = 0;
                int EntryNo = 0;
                //var allowedExtensions = new[] { ".docx", ".xlsx", ".txt", ".jpeg" };
                string[] allowedExtensions = ConfigurationManager.AppSettings["Extensions"].Split(separator: new char[] { ';' });

                foreach (string upload in Request.Files)
                {
                    if (Request.Files[upload].FileName != "")
                    {


                        var extension = Path.GetExtension(Request.Files[upload].FileName);
                        if (!allowedExtensions.Contains(extension))
                        {
                            throw new Exception(Errors.Extension);
                        }
                        //on recupere les infos de Navision avec Fkvalues
                        string subPath = FKValues.Parameters.NavisionUserName;
                        pathValue = FKValues.Parameters.Path_Folder_Uploaded_Files + subPath;

                        //Create dossier mais si existe déjà ignore
                        System.IO.Directory.CreateDirectory(pathValue);
                        filename = Path.GetFileName(Request.Files[upload].FileName);
                        string fullPath = pathValue + "\\" + filename;

                        //si le fichier existe déjà on sort et message d'erreur 
                        if (System.IO.File.Exists(fullPath))
                        {
                            throw new Exception(Errors.FileExists);
                        }

                        //on sauvergarde le fichier dans le chemin voulu
                        Request.Files[upload].SaveAs(Path.Combine(pathValue, filename));

                        //On recupere les valeurs ajouées au Formadata via Ajax
                        LineNo = Convert.ToInt32(Request.Form["Line_No"]);
                        EntryNo = Convert.ToInt32(Request.Form["Entry_No"]);


                        //On appele le WS dans le model MLEJL
                        MixedLedgerEntryJnlLineDataContext.saveFile(LineNo, EntryNo, filename, pathValue);

                    }
                }

                //Si tout s'est bien passé, on renvoit a la vue Ok en Json
                return Json(new ControllerReturn() { ObjectToReturn = "OK" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Si erreur, envoie en Json du message d'erreur à la vue 
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
                    MixedLedgerEntryJnlLineDataContext.createExpenseLine(mixedLedgerEntryJnlLine);
                }
                catch (Exception ex)
                {
                    return Json(ex.Message, JsonRequestBehavior.AllowGet);
                }
            }
            return JsonUnspecified(new[] { mixedLedgerEntryJnlLine }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Read
        [HttpPost]
        public JsonResult Read([DataSourceRequest] DataSourceRequest request, DateTime startDate, DateTime endDate, bool over_write)
        {
            return JsonUnspecified(MixedLedgerEntryJnlLineDataContext.GetMixedLedgerEntryJnlLines(startDate, endDate, "expense", over_write, User.Identity.Name.ToUpper()).ToDataSourceResult(request));
        }

        #region récupération des listes
        public JsonResult GetActionFamilies(string Matter_No, int Matter_Line_No)
        {
            try
            {
                return Json(ActionFamilyDataContext.GetActionFamiliesFromMatter(Matter_No, Matter_Line_No), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetDesignationActionFromtActionFamilies(string Action_Family_Code)
        {
            try
            {
                return Json(new ControllerReturn() { ObjectToReturn = ActionFamilyDataContext.GetActionFamilies(false).Where(w => w.Code == Action_Family_Code).FirstOrDefault() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new ControllerReturn() { Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
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
                    MixedLedgerEntryJnlLineDataContext.updateExpenseLine(mixedLedgerEntryJnlLine);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(ex.Source, ex.Message);
                }
            }

            return JsonUnspecified(new[] { mixedLedgerEntryJnlLine }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region Delete
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete([DataSourceRequest] DataSourceRequest request, MixedLedgerEntryJnlLine mixedLedgerEntryJnlLine)
        {
            try
            {
                MixedLedgerEntryJnlLineDataContext.deleteExpenseLine(mixedLedgerEntryJnlLine);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(ex.Source, ex.Message);
            }

            return Json(new[] { mixedLedgerEntryJnlLine }.ToDataSourceResult(request, ModelState));
        }
        #endregion

        #region récupération taux fiscaux
        //  public JsonResult getFiscalRate(string idUser)
        [HttpPost]
        public JsonResult getFiscalRate([DataSourceRequest] DataSourceRequest request)
        {
            return JsonUnspecified(MixedLedgerEntryJnlLineDataContext.getFiscalRate(User.Identity.Name.ToUpper()));
        }
        #endregion
    }