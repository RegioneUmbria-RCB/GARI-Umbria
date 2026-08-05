import { Component, EventEmitter, OnDestroy, OnInit, Output } from '@angular/core';
import { map, Observable, skip, Subject, takeUntil } from 'rxjs';
import { DropdownListItem } from 'gias-kendo-grid';
import { TreeGisService } from '../services/tree-gis.service';
import { TreeContainerService } from '../services/tree-container.service';
import { TreeGisFiltersService } from './gis-tree-filters.service';
import { faAngleDown, faAngleUp, faArrowsRotate, faFilter, faSearch, faBars, faCircleXmark } from '@fortawesome/free-solid-svg-icons';
import { AnagraficaService } from 'app/anagrafica/anagrafica.service';
import { GisService } from 'app/GIS/GIS.service';
import { SharedDataService } from 'app/GIS/services/shared-data.service';
import { consoleLogDebug } from 'app/Service/utils';
import { enum_logDebugArea, enum_logDebugTipo } from 'app/Model/log-debug';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { Enum_TipoComportamento_FiltroRicerca, Enum_TipoMostra_FiltroRicerca, enum_PagineGiasNG, enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { TranslocoService } from '@jsverse/transloco';
import { GestioneRichiesteService } from 'app/Service/gestione-richieste.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { GiasIFrameWindowService } from 'gias-ui-kit';
import { Enum_SiteRedirector } from 'app/Model/siti.enum';
import { contestoPostMessage, PostMessageSelezioneFiltrone } from 'gias-ui-kit';
import { AggiornaFiltroImpianti_In, ChiaveImpianto_In, FiltroTemporale, FiltroTemporale_enum_OperatoreFiltroTemporale, FiltroTemporale_enum_TipoFiltroTemporale, GisClient } from 'app/Service/api.service';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { enum_TipoChiaveFiltrone } from 'app/Model/Filtrone';
import { AdvancedTimeFilterService } from 'app/GIS/services/advanced-time-filter.service';
import moment from 'moment';
import { ParametriFiltroRicercaNG } from 'app/filtro-ricerca/utils';
import { DateUtils, FiltroTemporaleAvanzato } from 'app/Utility/date-utils';

const DEFAULT_ITEM_POSITION = 0;

export interface GisTreeFilters {
  centro: DropdownListItem
};

@Component({
  standalone: false,
  selector: 'tree-filters-gis',
  templateUrl: './gis-tree-filters.component.html',
  styleUrls: ['./gis-tree-filters.component.scss'],
  providers: []
})
export class GisTreeFiltersComponent implements OnInit, OnDestroy {
  @Output() filtersChanged: EventEmitter<GisTreeFilters> = new EventEmitter();

  visualizzaFiltroCatasto = false;
  permessoVisualizzazioneTotale = false;
  visualizzazioneTotale = false;
  centriAziendaliDDL: Observable<DropdownListItem[]>;
  signal: Subject<void> = new Subject();
  currentCentro: DropdownListItem;
  faReload = faArrowsRotate;
  faFilter = faFilter;
  faCircleXmark = faCircleXmark;
  faBars = faBars;
  filtroCatasto: boolean = false;
  iconaFrecciaFiltroCatasto = faAngleDown;
  faSearch = faSearch;
  filtroAvanzatoImpianti = false;
  messaggioFiltroAvanzatoImpianti = "";
  labelFiltroAvanzatoImpianti = "";

  constructor(
    private manager: TreeGisFiltersService,
    public treeGisService: TreeGisService,
    public anagrafica: AnagraficaService,
    public treeContainer: TreeContainerService,
    private gisService: GisService,
    private sharedDataService: SharedDataService,
    private permessiUtenteService: PermessiUtenteService,
    private translocoService: TranslocoService,
    private gestioneRichiesteService: GestioneRichiesteService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private giasIFrameWindowService: GiasIFrameWindowService,
    private gisClient: GisClient,
    private giasDialogService: GiasDialogService,
    private advancedTimeFilterService: AdvancedTimeFilterService,
  ) {

    this.labelFiltroAvanzatoImpianti = this.translocoService.translate('FiltroAvanzatoImpianti');

    this.centriAziendaliDDL = manager.pipe(map((items) => {
      return items;
    }));

    manager.pipe(takeUntil(this.signal), skip(1)).subscribe(items => {
      this.currentCentro = items[DEFAULT_ITEM_POSITION];
      this.filtersChanged.emit({ centro: items[DEFAULT_ITEM_POSITION] });
    })

    this.treeGisService.watchObjParametriAgenda(this.signal).pipe(skip(1)).subscribe(s => {
      consoleLogDebug(
        enum_logDebugArea.App,
        enum_logDebugTipo.CambioAzienda,
        this.constructor.name,
        'watchObjParametriAgenda'
      );
      this.manager.caricaInteroAlberoConFiltri();
    });

    this.anagrafica.filterData.pipe(skip(1), takeUntil(this.signal)).subscribe(s => {
      this.ricaricaAlbero();
    })

    manager.caricaInteroAlberoConFiltri();

    this.sharedDataService.getResetVisualizzazioneTotale$()
      .pipe(takeUntil(this.signal))
      .subscribe(flag => {
        if (this.visualizzazioneTotale) {
          this.visualizzazioneTotale = false;
          this.sharedDataService.setVisualizzazioneTotale(this.visualizzazioneTotale);
        }
      })

    this.sharedDataService.getCfgAlberoGisUtente$()
      .pipe(takeUntil(this.signal))
      .subscribe(cfg => {
        if (cfg[0].CfgGisUtente?.chkMostraCatasto || cfg[0].CfgGisUtente?.chkMostraCatastoAppezzamento) {
          this.visualizzaFiltroCatasto = true;
        } else {
          this.visualizzaFiltroCatasto = false;
        }
      })

    this.sharedDataService.getVisualizzazioneTotale$()
      .pipe(takeUntil(this.signal))
      .subscribe(visualizzazioneTotale => this.visualizzazioneTotale = visualizzazioneTotale);

    this.getPermessoVisualizzazioneTotale();

    this.giasIFrameWindowService.postMessageContextResult
      .pipe(takeUntil(this.signal))
      .subscribe(postMessageContextResult => {
        if (postMessageContextResult &&
          postMessageContextResult.contestoPostMessage !== undefined &&
          postMessageContextResult.contestoPostMessage === contestoPostMessage.FiltroneImpianti) {
          this.applicaFiltroAvanzatoImpianti(postMessageContextResult);
          if (postMessageContextResult.parametri.length > 0) {
            this.applicaFiltroAvanzatoTemporale(postMessageContextResult.parametri);
          }
        }
      })

  }

  private getPermessoVisualizzazioneTotale() {
    const permessoLettura = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Cartografia_VisualizzazioneTotale, 0);
    const permessoScrittura = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Cartografia_VisualizzazioneTotale, 2);
    this.permessoVisualizzazioneTotale = permessoLettura || permessoScrittura;
  }

  ngOnInit(): void {
    // this.treeContainerContext = this.treeContainer.treeContainerContext;
  }

  ngOnDestroy(): void {
    this.signal.next();
    this.signal.complete();
  }

  ricaricaAlberoConFeature() {
    this.ricaricaAlberoForzato();
    this.treeGisService.ricaricaFeatureDaAlbero(true);
  }

  ricaricaAlberoConFeatureSenzaCentrareMappa() {
    this.ricaricaAlberoForzato();
    this.treeGisService.ricaricaFeatureDaAlbero(false);
  }

  ricaricaAlberoForzato() {
    this.treeGisService.loadDataInternalFilters(true);
  }

  ricaricaAlbero() {
    this.treeContainer.selectedImpresaChangedSoUpdateTree = true;
    this.manager.caricaInteroAlberoConFiltri();
  }

  onFiltersChanged() {
    const filters = {
      centro: this.currentCentro
    };
    this.treeContainer.selectedImpresaChangedSoUpdateTree = true;
    this.filtersChanged.emit(filters);
    this.manager.centroSelezionato.next(filters);
  }

  onChangeVisualizzazioneTotale(checked: boolean) {
    this.applicaVisualizzazioneTotale(checked, true);
  }

  private applicaVisualizzazioneTotale(
    checked: boolean,
    autozoom: boolean
  ) {
    this.sharedDataService.setVisualizzazioneTotale(checked);
    if (checked) {
      // Feature tutte le aziende
      this.caricaFeatureVisualizzazioneTotale(autozoom);
      this.gisService.startOnZoomFeatureReload();
    } else {
      // Feature azienda corrente
      this.treeGisService.ricaricaFeatureAziendaCorrente();
      this.gisService.stopOnZoomFeatureReload();
    }
  }

  caricaFeatureVisualizzazioneTotale(autozoom: boolean) {
    if (autozoom === true) {
      this.gisService.impostaZoomMinimoVisualizzazioneTotale();
    }
    this.gisService.updateGeoJsonFilterServiceVisualizzazioneTotale();
  }

  toggleFiltroCatasto() {
    this.filtroCatasto = !this.filtroCatasto;
    if (this.filtroCatasto) {
      this.iconaFrecciaFiltroCatasto = faAngleUp;
    } else {
      this.iconaFrecciaFiltroCatasto = faAngleDown;
    }
  }

  // Filtro impanti avanzato

  apriFiltroAvanzatoImpianti() {
    this.openFiltrone();
  }

  private openFiltrone(): void {

    const newObjParametriAgenda: ObjParametriAgenda = JSON.parse(JSON.stringify(this.objParametriAgendaService.getObjParamValue()));

    newObjParametriAgenda.Pagina_Provenienza = enum_PagineGiasNG.Pagina_GIS;

    newObjParametriAgenda.Impianti = [];

    //si va direttamente sul Filtro Ricerca NG attività 8928
    let paramFiltroRicercaNG = new ParametriFiltroRicercaNG;
    paramFiltroRicercaNG.TipoMostraGestitiChiamante = [Enum_TipoMostra_FiltroRicerca.Impianti];
    paramFiltroRicercaNG.SitoDestinazioneDopoIlRedirect = Enum_SiteRedirector.GiasNG;
    paramFiltroRicercaNG.PaginaDestinazioneDopoIlRedirect = enum_PagineGiasNG.Pagina_GIS;
    paramFiltroRicercaNG.PaginaProvenienza = enum_PagineGiasNG.Pagina_GIS;
    paramFiltroRicercaNG.TipoComportamentoFiltroRicercaNG = Enum_TipoComportamento_FiltroRicerca.SelezionamentoEntita;
    paramFiltroRicercaNG.Piva = newObjParametriAgenda.Piva;

    newObjParametriAgenda.GenericObj_string = JSON.stringify(paramFiltroRicercaNG);
    this.objParametriAgendaService.changeObjParametriAgenda(newObjParametriAgenda);

    this.gestioneRichiesteService.gestionePassaggioStessoSito(enum_PagineGiasNG.Pagina_Filtro_Ricerca).then(resp => {
      this.giasIFrameWindowService.open({
        title: this.translocoService.translate('FiltroAvanzatoImpianti'),
        content: resp + "?seFrame=1",
        height: window.innerHeight * 0.9,
        width: window.innerWidth * 0.9
      });
      this.giasIFrameWindowService.window.window.onDestroy(() => {
        // Qui ci passa sia in chiudi finestra che in prosegui tramite bottone;
        // passa prima da qui che dall'intercettazione della chiusura finestra
        // dentro a GiasIFrameWindowService.
        // console.log('GiasIFrameWindowService.onDestroy');
      });
      this.giasIFrameWindowService.window.result.subscribe((messaggio: string) => {
        // Chiusura finestra con messaggio per capire se dipende da postMessage
        // console.log(messaggio);
      });
    });

  }

  private applicaFiltroAvanzatoImpianti(selezioneFiltrone: PostMessageSelezioneFiltrone) {

    if (selezioneFiltrone.tipoChiavi !== enum_TipoChiaveFiltrone.Impianto) {
      this.giasDialogService.baseError("", "ErroreFiltroAvanzatoImpiantiTipoChiavi");
      return;
    }

    let idTestata = 0;

    let elencoImpianti = this.popolaElencoImpianti(selezioneFiltrone);

    this.gisClient
      .gisAggiornaFiltroImpianti({
        IdTestata: idTestata,
        Impianti: elencoImpianti
      } as AggiornaFiltroImpianti_In)
      .subscribe({
        next: result => {
          this.filtroAvanzatoImpianti = true;
          this.componiMessaggioFiltroImpianti(elencoImpianti.length, result.RispostaStringa.TotaleElementiGraficiImpianto, result.RispostaStringa.ImpostaVisibilitaTotale)
          this.aggiornaFiltroImpiantiCfgAlbero(result.RispostaStringa.IdTestata, result.RispostaStringa.ImpostaVisibilitaTotale);
        },
        error: _ => {
          this.giasDialogService.baseError("", "ErroreFiltroAvanzatoImpianti");
        }
      });
  }

  private applicaFiltroAvanzatoTemporale(params: string[]) {
    const opStart = params[0][0] == "precedente"
      ? FiltroTemporale_enum_OperatoreFiltroTemporale.PrecedenteUguale
      : FiltroTemporale_enum_OperatoreFiltroTemporale.SuccessivoUguale;

    const opEnd = params[1][0] == "precedente"
      ? FiltroTemporale_enum_OperatoreFiltroTemporale.PrecedenteUguale
      : FiltroTemporale_enum_OperatoreFiltroTemporale.SuccessivoUguale;

    const startDate = new Date(moment(params[0][1], 'DD/MM/YYYY').format());
    const endDate = new Date(moment(params[1][1], 'DD/MM/YYYY').format());

    const periodFilter = {
      TipoFiltroTemporale: FiltroTemporale_enum_TipoFiltroTemporale.ValidiAllaData,
      DataInizio: new Date(startDate),
      DataFine: new Date(endDate),
      TipoOperatoreDataInizio: opStart,
      TipoOperatoreDataFine: opEnd
    } as FiltroTemporale;

    const startDateOnlyDate = new Date(startDate);
    startDateOnlyDate.setHours(0, 0, 0, 0);
    const singleDateFilter = {
      TipoFiltroTemporale: FiltroTemporale_enum_TipoFiltroTemporale.IntervalloTemporale,
      DataInizio: DateUtils.calcolaDataInizioSingolaData(startDateOnlyDate),
      DataFine: DateUtils.calcolaDataFineSingolaData(startDateOnlyDate),
      TipoOperatoreDataInizio: FiltroTemporale_enum_OperatoreFiltroTemporale.SuccessivoUguale,
      TipoOperatoreDataFine: FiltroTemporale_enum_OperatoreFiltroTemporale.PrecedenteUguale
    } as FiltroTemporale;
    const advancedFilter = {
      filtroTemporalePeriodo: periodFilter,
      filtroTemporaleSingolaData: singleDateFilter
    } as FiltroTemporaleAvanzato;

    this.advancedTimeFilterService.setAdvancedTimeFilter(advancedFilter);
  }

  private popolaElencoImpianti(selezioneFiltrone: PostMessageSelezioneFiltrone): ChiaveImpianto_In[] {
    let elencoImpianti = [];

    selezioneFiltrone.elencoChiavi.forEach(chiaveImpianto => {
      const separatore = "_";
      const piva = chiaveImpianto.split(separatore)[0];
      const saCod = chiaveImpianto.split(separatore)[1];
      const appezza = chiaveImpianto.split(separatore)[2];
      const idReg = chiaveImpianto.split(separatore)[3];
      const vegCod = chiaveImpianto.split(separatore)[4];
      const progettoCod = chiaveImpianto.split(separatore)[5];

      const objImpianto: ChiaveImpianto_In = {
        Piva: piva,
        Sa_Cod: parseInt(saCod),
        Appezza: parseInt(appezza),
        Id_Reg: parseInt(idReg),
        Progetto_Cod: parseInt(progettoCod),
        Veg_Cod: parseInt(vegCod)
      };

      elencoImpianti.push(objImpianto);

    });

    return elencoImpianti;
  }

  private componiMessaggioFiltroImpianti(
    impiantiFiltrati: number,
    impiantiTotali: number,
    impostaVisibilitaTotale: boolean
  ) {
    this.messaggioFiltroAvanzatoImpianti = this.translocoService.translate(
      "FiltroAvanzatoMessaggio",
      [impiantiFiltrati, impiantiTotali]
    );
    if (impostaVisibilitaTotale === true) {
      this.messaggioFiltroAvanzatoImpianti += ". " + this.translocoService.translate("FiltroAvanzatoPiuAziende");
    }
    this.messaggioFiltroAvanzatoImpianti += ".";
  }

  private aggiornaFiltroImpiantiCfgAlbero(
    idTestata: number,
    impostaVisibilitaTotale: boolean
  ) {
    const config = this.sharedDataService.getCfgAlberoGisUtente()[0];
    if (config?.CfgAlbero === undefined) {
      this.giasDialogService.baseError("", "ErroreNonPrevisto");
      return;
    }
    config.CfgAlbero.FiltroImpiantiIdTestataTemp = idTestata;
    this.sharedDataService.setCfgAlberoGisUtente(config, false);

    if (this.visualizzazioneTotale === false) {
      //--------------------------------------------------------------------------------
      // Visualizzazione totale non attiva
      //--------------------------------------------------------------------------------
      if (impostaVisibilitaTotale === true) {
        // Abilito visualizzazione totale
        this.visualizzazioneTotale = true;
        this.applicaVisualizzazioneTotale(this.visualizzazioneTotale, false);
      } else {
        // Ricarico albero/feature senza centrare mappa
        this.ricaricaAlberoConFeatureSenzaCentrareMappa();
      }
    } else {
      //--------------------------------------------------------------------------------
      // Visualizzazione totale attiva
      //--------------------------------------------------------------------------------
      // Ricarico feature visualizzazione totale
      this.caricaFeatureVisualizzazioneTotale(false);
    }

  }

  toggleFiltroAvanzatoImpianti() {
    if (this.filtroAvanzatoImpianti === true) {
      this.resetFiltroAvanzatoImpianti();
    } else {
      this.apriFiltroAvanzatoImpianti();
    }
  }

  private resetFiltroAvanzatoImpianti() {
    this.filtroAvanzatoImpianti = false;
    this.messaggioFiltroAvanzatoImpianti = "";
    this.giasIFrameWindowService.postMessageContextResult.next(null);
    this.aggiornaFiltroImpiantiCfgAlbero(0, false);
  }

}
