
import { Component, ViewChild, AfterViewInit, OnDestroy, output } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { TranslocoService } from "@jsverse/transloco";
import { Subject, take, takeUntil, tap, map, filter, forkJoin } from 'rxjs';
import { GiasDropDownTemplateService, GiasMultiSelectTemplateService, GiiasMultiselectTemplateSComponent, GiasDropDownTemplateSComponent } from 'gias-ui-kit';
import { SHARED_IMPORTS } from 'app/qualita-tracciabilita/qualita-tracciabilita.module';
import { BaseCodeDescr } from 'app/Model/baseClass/baseCodeDescr';
import { FiltriAnalisiConformitaForm, FiltriAnalisiConformita, FiltriAnalisiConformitaDes } from 'app/qualita-tracciabilita/models/filtri-nuova-analisi-conformita.model';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { MasterService } from 'app/Service/master.service';
import { VisibilitaService } from 'app/profilazione/services/visibilita.service';
import { BaseCodeDescrStr } from 'app/Model/baseClass/baseCodeDescrStr';
import { CentriAziendaliService } from 'app/Service/Anagrafica/centri.service';
import { SpecieVegetaliService } from 'app/Service/Metaschema/specie-vegetali.service';
import { ImpiantiService } from 'app/Service/Anagrafica/impianti.service';
import { DisciplinariService } from 'app/Service/Metaschema/disciplinari.service';
import { VerificaDisciplinariService } from 'app/qualita-tracciabilita/services/verifica-disciplinari.service';
import { OperationTypes } from "app/qualita-tracciabilita/models/operation-types.enum";
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { enum_Impostazioni_Utenti } from 'app/Model/Impostazioni_Utenti.enum';
import { IntervalloTemporale } from 'app/Model/anagrafiche/IntervalloTemporale';
import { GiasMessageService } from "app/Service/gias-message.service";


@Component({
  standalone: true,
  selector: 'app-filtri-nuova-analisi-conformita',
  imports: [SHARED_IMPORTS],
  providers: [GiasDropDownTemplateService, GiasMultiSelectTemplateService],
  templateUrl: './filtri-nuova-analisi-conformita.component.html',
  styleUrl: './filtri-nuova-analisi-conformita.component.css'
})
export class FiltriNuovaAnalisiConformitaComponent implements AfterViewInit, OnDestroy {
  @ViewChild('company') companyDdl: GiasDropDownTemplateSComponent | undefined;
  @ViewChild('sacod') sacodMulti: GiiasMultiselectTemplateSComponent | undefined;
  @ViewChild('vegcod') vegcodMulti: GiiasMultiselectTemplateSComponent | undefined;
  @ViewChild('impianti') impiantiMulti: GiiasMultiselectTemplateSComponent | undefined;
  @ViewChild('operazioni') operazioniMulti: GiiasMultiselectTemplateSComponent | undefined;
  @ViewChild('protocolli') protocolliMulti: GiasDropDownTemplateSComponent | undefined;

  onSearch = output<FiltriAnalisiConformitaDes>();

  protected innerForm = new FormGroup<FiltriAnalisiConformitaForm>(new FiltriAnalisiConformitaForm());

  protected companies: BaseCodeDescrStr[] = [];
  protected businessCenters: BaseCodeDescr[] = [];
  protected species: BaseCodeDescr[] = [];
  protected operations: BaseCodeDescr[] = [
    new BaseCodeDescr(OperationTypes.All, this.transloco.translate('TutteLeOperazioni')),
    new BaseCodeDescr(OperationTypes.Treatment, this.transloco.translate('Trattamenti')),
    new BaseCodeDescr(OperationTypes.Fertilization, this.transloco.translate('Fertilizzazioni')),
    new BaseCodeDescr(OperationTypes.Harvest, this.transloco.translate('Raccolte')),
  ];
  protected plots: BaseCodeDescrStr[] = [];
  protected protocols: BaseCodeDescrStr[] = [];
  protected defaultBusinessCenter = new BaseCodeDescr(0, this.transloco.translate('TuttiICentriAziendali'));
  protected defaultProtocol = new BaseCodeDescrStr('', this.transloco.translate('IndicatoInOperazione'));
  protected defaultSpecie = new BaseCodeDescr(0, this.transloco.translate('TutteLeSpecie'));
  protected defaultPlot = new BaseCodeDescrStr('', this.transloco.translate('TuttiGliImpianti'));
  protected defaultOperation = this.operations[0];

  private _signal$ = new Subject<void>();

  constructor(
    private transloco: TranslocoService,
    private master: MasterService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private visibilitaService: VisibilitaService,
    private centriService: CentriAziendaliService,
    private specieService: SpecieVegetaliService,
    private impiantiService: ImpiantiService,
    private disciplinariService: DisciplinariService,
    private permessiUtenteService: PermessiUtenteService,
    private message: GiasMessageService
  ) {
    //this.loadOperations();
    this.setDatesFromSettings();
  }

  private get piva(): string { return this.innerForm.get('piva').value; }
  private get sa_cod(): number[] { return this.innerForm.get('sa_cod').value; }
  private get veg_cod(): number[] { return this.innerForm.get('veg_cod').value; }
  private get startDate(): Date { return this.innerForm.get('data_da').value; }
  private get endDate(): Date { return this.innerForm.get('data_a').value; }

  ngAfterViewInit(): void {
    this.handleChanges();
    const paramAgenda = this.objParametriAgendaService.getObjParamValue();
    this.companies = [new BaseCodeDescrStr(paramAgenda.Piva, paramAgenda.RagSoc)];
    this.innerForm.get('piva').setValue(paramAgenda.Piva);
    this.loadCompanies();
    this.toggleLoading(this.protocolliMulti, true);
  }

  ngOnDestroy(): void {
    this._signal$.next();
    this._signal$.complete();
  }

  /**
   * Handles the form submission event.
   *
   * This method first simplifies the current filter values, then retrieves the form values
   * as a `FiltriAnalisiConformita` object. It passes these filters to the `analisiService`
   * to initiate the conformity analysis process.
   */
  onSubmit() {
    if (this.validateNewRequest()) {
      this.simplifyFilters();
      const filters: FiltriAnalisiConformitaDes = this.innerForm.value as FiltriAnalisiConformitaDes;
      filters.data_richiesta = new Date();
      filters.rag_soc = this.companies.find(c => c.codice === filters.piva)?.descrizione ?? "";
      filters.sa_nome = this.businessCenters.filter(b => filters.sa_cod.includes(b.codice)).map(b => b.descrizione).join(', ');
      filters.veg_des = this.species.filter(s => filters.veg_cod.includes(s.codice)).map(s => s.descrizione).join(', ');
      filters.impianti_des = this.plots.filter(p => filters.impianti.includes(p.codice)).map(p => p.descrizione).join(', ');
      filters.operazioni_des = this.operations.filter(o => filters.operazioni.includes(o.codice)).map(o => o.descrizione).join(', ');
      filters.dpi_des = this.protocols.find(p => p.codice === filters.dpi)?.descrizione ?? "";
      this.onSearch.emit(filters);
    }
  }

  private validateNewRequest(): boolean {
    const filters: FiltriAnalisiConformitaDes = this.innerForm.value as FiltriAnalisiConformitaDes;
    if (!(filters.flagMagazzino || filters.flagNormative)) {
      this.message.errorMessage("SelezionareAlmenoUnaModalitaVerifica", false, true);
      return false;
    }
    return true;
  }

  /**
   * Simplifies the filter selections in the form by applying the following rules:
   * - If 'sa_cod' contains more than one value and includes 0, it is reset to only [0].
   * - If 'veg_cod' contains more than one value and includes 0, it is reset to only [0].
   * - If 'impianti' contains more than one value and includes an empty string, it is reset to an empty array.
   * - If 'operazioni' contains more than one value and includes 0, it is reset to only [0].
   *
   * This ensures that the filter selections do not contain conflicting or redundant values.
   */
  private simplifyFilters() {
    const filters = this.innerForm.value as FiltriAnalisiConformita;
    if (filters.sa_cod.length > 1 && filters.sa_cod.includes(0)) this.innerForm.get('sa_cod').setValue([0]);
    if (filters.veg_cod.length > 1 && filters.veg_cod.includes(0)) this.innerForm.get('veg_cod').setValue([0]);
    if (filters.impianti.length > 1 && filters.impianti.includes("")) this.innerForm.get('impianti').setValue([]);
    if (filters.operazioni.length > 1 && filters.operazioni.includes(OperationTypes.All))
      this.innerForm.get('operazioni').setValue([OperationTypes.All]);
  }

  private setDatesFromSettings() {
    const annata = this.permessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.UTENTE_InizioFineAnnataAgraria);
    const values = annata?.Valore?.match(/.{2}/g);
    if (values && values.length == 4) {
      const startDay = +values[0];
      const startMonth = +values[1];
      const endDay = +values[2];
      const endMonth = +values[3];
      const currentYear = new Date().getFullYear();
      let startDate = new Date(currentYear, startMonth - 1, startDay);
      let endDate = new Date(currentYear, endMonth - 1, endDay);
      if (endDate < new Date()) {
        startDate.setFullYear(currentYear - 1);
      }
      if (startDate > endDate) {
        endDate.setFullYear(currentYear + 1);
      }
      this.innerForm.get('data_da').patchValue(startDate);
      this.innerForm.get('data_a').patchValue(endDate);
    }
  }

  private toggleLoading(ddl: GiiasMultiselectTemplateSComponent | GiasDropDownTemplateSComponent, active?: boolean) {
    if (!ddl) return;
    ddl.loading = active !== undefined ? active : !ddl.loading;
  }

  private isDateIntervalValid(): boolean {
    return this.startDate && this.endDate && this.startDate <= this.endDate;
  }

  // #region Load Data
  private loadCompanies() {
    if (this.companyDdl != undefined) this.companyDdl.loading = true;
    this.visibilitaService.leggiVisibilitaUtente(this.master.getCurrentUserUsername(), false, false)
      .pipe(take(1), tap(() => { if (this.companyDdl != undefined) this.companyDdl.loading = false; }))
      .subscribe(vis => this.companies = vis.map(x => new BaseCodeDescrStr(x.piva, x.rag_soc)));
  }

  private loadCompanyCenters() {
    this.toggleLoading(this.sacodMulti);
    const p = this.objParametriAgendaService.getObjParamValue();
    p.piva = this.innerForm.get('piva').value;
    this.centriService.leggiCentri(p).pipe(
      tap(centri => this.businessCenters = centri.map(c => new BaseCodeDescr(c.sa_cod, c.sa_nome))),
      tap(() => this.toggleLoading(this.sacodMulti))
    ).subscribe(() => {
      if (this.businessCenters.length == 1) {
        this.innerForm.get('sa_cod').setValue([this.businessCenters[0].codice]);
      } else {
        this.businessCenters.unshift(this.defaultBusinessCenter);
        this.innerForm.get('sa_cod').setValue([this.defaultBusinessCenter.codice]);
      }
    });
  }

  private loadSpecies() {
    this.toggleLoading(this.vegcodMulti);
    const sa_cod = this.sa_cod.length == 1 ? this.sa_cod[0] : 0;
    this.specieService.leggiPerCentroAziendale(this.piva, sa_cod)
      .pipe(tap(() => this.toggleLoading(this.vegcodMulti)))
      .subscribe((species) => {
        this.species = species;
        if (this.species.length == 1) {
          this.innerForm.get('veg_cod').setValue([this.species[0].codice]);
        } else {
          this.species.unshift(this.defaultSpecie);
          this.innerForm.get('veg_cod').setValue([this.defaultSpecie.codice]);
        }
      });
  }

  private loadPlots() {
    if (!this.isDateIntervalValid()) return;

    this.toggleLoading(this.impiantiMulti);
    this.impiantiService.leggiPerCentroSpecie(
      this.piva, this.sa_cod.length == 1 ? this.sa_cod[0] : 0,
      this.veg_cod.length == 1 ? this.veg_cod[0] : 0,
      this.startDate, this.endDate
    ).pipe(tap(() => this.toggleLoading(this.impiantiMulti)))
      .subscribe((plots) => {
        this.plots = plots;
        if (this.plots.length == 1) {
          this.innerForm.get('impianti').setValue([this.plots[0].codice]);
        } else {
          this.plots.unshift(this.defaultPlot);
          this.innerForm.get('impianti').setValue([this.defaultPlot.codice]);
        }
      });
  }

  private loadProtocols() {
    if (!this.isDateIntervalValid()) return;

    this.toggleLoading(this.protocolliMulti, true);
    this.disciplinariService.leggi({
      lavorazione: null,
      specie: new BaseCodeDescr(this.veg_cod.length == 1 ? this.veg_cod[0] : 0),
      data: new Date(),
      privato: false,
      regolamento: null,
      validita: new IntervalloTemporale(this.startDate, this.endDate),
      IncludiNessunDisciplinare: true,
      IncludiBiologico: true
    }).pipe(tap(protocols => this.protocols = (protocols ?? []) as BaseCodeDescrStr[]))
      .subscribe(() => {
        const dpi: string = this.innerForm.get('dpi').value;
        if (this.protocols.every(x => x.codice !== dpi)) {
          this.innerForm.get('dpi').setValue(this.defaultProtocol.codice);
        }
        this.toggleLoading(this.protocolliMulti);
      });
  }
  // #endregion

  // #region Event Handlers

  /**
   * Subscribes to value changes of specific form controls within `innerForm` and triggers
   * corresponding reset or load operations based on the new values.
   *
   * - When the 'piva' control changes, resets centers and species selections.
   * - When the 'sa_cod' control changes and the new value array has exactly one element,
   *   resets the plots selection.
   * - When the 'veg_cod' control changes and the new value array has exactly one element,
   *   resets the plots selection and loads protocols.
   *
   * All subscriptions are automatically unsubscribed when the `_signal$` observable emits.
   *
   * @private
   */
  private handleChanges() {
    this.innerForm.get('piva').valueChanges.pipe(takeUntil(this._signal$))
      .subscribe(() => {
        this.resetCenters();
        this.resetSpecies();
      });

    this.innerForm.get('sa_cod').valueChanges
      .pipe(takeUntil(this._signal$), filter(centers => centers.length == 1))
      .subscribe(() => this.resetPlots());

    this.innerForm.get('veg_cod').valueChanges
      .pipe(takeUntil(this._signal$), filter(species => species.length == 1))
      .subscribe(() => {
        this.resetPlots();
        this.loadProtocols();
      });

    this.innerForm.get('data_da').valueChanges.pipe(takeUntil(this._signal$), filter(() => this.isDateIntervalValid()))
      .subscribe(() => {
        this.resetPlots();
        this.loadProtocols();
      });
    this.innerForm.get('data_a').valueChanges.pipe(takeUntil(this._signal$), filter(() => this.isDateIntervalValid()))
      .subscribe(() => {
        this.resetPlots();
        this.loadProtocols();
      });
  }

  private resetCenters() {
    this.businessCenters = [];
    this.innerForm.get('sa_cod').setValue([]);
    this.loadCompanyCenters();
  }

  private resetPlots() {
    this.plots = [];
    this.innerForm.get('impianti').setValue([]);
    this.loadPlots();
  }

  private resetSpecies() {
    this.species = [];
    this.innerForm.get('veg_cod').setValue([]);
    this.loadSpecies();
  }

  // #endregion
}
