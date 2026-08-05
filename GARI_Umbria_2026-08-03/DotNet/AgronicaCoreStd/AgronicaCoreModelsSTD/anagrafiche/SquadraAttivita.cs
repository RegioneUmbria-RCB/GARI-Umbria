using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    /// <summary>
    /// usata dalla sincro app2web delle squadre
    /// </summary>
    public class SquadraAttivita
    {
        public string partitaIva;
        public string codiceSquadra;
        public string descrizione;
        public List<string> caposquadra = new List<string>();
        public List<string> membri = new List<string>();
        public DateTime? validoDal;
        public DateTime? validoAL;
        public bool flag_cancellazione;
    }
}
