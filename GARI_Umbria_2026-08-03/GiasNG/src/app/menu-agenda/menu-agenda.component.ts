import {DOCUMENT, ViewportScroller} from '@angular/common';
import {
  AfterViewInit,
  Component,
  HostListener,
  Inject,
  Input,
  OnDestroy,
  OnInit,
  ViewChild,
  ViewEncapsulation
} from '@angular/core';
import {
    faAnchorCircleXmark,
    faFileExcel,
    faIndustry,
    faObjectGroup,
    faObjectUngroup,
    faPaperclip,
    faPrint
} from '@fortawesome/free-solid-svg-icons';
import {TranslocoService} from '@jsverse/transloco';
import {SelectEvent, TabStripComponent} from '@progress/kendo-angular-layout';
import {MenuContestualeService} from 'app/Master/menu-contestuale/menu-contestuale.service';
import {enum_PagineAgenda_2010, Enum_SiteRedirector} from 'app/Model/siti.enum';
import {
  GestioneRichiesteService,
  KeyValuePair,
  ParametriAggiuntivi_QueryString
} from 'app/Service/gestione-richieste.service';
import {ObjParametriAgendaService} from 'app/Service/obj-parametri-agenda.service';
import {isNullOrUndefined, isUndefined, NumToStr} from 'app/Service/utils';
import {GiasIFrameWindowService} from 'gias-ui-kit';
import {filter, from, map, Subject, take, takeUntil} from 'rxjs';
import {FiltersService} from './components/filters/filters.service';
import {MenuAgendaDataStore} from './shared_services/menu-agenda-datastore.service';
import {GridCommandItem, RicettaRow, TabTypes} from './components/utils';
import {PermessiUtenteService} from 'app/Service/permessi-utente.service';
import {enum_CodificaStampe, enum_Security_Attivita} from 'app/Model/TipiEnumerativi';
import {enum_Impostazioni_Utenti} from 'app/Model/Impostazioni_Utenti.enum';
import {
  ImpostazioniAziendeCentriService
} from '../profilazione/services/impostazioni/impostazioni-aziende-centri.service';
import {ActivatedRoute} from "@angular/router";
import {AGRODATAFINE, AGRODATAINIZIO} from "../Model/CostantiPersonalizzate";
import {OpzioniAgenda} from './dtos/opzioni-agenda.model';

@Component({
    standalone: false,
  selector: 'app-menu-agenda',
  templateUrl: './menu-agenda.component.html',
  styleUrls: ['./menu-agenda.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class MenuAgendaComponent implements AfterViewInit, OnInit, OnDestroy {
  @Input() opzioniAgenda: OpzioniAgenda = new OpzioniAgenda({});
  @ViewChild('tabstrip') public tabstrip: TabStripComponent;

  public permessoQdC_R: boolean;
  public permessoQdC_W: boolean;
  public permessoOrdiniLavoroRicette_R: boolean;
  public permessoOrdiniLavoroRicette_W: boolean;
  public permessoBrogliaccio_R: boolean;
  public permessoBrogliaccio_W: boolean;

  public StampeBtns: GridCommandItem[] = [];
  public faIndustry = faIndustry;

  // Lista Stampe che non devono essere lanciate dal menù agenda
  public List_Stampe_Da_Escludere: enum_CodificaStampe[] = [enum_CodificaStampe.PianoColturale,
    enum_CodificaStampe.PianoColturaleCatasto, enum_CodificaStampe.PianoColturaleCatastoGrid];

  private signal: Subject<void> = new Subject();
  private DEBUG_FORCE_SHOW_DDT: boolean = false;

  constructor(
    @Inject(DOCUMENT) private document: Document,
    private agendaService: ObjParametriAgendaService,
    private menu: MenuContestualeService,
    private dataStore: MenuAgendaDataStore,
    private filters: FiltersService,
    private transloco: TranslocoService,
    private gestioneRichieste: GestioneRichiesteService,
    private windowService: GiasIFrameWindowService,
    private permessiUtenteService: PermessiUtenteService,
    private permessiImpreseService: ImpostazioniAziendeCentriService,
    private scroller: ViewportScroller,
    private route: ActivatedRoute
  ) {
    this.refreshGridOnCompanyChange();
    this.scroller.setOffset([0, 50]);
    this.filters.subscribeToFiltersChange()
      .pipe(takeUntil(this.signal), filter(data => !!data))
      .subscribe(data => this.scrollToGrid())
  }

  get showFilters(): boolean {
    if (this.opzioniAgenda.isFromGis) return false;
    else return (this.impresaSelezionata() && !this.opzioniAgenda.nascondiFiltri)
  }

  ngOnInit(): void {
    this.filters.nascondiFiltri = this.opzioniAgenda.nascondiFiltri;
    this.filters.CaricaParametriImpostazioniApp();
    this.dataStore.isMenuAgendaCalledFromGis = this.opzioniAgenda.isFromGis;

    this.menu.changeMenuContestualeSettings({
      show: true,
      background_color: "#92D050",
      color: 'white',
      title: this.transloco.translate('QdC'),
      search: true,
      bookmarks: true,
      contextualMenu: true,
      IDTipoSezione: 2,
      IDSezionePadre: 14
    });

    this.readPermessi();
    this.readImpostazioni();

    if (this.DEBUG_FORCE_SHOW_DDT) this.dataStore.setupShowDDT = true;

    this.route.queryParams.pipe(map(R => {
      if (R.d_t)
        this.dataStore.currentTab = +R.d_t;
      else
        this.dataStore.setPrevSelectedTab();
      if (R.mode) {
        this.filters.Mode = R.mode;
      }

      if (R.OperazioniImpianto == 1) {
        this.opzioniAgenda.nascondiFiltri = true;
        this.filters.nascondiFiltri = true;
      } else {
        this.opzioniAgenda.nascondiFiltri = false;
        this.filters.nascondiFiltri = false;
      }

    }), takeUntil(this.signal)).subscribe();
  }

  public impresaSelezionata() {
    let agenda = this.agendaService.getObjParamValue();
    return agenda.Piva != null && agenda.Piva !== '';
  }

  private readPermessi() {
    this.permessoQdC_R = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Agenda_AccessoMenu_NG, 0);
    this.permessoQdC_W = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Agenda_AccessoMenu_NG, 2);
    this.permessoBrogliaccio_R = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Brogliaccio, 0);
    this.permessoBrogliaccio_W = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Brogliaccio, 2);
    this.permessoOrdiniLavoroRicette_R = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Gest_Ricette, 0);
    this.permessoOrdiniLavoroRicette_W = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Gest_Ricette, 2);
  }

  private readImpostazioni() {
    from(this.dataStore.getStampePreferite()).pipe(
      takeUntil(this.signal),
      map(val => (!val || val.length === 0) ? [
        new GridCommandItem(this.transloco.translate("SchedaCampagnaMultiCentro"), enum_CodificaStampe.SchedaCampagnaMultiCentro),
        new GridCommandItem(this.transloco.translate("GlobalGAPMultiCentro"), enum_CodificaStampe.Eurep_Gap_Multicentro),
        new GridCommandItem(this.transloco.translate("RegistroDeiTrattamenti"), enum_CodificaStampe.RegistroTrattamenti_Semplificata),
        new GridCommandItem(this.transloco.translate("RegistroFertilizzazioni"), enum_CodificaStampe.Registro_Fertilizzazioni)
      ] : val),
      map(val => val.filter(v => !this.List_Stampe_Da_Escludere.includes(v.action)))
    ).subscribe(val => this.StampeBtns = val);

    this.dataStore.setupShowDDT = this.permessiImpreseService.getValore_Impostazione_Scalare_Centro_Azienda_Utente_SuperUser(
      this.agendaService.getObjParamValue().Piva, 0,
      enum_Impostazioni_Utenti.UTENTE_MOSTRA_DDT_MENU_AGENDA) !== '0';
  }

  ngAfterViewInit(): void {
    if (this.opzioniAgenda.isFromGis) {
      let fs = this.filters.filtersGetValue();
      fs.Da = AGRODATAINIZIO;
      fs.A = AGRODATAFINE;
      fs.Impianti = this.opzioniAgenda.impianti;
      this.filters.applyFilters(fs);
    }

    this.addWindowSizeListener();
    this.dataStore.selectedTabstrip
      .pipe(takeUntil(this.signal)).subscribe((selected: number) => {
      if (this.impresaSelezionata())
        this.tabstrip.selectTab(selected);
      let maxHeight = (document.body.clientHeight - 100);
      document.getElementById('tabstrip').setAttribute('style', 'max-height: ' + maxHeight + 'px')
    });
  }

  ngOnDestroy(): void {
    this.signal.next();
    this.dataStore.memorizeCurActiveTab(this.dataStore.selectedTabstrip.value);
  }

  @HostListener('window:beforeunload')
  beforeUnloadHandler(event) {
    this.dataStore.memorizeCurActiveTab(this.dataStore.selectedTabstrip.value);
  }

  onTabSelect(event: SelectEvent) {
    this.dataStore.selectedTabstrip.next(event.index);
  }

  onRedirectToStamps(event) {
    console.log(event);
    let richiesta: enum_CodificaStampe;
    let title: string;

    // Esempio parametri:
    // {lav_cod:"0", tipo_operazione:"  Stampa-13  ", specie:"-1", impianti: "" }
    let impiantiStr = '';
    if (this.filters.fg.value.Impianti.length > 0) {
      impiantiStr = this.filters.fg.value.Impianti
        .map(i => i.chiave)
        .reduce((k1, k2) => k1 + '|' + k2);
    }

    const parametri: ParametriAggiuntivi_QueryString[] = [
      KeyValuePair.Create("lav_cod", '0'),
      KeyValuePair.Create("specie", this.filters.fg.value.Specie.veg_cod),
      KeyValuePair.Create("impianti", impiantiStr),
    ];
    // this.getSelectedRows();

    parametri.push(KeyValuePair.Create("tipo_operazione", 'Stampa-' + event.action));
    richiesta = event.action;
    title = this.transloco.translate(event.actionName);

    if (!richiesta) return;

    from(this.gestioneRichieste.gestionePassaggioAltroSito(
      Enum_SiteRedirector.Sito_AgronicaStampe_2010,
      richiesta, parametri
    )).pipe(take(1))
      .subscribe(link => this.windowService.open({
        title: title,
        content: link,
        height: window.innerHeight * 0.9,
        width: window.innerWidth * 0.9
      }));
  }

  /* Pulsanti Documentale */
  public ApriKendoWindowRicercaDocumenti(dataItem: any) {
    let Id_Area = 11;
    let params = parseGridRowForDocumentale(dataItem, this.dataStore.currentTab);

    switch (parseInt(params.Lav_cod as string)) {
      case 2004: //Ordine Acquisto
      case 1025: //DDT Ricevuto
      case 1054:
      case 1076:
      case 1078: //Conferimento
      case 1031: //DDT Emesso
      case 2002: //Ordine Vendita
      case 1000: //Ordine Vendita
      case 1001: //Fattura emessa
        Id_Area = 10;
        break;
      case 1022: // carico di magazzino
      case 1023: // scarico di magazzino
        Id_Area = 13;
        break;
    }

    const parametri: ParametriAggiuntivi_QueryString[] = [
      KeyValuePair.Create("type", "doc"),
      KeyValuePair.Create("area_provenienza", NumToStr(Id_Area)),
      KeyValuePair.Create("p", params.Piva),
      KeyValuePair.Create("id_agenda", params.Id_Agenda),
      KeyValuePair.Create("Ricetta_Operazione_Cod", NumToStr(params.Ricetta_Operazione_Cod))
    ];

    this.gestioneRichieste.gestionePassaggioAltroSito(
      Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
      enum_PagineAgenda_2010.Pagina_Scadenzario_Lista,
      parametri).then(link => {
      this.windowService.open({
        title: this.transloco.translate("RicercaDocumenti"),
        content: link,
        height: window.innerHeight * 0.9,
        width: window.innerWidth * 0.9
      });
    });
  }

  public ApriKendoWindowAggiungiNuovoAllegato(dataItem: any) {
    let ID_Alert_Entita = -1;
    let ID_Elenco = -1;
    let Modalita = "doc";

    let Id_Area = 11;
    let Tipologia;

    let params = parseGridRowForDocumentale(dataItem, this.dataStore.currentTab);

    if (isNullOrUndefined(Tipologia) || Tipologia == "") {
      switch (parseInt(params.Lav_cod as string)) {
        case 2004: //Ordine Acquisto
          Tipologia = -18;
          Id_Area = 10;
          break;
        case 1025: //DDT Ricevuto
          Tipologia = -19;
          Id_Area = 10;
          break;
        case 1054:
        case 1076:
        case 1078: //Conferimento
          Tipologia = -20;
          Id_Area = 10;
          break;
        case 1031: //DDT Emesso
          Tipologia = -21;
          Id_Area = 10;
          break;
        case 2002: //Ordine Vendita
          Tipologia = -22;
          Id_Area = 10;
          break;
        case 1000: //Ordine Vendita
          Tipologia = -23;
          Id_Area = 10;
          break;
        case 1001: //Fattura emessa
          Tipologia = -24;
          Id_Area = 10;
          break;
        case 1022: // carico di magazzino
          Tipologia = -28;
          Id_Area = 13;
          break;
        case 1023: // scarico di magazzino
          Tipologia = -29;
          Id_Area = 13;
          break;
      }

      Tipologia = isUndefined(Tipologia) ? 0 : Tipologia;

      /** TODO(RV) da chiedere a Mouad quando Id_Area deve avere il valore 10 */
      // if (isNullOrUndefined(Tipologia) && Tipologia != "") {
      //     //Id_Area = 10;
      // }
    }

    var scadstr = JSON.stringify({
      'Piva': params.Piva, 'ID_Elenco': ID_Elenco, 'ID_Alert_Entita': ID_Alert_Entita,
      'Id_Area': Id_Area, 'Tipologia': Tipologia, 'area_provenienza': Id_Area,
      'Id_Agenda': params.Id_Agenda, 'Ricetta_Operazione_Cod': params.Ricetta_Operazione_Cod
    });

    const parametri: ParametriAggiuntivi_QueryString[] = [
      KeyValuePair.Create("scadstr", scadstr),
      KeyValuePair.Create("type", Modalita),
      KeyValuePair.Create("p", params.Piva)
    ];

    this.gestioneRichieste.gestionePassaggioAltroSito(
      Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
      enum_PagineAgenda_2010.Pagina_Scadenzario_CreaModifica,
      parametri).then(link => {
      this.windowService.open({
        title: this.transloco.translate("NuovoDocumento"),
        content: link,
        height: window.innerHeight * 0.9,
        width: window.innerWidth * 0.9
      });
    });
  }

  private scrollToGrid() {
    let e = document.querySelectorAll("kendo-grid-group-panel")[0];
    if (!e) return;
    setTimeout(function () {
      e.scrollIntoView({behavior: "smooth", block: "start",});
    }, 100);
  }

  private addWindowSizeListener() {
    const margin = 100;
    const handleResize = () => {
      let qualifier = (window.innerHeight - margin) + 'px';
      document.getElementsByClassName('k-content')[0].setAttribute('style', 'height: ' + qualifier);
      document.getElementsByClassName('k-tabstrip k-tabstrip-top k-tabstrip-scrollable')[0]
        .setAttribute('style', 'max-height: ' + qualifier);
    }
    window.addEventListener("resize", handleResize);
  }

  private refreshGridOnCompanyChange() {
    this.menu.currentMenuContestualeSettings
      .pipe(takeUntil(this.signal)).subscribe(() => {
      this.dataStore.resetGridData();
      this.filters.applyFilters()
    });
  }
}

function parseGridRowForDocumentale(row, currTab: TabTypes): ApriDocumentaleParams {
  let Ricetta_Operazione_Cod: number;
  let Id_Agenda: string;
  let Piva: string;
  let lav_cod: string | number;

  if (currTab === TabTypes.QuadernoDiCampagna) {
    Piva = row.Piva;
    Id_Agenda = row.id_agenda ?? "0";
    Ricetta_Operazione_Cod = 0;
    lav_cod = row.Lav_cod;
  } else if (currTab === TabTypes.Brogliaccio) {
    Piva = row.piva;
    Id_Agenda = "0";
    Ricetta_Operazione_Cod = row.Ricetta_Operazione_Cod ?? 0;
    lav_cod = row.lav_cod;
  } else { // ricetta per forza
    let ricetta = row as RicettaRow;
    Piva = ricetta.piva;
    Id_Agenda = "0";
    Ricetta_Operazione_Cod = ricetta.Ricetta_Operazione_Cod ?? 0;
    lav_cod = ricetta.lav_cod;
  }

  return {
    Lav_cod: lav_cod,
    Piva: Piva,
    Id_Agenda: Id_Agenda,
    Ricetta_Operazione_Cod: Ricetta_Operazione_Cod
  };
}

interface ApriDocumentaleParams {
  Lav_cod: string | number; // todo
  Piva: string;
  Ricetta_Operazione_Cod: number;
  Id_Agenda: string; // todo
}
