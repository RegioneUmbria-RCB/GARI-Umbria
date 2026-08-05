using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App
{
    public class ParcoMacchineEntity
    {
        public string partitaIva;
        public int centroAziendaleCod;
        public int codice;
        public string descrizione;
        public string modello;
        public int macchinaCod;
        public DateTime validitaFrom;
        public DateTime validitaTo;

        public string tipoMacchinaCod;
        public int titoloPossessoCod;
        public int finalitaCod;
        public string proprietario;
        public string targa;
        public string numImmatricolazione;
        public DateTime dataImmatricolazione;
        public string codiceAnagrafe;
        public string immagine;

        public string BTM_Serial;
        public string VIN;
        public string Img_Thumbnail;
        public string Img_Thumbnail_FileName;
        public string Img_Thumbnail_Extension;
        public string Img_Large;
        public string Img_Large_FileName;
        public string Img_Large_Extension;

        public string Distinta_Installazione;
        public string Contratto_Installazione;
        public string Tipologia_Installazione;
        public DateTime? Data_Inizio_Installazione;
        public DateTime? Data_Fine_Installazione;
        public string Stato_Installazione;
        public string Provincia_Istat_Installazione;
        public string Comune_Istat_Installazione;
        public string Indirizzo_Installazione;

        public float? Latitudine_Installazione;
        public float? Longitudine_Installazione;
        public string contattoCod;
        public int visibileCtrlGestione;

    }
}
