import { Injectable, OnDestroy } from "@angular/core";
import { map, Subject, takeUntil } from "rxjs";
import { BaseCodeDescr, ObjParametriAgenda, Enum_DBTypeOperation } from "gias-ui-kit";
import { OperazioniZooClient, TerapieClient } from "app/Service/net-core6-api.service";
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from "@angular/forms";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { TranslocoService } from "@jsverse/transloco";
import { GestioneRichiesteService } from "app/Service/gestione-richieste.service";
import { Router } from "@angular/router";
import { GiasMessageService } from "gias-kendo-grid";
import { enum_PagineGiasNG } from "app/Model/TipiEnumerativi";

export class Stalla {
    Sta_Num: number;
    Sta_Des: string;
    BDN_Allev_IdFiscale: string;
    BDN_Codice_Azienda: string;

    constructor(staNum, staDes, BDNproprietario, BDNcodAzienda) {
        this.Sta_Num = staNum;
        this.Sta_Des = staDes;
        this.BDN_Allev_IdFiscale = BDNproprietario;
        this.BDN_Codice_Azienda = BDNcodAzienda;
    }
}

export class ProtocolloxIntervento {
    Id: number;
    Prot_Alt: number;

    constructor(id, prot_Alt) {
        this.Id = id;
        this.Prot_Alt = prot_Alt;
    }
}

export class Intervento {
    Id: number;
    Nome: string;
    Ordine: number;
    Protocolli: Array<ProtocolloxIntervento>;

    constructor(id, ordine, nome?) {
        this.Id = id;
        this.Ordine = ordine;
        this.Nome = nome || '';
        this.Protocolli = [];
    }
}

class TerapiaZoo {
    Id_Terapia: number;
    Piva: string;
    Sa_Cod: number;
    Sta_Num: number;
    Descrizione: string;
    Interventi?: Array<Intervento>;

    constructor(idTerapia?: number, piva?: string, saCod?: number, staNum?: number, descrizione?: string) {
        this.Id_Terapia = idTerapia || 0;
        this.Piva = piva || '';
        this.Sa_Cod = saCod || 0;
        this.Sta_Num = staNum || 0;
        this.Descrizione = descrizione || '';
        this.Interventi = [];
    }
}

class FormInterventoZoo {
    Codice: number;
    Nome: string;
    Protocolli: Array<ProtocolloxIntervento>;

    constructor(codice, nome, protocols?) {
        this.Codice = codice;
        this.Nome = nome || '';
        this.Protocolli = protocols || [];
    }
}

class FormTerapiaZoo {
    Id_Terapia?: number;
    CentroAziendale: BaseCodeDescr;
    Stalla: Stalla;
    Nome: string;
    Interventi: Array<FormInterventoZoo>;

    constructor() {
        this.Interventi = [];
    }
}

@Injectable()
export class TerapiaZooFormService implements OnDestroy {

    private isInitialLoadComplete: boolean = false;

    signal: Subject<void> = new Subject();

    formTerapiaZoo: FormGroup;

    defaultCentro: BaseCodeDescr;
    defaultStalla: Stalla;

    Array_CentriAziendale: Array<BaseCodeDescr> = [];
    Array_Stalle: Array<Stalla> = [];

    currentSaCod: number = 0;

    interventi: Array<FormInterventoZoo>;
    interventiCount: number = 0;
    interventionAdded$: Subject<number> = new Subject<number>();

    objP_Agenda: ObjParametriAgenda;
    therapyObj: TerapiaZoo;

    showGriglie: boolean = false;

    ddlSelectionSubject: Subject<void> = new Subject();

    isNewMode: boolean = false;
    isInfoMode: boolean = false;
    isUpdateMode: boolean = false;

    private createProtocolloFormGroup(id?: number, protAlt?: number): FormGroup {
        const mask = new ProtocolloxIntervento(id || 0, protAlt || 0);
        return this.fb.group({
            Id: [mask.Id],
            Prot_Alt: [mask.Prot_Alt],
        });
    }

    private createInterventoFormGroup(intervento?: FormInterventoZoo): FormGroup {
        const protocolsCount = intervento?.Protocolli?.length ?? 0;
        const defaultName = intervento?.Nome ?? `${this.transloco.translate('Intervento')} ${this.interventiCount + 1}`;
        // this.interventiCount += 1;
        const mask = intervento || new FormInterventoZoo(null, defaultName, null);
        const protocolFormGroups = (mask.Protocolli || []).map(protocol =>
            this.createProtocolloFormGroup(protocol.Id, protocol.Prot_Alt)
        );
        return this.fb.group({
            Codice: [mask.Codice],
            Nome: [mask.Nome, Validators.required],
            Protocolli: this.fb.array(
                protocolFormGroups,
                [Validators.required, Validators.minLength(1)]
            )
        });
    }

    private createMainForm(mask: FormTerapiaZoo): FormGroup {
        return this.fb.group({
            CentroAziendale: new FormControl(mask.CentroAziendale, [Validators.required]),
            Stalla: new FormControl(mask.Stalla, [Validators.required]),
            Nome: new FormControl(mask.Nome, [Validators.required]),
            Interventi: this.fb.array(
                mask.Interventi.map(intervento => this.createInterventoFormGroup(intervento))
            )
        });
    }

    private clearAllProtocols() {
        if (this.isInfoMode) return;

        // If all the page is loaded (ddl Centro and Stalla), then is possible to wipe the current protocols at the centro/stalla change
        // (in the case the centro/stalla ddls will be editable in update mode, this prevent to lose data before the first load)
        if (this.isUpdateMode && !this.isInitialLoadComplete) return;

        this.interventiFormArray.controls.forEach(control => {
            const protocolArray = (control as FormGroup).get('Protocolli') as FormArray;
            while (protocolArray.length) {
                protocolArray.removeAt(0);
            }
        });
    }

    constructor(
        public fb: FormBuilder,
        private agendaService: ObjParametriAgendaService,
        public transloco: TranslocoService,
        private gestioneRichieste: GestioneRichiesteService,
        private router: Router,
        private zooService: OperazioniZooClient,
        private therapyService: TerapieClient,
        private giasMessageService: GiasMessageService
    ) {
        this.defaultCentro = new BaseCodeDescr(0, this.transloco.translate('TuttiICentriAziendali'));
        this.defaultStalla = new Stalla(0, this.transloco.translate('TutteLeStalle'), "", "");

        this.objP_Agenda = this.agendaService.getObjParamValue();

        this.isNewMode = this.objP_Agenda.TipoOperazioneDB == Enum_DBTypeOperation.Write;
        this.isInfoMode = this.objP_Agenda.TipoOperazioneDB == Enum_DBTypeOperation.Read;
        this.isUpdateMode = this.objP_Agenda.TipoOperazioneDB == Enum_DBTypeOperation.Update;

        if (this.isNewMode) {
            this.therapyObj = new TerapiaZoo(undefined, this.objP_Agenda.Piva, this.objP_Agenda.Sa_Cod, this.objP_Agenda.Fabbricato);
        } else if (this.isInfoMode || this.isUpdateMode) {
            const raw = this.objP_Agenda.GenericObj_string;
            try {
                const parsed = typeof raw === 'string' ? JSON.parse(raw) : raw;
                const idTerapia = parsed.Id_Terapia ?? parsed.Id ?? 0;
                const piva = parsed.Piva ?? parsed.PivaAzienda ?? '';
                const saCod = parsed.Sa_Cod ?? parsed.sa_cod ?? 0;
                const staNum = parsed.Sta_Num ?? parsed.StaNum ?? 0;
                const descrizione = parsed.Descrizione ?? parsed.Nome ?? '';

                this.therapyObj = new TerapiaZoo(idTerapia, piva, saCod, staNum, descrizione);

                this.therapyObj.Interventi = Array.isArray(parsed.Interventi)
                    ? parsed.Interventi.map((i: any, idx: number) => {
                        const interventoId = i.Id_Intervento ?? i.Id ?? null;
                        const interventoNome = i.Descrizione ?? i.Nome ?? '';
                        const intervento = new Intervento(interventoId, i.Ordine ?? idx + 1, interventoNome);

                        intervento.Protocolli = Array.isArray(i.Protocolli)
                            ? i.Protocolli.map((p: any) =>
                                new ProtocolloxIntervento(p.Id ?? p.Id_Protocollo ?? 0, p.Id_Protocollo_Alt ?? 0)
                              )
                            : [];

                        return intervento;
                    })
                    : [];
                this.interventiCount = this.therapyObj.Interventi.length;
            } catch {
                this.therapyObj = undefined as unknown as TerapiaZoo;
            }
        }

        const therapyMask: Partial<FormTerapiaZoo> | undefined = this.therapyObj ? {
            Id_Terapia: this.therapyObj.Id_Terapia,
            CentroAziendale: this.defaultCentro,
            Stalla: this.defaultStalla,
            Nome: this.therapyObj.Descrizione,
            Interventi: (this.therapyObj.Interventi || []).map(i =>
                new FormInterventoZoo(i.Id, i.Nome ?? '', i.Protocolli)
            )
        } : undefined;

        this.formTerapiaZoo = this.createMainForm(this.initializeFormTerapiaZoo(therapyMask));

        this.formTerapiaZoo.get('CentroAziendale').valueChanges
            .pipe(takeUntil(this.signal))
            .subscribe(value => {
                if (value == undefined || value == null) value = this.defaultCentro;
                this.currentSaCod = value.codice;
                this.loadStalleDDL(this.objP_Agenda.Piva, value.codice);

                this.clearAllProtocols();
                this.ddlSelectionSubject.next();
            });
        this.formTerapiaZoo.get('Stalla').valueChanges
            .pipe(takeUntil(this.signal))
            .subscribe(value => {
                if (value == undefined || value == null) value = this.defaultStalla;

                this.clearAllProtocols();
                this.ddlSelectionSubject.next();
            });
    }

    get interventiFormArray(): FormArray {
        return this.formTerapiaZoo.get('Interventi') as FormArray;
    }

    getProtocolsFormArray(interventionIndex: number): FormArray {
        const interventionGroup = this.interventiFormArray.at(interventionIndex) as FormGroup;
        return interventionGroup.get('Protocolli') as FormArray;
    }

    addIntervento(): void {
        const newInterventoFormGroup = this.createInterventoFormGroup();
        this.interventiFormArray.push(newInterventoFormGroup);
        this.interventiCount += 1;
        this.interventionAdded$.next(this.interventiFormArray.length - 1);
    }

    removeIntervento(index: number): void {
        this.interventiCount -= 1;
        this.interventiFormArray.removeAt(index);
    }

    ngOnDestroy(): void {
        this.signal.next();
        this.signal.complete();
    }

    signalChangeDrugSelected(codice: number, udm?: string): void {
        // if (this.objParametriAgenda.TipoOperazioneDB !== Enum_DBTypeOperation.Update)
            // this._changeDrugSelected.set({ codice, udm });
    }

    initializeFormTerapiaZoo(vals?: Partial<FormTerapiaZoo>): FormTerapiaZoo {
        return {
            Id_Terapia: vals?.Id_Terapia ?? null,
            CentroAziendale: vals?.CentroAziendale ?? this.defaultCentro,
            Stalla: vals?.Stalla ?? this.defaultStalla,
            Nome: vals?.Nome ?? '',
            Interventi: vals?.Interventi ?? []
        };
    }

    loadCentriAziendaliDDL() {
        this.zooService.operazioniZooGetCentriAziendali(this.objP_Agenda.Piva)
            .pipe(
                map(r => JSON.parse(r.RispostaStringa)),
                map(centers => centers.map(x => new BaseCodeDescr(x['sa_cod'], x['sa_nome'])))
            )
            .subscribe(result => {
                this.Array_CentriAziendale = result;
                if (this.therapyObj) {
                    const initialCentro = result.find(item => item.codice == this.therapyObj.Sa_Cod) ?? this.defaultCentro;
                    this.formTerapiaZoo.get('CentroAziendale').patchValue(initialCentro);
                }
        });
    }

    loadStalleDDL(piva: string, saCod: number) {
        this.formTerapiaZoo.get('Stalla').patchValue(this.defaultStalla, { emitEvent: false });
        this.Array_Stalle.splice(0, this.Array_Stalle.length);

        if (saCod !== 0) {
            this.zooService.operazioniZooGetStalle(piva, saCod)
                .pipe(map(r => JSON.parse(r.RispostaStringa)))
                .subscribe(result => {
                    const stalleResult = result.map(x => new Stalla(x['STA_NUM'], x['STA_DES'], x['BDN_Allev_IdFiscale'], x['BDN_Codice_Azienda']));
                    this.Array_Stalle.push(...stalleResult);
                    if (this.therapyObj) {
                        const targetStaNum = this.therapyObj.Sta_Num;
                        const initialStalla = this.Array_Stalle.find(item => item.Sta_Num == targetStaNum);
                        this.formTerapiaZoo.get('Stalla').patchValue(initialStalla || this.defaultStalla);
                    }
                    if (stalleResult.length === 1) {
                        const singleStalla = stalleResult[0];
                        this.formTerapiaZoo.get('Stalla').patchValue(singleStalla, { emitEvent: false });
                    }
                    this.ddlSelectionSubject.next();
                    this.isInitialLoadComplete = true;
                });
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

    getInterventiToSave(): Array<Intervento> {
        const interventi: Array<Intervento> = [];

        this.interventiFormArray.controls.forEach((interventoGroup: FormGroup, index) => {
            if (interventoGroup.valid) {
                const formValue = interventoGroup.value;

                const intervento = new Intervento(
                    formValue.Codice,
                    index + 1,
                    formValue.Nome
                );

                const protocolsArray = interventoGroup.get('Protocolli') as FormArray;
                protocolsArray.controls.forEach(protocolGroup => {
                    const protocolValue = protocolGroup.value;
                    const protocollo = new ProtocolloxIntervento(
                        protocolValue.Id,
                        protocolValue.Prot_Alt,
                    );
                    intervento.Protocolli.push(protocollo);
                });

                interventi.push(intervento);
            }
        });

        return interventi;
    }
}