using System;

namespace OutData.Zoo.DataMars
{
    /// <summary>
    /// DTO di output dell'operazione di acquisizione pesate Datamars.
    /// <para>Riferimento spec: DS01-BL AcquisizionePesateDatamarsAPI â€” Output.</para>
    /// </summary>
    public sealed class RisultatoAcquisizionePesate
    {
        /// <summary>Indica se l'operazione complessiva Ã¨ andata a buon fine (almeno parzialmente).</summary>
        public bool RispostaOK { get; set; }

        /// <summary>Numero totale di pesate acquisite con successo su tutte le sessioni.</summary>
        public int NumPesateAcquisite { get; set; }

        /// <summary>Numero totale di sessioni elaborate.</summary>
        public int NumSessioni { get; set; }

        /// <summary>Numero totale di pesate scartate per errori di validazione.</summary>
        public int NumPesateScartate { get; set; }

        /// <summary>Timestamp UTC di esecuzione del ciclo di acquisizione.</summary>
        public DateTime DataEsecuzione { get; set; }
    }
}
