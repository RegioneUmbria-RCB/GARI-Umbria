namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Rappresenta una singola operazione colturale nell'array <c>operazioni[]</c> del payload M4.
    /// Contiene i dettagli dell'operazione e i prodotti utilizzati (fertilizzanti, agrofarmaci, sementi, raccolte).
    /// Riferimento spec: FS2.04 Mapping Oggetto "Operazione" — DS04.2-BL Mappature.
    /// </summary>
    public class OperazionePayload
    {
        /// <summary>
        /// Identificativo operazione (<c>Agenda.Id_Agenda</c>).
        /// Riferimento spec: FS2.04 mapping <c>id_operazione</c>.
        /// </summary>
        public string id_operazione { get; set; }

        /// <summary>
        /// Tipo operazione ricavato da <c>Codifica_Operazioni_SistemiEsterni.lav_cod_esterno</c>
        /// tramite join INNER su <c>Agenda.Lav_Cod</c>.
        /// Riferimento spec: FS2.04 mapping <c>tipo_operazione</c> — DS04.2-BL Mappature.
        /// </summary>
        public string tipo_operazione { get; set; } = string.Empty;

        /// <summary>
        /// Data dell'operazione (<c>Agenda.Validita_Inizio</c>), formato ISO 8601.
        /// Riferimento spec: FS2.04 mapping <c>data_operazione</c>.
        /// </summary>
        public DateOnly data_operazione { get; set; }

        /// <summary>
        /// Superficie trattata in ettari (<c>Mov_Destinazioni.Qta2</c>).
        /// Riferimento spec: FS2.04 mapping <c>superficie_operazione</c> — DS04.2-BL Mappature.
        /// </summary>
        public decimal? superficie_operazione_ha { get; set; }

        /// <summary>
        /// Lista fertilizzanti applicati nell'operazione.
        /// Valorizzato se <c>Elem_Cod = 3</c>. Lista vuota se non applicabile.
        /// Riferimento spec: FS2.04 mapping <c>fertilizzanti[]</c>.
        /// </summary>
        public List<FertilizzantePayload> fertilizzanti { get; set; } = new List<FertilizzantePayload>();

        /// <summary>
        /// Lista agrofarmaci applicati nell'operazione.
        /// Valorizzato se <c>Elem_Cod = 191</c>. Lista vuota se non applicabile.
        /// Riferimento spec: FS2.04 mapping <c>agrofarmaci[]</c>.
        /// </summary>
        public List<AgrofarmacоPayload> agrofarmaci { get; set; } = new List<AgrofarmacоPayload>();

        /// <summary>
        /// Lista sementi utilizzate nell'operazione.
        /// Valorizzato se operazione di Semina o Trapianto (<c>Elem_Cod = 10</c>). Lista vuota se non applicabile.
        /// Riferimento spec: FS2.04 mapping <c>sementi[]</c>.
        /// </summary>
        public List<SementePayload> sementi { get; set; } = new List<SementePayload>();

        /// <summary>
        /// Lista prodotti raccolti nell'operazione.
        /// Valorizzato se operazione di Raccolta (<c>Elem_Cod = 210</c>). Lista vuota se non applicabile.
        /// Riferimento spec: FS2.04 mapping <c>prodotti_raccolti[]</c>.
        /// </summary>
        public List<ProdottoRaccoltоPayload> prodotti_raccolti { get; set; } = new List<ProdottoRaccoltоPayload>();
    }
}
