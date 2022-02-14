using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppModelv2_WebApp_OpenIDConnect_DotNet.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class CommentLine
    {
        public string tableName { get; set; }
        public string No { get; set; }
        public int Document_Line_No { get; set; }
        public int lineNo { get; set; }
        public string Matter_Family { get; set; }
        public int Matter_Entry_Type { get; set; }
        public string Comment { get; set; }
    }

    public class CommentLineRoot
    {
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }
        public List<CommentLine> Value { get; set; }
    }
    public static class CommentLineDataContext
    {
        /// <summary>
        /// Permet de récupérer les commentaires
        /// </summary>
        /// <param name="over_write">Faut il écraser les valeurs stockées en session?</param>
        /// <returns></returns>
        public static List<CommentLine> GetCommentLines(bool over_write)
        {
            //On récupère les valeurs en sessions
            IEnumerable<CommentLine> cmLines = (IEnumerable<CommentLine>)HttpContext.Current.Session["CommentLines"];
            
            //pour un souci de performance, on ne requete le serveur Navision que si l'on a pas de données ou si l'on a forcé l'appel du serveur
            if (cmLines == null || over_write)
            {
                //On initialise la liste
                List<CommentLine> commentLines = new List<CommentLine>();
                //On créé la requete de service OData pour le type d'objet
                string IdToken = System.Web.HttpContext.Current.Session["IdToken_Ticket"].ToString();
                var httpClient = new System.Net.Http.HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", IdToken);

                const string environmentsUri = "https://api.businesscentral.dynamics.com/v2.0/d1176b80-8023-4b7a-adc8-99a22336517f/SBLawyer_TEST/api/sbconsulting/sblawyer/v1.0/worktypes?company=99a224b4-d558-ec11-bb7c-000d3a2b9aee";

                var response = httpClient.GetAsync(environmentsUri).Result;
                var content = response.Content.ReadAsStringAsync().Result;
                //On récupère le schéma de réponse
                var result = JsonConvert.DeserializeObject<CommentLineRoot>(content);

                //On récupère les familles d'action de frais via le service Navision
                //Uri uri = new Uri(ConfigurationManager.AppSettings["NavisionODataUri"]);
                //var context = new NavServiceRef.NAV(uri);

                //context.Credentials = CredentialCache.DefaultNetworkCredentials;
                //context.Credentials = SBLawyerWebApplication.Libs.SBLawyerCredential.GetCredentialCache(UsernameSession, PasswordSession);

                //DataServiceQueryContinuation<NavServiceRef.CommentLineWS> token = null;

                try
                {
                    //On récupèrea réponse du serveur
                    //QueryOperationResponse<NavServiceRef.CommentLineWS> response = context.CommentLineWS.Execute() as QueryOperationResponse<NavServiceRef.CommentLineWS>;

                    //Pour chaque réponse 
                    //do
                    //{
                        //Si le nextLink n'est pas null, il y a d'autre page de données.
                        //if (token != null)
                        //{
                            //On récupère la page de données suivante
                            //response = context.Execute<NavServiceRef.CommentLineWS>(token) as QueryOperationResponse<NavServiceRef.CommentLineWS>;
                        //}

                        //On traite la réponse
                        foreach (CommentLine cl in result.Value)
                        {
                            CommentLine ligne = new CommentLine();

                            //ligne.Key = cl.No;
                            ligne.No = cl.No;
                            ligne.Matter_Family = cl.Matter_Family;
                            if (cl.Matter_Entry_Type != 0)
                                ligne.Matter_Entry_Type = cl.Matter_Entry_Type;
                            ligne.Comment = cl.Comment;

                            commentLines.Add(ligne);
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

                HttpContext.Current.Session["CommentLines"] = commentLines;
                cmLines = commentLines as IEnumerable<CommentLine>;
            }
            return cmLines.ToList();
        }

        /// <summary>
        /// Permet de récupérer la liste des lignes de commentaire en fonction de l'affaire
        /// </summary>
        /// <param name="Matter_No"></param>
        /// <returns>Liste de type d'action</returns>
        public static List<CommentLine> GetCommentLinesFromMatter(string Matter_No, int Matter_Entry_Type)
        {
            if (Matter_No != "" && Matter_Entry_Type != 0)
            {
                //On récupère les familles d'action de frais via le service Navision
                //Uri uri = new Uri(ConfigurationManager.AppSettings["NavisionODataUri"]);
                //var context = new NavServiceRef.NAV(uri);
                //string UsernameSession = System.Web.HttpContext.Current.Session["Username"].ToString();
                //string PasswordSession = System.Web.HttpContext.Current.Session["Password"].ToString();

                //context.Credentials = CredentialCache.DefaultNetworkCredentials;
                //context.Credentials = SBLawyerWebApplication.Libs.SBLawyerCredential.GetCredentialCache(UsernameSession, PasswordSession);

                //DataServiceQueryContinuation<NavServiceRef.CommentLineWS> token = null;

                try
                {

                    //On traite la réponse
                    //On récupère tous les commentaires
                    List<CommentLine> comments = new List<CommentLine>();
                    List<CommentLine> rawComments = new List<CommentLine>();
                    string IdToken = System.Web.HttpContext.Current.Session["IdToken_Ticket"].ToString();
                    var httpClient = new System.Net.Http.HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", IdToken);

                    const string environmentsUri = "https://api.businesscentral.dynamics.com/v2.0/d1176b80-8023-4b7a-adc8-99a22336517f/SBLawyer_TEST/api/sbconsulting/sblawyer/v1.0/worktypes?company=99a224b4-d558-ec11-bb7c-000d3a2b9aee";

                    var response = httpClient.GetAsync(environmentsUri).Result;
                    var content = response.Content.ReadAsStringAsync().Result;
                    //On récupère le schéma de réponse
                    var result = JsonConvert.DeserializeObject<CommentLineRoot>(content);
                    //On fusionne les commentaires en un seul
                    CommentLine comment = new CommentLine() { Comment = "" };

                    //YSA-DO Filter API URI
                    //On récupère la réponse du serveur
                    DataServiceQuery commentLinesDataServiceQuery = context.CommentLineWS.Where(cl => cl.No == Matter_No) as DataServiceQuery;
                    QueryOperationResponse<NavServiceRef.CommentLineWS> response = (QueryOperationResponse<NavServiceRef.CommentLineWS>)context.Execute<NavServiceRef.CommentLineWS>(new Uri(commentLinesDataServiceQuery.ToString()));

                    //Pour chaque réponse 
                    //do
                    //{
                        //Si le nextLink n'est pas null, il y a d'autre page de données.
                        //if (token != null)
                        //{
                            //On récupère la page de données suivante
                            //response = context.Execute<NavServiceRef.CommentLineWS>(token) as QueryOperationResponse<NavServiceRef.CommentLineWS>;
                        //}

                        //On construit les données au format SBLawyer WebApp
                        foreach (var cl in result.Value)
                        {
                            if (cl.Matter_Entry_Type == Matter_Entry_Type || cl.Matter_Entry_Type == 0)
                            {
                                rawComments.Add(cl);
                            }
                        }
                    //}

                    //On récupère la prochaine page et on continue tant qu'il y a une prochaine page
                    //while ((token = response.GetContinuation()) != null);

                    foreach (var c in rawComments)
                    {
                        comment.Comment += c.Comment + ". <br/>";
                    }

                    if (rawComments.Count > 0)
                        comments.Add(comment);

                    return comments;
                }
                catch (JsonException ex)
                {
                    throw new ApplicationException(
                        "Erreur l'ors de l'execution de la requete OData.", ex);
                }

            }
            else
                return new List<CommentLine>();

        }

        public static CommentLine createCommentLine()
        {
            return new CommentLine();
        }

        public static CommentLine updateCommentLine()
        {
            return new CommentLine();
        }

        public static CommentLine deleteCommentLine()
        {
            return new CommentLine();
        }
    }

}