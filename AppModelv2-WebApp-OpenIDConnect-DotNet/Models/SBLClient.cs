using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace AppModelv2_WebApp_OpenIDConnect_DotNet.Models
{
    public class SBLClient
    {
        public string No { get; set; }
        public string Sell_to_Name { get; set; }
        public string SBLClient_Label { get; set; }
        public string Search_Name { get; set; }
        public string Blocked { get; set; }
        public int Matter_Count_Active { get; set; }
    }

    public class SBLClientRoot
    {
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }
        public List<SBLClient> Value { get; set; }
    }
    public static class SBLClientDataContext
    {
        public static List<SBLClient> GetSBLClients(bool over_write)
        {
            List<SBLClient> result = (List<SBLClient>)HttpContext.Current.Session["SBLClients"];

            if (result == null || over_write)
            {
                List<SBLClient> SBLClientsLines = new List<SBLClient>();

                //Construction du lien API
                string BaseURL = ConfigurationManager.AppSettings["BaseURL"];
                string WorkTypeAPI = ConfigurationManager.AppSettings["customers"];
                string BCCompany = ConfigurationManager.AppSettings["BCCompany"];

                //Récupération du Token si existe
                string IdToken = System.Web.HttpContext.Current.Session["IdToken_Ticket"].ToString();
                var httpClient = new System.Net.Http.HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", IdToken);

                try
                {
                    int minClientActiveMatters = Convert.ToInt32(ConfigurationManager.AppSettings["MinClientActiveMatters"]);
                    //On créé la requete de service OData pour le type d'objet
                    string filters = "Matter_Count_Active gt" + minClientActiveMatters;
                    string environmentsUri = BaseURL + WorkTypeAPI + BCCompany + filters;

                    var response = httpClient.GetAsync(environmentsUri).Result;
                    var content = response.Content.ReadAsStringAsync().Result;
                    //On récupère le schéma de réponse
                    var result_SBClient = JsonConvert.DeserializeObject<SBLClientRoot>(content);



                    //On traite la réponse
                    foreach (SBLClient c in result_SBClient.Value)
                        {
                            SBLClient ligne = new SBLClient();

                            ligne.No = c.No;
                            ligne.Sell_to_Name = c.Sell_to_Name;
                            ligne.SBLClient_Label = c.SBLClient_Label;
                            SBLClientsLines.Add(ligne);
                        }

                }
                catch (JsonException ex)
                {
                    throw new ApplicationException(
                        "Erreur l'ors de l'execution de la requete OData.", ex);
                }

                HttpContext.Current.Session["SBLClients"] = result = SBLClientsLines;
            }
            return result;
        }
    }
    public static SBLClient createSBLClient()
    {
        return new SBLClient();
    }

    public static SBLClient updateSBLClient()
    {
        return new SBLClient();
    }

    public static SBLClient deleteSBLClient()
    {
        return new SBLClient();
    }
}