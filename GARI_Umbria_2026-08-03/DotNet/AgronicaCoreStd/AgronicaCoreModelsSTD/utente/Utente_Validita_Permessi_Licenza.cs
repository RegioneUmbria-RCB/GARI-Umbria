using System;

namespace AgronicaCoreModelsSTD.utente
{
    public class Messaggio_Utente_Permessi
    {
        public bool Permessi_Scaduti_O_In_Scadenza = false;
        public bool Licenza_Scaduta_O_In_Scadenza = false;
        public string Messaggio = String.Empty;
    }

}
