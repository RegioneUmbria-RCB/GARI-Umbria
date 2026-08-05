using InData.FoodMetaVerse;

namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Models
{
    /// <summary>
    /// Input per la persistenza batch dei risultati degli indicatori H2O nella tabella Lookup_Sost_H20_Lotto.
    /// Ogni elemento di <see cref="Risultati"/> corrisponde a una riga da inserire.
    /// Riferimento spec: DS08-BL PersistenzaRisultatiPerColture — Input.
    /// </summary>
    public class PersistenzaRisultatiPerColtureInput
    {
        /// <summary>
        /// Identificativo univoco dell'evento di calcolo che ha prodotto questi risultati.
        /// Legato all'evento calcolaPerimetro.
        /// </summary>
        public Guid IdInvocazione { get; set; }

        /// <summary>
        /// Timestamp di calcolo da persistere. Se non valorizzato, sarà impostato a UTC now.
        /// </summary>
        public DateTime DataCalcolo { get; set; }

        /// <summary>
        /// Lista di righe da inserire nella tabella lookup_sost_h2o_lotto.
        /// Una riga per combinazione (filiera, azienda, anno, appezzamento, varieta, esercizio, lotto_raccolta).
        /// </summary>
        public List<WriteLookupSostH2OLotto> Risultati { get; set; } = new();
    }
}
