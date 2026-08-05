import { Injectable, OnDestroy, signal } from "@angular/core";
import { FormBuilder, FormControl, FormGroup, Validators } from "@angular/forms";
import { TranslocoService } from "@jsverse/transloco";
import { BaseCodeDescr } from "app/Model/baseClass/baseCodeDescr";
import { OperazioniZooClient } from "app/Service/net-core6-api.service";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { Subject, debounceTime, map, takeUntil } from "rxjs";
import { DettagliProtocollo } from "../model/dettagli-protocollo.model";
import { Enum_DBTypeOperation, ObjParametriAgenda } from "gias-ui-kit";
import { enum_FarmacoCategoria, enum_TypeTab_Zootecnia } from "app/zoo/models/tipi-enumerativi-zoo";
import { enum_PagineGiasNG } from "app/Model/TipiEnumerativi";
import { GestioneRichiesteService } from "app/Service/gestione-richieste.service";
import { Router } from "@angular/router";
import { enum_TipoPrescrizione } from "app/zoo/models/tipo-prescrizione.enum";
import { GiasMessageService } from "gias-kendo-grid";

export class Stalla {
    STA_NUM: number;
    STA_DES: string;
    BDN_Allev_IdFiscale: string;
    BDN_Codice_Azienda: string;

    constructor(staNum, staDes, BDNproprietario, BDNcodAzienda) {
        this.STA_NUM = staNum;
        this.STA_DES = staDes;
        this.BDN_Allev_IdFiscale = BDNproprietario;
        this.BDN_Codice_Azienda = BDNcodAzienda;
    }
}

class TrattamentoZoo {
    Data: Date;
    // Ora: Time;
    CentroAziendale: BaseCodeDescr;
    Stalla: Stalla;
    Raggruppamento: BaseCodeDescr;
    Note: string;
}

function CreateFormGroup(fb: FormBuilder, mask: TrattamentoZoo) {

    return fb.group({
        Data: new FormControl(mask.Data),
        // Ora: new FormControl(mask.Ora),
        CentroAziendale: new FormControl(mask.CentroAziendale, [Validators.required]),
        Stalla: new FormControl(mask.Stalla, [Validators.required]),
        Raggruppamento: new FormControl(mask.Raggruppamento),
        Note: new FormControl(mask.Note)
    });

}

@Injectable()
export class TrattamentoZooFormService implements OnDestroy {

    signal: Subject<void> = new Subject();

    formTrattamentoZoo: FormGroup = CreateFormGroup(this.fb, this.InitializeFormTrattamentoZoo());

    defaultCentro: BaseCodeDescr;
    defaultStalla: Stalla;
    defaultRaggruppamento: BaseCodeDescr;

    Array_CentriAziendale: Array<BaseCodeDescr> = [];
    Array_Stalle: Array<Stalla> = [];
    Array_Stalle_Raggruppamenti: Array<BaseCodeDescr> = [];

    objParametriAgenda: ObjParametriAgenda;

    dettagliProtocollo: DettagliProtocollo;
    attivitaDaChiamante: any;

    showGriglie: boolean = false;
    showGridProdotti: boolean = true;

    // drugIsSelected: boolean = false;
    ddlSelectionSubject: Subject<void> = new Subject();
    ddlSelectionRaggruppamentoSubject: Subject<void> = new Subject();

    private _changeDrugSelected = signal<{ codice: number | null, udm?: string }>({ codice: null });
    readonly changeDrugSelected = this._changeDrugSelected.asReadonly();

    //array delle row selezionate nelle griglie
    arrayDettagliProdottiSomministrazione: Array<any> = [];

    currentSaCod: number = 0;

    isInfoMode: boolean = false;
    isInfoOrUpdateMode: boolean = false;
    isFirstSomm: boolean = false;
    hasSuccessiveConfermate: boolean = false;
    withDiffAICs: boolean = false;

    arrayFarmCat: Array<enum_FarmacoCategoria> = [];

    //di default, poniamo che arriviamo dai protocolli
    typeTab_Zootecnia: enum_TypeTab_Zootecnia = enum_TypeTab_Zootecnia.ZooOperations;

    constructor(
                private fb: FormBuilder,
                private agendaService: ObjParametriAgendaService,
                public transloco: TranslocoService,
                private gestioneRichieste: GestioneRichiesteService,
                private router: Router,
                private zooService: OperazioniZooClient,
                private giasMessageService: GiasMessageService
            ) {

                this.objParametriAgenda = this.agendaService.getObjParamValue();

                this.isInfoMode = this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Read;
                this.isInfoOrUpdateMode = (this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Read) || (this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Update);

                this.defaultCentro = new BaseCodeDescr(0, this.transloco.translate('TuttiICentriAziendali'));
                this.defaultStalla = new Stalla(0, this.transloco.translate('TutteLeStalle'), "", "");
                this.defaultRaggruppamento = new BaseCodeDescr(0, this.transloco.translate('TuttiIRaggruppamenti'));

                this.formTrattamentoZoo.get('CentroAziendale').valueChanges.pipe(takeUntil(this.signal)).subscribe(value => {
                    this.currentSaCod = value.codice;
                    this.loadStalleDDL(this.objParametriAgenda.Piva, value.codice);
                });

                this.formTrattamentoZoo.get('Stalla').valueChanges.pipe(takeUntil(this.signal)).subscribe(value => {
                    this.loadRaggruppamenti(this.objParametriAgenda.Piva, this.currentSaCod, value.STA_NUM);
                });

                this.formTrattamentoZoo.get('Raggruppamento').valueChanges.pipe(takeUntil(this.signal)).subscribe(value => {
                    this.ddlSelectionRaggruppamentoSubject.next();
                });
    }

    onBlurData() {
        const value = this.formTrattamentoZoo.get('Data').value;
        if (this.showGriglie) {
            if (value instanceof Date && !isNaN(value.getTime())) {
                // this.drugIsSelected = false;
                this.ddlSelectionSubject.next();
            } else {
                this.giasMessageService.errorMessage('DataNonValida', false, true); // this.giasDialogService.baseSuccess('SalvataggioAvvenutoConSuccesso', '');
            }
        }
    }

    signalChangeDrugSelected(codice: number, udm?: string): void {
        // if (this.objParametriAgenda.TipoOperazioneDB !== Enum_DBTypeOperation.Update)
            this._changeDrugSelected.set({ codice, udm });
    }

    ngOnDestroy(): void {
        this.signal.next();
        this.signal.complete();
    }

    InitializeFormTrattamentoZoo(vals?: Partial<TrattamentoZoo>): TrattamentoZoo {
        return {
            Data: vals?.Data ?? new Date(),
            // Ora: vals?.Ora ?? null,
            CentroAziendale: vals?.CentroAziendale ?? null,
            Stalla: vals?.Stalla ?? null,
            Raggruppamento: vals?.Raggruppamento ?? null,
            Note: vals?.Note ?? ''
        };
    }

    loadCentriAziendaliDDL() {

        this.zooService.operazioniZooGetCentriAziendali(this.objParametriAgenda.Piva)
        .pipe(
            map(r => JSON.parse(r.RispostaStringa)),
            map(centers => centers.map(x => new BaseCodeDescr(x['sa_cod'], x['sa_nome'])))
        )
        .subscribe(result => {
            this.Array_CentriAziendale = result;

            if (this.attivitaDaChiamante)
                this.formTrattamentoZoo.get('CentroAziendale').patchValue(result.find(item => item.codice == this.attivitaDaChiamante.centroAziendale.primaryKey.codice));

        });

    }

    loadStalleDDL(piva: string, saCod: number) {

        this.formTrattamentoZoo.get('Stalla').patchValue(this.defaultStalla);

        if (saCod !== 0) {
            this.zooService.operazioniZooGetStalle(piva, saCod)
            .pipe(
            map(r => JSON.parse(r.RispostaStringa))
            )
            .subscribe(result => {
                this.Array_Stalle = result;

                //se arrivo con l'oggetto parametrizzato
                if (this.attivitaDaChiamante) 
                    this.formTrattamentoZoo.get('Stalla').patchValue(result.find(item => item.STA_NUM == this.attivitaDaChiamante.fabbricatoCod));
            });
        } else {
            this.Array_Stalle.splice(0, this.Array_Stalle.length);
        }
    }

    loadRaggruppamenti(piva: string, saCod: number, stalla: number) {

        this.formTrattamentoZoo.get('Raggruppamento').patchValue(this.defaultRaggruppamento, { emitEvent: false });
        
        if (stalla !== 0) {
            // this.reloadGridParametri();
            if (!this.showGriglie)
                this.showGriglie = true;
            else
                this.ddlSelectionSubject.next();

            this.zooService.operazioniZooGetRaggruppamenti(piva, saCod, stalla)
            .pipe(
            map(r => JSON.parse(r.RispostaStringa)),
            map(stables => stables.map(x => new BaseCodeDescr(x['Raggruppamento_Cod'], x['Raggruppamento_Des']))),
            )
            .subscribe(result => {
                this.Array_Stalle_Raggruppamenti = result;
            });
        } else {
            this.showGriglie = false;
            this.Array_Stalle_Raggruppamenti.splice(0, this.Array_Stalle_Raggruppamenti.length);
        }
    }

    reloadGridParametri() {
        this.showGriglie = false;
        setTimeout(() => this.showGriglie = true, 0);
    }

    goToGridPage() {
        this.gestioneRichieste.gestionePassaggioStessoSito(enum_PagineGiasNG.Pagina_Menu_Zoo).then(resp => {
           this.router.navigate([resp]);
        });
    }

    checkIsVeterinaryOrIndTerapeuticPrescription(): boolean {
        return this.dettagliProtocollo.tipoPrescrizione == enum_TipoPrescrizione.Veterinaria
            || this.dettagliProtocollo.tipoPrescrizione == enum_TipoPrescrizione.Indicazione_Terapeutica;
    }

    checkIfIsEditingFromVeterinaryPrescripton(): boolean {
        return this.objParametriAgenda.TipoOperazioneDB === Enum_DBTypeOperation.Update
            && this.dettagliProtocollo.tipoPrescrizione == enum_TipoPrescrizione.Indicazione_Terapeutica;
    }
}
