import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { Subject, Subscription, takeUntil, map } from 'rxjs';
import { TranslocoService } from '@jsverse/transloco';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { GiasDropDownTemplateSComponent, ObjParametriAgenda } from 'gias-ui-kit';
import { generateGridProviders } from 'gias-kendo-grid';
import { GridPublicService } from 'gias-kendo-grid';
import { faMagnifyingGlass, faSliders, faXmark } from '@fortawesome/free-solid-svg-icons';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { MasterService } from 'app/Service/master.service';
import { SpecieVegetaliService } from 'app/Service/Metaschema/specie-vegetali.service';
import { VarietaService } from 'app/Service/Metaschema/varieta.service';
import { RegolamentiService } from 'app/Service/Metaschema/regolamenti.service';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { ImpreseFilterService } from 'app/Master/menu-contestuale/imprese-filter/imprese-filter.service';
import { ImpreseParametriGHGService } from './imprese-parametri-ghg.service';
import { ImpreseParametriGHGGridService } from './imprese-parametri-ghg-grid.service';
import { DrawerComponent } from '@progress/kendo-angular-layout';

@Component({
    standalone: false,
    selector: 'app-imprese-parametri-ghg',
    templateUrl: './imprese-parametri-ghg.component.html',
    styleUrls: ['./imprese-parametri-ghg.component.css'],
    providers: [
        ...generateGridProviders(ImpreseParametriGHGGridService, ImpreseParametriGHGComponent)]
})
export class ImpreseParametriGHGComponent implements OnInit, OnDestroy, AfterViewInit {

    @ViewChild('drawer') drawer: DrawerComponent;

    datiCaricati: boolean = false;
    objParametriAgenda: ObjParametriAgenda;
    formEnable: boolean;

    binaryButtonEnable: boolean;
    binaryButtonValue: boolean;
    binaryButtonName: string;

    faSearch = faMagnifyingGlass;
    faSliders = faSliders;
    faClose = faXmark;

    displayButton: boolean = true;

    private inFrame: boolean;

    defaultAzienda = { codice: "", descrizione: "" }
    defaultVarieta = { codice: 0, descrizione: "" }
    defaultRegolamento = { codice: 0, descrizione: "" }

    formCaricaDati: Subscription;
    valueChangesSub: Subscription;

    public signal$: Subject<void> = new Subject();

    showFilters: boolean = false;
    drawerWidth: number;

    impreseParametriGHGForm: FormGroup = new FormGroup({
        azienda: new FormControl([]),
        specie: new FormControl([]),
        varieta: new FormControl([this.defaultVarieta]),
        regolamento: new FormControl([this.defaultRegolamento]),
        validita: this.fb.group({
            inizio: [AGRODATAINIZIO],
            fine: [AGRODATAFINE]
        })
    });

    constructor(

        private masterService: MasterService,
        private transloco: TranslocoService,
        private fb: FormBuilder,
        private router: Router,
        private location: Location,
        private giasMessageService: GiasMessageService,
        private giasDialogService: GiasDialogService,
        private translocoService: TranslocoService,
        private impreseFiltroService: ImpreseFilterService,
        private specieService: SpecieVegetaliService,
        private varietaService: VarietaService,
        private regolamentoService: RegolamentiService,
        private gridPublicService: GridPublicService,
        private route: ActivatedRoute,
        private impreseParametriGHGService: ImpreseParametriGHGService,
        private objParametriAgendaService: ObjParametriAgendaService
    ) { }

    async ngOnInit() {
        this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue()
        this.handleQueryParams();
        this.formSettings();
    }

    ngAfterViewInit(): void {
    }

    applyFilters() {
        let form = this.impreseParametriGHGForm.controls
        if (form.validita.value.inizio > form.validita.value.fine) {
            this.giasMessageService.errorMessage(this.transloco.translate('DataInizioNonDeveEssereMaggioreDiDataFine'));
            return;
        }
        this.impreseParametriGHGService.setFiltroRicerca({
            piva: form.azienda.value.codice,
            specie: form.specie.value.map(t => t.codice),
            culCod: form.varieta.value.codice,
            regolamentoCod: form.regolamento.value.codice,
            dataValiditaInizio: form.validita.value.inizio,
            dataValiditaFine: form.validita.value.fine,
        });
        this.gridPublicService.refresh(true);
        this.onCancel();
    }

    formSettings() {
        this.impreseParametriGHGForm.controls['specie'].valueChanges.pipe(map(R => {
            this.impreseParametriGHGForm.controls['varieta'].patchValue(this.defaultVarieta);
            if (R.length <= 1) {
                this.impreseParametriGHGForm.controls['varieta'].enable();
            } else {
                this.impreseParametriGHGForm.controls['varieta'].disable();
            }
        })).subscribe();

        this.impreseParametriGHGForm.controls['azienda'].valueChanges.pipe(map(R => {
            this.impreseParametriGHGService.setImpresaSelezionata({ codice: R.codice, descrizione: R.descrizione })
        })).subscribe();

        this.impreseParametriGHGForm.controls['azienda'].patchValue({ codice: this.objParametriAgenda.Piva, descrizione: this.objParametriAgenda.RagSoc })
        this.impreseParametriGHGForm.controls['specie'].patchValue([])
        this.impreseParametriGHGForm.controls['varieta'].patchValue(this.defaultVarieta)
        this.impreseParametriGHGForm.controls['regolamento'].patchValue(this.defaultRegolamento)
        this.impreseParametriGHGForm.controls['validita'].patchValue({
            inizio: AGRODATAINIZIO,
            fine: AGRODATAFINE
        })
        this.applyFilters()
    }

    ngOnDestroy(): void {
        if (this.formCaricaDati != undefined) {
            this.formCaricaDati.unsubscribe();
        }
        if (this.valueChangesSub != undefined) {
            this.valueChangesSub.unsubscribe();
        }

        this.signal$.next();
        this.signal$.complete();
    }

    private handleQueryParams(): void {
        this.route.queryParams.pipe(takeUntil(this.signal$))
            .subscribe(params => {
                if (params.seFrame == 1) {
                    let header = this.masterService.getHeader();
                    header.visible = false;
                    this.masterService.changeHeader(header);

                    let footer = this.masterService.getFooter();
                    footer.visible = false;
                    this.masterService.changeFooter(footer);

                    this.inFrame = true;
                }
            });
    }

    async openDdl(ddlEl: GiasDropDownTemplateSComponent) {

        switch (ddlEl.giasFormControlName) {
            case 'azienda':
                if (ddlEl.listItems == undefined || ddlEl.listItems.length <= 1) {
                    ddlEl.loading = true;
                    this.impreseFiltroService.filtraImprese('').pipe(map(R => {
                        ddlEl.loading = false;
                        ddlEl.listItems = R.map(t => { return { codice: t.partitaIva, descrizione: t.ragioneSociale } })
                    })).subscribe();
                }
                break;
            case 'specie':
                ddlEl.listItems = await this.specieService.leggi();
                break;
            case 'varieta':
                if (this.impreseParametriGHGForm.controls['specie'].value.length == 1) {
                    this.varietaService.leggiAsObs(new Specie(this.impreseParametriGHGForm.controls['specie'].value[0].codice))
                        .subscribe(items => ddlEl.listItems = items);
                } else {
                    ddlEl.listItems = [];
                }
                break;
            case 'regolamento':
                this.regolamentoService.leggi().pipe(map(R => {
                    ddlEl.listItems = R.map(t => { return { codice: t.codice, descrizione: t.descrizione } })
                })).subscribe();
                break;
        }
    }
    onCancel() {
        this.displayButton = true;
        this.showFilters = false;

        if (!this.drawer)
            return;

        if (!!this.drawer.expanded) {
            this.drawer.toggle();
        }
    }

    onOpenFilters() {
        this.showFilters = true;
        this.displayButton = false;
        this.drawer.toggle();
    }
}
