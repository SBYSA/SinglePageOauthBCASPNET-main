using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace AppModelv2_WebApp_OpenIDConnect_DotNet.Models
{
    public class MatterJnlLine
    {
        public string Matter_Template_Name { get; set; }
        public string matterBatchUserID { get; set; }
        public int Line_No { get; set; }
        public string Resource_No { get; set; }
        public string Matter_No { get; set; }
        public string Matter_Name { get; set; }
        public string Planning_Date { get; set; }
        public int Matter_Line_No { get; set; }
        public string Currency_Code { get; set; }
        public string Matter_Family { get; set; }
        public string Description_MatterLine { get; set; }
        public string Description { get; set; }
        public string Matter_Category { get; set; }
        public string Action_Code { get; set; }
        public string Unit_of_Measure_Code { get; set; }
        public string Partner_Code { get; set; }
        public string Sell_to_Customer_No { get; set; }
        public string Sell_to_Customer_Name { get; set; }
        public string External_Document_No { get; set; }
        public bool Non_Billable { get; set; }
        public double Quantity { get; set; }
        public double Quantity_Base { get; set; }
        public int Unit_Price_LCY { get; set; }
        public int Line_Amount_LCY { get; set; }
        public int Line_Amount { get; set; }
        public int Entry_No_To_Close { get; set; }
        public bool Editable_Entry { get; set; }
        public bool Company_Payment_Card { get; set; }
        public int Kilometers { get; set; }
        public string Partner_name { get; set; }
        public bool WriteOff { get; set; }
    }

    public class MatterJnlLineRoot
    {
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }
        public List<MatterJnlLine> Value { get; set; }
    }
    public static class MatterJnlLineDataContext
    {
        /// <summary>
        /// Permet de récupérer la liste des lignes de temps sur une période donnée
        /// </summary>
        /// <param name="beginDate">Date de début de recherche</param>
        /// <param name="endDate">Date de fin de recherche</param>
        /// <returns>Liste de ligne de temps</returns>
        public static List<MatterJnlLine> getMatterJnlLines(DateTime beginDate, DateTime endDate, string templateName)
        {
            //on initialise les lignes de temps
            List<MatterJnlLine> matterJnlLines = new List<MatterJnlLine>();

            string IdToken = System.Web.HttpContext.Current.Session["IdToken_Ticket"].ToString();
            var httpClient = new System.Net.Http.HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", IdToken);

            //Construction du lien API
            string BaseURL = ConfigurationManager.AppSettings["BaseURL"];
            string WorkTypeAPI = ConfigurationManager.AppSettings["matterjnllines"];
            string BCCompany = ConfigurationManager.AppSettings["BCCompany"];

            string environmentsUri = BaseURL + WorkTypeAPI + BCCompany;

            try
            {
                //YSA-DO Add filter to URI with beginDate, endDAte, templateName
                var response = httpClient.GetAsync(environmentsUri).Result;
                var content = response.Content.ReadAsStringAsync().Result;

                var result = JsonConvert.DeserializeObject<MatterJnlLineRoot>(content);

                //On traite la réponse
                foreach (MatterJnlLine mjl in result.Value)
                {

                    MatterJnlLine ligne = new MatterJnlLine();
                    ligne.Action_Code = mjl.Action_Code;
                    ligne.Description = mjl.Description;
                    ligne.Description_MatterLine = mjl.Description_MatterLine;
                    ligne.Entry_No_To_Close = mjl.Entry_No_To_Close;
                    ligne.Unit_Price_LCY = mjl.Unit_Price_LCY;
                    ligne.Line_Amount = mjl.Line_Amount_LCY;
                    ligne.Line_No = mjl.Line_No;
                    ligne.Matter_Family = mjl.Matter_Family;
                    ligne.Matter_Line_No = mjl.Matter_Line_No;
                    ligne.Matter_No = mjl.Matter_No;
                    ligne.Matter_Name = mjl.Matter_Name;
                    ligne.Planning_Date = mjl.Planning_Date;
                    ligne.Quantity_Base = mjl.Quantity_Base;
                    ligne.Partner_Code = mjl.Partner_Code;
                    ligne.Partner_name = mjl.Partner_name;
                    ligne.Sell_to_Customer_No = mjl.Sell_to_Customer_No;
                    ligne.Sell_to_Customer_Name = mjl.Sell_to_Customer_Name;
                    ligne.Unit_of_Measure_Code = mjl.Unit_of_Measure_Code;
                    ligne.Editable_Entry = mjl.Editable_Entry;
                    ligne.Kilometers = mjl.Kilometers;
                    ligne.WriteOff = mjl.WriteOff;

                    matterJnlLines.Add(ligne);
                }
            }
            catch (JsonException ex)
            {
                throw new ApplicationException(
                    "Erreur l'ors de l'execution de la requete OData.", ex);
            }

            return matterJnlLines;
        }
    }

}