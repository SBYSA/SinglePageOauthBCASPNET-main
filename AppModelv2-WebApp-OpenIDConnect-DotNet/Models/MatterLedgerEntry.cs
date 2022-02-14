using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace AppModelv2_WebApp_OpenIDConnect_DotNet.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class MatterLedgerEntry
    {
        public int Entry_No { get; set; }
        public string Planning_Date { get; set; }
        public string Matter_No { get; set; }
        public string Matter_Name { get; set; }
        public int Matter_Line_No { get; set; }
        public string Matter_Family { get; set; }
        public string Description_MatterLine { get; set; }
        public string Entry_Type { get; set; }
        public string Type { get; set; }
        public string Resource_No { get; set; }
        public string Resource_Name { get; set; }
        public string Resource_Group_No { get; set; }
        public string Unit_of_Measure_Code { get; set; }
        public string Matter_Category { get; set; }
        public string Action_Code { get; set; }
        public bool Non_Billable { get; set; }
        public string Description { get; set; }
        public double Quantity_Base { get; set; }
        public double Remaining_Quantity { get; set; }
        public int Unit_Price_LCY { get; set; }
        public string Currency_Code { get; set; }
        public string Status { get; set; }
        public bool Write_Off { get; set; }
        public string Sell_to_Customer_No { get; set; }
        public string Sell_to_Customer_Name { get; set; }
        public string Partner_Code { get; set; }
        public double Line_Amount { get; set; }
        public double Line_Amount_LCY { get; set; }
        public bool Editable_Entry { get; set; }
        public int Matter_Entry_Type { get; set; }
        public int intLockBy { get; set; }
        public string Partner_name { get; set; }
        public int Kilometers { get; set; }
    }

    public class MatterLedgerEntryRoot
    {
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }
        public List<MatterLedgerEntry> Value { get; set; }
    }

    public static class MatterLedgerEntryDataContext
    {
        /// <summary>
        /// Permet de récupérer la liste des lignes de temps validées sur une période donnée
        /// </summary>
        /// <param name="beginDate">Date de début de recherche</param>
        /// <param name="endDate">Date de fin de recherche</param>
        /// <returns>Liste de lignes de temps validées</returns>
        public static List<MatterLedgerEntry> getMatterLedgerEntries(DateTime beginDate, DateTime endDate, string lineType)
        {
            List<MatterLedgerEntry> matterLedgerEntries = new List<MatterLedgerEntry>();

            string IdToken = System.Web.HttpContext.Current.Session["IdToken_Ticket"].ToString();
            var httpClient = new System.Net.Http.HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", IdToken);

            //Construction du lien API
            string BaseURL = ConfigurationManager.AppSettings["BaseURL"];
            string WorkTypeAPI = ConfigurationManager.AppSettings["matterledgentries"];
            string BCCompany = ConfigurationManager.AppSettings["BCCompany"];

            string environmentsUri = BaseURL + WorkTypeAPI + BCCompany;

            try
            {

                string filters = "";
                switch (lineType)
                {
                    case "time":
                        filters = "Planning_Date ge " + beginDate + " le " + endDate;
                        break;
                    default: //Expense
                        filters = "Planning_Date ge " + beginDate + " le " + endDate;
                        break;
                }

                var response = httpClient.GetAsync(environmentsUri).Result;
                var content = response.Content.ReadAsStringAsync().Result;

                var result = JsonConvert.DeserializeObject<MatterLedgerEntryRoot>(content);

                switch (lineType)
                {
                    case "time":
                        foreach (MatterLedgerEntry mle in result.Value)
                        {
                            if (mle.Matter_Entry_Type == 1 && mle.intLockBy != 4 && mle.intLockBy != 5)
                            {
                                MatterLedgerEntry entry = new MatterLedgerEntry();

                                entry.Action_Code = mle.Action_Code;
                                entry.Description = mle.Description;
                                entry.Description_MatterLine = mle.Description_MatterLine;
                                entry.Editable_Entry = mle.Editable_Entry;
                                entry.Entry_No = mle.Entry_No;
                                entry.Entry_Type = mle.Entry_Type;
                                entry.Unit_Price_LCY = mle.Unit_Price_LCY;
                                entry.Line_Amount = mle.Line_Amount_LCY;
                                entry.Matter_Entry_Type = mle.Matter_Entry_Type;
                                entry.Matter_Line_No = mle.Matter_Line_No;
                                entry.Matter_No = mle.Matter_No;
                                entry.Planning_Date = mle.Planning_Date;
                                entry.Quantity_Base = mle.Quantity_Base;
                                entry.Partner_Code = mle.Partner_Code;
                                entry.Partner_name = mle.Partner_name;
                                entry.Sell_to_Customer_No = mle.Sell_to_Customer_No;
                                entry.Sell_to_Customer_Name = mle.Sell_to_Customer_Name;
                                entry.Unit_of_Measure_Code = mle.Unit_of_Measure_Code;
                                entry.Matter_Name = mle.Matter_Name;
                                entry.Write_Off = mle.Write_Off;

                                matterLedgerEntries.Add(entry);
                            }
                        }

                        break;
                    default: //Expense
                        foreach (MatterLedgerEntry mle in result.Value)
                        {
                            if ((mle.Matter_Entry_Type == 2 || mle.Matter_Entry_Type == 3) && mle.intLockBy != 4 && mle.intLockBy != 5)
                            {
                                MatterLedgerEntry entry = new MatterLedgerEntry();

                                entry.Action_Code = mle.Action_Code;
                                entry.Description = mle.Description;
                                entry.Description_MatterLine = mle.Description_MatterLine;
                                entry.Editable_Entry = mle.Editable_Entry;
                                entry.Entry_No = mle.Entry_No;
                                entry.Entry_Type = mle.Entry_Type;
                                entry.Unit_Price_LCY = mle.Unit_Price_LCY;
                                entry.Line_Amount = mle.Line_Amount_LCY;
                                entry.Matter_Entry_Type = mle.Matter_Entry_Type;
                                entry.Matter_Line_No = mle.Matter_Line_No;
                                entry.Matter_No = mle.Matter_No;
                                entry.Planning_Date = mle.Planning_Date;
                                entry.Quantity_Base = mle.Quantity_Base;
                                entry.Partner_Code = mle.Partner_Code;
                                entry.Partner_name = mle.Partner_name;
                                entry.Sell_to_Customer_No = mle.Sell_to_Customer_No;
                                entry.Sell_to_Customer_Name = mle.Sell_to_Customer_Name;
                                entry.Unit_of_Measure_Code = mle.Unit_of_Measure_Code;
                                entry.Matter_Name = mle.Matter_Name;
                                entry.Kilometers = mle.Kilometers;
                                entry.Write_Off = mle.Write_Off;

                                matterLedgerEntries.Add(entry);
                            }
                        }
                        break;
                }
            }
            catch (JsonException ex)
            {
                throw new ApplicationException(
                    "Erreur l'ors de l'execution de la requete OData.", ex);
            }

            return matterLedgerEntries;
        }
    }

}