import { Component, AfterViewInit, OnDestroy, ViewChild, TemplateRef, AfterViewChecked, AfterContentChecked } from '@angular/core';
import { TranslocoService } from "@jsverse/transloco";
import { SHARED_IMPORTS } from 'app/qualita-tracciabilita/qualita-tracciabilita.module';
import { RiepilogoRichiesteGridService } from './riepilogo-richieste-grid.service';
import { generateGridProviders, GiasKendoGridComponent } from 'gias-kendo-grid';
import { VerificaDisciplinariService } from 'app/qualita-tracciabilita/services/verifica-disciplinari.service';
import { RisultatiAnalisiConformitaComponent } from 'app/qualita-tracciabilita/components/risultati-analisi-conformita/risultati-analisi-conformita.component';
import { FiltriNuovaAnalisiConformitaComponent } from 'app/qualita-tracciabilita/components/filtri-nuova-analisi-conformita/filtri-nuova-analisi-conformita.component';
import { GiasDialogService } from 'app/Service/gias-dialog.service'
import { faPlus, faArrowRotateRight } from '@fortawesome/free-solid-svg-icons';
import { FiltriAnalisiConformitaDes, FiltriAnalisiConformita } from 'app/qualita-tracciabilita/models/filtri-nuova-analisi-conformita.model';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { FormGroup, FormControl } from '@angular/forms';
import { BaseCodeDescr } from 'app/Model/baseClass/baseCodeDescr';
import { BaseCodeDescrStr } from 'app/Model/baseClass/baseCodeDescrStr';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { SpecieVegetaliService } from 'app/Service/Metaschema/specie-vegetali.service';
import { VisibilitaService } from 'app/profilazione/services/visibilita.service';
import { GiasDropDownTemplateService, GiasMultiSelectTemplateService, GiiasMultiselectTemplateSComponent, GiasDropDownTemplateSComponent } from 'gias-ui-kit';
import { tap, map, takeUntil, Subject } from 'rxjs';
import { MasterService } from 'app/Service/master.service';
import { CookieService } from "app/Service/cookie.service";
import { RequestFiltersHelperService } from 'app/qualita-tracciabilita/services/request-filters-helper.service';
import { RisultatiAnalisiMagazzinoComponent } from 'app/qualita-tracciabilita/components/risultati-analisi-magazzino/risultati-analisi-magazzino.component';

@Component({
  standalone: true,
  selector: 'app-riepilogo-richieste',
  imports: [
    SHARED_IMPORTS,
    RisultatiAnalisiConformitaComponent,
    RisultatiAnalisiMagazzinoComponent,
    FiltriNuovaAnalisiConformitaComponent,
  ],
  templateUrl: './riepilogo-richieste.component.html',
  styleUrl: './riepilogo-richieste.component.css',
  providers: [...generateGridProviders(RiepilogoRichiesteGridService, RiepilogoRichiesteComponent),
    GiasDropDownTemplateService, GiasMultiSelectTemplateService
  ]
})
export class RiepilogoRichiesteComponent implements AfterViewInit, AfterContentChecked, OnDestroy {
  @ViewChild('company') companyDdl: GiasDropDownTemplateSComponent | undefined;
  @ViewChild('vegcod') vegcodMulti: GiiasMultiselectTemplateSComponent | undefined;
  @ViewChild('sacod') sacodMulti: GiiasMultiselectTemplateSComponent | undefined;

  @ViewChild('aDetailsRef') public aDetailsRef: TemplateRef<any>;
  @ViewChild('newRequestForm') public newRequestForm: TemplateRef<any>;

  protected readonly faPlus = faPlus;
  protected readonly faSync = faArrowRotateRight;
  protected readonly defaultSpecie = new BaseCodeDescr(0, this.transloco.translate('TutteLeSpecie'));
  protected companies: BaseCodeDescrStr[] = [];
  protected centers: BaseCodeDescr[] = [this.filtersHelper.defaultBusinessCenter];
  protected species: BaseCodeDescr[] = [this.filtersHelper.defaultSpecie];
  protected showRegulamentations = true;
  protected showStorages = true;
  protected tabSelectedIdx = 0;

  protected searchFilters: FormGroup = new FormGroup({
    piva: new FormControl(''),
    sa_cod: new FormControl([0]),
    veg_cod: new FormControl([0]),
    data_da: new FormControl(this.monthStart),
    data_a: new FormControl(this.monthEnd),
    flagNormative: new FormControl(true),
    flagMagazzino: new FormControl(true),
  });
  private _openDialog: DialogRef;
  private _destroy$ = new Subject<void>();

  constructor(
    private analisiService: VerificaDisciplinariService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private visibilitaService: VisibilitaService,
    private specieService: SpecieVegetaliService,
    private master: MasterService,
    private cookies: CookieService,
    private dialog: GiasDialogService,
    private transloco: TranslocoService,
    private filtersHelper: RequestFiltersHelperService
  ) {
    this.initFilters();
    this.filterRequests();
    this.loadCompanies();
    this.loadCenters();
    this.loadSpecies();

    this.analisiService.risultatoAnalisi$.pipe(takeUntil(this._destroy$)).subscribe(() => {
      this.showStorages = this.analisiService.analysisDataItem.Storage;
      this.showRegulamentations = this.analisiService.analysisDataItem.Regulations;

      if (!this.showRegulamentations)
        this.tabSelectedIdx = 1;
      else
        this.tabSelectedIdx = 0;
    });
  }

  public ngOnDestroy(): void {
    this._destroy$.next();
    this._destroy$.complete();
  }

  private get piva(): string {
    return this.searchFilters.get('piva').value;
  }

  private get sacod(): number[] {
    return this.searchFilters.get('sa_cod').value;
  }

  public get monthStart(): Date {
    const now = new Date();
    return new Date(now.getFullYear(), now.getMonth(), 1);
  }

  public get monthEnd(): Date {
    const now = new Date();
    return new Date(now.getFullYear(), now.getMonth() + 1, 0);
  }

  public ngAfterViewInit(): void {
    this.analisiService.analysisDetailRef = this.aDetailsRef;
  }

  public ngAfterContentChecked(): void {
    document.querySelector(".k-dialog-content")?.setAttribute("style", "overflow: auto");
  }

  public createNew() {
    console.debug('new req');
    this._openDialog = this.dialog.dialogMessageRef(
      this.transloco.translate('NuovaRichiestaVerificaConformita'),
      this.newRequestForm, [], '50%', '85%');
  }

  public refreshGrid() {
    console.debug('refreshGrid');
    this.filterRequests();
  }

  public forwardNewRequest(filters: FiltriAnalisiConformitaDes) {
    this._openDialog.close();
    this.analisiService.sendNewAnalysisRequest(filters);
    document.querySelector('button[id="syncRequestsBtn"]')["disabled"] = false;
  }

  public filterRequests() {
    this.master.set_isLoading({ isLoading: true });
    let filtersValue = new FiltriAnalisiConformita();
    filtersValue.piva = this.searchFilters.value.piva;
    filtersValue.sa_cod = this.searchFilters.value.sa_cod;
    filtersValue.veg_cod = this.searchFilters.value.veg_cod;
    filtersValue.data_da = this.searchFilters.value.data_da;
    filtersValue.data_a = this.searchFilters.value.data_a;
    filtersValue.flagNormative = this.searchFilters.value.flagNormative;
    filtersValue.flagMagazzino = this.searchFilters.value.flagMagazzino;
    this.analisiService.readActiveAnalysisRequests(filtersValue)
      .subscribe(() => this.master.set_isLoading({ isLoading: false }));
  }

  private initFilters() {
    const paramAgenda = this.objParametriAgendaService.getObjParamValue();
    this.companies = [new BaseCodeDescrStr(paramAgenda.Piva, paramAgenda.RagSoc)];
    this.searchFilters.get('piva').setValue(paramAgenda.Piva);
  }

  private loadCompanies() {
    if (this.companyDdl != undefined) this.companyDdl.loading = true;
    this.filtersHelper.loadCompanies()
      .pipe(tap(() => { if (this.companyDdl != undefined) this.companyDdl.loading = false; }))
      .subscribe(companies => this.companies = companies);

    this.searchFilters.get('piva').valueChanges.subscribe(() => {
      this.loadCenters();
      this.loadSpecies();
    });
  }

  private loadCenters() {
    this.toggleLoading(this.sacodMulti);
    this.centers = [this.filtersHelper.defaultBusinessCenter];
    this.filtersHelper.loadCompanyCenters(this.piva)
      .pipe(
        tap(() => this.toggleLoading(this.sacodMulti)),
        map(centers => centers.filter(c => c.codice != undefined))
      ).subscribe(centers => {
        if (centers.length != 1 && centers.findIndex(c => c.codice == 0) == -1) {
          centers.unshift(this.filtersHelper.defaultBusinessCenter);
        }
        this.centers = centers;
        const value = centers.length == 1 ? centers[0].codice : this.filtersHelper.defaultBusinessCenter.codice;
        this.searchFilters.get('sa_cod').setValue([value]);
      });
  }

  private loadSpecies() {
    this.toggleLoading(this.vegcodMulti);
    this.filtersHelper.loadSpecies(this.piva, this.sacod.length == 1 ? this.sacod[0] : 0)
      .pipe(tap(() => this.toggleLoading(this.vegcodMulti)))
      .subscribe((species) => {
        this.species = species;
        const value = species.length == 1 ? species[0].codice : this.filtersHelper.defaultSpecie.codice;
        this.searchFilters.get('veg_cod').setValue([value]);
      });
  }

  private toggleLoading(ddl: GiiasMultiselectTemplateSComponent, toggle?: boolean) {
    if (!ddl) return;
    ddl.loading = toggle == undefined ? !ddl.loading : toggle;
  }

}
