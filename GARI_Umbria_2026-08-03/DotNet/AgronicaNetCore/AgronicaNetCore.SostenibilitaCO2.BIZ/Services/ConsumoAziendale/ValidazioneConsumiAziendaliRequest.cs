using AgronicaNetCore.SostenibilitaCO2.BIZ.Models;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.ConsumoAziendale
{
    /// <summary>
    /// Input per la business logic <c>ValidazioneDatiConsumoAziendale</c> (DS02-BL).
    /// Aggrega il perimetro aziende validato (DS01-BL) e i dati immessi dall'utente
    /// nelle tabelle Carburanti ed Energia.
    /// Riferimento spec: DS02-BL ValidazioneDatiConsumoAziendale — Input.
    /// </summary>
    public class ValidazioneConsumiAziendaliRequest
    {
        /// <summary>
        /// PIVA delle aziende incluse nel perimetro selezionato, output di DS01-BL.
        /// Usato come insieme di riferimento per la validazione subset (Regola 1).
        /// </summary>
        public List<string> PerimetroAziende { get; set; } = new();

        /// <summary>
        /// Dati di consumo carburante immessi dall'utente.
        /// Lista vuota è valida — scenario zero-consumo (Regola 7).
        /// </summary>
        public List<CarburanteConsumo> Carburanti { get; set; } = new();

        /// <summary>
        /// Dati di consumo energetico immessi dall'utente.
        /// Lista vuota è valida — scenario zero-consumo (Regola 7).
        /// </summary>
        public List<EnergiaConsumo> Energia { get; set; } = new();
    }
}
