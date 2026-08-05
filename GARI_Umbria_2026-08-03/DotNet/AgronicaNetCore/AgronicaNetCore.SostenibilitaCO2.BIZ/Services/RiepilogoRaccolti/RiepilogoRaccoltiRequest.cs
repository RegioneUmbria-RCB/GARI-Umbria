namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.RiepilogoRaccolti
{
    /// <summary>
    /// Parametri di input per il calcolo del riepilogo raccolti CO₂.
    /// </summary>
    public class RiepilogoRaccoltiRequest
    {
        /// <summary>
        /// P.IVA della filiera padre (selezionata da FS2.01).
        /// </summary>
        public string PivaFiliera { get; set; } = string.Empty;

        /// <summary>
        /// Codice specie vegetale selezionata (da FS2.02).
        /// Il valore 0 indica "tutte le colture".
        /// </summary>
        public int VegCod { get; set; }

        /// <summary>
        /// Anno campagna di riferimento; usato per costruire l'intervallo temporale
        /// 1 gennaio <see cref="Anno"/> – 31 dicembre <see cref="Anno"/>.
        /// </summary>
        public int Anno { get; set; }
    }
}
