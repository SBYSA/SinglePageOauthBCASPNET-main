using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace AppModelv2_WebApp_OpenIDConnect_DotNet.Models
{
    public class WorkType
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string Matter_Category { get; set; }
        public string Unit_of_Measure_Code { get; set; }
        public bool Non_Billable { get; set; }
        public bool By_Default { get; set; }
    }

    public class WorkTypeRoot
    {
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }
        public List<WorkType> Value { get; set; }
    }

    public class PreFillRoot
    {
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }
        public bool Value { get; set; }
    }

    /// <summary>
    /// DataContext des types d'action, permet d'effectuer les opération CRUD
    /// </summary>
    public static class WorkTypeDataContext
    {
        /// <summary>
        /// Permet de récupérer les worktypes de la saisie des temps
        /// </summary>
        /// <param name="over_write">Faut il écraser les valeurs stockées en session?</param>
        /// <returns></returns>
        public static IEnumerable<WorkType> GetWorkTypes(bool over_write)
        {
            return GetWorkTypes(over_write, code_langue: "");
        }

        /// <summary>
        /// Permet de récupérer les worktypes de la saisie des temps avec le code langue pour la traduction
        /// </summary>
        /// <param name="over_write">Faut il écraser les valeurs stockées en session?</param>
        /// <param name="code_langue">option: code langue</param>
        public static IEnumerable<WorkType> GetWorkTypes(bool over_write, string code_langue)
        {
            //On récupère les valeurs en sessions
            IEnumerable<WorkType> SessionWT = (List<WorkType>)HttpContext.Current.Session["WorkTypes"] as IEnumerable<WorkType>;

            //pour un souci de performance, on ne requete le serveur Navision que si l'on a pas de données ou si l'on a forcé l'appel du serveur
            if (SessionWT == null || over_write)
            {
                //On initialise la liste
                List<WorkType> workTypeLines = new List<WorkType>();
                try
                {
                    //Construction du lien API
                    string BaseURL = ConfigurationManager.AppSettings["BaseURL"];
                    string WorkTypeAPI = ConfigurationManager.AppSettings["worktypes"];
                    string BCCompany = ConfigurationManager.AppSettings["BCCompany"];
                    
                    //Récupération du Token si existe
                    string IdToken = System.Web.HttpContext.Current.Session["IdToken_Ticket"].ToString();
                    var httpClient = new System.Net.Http.HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", IdToken);

                    string environmentsUri = BaseURL + WorkTypeAPI + BCCompany;

                    var response = httpClient.GetAsync(environmentsUri).Result;
                    var content = response.Content.ReadAsStringAsync().Result;
                    //On récupère le schéma de réponse
                    var result = JsonConvert.DeserializeObject<WorkTypeRoot>(content);

                    //YSA-DO Filter API URI



                    //Pour chaque réponse 
                    //do
                    //{
                    //Si le nextLink n'est pas null, il y a d'autre page de données.
                    //if (token != null)
                    //{
                    //On récupère la page de données suivante
                    //response = context.Execute<NavServiceRef.WorkType>(token) as QueryOperationResponse<NavServiceRef.WorkType>;
                    //}

                    //On traite la réponse
                    foreach (WorkType w in result.Value)
                        {
                            WorkType ligne = new WorkType();

                            ligne.Code = w.Code;
                            //ligne.Description = w.By_Default

                            if (idPreFill == true)
                            {
                                if (code_langue == "")
                                {
                                    ligne.Description = w.Description;
                                }
                                else
                                {


                                    string responseDescription = webService.GetWorktypeTranslation(ligne.Code, code_langue);

                                    if (responseDescription == "")
                                    {
                                        ligne.Description = w.Description;
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


                            ligne.Matter_Category = w.Matter_Category;
                            workTypeLines.Add(ligne);
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

                HttpContext.Current.Session["WorkTypes"] = workTypeLines;
                SessionWT = workTypeLines as IEnumerable<WorkType>;
            }

            return SessionWT;
        }

        /// <summary>
        /// Permet de récupérer la liste des type d'action de temps en fonction de l'affaire
        /// </summary>
        /// <param name="Matter_No"></param>
        /// <returns>Liste de type d'action</returns>
        public static IEnumerable<WorkType> GetWorkTypesFromMatter(string Matter_No)
        {
            List<WorkType> GetWorkTypeList = new List<WorkType>();
            if (Matter_No != "")
            {
                string matter_Category = MatterDataContext.GetMattersDetails(Matter_No).Matter_Category;
                string code_langue = MatterDataContext.GetMattersDetails(Matter_No).Language_Code;

                if (code_langue != "")
                {
                    return GetWorkTypes(true, code_langue).Where(w => w.Matter_Category == "" || w.Matter_Category == matter_Category);
                }
                else
                {
                    return GetWorkTypes(false).Where(w => w.Matter_Category == "" || w.Matter_Category == matter_Category);
                }
            }
            else
                return GetWorkTypes(false);

        }

        /// <summary>
        /// Permet de récupérer les worktypes de la saisie des temps avec le code langue pour la traduction
        /// </summary>
        /// <param name="Matter_No">Numéro dossier pour recupérer code langue</param>
        /// <returns></returns>
        public static List<WorkType> GetWorkTypeByDefaut(string Matter_No)
        {
            //On initialise la liste
            List<WorkType> Worktype = new List<WorkType>();

            string IdToken = System.Web.HttpContext.Current.Session["IdToken_Ticket"].ToString();
            var httpClient = new System.Net.Http.HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", IdToken);

            const string environmentsUri = "https://api.businesscentral.dynamics.com/v2.0/d1176b80-8023-4b7a-adc8-99a22336517f/SBLawyer_TEST/api/sbconsulting/sblawyer/v1.0/companies(e34b1b2c-7f09-ec11-bb73-000d3a3926f4)/worktypes";

            //on recupere le code langue du dossier
            string code_langue = MatterDataContext.GetMattersDetails(Matter_No).codeLangue;

            const string envUri = "https://api.businesscentral.dynamics.com/v2.0/d1176b80-8023-4b7a-adc8-99a22336517f/sblawyer_test/ODataV4/MatterFunction_GetPreFillExpenseJnlLine?company=99a224b4-d558-ec11-bb7c-000d3a2b9aee";
            var resp = httpClient.GetAsync(envUri).Result;
            var contentResp = resp.Content.ReadAsStringAsync().Result;

            var webService = JsonConvert.DeserializeObject<PreFillRoot>(contentResp);
            var idPreFill = webService.Value;
            //var idPreFill = true;
            //YSA-DO Filter API URI
            try
            {
                //YSA-DO Filter On By_Default => true
                // On trie la liste des Worktype recupérer via Ws pour recupérer celui par defaut
                //On le met dans une liste IEnumerable pour recupérer la propriété count 
                //IEnumerable<NavServiceRef.WorkType> response = from w in context.WorkType where (bool)w.By_Default == true select w;

                var response = httpClient.GetAsync(environmentsUri).Result;
                var content = response.Content.ReadAsStringAsync().Result;

                var result = JsonConvert.DeserializeObject<WorkTypeRoot>(content);

                //si je recupere un WT par defaut 
                if (result.Value.Count() != 0)
                {

                    foreach (WorkType w in result.Value)
                    {

                        WorkType ligne = new WorkType();
                        ligne.Code = w.Code;

                        ligne.By_Default = (bool)w.By_Default;

                        const string envUri2 = "https://api.businesscentral.dynamics.com/v2.0/d1176b80-8023-4b7a-adc8-99a22336517f/sblawyer_test/ODataV4/MatterFunction_GetWorktypeTranslation?company=99a224b4-d558-ec11-bb7c-000d3a2b9aee";
                        resp = httpClient.GetAsync(envUri2).Result;
                        contentResp = resp.Content.ReadAsStringAsync().Result;

                        webService = JsonConvert.DeserializeObject<PreFillRoot>(contentResp);
                        //idPreFill = webService.Value;

                        //YSA-DO Define Root
                        //get traduction description via WS
                        string responseDescription = webService.Value;

                        if (idPreFill == true)
                        {

                            if (responseDescription == "")
                            {
                                ligne.Description = w.Description;
                            }
                            else
                            {
                                ligne.Description = responseDescription;
                            }

                        }
                        else
                        {
                            ligne.Description = "";
                        }



                        ligne.Matter_Category = w.Matter_Category;

                        Worktype.Add(ligne);
                    }
                }


            }
            catch (JsonException ex)
            {
                throw new ApplicationException(
                    "Erreur l'ors de l'execution de la requete OData.", ex);
            }
            return Worktype;
        }

        /// <summary>
        /// Verifie si un worktype par defaut existe dans Nav
        /// </summary>
        /// <returns>boolean</returns>
        public static bool checkDefautWorktype()
        {
            //On initialise a liste
            List<WorkType> matterLineLines = new List<WorkType>();
            string IdToken = System.Web.HttpContext.Current.Session["IdToken_Ticket"].ToString();
            var httpClient = new System.Net.Http.HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", IdToken);

            const string environmentsUri = "https://api.businesscentral.dynamics.com/v2.0/d1176b80-8023-4b7a-adc8-99a22336517f/SBLawyer_TEST/api/sbconsulting/sblawyer/v1.0/companies(e34b1b2c-7f09-ec11-bb73-000d3a3926f4)/worktypes";


            try
            {
                //YSA-DO Filter to ByDefault = true
                // On trie la liste des Worktype recupérer via Ws pour recupérer celui par defaut
                //IEnumerable<NavServiceRef.WorkType> response = from w in context.WorkType where (bool)w.By_Default == true select w;
                var response = httpClient.GetAsync(environmentsUri).Result;
                var content = response.Content.ReadAsStringAsync().Result;

                var result = JsonConvert.DeserializeObject<WorkTypeRoot>(content);
                //DataServiceQuery workTypesDataServiceQuery = context.WorkType as DataServiceQuery;



                if (result.Value.Count() == 0)
                {

                    return false;

                }
                else
                {
                    return true;
                }


            }
            catch (JsonException ex)
            {
                throw new ApplicationException(
                    "Erreur l'ors de l'execution de la requete OData.", ex);
            }

        }
        public static WorkType createWorkType()
        {
            return new WorkType();
        }

        public static WorkType updateWorkType()
        {
            return new WorkType();
        }

        public static WorkType deleteWorkType()
        {
            return new WorkType();
        }
    }
}