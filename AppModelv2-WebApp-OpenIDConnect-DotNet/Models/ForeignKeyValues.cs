using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppModelv2_WebApp_OpenIDConnect_DotNet.Models
{
    public class ForeignKeyValues
    {
        public List<Resource> Associates { get; set; }
        public List<SBLClient> SBLClients { get; set; }
        public List<WorkType> WorkTypes { get; set; }
        public List<ActionFamily> ActionFamilies { get; set; }

        public UserParameters Parameters { get; set; }
    }
    public static class ForeignKeyValuesDataContext
    {
        /// <summary>
        /// Permet de récupérer les liste de données des combos pour la saisie des temps
        /// </summary>
        /// <returns>Liste de Resource, Matter, SBLClient, WorkType, MatterLine</returns>
        public static ForeignKeyValues getTimeForeignKeyValues()
        {
            HttpContext.Current.Session["gridType"] = "time";
            IEnumerable<WorkType> workTypesIE = WorkTypeDataContext.GetWorkTypes(true);
            //On prépare les données
            ForeignKeyValues values = new ForeignKeyValues
            {

                Associates = ResourceDataContext.GetAssociates(false), //GetBaseUnitOfMeasure a déjà rafraichit la liste
                SBLClients = SBLClientDataContext.GetSBLClients(true),
                WorkTypes = workTypesIE.ToList(),
                Parameters = UserParametersDataContext.GetUserParameters(true)
            };

            return values;
        }
        /// <summary>
        /// Permet de récupérer les liste de données des combos pour la saisie des frais
        /// </summary>
        /// <returns>Liste de Resource, Matter, SBLClient, WorkType, MatterLine</returns>
        public static ForeignKeyValues getExpenseForeignKeyValues()
        {
            HttpContext.Current.Session["gridType"] = "expense";

            //On prépare les données
            ForeignKeyValues values = new ForeignKeyValues
            {
                Associates = ResourceDataContext.GetAssociates(false), //GetBaseUnitOfMeasure a déjà rafraichit la liste
                SBLClients = SBLClientDataContext.GetSBLClients(true),
                ActionFamilies = ActionFamilyDataContext.GetActionFamilies(true),
                Parameters = UserParametersDataContext.GetUserParameters(true)
            };

            return values;
        }
    }
}