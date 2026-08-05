using AgronicaCoreModelsSTD.attivita.centri_di_costo;
using AgronicaCoreModelsSTD.metaschema.avversita;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
   public class PopolaRilievo

    {
        public string lavCod { get; set; }
        public string vegCod { get; set; }
        public string dpiCod { get; set; }
        public string idRcdpi { get; set; }
        public string dpiPubblicoPrivato { get; set; }
        /// <summary>
        /// Solitamente popolato lato server. Basato sul valore delle impostazioni:
        /// enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_FASI_FENOLOGICHE,
        /// enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_AVVERSITA,
        /// enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_ERBE_INFESTANTI,
        /// enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_INDICI_MATURITA,
        /// enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_INDICI_RESE_RACCOLTA,
        /// enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_DANNI_ALLA_RACCOLTA
        /// </summary>
        public Boolean personalizzate { get; set; }
        public List<EsercizioCDC> eserciziCDC { get; set; }
        public AvversitaGruppo avversitaGruppo { get; set; }
    }
}
