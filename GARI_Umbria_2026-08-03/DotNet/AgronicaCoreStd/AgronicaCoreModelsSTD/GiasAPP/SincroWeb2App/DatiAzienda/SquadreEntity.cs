using System;
using System.Collections.Generic;

namespace AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda
{
    /// <summary>
    /// Ref: DS08-BL section 3.3.
    /// Rappresenta una squadra aziendale sincronizzata nel payload DatiAzienda.
    /// </summary>
    public class SquadreEntity
    {
        public string codiceSquadra;
        public string descrizione;
        public List<string> caposquadra = new List<string>();
        public List<string> membri = new List<string>();
        public DateTime? validoDal;
        public DateTime? validoAL;
    }
}