using AgronicaCoreModelsSTD.Gis;

namespace AgronicaCoreDTOStd.InData.Gis
{
    /// <summary>
    /// Classe per impostazione parametri configurazione albero
    /// </summary>
    public class ImpostaConfigurazioneAlbero
    {
        //--------------------------------------------------------------------------------
        //Opzioni Impostazione
        //--------------------------------------------------------------------------------
        
        
        /// <value>
        /// Indica se leggere la configurazione Gis utente da database
        /// </value>
        /// <example>true</example>
        public bool LeggiConfigurazioneDaDatabase { get; set; } = false;
        
        //--------------------------------------------------------------------------------
        //Classe Configurazione Gis Utente
        //--------------------------------------------------------------------------------

        /// <summary>
        /// Configurazione utente
        /// </summary>
        public ConfigurazioneGisUtente CfgGisUtente { get; set; } = new ConfigurazioneGisUtente();
        
        
        
        //--------------------------------------------------------------------------------
        //Campi Configurazione Albero
        //--------------------------------------------------------------------------------

        /// <summary>
        /// ID della ricerca impostata attraverso il filtro
        /// </summary>
        /// <example>0</example>
        public int FiltroImpiantiIdTestataTemp { get; set; } = 0;
        
        /// <summary>
        /// Elenco dei codici delle specie vegetali (letto da configurazione)
        /// </summary>
        public string Elenco_Icone_SpecieVegetali { get; set; }

        /// <summary>
        /// P.Iva impresa selezionata, obbligatorio
        /// </summary>
        /// <example>01704430519</example>
        public string Piva { get; set; }

        /// <summary>
        /// Codice del centro Aziendale
        /// </summary>
        /// <example>0</example>
        public string Sa_Cod { get; set; }

        /// <summary>
        /// Dati di configurazione se nella modalità sementieri (stringa vuota oppure es.: 23|0|0|0|105|Segale 2023|23/08/2022 00:00:00|30/06/2023 00:00:00)
        /// </summary>
        /// <example></example>
        public string DatiSportelloSementieri { get; set; }
    }

}
