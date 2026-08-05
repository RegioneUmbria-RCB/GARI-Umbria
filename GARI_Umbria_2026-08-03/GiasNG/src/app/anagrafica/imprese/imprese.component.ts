import { Component, Inject, OnDestroy, OnInit, TemplateRef, ViewChild, ViewContainerRef, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { EditEvent } from '@progress/kendo-angular-grid';
import { GridPublicService } from 'gias-kendo-grid';
import { Subject, Subscription, takeUntil } from 'rxjs';
import { ObjParametriAgendaService } from '../../Service/obj-parametri-agenda.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { KendoGridColumn } from 'gias-kendo-grid';
import { ImpreseGridHttpService } from './imprese-grid.service';
import { ImpresaKendoServerResult } from './imprese.model';
import { CookieService } from 'ngx-cookie-service';
import { WindowRef } from '@progress/kendo-angular-dialog';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { GestioneRichiesteService } from 'app/Service/gestione-richieste.service';
import { enum_PagineAgenda_2010, Enum_SiteRedirector } from 'app/Model/siti.enum';
import { GiasIFrameWindowService } from 'gias-ui-kit';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { ImpreseFactoryService, IMPRESE_SERVICE_TOKEN } from 'app/Service/ServiceFactory/imprese.factory.service';
import { PivaValidatorService } from './piva-validator.service';
import { faPlus } from '@fortawesome/free-solid-svg-icons';
import { AnagraficaBusinessLogicService } from '../services/anagrafica-business-logic.service';
import { generateGridProvidersAnagrafica } from 'app/Utility/Template/kendo-grid/services/providers';
import { ObjParametriAgenda } from 'gias-ui-kit';
import {ExternalNavigationService} from '../../Service/external-navigation.service';
import { enum_Security_Attivita, SpecialNavigation } from 'app/Model/TipiEnumerativi';

@Component({
  standalone: false,
  selector: 'gias-anagrafica-imprese',
  templateUrl: './imprese.component.html',
  styleUrls: ['./imprese.component.scss'],
  providers: [
    ...generateGridProvidersAnagrafica(ImpreseGridHttpService, ImpreseComponent),
    PivaValidatorService,
    AnagraficaBusinessLogicService
  ],
  encapsulation: ViewEncapsulation.None
})
export class ImpreseComponent implements OnInit, OnDestroy {
  @ViewChild('iframeTemplate') private iframeTemplate: TemplateRef<any>;
  @ViewChild('saCodCellTemplate', { static: true }) saCodCellTemplate: TemplateRef<any>;

  objParametriAgenda: ObjParametriAgenda;
  editSubscription: Subscription;
  window: WindowRef;
  caricaTutte: boolean = false;

  name = 'Set iframe source';
  url = 'https://angular.io/api/router/RouterLink';
  urlSafe: SafeResourceUrl;
  public permessoEdit: boolean;

  public signal$: Subject<void> = new Subject();

  constructor(
    @Inject(IMPRESE_SERVICE_TOKEN) private impreseService: ImpreseFactoryService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private router: Router,
    private kendoGridService: GridPublicService,
    private cookieService: CookieService,
    private windowService: GiasIFrameWindowService,
    private sanitizer: DomSanitizer,
    private gestioneRichiesteService: GestioneRichiesteService,
    private vcRef: ViewContainerRef,
    private route: ActivatedRoute,
    private permessiUtenteService: PermessiUtenteService,
    private externalNavigation: ExternalNavigationService
  ) {  }

  async ngOnInit() {
    this.impreseService.setCaricaTutteImprese(false);
    this.impreseService.resetMaxData();
    this.objParametriAgendaService.currentObjParametriAgenda.pipe(takeUntil(this.signal$)).subscribe((val) => {
      this.objParametriAgenda = val;
    });
    this.permessoEdit = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_Impresa, 2);

    this.editSubscription = this.kendoGridService.changeDetected.GiasSubscribe((event: any) => {
      // if(event?.action === 'edit') {
      //     this.modificaAzienda(event);
      // }
      if(event?.action === 'remove') {
        this.eliminaAzienda(event);
      }
      if(event?.action === 'info') {
        this.infoAzienda(event);
      }
    });
  }

  ngOnDestroy(){
    this.editSubscription?.unsubscribe();
    this.signal$.next();
    this.signal$.complete();
  }

  infoAzienda(event: EditEvent) {
    const datiRiga = event.dataItem;

    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();

    this.objParametriAgenda.Piva = datiRiga.Piva;
    this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Read;

    this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);
    this.router.navigate(['Impresa-Edit'], { relativeTo: this.route });
  }

  modificaAzienda(event: EditEvent) {
    const datiRiga = event.dataItem;
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();

    this.objParametriAgenda.Piva = datiRiga.piva;
    this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Update;

    this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);

    this.router.navigate(['Impresa-Edit'], { relativeTo: this.route });
  }


  eliminaAzienda(event: EditEvent) {
    const datiRiga = event.dataItem;
    this.objParametriAgenda.Piva = datiRiga.piva;
    this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Delete;
  }

  ImpresaSelezionata(event: any) {
    if (event.selectedRows.length > 0) {
      this.objParametriAgenda.Piva = event.selectedRows[0].dataItem.piva;
      this.objParametriAgenda.RagSoc = event.selectedRows[0].dataItem.rag_soc;
      this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);
      //this.gestioneEsercizi();
    }
  }

  gestioneEsercizi() {
    let auto = this.cookieService.get('MenuBS_Anagrafica.gestioneEserciziWindow');
    if (auto == undefined || auto == '') {
      this.cookieService.set('MenuBS_Anagrafica.gestioneEserciziWindow', '1', 10000, '/');
      auto = '1';
    }

    if (auto == '1') {
      this.caricaGestioneEsercizi();
    }
  }

  caricaGestioneEsercizi() {

    this.gestioneRichiesteService.gestionePassaggioAltroSito(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, enum_PagineAgenda_2010.Pagina_Gestione_Esercizi).then(resp => {
      this.url = resp;
      this.window = this.windowService.open({
          title: this.objParametriAgenda.RagSoc + ' - ' + 'Chiusura/Apertura Esercizi',
          content: this.url,
          height: window.innerHeight * 0.9,
          width: window.innerWidth * 0.9
        }
      );
    });
  }

  onNuovo() {
    this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
    this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);
    this.router.navigate(['Impresa-Edit'], { relativeTo: this.route });
  }

  onCaricaTutte() {
    if(!this.caricaTutte && this.loadAllCompaniesAllowed()) {
      this.caricaTutte = true;
      this.impreseService.setCaricaTutteImprese(this.caricaTutte)
      this.impreseService.resetMaxData();
      this.kendoGridService.refresh(true);
    }
  }

  loadAllCompaniesAllowed(): boolean {
    return this.externalNavigation.specialNavigationCookie === SpecialNavigation.None;
  }
}
