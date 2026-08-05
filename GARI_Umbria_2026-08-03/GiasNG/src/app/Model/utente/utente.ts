import { Utente_Impostazioni } from "./utente_impostazioni";
import { Utente_Permesso } from "./utente_permesso";
import { Messaggio_Utente_Permessi } from "./messaggio_utente_permessi";

export class Utente {
    constructor (
        public Username?: string,
        public Nome?: string,
        public Cognome?: string,
        public Rag_Soc?: string,
        public Cod_Fisc?: string,
        public UsernameCommerciale?: string,
        public Permessi: Utente_Permesso[] = [],
        public Impostazioni: Utente_Impostazioni[] = [],
        public ValiditaUtentePermessiLicenza?: Messaggio_Utente_Permessi
    ) {}
}
