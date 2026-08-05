using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.TipiCarburante
{
    /// <summary>
    /// Contratto per l'accesso alla tabella <c>Tipi_Carburante</c>.
    /// Riferimento spec: DS02-BL ValidazioneDatiConsumoAziendale — Input <c>carburanti[].tipo_carburante</c>.
    /// </summary>
    public interface ITipiCarburanteDAL
    {
        /// <summary>
        /// Restituisce tutti i tipi carburante validi dalla tabella <c>Tipi_Carburante</c>.
        /// </summary>
        /// <param name="objParametriServer">Parametri di connessione al database GIAS.</param>
        Task<IReadOnlyList<TipoCarburanteEntity>> GetAllAsync(AgronicaCoreParametriServer objParametriServer);
    }
}
