using System;
using System.Collections.Generic;
using System.Text;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.metaschema;

namespace AgronicaCoreModelsSTD.Zoo
{
    public class GiacenzaZoo
    {
        #region Properties

        /// <summary>
        /// Piva e Cod_Animale
        /// </summary>
        public PK primaryKey { get; set; }
        /// <summary>
        /// Capo in giacenza
        /// </summary>
        public CapoAnimaleLight capo { get; set; }
        /// <summary>
        /// Centro di appartenenza
        /// </summary>
        public CentroAziendaleLight centro { get; set; }
        /// <summary>
        /// Stalla di appartenenza
        /// </summary>
        public FabbricatoLight stalla { get; set; }
        /// <summary>
        /// Raggruppamento stalla di appartenenza
        /// </summary>
        public SottogruppoStallaLight raggruppamento { get; set; }
        /// <summary>
        /// Lotto
        /// </summary>
        public string lotto { get; set; }
        /// <summary>
        /// giorni di presenza in stalla dall'arrivo
        /// </summary>
        public int giorniInStalla { get; set; }
        /// <summary>
        /// codice AUSL dell'azienda di nascita
        /// </summary>
        public string auslAzNascita { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string utenteCreazione { get; set; }    
        /// <summary>
        /// 
        /// </summary>
        public DateTime dataCreazione { get; set; }        
        /// <summary>
        /// 
        /// </summary>
        public string utenteModifica { get; set; }        
        /// <summary>
        /// 
        /// </summary>
        public DateTime dataModifica { get; set; }

        /*
        /// <summary>
        /// Udm_Cod = 38 (numero)
        /// </summary>
        public UnitaDiMisura unitaMisura { get; set; }
        /// <summary>
        /// presenza del capo in BDN o meno
        /// </summary>
        public string flagBDN { get; set; }

        /// <summary>
        /// Codice Fiscale del fornitore fatturazione
        /// </summary>
        public string fornFatt_CF { get; set; }
        /// <summary>
        /// Ragione Sociale del fornitore fatturazione
        /// </summary>
        public string fornFatt_RagSoc { get; set; }
        /// <summary>
        /// Codice Fiscale del fornitore di provenienza
        /// </summary>
        public string fornProv_CF { get; set; }
        /// <summary>
        /// Ragione Sociale del fornitore di provenienza
        /// </summary>
        public string fornProv_RagSoc { get; set; }

        /// <summary>
        /// Cod_Contatto del fornitore di provenienza
        /// </summary>
        public string fornProv_Contatto { get; set; }
        /// <summary>
        /// Numero di Bolla del fornitore
        /// </summary>
        public string numBolla_Forn { get; set; }
        /// <summary>
        /// Numero di Bolla di uscita
        /// </summary>
        public string numBolla_Uscita{ get; set; }
        /// <summary>
        /// Data di ingresso del DDT
        /// </summary>
        public DateTime dataIngressoDDT { get; set; }
        /// <summary>
        /// Data di uscita del DDT
        /// </summary>
        public DateTime dataUscitaDDT { get; set; }
        */

        /// <summary>
        /// Giorni in stalla a partire dalla data di primo caricamento 
        /// </summary>
        public string giorniInStalla_primoCaricamento { get; set; } 

        /// <summary>
        /// Data primo caricamento 
        /// </summary>
        public DateTime dataPrimoCaricamento { get; set; }

        #endregion

        public class PK
        {
            /// <summary>
            /// Piva di appartenenza
            /// </summary>
            public string partitaIva { get; set; }

            /// <summary>
            /// Cod_Animale
            /// </summary>
            public string codice { get; set; }

            public PK(string partitaIva, string codice)
            {
                this.partitaIva = partitaIva;
                this.codice = codice;
            }

            public PK()
            {
            }
        }


    }
}
