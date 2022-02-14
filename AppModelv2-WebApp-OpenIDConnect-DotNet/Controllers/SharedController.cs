using AppModelv2_WebApp_OpenIDConnect_DotNet.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppModelv2_WebApp_OpenIDConnect_DotNet.Controllers
{
    public class SharedController : BaseController
    {
        #region Récupération des identifiants en fonction du label (combinaison nom + code)
        public ActionResult GetMatterChilds(string Matter_No, string Partner_Code, string Sell_to_Customer_No, string gridType)
        {
            try
            {
                MatterChild matterChild = new MatterChild()
                {
                    Matter_No = "",
                    Matter_Line_Label = "",
                    Matter_Line_No = 0,
                    Partner_Code = "",
                    Partner_Label = "",
                    Sell_to_Customer_No = "",
                    SBLClient_Label = "",
                    Matter_Entry_Type = 0
                };

                matterChild.Matter_No = Matter_No;

                MatterLine matterLine;

                switch (gridType)
                {
                    case "time":
                        matterLine = MatterLineDataContext.GetTimeMatterMatterLines(Matter_No).FirstOrDefault();
                        break;
                    default:
                        matterLine = MatterLineDataContext.GetExpenseMatterMatterLines(Matter_No).FirstOrDefault();
                        break;
                }

                if (matterLine != null)
                {
                    matterChild.Matter_Line_Label = matterLine.Matter_Line_Label;
                    matterChild.Matter_Line_No = matterLine.Line_No;
                    matterChild.Matter_Entry_Type = matterLine.Matter_Entry_Type;
                }

                Resource resource = ResourceDataContext.GetAssociates(false).Where(a => a.No == Partner_Code).FirstOrDefault();
                if (resource != null)
                {
                    matterChild.Partner_Code = resource.No;
                    matterChild.Partner_Label = resource.Partner_Label;
                }

                SBLClient sBLClient = SBLClientDataContext.GetSBLClients(false).Where(c => c.No == Sell_to_Customer_No).FirstOrDefault();
                if (sBLClient != null)
                {
                    matterChild.Sell_to_Customer_No = sBLClient.No;
                    matterChild.SBLClient_Label = sBLClient.SBLClient_Label;
                }

                return Json(new ControllerReturn() { ObjectToReturn = matterChild }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new ControllerReturn() { Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetAssociateFromPartner_Label(string Partner_Label)
        {
            try
            {
                if (Partner_Label != "")
                {
                    return Json(ResourceDataContext.GetAssociates(false).Where(a => a.Partner_Label == Partner_Label).FirstOrDefault(), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new ControllerReturn() { ObjectToReturn = new Resource() }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new ControllerReturn() { Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetSBLClientFromSBLClient_Label(string SBLClient_Label)
        {
            try
            {
                if (SBLClient_Label != "")
                {
                    return Json(new ControllerReturn()
                    {
                        ObjectToReturn = SBLClientDataContext.GetSBLClients(false).Where(c => c.SBLClient_Label == SBLClient_Label).FirstOrDefault()
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new ControllerReturn() { ObjectToReturn = new SBLClient() }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new ControllerReturn() { Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetMatterLineDetails(string Matter_Line_Label, string Matter_No)
        {
            try
            {
                if (!String.IsNullOrEmpty(Matter_Line_Label) && !String.IsNullOrEmpty(Matter_No))
                {
                    return Json(new ControllerReturn()
                    {
                        ObjectToReturn = MatterLineDataContext.GetTimeMatterMatterLines(Matter_No).Where(ml => ml.Matter_Line_Label == Matter_Line_Label).FirstOrDefault()
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new ControllerReturn() { ObjectToReturn = new MatterLine() }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new ControllerReturn() { Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion



        #region récupération des listes
        
        public JsonResult GetAssociates()
        {
            try
            {
                List<Resource> associates = new List<Resource>();
                foreach (Resource r in ResourceDataContext.GetAssociates(false))
                {
                    associates.Add(r);
                }

                return Json(associates, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetSBLClients()
        {
            try
            {
                List<SBLClient> clients = new List<SBLClient>();
                foreach (SBLClient c in SBLClientDataContext.GetSBLClients(false))
                {
                    clients.Add(c);
                }

                return Json(clients, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetUserMatterList()
        {
            try
            {
                List<Matter> userMatterList = new List<Matter>();
                foreach (Matter m in MatterDataContext.GetUserMatters("", "", "", ""))
                {
                    userMatterList.Add(m);
                }

                return Json(userMatterList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetMatters(string Partner_Label, string SBLClient_Label)
        {
            try
            {

                if (Partner_Label != "" && SBLClient_Label != "")
                {
                    Resource resource = ResourceDataContext.GetAssociates(false).Where(a => a.Partner_Label == Partner_Label).FirstOrDefault();
                    SBLClient sBLClient = SBLClientDataContext.GetSBLClients(false).Where(c => c.SBLClient_Label == SBLClient_Label).FirstOrDefault();

                    if (resource != null && sBLClient != null)
                    {
                        return Json(MatterDataContext.GetMatters("", "", sBLClient.No, resource.No).Where(m => m.Editable_Status == true), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new List<Matter>(), JsonRequestBehavior.AllowGet);
                    }
                }
                else if (Partner_Label != "")
                {
                    Resource resource = ResourceDataContext.GetAssociates(false).Where(a => a.Partner_Label == Partner_Label).FirstOrDefault();
                    if (resource != null)
                    {
                        return Json(MatterDataContext.GetMatters("", "", "", resource.No).Where(m => m.Editable_Status == true), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new List<Matter>(), JsonRequestBehavior.AllowGet);
                    }
                }
                else if (SBLClient_Label != "")
                {
                    SBLClient sBLClient = SBLClientDataContext.GetSBLClients(false).Where(c => c.SBLClient_Label == SBLClient_Label).FirstOrDefault();
                    if (sBLClient != null)
                    {
                        return Json(MatterDataContext.GetMatters("", "", sBLClient.No, "").Where(m => m.Editable_Status == true), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new List<Matter>(), JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(MatterDataContext.GetUserMatters("", "", "", "").Where(m => m.Editable_Status == true), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetMatterLines(string Matter_No, string Type)
        {
            try
            {
                IEnumerable<MatterLine> matterLines;
                switch (Type)
                {
                    case "time":
                        matterLines = MatterLineDataContext.GetTimeMatterMatterLines(Matter_No);
                        break;
                    default:
                        matterLines = MatterLineDataContext.GetExpenseMatterMatterLines(Matter_No);
                        break;
                }
                return Json(matterLines, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetCommentLines(int Matter_Line_No, string Matter_No, int Matter_Entry_Type, string Type)
        {
            try
            {
                IEnumerable<CommentLine> commentLines;
                commentLines = CommentLineDataContext.GetCommentLinesFromMatter(Matter_No, Matter_Entry_Type);

                return Json(new ControllerReturn() { ObjectToReturn = commentLines }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new ControllerReturn() { Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        public ActionResult keepAlive()
        {
            HttpContext.Session["KeepSessionAlive"] = DateTime.Now;
            return Json(new ControllerReturn() { ObjectToReturn = "OK" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckDate(string Planning_Date)
        {
            try
            {
                DateTime date = DateTime.ParseExact(Planning_Date, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                MixedLedgerEntryJnlLineDataContext.CheckDate(date);
                return Json(new ControllerReturn() { ObjectToReturn = "OK" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new ControllerReturn() { Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult CheckDateExpense(string Planning_Date)
        {
            try
            {
                DateTime date = DateTime.ParseExact(Planning_Date, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                MixedLedgerEntryJnlLineDataContext.CheckDateExpense(date);
                return Json(new ControllerReturn() { ObjectToReturn = "OK" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new ControllerReturn() { Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult CheckQuantity_Base(decimal Quantity_Base, string Matter_No)
        {
            try
            {
                return Json(new ControllerReturn() { ObjectToReturn = MixedLedgerEntryJnlLineDataContext.CheckQuantityTime(Quantity_Base, Matter_No, User.Identity.Name.ToUpper()) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new ControllerReturn() { Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult CheckTime(string TimeString)
        {
            try
            {
                SB.Tool.Converters.DecimalToTimeStringConverter timeConverter = new SB.Tool.Converters.DecimalToTimeStringConverter();
                decimal Quantity_Base = timeConverter.ConvertAnyStringToDecimal(
                    ref TimeString,
                    ResourceDataContext.GetDefaultTimeFactor(System.Web.HttpContext.Current.Session["Username"].ToString().ToUpper(), false),
                    UserParametersDataContext.GetUserParameters(false).TimeFormat,
                    "time",
                    0,
                    "=",
                    false);

                return Json(new ControllerReturn()
                {
                    ObjectToReturn = new QuantityTime() { Time = TimeString, Quantity = Quantity_Base }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new ControllerReturn() { Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult SaveColumnSize(string ColumnName, int ColumnNewWidth, int ColumnOldWidth, string Type)
        {
            try
            {
                string cookieName = "_column" + Type + ColumnName + "Size";

                HttpCookie cookie = Request.Cookies[cookieName];
                if (cookie != null)
                    cookie.Value = ColumnNewWidth.ToString();
                else
                {
                    cookie = new HttpCookie(cookieName);
                    cookie.Value = ColumnNewWidth.ToString();
                    cookie.Expires = DateTime.Now.AddYears(1);
                }
                Response.Cookies.Add(cookie);

                return Json(new ControllerReturn()
                {
                    ObjectToReturn = cookie
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new ControllerReturn() { Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        struct MatterChild
        {
            private string matter_No;
            public string Matter_No
            {
                get { return matter_No; }
                set { matter_No = value; }
            }

            private int matter_Line_No;
            public int Matter_Line_No
            {
                get { return matter_Line_No; }
                set { matter_Line_No = value; }
            }

            private string matter_Line_Label;
            public string Matter_Line_Label
            {
                get { return matter_Line_Label; }
                set { matter_Line_Label = value; }
            }

            private string partner_Code;
            public string Partner_Code
            {
                get { return partner_Code; }
                set { partner_Code = value; }
            }

            private string partner_Label;
            public string Partner_Label
            {
                get { return partner_Label; }
                set { partner_Label = value; }
            }

            private string sell_to_Customer_No;
            public string Sell_to_Customer_No
            {
                get { return sell_to_Customer_No; }
                set { sell_to_Customer_No = value; }
            }

            private string sBLClient_Label;
            public string SBLClient_Label
            {
                get { return sBLClient_Label; }
                set { sBLClient_Label = value; }
            }

            private int matter_Entry_Type;
            public int Matter_Entry_Type
            {
                get { return matter_Entry_Type; }
                set { matter_Entry_Type = value; }
            }

        }

        struct QuantityTime
        {
            public string Time { get; set; }
            public decimal Quantity { get; set; }
        }
    }
}