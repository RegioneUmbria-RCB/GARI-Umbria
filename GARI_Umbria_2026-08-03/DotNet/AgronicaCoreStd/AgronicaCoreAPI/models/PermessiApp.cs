namespace AgronicaCoreAPI.models
{
    public class PermessiApp
    {
        public bool attivita = true;
        public bool rilievi = true;
        public bool visite = true;
        public bool documenti = true;

        public bool pianocolturale = true;
        public bool magazzini = true;
        public bool macchine = true;
        public bool aziende = true;

        public PermessiApp(string permessiApp) {
            if (!string.IsNullOrEmpty(permessiApp)) {
                var permessi = permessiApp.Split("_");
                if (permessi.Length > 0) setPermessi(permessi[0].Split("|"));
                if (permessi.Length > 1) setPermessiUtente(permessi[1].Split("|"));
            }
        }

        private void setPermessi(string[] permessi)
        {
            attivita = permessi.Length > 0 && permessi[0] != "0";
            rilievi = permessi.Length > 1 && permessi[1] != "0";
            visite = permessi.Length > 2 && permessi[2] != "0";
            documenti = permessi.Length > 3 && permessi[3] != "0";

            // considero attività anche ricette e pianificate
            attivita = attivita || (permessi.Length > 4 && permessi[4] != "0");
            attivita = attivita || (permessi.Length > 5 && permessi[5] != "0");
        }

        private void setPermessiUtente(string[] permessi)
        {
            pianocolturale = permessi.Length > 1 && permessi[1] != "0";
            magazzini = permessi.Length > 2 && permessi[2] != "0";
            magazzini = magazzini || (permessi.Length > 5 && permessi[5] != "0");
            macchine = permessi.Length > 3 && permessi[3] != "0";
            macchine = macchine || permessi.Length > 4 && permessi[4] != "0";
            aziende = permessi.Length > 7 && permessi[7] != "0";
            aziende = aziende || (permessi.Length > 8 && permessi[8] != "0");
        }
    }
}
