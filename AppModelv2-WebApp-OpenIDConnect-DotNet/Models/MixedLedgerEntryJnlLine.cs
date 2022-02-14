using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Configuration;

namespace AppModelv2_WebApp_OpenIDConnect_DotNet.Models
{
    public class MixedLedgerEntryJnlLine
    { 
        public int LineId { get; set; }

        //Spécifique à MatterJnlLine
        public int Line_No { get; set; }

        //Spécifique à MatterLedgerEntry
        public int Entry_No { get; set; }
        public string Entry_Type { get; set; }
        public int? Matter_Entry_Type { get; set; }

        //Commun
        [UIHint("ClientResource")]
        public string Partner_Label { get; set; }

        [UIHint("ClientMatter")]
        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "IsRequired_Matter_Label")]
        public string Matter_Label { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "IsRequired_Planning_Date")]
        public DateTime Planning_Date { get; set; }

        [UIHint("ClientMatterLine")]
        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "IsRequired_Matter_Line_Label")]
        public string Matter_Line_Label { get; set; }

        [UIHint("ClientWorkType")]
        //[Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "IsRequired_Action_Code")]
        public string Action_Code { get; set; }

        [UIHint("ClientActionFamily")]
        //[Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "IsRequired_Action_Family_Code")]
        public string Action_Family_Code { get; set; }

        [UIHint("ClientSBLClient")]
        public string SBLClient_Label { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "IsRequired_Description")]
        [StringLength(250)]
        public string Description { get; set; }

        [UIHint("ClientQuantity")]
        //[PosNumberNoZero(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "ERROR_QteSupZero")]
        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "IsRequired_Quantity_Base")]
        public double? Quantity_Base { get; set; }

        //[UIHint("ClientTime")]
        //[Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "IsRequired_Time")]
        public string TimeString { get; set; }

        [UIHint("ClientQuantity")]
        public decimal? Unit_Price { get; set; }

        [UIHint("ClientAttachedFile")]
        public object File { get; set; }
        public string NameFile { get; set; }

        public int Matter_Line_No { get; set; }
        public string Sell_to_Customer_No { get; set; }
        public string Sell_to_Customer_Name { get; set; }
        public string Partner_Code { get; set; }
        public string Partner_Name { get; set; }
        public string Matter_No { get; set; }
        public string Unit_of_Measure_Code { get; set; }
        public string Description_Family { get; set; }
        public bool Editable_Entry { get; set; }
        public bool TimeOrQuantity { get; set; } //false signifie qu'on a changé la quantité
        public bool IsMatterInMyMatters { get; set; }
        public double? Line_Amount { get; set; }
        public decimal? Kilometers { get; set; }

        [UIHint("ClientWriteOff")]
        public bool? WriteOff { get; set; }

    }
    /// <summary>
    /// Permet de la vaidation d'un decimal qui doit être >=0
    /// </summary>
    public class PosNumberNoZeroAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }
            decimal number;
            if (decimal.TryParse(value.ToString(), out number))
            {
                if (number > 0)
                    return true;
            }
            return false;

        }
    }
    /// <summary>
    /// DataContext des lignes de temps ou de frais mixtes, permet d'effectuer les opération CRUD
    /// </summary>
    public static class MixedLedgerEntryJnlLineDataContext
    {
        /// <summary>
        /// Permet de récupérer la liste des lignes de temps ou frais (jnlLine et entry confondues)
        /// </summary>
        /// <param name="startDate">Date de début de recherche</param>
        /// <param name="endDate">Date de fin de recherche</param>
        /// <param name="lineType">Ty pe de ligne ("time" ou "expense")</param>
        /// <param name="over_write">Faut il ecraser les valeurs stockées dans la session?</param>
        /// <returns></returns>
        public static List<MixedLedgerEntryJnlLine> GetMixedLedgerEntryJnlLines(DateTime startDate, DateTime endDate, string lineType, bool over_write, string userId)
        {

            //On récupère les lignes sotckées en session
            List<MixedLedgerEntryJnlLine> result_session = (List<MixedLedgerEntryJnlLine>)HttpContext.Current.Session["MixedLedgerEntryJnlLines"];

            //on récupère les dates de filtres 
            DateTime actualStartDate;
            DateTime actualEndDate;

            if (HttpContext.Current.Session["actualStartDate"] != null)
                actualStartDate = (DateTime)HttpContext.Current.Session["actualStartDate"];
            else
                actualStartDate = new DateTime();

            if (HttpContext.Current.Session["actualEndDate"] != null)
                actualEndDate = (DateTime)HttpContext.Current.Session["actualEndDate"];
            else
                actualEndDate = new DateTime();

            //YSA-DO    
            //On récupère le modèle de ligne de temps
            //MatterFuncWebRef.MatterFunction webService = new MatterFuncWebRef.MatterFunction();

            string IdToken = System.Web.HttpContext.Current.Session["IdToken_Ticket"].ToString();
            var httpClient = new System.Net.Http.HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", IdToken);
            
            //YSA-DO Post Codeunit Matter Function
            //const string environmentsUri = "https://api.businesscentral.dynamics.com/v2.0/d1176b80-8023-4b7a-adc8-99a22336517f/SBLawyer_TEST/api/sbconsulting/sblawyer/v1.0/companies(e34b1b2c-7f09-ec11-bb73-000d3a3926f4)/worktypes";

            

            string templateName = "";
            switch (lineType)
            {
                case "time":
                    templateName = webService.GetTimeTemplateName();
                    break;
                default://"expense"
                    templateName = webService.GetExpenseTemplateName();
                    break;
            }


            if (startDate != actualStartDate || endDate != actualEndDate || over_write)
            {
                List<string> userMattersNos = new List<string>();
                foreach (Matter m in MatterDataContext.GetUserMatters("", "", "", ""))
                    userMattersNos.Add(m.Matter_No);

                List<MixedLedgerEntryJnlLine> mixedLedgerEntryJnlLines = new List<MixedLedgerEntryJnlLine>();

                List<MatterJnlLine> matterJnlLines = MatterJnlLineDataContext.getMatterJnlLines(startDate, endDate, templateName);
                List<MatterLedgerEntry> matterLedgerEntries = MatterLedgerEntryDataContext.getMatterLedgerEntries(startDate, endDate, lineType);

                int defaultTimeFactor = ResourceDataContext.GetDefaultTimeFactor(userId, false);
                string timeFormat = UserParametersDataContext.GetUserParameters(false).TimeFormat;
                int id = 0;

                SB.Tool.Converters.DecimalToTimeStringConverter timeConverter = new SB.Tool.Converters.DecimalToTimeStringConverter();

                foreach (MatterJnlLine mjl in matterJnlLines)
                {
                    id++;

                    MixedLedgerEntryJnlLine mlejl = new MixedLedgerEntryJnlLine();

                    mlejl.LineId = id;
                    mlejl.Line_No = mjl.Line_No;
                    mlejl.Editable_Entry = mjl.Editable_Entry;
                    if (mjl.Entry_No_To_Close != 0)
                    {
                        mlejl.Entry_No = mjl.Entry_No_To_Close;
                    }
                    else
                    {
                        mlejl.Entry_No = 0;
                    }
                    mlejl.Partner_Code = mjl.Partner_Code;
                    mlejl.Partner_Name = mjl.Partner_name;
                    mlejl.Partner_Label = mjl.Partner_name + " (" + mjl.Partner_Code + ")";
                    mlejl.Matter_No = mjl.Matter_No;
                    mlejl.Matter_Label = mjl.Matter_Name + " (" + mjl.Matter_No + ")";

                    if (mjl.Planning_Date != "")
                    {
                        mlejl.Planning_Date = DateTime.Parse(mjl.Planning_Date);

                    }
                    if (mjl.Matter_Line_No != 0)
                    {
                        mlejl.Matter_Line_No = mjl.Matter_Line_No;
                        mlejl.Matter_Line_Label = mjl.Description_MatterLine; //Before => mlejl.Matter_Line_Label = mjl.Description_Family
                    }
                    mlejl.Description_Family = mjl.Description_MatterLine; //Before => mlejl.Description_Family = mjl.Description_Family;
                    mlejl.Description = mjl.Description;
                    mlejl.Action_Code = mjl.Action_Code;
                    mlejl.Action_Family_Code = mjl.Action_Code;
                    mlejl.Unit_of_Measure_Code = mjl.Unit_of_Measure_Code;
                    mlejl.Sell_to_Customer_No = mjl.Sell_to_Customer_No;
                    mlejl.Sell_to_Customer_Name = mjl.Sell_to_Customer_Name;
                    mlejl.SBLClient_Label = mjl.Sell_to_Customer_Name + " (" + mjl.Sell_to_Customer_No + ")";


                    if (mjl.Quantity_Base != 0)
                    {
                        if (lineType == "time")
                        {
                            string timeString = mjl.Quantity_Base.ToString();


                            mlejl.Quantity_Base = timeConverter.ConvertAnyStringToDecimal(
                                ref timeString,
                                defaultTimeFactor,
                                timeFormat,
                                "time",
                                0,
                                "=",
                                false);
                            mlejl.TimeString = timeString;
                        }
                        else
                            mlejl.Quantity_Base = mjl.Quantity_Base;

                    }
                    if (mjl.Unit_Price_LCY != 0)
                        mlejl.Unit_Price = mjl.Unit_Price_LCY;

                    mlejl.Line_Amount = mjl.Line_Amount;//Before => mlejl.Line_Amount = mjl.Line_Amount.HasValue ? mjl.Line_Amount.Value : 0;
                    mlejl.TimeOrQuantity = false;

                    if (userMattersNos.Contains(mjl.Matter_No))
                        mlejl.IsMatterInMyMatters = true;
                    else
                        mlejl.IsMatterInMyMatters = false;

                    if (mlejl.File != null)
                    {
                        mlejl.File = mjl.file.ToString();
                    }

                    string checkFileName = "";
                    checkFileName = webService.GetPathJoinFile(mlejl.Line_No, Convert.ToInt32(mlejl.Entry_No));

                    if (checkFileName != "")
                    {
                        mlejl.NameFile = checkFileName;
                    }


                    mlejl.Kilometers = mjl.Kilometers;

                    mlejl.WriteOff = mjl.WriteOff;




                    mixedLedgerEntryJnlLines.Add(mlejl);


                }

                foreach (MatterLedgerEntry mle in matterLedgerEntries)
                {
                    id++;

                    MixedLedgerEntryJnlLine mlejl = new MixedLedgerEntryJnlLine();

                    mlejl.LineId = id;
                    mlejl.Entry_No = mle.Entry_No;
                    mlejl.Entry_Type = mle.Entry_Type;
                    mlejl.Editable_Entry = mle.Editable_Entry;
                    mlejl.Matter_Entry_Type = mle.Matter_Entry_Type;
                    mlejl.Partner_Code = mle.Partner_Code;
                    mlejl.Partner_Name = mle.Partner_name;
                    mlejl.Partner_Label = mle.Partner_name + " (" + mle.Partner_Code + ")";
                    mlejl.Matter_No = mle.Matter_No;
                    mlejl.Matter_Label = mle.Matter_Name + " (" + mle.Matter_No + ")";
                    if (mle.Planning_Date != "")
                    {
                        mlejl.Planning_Date = DateTime.Parse(mle.Planning_Date);

                    }
                    if (mle.Matter_Line_No != 0)
                    {
                        mlejl.Matter_Line_No = mle.Matter_Line_No;
                        mlejl.Matter_Line_Label = mle.Description_MatterLine;//Before => mlejl.Matter_Line_Label = mle.Description_Family;
                    }
                    mlejl.Description_Family = mle.Description_MatterLine;//Before => mlejl.Description_Family = mle.Description_Family;
                    mlejl.Description = mle.Description;
                    mlejl.Action_Code = mle.Action_Code;
                    mlejl.Action_Family_Code = mle.Action_Code;
                    mlejl.Unit_of_Measure_Code = mle.Unit_of_Measure_Code;
                    mlejl.Sell_to_Customer_No = mle.Sell_to_Customer_No;
                    mlejl.Sell_to_Customer_Name = mle.Sell_to_Customer_Name;
                    mlejl.SBLClient_Label = mle.Sell_to_Customer_Name + " (" + mle.Sell_to_Customer_No + ")";
                    if (mle.Quantity_Base != 0)
                    {
                        if (lineType == "time")
                        {

                            string timeString = mle.Quantity_Base.ToString();

                            mlejl.Quantity_Base = timeConverter.ConvertAnyStringToDecimal(
                                ref timeString,
                                defaultTimeFactor,
                                timeFormat,
                                "time",
                                0,
                                "=",
                                false);
                            mlejl.TimeString = timeString;
                        }
                        else
                            mlejl.Quantity_Base = mle.Quantity_Base;
                    }
                    if (mle.Unit_Price_LCY != 0)
                        mlejl.Unit_Price = mle.Unit_Price_LCY;

                    mlejl.Line_Amount = mle.Line_Amount;//Before => mlejl.Line_Amount = mle.Line_Amount.HasValue ? mle.Line_Amount.Value : 0;
                    mlejl.TimeOrQuantity = false;
                    if (userMattersNos.Contains(mle.Matter_No))
                        mlejl.IsMatterInMyMatters = true;
                    else
                        mlejl.IsMatterInMyMatters = false;

                    string checkFileName = "";
                    checkFileName = webService.GetPathJoinFile(mlejl.Line_No, Convert.ToInt32(mlejl.Entry_No));

                    if (checkFileName != "")
                    {
                        mlejl.NameFile = checkFileName;
                    }

                    mlejl.Kilometers = mle.Kilometers;

                    mlejl.WriteOff = mle.Write_Off;


                    mixedLedgerEntryJnlLines.Add(mlejl);



                }

                HttpContext.Current.Session["MixedLedgerEntryJnlLines"] = result_session = mixedLedgerEntryJnlLines;
                HttpContext.Current.Session["actualStartDate"] = startDate;
                HttpContext.Current.Session["actualEndDate"] = endDate;
            }

            return result_session;
        }

        /// <summary>
        /// Créé une ligne de temps dans les journal lines via le service de Navision
        /// </summary>
        /// <param name="mixedLedgerEntryJnlLine">Ligne à créer</param>
        public static void createTimeLine(MixedLedgerEntryJnlLine mixedLedgerEntryJnlLine)
        {
            //On récupère les valeurs en session
            List<MixedLedgerEntryJnlLine> mixedLedgerEntryJnlLines = (List<MixedLedgerEntryJnlLine>)HttpContext.Current.Session["MixedLedgerEntryJnlLines"];

            //On récupère la dernière ligne
            var last = mixedLedgerEntryJnlLines.LastOrDefault();

            //On incrémente le nombre de ligne
            mixedLedgerEntryJnlLine.LineId = last != null ? last.LineId + 1 : 1;

            //#warning On devra retirer le line amount à 0 le jour on ou l'utilisera
            double? lineAmount_lcy = 0;
            string timeString = mixedLedgerEntryJnlLine.TimeString == null ? "" : mixedLedgerEntryJnlLine.TimeString;
            double? Quantity_Base = mixedLedgerEntryJnlLine.Quantity_Base;

            decimal decUnitPrice = 0;

            if (mixedLedgerEntryJnlLine.WriteOff == null)
            {
                mixedLedgerEntryJnlLine.WriteOff = false;
            }

            //mixedLedgerEntryJnlLine.code_langue = last.code_langue

            //Sauvegarde via le service Navision
            //MatterFuncWebRef.MatterFunction webService = new MatterFuncWebRef.MatterFunction();
            //webService.Credentials = CredentialCache.DefaultNetworkCredentials;
            //string UsernameSession = System.Web.HttpContext.Current.Session["Username"].ToString();
            //string PasswordSession = System.Web.HttpContext.Current.Session["Password"].ToString();
            //webService.Credentials = SBLawyerWebApplication.Libs.SBLawyerCredential.GetCredentialCache(UsernameSession, PasswordSession);
            //YSA-DO Post Matter Function wuth json body
            //webService.Url = ConfigurationManager.AppSettings["NavisionMatterFunctionUrl"];
            //ServicePointManager.Expect100Continue = false;
            int line_No = webService.SaveTimeMatterJnlLine(
                mixedLedgerEntryJnlLine.Line_No,
                mixedLedgerEntryJnlLine.Matter_No,
                mixedLedgerEntryJnlLine.Matter_Line_No,
                mixedLedgerEntryJnlLine.Planning_Date.ToString("dd/MM/yyyy"),
                mixedLedgerEntryJnlLine.Description,
                mixedLedgerEntryJnlLine.Unit_of_Measure_Code,
                mixedLedgerEntryJnlLine.Action_Code,
                ref Quantity_Base,
                ref timeString,
                mixedLedgerEntryJnlLine.TimeOrQuantity,
                ref lineAmount_lcy,
                mixedLedgerEntryJnlLine.Entry_No,
                ref decUnitPrice,
                (bool)mixedLedgerEntryJnlLine.WriteOff);

            //On récupère le numéro de ligne Navision, la quantité corrigée et le temps corrigé
            mixedLedgerEntryJnlLine.Line_No = line_No;
            mixedLedgerEntryJnlLine.Line_Amount = lineAmount_lcy;
            mixedLedgerEntryJnlLine.Quantity_Base = Quantity_Base;
            mixedLedgerEntryJnlLine.TimeString = timeString;

            mixedLedgerEntryJnlLine.Unit_Price = decUnitPrice;

            //On ajoute la ligne créée dans laliste stockée dans la session
            mixedLedgerEntryJnlLines.Add(mixedLedgerEntryJnlLine);
            HttpContext.Current.Session["MixedLedgerEntryJnlLines"] = mixedLedgerEntryJnlLines;
        }

        /// <summary>
        /// Créé un frais dans les journal lines via le service de Navision
        /// </summary>
        /// <param name="mixedLedgerEntryJnlLine">Ligne de frais à créer</param>
        public static void createExpenseLine(MixedLedgerEntryJnlLine mixedLedgerEntryJnlLine)
        {
            //On récupère les valeurs en session
            List<MixedLedgerEntryJnlLine> mixedLedgerEntryJnlLines = (List<MixedLedgerEntryJnlLine>)HttpContext.Current.Session["MixedLedgerEntryJnlLines"];

            //On récupère la dernière ligne
            MixedLedgerEntryJnlLine last = mixedLedgerEntryJnlLines.LastOrDefault();

            //On incrémente le nombre de ligne
            mixedLedgerEntryJnlLine.LineId = last != null ? last.LineId + 1 : 1;

            //#warning On devra retirer le line amount  et le kilometers à 0 le jour on ou l'utilisera
            double? lineAmount_lcy = 0;
            //decimal kilometers = 0;
            decimal kilometers = mixedLedgerEntryJnlLine.Kilometers.Value;

            if (mixedLedgerEntryJnlLine.WriteOff == null)
            {
                mixedLedgerEntryJnlLine.WriteOff = false;
            }

            //Sauvegarde via le service Navision
            MatterFuncWebRef.MatterFunction webService = new MatterFuncWebRef.MatterFunction();
            //webService.Credentials = CredentialCache.DefaultNetworkCredentials;
            string UsernameSession = System.Web.HttpContext.Current.Session["Username"].ToString();
            string PasswordSession = System.Web.HttpContext.Current.Session["Password"].ToString();
            webService.Credentials = SBLawyerWebApplication.Libs.SBLawyerCredential.GetCredentialCache(UsernameSession, PasswordSession);
            webService.Url = ConfigurationManager.AppSettings["NavisionMatterFunctionUrl"];


            int line_No = webService.SaveExpenseMatterJnlLine(
                mixedLedgerEntryJnlLine.Line_No,
                mixedLedgerEntryJnlLine.Matter_No,
                mixedLedgerEntryJnlLine.Matter_Line_No,
                mixedLedgerEntryJnlLine.Planning_Date.ToString("dd/MM/yyyy"),
                mixedLedgerEntryJnlLine.Description,
                mixedLedgerEntryJnlLine.Action_Family_Code,
                mixedLedgerEntryJnlLine.Quantity_Base.Value,
                mixedLedgerEntryJnlLine.Unit_Price.Value,
                ref lineAmount_lcy,
                false,
                ref kilometers,
                mixedLedgerEntryJnlLine.Entry_No,
                (bool)mixedLedgerEntryJnlLine.WriteOff);


            //On récupère le numéro de ligne Navision
            mixedLedgerEntryJnlLine.Line_No = line_No;
            mixedLedgerEntryJnlLine.Line_Amount = lineAmount_lcy;

            //On ajoute la ligne créée dans laliste stockée dans la session
            mixedLedgerEntryJnlLines.Add(mixedLedgerEntryJnlLine);
            HttpContext.Current.Session["MixedLedgerEntryJnlLines"] = mixedLedgerEntryJnlLines;
        }

        /// <summary>
        /// Sauvegarde Piéce jointe via le service Navision
        /// </summary>
        /// <param name="lineNo">Numero Ligne feuille de frais</param>
        /// <param name="EntryNo">Numero ecriture compta</param>
        /// <param name="Filename">Nom fichier</param>
        /// <param name="lineNo">Chemin Fichier</param>
        public static void saveFile(int lineNo, int EntryNo, string Filename, string Path)
        {

            //Appel WS Navision
            MatterFuncWebRef.MatterFunction webService = new MatterFuncWebRef.MatterFunction();
            //webService.Credentials = CredentialCache.DefaultNetworkCredentials;
            string UsernameSession = System.Web.HttpContext.Current.Session["Username"].ToString();
            string PasswordSession = System.Web.HttpContext.Current.Session["Password"].ToString();
            webService.Credentials = SBLawyerWebApplication.Libs.SBLawyerCredential.GetCredentialCache(UsernameSession, PasswordSession);
            webService.Url = ConfigurationManager.AppSettings["NavisionMatterFunctionUrl"];

            //Sauvegarde du fichier dans Navision 
            int NewlineNo = webService.SavePathJoinFile(lineNo, EntryNo, Filename, Path);
        }

        /// <summary>
        /// Sauvegarde un temps via le service Navision
        /// </summary>
        /// <param name="mixedLedgerEntryJnlLine">Ligne de temps à sauvegarder</param>
        public static void updateTimeLine(MixedLedgerEntryJnlLine mixedLedgerEntryJnlLine)
        {
            //On récupère les valeurs en session
            List<MixedLedgerEntryJnlLine> mixedLedgerEntryJnlLines = (List<MixedLedgerEntryJnlLine>)HttpContext.Current.Session["MixedLedgerEntryJnlLines"];

            //On stocke le line aomunt pour pouvoir l'utiliser en référence
            double? lineAmount_lcy = mixedLedgerEntryJnlLine.Line_Amount.HasValue ? mixedLedgerEntryJnlLine.Line_Amount.Value : 0;
            double? Quantity_Base = mixedLedgerEntryJnlLine.Quantity_Base.Value;
            string timeString = mixedLedgerEntryJnlLine.TimeString == null ? "" : mixedLedgerEntryJnlLine.TimeString;

            decimal decUnitPrice = 0;

            //Sauvegarde via le service Navision
            MatterFuncWebRef.MatterFunction webService = new MatterFuncWebRef.MatterFunction();
            //webService.Credentials = CredentialCache.DefaultNetworkCredentials;
            string UsernameSession = System.Web.HttpContext.Current.Session["Username"].ToString();
            string PasswordSession = System.Web.HttpContext.Current.Session["Password"].ToString();
            webService.Credentials = SBLawyerWebApplication.Libs.SBLawyerCredential.GetCredentialCache(UsernameSession, PasswordSession);
            webService.Url = ConfigurationManager.AppSettings["NavisionMatterFunctionUrl"];
            //ServicePointManager.Expect100Continue = false;
            int line_No = webService.SaveTimeMatterJnlLine(
                mixedLedgerEntryJnlLine.Line_No,
                mixedLedgerEntryJnlLine.Matter_No,
                mixedLedgerEntryJnlLine.Matter_Line_No,
                mixedLedgerEntryJnlLine.Planning_Date.ToString("dd/MM/yyyy"),
                mixedLedgerEntryJnlLine.Description,
                mixedLedgerEntryJnlLine.Unit_of_Measure_Code,
                mixedLedgerEntryJnlLine.Action_Code,
                ref Quantity_Base,
                ref timeString,
                mixedLedgerEntryJnlLine.TimeOrQuantity,
                ref lineAmount_lcy,
                mixedLedgerEntryJnlLine.Entry_No,
                ref decUnitPrice,
                (bool)mixedLedgerEntryJnlLine.WriteOff);

            //On récupère le numéro de ligne Navision et le lineAmount mis à jour
            mixedLedgerEntryJnlLine.Line_No = line_No;
            mixedLedgerEntryJnlLine.Line_Amount = lineAmount_lcy;
            mixedLedgerEntryJnlLine.Quantity_Base = Quantity_Base;
            mixedLedgerEntryJnlLine.TimeString = timeString;

            mixedLedgerEntryJnlLine.Unit_Price = decUnitPrice;

            //On retire l'ancienne ligne de la liste stockée dans la session
            mixedLedgerEntryJnlLines.Remove(mixedLedgerEntryJnlLines.Where(mlejl => mlejl.LineId == mixedLedgerEntryJnlLine.LineId).First());

            //On ajoute la ligne créée dans la liste stockée dans la session
            mixedLedgerEntryJnlLines.Add(mixedLedgerEntryJnlLine);
            HttpContext.Current.Session["MixedLedgerEntryJnlLines"] = mixedLedgerEntryJnlLines;
        }

        /// <summary>
        /// Sauvegarde un temps via le service Navision
        /// </summary>
        /// <param name="mixedLedgerEntryJnlLine">Ligne de temps à sauvegarder</param>
        public static void updateExpenseLine(MixedLedgerEntryJnlLine mixedLedgerEntryJnlLine)
        {
            //On récupère les valeurs en session
            List<MixedLedgerEntryJnlLine> mixedLedgerEntryJnlLines = (List<MixedLedgerEntryJnlLine>)HttpContext.Current.Session["MixedLedgerEntryJnlLines"];

            //On stocke le line amount et les kilometers pour pouvoir les utiliser en référence
            double? lineAmount_lcy = mixedLedgerEntryJnlLine.Line_Amount.HasValue ? mixedLedgerEntryJnlLine.Line_Amount.Value : 0;
            decimal kilometers = mixedLedgerEntryJnlLine.Kilometers.HasValue ? mixedLedgerEntryJnlLine.Kilometers.Value : 0;

            //Sauvegarde via le service Navision
            MatterFuncWebRef.MatterFunction webService = new MatterFuncWebRef.MatterFunction();
            //webService.Credentials = CredentialCache.DefaultNetworkCredentials;
            string UsernameSession = System.Web.HttpContext.Current.Session["Username"].ToString();
            string PasswordSession = System.Web.HttpContext.Current.Session["Password"].ToString();
            webService.Credentials = SBLawyerWebApplication.Libs.SBLawyerCredential.GetCredentialCache(UsernameSession, PasswordSession);
            webService.Url = ConfigurationManager.AppSettings["NavisionMatterFunctionUrl"];

            int line_No = webService.SaveExpenseMatterJnlLine(
                                    mixedLedgerEntryJnlLine.Line_No,
                                    mixedLedgerEntryJnlLine.Matter_No,
                                    mixedLedgerEntryJnlLine.Matter_Line_No,
                                    mixedLedgerEntryJnlLine.Planning_Date.ToString("dd/MM/yyyy"),
                                    mixedLedgerEntryJnlLine.Description,
                                    mixedLedgerEntryJnlLine.Action_Family_Code,
                                    mixedLedgerEntryJnlLine.Quantity_Base.Value,
                                    mixedLedgerEntryJnlLine.Unit_Price.Value,
                                    ref lineAmount_lcy,
                                    false,
                                    ref kilometers,
                                    mixedLedgerEntryJnlLine.Entry_No,
                                    (bool)mixedLedgerEntryJnlLine.WriteOff);


            //On récupère le numéro de ligne Navision, le line Amount et les kilometers mis à jour
            mixedLedgerEntryJnlLine.Line_No = line_No;
            mixedLedgerEntryJnlLine.Line_Amount = lineAmount_lcy;
            mixedLedgerEntryJnlLine.Kilometers = kilometers;

            //On retire l'ancienne ligne de la liste stockée dans la session
            mixedLedgerEntryJnlLines.Remove(mixedLedgerEntryJnlLines.Where(mlejl => mlejl.LineId == mixedLedgerEntryJnlLine.LineId).First());
            //On ajoute la ligne créée dans la liste stockée dans la session
            mixedLedgerEntryJnlLines.Add(mixedLedgerEntryJnlLine);
            HttpContext.Current.Session["MixedLedgerEntryJnlLines"] = mixedLedgerEntryJnlLines;
        }

        /// <summary>
        /// Permet de supprimer un temps de Navision via la fonction de sauvegarde en passant la quantité à 0 
        /// </summary>
        /// <param name="mixedLedgerEntryJnlLine">Ligne de temps à supprimer</param>
        public static void deleteTimeLine(MixedLedgerEntryJnlLine mixedLedgerEntryJnlLine)
        {
            //On récupère les valeurs en session
            List<MixedLedgerEntryJnlLine> mixedLedgerEntryJnlLines = (List<MixedLedgerEntryJnlLine>)HttpContext.Current.Session["MixedLedgerEntryJnlLines"];

            //Pour notifier la supression on doit avoir Line_No !=0 ou Entry_No !=0 et Quantity_Base == 0
            mixedLedgerEntryJnlLine.Quantity_Base = 0;

            //On stocke le line aount pour pouvoir l'utiliser en référence
            double? lineAmount_lcy = mixedLedgerEntryJnlLine.Line_Amount.HasValue ? mixedLedgerEntryJnlLine.Line_Amount.Value : 0;
            double? Quantity_Base = mixedLedgerEntryJnlLine.Quantity_Base.Value;
            string timeString = mixedLedgerEntryJnlLine.TimeString;
            decimal decUnitPrice = 0;

            //Suppression (Sauvegarde) via le service Navision
            MatterFuncWebRef.MatterFunction webService = new MatterFuncWebRef.MatterFunction();
            //webService.Credentials = CredentialCache.DefaultNetworkCredentials;
            string UsernameSession = System.Web.HttpContext.Current.Session["Username"].ToString();
            string PasswordSession = System.Web.HttpContext.Current.Session["Password"].ToString();
            webService.Credentials = SBLawyerWebApplication.Libs.SBLawyerCredential.GetCredentialCache(UsernameSession, PasswordSession);
            webService.Url = ConfigurationManager.AppSettings["NavisionMatterFunctionUrl"];
            //ServicePointManager.Expect100Continue = false;
            int line_No = webService.SaveTimeMatterJnlLine(
                mixedLedgerEntryJnlLine.Line_No,
                mixedLedgerEntryJnlLine.Matter_No,
                mixedLedgerEntryJnlLine.Matter_Line_No,
                mixedLedgerEntryJnlLine.Planning_Date.ToString("dd/MM/yyyy"),
                mixedLedgerEntryJnlLine.Description,
                mixedLedgerEntryJnlLine.Unit_of_Measure_Code,
                mixedLedgerEntryJnlLine.Action_Code,
                ref Quantity_Base,
                ref timeString,
                mixedLedgerEntryJnlLine.TimeOrQuantity,
                ref lineAmount_lcy,
                mixedLedgerEntryJnlLine.Entry_No,
                ref decUnitPrice,
                (bool)mixedLedgerEntryJnlLine.WriteOff);

            mixedLedgerEntryJnlLine.Line_No = line_No;
            mixedLedgerEntryJnlLine.TimeString = timeString;
            mixedLedgerEntryJnlLine.Unit_Price = decUnitPrice;

            //On retire l'ancienne ligne de la liste stockée dans la session
            mixedLedgerEntryJnlLines.Remove(mixedLedgerEntryJnlLines.Where(mlejl => mlejl.LineId == mixedLedgerEntryJnlLine.LineId).First());
            HttpContext.Current.Session["MixedLedgerEntryJnlLines"] = mixedLedgerEntryJnlLines;
        }
        /// <summary>
        /// Permet de supprimer un frais de Navision via la fonction de sauvegarde en passant la quantité à 0 
        /// </summary>
        /// <param name="mixedLedgerEntryJnlLine">Ligne de temps à supprimer</param>
        public static void deleteExpenseLine(MixedLedgerEntryJnlLine mixedLedgerEntryJnlLine)
        {
            //On récupère les valeurs en session
            List<MixedLedgerEntryJnlLine> mixedLedgerEntryJnlLines = (List<MixedLedgerEntryJnlLine>)HttpContext.Current.Session["MixedLedgerEntryJnlLines"];

            //Pour notifier la supression on doit avoir Line_No !=0 ou Entry_No !=0 et Quantity_Base == 0
            mixedLedgerEntryJnlLine.Quantity_Base = 0;

            //On stocke le line amount et les kilometers pour pouvoir les utiliser en référence
            double? lineAmount_lcy = mixedLedgerEntryJnlLine.Line_Amount.HasValue ? mixedLedgerEntryJnlLine.Line_Amount.Value : 0;
            decimal kilometers = mixedLedgerEntryJnlLine.Kilometers.HasValue ? mixedLedgerEntryJnlLine.Kilometers.Value : 0;

            //Suppression (Sauvegarde) via le service Navision
            MatterFuncWebRef.MatterFunction webService = new MatterFuncWebRef.MatterFunction();
            //webService.Credentials = CredentialCache.DefaultNetworkCredentials;
            string UsernameSession = System.Web.HttpContext.Current.Session["Username"].ToString();
            string PasswordSession = System.Web.HttpContext.Current.Session["Password"].ToString();
            webService.Credentials = SBLawyerWebApplication.Libs.SBLawyerCredential.GetCredentialCache(UsernameSession, PasswordSession);
            webService.Url = ConfigurationManager.AppSettings["NavisionMatterFunctionUrl"];


            int line_No = webService.SaveExpenseMatterJnlLine(
                            mixedLedgerEntryJnlLine.Line_No,
                            mixedLedgerEntryJnlLine.Matter_No,
                            mixedLedgerEntryJnlLine.Matter_Line_No,
                            mixedLedgerEntryJnlLine.Planning_Date.ToString("dd/MM/yyyy"),
                            mixedLedgerEntryJnlLine.Description,
                            mixedLedgerEntryJnlLine.Action_Family_Code,
                            mixedLedgerEntryJnlLine.Quantity_Base.Value,
                            mixedLedgerEntryJnlLine.Unit_Price.Value,
                            ref lineAmount_lcy,
                            false,
                            ref kilometers,
                            mixedLedgerEntryJnlLine.Entry_No,
                            (bool)mixedLedgerEntryJnlLine.WriteOff);


            //On retire l'ancienne ligne de la liste stockée dans la session
            mixedLedgerEntryJnlLines.Remove(mixedLedgerEntryJnlLines.Where(mlejl => mlejl.LineId == mixedLedgerEntryJnlLine.LineId).First());
            HttpContext.Current.Session["MixedLedgerEntryJnlLines"] = mixedLedgerEntryJnlLines;
        }

        /// <summary>
        /// Lance la validation dans Navision des journal line de temps pour les transformer en entry
        /// </summary>
        public static void ValidateTimeJournalLines()
        {
            //Validation des lignes via le service Navision
            PostTimeMatterJnlLineWebRef.MatterPostLine webService = new PostTimeMatterJnlLineWebRef.MatterPostLine();
            //webService.Credentials = CredentialCache.DefaultNetworkCredentials;
            string UsernameSession = System.Web.HttpContext.Current.Session["Username"].ToString();
            string PasswordSession = System.Web.HttpContext.Current.Session["Password"].ToString();
            webService.Credentials = SBLawyerWebApplication.Libs.SBLawyerCredential.GetCredentialCache(UsernameSession, PasswordSession);
            webService.Url = ConfigurationManager.AppSettings["NavisionPostLineUrl"];

            webService.PostTimeMatterJnlLine();

            //On change les dates pour que le read rafraichisse les lignes
            HttpContext.Current.Session["actualStartDate"] = new DateTime();
            HttpContext.Current.Session["actualEndDate"] = new DateTime();
        }

        /// <summary>
        /// Lance la validation dans Navision des journal line  de fraispour les transformer en entry
        /// </summary>
        public static void ValidateExpenseJournalLines()
        {
            //Validation des lignes via le service Navision
            PostTimeMatterJnlLineWebRef.MatterPostLine webService = new PostTimeMatterJnlLineWebRef.MatterPostLine();
            //webService.Credentials = CredentialCache.DefaultNetworkCredentials;
            string UsernameSession = System.Web.HttpContext.Current.Session["Username"].ToString();
            string PasswordSession = System.Web.HttpContext.Current.Session["Password"].ToString();
            webService.Credentials = SBLawyerWebApplication.Libs.SBLawyerCredential.GetCredentialCache(UsernameSession, PasswordSession);
            webService.Url = ConfigurationManager.AppSettings["NavisionPostLineUrl"];

            webService.PostExpenseMatterJnlLine();

            //On change les dates pour que le read rafraichisse les lignes
            HttpContext.Current.Session["actualStartDate"] = new DateTime();
            HttpContext.Current.Session["actualEndDate"] = new DateTime();
        }
        /// <summary>
        /// Permet de vérifier que la date  d'une ligne est bien dans la période de saisie autorisée
        /// </summary>
        /// <param name="Planning_Date"></param>
        public static void CheckDate(DateTime Planning_Date)
        {
            //Vérification des dates via le service Navision
            MatterFuncWebRef.MatterFunction webService = new MatterFuncWebRef.MatterFunction();
            //webService.Credentials = CredentialCache.DefaultNetworkCredentials;
            string UsernameSession = System.Web.HttpContext.Current.Session["Username"].ToString();
            string PasswordSession = System.Web.HttpContext.Current.Session["Password"].ToString();
            webService.Credentials = SBLawyerWebApplication.Libs.SBLawyerCredential.GetCredentialCache(UsernameSession, PasswordSession);
            webService.Url = ConfigurationManager.AppSettings["NavisionMatterFunctionUrl"];
            //ServicePointManager.Expect100Continue = false;
            webService.TimeNotAllowed(Planning_Date);
            //webService.ExpenseNotAllowed(Planning_Date);
        }

        public static void CheckDateExpense(DateTime Planning_Date)
        {
            //Vérification des dates via le service Navision
            MatterFuncWebRef.MatterFunction webService = new MatterFuncWebRef.MatterFunction();
            //webService.Credentials = CredentialCache.DefaultNetworkCredentials;
            string UsernameSession = System.Web.HttpContext.Current.Session["Username"].ToString();
            string PasswordSession = System.Web.HttpContext.Current.Session["Password"].ToString();
            webService.Credentials = SBLawyerWebApplication.Libs.SBLawyerCredential.GetCredentialCache(UsernameSession, PasswordSession);
            webService.Url = ConfigurationManager.AppSettings["NavisionMatterFunctionUrl"];
            //ServicePointManager.Expect100Continue = false;

            webService.ExpenseNotAllowed(Planning_Date);
        }

        /// <summary>
        /// Permet de vérifier une quantité et d'en retourner un temps en fonction d'un type d'unité ou d'une affaire
        /// </summary>
        /// <param name="Quantity">Quantité à vérifier</param>
        /// <param name="Matter_No">Affaire dont on récupère l'unité de mesure</param>
        /// <returns>Couple de quantité et de temps</returns>
        public static QuantityTime CheckQuantityTime(decimal Quantity, string Matter_No, string user)
        {
            MatterFuncWebRef.MatterFunction webService = new MatterFuncWebRef.MatterFunction();
            //webService.Credentials = CredentialCache.DefaultNetworkCredentials;
            string UsernameSession = System.Web.HttpContext.Current.Session["Username"].ToString();
            string PasswordSession = System.Web.HttpContext.Current.Session["Password"].ToString();
            webService.Credentials = SBLawyerWebApplication.Libs.SBLawyerCredential.GetCredentialCache(UsernameSession, PasswordSession);
            webService.Url = ConfigurationManager.AppSettings["NavisionMatterFunctionUrl"];
            //ServicePointManager.Expect100Continue = false;

            DateTime checkedTime = Convert.ToDateTime(webService.CalcTimeSubtractFactor(ref Quantity, ResourceDataContext.GetBaseUnitOfMeasure(user, false), Matter_No));

            return new QuantityTime() { Time = checkedTime, Quantity_Base = Quantity };
        }

        /// <summary>
        /// Permet de vérifier un temps et d'en retourner une quantité en fonction d'un type d'unité ou d'une affaire
        /// </summary>
        /// <param name="Quantity">Quantité à vérifier</param>
        /// <param name="Matter_No">Affaire dont on récupère l'unité de mesure</param>
        /// <returns>Couple de quantité et de temps</returns>
        public static QuantityTime CheckQuantityTime(DateTime timeToCheck, string Matter_No, string user)
        {
            string timeString = timeToCheck.Hour.ToString("00") + ":" + timeToCheck.Minute.ToString("00");

            MatterFuncWebRef.MatterFunction webService = new MatterFuncWebRef.MatterFunction();
            //webService.Credentials = CredentialCache.DefaultNetworkCredentials;
            string UsernameSession = System.Web.HttpContext.Current.Session["Username"].ToString();
            string PasswordSession = System.Web.HttpContext.Current.Session["Password"].ToString();
            webService.Credentials = SBLawyerWebApplication.Libs.SBLawyerCredential.GetCredentialCache(UsernameSession, PasswordSession);
            webService.Url = ConfigurationManager.AppSettings["NavisionMatterFunctionUrl"];
            //ServicePointManager.Expect100Continue = false;

            decimal checkedQuantity = webService.CalcQtyTimeDeltaFactor(ref timeString, ResourceDataContext.GetBaseUnitOfMeasure(user, false), Matter_No);

            return new QuantityTime() { Time = timeToCheck.AddYears(2000), Quantity_Base = checkedQuantity };
        }

        /// <summary>
        /// Permet de récupérer le montant des IK
        /// </summary>
        /// <param name="IdUser">Id user</param>
        /// <returns>decimal montant IK</returns>
        public static decimal getFiscalRate(string IdUser)
        {

            MatterFuncWebRef.MatterFunction webService = new MatterFuncWebRef.MatterFunction();
            //webService.Credentials = CredentialCache.DefaultNetworkCredentials;
            string UsernameSession = System.Web.HttpContext.Current.Session["Username"].ToString();
            string PasswordSession = System.Web.HttpContext.Current.Session["Password"].ToString();
            webService.Credentials = SBLawyerWebApplication.Libs.SBLawyerCredential.GetCredentialCache(UsernameSession, PasswordSession);
            webService.Url = ConfigurationManager.AppSettings["NavisionMatterFunctionUrl"];
            //ServicePointManager.Expect100Continue = false;

            decimal getIkRate = webService.GetFiscalRate(IdUser);

            return getIkRate;
        }
        public struct QuantityTime
        {
            public DateTime Time { get; set; }
            public decimal Quantity_Base { get; set; }
        }
    }

}