using AgronicaCoreModelsSTD.Gis;

namespace AgronicaCoreDTOStd.InData.Gis
{
    /// <summary>
    /// Classe scrittura parametri configurazione Gis per utente
    /// </summary>      
    public class ScriviConfigurazioneGisUtente
    {
        //--------------------------------------------------------------------------------
        //Opzioni Scrittura
        //--------------------------------------------------------------------------------

        /// <summary>
        /// Indica se memorizzare i dati relativi al sistema di di riferimento predefinito
        /// </summary>
        /// <example>false</example>
        public bool MemorizzaSistemaDiRiferimentoPredefinito { get; set; } = false;

        /// <summary>
        /// Indica se memorizzare i dati relativi alla operazione colturale (Gruppo e Tipo)
        /// </summary>
        /// <example>false</example>
        public bool MemorizzaOperazioneColturale { get; set; } = false;

        //--------------------------------------------------------------------------------
        //Classe Configurazioni Gis Utente
        //--------------------------------------------------------------------------------

        /// <summary>
        /// Configurazione da memorizzare
        /// </summary>
        public ConfigurazioneGisUtente CfgGisUtente { get; set; } = new ConfigurazioneGisUtente();

        /// <summary>
        /// Indica se i valori passati sono da salvare come default.
        /// </summary>
        /// <example>false</example>
        public bool salvaDefault { get; set; } = false;
    }
}
