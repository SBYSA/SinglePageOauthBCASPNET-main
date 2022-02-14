using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace AppModelv2_WebApp_OpenIDConnect_DotNet.Models
{
    public class UserParameters
    {
        public string User_ID { get; set; }
        public bool Time_Hide_Associate { get; set; }
        public bool Time_Hide_Quantity { get; set; }
        public bool Time_Hide_Line_Amount { get; set; }
        public bool Time_Hide_Post_Conf_Message { get; set; }
        public bool Time_Hide_Unit_of_Measure { get; set; }
        public bool Time_Hide_Matter_Line_Column { get; set; }
        public bool Time_Hide_Unit_Price { get; set; }
        public bool Time_Hide_Action_Code { get; set; }
        public bool Time_Hide_Write_Off { get; set; }
        public bool Can_Export_Excel { get; set; }
        public bool Exp_Hide_Associate { get; set; }
        public bool Exp_Hide_Quantity { get; set; }
        public bool Exp_Hide_Line_Amount { get; set; }
        public bool Exp_Hide_Post_Conf_Message { get; set; }
        public bool Exp_Hide_Unit_of_Measure { get; set; }
        public bool Exp_Hide_Matter_Line_Column { get; set; }
        public bool Exp_Hide_Column_Kilometer { get; set; }
        public bool Exp_Hide_Write_off { get; set; }
        public bool Can_Export_Excel_Expense { get; set; }
        public bool Can_Upload_Attached_File { get; set; }
        public string Path_Folder_Uploaded_Files { get; set; }
        public string Color_Serie_1 { get; set; }
        public string Color_Serie_2 { get; set; }
        public string Color_Serie_3 { get; set; }
        public string Color_Serie_4 { get; set; }
        public string Color_Serie_5 { get; set; }
        public string Color_Serie_6 { get; set; }
        public string Sel_Line_Background_Color { get; set; }
        public string Sel_Line_Foreground_Color { get; set; }
        public string Jnl_Background_Color { get; set; }
        public string Jnl_Foreground_Color { get; set; }
        public string Mle_Background_Color { get; set; }
        public string Mle_Foreground_Color { get; set; }
        public string Callback_Background_Color { get; set; }
        public string Callback_Foreground_Color { get; set; }
        public string Not_Editable_Background_Color { get; set; }
        public string Not_Editable_Foreground_Color { get; set; }
        public bool Can_Read_Web_Portal_Setup { get; set; }
        public bool Can_Read_TimeSheet { get; set; }
        public bool Can_Read_ExpenseSheet { get; set; }
        public bool Can_Read_TimeSheet_Synt { get; set; }
        public bool Can_Read_Total_Hours_Chart { get; set; }
        public bool Can_Read_Hours_Distribution { get; set; }
        public bool Can_Read_Gauge_Chart { get; set; }
        public bool Can_Read_Gauge_Chart_Hour { get; set; }
        public bool Can_Add_Row { get; set; }
        public bool Can_Filter_By_Date { get; set; }
        public bool Timer_Validation { get; set; }
    }

    public class UserParametersRoot
    {
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }
        public List<UserParameters> Value { get; set; }
    }
    public static class UserParametersDataContext
    {
        /// <summary>
        /// Pe
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public static UserParameters GetUserParameters(bool overwrite)
        {
            UserParameters parameters = new UserParameters();
            string UsernameSession = System.Web.HttpContext.Current.Session["Username"].ToString();
            // To reactivate string PasswordSession = System.Web.HttpContext.Current.Session["Password"].ToString();


            if (parameters == null || overwrite)
            {
                string userID = System.Web.HttpContext.Current.Session["Username"].ToString().ToUpper();
                parameters = new UserParameters();

                //Construction du lien API
                string BaseURL = ConfigurationManager.AppSettings["BaseURL"];
                string WorkTypeAPI = ConfigurationManager.AppSettings["webportalsetups"];
                string BCCompany = ConfigurationManager.AppSettings["BCCompany"];

                //Récupération du Token si existe
                string IdToken = System.Web.HttpContext.Current.Session["IdToken_Ticket"].ToString();
                var httpClient = new System.Net.Http.HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", IdToken);

                string environmentsUri = BaseURL + WorkTypeAPI + BCCompany;

                var response = httpClient.GetAsync(environmentsUri).Result;
                var content = response.Content.ReadAsStringAsync().Result;
                //On récupère le schéma de réponse
                var result = JsonConvert.DeserializeObject<UserParametersRoot>(content);
                if (result.Value.Count() > 1)
                {
                    //Récupération du Token si existe
                    IdToken = System.Web.HttpContext.Current.Session["IdToken_Ticket"].ToString();
                    httpClient = new System.Net.Http.HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", IdToken);

                    string filters = "User_ID eq '" + userID + "'";

                    environmentsUri = BaseURL + WorkTypeAPI + BCCompany + filters;

                    response = httpClient.GetAsync(environmentsUri).Result;
                    content = response.Content.ReadAsStringAsync().Result;
                    //On récupère le schéma de réponse
                    result = JsonConvert.DeserializeObject<UserParametersRoot>(content);
                }
                foreach (UserParameters userP in result.Value)
                {
                    parameters.User_ID = userP.User_ID;

                    parameters.Sel_Line_Background_Color = userP.Sel_Line_Background_Color;
                    parameters.Sel_Line_Foreground_Color = userP.Sel_Line_Foreground_Color;

                    parameters.Not_Editable_Background_Color = userP.Not_Editable_Background_Color;
                    parameters.Not_Editable_Foreground_Color = userP.Not_Editable_Foreground_Color;

                    parameters.Jnl_Background_Color = userP.Jnl_Background_Color;
                    parameters.Jnl_Foreground_Color = userP.Jnl_Foreground_Color;

                    parameters.Mle_Background_Color = userP.Mle_Background_Color;
                    parameters.Mle_Foreground_Color = userP.Mle_Foreground_Color;

                    parameters.Callback_Background_Color = userP.Callback_Background_Color;
                    parameters.Callback_Foreground_Color = userP.Callback_Foreground_Color;

                    parameters.Color_Serie_1 = userP.Color_Serie_1;
                    parameters.Color_Serie_2 = userP.Color_Serie_2;
                    parameters.Color_Serie_3 = userP.Color_Serie_3;
                    parameters.Color_Serie_4 = userP.Color_Serie_4;
                    parameters.Color_Serie_5 = userP.Color_Serie_5;
                    parameters.Color_Serie_6 = userP.Color_Serie_6;

                    parameters.Exp_Hide_Line_Amount = userP.Exp_Hide_Line_Amount;
                    parameters.Exp_Hide_Post_Conf_Message = userP.Exp_Hide_Post_Conf_Message;
                    parameters.Exp_Hide_Quantity = userP.Exp_Hide_Quantity;
                    parameters.Exp_Hide_Unit_of_Measure = userP.Exp_Hide_Unit_of_Measure;
                    parameters.Exp_Hide_Matter_Line_Column = userP.Exp_Hide_Matter_Line_Column;
                    parameters.Exp_Hide_Associate = userP.Exp_Hide_Associate;

                    parameters.Time_Hide_Line_Amount = userP.Time_Hide_Line_Amount;
                    parameters.Time_Hide_Post_Conf_Message = userP.Time_Hide_Post_Conf_Message;
                    parameters.Time_Hide_Quantity = userP.Time_Hide_Quantity;
                    parameters.Time_Hide_Unit_of_Measure = userP.Time_Hide_Unit_of_Measure;
                    parameters.Time_Hide_Matter_Line_Column = userP.Time_Hide_Matter_Line_Column;
                    parameters.Time_Hide_Associate = userP.Time_Hide_Associate;
                    parameters.Time_Hide_Unit_Price = userP.Time_Hide_Unit_Price;

                    parameters.Can_Read_ExpenseSheet = userP.Can_Read_ExpenseSheet;
                    parameters.Can_Read_TimeSheet = userP.Can_Read_TimeSheet;
                    parameters.Can_Read_Web_Portal_Setup = userP.Can_Read_Web_Portal_Setup;

                    parameters.Can_Read_Gauge_Chart_Hour = userP.Can_Read_Gauge_Chart;
                    parameters.Can_Read_Gauge_Chart_Hour = userP.Can_Read_Gauge_Chart_Hour; //YSA Gauge Chart Hour
                    parameters.Can_Read_Hours_Distribution = userP.Can_Read_Hours_Distribution;
                    parameters.Can_Read_Total_Hours_Chart = userP.Can_Read_Total_Hours_Chart;
                    parameters.Can_Read_TimeSheet_Synt = userP.Can_Read_TimeSheet_Synt;
                    parameters.Can_Export_Excel = userP.Can_Export_Excel;
                    parameters.Can_Export_Excel_Expense = userP.Can_Export_Excel_Expense;
                    parameters.Can_Upload_Attached_File = userP.Can_Upload_Attached_File;
                    parameters.Path_Folder_Uploaded_Files = userP.Path_Folder_Uploaded_Files;

                    parameters.Time_Hide_Action_Code = userP.Time_Hide_Action_Code;
                    parameters.Timer_Validation = userP.Timer_Validation;
                    parameters.Can_Filter_By_Date = userP.Can_Filter_By_Date;
                    parameters.Can_Add_Row = userP.Can_Add_Row;
                    parameters.Exp_Hide_Column_Kilometer = userP.Exp_Hide_Column_Kilometer;
                    parameters.Time_Hide_Write_Off = userP.Time_Hide_Write_Off;
                    parameters.Exp_Hide_Write_off = userP.Exp_Hide_Write_off;
                }

                

                MatterFuncWebRef.MatterFunction webService = new MatterFuncWebRef.MatterFunction();
                //webService.Credentials = CredentialCache.DefaultNetworkCredentials;
                //to reactivate webService.Credentials = SBLawyerWebApplication.Libs.SBLawyerCredential.GetCredentialCache(UsernameSession, PasswordSession);

                webService.Url = ConfigurationManager.AppSettings["NavisionMatterFunctionUrl"];

                parameters.NavisionUserName = webService.GetNameFromUserID();
                parameters.CanEditWebPortalSetup = webService.GetWebPortalSetupPermission();
                parameters.TimeFormat = webService.GetFormatMatterSetup();


            }
            
            return parameters;
        }
        public static UserParameters createUserParameters()
        {
            return new UserParameters();
        }
        public static UserParameters createUserParametersWithColors(
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
            //On récupère les types actions de temps via le service Navision
            Uri uri = new Uri(ConfigurationManager.AppSettings["NavisionODataUri"]);
            var NAV = new NavServiceRef.NAV(uri);

            string UsernameSession = System.Web.HttpContext.Current.Session["Username"].ToString();
            string PasswordSession = System.Web.HttpContext.Current.Session["Password"].ToString();

            //NAV.Credentials = CredentialCache.DefaultNetworkCredentials;
            NAV.Credentials = SBLawyerWebApplication.Libs.SBLawyerCredential.GetCredentialCache(UsernameSession, PasswordSession);

            //Création du paramètrage utilisateur
            string userID = System.Web.HttpContext.Current.Session["Username"].ToString().ToUpper();
            NavServiceRef.WebPortalSetup navUserParameters = NavServiceRef.WebPortalSetup.CreateWebPortalSetup(userID);

            //Mise a jour des données modifiées
            navUserParameters.Sel_Line_Background_Color = SelectedLineBackgroundColor;
            navUserParameters.Sel_Line_Foreground_Color = SelectedLineForegroundColor;

            navUserParameters.Not_Editable_Background_Color = NotEditableBackgroundColor;
            navUserParameters.Not_Editable_Foreground_Color = NotEditableForegroundColor;

            navUserParameters.Jnl_Background_Color = JnlBackgroundColor;
            navUserParameters.Jnl_Foreground_Color = JnlForegroundColor;

            navUserParameters.Mle_Background_Color = MleBackgroundColor;
            navUserParameters.Mle_Foreground_Color = MleForegroundColor;

            navUserParameters.Callback_Background_Color = CallBackBackgroundColor;
            navUserParameters.Callback_Foreground_Color = CallBackForegroundColor;

            navUserParameters.Color_Serie_1 = SerieColor1;
            navUserParameters.Color_Serie_2 = SerieColor2;
            navUserParameters.Color_Serie_3 = SerieColor3;
            navUserParameters.Color_Serie_4 = SerieColor4;
            navUserParameters.Color_Serie_5 = SerieColor5;
            navUserParameters.Color_Serie_6 = SerieColor6;


            NAV.AddToWebPortalSetup(navUserParameters);
            NAV.SaveChanges(SaveChangesOptions.PatchOnUpdate);


            return new UserParameters()
            {
                User_ID = navUserParameters.User_ID,

                Sel_Line_Background_Color = navUserParameters.Sel_Line_Background_Color,
                Sel_Line_Foreground_Color = navUserParameters.Sel_Line_Foreground_Color,

                Not_Editable_Background_Color = navUserParameters.Not_Editable_Background_Color,
                Not_Editable_Foreground_Color = navUserParameters.Not_Editable_Foreground_Color,

                Jnl_Background_Color = navUserParameters.Jnl_Background_Color,
                Jnl_Foreground_Color = navUserParameters.Jnl_Foreground_Color,

                Mle_Background_Color = navUserParameters.Mle_Background_Color,
                Mle_Foreground_Color = navUserParameters.Mle_Foreground_Color,

                Callback_Background_Color = navUserParameters.Callback_Background_Color,
                Callback_Foreground_Color = navUserParameters.Callback_Foreground_Color,

                Color_Serie_1 = navUserParameters.Color_Serie_1,
                Color_Serie_2 = navUserParameters.Color_Serie_2,
                Color_Serie_3 = navUserParameters.Color_Serie_3,
                Color_Serie_4 = navUserParameters.Color_Serie_4,
                Color_Serie_5 = navUserParameters.Color_Serie_5,
                Color_Serie_6 = navUserParameters.Color_Serie_6,

                Exp_Hide_Line_Amount = navUserParameters.Exp_Hide_Line_Amount.HasValue ? navUserParameters.Exp_Hide_Line_Amount.Value : false,
                Exp_Hide_Post_Conf_Message = navUserParameters.Exp_Hide_Post_Conf_Message.HasValue ? navUserParameters.Exp_Hide_Post_Conf_Message.Value : false,
                Exp_Hide_Quantity = navUserParameters.Exp_Hide_Quantity.HasValue ? navUserParameters.Exp_Hide_Quantity.Value : false,
                Exp_Hide_Unit_of_Measure = navUserParameters.Exp_Hide_Unit_of_Measure.HasValue ? navUserParameters.Exp_Hide_Unit_of_Measure.Value : false,
                Exp_Hide_Matter_Line_Column = navUserParameters.Exp_Hide_Matter_Line_Column.HasValue ? navUserParameters.Exp_Hide_Matter_Line_Column.Value : false,
                Exp_Hide_Associate = navUserParameters.Exp_Hide_Associate.HasValue ? navUserParameters.Exp_Hide_Associate.Value : false,

                Time_Hide_Line_Amount = navUserParameters.Time_Hide_Line_Amount.HasValue ? navUserParameters.Time_Hide_Line_Amount.Value : false,
                Time_Hide_Post_Conf_Message = navUserParameters.Time_Hide_Post_Conf_Message.HasValue ? navUserParameters.Time_Hide_Post_Conf_Message.Value : false,
                Time_Hide_Quantity = navUserParameters.Time_Hide_Quantity.HasValue ? navUserParameters.Time_Hide_Quantity.Value : false,
                Time_Hide_Unit_of_Measure = navUserParameters.Time_Hide_Unit_of_Measure.HasValue ? navUserParameters.Time_Hide_Unit_of_Measure.Value : false,
                Time_Hide_Matter_Line_Column = navUserParameters.Time_Hide_Matter_Line_Column.HasValue ? navUserParameters.Time_Hide_Matter_Line_Column.Value : false,
                Time_Hide_Associate = navUserParameters.Time_Hide_Associate.HasValue ? navUserParameters.Time_Hide_Associate.Value : false,
                Time_Hide_Unit_Price = navUserParameters.Time_Hide_Unit_Price.HasValue ? navUserParameters.Time_Hide_Unit_Price.Value : false,

                Can_Read_ExpenseSheet = navUserParameters.Can_Read_ExpenseSheet.HasValue ? navUserParameters.Can_Read_ExpenseSheet.Value : false,
                Can_Read_TimeSheet = navUserParameters.Can_Read_TimeSheet.HasValue ? navUserParameters.Can_Read_TimeSheet.Value : false,
                Can_Read_Web_Portal_Setup = navUserParameters.Can_Read_Web_Portal_Setup.HasValue ? navUserParameters.Can_Read_Web_Portal_Setup.Value : false,
                Can_Read_Gauge_Chart = navUserParameters.Can_Read_Gauge_Chart.HasValue ? navUserParameters.Can_Read_Gauge_Chart.Value : false,
                Can_Read_Gauge_Chart_Hour = navUserParameters.Can_Read_Gauge_Chart_Hour.HasValue ? navUserParameters.Can_Read_Gauge_Chart_Hour.Value : false, // YSA Gauge Chart Hour
                Can_Read_Hours_Distribution = navUserParameters.Can_Read_Hours_Distribution.HasValue ? navUserParameters.Can_Read_Hours_Distribution.Value : false,
                Can_Read_Total_Hours_Chart = navUserParameters.Can_Read_Total_Hours_Chart.HasValue ? navUserParameters.Can_Read_Total_Hours_Chart.Value : false,
                Can_Read_TimeSheet_Synt = navUserParameters.Can_Read_TimeSheet_Synt.HasValue ? navUserParameters.Can_Read_TimeSheet_Synt.Value : false,
                Can_Export_Excel = navUserParameters.Can_Export_Excel.HasValue ? navUserParameters.Can_Export_Excel.Value : false,
                Can_Export_Excel_Expense = navUserParameters.Can_Export_Excel_Expense.HasValue ? navUserParameters.Can_Export_Excel_Expense.Value : false,
                Can_Upload_Attached_File = navUserParameters.Can_Upload_Attached_File.HasValue ? navUserParameters.Can_Upload_Attached_File.Value : false,
                Path_Folder_Uploaded_Files = navUserParameters.Path_Folder_Uploaded_Files,

                Time_Hide_Action_Code = navUserParameters.Time_Hide_Action_Code.HasValue ? navUserParameters.Time_Hide_Action_Code.Value : false,
                Timer_Validation = navUserParameters.Timer_Validation.HasValue ? (bool)navUserParameters.Timer_Validation : false,
                Can_Filter_By_Date = navUserParameters.Can_Filter_By_Date.HasValue ? (bool)navUserParameters.Can_Filter_By_Date : false,
                Can_Add_Row = navUserParameters.Can_Add_Row.HasValue ? (bool)navUserParameters.Can_Add_Row : false,
                Exp_Hide_Column_Kilometer = navUserParameters.Exp_Hide_Column_Kilometer.HasValue ? (bool)navUserParameters.Exp_Hide_Column_Kilometer : false,
                Time_Hide_Write_Off = navUserParameters.Time_Hide_Write_Off.HasValue ? navUserParameters.Time_Hide_Write_Off.Value : false,
                Exp_Hide_Write_off = navUserParameters.Exp_Hide_Write_off.HasValue ? navUserParameters.Exp_Hide_Write_off.Value : false
            };
        }

        public static UserParameters createUserParametersWithVisibilities(
            bool HideQuantityTime,
            bool HideLineAmountTime,
            bool HideMatterLineColumnTime,
            bool HideUnitOfMeasureTime,
            bool HideAssociateTime,
            bool HideUnitPrice,
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
            bool CanReadGaugeChartHour, // YSA Gauge Chart Hour
            bool CanReadHoursDistribution,
            bool CanReadTotalHours,
            bool CanReadSynthesis,
            bool CanExportExcel,
            bool CanExportExcelExpenseSheet,
            bool CanUploadAttachedFile,
            string PathUploadAttachedFile,
            bool HideActionCode,
            bool TimerValidation,
            bool CanFilterByDate,
            bool CanAddRow,
            bool HideKilometer,
            bool HideTimeWriteOff,
            bool HideExpWriteOff)
        {
            //On récupère les types actions de temps via le service Navision
            Uri uri = new Uri(ConfigurationManager.AppSettings["NavisionODataUri"]);
            var NAV = new NavServiceRef.NAV(uri);

            string UsernameSession = System.Web.HttpContext.Current.Session["Username"].ToString();
            string PasswordSession = System.Web.HttpContext.Current.Session["Password"].ToString();


            //NAV.Credentials = CredentialCache.DefaultNetworkCredentials;
            NAV.Credentials = SBLawyerWebApplication.Libs.SBLawyerCredential.GetCredentialCache(UsernameSession, PasswordSession);

            //Création du paramètrage utilisateur
            string userID = System.Web.HttpContext.Current.Session["Username"].ToString().ToUpper();
            NavServiceRef.WebPortalSetup navUserParameters = NavServiceRef.WebPortalSetup.CreateWebPortalSetup(userID);

            //Mise a jour des données modifiées
            navUserParameters.Time_Hide_Quantity = HideQuantityTime;
            navUserParameters.Time_Hide_Line_Amount = HideLineAmountTime;
            navUserParameters.Time_Hide_Matter_Line_Column = HideMatterLineColumnTime;
            navUserParameters.Time_Hide_Unit_of_Measure = HideUnitOfMeasureTime;
            navUserParameters.Time_Hide_Associate = HideAssociateTime;
            navUserParameters.Time_Hide_Unit_Price = HideUnitPrice;
            navUserParameters.Time_Hide_Post_Conf_Message = HidePostingConfirmMessageTime;
            navUserParameters.Exp_Hide_Quantity = HideQuantityExpense;
            navUserParameters.Exp_Hide_Line_Amount = HideLineAmountExpense;
            navUserParameters.Exp_Hide_Unit_of_Measure = HideUnitOfMeasureExpense;
            navUserParameters.Exp_Hide_Associate = HideAssociateExpense;
            navUserParameters.Exp_Hide_Post_Conf_Message = HidePostingConfirmMessageExpense;
            navUserParameters.Can_Read_ExpenseSheet = CanReadExpenseSheet;
            navUserParameters.Can_Read_TimeSheet = CanReadTimeSheet;
            navUserParameters.Can_Read_Web_Portal_Setup = CanReadWebPortalSetup;
            navUserParameters.Can_Read_Gauge_Chart = CanReadGaugeChart;
            navUserParameters.Can_Read_Gauge_Chart_Hour = CanReadGaugeChartHour; // YSA Gauge Chart Hour
            navUserParameters.Can_Read_Hours_Distribution = CanReadHoursDistribution;
            navUserParameters.Can_Read_Total_Hours_Chart = CanReadTotalHours;
            navUserParameters.Can_Read_TimeSheet_Synt = CanReadSynthesis;
            navUserParameters.Can_Export_Excel = CanExportExcel;
            navUserParameters.Can_Export_Excel_Expense = CanExportExcelExpenseSheet;
            navUserParameters.Can_Upload_Attached_File = CanUploadAttachedFile;
            navUserParameters.Path_Folder_Uploaded_Files = PathUploadAttachedFile;
            navUserParameters.Time_Hide_Action_Code = HideActionCode;
            navUserParameters.Timer_Validation = TimerValidation;
            navUserParameters.Can_Filter_By_Date = CanFilterByDate;
            navUserParameters.Can_Add_Row = CanAddRow;
            navUserParameters.Exp_Hide_Column_Kilometer = HideKilometer;
            navUserParameters.Time_Hide_Write_Off = HideTimeWriteOff;
            navUserParameters.Exp_Hide_Write_off = HideExpWriteOff;

            NAV.AddToWebPortalSetup(navUserParameters);
            NAV.SaveChanges(SaveChangesOptions.PatchOnUpdate);


            return new UserParameters()
            {
                User_ID = navUserParameters.User_ID,

                Sel_Line_Background_Color = navUserParameters.Sel_Line_Background_Color,
                Sel_Line_Foreground_Color = navUserParameters.Sel_Line_Foreground_Color,

                Not_Editable_Background_Color = navUserParameters.Not_Editable_Background_Color,
                Not_Editable_Foreground_Color = navUserParameters.Not_Editable_Foreground_Color,

                Jnl_Background_Color = navUserParameters.Jnl_Background_Color,
                Jnl_Foreground_Color = navUserParameters.Jnl_Foreground_Color,

                Mle_Background_Color = navUserParameters.Mle_Background_Color,
                Mle_Foreground_Color = navUserParameters.Mle_Foreground_Color,

                Callback_Background_Color = navUserParameters.Callback_Background_Color,
                Callback_Foreground_Color = navUserParameters.Callback_Foreground_Color,

                Color_Serie_1 = navUserParameters.Color_Serie_1,
                Color_Serie_2 = navUserParameters.Color_Serie_2,
                Color_Serie_3 = navUserParameters.Color_Serie_3,
                Color_Serie_4 = navUserParameters.Color_Serie_4,
                Color_Serie_5 = navUserParameters.Color_Serie_5,
                Color_Serie_6 = navUserParameters.Color_Serie_6,

                Exp_Hide_Line_Amount = navUserParameters.Exp_Hide_Line_Amount.HasValue ? navUserParameters.Exp_Hide_Line_Amount.Value : false,
                Exp_Hide_Post_Conf_Message = navUserParameters.Exp_Hide_Post_Conf_Message.HasValue ? navUserParameters.Exp_Hide_Post_Conf_Message.Value : false,
                Exp_Hide_Quantity = navUserParameters.Exp_Hide_Quantity.HasValue ? navUserParameters.Exp_Hide_Quantity.Value : false,
                Exp_Hide_Unit_of_Measure = navUserParameters.Exp_Hide_Unit_of_Measure.HasValue ? navUserParameters.Exp_Hide_Unit_of_Measure.Value : false,
                Exp_Hide_Matter_Line_Column = navUserParameters.Exp_Hide_Matter_Line_Column.HasValue ? navUserParameters.Exp_Hide_Matter_Line_Column.Value : false,
                Exp_Hide_Associate = navUserParameters.Exp_Hide_Associate.HasValue ? navUserParameters.Exp_Hide_Associate.Value : false,

                Time_Hide_Line_Amount = navUserParameters.Time_Hide_Line_Amount.HasValue ? navUserParameters.Time_Hide_Line_Amount.Value : false,
                Time_Hide_Post_Conf_Message = navUserParameters.Time_Hide_Post_Conf_Message.HasValue ? navUserParameters.Time_Hide_Post_Conf_Message.Value : false,
                Time_Hide_Quantity = navUserParameters.Time_Hide_Quantity.HasValue ? navUserParameters.Time_Hide_Quantity.Value : false,
                Time_Hide_Unit_of_Measure = navUserParameters.Time_Hide_Unit_of_Measure.HasValue ? navUserParameters.Time_Hide_Unit_of_Measure.Value : false,
                Time_Hide_Matter_Line_Column = navUserParameters.Time_Hide_Matter_Line_Column.HasValue ? navUserParameters.Time_Hide_Matter_Line_Column.Value : false,
                Time_Hide_Associate = navUserParameters.Time_Hide_Associate.HasValue ? navUserParameters.Time_Hide_Associate.Value : false,
                Time_Hide_Unit_Price = navUserParameters.Time_Hide_Unit_Price.HasValue ? navUserParameters.Time_Hide_Unit_Price.Value : false,

                Can_Read_ExpenseSheet = navUserParameters.Can_Read_ExpenseSheet.HasValue ? navUserParameters.Can_Read_ExpenseSheet.Value : false,
                Can_Read_TimeSheet = navUserParameters.Can_Read_TimeSheet.HasValue ? navUserParameters.Can_Read_TimeSheet.Value : false,
                Can_Read_Web_Portal_Setup = navUserParameters.Can_Read_Web_Portal_Setup.HasValue ? navUserParameters.Can_Read_Web_Portal_Setup.Value : false,

                Can_Read_Gauge_Chart = navUserParameters.Can_Read_Gauge_Chart.HasValue ? navUserParameters.Can_Read_Gauge_Chart.Value : false,
                Can_Read_Gauge_Chart_Hour = navUserParameters.Can_Read_Gauge_Chart_Hour.HasValue ? navUserParameters.Can_Read_Gauge_Chart_Hour.Value : false, //YSA Gauge Chart
                Can_Read_Hours_Distribution = navUserParameters.Can_Read_Hours_Distribution.HasValue ? navUserParameters.Can_Read_Hours_Distribution.Value : false,
                Can_Read_Total_Hours_Chart = navUserParameters.Can_Read_Total_Hours_Chart.HasValue ? navUserParameters.Can_Read_Total_Hours_Chart.Value : false,
                Can_Read_TimeSheet_Synt = navUserParameters.Can_Read_TimeSheet_Synt.HasValue ? navUserParameters.Can_Read_TimeSheet_Synt.Value : false,
                Can_Export_Excel = navUserParameters.Can_Export_Excel.HasValue ? navUserParameters.Can_Export_Excel.Value : false,
                Can_Export_Excel_Expense = navUserParameters.Can_Export_Excel_Expense.HasValue ? navUserParameters.Can_Export_Excel_Expense.Value : false,
                Can_Upload_Attached_File = navUserParameters.Can_Upload_Attached_File.HasValue ? navUserParameters.Can_Upload_Attached_File.Value : false,
                Path_Folder_Uploaded_Files = navUserParameters.Path_Folder_Uploaded_Files,

                Time_Hide_Action_Code = navUserParameters.Time_Hide_Action_Code.HasValue ? navUserParameters.Time_Hide_Action_Code.Value : false,
                Timer_Validation = navUserParameters.Timer_Validation.HasValue ? (bool)navUserParameters.Timer_Validation : false,
                Can_Filter_By_Date = navUserParameters.Can_Filter_By_Date.HasValue ? (bool)navUserParameters.Can_Filter_By_Date : false,
                Can_Add_Row = navUserParameters.Can_Add_Row.HasValue ? (bool)navUserParameters.Can_Add_Row : false,
                Exp_Hide_Column_Kilometer = navUserParameters.Exp_Hide_Column_Kilometer.HasValue ? (bool)navUserParameters.Exp_Hide_Column_Kilometer : false,
                Time_Hide_Write_Off = navUserParameters.Time_Hide_Write_Off.HasValue ? navUserParameters.Time_Hide_Write_Off.Value : false,
                Exp_Hide_Write_off = navUserParameters.Exp_Hide_Write_off.HasValue ? navUserParameters.Exp_Hide_Write_off.Value : false
            };
        }

        public static UserParameters updateUserParameters(UserParameters parameters)
        {
            //On récupère les types actions de temps via le service Navision
            Uri uri = new Uri(ConfigurationManager.AppSettings["NavisionODataUri"]);
            var NAV = new NavServiceRef.NAV(uri);

            string UsernameSession = System.Web.HttpContext.Current.Session["Username"].ToString();
            string PasswordSession = System.Web.HttpContext.Current.Session["Password"].ToString();


            //NAV.Credentials = CredentialCache.DefaultNetworkCredentials;
            NAV.Credentials = SBLawyerWebApplication.Libs.SBLawyerCredential.GetCredentialCache(UsernameSession, PasswordSession);

            var requestUserParameters = from up in NAV.WebPortalSetup
                                        select up;

            NavServiceRef.WebPortalSetup resultUserParameters = requestUserParameters.SingleOrDefault();

            if (resultUserParameters != null)
            {
                resultUserParameters.Sel_Line_Background_Color = parameters.Sel_Line_Background_Color;
                resultUserParameters.Sel_Line_Foreground_Color = parameters.Sel_Line_Foreground_Color;

                resultUserParameters.Not_Editable_Background_Color = parameters.Not_Editable_Background_Color;
                resultUserParameters.Not_Editable_Foreground_Color = parameters.Not_Editable_Foreground_Color;

                resultUserParameters.Jnl_Background_Color = parameters.Jnl_Background_Color;
                resultUserParameters.Jnl_Foreground_Color = parameters.Jnl_Foreground_Color;

                resultUserParameters.Mle_Background_Color = parameters.Mle_Background_Color;
                resultUserParameters.Mle_Foreground_Color = parameters.Mle_Foreground_Color;

                resultUserParameters.Callback_Background_Color = parameters.Callback_Background_Color;
                resultUserParameters.Callback_Foreground_Color = parameters.Callback_Foreground_Color;

                resultUserParameters.Color_Serie_1 = parameters.Color_Serie_1;
                resultUserParameters.Color_Serie_2 = parameters.Color_Serie_2;
                resultUserParameters.Color_Serie_3 = parameters.Color_Serie_3;
                resultUserParameters.Color_Serie_4 = parameters.Color_Serie_4;
                resultUserParameters.Color_Serie_5 = parameters.Color_Serie_5;
                resultUserParameters.Color_Serie_6 = parameters.Color_Serie_6;

                resultUserParameters.Exp_Hide_Line_Amount = parameters.Exp_Hide_Line_Amount;
                resultUserParameters.Exp_Hide_Post_Conf_Message = parameters.Exp_Hide_Post_Conf_Message;
                resultUserParameters.Exp_Hide_Quantity = parameters.Exp_Hide_Quantity;
                resultUserParameters.Exp_Hide_Unit_of_Measure = parameters.Exp_Hide_Unit_of_Measure;
                resultUserParameters.Exp_Hide_Associate = parameters.Exp_Hide_Associate;

                resultUserParameters.Time_Hide_Line_Amount = parameters.Time_Hide_Line_Amount;
                resultUserParameters.Time_Hide_Matter_Line_Column = parameters.Time_Hide_Matter_Line_Column;
                resultUserParameters.Time_Hide_Post_Conf_Message = parameters.Time_Hide_Post_Conf_Message;
                resultUserParameters.Time_Hide_Quantity = parameters.Time_Hide_Quantity;
                resultUserParameters.Time_Hide_Unit_of_Measure = parameters.Time_Hide_Unit_of_Measure;
                resultUserParameters.Time_Hide_Associate = parameters.Time_Hide_Associate;

                resultUserParameters.Can_Read_ExpenseSheet = parameters.Can_Read_ExpenseSheet;
                resultUserParameters.Can_Read_TimeSheet = parameters.Can_Read_TimeSheet;
                resultUserParameters.Can_Read_Web_Portal_Setup = parameters.Can_Read_Web_Portal_Setup;
                resultUserParameters.Can_Read_Gauge_Chart = parameters.Can_Read_Gauge_Chart;
                resultUserParameters.Can_Read_Gauge_Chart_Hour = parameters.Can_Read_Gauge_Chart_Hour; // YSA Gauge Chart
                resultUserParameters.Can_Read_Hours_Distribution = parameters.Can_Read_Hours_Distribution;
                resultUserParameters.Can_Read_Total_Hours_Chart = parameters.Can_Read_Total_Hours_Chart;
                resultUserParameters.Can_Read_TimeSheet_Synt = parameters.Can_Read_TimeSheet_Synt;
                resultUserParameters.Can_Export_Excel = parameters.Can_Export_Excel;
                resultUserParameters.Can_Export_Excel_Expense = parameters.Can_Export_Excel_Expense;
                resultUserParameters.Time_Hide_Unit_Price = parameters.Time_Hide_Unit_Price;
                resultUserParameters.Can_Upload_Attached_File = parameters.Can_Upload_Attached_File;
                resultUserParameters.Path_Folder_Uploaded_Files = parameters.Path_Folder_Uploaded_Files;
                resultUserParameters.Time_Hide_Action_Code = parameters.Time_Hide_Action_Code;
                resultUserParameters.Timer_Validation = parameters.Timer_Validation;
                resultUserParameters.Can_Filter_By_Date = parameters.Can_Filter_By_Date;
                resultUserParameters.Can_Add_Row = parameters.Can_Add_Row;
                resultUserParameters.Exp_Hide_Column_Kilometer = parameters.Exp_Hide_Column_Kilometer;
                resultUserParameters.Time_Hide_Write_Off = parameters.Time_Hide_Write_Off;
                resultUserParameters.Exp_Hide_Write_off = parameters.Exp_Hide_Write_off;

                NAV.UpdateObject(resultUserParameters);
                NAV.SaveChanges(SaveChangesOptions.PatchOnUpdate);
            }

            return parameters;
        }

        public static void updateColor(
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
            string NotEditableForegroundColor)
        {
            string userID = System.Web.HttpContext.Current.Session["Username"].ToString().ToUpper();

            //On récupère les types actions de temps via le service Navision
            Uri uri = new Uri(ConfigurationManager.AppSettings["NavisionODataUri"]);
            var NAV = new NavServiceRef.NAV(uri);
            string UsernameSession = System.Web.HttpContext.Current.Session["Username"].ToString();
            string PasswordSession = System.Web.HttpContext.Current.Session["Password"].ToString();

            //NAV.Credentials = CredentialCache.DefaultNetworkCredentials;
            NAV.Credentials = SBLawyerWebApplication.Libs.SBLawyerCredential.GetCredentialCache(UsernameSession, PasswordSession);


            var requestUserParameters = from up in NAV.WebPortalSetup
                                        select up;

            NavServiceRef.WebPortalSetup resultUserParameters = requestUserParameters.Where(up => up.User_ID == userID).SingleOrDefault();

            if (resultUserParameters != null)
            {
                resultUserParameters.Sel_Line_Background_Color = SelectedLineBackgroundColor;
                resultUserParameters.Sel_Line_Foreground_Color = SelectedLineForegroundColor;

                resultUserParameters.Not_Editable_Background_Color = NotEditableBackgroundColor;
                resultUserParameters.Not_Editable_Foreground_Color = NotEditableForegroundColor;

                resultUserParameters.Jnl_Background_Color = JnlBackgroundColor;
                resultUserParameters.Jnl_Foreground_Color = JnlForegroundColor;


                resultUserParameters.Mle_Background_Color = MleBackgroundColor;
                resultUserParameters.Mle_Foreground_Color = MleForegroundColor;

                resultUserParameters.Callback_Background_Color = CallBackBackgroundColor;
                resultUserParameters.Callback_Foreground_Color = CallBackForegroundColor;

                resultUserParameters.Color_Serie_1 = SerieColor1;
                resultUserParameters.Color_Serie_2 = SerieColor2;
                resultUserParameters.Color_Serie_3 = SerieColor3;
                resultUserParameters.Color_Serie_4 = SerieColor4;
                resultUserParameters.Color_Serie_5 = SerieColor5;
                resultUserParameters.Color_Serie_6 = SerieColor6;


                NAV.UpdateObject(resultUserParameters);
                NAV.SaveChanges(SaveChangesOptions.PatchOnUpdate);
            }
        }

        public static void updateVisibility(
            bool HideQuantityTime,
            bool HideLineAmountTime,
            bool HideMatterLineColumnTime,
            bool HideUnitOfMeasureTime,
            bool HideAssociateTime,
            bool HideUnitPrice,
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
            bool CanUploadAttachedFile,
            string PathUploadAttachedFile,
            bool HideActionCode,
            bool TimerValidation,
            bool CanFilterByDate,
            bool CanAddRow,
            bool HideKilometer,
            bool HideTimeWriteOff,
            bool HideExpWriteOff)
        {
            string userID = System.Web.HttpContext.Current.Session["Username"].ToString().ToUpper();


            //On récupère les types actions de temps via le service Navision
            Uri uri = new Uri(ConfigurationManager.AppSettings["NavisionODataUri"]);
            var NAV = new NavServiceRef.NAV(uri);

            string UsernameSession = System.Web.HttpContext.Current.Session["Username"].ToString();
            string PasswordSession = System.Web.HttpContext.Current.Session["Password"].ToString();

            //NAV.Credentials = CredentialCache.DefaultNetworkCredentials;
            NAV.Credentials = SBLawyerWebApplication.Libs.SBLawyerCredential.GetCredentialCache(UsernameSession, PasswordSession);

            var requestUserParameters = from up in NAV.WebPortalSetup
                                        select up;

            NavServiceRef.WebPortalSetup resultUserParameters = requestUserParameters.Where(up => up.User_ID == userID).SingleOrDefault();

            if (resultUserParameters != null)
            {
                resultUserParameters.Time_Hide_Quantity = HideQuantityTime;
                resultUserParameters.Time_Hide_Line_Amount = HideLineAmountTime;
                resultUserParameters.Time_Hide_Matter_Line_Column = HideMatterLineColumnTime;
                resultUserParameters.Time_Hide_Unit_of_Measure = HideUnitOfMeasureTime;
                resultUserParameters.Time_Hide_Associate = HideAssociateTime;
                resultUserParameters.Time_Hide_Unit_Price = HideUnitPrice;
                resultUserParameters.Time_Hide_Post_Conf_Message = HidePostingConfirmMessageTime;
                resultUserParameters.Exp_Hide_Quantity = HideQuantityExpense;
                resultUserParameters.Exp_Hide_Line_Amount = HideLineAmountExpense;
                resultUserParameters.Exp_Hide_Unit_of_Measure = HideUnitOfMeasureExpense;
                resultUserParameters.Exp_Hide_Associate = HideAssociateExpense;
                resultUserParameters.Exp_Hide_Post_Conf_Message = HidePostingConfirmMessageExpense;
                resultUserParameters.Can_Read_ExpenseSheet = CanReadExpenseSheet;
                resultUserParameters.Can_Read_TimeSheet = CanReadTimeSheet;
                resultUserParameters.Can_Read_Web_Portal_Setup = CanReadWebPortalSetup;
                resultUserParameters.Can_Read_Gauge_Chart = CanReadGaugeChart;
                resultUserParameters.Can_Read_Gauge_Chart_Hour = CanReadGaugeChartHour; // YSA Gauge Chart
                resultUserParameters.Can_Read_Hours_Distribution = CanReadHoursDistribution;
                resultUserParameters.Can_Read_Total_Hours_Chart = CanReadTotalHours;
                resultUserParameters.Can_Read_TimeSheet_Synt = CanReadSynthesis;
                resultUserParameters.Can_Export_Excel = CanExportExcel;
                resultUserParameters.Can_Export_Excel_Expense = CanExportExcelExpenseSheet;
                resultUserParameters.Can_Upload_Attached_File = CanUploadAttachedFile;
                resultUserParameters.Path_Folder_Uploaded_Files = PathUploadAttachedFile;
                resultUserParameters.Time_Hide_Action_Code = HideActionCode;
                resultUserParameters.Timer_Validation = TimerValidation;
                resultUserParameters.Can_Filter_By_Date = CanFilterByDate;
                resultUserParameters.Can_Add_Row = CanAddRow;
                resultUserParameters.Exp_Hide_Column_Kilometer = HideKilometer;
                resultUserParameters.Time_Hide_Write_Off = HideTimeWriteOff;
                resultUserParameters.Exp_Hide_Write_off = HideExpWriteOff;

                NAV.UpdateObject(resultUserParameters);
                NAV.SaveChanges(SaveChangesOptions.PatchOnUpdate);
            }

        }
        public static UserParameters deleteUserParameters()
        {
            //On récupère les types actions de temps via le service Navision
            Uri uri = new Uri(ConfigurationManager.AppSettings["NavisionODataUri"]);
            var NAV = new NavServiceRef.NAV(uri);

            string UsernameSession = System.Web.HttpContext.Current.Session["Username"].ToString();
            string PasswordSession = System.Web.HttpContext.Current.Session["Password"].ToString();


            //NAV.Credentials = CredentialCache.DefaultNetworkCredentials;
            NAV.Credentials = SBLawyerWebApplication.Libs.SBLawyerCredential.GetCredentialCache(UsernameSession, PasswordSession);
            //Création du paramètrage utilisateur
            string userID = System.Web.HttpContext.Current.Session["Username"].ToString().ToUpper();

            var requestUserParameters = from up in NAV.WebPortalSetup
                                        select up;

            var navUserParameters = requestUserParameters.Where(up => up.User_ID == userID).AsEnumerable().First();

            NAV.DeleteObject(navUserParameters);
            NAV.SaveChanges(SaveChangesOptions.PatchOnUpdate);

            return new UserParameters()
            {
                User_ID = navUserParameters.User_ID,

                Sel_Line_Background_Color = navUserParameters.Sel_Line_Background_Color,
                Sel_Line_Foreground_Color = navUserParameters.Sel_Line_Foreground_Color,

                Not_Editable_Background_Color = navUserParameters.Not_Editable_Background_Color,
                Not_Editable_Foreground_Color = navUserParameters.Not_Editable_Foreground_Color,

                Jnl_Background_Color = navUserParameters.Jnl_Background_Color,
                Jnl_Foreground_Color = navUserParameters.Jnl_Foreground_Color,

                Mle_Background_Color = navUserParameters.Mle_Background_Color,
                Mle_Foreground_Color = navUserParameters.Mle_Foreground_Color,

                Callback_Background_Color = navUserParameters.Callback_Background_Color,
                Callback_Foreground_Color = navUserParameters.Callback_Foreground_Color,

                Color_Serie_1 = navUserParameters.Color_Serie_1,
                Color_Serie_2 = navUserParameters.Color_Serie_2,
                Color_Serie_3 = navUserParameters.Color_Serie_3,
                Color_Serie_4 = navUserParameters.Color_Serie_4,
                Color_Serie_5 = navUserParameters.Color_Serie_5,
                Color_Serie_6 = navUserParameters.Color_Serie_6,

                Exp_Hide_Line_Amount = navUserParameters.Exp_Hide_Line_Amount.HasValue ? navUserParameters.Exp_Hide_Line_Amount.Value : false,
                Exp_Hide_Post_Conf_Message = navUserParameters.Exp_Hide_Post_Conf_Message.HasValue ? navUserParameters.Exp_Hide_Post_Conf_Message.Value : false,
                Exp_Hide_Quantity = navUserParameters.Exp_Hide_Quantity.HasValue ? navUserParameters.Exp_Hide_Quantity.Value : false,
                Exp_Hide_Unit_of_Measure = navUserParameters.Exp_Hide_Unit_of_Measure.HasValue ? navUserParameters.Exp_Hide_Unit_of_Measure.Value : false,
                Exp_Hide_Matter_Line_Column = navUserParameters.Exp_Hide_Matter_Line_Column.HasValue ? navUserParameters.Exp_Hide_Matter_Line_Column.Value : false,
                Exp_Hide_Associate = navUserParameters.Exp_Hide_Associate.HasValue ? navUserParameters.Exp_Hide_Associate.Value : false,

                Time_Hide_Line_Amount = navUserParameters.Time_Hide_Line_Amount.HasValue ? navUserParameters.Time_Hide_Line_Amount.Value : false,
                Time_Hide_Post_Conf_Message = navUserParameters.Time_Hide_Post_Conf_Message.HasValue ? navUserParameters.Time_Hide_Post_Conf_Message.Value : false,
                Time_Hide_Quantity = navUserParameters.Time_Hide_Quantity.HasValue ? navUserParameters.Time_Hide_Quantity.Value : false,
                Time_Hide_Unit_of_Measure = navUserParameters.Time_Hide_Unit_of_Measure.HasValue ? navUserParameters.Time_Hide_Unit_of_Measure.Value : false,
                Time_Hide_Matter_Line_Column = navUserParameters.Time_Hide_Matter_Line_Column.HasValue ? navUserParameters.Time_Hide_Matter_Line_Column.Value : false,
                Time_Hide_Associate = navUserParameters.Time_Hide_Associate.HasValue ? navUserParameters.Time_Hide_Associate.Value : false,
                Time_Hide_Unit_Price = navUserParameters.Time_Hide_Unit_Price.HasValue ? navUserParameters.Time_Hide_Unit_Price.Value : false,

                Can_Read_ExpenseSheet = navUserParameters.Can_Read_ExpenseSheet.HasValue ? navUserParameters.Can_Read_ExpenseSheet.Value : false,
                Can_Read_TimeSheet = navUserParameters.Can_Read_TimeSheet.HasValue ? navUserParameters.Can_Read_TimeSheet.Value : false,
                Can_Read_Web_Portal_Setup = navUserParameters.Can_Read_Web_Portal_Setup.HasValue ? navUserParameters.Can_Read_Web_Portal_Setup.Value : false,
                Can_Read_Gauge_Chart = navUserParameters.Can_Read_Gauge_Chart.HasValue ? navUserParameters.Can_Read_Gauge_Chart.Value : false,
                Can_Read_Gauge_Chart_Hour = navUserParameters.Can_Read_Gauge_Chart_Hour.HasValue ? navUserParameters.Can_Read_Gauge_Chart_Hour.Value : false, // YSA Gauge Chart
                Can_Read_Hours_Distribution = navUserParameters.Can_Read_Hours_Distribution.HasValue ? navUserParameters.Can_Read_Hours_Distribution.Value : false,
                Can_Read_Total_Hours_Chart = navUserParameters.Can_Read_Total_Hours_Chart.HasValue ? navUserParameters.Can_Read_Total_Hours_Chart.Value : false,
                Can_Read_TimeSheet_Synt = navUserParameters.Can_Read_TimeSheet_Synt.HasValue ? navUserParameters.Can_Read_TimeSheet_Synt.Value : false,
                Can_Export_Excel = navUserParameters.Can_Export_Excel.HasValue ? navUserParameters.Can_Export_Excel.Value : false,
                Can_Export_Excel_Expense = navUserParameters.Can_Export_Excel_Expense.HasValue ? navUserParameters.Can_Export_Excel_Expense.Value : false,
                Can_Upload_Attached_File = navUserParameters.Can_Upload_Attached_File.HasValue ? navUserParameters.Can_Upload_Attached_File.Value : false,
                Path_Folder_Uploaded_Files = navUserParameters.Path_Folder_Uploaded_Files,

                Time_Hide_Action_Code = navUserParameters.Time_Hide_Action_Code.HasValue ? navUserParameters.Time_Hide_Action_Code.Value : false,
                Timer_Validation = navUserParameters.Timer_Validation.HasValue ? (bool)navUserParameters.Timer_Validation : false,
                Can_Filter_By_Date = navUserParameters.Can_Filter_By_Date.HasValue ? (bool)navUserParameters.Can_Filter_By_Date : false,
                Can_Add_Row = navUserParameters.Can_Add_Row.HasValue ? (bool)navUserParameters.Can_Add_Row : false,
                Exp_Hide_Column_Kilometer = navUserParameters.Exp_Hide_Column_Kilometer.HasValue ? navUserParameters.Exp_Hide_Column_Kilometer.Value : false,
                Time_Hide_Write_Off = navUserParameters.Time_Hide_Write_Off.HasValue ? navUserParameters.Time_Hide_Write_Off.Value : false,
                Exp_Hide_Write_off = navUserParameters.Exp_Hide_Write_off.HasValue ? navUserParameters.Exp_Hide_Write_off.Value : false

            };
        }

        public static void InsertUserparametersFromWebService(UserParameters parameters, Value userParam)
        {
            parameters.User_ID = userParam.User_ID;

            parameters.Sel_Line_Background_Color = userParam.Sel_Line_Background_Color;
            parameters.Sel_Line_Foreground_Color = userParam.Sel_Line_Foreground_Color;

            parameters.Not_Editable_Background_Color = userParam.Not_Editable_Background_Color;
            parameters.Not_Editable_Foreground_Color = userParam.Not_Editable_Foreground_Color;

            parameters.Jnl_Background_Color = userParam.Jnl_Background_Color;
            parameters.Jnl_Foreground_Color = userParam.Jnl_Foreground_Color;

            parameters.Mle_Background_Color = userParam.Mle_Background_Color;
            parameters.Mle_Foreground_Color = userParam.Mle_Foreground_Color;

            parameters.Callback_Background_Color = userParam.Callback_Background_Color;
            parameters.Callback_Foreground_Color = userParam.Callback_Foreground_Color;

            parameters.Color_Serie_1 = userParam.Color_Serie_1;
            parameters.Color_Serie_2 = userParam.Color_Serie_2;
            parameters.Color_Serie_3 = userParam.Color_Serie_3;
            parameters.Color_Serie_4 = userParam.Color_Serie_4;
            parameters.Color_Serie_5 = userParam.Color_Serie_5;
            parameters.Color_Serie_6 = userParam.Color_Serie_6;

            parameters.Exp_Hide_Line_Amount = userParam.Exp_Hide_Line_Amount;//  ? userParam.Exp_Hide_Line_Amount.Value : false;
            parameters.Exp_Hide_Post_Conf_Message = userParam.Exp_Hide_Post_Conf_Message; //? userParam.Exp_Hide_Post_Conf_Message.Value : false;
            parameters.Exp_Hide_Quantity = userParam.Exp_Hide_Quantity;//.HasValue ? userParam.Exp_Hide_Quantity.Value : false;
            parameters.Exp_Hide_Unit_of_Measure = userParam.Exp_Hide_Unit_of_Measure;//.HasValue ? navUserParameters.Exp_Hide_Unit_of_Measure.Value : false;
            parameters.Exp_Hide_Matter_Line_Column = userParam.Exp_Hide_Matter_Line_Column;//.HasValue ? navUserParameters.Exp_Hide_Matter_Line_Column.Value : false;
            parameters.Exp_Hide_Associate = userParam.Exp_Hide_Associate;//.HasValue ? navUserParameters.Exp_Hide_Associate.Value : false;

            parameters.Time_Hide_Line_Amount = userParam.Time_Hide_Line_Amount;//.HasValue ? navUserParameters.Time_Hide_Line_Amount.Value : false;
            parameters.Time_Hide_Post_Conf_Message = userParam.Time_Hide_Post_Conf_Message;//.HasValue ? navUserParameters.Time_Hide_Post_Conf_Message.Value : false;
            parameters.Time_Hide_Quantity = userParam.Time_Hide_Quantity;//.HasValue ? navUserParameters.Time_Hide_Quantity.Value : false;
            parameters.Time_Hide_Unit_of_Measure = userParam.Time_Hide_Unit_of_Measure;//.HasValue ? navUserParameters.Time_Hide_Unit_of_Measure.Value : false;
            parameters.Time_Hide_Matter_Line_Column = userParam.Time_Hide_Matter_Line_Column;//.HasValue ? navUserParameters.Time_Hide_Matter_Line_Column.Value : false;
            parameters.Time_Hide_Associate = userParam.Time_Hide_Associate;//.HasValue ? navUserParameters.Time_Hide_Associate.Value : false;
            parameters.Time_Hide_Unit_Price = userParam.Time_Hide_Unit_Price;//.HasValue ? navUserParameters.Time_Hide_Unit_Price.Value : false;

            parameters.Can_Read_ExpenseSheet = userParam.Can_Read_ExpenseSheet;//.HasValue ? navUserParameters.Can_Read_ExpenseSheet.Value : false;
            parameters.Can_Read_TimeSheet = userParam.Can_Read_TimeSheet;//.HasValue ? navUserParameters.Can_Read_TimeSheet.Value : false;
            parameters.Can_Read_Web_Portal_Setup = userParam.Can_Read_Web_Portal_Setup;//.HasValue ? navUserParameters.Can_Read_Web_Portal_Setup.Value : false;

            parameters.Can_Read_Gauge_Chart = userParam.Can_Read_Gauge_Chart;//.HasValue ? navUserParameters.Can_Read_Gauge_Chart.Value : false;
            parameters.Can_Read_Gauge_Chart_Hour = userParam.Can_Read_Gauge_Chart_Hour;//.HasValue ? navUserParameters.Can_Read_Gauge_Chart_Hour.Value : false; //YSA Gauge Chart Hour
            parameters.Can_Read_Hours_Distribution = userParam.Can_Read_Hours_Distribution;//.HasValue ? navUserParameters.Can_Read_Hours_Distribution.Value : false;
            parameters.Can_Read_Total_Hours_Chart = userParam.Can_Read_Total_Hours_Chart;//.HasValue ? navUserParameters.Can_Read_Total_Hours_Chart.Value : false;
            parameters.Can_Read_TimeSheet_Synt = userParam.Can_Read_TimeSheet_Synt;//.HasValue ? navUserParameters.Can_Read_TimeSheet_Synt.Value : false;
            parameters.Can_Export_Excel = userParam.Can_Export_Excel;//.HasValue ? navUserParameters.Can_Export_Excel.Value : false;
            parameters.Can_Export_Excel_Expense = userParam.Can_Export_Excel_Expense;//.HasValue ? navUserParameters.Can_Export_Excel_Expense.Value : false;
            parameters.Can_Upload_Attached_File = userParam.Can_Upload_Attached_File;//.HasValue ? navUserParameters.Can_Upload_Attached_File.Value : false;
            parameters.Path_Folder_Uploaded_Files = userParam.Path_Folder_Uploaded_Files;

            parameters.Time_Hide_Action_Code = userParam.Time_Hide_Action_Code;//.HasValue ? navUserParameters.Time_Hide_Action_Code.Value : false;
            parameters.Timer_Validation = userParam.Timer_Validation;//.HasValue ? navUserParameters.Timer_Validation.Value : false;
            parameters.Can_Filter_By_Date = userParam.Can_Filter_By_Date;//.HasValue ? navUserParameters.Can_Filter_By_Date.Value : false;
            parameters.Can_Add_Row = userParam.Can_Add_Row;//.HasValue ? navUserParameters.Can_Add_Row.Value : false;
            parameters.Exp_Hide_Column_Kilometer = userParam.Exp_Hide_Column_Kilometer;//.HasValue ? navUserParameters.Exp_Hide_Column_Kilometer.Value : false;
            parameters.Time_Hide_Write_Off = userParam.Time_Hide_Write_Off;//.HasValue ? navUserParameters.Time_Hide_Write_Off.Value : false;
            parameters.Exp_Hide_Write_off = userParam.Exp_Hide_Write_off;//.HasValue ? navUserParameters.Exp_Hide_Write_off.Value : false;

            MatterFuncWebRef.MatterFunction webService = new MatterFuncWebRef.MatterFunction();

            //webService.Credentials = SBLawyerWebApplication.Libs.SBLawyerCredential.GetCredentialCache(UsernameSession, PasswordSession);

            webService.Url = ConfigurationManager.AppSettings["NavisionMatterFunctionUrl"];

            parameters.NavisionUserName = webService.GetNameFromUserID();
            parameters.CanEditWebPortalSetup = webService.GetWebPortalSetupPermission();
            parameters.TimeFormat = webService.GetFormatMatterSetup();
        }
    }
}