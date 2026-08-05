using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    public class ChiaveAlbero
    {
        public int TipoNodo { get; set; }
        public string Piva { get; set; }
        public int Sa_Cod { get; set; }
        public int Campo_Cod { get; set; }
        public int Appezza { get; set; }
        public int Id_Imp { get; set; }

        public string p_Part_Cod { get; set; }
        public string p_Provincia_Cod { get; set; }
        public string p_Comune_Cod { get; set; }
        public string p_Sezione { get; set; }
        public string p_Foglio { get; set; }
        public string p_Numero { get; set; }
        public string p_Subalterno { get; set; }

        public string Cod_Fiscale { get; set; }
        public string Fabbricato_Cod { get; set; }
        public string Prodotto_Cod { get; set; }
        public string Data_Lavorazione { get; set; }

        public string Analisi_Certificato_Cod { get; set; }
        public string Analisi_Testata_Cod { get; set; }
        public string Analisi_Dettaglio_Cod { get; set; }
        public string Analisi_Campione_Cod { get; set; }

        public string PianoConcimazioneTestata_Cod { get; set; }
        public string Progetto_Cod { get; set; }
        public string Programmazione_Cod { get; set; }
        public string Programmazione_Entita_Cod { get; set; }
        public string Id_Agenda { get; set; }
        public string PivaPadre { get; set; }
        public string Ricetta_Cod { get; set; }
        public string RicettaOperazione_Cod { get; set; }
    }
}
