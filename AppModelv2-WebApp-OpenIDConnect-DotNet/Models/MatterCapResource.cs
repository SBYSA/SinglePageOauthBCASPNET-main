using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppModelv2_WebApp_OpenIDConnect_DotNet.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class MatterCapResource
    {
        [JsonProperty("@odata.etag")]
        public string OdataEtag { get; set; }
        public string periodType { get; set; }
        public string Period_Start { get; set; }
        public int Period_No { get; set; }
        public string Period_Name { get; set; }
        public bool bNonworking_g { get; set; }
        public string Description { get; set; }
        public int decQtyChargeable_g { get; set; }
        public int decQtyNoChargeable_g { get; set; }
        public int decQtyInternal_g { get; set; }
        public int decQtyAvailableDay_g { get; set; }
        public int Series_Quantity_1 { get; set; }
        public int Series_Quantity_2 { get; set; }
        public int Series_Quantity_3 { get; set; }
        public int Series_Quantity_4 { get; set; }
        public int Series_Quantity_5 { get; set; }
        public int Series_Quantity_6 { get; set; }
    }

    public class MatterCapResourceRoot
    {
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }
        public List<MatterCapResource> Value { get; set; }
    }

    public class MatterCapResourceStat
    {
        public List<MatterCapResourceTypedStat> StatsList { get; set; }
        public List<MatterCapResource> MatterCapResources { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal Serie1Quantity { get; set; }
        public decimal Serie2Quantity { get; set; }
        public decimal Serie3Quantity { get; set; }
        public decimal Serie4Quantity { get; set; }
        public decimal Serie5Quantity { get; set; }
        public decimal Serie6Quantity { get; set; }
        public decimal OccupationRate { get; set; }
        public double OccupationHour { get; set; }
        public string OccupationHourTime { get; set; }
        public decimal AvailableQuantity { get; set; }
    }
    public class MatterCapResourceTypedStat
    {
        public decimal Quantity { get; set; }
        public decimal AvailableQuantity { get; set; }
        public decimal QuantityPercent { get; set; }
        public string Description { get; set; }
    }

    public static class MatterCapResourceDataContext
    {
        public static MatterCapResource createMatterCapResource()
        {
            return new MatterCapResource();
        }

        /// <summary>
        /// Permet de récupérer la liste des capacités de ressources
        /// </summary>
        /// <param name="beginDate"></param>
        /// <param name="endate"></param>
        /// <returns>Liste de capacité de ressource</returns>
        public static List<MatterCapResource> readMatterCapResources(DateTime startDate, DateTime endDate, string filterMode, bool overwrite)
        {
            List<MatterCapResource> result = (List<MatterCapResource>)HttpContext.Current.Session["MatterCapResources"];
            DateTime actualStartDate;
            DateTime actualEndDate;
            string actualFilterMode;


            if (HttpContext.Current.Session["actualStartDate"] != null)
                actualStartDate = (DateTime)HttpContext.Current.Session["actualStartDate"];
            else
                actualStartDate = new DateTime();

            if (HttpContext.Current.Session["actualEndDate"] != null)
                actualEndDate = (DateTime)HttpContext.Current.Session["actualEndDate"];
            else
                actualEndDate = new DateTime();

            if (HttpContext.Current.Session["actualFilterMode"] != null)
                actualFilterMode = (string)HttpContext.Current.Session["actualFilterMode"];
            else
                actualFilterMode = filterMode;

            if (result == null || startDate != actualStartDate || endDate != actualEndDate || filterMode != actualFilterMode || overwrite)
            {
                HttpContext.Current.Session["actualStartDate"] = startDate;
                HttpContext.Current.Session["actualEndDate"] = endDate;
                HttpContext.Current.Session["actualFilterMode"] = filterMode;

                Uri uri = new Uri(ConfigurationManager.AppSettings["NavisionODataUri"]);
                var NAV = new NavServiceRef.NAV(uri);

                string UsernameSession = System.Web.HttpContext.Current.Session["Username"].ToString();
                string PasswordSession = System.Web.HttpContext.Current.Session["Password"].ToString();

                NAV.Credentials = SBLawyerWebApplication.Libs.SBLawyerCredential.GetCredentialCache(UsernameSession, PasswordSession);
                //NAV.Credentials = CredentialCache.DefaultNetworkCredentials;

                var matterCapResourceWSList = from mcr in NAV.MatterCapResourceWS
                                              where mcr.Period_Start >= startDate
                                              && mcr.Period_Start <= endDate
                                              select mcr;

                //On construit les données au format SBLawyer WebApp
                List<MatterCapResource> MatterCapResources = new List<MatterCapResource>();

                foreach (var mcr in matterCapResourceWSList)
                {
                    MatterCapResource ligne = new MatterCapResource();

                    ligne.bNonworking_g = mcr.bNonworking_g;
                    ligne.decQtyAvailableDay_g = mcr.decQtyAvailableDay_g;
                    ligne.Series_Quantity_1 = mcr.Series_Quantity_1.HasValue ? mcr.Series_Quantity_1.Value : 0;
                    ligne.Series_Quantity_2 = mcr.Series_Quantity_2.HasValue ? mcr.Series_Quantity_2.Value : 0;
                    ligne.Series_Quantity_3 = mcr.Series_Quantity_3.HasValue ? mcr.Series_Quantity_3.Value : 0;
                    ligne.Series_Quantity_4 = mcr.Series_Quantity_4.HasValue ? mcr.Series_Quantity_4.Value : 0;
                    ligne.Series_Quantity_5 = mcr.Series_Quantity_5.HasValue ? mcr.Series_Quantity_5.Value : 0;
                    ligne.Series_Quantity_6 = mcr.Series_Quantity_6.HasValue ? mcr.Series_Quantity_6.Value : 0;
                    ligne.Description = mcr.Description;
                    ligne.OdataEtag = mcr.ETag;
                    ligne.Period_Name = mcr.Period_Start.ToString("dddd dd");
                    ligne.Period_No = mcr.Period_No;
                    ligne.Period_Start = mcr.Period_Start;
                    ligne.periodType = mcr.Period_Type;

                    MatterCapResources.Add(ligne);
                }

                List<MatterCapResource> aggregatedMatterCapResource = new List<MatterCapResource>();

                switch (filterMode)
                {
                    case "week":
                        aggregatedMatterCapResource = MatterCapResources;
                        break;
                    case "month":
                        aggregatedMatterCapResource = MatterCapResources;
                        break;
                    case "year":
                        for (int i = 1; i <= 12; i++)
                            aggregatedMatterCapResource.Add(new MatterCapResource() { Period_Start = new DateTime(startDate.Year, i, 1), decQtyAvailableDay_g = 0, decQtySerie1_g = 0, decQtySerie2_g = 0, decQtySerie3_g = 0, Period_Name = new DateTime(startDate.Year, i, 1).ToString("MMMM") });

                        foreach (MatterCapResource mcr in MatterCapResources)
                        {
                            MatterCapResource mcrMonth = aggregatedMatterCapResource.ElementAt(mcr.Period_Start.Value.Month - 1);
                            mcrMonth.decQtyAvailableDay_g += mcr.decQtyAvailableDay_g;
                            mcrMonth.Series_Quantity_1 += mcr.Series_Quantity_1;
                            mcrMonth.Series_Quantity_2 += mcr.Series_Quantity_2;
                            mcrMonth.Series_Quantity_3 += mcr.Series_Quantity_3;
                            mcrMonth.Series_Quantity_4 += mcr.Series_Quantity_4;
                            mcrMonth.Series_Quantity_5 += mcr.Series_Quantity_5;
                            mcrMonth.Series_Quantity_6 += mcr.Series_Quantity_6;
                        }
                        break;
                }

                HttpContext.Current.Session["MatterCapResources"] = result = aggregatedMatterCapResource;
            }

            return result;
        }

        /// <summary>
        /// Récupère les statistiques des capacité de ressource des affaires
        /// </summary>
        /// <param name="startDate">date de début de filtre</param>
        /// <param name="endDate">date de fin de filtre</param>
        /// <returns>statistiques sous forme de liste</returns>
        public static MatterCapResourceStat readMatterCapResourcesStat(DateTime startDate, DateTime endDate, string filterMode, bool overwrite)
        {
            //Liste brute
            List<MatterCapResource> matterCapResources = readMatterCapResources(startDate, endDate, filterMode, overwrite);

            //Listes de statistiques

            //ma liste de categories graph color
            List<MatterCapResourceTypedStat> statSerie = new List<MatterCapResourceTypedStat>();

            if (System.Web.HttpContext.Current.Session["Fullname"] != null)
            {
                string UsernameSession = System.Web.HttpContext.Current.Session["Username"].ToString();
                string IdToekn = System.Web.HttpContext.Current.Session["IdToken_Ticket"].ToString();

                var httpClient = new System.Net.Http.HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", IdToekn);

                const string environmentsUri = "https://api.businesscentral.dynamics.com/v2.0/d1176b80-8023-4b7a-adc8-99a22336517f/SBLawyer_DEV/api/SBConsulting/SBLawyer/v1.0/companies(cd2a8812-f4be-eb11-9f0a-000d3ae75274)/graphicseries";

                var response = httpClient.GetAsync(environmentsUri).Result;
                var content = response.Content.ReadAsStringAsync().Result;
                MatterCapResourceStat statistics = new MatterCapResourceStat() { StatsList = new List<MatterCapResourceTypedStat>(), MatterCapResources = matterCapResources, Serie1Quantity = 0, Serie2Quantity = 0, Serie3Quantity = 0, Serie4Quantity = 0, Serie5Quantity = 0, Serie6Quantity = 0, TotalQuantity = 0, AvailableQuantity = 0, OccupationRate = 0, OccupationHour = 0 };
                if (response.IsSuccessStatusCode)
                {
                    var RootObject = JsonConvert.DeserializeObject<MatterCapResourceRoot>(content);
                    List<MatterCapResource> MatterCapResource = RootObject.Value;

                    var gss = from Gss in MatterCapResource
                              select Gss;

                    foreach (var Gss in gss)
                    {

                        //System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName
                        MatterCapResourceTypedStat ligne = new MatterCapResourceTypedStat();

                        if (System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == "fr")
                        {
                            ligne.Description = Gss.Serie_Name;
                        }
                        if (System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == "en")
                        {
                            ligne.Description = Gss.Serie_Name_ENU;
                        }
                        ligne.Quantity = 0;
                        ligne.QuantityPercent = 0;

                        statSerie.Add(ligne);


                    }


                    //On récupère les charges et le temps disponible sur la journée
                    decimal decTotalQuantity = decimal.Zero;

                    foreach (MatterCapResource mcr in matterCapResources)
                    {

                        if (statSerie.Count >= 1)
                            statSerie[0].Quantity += mcr.Series_Quantity_1;
                        if (statSerie.Count >= 2)
                            statSerie[1].Quantity += mcr.Series_Quantity_2;
                        if (statSerie.Count >= 3)
                            statSerie[2].Quantity += mcr.Series_Quantity_3;
                        if (statSerie.Count >= 4)
                            statSerie[3].Quantity += mcr.Series_Quantity_4;
                        if (statSerie.Count >= 5)
                            statSerie[4].Quantity += mcr.Series_Quantity_5;
                        if (statSerie.Count >= 6)
                            statSerie[5].Quantity += mcr.Series_Quantity_6;

                        decTotalQuantity += (mcr.Series_Quantity_1 + mcr.Series_Quantity_2+ mcr.Series_Quantity_3+ mcr.Series_Quantity_4+ mcr.Series_Quantity_5 + mcr.Series_Quantity_6);



                        statistics.AvailableQuantity += mcr.decQtyAvailableDay_g;//Before => statistics.AvailableQuantity += mcr.decQtyAvailableDay_g.HasValue ? mcr.decQtyAvailableDay_g.Value : 0;
                    }
                    if (decTotalQuantity != 0)
                    {
                        if (statSerie.Count >= 1)
                            statSerie[0].QuantityPercent = Math.Round(statSerie[0].Quantity / (decTotalQuantity) * 100, 2);
                        if (statSerie.Count >= 2)
                            statSerie[1].QuantityPercent = Math.Round(statSerie[1].Quantity / (decTotalQuantity) * 100, 2);
                        if (statSerie.Count >= 3)
                            statSerie[2].QuantityPercent = Math.Round(statSerie[2].Quantity / (decTotalQuantity) * 100, 2);
                        if (statSerie.Count >= 4)
                            statSerie[3].QuantityPercent = Math.Round(statSerie[3].Quantity / (decTotalQuantity) * 100, 2);
                        if (statSerie.Count >= 5)
                            statSerie[4].QuantityPercent = Math.Round(statSerie[4].Quantity / (decTotalQuantity) * 100, 2);
                        if (statSerie.Count >= 6)
                            statSerie[5].QuantityPercent = Math.Round(statSerie[5].Quantity / (decTotalQuantity) * 100, 2);
                    }

                    statistics.StatsList = statSerie;

                    if (statSerie.Count >= 1)
                        statistics.Serie1Quantity = Math.Round(statSerie[0].Quantity, 2);
                    if (statSerie.Count >= 2)
                        statistics.Serie2Quantity = Math.Round(statSerie[1].Quantity, 2);
                    if (statSerie.Count >= 3)
                        statistics.Serie3Quantity = Math.Round(statSerie[2].Quantity, 2);
                    if (statSerie.Count >= 4)
                        statistics.Serie4Quantity = Math.Round(statSerie[3].Quantity, 2);
                    if (statSerie.Count >= 5)
                        statistics.Serie5Quantity = Math.Round(statSerie[4].Quantity, 2);
                    if (statSerie.Count >= 6)
                        statistics.Serie6Quantity = Math.Round(statSerie[5].Quantity, 2);

                    statistics.TotalQuantity = Math.Round(decTotalQuantity, 2);
                    //Pourcentage d'occupation sur la période
                    statistics.OccupationRate = statistics.AvailableQuantity != 0 ? Math.Round(statistics.TotalQuantity / statistics.AvailableQuantity * 100, 2) : 0;

                    MatterFuncWebRef.MatterFunction webService = new MatterFuncWebRef.MatterFunction();
                    //webService.Credentials = SBLawyerWebApplication.Libs.SBLawyerCredential.GetCredentialCache(UsernameSession, PasswordSession);
                    webService.Url = ConfigurationManager.AppSettings["NavisionMatterFunctionUrl"];

                    // La culture vietnamienne.
                    CultureInfo viVn = new CultureInfo("vi-VN");

                }
                return statistics;
            }
            else
            {



                Uri uri = new Uri(ConfigurationManager.AppSettings["NavisionODataUri"]);
                var NAV = new NavServiceRef.NAV(uri);

                string UsernameSession = System.Web.HttpContext.Current.Session["Username"].ToString();
                string PasswordSession = System.Web.HttpContext.Current.Session["Password"].ToString();

                NAV.Credentials = SBLawyerWebApplication.Libs.SBLawyerCredential.GetCredentialCache(UsernameSession, PasswordSession);

                //NAV.Credentials = CredentialCache.DefaultNetworkCredentials;

                var gss = from Gss in NAV.GraphicSeriesSetup
                          select Gss;

                //On construit les données au format SBLawyer WebApp


                foreach (var Gss in gss)
                {

                    //System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName
                    MatterCapResourceTypedStat ligne = new MatterCapResourceTypedStat();

                    if (System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == "fr")
                    {
                        ligne.Description = Gss.Serie_Name;
                    }
                    if (System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == "en")
                    {
                        ligne.Description = Gss.Serie_Name_ENU;
                    }
                    ligne.Quantity = 0;
                    ligne.QuantityPercent = 0;

                    statSerie.Add(ligne);


                }


                //MatterCapResourceTypedStat statSerie1 = new MatterCapResourceTypedStat() { Description = Resources.Charts.chargeable, Quantity = 0, QuantityPercent = 0 };
                //MatterCapResourceTypedStat statSerie2 = new MatterCapResourceTypedStat() { Description = Resources.Charts.notChargeable, Quantity = 0, QuantityPercent = 0 };
                //MatterCapResourceTypedStat statSerie3 = new MatterCapResourceTypedStat() { Description = Resources.Charts.intern, Quantity = 0, QuantityPercent = 0 };
                //MatterCapResourceTypedStat statSerie4 = new MatterCapResourceTypedStat() { Description = Resources.Charts.chargeable, Quantity = 0, QuantityPercent = 0 };
                //MatterCapResourceTypedStat statSerie5 = new MatterCapResourceTypedStat() { Description = Resources.Charts.notChargeable, Quantity = 0, QuantityPercent = 0 };
                //MatterCapResourceTypedStat statSerie6 = new MatterCapResourceTypedStat() { Description = Resources.Charts.intern, Quantity = 0, QuantityPercent = 0 };


                //Statistiques
                MatterCapResourceStat statistics = new MatterCapResourceStat() { StatsList = new List<MatterCapResourceTypedStat>(), MatterCapResources = matterCapResources, Serie1Quantity = 0, Serie2Quantity = 0, Serie3Quantity = 0, Serie4Quantity = 0, Serie5Quantity = 0, Serie6Quantity = 0, TotalQuantity = 0, AvailableQuantity = 0, OccupationRate = 0, OccupationHour = 0 };

                //On récupère les charges et le temps disponible sur la journée
                decimal decTotalQuantity = decimal.Zero;

                foreach (MatterCapResource mcr in matterCapResources)
                {

                    if (statSerie.Count >= 1)
                        statSerie[0].Quantity += mcr.Series_Quantity_1;
                    if (statSerie.Count >= 2)
                        statSerie[1].Quantity += mcr.Series_Quantity_2;
                    if (statSerie.Count >= 3)
                        statSerie[2].Quantity += mcr.Series_Quantity_3;
                    if (statSerie.Count >= 4)
                        statSerie[3].Quantity += mcr.Series_Quantity_4;
                    if (statSerie.Count >= 5)
                        statSerie[4].Quantity += mcr.Series_Quantity_5;
                    if (statSerie.Count >= 6)
                        statSerie[5].Quantity += mcr.Series_Quantity_6;

                    decTotalQuantity += (mcr.Series_Quantity_1 + mcr.Series_Quantity_2 + mcr.Series_Quantity_3 + mcr.Series_Quantity_4 + mcr.Series_Quantity_5 + mcr.Series_Quantity_6);



                    statistics.AvailableQuantity += mcr.decQtyAvailableDay_g;//Before => statistics.AvailableQuantity += mcr.decQtyAvailableDay_g.HasValue ? mcr.decQtyAvailableDay_g.Value : 0;
                }

                //On calcul les pourcentages des 3 types de charge
                if (decTotalQuantity != 0)
                {
                    if (statSerie.Count >= 1)
                        statSerie[0].QuantityPercent = Math.Round(statSerie[0].Quantity / (decTotalQuantity) * 100, 2);
                    if (statSerie.Count >= 2)
                        statSerie[1].QuantityPercent = Math.Round(statSerie[1].Quantity / (decTotalQuantity) * 100, 2);
                    if (statSerie.Count >= 3)
                        statSerie[2].QuantityPercent = Math.Round(statSerie[2].Quantity / (decTotalQuantity) * 100, 2);
                    if (statSerie.Count >= 4)
                        statSerie[3].QuantityPercent = Math.Round(statSerie[3].Quantity / (decTotalQuantity) * 100, 2);
                    if (statSerie.Count >= 5)
                        statSerie[4].QuantityPercent = Math.Round(statSerie[4].Quantity / (decTotalQuantity) * 100, 2);
                    if (statSerie.Count >= 6)
                        statSerie[5].QuantityPercent = Math.Round(statSerie[5].Quantity / (decTotalQuantity) * 100, 2);
                }

                statistics.StatsList = statSerie;

                //On récupère les liste de statistiques de charge
                //statistics.StatsList.Add(statSerie1);
                //statistics.StatsList.Add(statSerie2);
                //statistics.StatsList.Add(statSerie3);
                //statistics.StatsList.Add(statSerie4);
                //statistics.StatsList.Add(statSerie5);
                //statistics.StatsList.Add(statSerie6);

                //On récupère les quantités totales

                if (statSerie.Count >= 1)
                    statistics.Serie1Quantity = Math.Round(statSerie[0].Quantity, 2);
                if (statSerie.Count >= 2)
                    statistics.Serie2Quantity = Math.Round(statSerie[1].Quantity, 2);
                if (statSerie.Count >= 3)
                    statistics.Serie3Quantity = Math.Round(statSerie[2].Quantity, 2);
                if (statSerie.Count >= 4)
                    statistics.Serie4Quantity = Math.Round(statSerie[3].Quantity, 2);
                if (statSerie.Count >= 5)
                    statistics.Serie5Quantity = Math.Round(statSerie[4].Quantity, 2);
                if (statSerie.Count >= 6)
                    statistics.Serie6Quantity = Math.Round(statSerie[5].Quantity, 2);


                //statistics.Serie1Quantity = Math.Round(statSerie1.Quantity, 2);
                //statistics.Serie3Quantity = Math.Round(statSerie2.Quantity, 2);
                //statistics.Serie2Quantity = Math.Round(statSerie3.Quantity, 2);

                //Somme des quantités de charge
                //statistics.TotalQuantity = Math.Round(statistics.Serie1Quantity + statistics.Serie3Quantity + statistics.Serie2Quantity, 2);
                statistics.TotalQuantity = Math.Round(decTotalQuantity, 2);
                //Pourcentage d'occupation sur la période
                statistics.OccupationRate = statistics.AvailableQuantity != 0 ? Math.Round(statistics.TotalQuantity / statistics.AvailableQuantity * 100, 2) : 0;

                MatterFuncWebRef.MatterFunction webService = new MatterFuncWebRef.MatterFunction();

                webService.Credentials = SBLawyerWebApplication.Libs.SBLawyerCredential.GetCredentialCache(UsernameSession, PasswordSession);
                webService.Url = ConfigurationManager.AppSettings["NavisionMatterFunctionUrl"];

                // La culture vietnamienne.
                CultureInfo viVn = new CultureInfo("vi-VN");

                //statistics.OccupationHour = Convert.ToDouble(webService.GetBusyHour(UsernameSession, startDate.ToString("d", viVn), endDate.ToString("d", viVn)));
                //TimeSpan timespan = TimeSpan.FromHours(statistics.OccupationHour);
                //statistics.OccupationHourTime = timespan.ToString("h\\:mm");

                return statistics;
            }


        }

        public static MatterCapResource updateMatterCapResource()
        {
            return new MatterCapResource();
        }

        public static MatterCapResource deleteMatterCapResource()
        {
            return new MatterCapResource();
        }
    }

}