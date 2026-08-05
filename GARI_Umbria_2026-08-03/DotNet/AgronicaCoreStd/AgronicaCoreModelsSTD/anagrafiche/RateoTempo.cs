using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class RateoTempo
    {
        public int Rateo_Cod {  get; set; }
        public DateTime? DataInizio {  get; set; }
        public DateTime? DataFine { get; set; }
        public DateTime? OraInizio { get; set; }
        public DateTime? OraFine { get; set; }
        public int Rotazione { get; set; }
        public DateTime ValiditaInizio { get; set; }
        public DateTime ValiditaFine { get; set; }

        public RateoTempo() { }

        public RateoTempo(int rateo_Cod, DateTime dataInizio, DateTime dataFine, DateTime oraInizio, DateTime oraFine, int rotazione, DateTime validitaInizio, DateTime validitaFine)
        {
            Rateo_Cod = rateo_Cod;
            DataInizio = dataInizio;
            DataFine = dataFine;
            OraInizio = oraInizio;
            OraFine = oraFine;
            Rotazione = rotazione;
            ValiditaInizio = validitaInizio;
            ValiditaFine = validitaFine;
        }

        /// <summary>
        /// Check if the data of two RateoTempo is the same.
        /// Does not check the Rateo_Cod.
        /// </summary>
        /// <param name="other">The other object</param>
        /// <param name="checkValidity">If true compares also the fields ValiditaInizio, ValiditaFine</param>
        /// <returns>True if</returns>
        public bool IsDataEqual(RateoTempo other, bool checkValidity = true)
        {
            bool isMainDataEquals = DataInizio == other.DataInizio && DataFine == other.DataFine
                && OraInizio == other.OraInizio && OraFine == other.OraFine
                && Rotazione == other.Rotazione;
            bool isValidityEquals = ValiditaInizio == other.ValiditaInizio && ValiditaFine == other.ValiditaFine;
            return isMainDataEquals && (!checkValidity || isValidityEquals);
        }
    }
}
