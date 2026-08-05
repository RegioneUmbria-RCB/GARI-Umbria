import { Component, OnDestroy, OnInit } from '@angular/core';
import { generateGridProviders } from 'gias-kendo-grid';
import { ContattiGridService } from './contatti-edit.service';
import { ContattiRootService } from './contattiRoot.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { map, Observable, Subject, takeUntil, tap } from 'rxjs';
import { PermessiUtenteService } from "../../../../Service/permessi-utente.service";
import { enum_salva_in, enum_Security_Attivita } from "../../../../Model/TipiEnumerativi";
import { ContattiService, ImpostazioneSemplice } from 'app/Service/Anagrafica/contatti.service';
import { Indirizzo } from 'app/Service/api.service';
import { enum_Impostazioni_Utenti } from 'app/Model/Impostazioni_Utenti.enum';


@Component({
    standalone: false,
    selector: 'app-contatti-edit',
    templateUrl: './contatti-edit.component.html',
    styleUrls: ['./contatti-edit.component.css'],
    providers: [
        ...generateGridProviders(ContattiGridService, ContattiEditComponent)
    ]
})
export class ContattiEditComponent implements OnInit, OnDestroy {
    private signal: Subject<void> = new Subject();
    isChecked: boolean = false;
    disabled: boolean = true;
    visible: boolean = false;
    saveParentCompanyDDL$: Observable<{ codice: string; descrizione: string }[]>;
    contattoPubblicoVisible: boolean = false;
    contattoPubblicoIsChecked: boolean = false;
    contattoPubblicoDisabled: boolean = true;
    defaultParentCompany = null;

    constructor(
        private contattiRootService: ContattiRootService,
        private objParametriAgendaService: ObjParametriAgendaService,
        private permessiUtenteService: PermessiUtenteService,
        private ContattiClientService: ContattiService,
    ) { this.isChecked = false; }

    ngOnDestroy(): void {
        this.signal.next();
        this.signal.complete();
    }
    clicked(event) {
        this.isChecked = event;
        this.contattiRootService.setSalvaInPadre(this.isChecked);
    }
    contattoPubblicoClicked(event) {
        this.contattoPubblicoIsChecked = event;
        this.contattiRootService.setCreaContattiPubblici(this.contattoPubblicoIsChecked);
    }
    ngOnInit(): void {

        if (this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB == Enum_DBTypeOperation.Write) {
            this.disabled = false;
            this.visible = true;
            this.contattoPubblicoVisible = true;
            if (this.permessiUtenteService.getPermesso(enum_Security_Attivita.Modifica_Contatti_Pubblici, 2) == true) {
                this.contattoPubblicoDisabled = false;
            }

            var contattoPubblicoVisibleDefault = this.permessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.SUPERUSER_NuovaAzienda_ContattiPubblici);

            if (contattoPubblicoVisibleDefault.Valore === "1") {
                this.contattoPubblicoIsChecked = true;
                this.contattiRootService.setCreaContattiPubblici(this.contattoPubblicoIsChecked);
            }
            this.contattiRootService.singoloPadreSource.asObservable().pipe(takeUntil(this.signal), map(val => {
                this.disabled = !val;
                this.isChecked = false;
            })).subscribe();

            var propContattoDefault = this.permessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.SUPERUSER_NuovaAzienda_ProprietaContatti);
            this.contattiRootService.setContattoAzienda({ proprietarioContattoAzienda: Number(propContattoDefault.Valore) });

            this.saveParentCompanyDDL$ = this.ContattiClientService.LeggiImpostazioniProprietaContatti().pipe(
                map((result: ImpostazioneSemplice[]) => {
                    if (result && result.length > 0 && result[0].data) {
                        const data = result[0].data.map(d => ({ codice: d.codice, descrizione: d.descrizione }));
                        var proprietario = this.contattiRootService.getContattoAzienda();
                        this.defaultParentCompany = data.find(d => d.codice == proprietario.proprietarioContattoAzienda.toString()) || null;
                        return data;
                    }
                    return [];
                })
            );
        }
    }

    onParentCompanyChange(value: any) {
        switch (value.codice) {
            case enum_salva_in.SalvaInAziendaSU.toString():
                this.contattiRootService.setContattoAzienda({ proprietarioContattoAzienda: value.codice });
                break;
            case enum_salva_in.SalvaInPrimaAziendaPadre.toString():
                this.contattiRootService.setContattoAzienda({ proprietarioContattoAzienda: value.codice });
                break;
            case enum_salva_in.SalvaInAziendaCorrente.toString():
                this.contattiRootService.setContattoAzienda({ proprietarioContattoAzienda: value.codice });
                break;
            default:
                break;
        }
    }
}