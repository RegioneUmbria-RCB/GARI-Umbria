import {Injectable, TemplateRef} from '@angular/core';
import {FormBuilder, FormGroup} from '@angular/forms';
import {Contatto} from 'app/Model/anagrafiche/Contatto';
import {ParcoMacchine} from 'app/Model/anagrafiche/ParcoMacchine';
import {AgendaClient} from 'app/Service/api.service';
import {RapportoContabile} from 'app/Model/anagrafiche/RapportoContabile';
import {RisorseUmane} from '../../../Model/anagrafiche/RisorseUmane';
import {RisorsaMacchina} from 'app/Model/attivita/risorse/RisorsaMacchina';
import {RisorsaPersona} from 'app/Model/attivita/risorse/RisorsaPersona';
import {AjaxAgronicaAPIService} from 'app/Service/ajax-agronica.api.service';
import {GiasDialogService} from 'app/Service/gias-dialog.service';
import {lastValueFrom, map, Observable, timeout} from 'rxjs';
import {AttivitaxModificaMutipla, enum_ModificaMultiplaOperazioni, ModificaMultipla_Operazione} from '../models';
import {MacchineService} from 'app/Service/Anagrafica/macchine.service';
import {MassEditOperaioRowModel} from './grid-add-operai/grid-add-operai.service';
import {MassEditMacchinaRowModel} from './grid-add-macchine/grid-add-macchine.service';
import {TranslocoService} from "@jsverse/transloco";
import {DateUtilsServiceService} from "../../../Service/date-utils-service.service";

//const link_ModificaMultipla_Attivita_Old = '/Agenda/Agenda.asmx/ModificaMultiplaListaAttivita';
const link_ModificaMultipla_Attivita = 'Agenda/ModificaMultiplaListaAttivita';

@Injectable({
    providedIn: 'root'
})
export class OperationEditService {

    editForm: FormGroup = this.fb.group({
        attivita: '',
        operazione: '',
        magazzino: '',
        manodopera: this.fb.group({
            mostraSoloAziendali: false,
            eliminaAssociati: false,
            contatti: []
        }),
        macchine: this.fb.group({
            mostraSoloAziendali: false,
            eliminaAssociate: false,
            macchine: []
        })
    });

    constructor(private fb: FormBuilder,
                private apiService: AjaxAgronicaAPIService,
                private dialogService: GiasDialogService,
                private transloco: TranslocoService,
                private dateUtils: DateUtilsServiceService
    ) {
    }

    get attivitaSelezionate(): AttivitaxModificaMutipla[] {
      return this.editForm.get('attivita').value
        .filter(s => s['Selected'] === true)
        .map(a => {
            const attivita: AttivitaxModificaMutipla = {
              ID_Agenda: parseInt(a.ID, 10),
              Raccoglitore_Cod: parseInt(a.Raccoglitore_Cod, 10),
              Lav_Cod: parseInt(a.Lav_cod, 10),
              Lav_Des: a.Lav_Des,
              Piva: a.Piva,
              Sa_Cod: parseInt(a.sa_cod, 10),
              Data: a.Data
            };
            if (typeof (a.Data) === 'string') {
              attivita.Data = this.dateUtils.parseDate(a.Data);
            }
            return attivita;
        });
    }

    public async saveCurrent() {
        try {
            this.apiService.ajaxAPIPost<any, any>(link_ModificaMultipla_Attivita,
                this.getSaveParams(), true
            ).pipe(map(async R => {
                let msg = '';
                if (R.ErroriGias.length > 1)
                    msg = R.ErroriGias.splice(1)
                        .map(e => e.messaggio)
                        .reduce((a, b) => a + '</br>' + b);
                if (!R.RispostaOK) {
                    this.dialogService.baseError('', msg, false);
                } else {
                    let s = await this.dialogService.baseSuccess(
                        'SalvataggioAvvenutoConSuccesso',
                        R.RispostaStringa[0].messaggio + '\n' + msg, undefined, false);
                    // this.dialogService.baseInfo('', msg);
                }
            })).subscribe();
        } catch (e) {
            console.log(e);
        }
    }

    /** Carica i contatti assegnabili come manodopera nella modifica multipla di
     * attività */
    public LeggiListaPersone(): Promise<Contatto[]> {
        return lastValueFrom(this.apiService.ajaxAPIPost<ModificaMultipla_Operazione, Contatto[]>(
            'Agenda/CaricaListaPersone', this.getSaveParams()
        ).pipe(map(res => res.RispostaOK ? res.RispostaStringa : [])));
    }

    /** Carica le macchine assegnabili nella modifica multipla di attività */
    listaMacchineModificaMultipla(): Observable<ParcoMacchine[]> {
        return this.apiService.ajaxAPIPost<ModificaMultipla_Operazione, ParcoMacchine[]>(
            'Agenda/CaricaListaMacchine', this.getSaveParams()
        ).pipe(map(R =>  R?.RispostaOK ? R.RispostaStringa : []));
    }

    private getSaveParams(): ModificaMultipla_Operazione {
        const data: ModificaMultipla_Operazione = {
            Tipo_Modifica: this.editForm.get('operazione').value,
            Attivita_list: [],
            Risorsa_list: [],
            Magazzino: this.editForm.get('magazzino').value,
            Solo_Aziendali: this.editForm.get('manodopera').get('mostraSoloAziendali').value || false,
            Elimina_Precedenti: false
        };
        data.Attivita_list = this.attivitaSelezionate;
        if (data.Tipo_Modifica === enum_ModificaMultiplaOperazioni.AGGIUNGI_MACCHINE) {
            data.Risorsa_list = this.editForm.get('macchine').get('macchine').value
                .filter(m => m['Selected'])
                .map(m => this.parseRisorsaMacchina(m));
            data.Elimina_Precedenti = this.editForm.get('macchine').get('eliminaAssociate').value;
            data.Solo_Aziendali = this.editForm.get('macchine').get('mostraSoloAziendali').value;
        } else if (data.Tipo_Modifica === enum_ModificaMultiplaOperazioni.AGGIUNGI_CONTATTI) {
            data.Risorsa_list = this.editForm.get('manodopera').get('contatti').value
                .filter(c => c['Selected'])
                .map(c => this.parseRisorsaPersona(c));
            data.Elimina_Precedenti = this.editForm.get('manodopera').get('eliminaAssociati').value;
            data.Solo_Aziendali = this.editForm.get('manodopera').get('mostraSoloAziendali').value;
        } else if (data.Tipo_Modifica === enum_ModificaMultiplaOperazioni.RIMUOVI_MACCHINE
            || data.Tipo_Modifica === enum_ModificaMultiplaOperazioni.RIMUOVI_CONTATTI) {
            data.Elimina_Precedenti = true;
        }
        return data;
    }

    private parseRisorsaPersona(row: MassEditOperaioRowModel): RisorsaPersona {
        let ris: RisorsaPersona = new RisorsaPersona();
        ris.risorsaUmana = new RisorseUmane();
        ris.risorsaUmana.rapportoContabile = new RapportoContabile(+row.Cod_Rapporto);
        ris.risorsaUmana.rapportoContabile.descrizione = row.Rapporto_Des;
        ris.risorsaUmana.contatto = new Contatto();
        ris.risorsaUmana.contatto.ragione_Sociale = row.Impresa;
        ris.risorsaUmana.codice = +row.Cod_RisUm;
        return ris;
    }

    private parseRisorsaMacchina(row: MassEditMacchinaRowModel): RisorsaMacchina {
        let ris: RisorsaMacchina = new RisorsaMacchina();
        ris.macchina = new ParcoMacchine();
        ris.macchina.descrizione = row.Mac_Des;
        ris.macchina.codice = parseInt(row.Mac_Cod, 10);
        return ris;
    }

}
