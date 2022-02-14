using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace AppModelv2_WebApp_OpenIDConnect_DotNet.Models
{
    public class MatterLine
    {
        public string Matter_No { get; set; }
        public int Line_No { get; set; }
        public string Description { get; set; }
        public string Matter_Line_Label { get; set; }
        public int Matter_Entry_Type { get; set; }
    }

    public class MatterLineRoot
    {
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }
        public List<MatterLine> Value { get; set; }
    }

    public static class MatterLineDataContext
    {
        /// <summary>
        /// Permet de récupérer les lignes d'affaires pour les lignes de temps
        /// </summary>
        /// <param name="Matter_No">Numéro d'affaire parent</param>
        /// <returns>Liste de ligne d'affaire de temps</returns>
        public static List<MatterLine> GetTimeMatterMatterLines(string Matter_No)
        {
            //On initialise a liste
            List<MatterLine> matterLineLines = new List<MatterLine>();
            string IdToken = System.Web.HttpContext.Current.Session["IdToken_Ticket"].ToString();
            var httpClient = new System.Net.Http.HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", IdToken);

            const string environmentsUri = "https://api.businesscentral.dynamics.com/v2.0/d1176b80-8023-4b7a-adc8-99a22336517f/SBLawyer_TEST/api/sbconsulting/sblawyer/v1.0/companies(e34b1b2c-7f09-ec11-bb73-000d3a3926f4)/matterlines";

            try
            {
                //YSA-DO => Filter for ml.Matter_No == Matter_No
                //On créé la requete de service OData pour le type d'objet
                //DataServiceQuery matterLinesDataServiceQuery = context.MatterLines
                //    .Where(ml =>
                //        ml.Matter_No == Matter_No
                //    //&& ml.Matter_Entry_Type == 1 //On veut des temps
                //    )
                //    as DataServiceQuery;

                var response = httpClient.GetAsync(environmentsUri).Result;
                var content = response.Content.ReadAsStringAsync().Result;

                var result = JsonConvert.DeserializeObject<MatterLineRoot>(content);

                //On traite la réponse
                foreach (MatterLine ml in result.Value)
                {
                    if (ml.Matter_Entry_Type == 1) //On veut des temps
                    {
                        MatterLine ligne = new MatterLine();

                        ligne.Matter_No = ml.Matter_No;
                        ligne.Line_No = ml.Line_No;
                        ligne.Description = ml.Description;
                        ligne.Matter_Line_Label = ml.Matter_Line_Label;
                        if (ml.Matter_Entry_Type != 0)
                            ligne.Matter_Entry_Type = ml.Matter_Entry_Type;

                        matterLineLines.Add(ligne);
                    }
                }
            }
            catch (JsonException ex)
            {
                throw new ApplicationException("Erreur l'ors de l'execution de la requete.", ex);
            }

            return matterLineLines;
        }

        /// <summary>
        /// Permet de récupérer les lignes d'affaires pour les lignes de frais
        /// </summary>
        /// <param name="Matter_No">Numéro d'affaire parent</param>
        /// <returns>Liste de ligne d'affaire de frais</returns>
        public static List<MatterLine> GetExpenseMatterMatterLines(string Matter_No)
        {
            //On initialise a liste
            List<MatterLine> matterLineLines = new List<MatterLine>();
            string IdToken = System.Web.HttpContext.Current.Session["IdToken_Ticket"].ToString();
            var httpClient = new System.Net.Http.HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", IdToken);

            const string environmentsUri = "https://api.businesscentral.dynamics.com/v2.0/d1176b80-8023-4b7a-adc8-99a22336517f/SBLawyer_TEST/api/sbconsulting/sblawyer/v1.0/companies(e34b1b2c-7f09-ec11-bb73-000d3a3926f4)/matterlines";
            try
            {
                //YSA-DO => Filter for ml.Matter_No == Matter_No
                //On créé la requete de service OData pour le type d'objet
                //DataServiceQuery matterLinesDataServiceQuery = context.MatterLines
                //    .Where(ml =>
                //        ml.Matter_No == Matter_No
                //    //&& ml.Matter_Entry_Type == 1 //On veut des temps
                //    )
                //    as DataServiceQuery;

                var response = httpClient.GetAsync(environmentsUri).Result;
                var content = response.Content.ReadAsStringAsync().Result;

                var result = JsonConvert.DeserializeObject<MatterLineRoot>(content);

                //On traite la réponse
                foreach (MatterLine ml in result.Value)
                {
                    if (ml.Matter_Entry_Type == 2 || ml.Matter_Entry_Type == 3)
                    {
                        MatterLine ligne = new MatterLine();

                        ligne.Matter_No = ml.Matter_No;
                        ligne.Line_No = ml.Line_No;
                        ligne.Description = ml.Description;
                        ligne.Matter_Line_Label = ml.Matter_Line_Label;

                        if (ml.Matter_Entry_Type != 0)
                            ligne.Matter_Entry_Type = ml.Matter_Entry_Type;

                        matterLineLines.Add(ligne);
                    }
                }
            }
            catch (JsonException ex)
            {
                throw new ApplicationException("Erreur l'ors de l'execution de la requete.", ex);
            }

            return matterLineLines;
        }


        public static MatterLine createMatterLine()
        {
            return new MatterLine();
        }

        public static MatterLine updateMatterLine()
        {
            return new MatterLine();
        }

        public static MatterLine deleteMatterLine()
        {
            return new MatterLine();
        }
    }

}