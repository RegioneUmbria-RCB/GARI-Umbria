using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.CodificheCAC.DAL.Model
{
    public class CodificaCACModel
    {
        public int? ID { get; set; }
        public int? Sistema_Cod { get; set; }
        public string? Codice_Esterno { get; set; }
        public string? Descrizione_Esterno { get; set; }
        public string? Tabella_Gias { get; set; }
        public string? Codice_Gias { get; set; }
        public DateTime? Data_Creazione { get; set; }
        public DateTime? Data_Modifica { get; set; }
        public string? Username_Creazione { get; set; }
        public string? Username_Modifica { get; set; }
        public DateTime? Validita_Inizio { get; set; }
        public DateTime? Validita_Fine { get; set; }
        public bool? flag_Cancellazione { get; set; }
        public string? Sistema { get; set; }
        public Enum_DBTypeOperation TipoOperazioneDB { get; set; }
    }
}
