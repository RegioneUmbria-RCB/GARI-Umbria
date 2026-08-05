namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Models
{
    /// <summary>
    /// Output della persistenza batch risultati H2O modalità "Per Colture".
    /// Riferimento spec: DS08-BL PersistenzaRisultatiPerColture — Output.
    /// </summary>
    public class PersistenzaRisultatiPerColtureOutput
    {
        /// <summary>Numero di righe effettivamente inserite nella tabella lookup_sost_h2o_lotto.</summary>
        public int RowsInserted { get; set; }

        /// <summary>Identificativo univoco dell'invocazione di calcolo associata all'inserimento.</summary>
        public Guid IdInvocazione { get; set; }
    }
}
