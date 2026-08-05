using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Base.Constants
{
    public class TableSequences
    {
        public List<string> SequenceTables { get; set; } = new List<string>() { 
            "agenda",
            "movimenti",
            "movimenti_dettagli",
            "movimenti_dettagli_tecnici",
            "movimenti_dettagli_tecnici_extra",
            "idtestatatemp",
            "ricette",
            "ricette_operazioni",
            "ricette_dettaglio_tecnico",
            "ricette_dettagli",
            "ricette_destinazioni",
            "raccoglitore",
            "gis_entita",
            "gis_elementigrafici",
            "impresa_progetto",
            "agronica_log_invio_chiamate",
            "profilazione_dati",
            "indirizzi"
            // aggiungere i nomi delle tabelle lower_case!
        };

        public List<string> SequenceTableExceptions { get; set; } = new List<string>() {
            "jDeereDataModel_EntitaGIAS",
            "materie_prime_campionature",
            "ws_vivaiPassaporti_operazioni",
            "regolamentiRMA",
            "PrincipiAttiviRMA",
            "Gis_entita",
            "Gis_elementiGrafici",
            "GIS_LayerTilesDescrizione",
            "particellecatastali",
            "lineaproduzione",
            "linea_classe_produzione",
            "preparazione",
            "pagamenti_causali",
            "imprese_sezionali",
            "liquidita",
            "ist_credito",
            "causaletrasporto",
            "TabellaRMA",
            "DerrateCodifica"
        };

    }
}
