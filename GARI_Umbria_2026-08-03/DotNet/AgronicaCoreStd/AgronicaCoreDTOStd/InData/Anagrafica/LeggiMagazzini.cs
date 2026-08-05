using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita;
using System;
using System.Collections.Generic;
using System.Text;

namespace InData.Anagrafica
{
    public class LeggiMagazzini
    {
        public Impresa impresa;

        public bool filtroMagazziniAPP;

        public LeggiMagazzini(string piva, bool filtro)
        {
            impresa = new Impresa() { partitaIva = piva };
            filtroMagazziniAPP = filtro;
        }

    }
}
