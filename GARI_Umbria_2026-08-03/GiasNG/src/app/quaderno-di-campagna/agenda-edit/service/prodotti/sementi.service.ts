import {Injectable} from "@angular/core";
import {enum_Impostazioni_Utenti} from "../../../../Model/Impostazioni_Utenti.enum";
import {Enum_DBTypeOperation} from 'gias-ui-kit';
import {enum_LAVCOD, enum_SEMINA_TIPO} from "../../../../Model/TipiEnumerativi";
import {ObjParametriAgendaService} from "../../../../Service/obj-parametri-agenda.service";
import {QdCService} from "../qdc.service";
import {PermessiUtenteService} from "../../../../Service/permessi-utente.service";
import {Lavorazione} from "../../../../Model/attivita/Lavorazione";
import {FormGroup} from "@angular/forms";
import { Tipo_Attivita} from 'gias-ui-kit';
import {RibaltamentoTypes} from "../../../../menu-agenda/components/utils";

@Injectable()

export class QdCSementiService {

    constructor(
        private objParametriAgendaService: ObjParametriAgendaService,
        public qdcservice: QdCService,
        private PermessiUtenteService: PermessiUtenteService,
    ) { }

    mostraOpzioniSemina(Form: FormGroup){
        //Mostro le opzioni semina solo se sono in scrittura di una nuova agenda e non sono in un
        // ribaltamento, non sono in sovescio e sono delle opzioni senza vincoli

        let mostra = false;

        let tipo_attivita: Tipo_Attivita = this.qdcservice.TestataForm.get("Tipo").getRawValue();

        let Operazione: Lavorazione = Form.get("Operazione").value;

        let Default_Opzione_Semina = this.PermessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.UTENTE_COD_SEMINA_TIPO);

        if(this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB === Enum_DBTypeOperation.Write &&
            tipo_attivita === Tipo_Attivita.QuadernoDiCampagna &&
            this.qdcservice.TipoRibaltamento === RibaltamentoTypes.Nessuno &&
            + Operazione.primaryKey.codice !== enum_LAVCOD.SOVESCIO &&
            !this.qdcservice.IsModalitaDemetra() &&
            ((!Default_Opzione_Semina || !Default_Opzione_Semina.Valore || Default_Opzione_Semina.Valore === "") ||
                (+ Default_Opzione_Semina?.Valore !== enum_SEMINA_TIPO.Solo_Semina_Vincolo &&
                    + Default_Opzione_Semina?.Valore !== enum_SEMINA_TIPO.Semina_e_Modifica_Appezzamenti_Vincolo &&
                    + Default_Opzione_Semina?.Valore !== enum_SEMINA_TIPO.Frazionamento_Appezzamenti_perLotto_e_Semina_sui_Singoli_Vincolo))){
            mostra = true;
        }

        return mostra;
    }

}
