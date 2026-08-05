using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class RifFabbricato : RifCentroAziendale
    {
        public int fabbricato_cod { get; set; }

        // Costruttore vuoto per consentire deserializzazioni della classe
        public RifFabbricato() : base()
        {
            saCod = 0;
        }

        public RifFabbricato(string _piva, int _saCod, int _fabbricato_cod) : base(_piva, _saCod)
        {
            fabbricato_cod = _fabbricato_cod;
        }

        public String getFabbricatoFullId(string separator = "_")
        {
            return $"{partitaIva}{separator}{saCod}{separator}{fabbricato_cod}";
        }
    }

    public class Fabbricato : FabbricatoLight
    {
        public int fabbricato_cod { get; set; }

        #region Properties
        //public PK primaryKey { get; set; }
        //public string descrizione { get; set; }

        public Indirizzo indirizzo { get; set; }
        public int tipo { get; set; }
        public ParticelleCatastali particella { get; set; }
        public double volumeConvenzionale { get; set; }
        public bool flag_cancellazione { get; set; }
        public IntervalloTemporale validita { get; set; }
        public String utente_ultima_modifica { get; set; }
        public bool usoDaTerzi { get; set; }

        public string descrizione_centro { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public Fabbricato() : base()
        {
            flag_cancellazione = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="piva"></param>
        /// <param name="saCod"></param>
        /// <param name="fabbCod"></param>
        /// <param name="descr"></param>
        public Fabbricato(string piva, int saCod, int fabbCod, string descr) : base(piva, saCod, fabbCod, descr)
        {

        }
        #endregion

        //public class PK
        //{
        //    public int codice { get; set; }
        //    public CentroAziendale.PK centroAziendalePK { get; set; }

        //    public PK()
        //    {

        //    }

        //}
    }

}
