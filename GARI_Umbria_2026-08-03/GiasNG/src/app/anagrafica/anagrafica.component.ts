import { AfterViewInit, Component, ElementRef, Injector, NgZone, OnDestroy, OnInit, ViewChild } from '@angular/core';

import { AGRODATAFINE, AGRODATAINIZIO, SMARTPHONE_WIDTH } from 'app/Model/CostantiPersonalizzate';

import { TreeContainerService } from 'app/Utility/Template/kendo-tree/services/tree-container.service';
import { ProvideAnagraficaTreeDeps } from 'app/Utility/Template/kendo-tree/utility/providers';
import { Subject } from 'rxjs';
import { MasterService } from '../Service/master.service';
import { PermessiUtenteService } from '../Service/permessi-utente.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { takeUntil, debounceTime } from 'rxjs/operators';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';
import { AnagraficaService } from './anagrafica.service';
import { faPrint, IconDefinition, faMap, faLeaf, faTruckPickup, faLayerGroup, faIndustry, faHome, faObjectGroup, faUserFriends } from '@fortawesome/free-solid-svg-icons';
import { GestioneRichiesteService } from 'app/Service/gestione-richieste.service';
import { enum_AnagraficaNgTabs, enum_CodificaStampe, enum_PagineGiasNG, enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { MenuContestualeService } from 'app/Master/menu-contestuale/menu-contestuale.service';
import { TranslocoPipe, TranslocoService } from '@jsverse/transloco';
import { BudgetService } from 'app/Service/Budget/budget.service';
import { isDate } from "moment";
import { SessionStorageService } from 'ngx-webstorage';
import { enum_TreeContext } from 'app/Utility/Template/kendo-tree/enum/tree-context';
import { HttpClient } from "@angular/common/http";
import { AjaxAgronicaAPIService } from "../Service/ajax-agronica.api.service";

@Component({
  standalone: false,
  selector: 'gias-anagrafica',
  templateUrl: './anagrafica.component.html',
  styleUrls: ['./anagrafica.component.scss'],
  providers: [...ProvideAnagraficaTreeDeps()]
})
/** Anagrafica component*/
export class AnagraficaComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('anchor', { static: false })
  public anchor: ElementRef<HTMLElement>;
  public AGRODATA_INIZIO = AGRODATAINIZIO;
  public signal$: Subject<void> = new Subject();

  @ViewChild('anagTopTar2', { static: false })
  public anagTopTar2: ElementRef<HTMLElement>;
  @ViewChild('topTabsContainer', { static: false })
  public topTabsContainer: ElementRef<HTMLElement>;
  @ViewChild('topTabsRightArrow', { static: false })
  public topTabsRightArrow: ElementRef<HTMLElement>;
  @ViewChild('topTabsLeftArrow', { static: false })
  public topTabsLeftArrow: ElementRef<HTMLElement>;

  public FinestraTemporaleInizio = AGRODATAINIZIO;
  public FinestraTemporaleFine = AGRODATAFINE;

  public windowHtml = window;
  SMARTPHONE_WIDTH = SMARTPHONE_WIDTH;

  public filtroForm: FormGroup = this.fb.group({
    data_filtro: [AGRODATAINIZIO],
    filter: [false]
  });

  private lastValueFormGroup: any;
  public margin = { horizontal: -46, vertical: 7 };
  public show = false;

  public Anagrafica_Impresa = enum_Security_Attivita.Anagrafica_Impresa;
  public Anagrafica_CentroAziendale = enum_Security_Attivita.Anagrafica_CentroAziendale;
  public Anagrafica_Fabbricato = enum_Security_Attivita.Anagrafica_Fabbricato;
  public Anagrafica_ParticellaCatastale = enum_Security_Attivita.Anagrafica_ParticellaCatastale;
  public Anagrafica_Campo = enum_Security_Attivita.Anagrafica_Campo;
  public Anagrafica_Impianto = enum_Security_Attivita.Anagrafica_Impianto;
  public Anagrafica_Contatto = enum_Security_Attivita.Anagrafica_Contatto;
  public Anagrafica_ParcoMacchine = enum_Security_Attivita.Anagrafica_ParcoMacchine;

  protected readonly enum_Security_Attivita = enum_Security_Attivita;
  protected readonly enum_AnagraficaNgTabs = enum_AnagraficaNgTabs;

  private _tabToShow: enum_PagineGiasNG | undefined = undefined;
  private _showTree: boolean = true;

  isBudget: boolean
  faMap = faMap;
  faLeaf = faLeaf;
  faTruckPickup = faTruckPickup;
  faLayerGroup = faLayerGroup;
  faIndustry = faIndustry;
  faHome = faHome;
  faObjectGroup = faObjectGroup;
  faUsers = faUserFriends;
  faPrint: IconDefinition = faPrint;

  //this.translocopipe.transform('Ragione_Sociale1')}

  readonly treeContextAnagrafiche = enum_TreeContext.Anagrafiche;

  constructor(
    private http: HttpClient,
    private ajaxAPI: AjaxAgronicaAPIService,
    private route: ActivatedRoute,
    private router: Router,
    private permessiUtenteService: PermessiUtenteService,
    private zone: NgZone,
    private fb: FormBuilder,
    public drawerService: TreeContainerService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private injector: Injector,
    private gestioneRichiesteService: GestioneRichiesteService,
    private anagraficaService: AnagraficaService,
    private menuContestualeService: MenuContestualeService,
    private transloco: TranslocoService,
    private translocopipe: TranslocoPipe,
    private masterService: MasterService,
    private giasMessageService: GiasMessageService,
    private budgetService: BudgetService,
    private sessionSt: SessionStorageService
  ) { }

  public get ddlStampeData(): Array<any> {
    const btns = [];
    if (this.permessiUtenteService.canWritePermesso(enum_Security_Attivita.Gest_Stampe)) {
        btns.push({
          text: this.transloco.translate('StampaQuadroP', {}),
          click: () => { this.StampaReport(enum_CodificaStampe.Quadro_P) },
        });
        btns.push({
          text: this.transloco.translate('RiepilogoSuperficiUtilizzate', {}),
          click: () => { this.StampaReport(enum_CodificaStampe.RiepilogoImpiegoSuperfici) },
        });
    }
    return btns;
  }

  public onToggle(): void {
    this.show = !this.show;
  }

  WithoutTime(dateTime) {
    const date = new Date(dateTime.getTime());
    date.setHours(0, 0, 0, 0);
    return date;
  }

  public getPermesso(attivita: number, operazione: number): boolean {
    return this.permessiUtenteService.getPermesso(attivita, operazione)
  }

  expandPanel() {
    this.drawerService.expander.next(!this.drawerService.expander.value);
  }

  ngOnDestroy(): void {
    let data: Date
    if (this.filtroForm.get("data_filtro").value == null || this.filtroForm.get("data_filtro").value.getTime() <= new Date('1970-01-01').getTime()) {
      data = AGRODATAINIZIO
    } else {
      data = new Date(this.sessionSt.retrieve('Anagrafica.FilterData').data)
    }
    this.filtroForm.get("data_filtro").patchValue(data)
    this.signal$.next();
    this.signal$.complete();
  }

  setOggi() {
    this.filtroForm.get("data_filtro").patchValue(new Date())
  }

  ngOnInit(): void {
    if (!this.budgetService.getBudget().activeBudget) {
      this.menuContestualeService.changeMenuContestualeSettings({
        show: true,
        background_color: "#2E8B57",
        color: 'white',
        title: this.transloco.translate('Anagrafica', {}),
        search: true,
        bookmarks: true,
        contextualMenu: true,
        IDTipoSezione: 2,
        IDSezionePadre: 8
      })
    }

    let data: Date
    if (this.sessionSt.retrieve('Anagrafica.FilterData').data == null || new Date(this.sessionSt.retrieve('Anagrafica.FilterData').data).getTime() <= new Date('1970-01-01').getTime()) {
      data = AGRODATAINIZIO
    } else {
      data = new Date(this.sessionSt.retrieve('Anagrafica.FilterData').data)
    }
    this.filtroForm.get("data_filtro").patchValue(data)

    //      const valore = this.filtroForm.getRawValue();
    //      this.anagraficaService.filterData.next({ data: valore.data_filtro, filter: true });
    //      this.anagraficaService.applicaFiltri()

    this.filtroForm.valueChanges.pipe(
      debounceTime(1000),
      takeUntil(this.signal$)
    ).subscribe((val) => {
      if (this.filtroForm.valid) {
        val = this.filtroForm.getRawValue();
        if (JSON.stringify(val.data_filtro) != JSON.stringify(this.lastValueFormGroup.data)) {
          this.lastValueFormGroup = val;
          if (isDate(val.data_filtro) && val.data_filtro > AGRODATAINIZIO) {
            this.anagraficaService.filterData.next({ data: val.data_filtro, filter: true });
          } else {
            this.anagraficaService.filterData.next({ data: val.data_filtro, filter: false });
          }
        }
      }
    });
    this.lastValueFormGroup = this.anagraficaService.filterData.getValue();
    this.filtroForm.patchValue(this.anagraficaService.filterData.getValue());

    this.filtroForm.controls['filter'].valueChanges.pipe(
      takeUntil(this.signal$)
    ).subscribe((val) => {
      if (!val) {
        this.filtroForm.controls['data_filtro'].disable({ emitEvent: false });
      } else {
        this.filtroForm.controls['data_filtro'].enable({ emitEvent: false });
      }
    });

    this.anagraficaService.filterData.pipe(
      takeUntil(this.signal$),
      debounceTime(500),
    ).subscribe((val) => {
      this.anagraficaService.applicaFiltri();
    });

    this.objParametriAgendaService.currentObjParametriAgenda
      .pipe(takeUntil(this.signal$))
      .subscribe((data) => {
        this.drawerService.selectedImpresaChangedSoUpdateTree = true;

        if (data.Piva === '' || !data.Piva) {
          this.drawerService.disabled = true;
        } else {
          this.drawerService.disabled = false;
        }
      });

    this.FinestraTemporaleInizio = this.masterService.objP_server.FinestraTemporaleInizio;
    this.FinestraTemporaleFine = this.masterService.objP_server.FinestraTemporaleFine;

    let cookie_page: string;
    // cookie_page = this.cookieService.get(this.permessiUtenteService.getUtente_Permessi().Username + '_Anagrafica');
    switch (cookie_page) {
      case 'Imprese':
        this.router.navigate(['Anagrafica/Imprese']);
        break;
      case 'Centri':
        this.router.navigate(['Anagrafica/Centri']);
        break;
      case 'Catasto':
        this.router.navigate(['Anagrafica/Catasto']);
        break;
      case 'Campi':
        this.router.navigate(['Anagrafica/Campi']);
        break;
      case 'Appezzamenti':
        this.router.navigate(['Anagrafica/Appezzamenti']);
        break;
      case 'Impianti':
        this.router.navigate(['Anagrafica/Impianti']);
        break;
      case 'Esercizi':
        this.router.navigate(['Anagrafica/Esercizi']);
        break;
      case 'Fabbricati':
        this.router.navigate(['Anagrafica/Fabbricati']);
        break;
      case 'Contatti':
        this.router.navigate(['Anagrafica/Contatti']);
        break;
      case 'Macchine':
        this.router.navigate(['Anagrafica/Macchine']);
        break;
    }

    this.isBudget = this.budgetService.getBudget().activeBudget
    this.handleQueryParams();
  }

  public ngAfterViewInit(): void {
    this.resizeTabs();
    this.zone.runOutsideAngular(() => {
      window.addEventListener('resize', () => {
        if (this.show) {
          this.zone.run(() => this.onToggle());
        }

        this.resizeTabs();
      });
    });
  }

  async StampaReport(id: number) {
    const agenda = this.objParametriAgendaService.getObjParamValue()
    if (agenda.Piva != '' && agenda.Piva != null && agenda.Piva != undefined) {
      const link = await this.gestioneRichiesteService.gestioneStampe(id);
      window.open(link);
    } else {
      this.giasMessageService.warningMessage(this.translocopipe.transform('SelezionareUnAzienda'));
    }
  }

  public moveToLeft() {
    // versione di codice con
    //const tabs = document.getElementById("anag-top-bar-2");
    this.anagTopTar2.nativeElement.scrollLeft -= 120;
  }

  public moveToRight() {
    // const tabs = document.getElementById("anag-top-bar-2");
    // tabs.scrollLeft += 100;
    this.anagTopTar2.nativeElement.scrollLeft += 120;
  }

  public mouseMoved(event: WheelEvent) {
    const direction = event.deltaY;
    if (direction <= 0) {
      this.moveToRight();
    } else {
      this.moveToLeft();
    }
  }

  public resizeTabs() {
    const c2Width = this.anagTopTar2.nativeElement.offsetWidth;
    const tabsWidth = this.topTabsContainer.nativeElement.clientWidth;
    if ((c2Width) <= tabsWidth) {
      // devo mostrare le frecce
      // console.log('devo mostrare le frecce');
      this.topTabsLeftArrow.nativeElement.style.display = 'block';
      this.topTabsRightArrow.nativeElement.style.display = 'block';
    } else {
      // nascondo le frecce
      // console.log('nascondo le frecce');
      this.topTabsLeftArrow.nativeElement.style.display = 'none';
      this.topTabsRightArrow.nativeElement.style.display = 'none';
    }
  }

  public chiamataAPI() {
    // let url = "https://hubmeteo.netagronica.it/MeteoAPI/Users/Authenticate";
    // let parametri = { username: "Demetra", password: "#d3m3tr4@m3t30#" }
    // this.http.post(url, )
    //this.ajaxAPI.ajaxPostNoCredential(url, parametri).subscribe((resp) => {console.log(resp)});
  }

  protected handleQueryParams(): void {
    this.route.queryParams.pipe(takeUntil(this.signal$))
      .subscribe(params => {
        if (params.tabToShow != undefined && params.tabToShow != '') {
          this._tabToShow = Number.parseInt(params.tabToShow);
        }
        /*Nel caso showTree non fosse stato passato in QueryStringFiltrino l'albero viene mostrato*/
        if (params.showTree != undefined && params.showTree != '') {
          this._showTree = (<string>params.showTree ?? 'true').toLowerCase().trim() === 'true';
        }
      });
  }

  protected showTab(tabToShow: enum_AnagraficaNgTabs): boolean {
    /*Nel caso tabToShow non fosse stato passato in QueryStringFiltrino la tab viene mostrata*/
    return tabToShow == (this._tabToShow ?? tabToShow);
  }

  protected showTree(): boolean {
    return this._showTree;
  }

}
