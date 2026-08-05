using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.RischiMeteo.DAL.DataLayer.Impianti
{
    /// <summary>
    /// Entità dati di un impianto recuperata da <c>Reg_Impianti</c> / <c>Cultivar</c> / <c>SpecieVegetali</c>.
    /// </summary>
    public class ImpiantoRischiMeteoEntity
    {
        public int? VegCod { get; init; }
        public int? CulCod { get; init; }
    }

    /// <summary>
    /// Contratto per il recupero dei dati di un impianto (specie, cultivar) da <c>Reg_Impianti</c>.
    /// Utilizzato come fallback quando <c>LeggiImpiantiAsync</c> non restituisce righe per un Esercizio
    /// privo di operazioni registrate.
    /// Riferimento spec: DS02-BL CostruttoPayloadM2 — Mapping coltura.codiceSpecie e codiceCultivar.
    /// </summary>
    public interface IImpiantiRischiMeteoDAL
    {
        /// <summary>
        /// Recupera <c>Veg_Cod</c> e <c>Cul_Cod</c> per l'impianto identificato da
        /// <c>piva</c>, <c>saCod</c>, <c>appezza</c> e <c>idReg</c>.
        /// </summary>
        Task<ImpiantoRischiMeteoEntity?> GetImpiantoAsync(
            string piva, int saCod, int appezza, int idReg,
            AgronicaCoreParametriServer objParametriServer);
    }
}
