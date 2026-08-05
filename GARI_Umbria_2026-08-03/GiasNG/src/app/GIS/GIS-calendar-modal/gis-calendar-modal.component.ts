import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { TranslocoService } from '@jsverse/transloco';
import { SharedDataService } from 'app/GIS/services/shared-data.service';
import { GoogleMapGeoJsonService } from 'app/GIS/google-map/google-map-geojson.service';
import { TreeGisService } from 'app/Utility/Template/kendo-tree/services/tree-gis.service';
import { enum_OrigineChiamataLoadGeoJson } from '../GIS-enum/GIS-origine-chiamata';
import { Subject, Subscription, takeUntil } from 'rxjs';
import { AdvancedTimeFilterService } from '../services/advanced-time-filter.service';
import { DropdownListEvent, RadioButtonValue } from 'gias-ui-kit';
import { FiltroTemporale, FiltroTemporale_enum_OperatoreFiltroTemporale, FiltroTemporale_enum_TipoFiltroTemporale } from 'app/Service/api.service';
import { DateUtils, FiltroTemporaleAvanzato } from 'app/Utility/date-utils';


@Component({
    standalone: false,
    selector: 'app-gis-calendar-modal',
    templateUrl: './gis-calendar-modal.component.html',
    styleUrls: ['./gis-calendar-modal.component.css']
})
export class GISCalendarModalComponent implements OnInit, OnDestroy {

  public signal$: Subject<void> = new Subject();

  public isModalOpen = false;
  public tipoFiltroDataAbilitato = false;
  public isRicercaAvanzata = false;
  public labelTipoRicerca = null;
  public labelDataInizio = null;

  private subscriptionTimeFilter: Subscription;
  public filtroTemporalePeriodo: FiltroTemporale;
  public filtroTemporaleSingolaData: FiltroTemporale;

  public form: FormGroup | null;

  tipoFiltroData: Array<any> = [
    {
      codice: FiltroTemporale_enum_TipoFiltroTemporale.ValidiAllaData,
      descrizione: this.translocoService.translate('ValidiAllaData')
    }, {
      codice: FiltroTemporale_enum_TipoFiltroTemporale.ValidiSuAnnataAgrariaInCorso,
      descrizione: this.translocoService.translate('ValidiSuAnnataAgrariaInCorso')
    }, {
      codice: FiltroTemporale_enum_TipoFiltroTemporale.IntervalloTemporale,
      descrizione: this.translocoService.translate('IntervalloTemporale')
    }
  ];

  tipoOperatoreFiltroData: Array<RadioButtonValue> = [
    {
      value: FiltroTemporale_enum_OperatoreFiltroTemporale.PrecedenteUguale,
      name: this.translocoService.translate('PrecedenteAl'),
      enable: true
    },
    {
      value: FiltroTemporale_enum_OperatoreFiltroTemporale.SuccessivoUguale,
      name: this.translocoService.translate('SuccessivoAl'),
      enable: true
    }
  ];

  constructor(
    private fb: FormBuilder,
    private translocoService: TranslocoService,
    private sharedDataService: SharedDataService,
    private googleMapGeoJsonService: GoogleMapGeoJsonService,
    private treeGisService: TreeGisService,
    private advancedTimeFilterService: AdvancedTimeFilterService
  ) {
    this.filtroTemporalePeriodo = this.sharedDataService.getFiltroTemporaleAvanzato().filtroTemporalePeriodo;
    this.filtroTemporaleSingolaData = this.sharedDataService.getFiltroTemporaleAvanzato().filtroTemporaleSingolaData;
    this.caricaLabelTipoRicerca();
  }

  caricaFiltroneTemporale() {
    this.subscriptionTimeFilter = this.advancedTimeFilterService.getAdvancedTimeFilter().subscribe(filtroAvanzato => {
      this.filtroTemporalePeriodo = filtroAvanzato.filtroTemporalePeriodo;
      this.filtroTemporaleSingolaData = filtroAvanzato.filtroTemporaleSingolaData;
      this.aggiornamentoFiltrone();
    });
  }

  caricaLabelTipoRicerca() {
    if (this.isRicercaAvanzata) {
      this.labelTipoRicerca = this.translocoService.translate('RicercaSemplice');
    } else {
      this.labelTipoRicerca = this.translocoService.translate('RicercaAvanzata');
    }
  }

  public open() {
    this.isModalOpen = true;
    this.propostaDatiIniziale();
  }

  private propostaDatiIniziale() {

    this.propostaDatiForm(
      this.filtroTemporalePeriodo,
      this.filtroTemporaleSingolaData
    );

  }

  private caricaLabelDataInizio() {
    const objForm = this.form.value;
    if (objForm.ddlTipoFiltroData.codice === FiltroTemporale_enum_TipoFiltroTemporale.ValidiAllaData) {
      this.labelDataInizio = this.translocoService.translate('DataValidita');
    } else {
      this.labelDataInizio = this.translocoService.translate('DataInizio');
    }
  }

  close(): void {
    this.isModalOpen = false;
  }

  ngOnInit(): void {

    this.buildForm();

    this.form.controls['dataInizio'].valueChanges
      .pipe(takeUntil(this.signal$))
      .subscribe((newDataInizio: Date) => {
        this.proponiDataInizioSingola(newDataInizio);
      });

    this.form.controls['dataFine'].valueChanges
      .pipe(takeUntil(this.signal$))
      .subscribe((newDataFine: Date) => {
        this.proponiDataFineSingola(newDataFine);
      });

    this.caricaFiltroneTemporale();
  }

  private proponiDataInizioSingola(newDataInizio: Date) {
    const startDate = new Date();
    startDate.setHours(0, 0, 0, 0);
    const nuovaDataInizioSingola = DateUtils.calcolaDataInizioSingolaData(newDataInizio);
    this.form.controls['dataInizioSingola'].setValue(nuovaDataInizioSingola);
    const objForm = this.form.value;
    if (objForm.ddlTipoFiltroData.codice === FiltroTemporale_enum_TipoFiltroTemporale.ValidiAllaData) {
      this.proponiDataFineSingola(newDataInizio)
    }
  }

  private proponiDataFineSingola(newDataFine: Date) {
    const nuovaDataFineSingola = DateUtils.calcolaDataFineSingolaData(newDataFine);
    this.form.controls['dataFineSingola'].setValue(nuovaDataFineSingola);
  }

  private buildForm(): void {
    this.form = this.fb.group({
      // DDL tipo filtro data
      ddlTipoFiltroData: this.getElemFiltroDataByCodice(this.filtroTemporalePeriodo.TipoFiltroTemporale),
      // Data inizio
      dataInizio: this.filtroTemporalePeriodo.DataInizio,
      tipoOperatoreDataInizio: this.filtroTemporalePeriodo.TipoOperatoreDataInizio,
      // Data fine
      dataFine: this.filtroTemporalePeriodo.DataFine,
      tipoOperatoreDataFine: this.filtroTemporalePeriodo.TipoOperatoreDataFine,
      // Data inizio singola
      dataInizioSingola: this.filtroTemporaleSingolaData.DataInizio,
      // Data fine singola
      dataFineSingola: this.filtroTemporaleSingolaData.DataFine
    });
    this.caricaLabelDataInizio();
  }

  private getElemFiltroDataByCodice(codice: FiltroTemporale_enum_TipoFiltroTemporale): any {

    let elemFiltroData = null;

    let elementiFiltroData = this.tipoFiltroData.filter((obj) => {
      return obj.codice === codice;
    });

    if (elementiFiltroData.length > 0) {
      elemFiltroData = elementiFiltroData[0];
    }

    return elemFiltroData;

  }

  ngOnDestroy() {
    this.subscriptionTimeFilter.unsubscribe();
    this.signal$.next();
    this.signal$.complete();
  }

  onTipoFiltroDataValueChange(ev: DropdownListEvent) {
    this.caricaLabelDataInizio();
    const objForm = this.form.value;
    switch (objForm.ddlTipoFiltroData.codice) {

      case FiltroTemporale_enum_TipoFiltroTemporale.ValidiAllaData:
        const dataOggi = new Date();
        dataOggi.setHours(0, 0, 0, 0);
        const filtroTemporaleDataOggi = {
          TipoFiltroTemporale: FiltroTemporale_enum_TipoFiltroTemporale.ValidiAllaData,
          DataInizio: new Date(dataOggi),
          DataFine: new Date(dataOggi),
          TipoOperatoreDataInizio: FiltroTemporale_enum_OperatoreFiltroTemporale.SuccessivoUguale,
          TipoOperatoreDataFine: FiltroTemporale_enum_OperatoreFiltroTemporale.PrecedenteUguale,
        } as FiltroTemporale;
        this.propostaDatiFormCalcolaSingolaData(filtroTemporaleDataOggi)
        break;

      case FiltroTemporale_enum_TipoFiltroTemporale.ValidiSuAnnataAgrariaInCorso:
        const cfgGisGenerali = this.sharedDataService.getCfgGisGenerali();
        let filtroTemporaleAnnataAgraria = {
          TipoFiltroTemporale: objForm.ddlTipoFiltroData.codice,
          DataInizio: new Date(cfgGisGenerali.AnnataAgrariaInizio),
          DataFine: new Date(cfgGisGenerali.AnnataAgrariaFine),
          TipoOperatoreDataInizio: FiltroTemporale_enum_OperatoreFiltroTemporale.SuccessivoUguale,
          TipoOperatoreDataFine: FiltroTemporale_enum_OperatoreFiltroTemporale.PrecedenteUguale,
        } as FiltroTemporale;
        this.propostaDatiFormCalcolaSingolaData(filtroTemporaleAnnataAgraria);
        break;

      case FiltroTemporale_enum_TipoFiltroTemporale.IntervalloTemporale:
        break;

    }
  }

  private propostaDatiFormCalcolaSingolaData(filtroPeriodo: FiltroTemporale) {
    const startDate = new Date(filtroPeriodo.DataInizio);
    startDate.setHours(0, 0, 0, 0);
    const filtroTemporaleSingolaData = {
      TipoFiltroTemporale: FiltroTemporale_enum_TipoFiltroTemporale.IntervalloTemporale,
      DataInizio: DateUtils.calcolaDataInizioSingolaData(startDate),
      DataFine: DateUtils.calcolaDataFineSingolaData(filtroPeriodo.DataFine),
      TipoOperatoreDataInizio: FiltroTemporale_enum_OperatoreFiltroTemporale.SuccessivoUguale,
      TipoOperatoreDataFine: FiltroTemporale_enum_OperatoreFiltroTemporale.PrecedenteUguale,

    } as FiltroTemporale;
    this.propostaDatiForm(filtroPeriodo, filtroTemporaleSingolaData);
  }

  private propostaDatiForm(
    filtroPeriodo: FiltroTemporale,
    filtroTemporaleSingolaData: FiltroTemporale
  ): void {
    // Data inizio
    this.form.controls['dataInizio'].setValue(filtroPeriodo.DataInizio);
    this.form.controls['tipoOperatoreDataInizio'].setValue(filtroPeriodo.TipoOperatoreDataInizio);
    // Data fine
    this.form.controls['dataFine'].setValue(filtroPeriodo.DataFine);
    this.form.controls['tipoOperatoreDataFine'].setValue(filtroPeriodo.TipoOperatoreDataFine);
    // Data inizio singola
    this.form.controls['dataInizioSingola'].setValue(filtroTemporaleSingolaData.DataInizio);
    // Data fine singola
    this.form.controls['dataFineSingola'].setValue(filtroTemporaleSingolaData.DataFine);
    // Aggiorna label data inizio
    this.caricaLabelDataInizio();
  }

  onClickTipoRicerca() {
    this.isRicercaAvanzata = !this.isRicercaAvanzata;
    this.caricaLabelTipoRicerca();
  }

  applicaFiltro() {
    const objForm = this.form.value;
    //--------------------------------------------------------------------------------
    // Filtro temporale periodo
    //--------------------------------------------------------------------------------
    this.filtroTemporalePeriodo.TipoFiltroTemporale = objForm.ddlTipoFiltroData.codice;
    this.filtroTemporalePeriodo.DataInizio = objForm.dataInizio;
    // Se validi alla data forzo DataFine e gli operatori
    if (objForm.ddlTipoFiltroData.codice === FiltroTemporale_enum_TipoFiltroTemporale.ValidiAllaData) {
      this.filtroTemporalePeriodo.DataFine = objForm.dataInizio;
      this.filtroTemporalePeriodo.TipoOperatoreDataInizio = FiltroTemporale_enum_OperatoreFiltroTemporale.SuccessivoUguale;
      this.filtroTemporalePeriodo.TipoOperatoreDataFine = FiltroTemporale_enum_OperatoreFiltroTemporale.PrecedenteUguale;
    } else {
      this.filtroTemporalePeriodo.DataFine = objForm.dataFine;
      this.filtroTemporalePeriodo.TipoOperatoreDataInizio = objForm.tipoOperatoreDataInizio;
      this.filtroTemporalePeriodo.TipoOperatoreDataFine = objForm.tipoOperatoreDataFine;
    }
    //--------------------------------------------------------------------------------
    // Filtro temporale singola data
    //--------------------------------------------------------------------------------
    this.filtroTemporaleSingolaData.TipoFiltroTemporale = FiltroTemporale_enum_TipoFiltroTemporale.IntervalloTemporale;
    this.filtroTemporaleSingolaData.TipoOperatoreDataInizio = FiltroTemporale_enum_OperatoreFiltroTemporale.SuccessivoUguale;
    this.filtroTemporaleSingolaData.TipoOperatoreDataFine = FiltroTemporale_enum_OperatoreFiltroTemporale.PrecedenteUguale;
    this.filtroTemporaleSingolaData.DataInizio = objForm.dataInizioSingola;
    this.filtroTemporaleSingolaData.DataFine = objForm.dataFineSingola;
    //--------------------------------------------------------------------------------
    this.aggiornaFiltroTemporaleAvanzato();
    this.ricaricaFeaturePiuAlbero();
    // Chiusura finestra
    this.close();
  }

  aggiornamentoFiltrone() {
    this.aggiornaFiltroTemporaleAvanzato();
    this.ricaricaFeaturePiuAlbero();
    if (this.form) {
      this.propostaDatiForm(this.filtroTemporalePeriodo, this.filtroTemporaleSingolaData);
      this.isRicercaAvanzata = true;
      this.caricaLabelTipoRicerca();
      this.visualizzaRicercaSemplice();
    }
  }

  private visualizzaRicercaSemplice() {
    this.form.controls['ddlTipoFiltroData'].setValue(FiltroTemporale_enum_TipoFiltroTemporale.IntervalloTemporale);
    this.caricaLabelDataInizio();
  }

  private aggiornaFiltroTemporaleAvanzato() {
    const filtroTemporaleAvanzato = new FiltroTemporaleAvanzato();
    filtroTemporaleAvanzato.filtroTemporalePeriodo = this.filtroTemporalePeriodo;
    filtroTemporaleAvanzato.filtroTemporaleSingolaData = this.filtroTemporaleSingolaData;
    this.sharedDataService.setFiltroTemporaleAvanzato(filtroTemporaleAvanzato);
  }

  private ricaricaFeaturePiuAlbero() {
    this.sharedDataService.setOrigineChiamataLoadGeoJson(enum_OrigineChiamataLoadGeoJson.FiltroTemporale);
    this.googleMapGeoJsonService.loadGeoJsonForzato(true);
    this.seRicaricaAlbero();
  }

  private seRicaricaAlbero() {
    if (this.treeGisService) {
      this.treeGisService.loadDataInternalFilters(false);
    }
  }

  visualizzaDataFine(): boolean {
    const objForm = this.form.value;
    return objForm.ddlTipoFiltroData.codice !== FiltroTemporale_enum_TipoFiltroTemporale.ValidiAllaData || this.sharedDataService.getCfgSementiAsValue() != undefined;
  }

  checkFiltroTemporaleAvanzato() {
    const filtroPeriodo = this.sharedDataService.getFiltroTemporaleAvanzato().filtroTemporalePeriodo;
    const filtroTemporaleSingolaData = this.sharedDataService.getFiltroTemporaleAvanzato().filtroTemporaleSingolaData;
    // Controllo se i filtri temporali sono cambiati
    if (filtroPeriodo.DataInizio.getTime() !== this.filtroTemporalePeriodo.DataInizio.getTime() ||
      filtroPeriodo.DataFine.getTime() !== this.filtroTemporalePeriodo.DataFine.getTime() ||
      filtroTemporaleSingolaData.DataInizio.getTime() !== this.filtroTemporaleSingolaData.DataInizio.getTime() ||
      filtroTemporaleSingolaData.DataFine.getTime() !== this.filtroTemporaleSingolaData.DataFine.getTime()) {
        // Aggiorno i filtri temporali se sono cambiati
        this.filtroTemporalePeriodo = filtroPeriodo;
        this.filtroTemporaleSingolaData = filtroTemporaleSingolaData;
    }
  }
}
