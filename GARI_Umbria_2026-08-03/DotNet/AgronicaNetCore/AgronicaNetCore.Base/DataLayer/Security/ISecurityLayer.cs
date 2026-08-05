using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Base.DataLayer.Security
{
    /// <summary>
    /// ATTENZIONE, per letture normali usare ISecurityLayerDAL, questa classe imposta securityServiceBypass uguale a true
    /// </summary>
    public interface ISecurityLayer
    {
        [Obsolete("ATTENZIONE, per letture normali usare ISecurityLayerDAL, questa classe imposta securityServiceBypass uguale a true")]
        public DataTable LeggiConnessioni(AgronicaCoreParametriSuperServer objParametriSuperServer);

        [Obsolete("ATTENZIONE, per letture normali usare ISecurityLayerDAL, questa classe imposta securityServiceBypass uguale a true")]
        public DataTable LeggiConnessioniDbServerEUtenti(int idDb, AgronicaCoreParametriSuperServer objParametriSuperServer);

        [Obsolete("ATTENZIONE, per letture normali usare ISecurityLayerDAL, questa classe imposta securityServiceBypass uguale a true")]
        public DataTable LeggiConfigurazioneSiti(string chiave, AgronicaCoreParametriSuperServer objParametriSuperServer);
    }
}
