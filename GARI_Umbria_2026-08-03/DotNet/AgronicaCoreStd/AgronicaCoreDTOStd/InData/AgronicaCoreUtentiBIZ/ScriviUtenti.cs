using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.AgronicaCoreUtentiBIZ
{
    public class ScriviUtenti
    {
        public int Operazione { get; set; }
        public List<AgronicaCoreModelsSTD.profilazione.Utente> Utenti { get; set; }
        public bool SettingsFromProfile { get; set; } = false;

    }
}
