using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppModelv2_WebApp_OpenIDConnect_DotNet.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Matter
    {
        internal string codeLangue;
        public string Matter_No { get; set; }
        public string Description { get; set; }
        public string Matter_Label { get; set; }        
        public string Parent_Matter_No { get; set; }
        public string Partner_No { get; set; }
        public string Partner_Name { get; set; }
        public string Matter_Category { get; set; }
        public string Matter_Type { get; set; }
        public string Invoicing_Type { get; set; }
        public string Sell_to_Customer_No { get; set; }
        public string Sell_to_Name { get; set; }
        public string Starting_Date { get; set; }
        public string Ending_Date { get; set; }
        public int Status { get; set; }
        public bool Editable_Status { get; set; }
        public bool ExistService { get; set; }
        public bool ExistExpense { get; set; }
        public string Language_Code { get; set; }
    }

    public class MatterRoot
    {
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }
        public List<Matter> Value { get; set; }
    }

    public static class MatterDataContext
    {
        /// <summary>
        /// Permet de récupèrer une affaire avec son identifiant
        /// </summary>
        /// <param name="Matter_No"></param>
        /// <returns></returns>
        public static Matter GetMattersDetails(string Matter_No)
        {
            string IdToken = System.Web.HttpContext.Current.Session["IdToken_Ticket"].ToString();
            var httpClient = new System.Net.Http.HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", IdToken);

            string filter = "?$filter=Matter_No eq " + Matter_No;

            string environmentsUri = "https://api.businesscentral.dynamics.com/v2.0/d1176b80-8023-4b7a-adc8-99a22336517f/SBLawyer_TEST/api/sbconsulting/sblawyer/v1.0/companies(e34b1b2c-7f09-ec11-bb73-000d3a3926f4)/matters" + filter;

            var response = httpClient.GetAsync(environmentsUri).Result;
            var content = response.Content.ReadAsStringAsync().Result;

            var result = JsonConvert.DeserializeObject<MatterRoot>(content);

            return result.Value.First();
        }
        public static List<Matter> GetMatters(string Matter_No, string Matter_Name, string Sell_to_Customer_No, string Partner_Code)
        {
            string IdToken = System.Web.HttpContext.Current.Session["IdToken_Ticket"].ToString();
            var httpClient = new System.Net.Http.HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", IdToken);

            string environmentsUri = "https://api.businesscentral.dynamics.com/v2.0/d1176b80-8023-4b7a-adc8-99a22336517f/SBLawyer_TEST/api/sbconsulting/sblawyer/v1.0/companies(e34b1b2c-7f09-ec11-bb73-000d3a3926f4)/matters";
            try
            {
                List<Matter> matterLines = new List<Matter>();
                //On prépare les filtres
                string filters = "";

                if (!String.IsNullOrEmpty(Matter_No))
                    filters += "Matter_No eq '" + Matter_No + "'";

                if (!String.IsNullOrEmpty(Matter_Name))
                {
                    if (!String.IsNullOrEmpty(filters))
                        filters += "&";
                    filters += "Description eq '" + Matter_Name.ToLower() + "'";
                }

                if (!String.IsNullOrEmpty(Sell_to_Customer_No))
                {
                    if (!String.IsNullOrEmpty(filters))
                        filters += "&";
                    filters += "Sell_to_Customer_No eq '" + Sell_to_Customer_No + "'";
                }

                if (!String.IsNullOrEmpty(Partner_Code))
                {
                    if (!String.IsNullOrEmpty(filters))
                        filters += "&";
                    filters += " Partner_No eq '" + Partner_Code + "'";
                }

                var response = httpClient.GetAsync(environmentsUri + filters).Result;
                var content = response.Content.ReadAsStringAsync().Result;

                var result = JsonConvert.DeserializeObject<MatterRoot>(content);

                List<Matter> matterList = new List<Matter>();
                foreach (Matter m in result.Value)
                    matterList.Add(m);
                switch (HttpContext.Current.Session["gridType"].ToString())
                {
                    case "time":
                        //On construit les données au format SBLawyer WebApp
                        foreach (var m in matterList)
                        {
                            if (m.ExistService)
                            {
                                Matter ligne = new Matter();

                                ligne.Matter_No = m.Matter_No;
                                ligne.Matter_Label = m.Matter_Label;
                                ligne.Description = m.Description;
                                ligne.Parent_Matter_No = m.Parent_Matter_No;
                                ligne.Partner_No = m.Partner_No;
                                ligne.Matter_Category = m.Matter_Category;
                                ligne.Matter_Type = m.Matter_Type;
                                ligne.Invoicing_Type = m.Invoicing_Type;
                                ligne.Sell_to_Customer_No = m.Sell_to_Customer_No;
                                ligne.Sell_to_Name = m.Sell_to_Name;
                                ligne.Starting_Date = m.Starting_Date;
                                ligne.Ending_Date = m.Ending_Date;
                                ligne.Status = m.Status;
                                ligne.Editable_Status = m.Editable_Status;
                                ligne.Language_Code = m.Language_Code;
                                matterLines.Add(ligne);
                            }
                        }
                        break;


                    case "expense":
                        //On construit les données au format SBLawyer WebApp
                        foreach (var m in matterList)
                        {
                            if (m.ExistExpense)
                            {
                                Matter ligne = new Matter();

                                ligne.Matter_No = m.Matter_No;
                                ligne.Matter_Label = m.Matter_Label;
                                ligne.Description = m.Description;
                                ligne.Parent_Matter_No = m.Parent_Matter_No;
                                ligne.Partner_No = m.Partner_No;
                                ligne.Matter_Category = m.Matter_Category;
                                ligne.Matter_Type = m.Matter_Type;
                                ligne.Invoicing_Type = m.Invoicing_Type;
                                ligne.Sell_to_Customer_No = m.Sell_to_Customer_No;
                                ligne.Sell_to_Name = m.Sell_to_Name;
                                ligne.Starting_Date = m.Starting_Date;
                                ligne.Ending_Date = m.Ending_Date;
                                ligne.Status = m.Status;
                                ligne.Editable_Status = m.Editable_Status;
                                ligne.Language_Code = m.Language_Code;
                                matterLines.Add(ligne);
                            }
                        }
                        break;

                    default:
                        //On construit les données au format SBLawyer WebApp
                        foreach (var m in matterList)
                        {
                            Matter ligne = new Matter();

                            ligne.Matter_No = m.Matter_No;
                            ligne.Matter_Label = m.Matter_Label;
                            ligne.Description = m.Description;
                            ligne.Parent_Matter_No = m.Parent_Matter_No;
                            ligne.Partner_No = m.Partner_No;
                            ligne.Matter_Category = m.Matter_Category;
                            ligne.Matter_Type = m.Matter_Type;
                            ligne.Invoicing_Type = m.Invoicing_Type;
                            ligne.Sell_to_Customer_No = m.Sell_to_Customer_No;
                            ligne.Sell_to_Name = m.Sell_to_Name;
                            ligne.Starting_Date = m.Starting_Date;
                            ligne.Ending_Date = m.Ending_Date;
                            ligne.Status = m.Status;
                            ligne.Editable_Status = m.Editable_Status;
                            ligne.Language_Code = m.Language_Code;
                        }
                        break;
                }

                return matterLines;
            }
            catch (JsonException ex)
            {
                throw new ApplicationException(
                    "Erreur l'ors de l'execution de la requete.", ex);
            }
        }
        public static List<Matter> GetAvailableMatters(List<Matter> userMatters, string Matter_No, string Matter_Name, string Sell_to_Customer_No, string Partner_Code)
        {
            List<Matter> matterLines = new List<Matter>();

            string IdToken = System.Web.HttpContext.Current.Session["IdToken_Ticket"].ToString();
            var httpClient = new System.Net.Http.HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", IdToken);

            string environmentsUri = "https://api.businesscentral.dynamics.com/v2.0/d1176b80-8023-4b7a-adc8-99a22336517f/SBLawyer_TEST/api/sbconsulting/sblawyer/v1.0/companies(e34b1b2c-7f09-ec11-bb73-000d3a3926f4)/matters";

            try
            {
                //On prépare les filtres
                string filters = "";

                if (!String.IsNullOrEmpty(Matter_No))
                    filters += " Matter_No eq '" + Matter_No + "'";

                if (!String.IsNullOrEmpty(Matter_Name))
                {
                    if (!String.IsNullOrEmpty(filters))
                        filters += "&";
                    filters += "Description eq '" + Matter_Name + "'";
                }

                if (!String.IsNullOrEmpty(Sell_to_Customer_No))
                {
                    if (!String.IsNullOrEmpty(filters))
                        filters += "&";
                    filters += "Sell_to_Customer_No eq '" + Sell_to_Customer_No + "'";
                }

                if (!String.IsNullOrEmpty(Partner_Code))
                {
                    if (!String.IsNullOrEmpty(filters))
                        filters += "&";
                    filters += "Partner_No eq '" + Partner_Code + "'";
                }

                List<string> matter_NoS = new List<string>();

                //On retire les matters de l'utilisateur
                if (userMatters.Count > 0)
                {
                    foreach (Matter m in userMatters)
                        matter_NoS.Add(m.Matter_No);
                }

                var response = httpClient.GetAsync(environmentsUri + filters).Result;
                var content = response.Content.ReadAsStringAsync().Result;

                var result = JsonConvert.DeserializeObject<MatterRoot>(content);

                foreach (Matter m in result.Value)
                {
                    if (!matter_NoS.Contains(m.Matter_No))
                    {
                        Matter ligne = new Matter();

                        ligne.Matter_No = m.Matter_No;
                        ligne.Matter_Label = m.Matter_Label;
                        ligne.Description = m.Description;
                        ligne.Parent_Matter_No = m.Parent_Matter_No;
                        ligne.Partner_No = m.Partner_No;
                        ligne.Matter_Category = m.Matter_Category;
                        ligne.Matter_Type = m.Matter_Type;
                        ligne.Invoicing_Type = m.Invoicing_Type;
                        ligne.Sell_to_Customer_No = m.Sell_to_Customer_No;
                        ligne.Sell_to_Name = m.Sell_to_Name;
                        ligne.Starting_Date = m.Starting_Date;
                        ligne.Ending_Date = m.Ending_Date;
                        ligne.Status = m.Status;
                        ligne.Editable_Status = m.Editable_Status;
                        //YSA-DO add Bool Selected into API page
                        //ligne.Selected = false;
                        ligne.Language_Code = m.Language_Code;
                        matterLines.Add(ligne);
                    }
                }
            }
            catch (JsonException ex)
            {
                throw new ApplicationException(
                    "Erreur l'ors de l'execution de la requete.", ex);
            }

            return matterLines;
        }

        /// <summary>
        /// Permet de récupérer les affaires de l'utilisateurs
        /// </summary>
        /// <param name="over_write">Faut il écraser les valeurs stockées en session?</param>
        /// <returns></returns>
        public static IEnumerable<Matter> GetUserMatters(string Matter_No, string Matter_Name, string Sell_to_Customer_No, string Partner_Code)
        {
            //On initialise la liste
            List<Matter> userMatterLines = new List<Matter>();

            string IdToken = System.Web.HttpContext.Current.Session["IdToken_Ticket"].ToString();
            var httpClient = new System.Net.Http.HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", IdToken);

            const string environmentsUri = "https://api.businesscentral.dynamics.com/v2.0/d1176b80-8023-4b7a-adc8-99a22336517f/SBLawyer_TEST/api/sbconsulting/sblawyer/v1.0/mymatters?company=99a224b4-d558-ec11-bb7c-000d3a2b9aee";

            
            //On récupère les familles d'action de frais via le service Navision
            //Uri uri = new Uri(ConfigurationManager.AppSettings["NavisionODataUri"]);
            //var context = new NavServiceRef.NAV(uri);

            //string UsernameSession = System.Web.HttpContext.Current.Session["Username"].ToString();
            //string PasswordSession = System.Web.HttpContext.Current.Session["Password"].ToString();

            //context.Credentials = SBLawyerWebApplication.Libs.SBLawyerCredential.GetCredentialCache(UsernameSession, PasswordSession);

            //context.Credentials = CredentialCache.DefaultNetworkCredentials;

            //DataServiceQueryContinuation<NavServiceRef.MyMatters> token = null;

            try
            {
                //On prépare les filtres
                string filters = "";

                if (!String.IsNullOrEmpty(Matter_No))
                    filters += "Matter_No eq '" + Matter_No + "'";

                if (!String.IsNullOrEmpty(Matter_Name))
                {
                    if (!String.IsNullOrEmpty(filters))
                        filters += "&";
                    filters += "Description eq '" + Matter_Name + "'";
                }

                if (!String.IsNullOrEmpty(Sell_to_Customer_No))
                {
                    if (!String.IsNullOrEmpty(filters))
                        filters += "&";
                    filters += "Sell_to_Customer_No eq '" + Sell_to_Customer_No + "'";
                }

                if (!String.IsNullOrEmpty(Partner_Code))
                {
                    if (!String.IsNullOrEmpty(filters))
                        filters += "&";
                    filters += " Partner_No eq '" + Partner_Code + "'";
                }

                //On créé la requete de service OData pour le type d'objet
                //DataServiceQuery userMattersDataServiceQuery = String.IsNullOrEmpty(filters) ? context.MyMatters as DataServiceQuery : context.MyMatters.AddQueryOption("$filter", filters) as DataServiceQuery;

                //On récupèrea réponse du serveur
                //QueryOperationResponse<NavServiceRef.MyMatters> response = (QueryOperationResponse<NavServiceRef.MyMatters>)context.Execute<NavServiceRef.MyMatters>(new Uri(userMattersDataServiceQuery.ToString()));

                var response = httpClient.GetAsync(environmentsUri + filters).Result;
                var content = response.Content.ReadAsStringAsync().Result;
                //On récupère le schéma de réponse
                var result = JsonConvert.DeserializeObject<MatterRoot>(content);
                //Pour chaque réponse 
                //do
                //{
                    //Si le nextLink n'est pas null, il y a d'autre page de données.
                    //if (token != null)
                    //{
                        //On récupère la page de données suivante
                        //response = context.Execute<NavServiceRef.MyMatters>(token) as QueryOperationResponse<NavServiceRef.MyMatters>;
                    //}

                    //On traite la réponse
                    foreach (Matter m in result.Value)
                    {
                        Matter ligne = new Matter();

                        ligne.Matter_No = m.Matter_No;
                        ligne.Matter_Label = m.Description + " (" + m.Matter_No + ")";
                        ligne.Description = m.Description;
                        ligne.Partner_No = m.Partner_No;
                        ligne.Sell_to_Customer_No = m.Sell_to_Customer_No;
                        ligne.Editable_Status = m.Editable_Status;
                        ligne.Selected = false;

                        userMatterLines.Add(ligne);
                    }
                //}

                //On récupère la prochaine page et on continue tant qu'il y a une prochaine page
                //while ((token = response.GetContinuation()) != null);
            }
            catch (JsonException ex)
            {
                throw new ApplicationException(
                    "Erreur l'ors de l'execution de la requete OData.", ex);
            }

            return userMatterLines;
        }

        public static Matter updateUserMatter(Matter userMatter)
        {
            if (userMatter.Selected)
            {
                Uri uri = new Uri(ConfigurationManager.AppSettings["NavisionODataUri"]);
                var NAV = new NavServiceRef.NAV(uri);

                string UsernameSession = System.Web.HttpContext.Current.Session["Username"].ToString();
                string PasswordSession = System.Web.HttpContext.Current.Session["Password"].ToString();

                NAV.Credentials = SBLawyerWebApplication.Libs.SBLawyerCredential.GetCredentialCache(UsernameSession, PasswordSession);
                //NAV.Credentials = CredentialCache.DefaultNetworkCredentials;

                var userMattersResponse = from matter in NAV.MyMatters
                                          where matter.Matter_No == userMatter.Matter_No
                                          select matter;

                NavServiceRef.MyMatters myMatter = userMattersResponse.First();

                NAV.DeleteObject(myMatter);
                NAV.SaveChanges();
            }

            return userMatter;
        }
        public static Matter updateAvailableMatter(Matter availableMatter)
        {
            Uri uri = new Uri(ConfigurationManager.AppSettings["NavisionODataUri"]);
            var NAV = new NavServiceRef.NAV(uri);

            string UsernameSession = System.Web.HttpContext.Current.Session["Username"].ToString();
            string PasswordSession = System.Web.HttpContext.Current.Session["Password"].ToString();

            NAV.Credentials = SBLawyerWebApplication.Libs.SBLawyerCredential.GetCredentialCache(UsernameSession, PasswordSession);
            //NAV.Credentials = CredentialCache.DefaultNetworkCredentials;

            if (availableMatter.Selected)
            {
                NAV.AddToMyMatters(new NavServiceRef.MyMatters() { User_ID = System.Web.HttpContext.Current.Session["Username"].ToString(), Matter_No = availableMatter.Matter_No });
                NAV.SaveChanges();
            }

            return availableMatter;
        }

        //YSA-DO
        //Add ==> CreateMatter, CreateUserMatter, updateMatter, updateUserMatter, deleteMatter, deleteUserMatter
    }
}