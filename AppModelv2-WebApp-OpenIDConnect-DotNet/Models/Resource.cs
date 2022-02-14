using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppModelv2_WebApp_OpenIDConnect_DotNet.Models
{
    public class Resource
    {
        public string No { get; set; }
        public string Partner_Name { get; set; }
        public string Partner_Label { get; set; }
        public string Type { get; set; }
        public string Base_Unit_of_Measure { get; set; }
        public string Resource_Group_No { get; set; }
        public string Search_Name { get; set; }
        public string User_ID { get; set; }
        public string Lawyer_Type { get; set; }
        public int DefaultTimeFactor { get; set; }
    }

    public class ResourceRoot
    {
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }
        public List<Resource> Value { get; set; }
    }

    public static class ResourceDataContext
    {
        public static List<Resource> GetResources(bool over_write)
        {
            List<Resource> resourceResult = (List<Resource>)HttpContext.Current.Session["Resources"];

            if (resourceResult == null || over_write)
            {
                List<Resource> resourceLines = new List<Resource>();

                string IdToken = System.Web.HttpContext.Current.Session["IdToken_Ticket"].ToString();
                var httpClient = new System.Net.Http.HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", IdToken);

                const string environmentsUri = "https://api.businesscentral.dynamics.com/v2.0/d1176b80-8023-4b7a-adc8-99a22336517f/SBLawyer_TEST/api/sbconsulting/sblawyer/v1.0/companies(e34b1b2c-7f09-ec11-bb73-000d3a3926f4)/resources";

                try
                {
                    var response = httpClient.GetAsync(environmentsUri).Result;
                    var content = response.Content.ReadAsStringAsync().Result;

                    var result = JsonConvert.DeserializeObject<ResourceRoot>(content);

                    foreach (Resource r in result.Value)
                    {
                        Resource ligne = new Resource();

                        ligne.No = r.No;
                        ligne.No = r.No;
                        ligne.Partner_Name = r.Partner_Name;
                        ligne.Partner_Label = r.Partner_Name + " (" + r.No + ")";
                        ligne.Type = r.Type;
                        ligne.Base_Unit_of_Measure = r.Base_Unit_of_Measure;
                        ligne.User_ID = r.User_ID;
                        ligne.Lawyer_Type = r.Lawyer_Type;
                        if (r.DefaultTimeFactor != 0)
                            ligne.DefaultTimeFactor = r.DefaultTimeFactor;

                        resourceLines.Add(ligne);
                    }
                }
                catch (JsonException ex)
                {
                    throw new ApplicationException(
                    "Erreur l'ors de l'execution de la requete.", ex);
                }

                HttpContext.Current.Session["Resources"] = resourceResult = resourceLines;
            }
            return resourceResult;
        }

        public static List<Resource> GetAssociates(bool over_write)
        {
            List<Resource> associatesLines = new List<Resource>();

            //#warning Attention à la langue
            //return GetResources().Where(r => r.Lawyer_Type == "Partner");
            foreach (Resource r in GetResources(over_write).Where(r => r.Lawyer_Type == "Associé"))
            {
                associatesLines.Add(r);
            }

            return associatesLines;
        }

        /// <summary>
        /// Permet de récupérer l'unité de mesure de l'utilisateur
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="over_write">Faut il ecraser les données de la session?</param>
        /// <returns></returns>
        public static string GetBaseUnitOfMeasure(string userId, bool over_write)
        {
            return GetResources(over_write).Where(r => r.User_ID == userId).First().Base_Unit_of_Measure;
        }

        /// <summary>
        /// Permet de récupérer le multiplicateur d'unité de mesure pour convertir la quantité en milisecondes
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="over_write">Faut il ecraser les données de la session?</param>
        /// <returns></returns>
        public static int GetDefaultTimeFactor(string userId, bool over_write)
        {
            return GetResources(over_write).Where(r => r.User_ID == userId).First().DefaultTimeFactor;
        }

        public static Resource createResource()
        {
            return new Resource();
        }

        public static Resource updateResource()
        {
            return new Resource();
        }

        public static Resource deleteResource()
        {
            return new Resource();
        }
    }
}