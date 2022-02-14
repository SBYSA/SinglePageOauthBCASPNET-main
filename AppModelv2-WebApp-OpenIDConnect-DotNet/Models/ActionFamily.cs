using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Configuration;

namespace AppModelv2_WebApp_OpenIDConnect_DotNet.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class ActionFamily
    {
        public string No { get; set; }
        public string Description { get; set; }
        public string Base_Unit_of_Measure { get; set; }
        public int Unit_Price { get; set; }
        public bool Blocked { get; set; }
        public string Matter_Line_Family { get; set; }
        public int Matter_Entry_Type { get; set; }
        public string Matter_Category { get; set; }
    }

    public class ActionFamilyRoot
    {
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }
        public List<ActionFamily> Value { get; set; }
    }

    public static class ActionFamilyDataContext
    {
        /// <summary>
        /// Permet de récupérer les familles d'action de la saisie des frais
        /// </summary>
        /// <param name="over_write">Faut il écraser les valeurs stockées en session?</param>
        /// <returns></returns>
        public static List<ActionFamily> GetActionFamilies(bool over_write)
        {
            return GetActionFamilies(over_write, code_langue: "");
        }

        /// <summary>
        /// Permet de récupérer les familles d'action de la saisie des frais
        /// </summary>
        /// <param name="over_write">Faut il écraser les valeurs stockées en session?</param>
        /// <returns></returns>
        public static List<ActionFamily> GetActionFamilies(bool over_write, string code_langue)
        {
            //On récupère les valeurs en sessions
            List<ActionFamily> actionFamilies = (List<ActionFamily>)HttpContext.Current.Session["ActionFamilies"];

            //Consomation d'API manuel ou absence de données
            if (actionFamilies == null || over_write)
            {
                List<ActionFamily> actionFamilyLines = new List<ActionFamily>();
                bool isPreFill = true;
                //if is => Pre-fill Description in Jnl (Bool)
                //YSA-DO var isPreFill = Codeunit Matter Function in API

                string IdToken = System.Web.HttpContext.Current.Session["IdToken_Ticket"].ToString();
                var httpClient = new System.Net.Http.HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", IdToken);

                const string environmentsUri = "https://api.businesscentral.dynamics.com/v2.0/d1176b80-8023-4b7a-adc8-99a22336517f/SBLawyer_TEST/api/sbconsulting/sblawyer/v1.0/companies(e34b1b2c-7f09-ec11-bb73-000d3a3926f4)/actionfamilies";

                var response = httpClient.GetAsync(environmentsUri).Result;
                var content = response.Content.ReadAsStringAsync().Result;

                var result = JsonConvert.DeserializeObject<ActionFamilyRoot>(content);

                foreach (ActionFamily af in result.Value)
                {
                    ActionFamily ligne = new ActionFamily();
                    ligne.No = af.No;

                    if (isPreFill == true)
                    {
                        if (code_langue == "")
                        {
                            ligne.Description = af.Description;

                        }
                        else
                        {
                            //YSA-DO Get Item Translation => string responseDescription = webService.getItemTranslation(ligne.Code, code_langue);
                            string responseDescription = "";
                            if (responseDescription == "")
                            {
                                ligne.Description = af.Description;
                            }
                            else
                            {
                                ligne.Description = responseDescription;
                            }
                        }
                    }
                    else
                    {
                        ligne.Description = "";
                    }

                    ligne.Matter_Category = af.Matter_Category;
                    ligne.Matter_Line_Family = af.Matter_Line_Family;
                    ligne.Matter_Entry_Type = af.Matter_Entry_Type;

                    actionFamilyLines.Add(ligne);
                }
                HttpContext.Current.Session["ActionFamilies"] = actionFamilies = actionFamilyLines;
            }

            return actionFamilies;
        }

        /// <summary>
        /// Permet de récupérer la liste des des failles d'action
        /// </summary>
        /// <param name="Matter_No">Numéro de l'affaire</param>
        /// <param name="Matter_Line_No">Numéro de la ligne d'affaire</param>
        /// <returns>Liste de type d'action</returns>
        public static List<ActionFamily> GetActionFamiliesFromMatter(string Matter_No, int Matter_Line_No)
        {
            if (Matter_No != "" && Matter_Line_No != 0)
            {
                //string matter_Category = MatterDataContext.GetMattersDetails(Matter_No).Matter_Category;
                //string code_langue = MatterDataContext.GetMattersDetails(Matter_No).codeLangue;
                //MatterLine matterLine = MatterLineDataContext.GetExpenseMatterMatterLines(Matter_No).Where(ml => ml.Matter_Line_No == Matter_Line_No).First();
                //int Matter_Entry_Type = matterLine.Matter_Entry_Type;

                //if (code_langue != "")
                //{

                //    return GetActionFamilies(true, code_langue).Where(af => (af.Matter_Category == "" || af.Matter_Category == matter_Category) && af.Matter_Entry_Type == Matter_Entry_Type);
                //}
                //else
                //{   //On recupère les familles action dont la catégorie affaire est égale à celle de l'affaire ou vide  ET dont la famille est egale à celle de la ligne d'affaire
                //    return GetActionFamilies(false).Where(af => (af.Matter_Category == "" || af.Matter_Category == matter_Category) && af.Matter_Entry_Type == Matter_Entry_Type);
                //}
                return GetActionFamilies(true);
            }
            else
                return GetActionFamilies(false);

        }

        public static ActionFamily createActionFamily()
        {
            return new ActionFamily();
        }

        public static ActionFamily updateActionFamily()
        {
            return new ActionFamily();
        }

        public static ActionFamily deleteActionFamily()
        {
            return new ActionFamily();
        }
    }
}