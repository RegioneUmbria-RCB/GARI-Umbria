import { Component, DoCheck, OnDestroy, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { ProfilazioneDataShareService } from '../../services/profilazione-data-share.service';
import { generateGridProviders } from 'gias-kendo-grid';
import { GridUtentiModificaService } from './grid-utenti-modifica.service';
import { ImpresaDTO } from '../../services/impostazioni/impostazioni-aziende-centri.service';
import { VisibilitaService } from '../../services/visibilita.service';
import { forkJoin, lastValueFrom, map, Observable, of, Subject, switchMap, takeUntil, tap } from 'rxjs';
import { TranslocoService } from '@jsverse/transloco';
import { GridAziendeCentriComponent } from '../../impostazioni-utente/menu-impostazioni-aziende-centri/pagina-impostazioni-massive/grid-aziende-centri/grid-aziende-centri.component';
import { ObjParametriAgendaService } from '../../../Service/obj-parametri-agenda.service';
import { cloneDeep } from 'lodash';
import { Enum_TipoComportamento_FiltroRicerca, Enum_TipoMostra_FiltroRicerca, enum_CodificaStampe, enum_FiltroneParams, enum_PagineGiasNG, enum_TipoFiltrone } from '../../../Model/TipiEnumerativi';
import { enum_PagineAgenda_2010, Enum_SiteRedirector } from '../../../Model/siti.enum';
import { GestioneRichiesteService, KeyValuePair, ParametriAggiuntivi_QueryString } from '../../../Service/gestione-richieste.service';
import { GiasIFrameWindowService } from 'gias-ui-kit';
import { take } from 'rxjs/operators';
import { GiasDialogService } from '../../../Service/gias-dialog.service';
import { MasterService } from '../../../Service/master.service';
import { PermessiUtenteService } from '../../../Service/permessi-utente.service';
import { faCopy } from '@fortawesome/free-solid-svg-icons';
import { FormControl, FormGroup } from '@angular/forms';
import { ProfilazioneUtentiService } from '../../services/profilazione-utenti.service';
import { DatiBaseUtente } from '../../../Service/api.service';
import { IUtenteDTO } from '../../models/utente-dto.model';
import { ParametriFiltroRicercaNG } from 'app/filtro-ricerca/utils';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { TabPraticheComponent } from '../pratiche/tab-pratiche/tab-pratiche.component';
import { PraticheVisibilitaService } from '../../services/pratiche-visibilita.service';
import { ConfigurazioneSitiService, EnumChiaviConfigurazioneSiti } from '../../../Service/configurazione-siti.service';

@Component({
  standalone: false,
  selector: 'app-visibilita-utente',
  templateUrl: './visibilita-utente-edit.component.html',
  styleUrls: ['./visibilita-utente-edit.component.css'],
  providers: [
    ...generateGridProviders(GridUtentiModificaService, VisibilitaUtenteEditComponent)
  ]
})
export class VisibilitaUtenteEditComponent implements OnInit, DoCheck, OnDestroy {
  @ViewChild('gridImprese') gridImprese: GridAziendeCentriComponent;
  @ViewChild('copyVisibilityRef') copyVisibilityRef: TemplateRef<any>;
  @ViewChild('tabPraticheRef') tabPraticheRef: TabPraticheComponent;
  public hasVisibilitaTotale = false;
  public selezionatiHannoVisibilitaTotale = false;
  public showPratiche = false;
  public selezionatiNonHannoVisibilita = false;
  public selezionatiVisibilitaDiversa = false;
  public virtual: any = { itemHeight: 28 };
  public praticheDirty = false;
  public configSaving = false;
  private configBaseline: { operatoreOR: boolean; filtroPraticheAttivo: boolean } | null = null;

  protected readonly faCopy = faCopy;
  protected utenti: IUtenteDTO[] = [];
  protected readonly form = new FormGroup({
    utente: new FormControl<string>(null)
  });
  protected readonly praticheForm = new FormGroup({
    operatoreOR: new FormControl<boolean>(false),
    filtroPraticheAttivo: new FormControl<boolean>(false),
  });
  protected get praticheOperatoreOR(): boolean {
    return this.praticheForm.get('operatoreOR').value;
  }
  protected get praticheFiltroPraticheAttivo(): boolean {
    return this.praticheForm.get('filtroPraticheAttivo').value;
  }
  protected utentiDdl: DatiBaseUtente[] = [];
  private signal = new Subject();

  constructor(
    private visibilitaService: VisibilitaService,
    private dataShare: ProfilazioneDataShareService,
    private master: MasterService,
    private permessi: PermessiUtenteService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private gestioneRichiesteService: GestioneRichiesteService,
    private GiasIFrameWindowService: GiasIFrameWindowService,
    private dialog: GiasDialogService,
    private transloco: TranslocoService,
    private utentiService: ProfilazioneUtentiService,
    private praticheService: PraticheVisibilitaService,
    private configurazioneSitiService: ConfigurazioneSitiService,
  ) { }

  ngOnInit(): void {
    this.utenti = this.visibilitaService.utentiSelezionati;
    this.dataShare.utentiEditVisibilita = this.utenti;

    this.visibilitaService.impreseSelezionate = [];
    this.visibilitaService.gridRefresh$.next(true);

    this.visibilitaService.haVisibilitaTotale(this.permessi.getCurrentUser().Username)
      .pipe(take(1))
      .subscribe(hasVisTot => this.hasVisibilitaTotale = hasVisTot);

    this.caricaImprese();

    this.configurazioneSitiService.leggiChiave(EnumChiaviConfigurazioneSiti.Utenti_Visibilita_Calcolo_Combinato_Pratiche)
      .pipe(take(1), takeUntil(this.signal))
      .subscribe(config => {
        this.showPratiche = config?.Valore === '1';

        if (this.showPratiche && this.utenti.length === 1) {
          this.praticheService.caricaConfigurazioneUtente(this.utenti[0].UserName)
            .pipe(take(1))
            .subscribe((praticheConfig) => {
              const operatoreOR = praticheConfig.OperatoreFiltri?.toUpperCase() === 'OR';
              const filtroPraticheAttivo = praticheConfig.FiltroPraticheAttivo ?? false;
              this.praticheForm.patchValue({ operatoreOR, filtroPraticheAttivo }, { emitEvent: false });
              this.configBaseline = { operatoreOR, filtroPraticheAttivo };
            });

          this.praticheForm.valueChanges.pipe(takeUntil(this.signal)).subscribe(() => {
            if (!this.tabPraticheRef && this.configBaseline) {
              const current = this.praticheForm.value;
              const dirty =
                current.operatoreOR !== this.configBaseline.operatoreOR ||
                current.filtroPraticheAttivo !== this.configBaseline.filtroPraticheAttivo;
              this.praticheDirty = dirty;
            }
          });
        }
      });
  }

  ngDoCheck(): void {
    this.checkWindowToResize();

    const loadingScreen = document.querySelector('.cdk-overlay-container')
    if (loadingScreen) {
      loadingScreen.setAttribute('style', 'z-index: 10000 !important;');
    }
  }

  ngOnDestroy(): void {
    this.signal.next(true);
    this.signal.complete();
  }

  public savePratiche(): void {
    if (this.tabPraticheRef) {
      this.tabPraticheRef.save();
      return;
    }

    if (!this.utenti[0]?.UserName || this.configSaving) return;

    this.configSaving = true;
    this.praticheService.caricaConfigurazioneUtente(this.utenti[0].UserName)
      .pipe(
        take(1),
        switchMap((loadedConfig) =>
          this.praticheService.salvaConfigurazione({
            ...loadedConfig,
            OperatoreFiltri: this.praticheOperatoreOR ? 'OR' : 'AND',
            FiltroPraticheAttivo: this.praticheFiltroPraticheAttivo,
          })
        )
      )
      .subscribe({
        next: (success) => {
          if (success) {
            this.configBaseline = {
              operatoreOR: this.praticheOperatoreOR,
              filtroPraticheAttivo: this.praticheFiltroPraticheAttivo,
            };
            this.praticheDirty = false;
          }
          this.configSaving = false;
        },
        error: () => {
          this.configSaving = false;
        },
      });
  }

  public overwriteVisibility() {
    this.checkWindowToResize();
    let saved = false;
    this.dialog.warningThen('Attenzione', 'prof.WarningOverWriteVisibility', true,
      //() => lastValueFrom(this.visibilitaService.sovrascriviVisibilita(this.utenti)));
      () => this.apriFiltroneThen(event => {
        if (event.data?.inData && !saved) {
          const selectedPive = event.data.inData.chiavi;
          const imprese = selectedPive.map((piva: string) => new ImpresaDTO(piva));
          void lastValueFrom(this.visibilitaService.sovrascriviVisibilita(this.utenti, imprese));
          saved = true;
          void lastValueFrom(this.caricaImpreseSingoloUtente(this.utenti[0].UserName));
        }
      })
    );
  }

  public updateVisibility() {
    this.checkWindowToResize();
    let saved = false;
    this.dialog.warningThen('Attenzione', 'prof.WarningUpdateVisibility', true,
      // () => lastValueFrom(this.visibilitaService.aggiungiVisibilita(this.utenti)));
      () => this.apriFiltroneThen(event => {
        if (event.data?.inData && !saved) {
          const selectedPive = event.data.inData.chiavi;
          const imprese = selectedPive.map((piva: string) => new ImpresaDTO(piva));
          this.aggiungiImpreseSelezionate(imprese);
          void lastValueFrom(this.visibilitaService.aggiungiVisibilita(this.utenti));
          saved = true;
        }
      })
    );
  }

  public assegnaTotale() {
    this.checkWindowToResize();
    this.dialog.warningThen('Attenzione', 'prof.WarningTotalVisibility', true,
      () => lastValueFrom(this.visibilitaService.assegnaVisibilitaTotale(this.utenti)));
  }

  public openCopyVisibilityDialog() {
    if (!this.utentiDdl || !this.utentiDdl.length) {
      this.utentiService.readUtentiDatiBase(true).pipe(take(1))
        .subscribe((utenti: DatiBaseUtente[]) => this.utentiDdl = utenti);
    }
    let winRefs = document.getElementsByClassName('k-window ng-star-inserted');
    for (let i = 0; i < winRefs.length; i++) {
      winRefs[i]?.classList.remove('resized');
    }
    this.dialog.dialogMessageObs_Result(
      this.transloco.translate('prof.SelezioneUtente'),
      this.copyVisibilityRef
    ).pipe(take(1)).subscribe(res => {
      if (res['returnObj']) {
        void lastValueFrom(this.visibilitaService.copiaVisibilita(
          this.visibilitaService.utentiSelezionati.map(u => u.UserName),
          this.form.get('utente').value
        ));
      }
      this.form.get('utente').patchValue(null);
    });
  }

  public apriFiltroneThen(handler: (event: any) => void) {
    this.getFiltroneUrl().then(url => {

      url += '?seFrame=1';

      let fWin = this.GiasIFrameWindowService.open({
        title: this.transloco.translate('prof.SelezioneAziende'),
        content: url
      });
      fWin.window.location.nativeElement.setAttribute('style', 'top: 5% !important;');
      const messageListener = (event => handler(event)).bind(this);
      window.addEventListener('message', messageListener);
      fWin.result.pipe(take(1)).subscribe(() => this.master.changeShowBackground(true));
    });
  }

  private aggiungiImpreseSelezionate(imprese: ImpresaDTO[]) {
    this.visibilitaService.impreseSelezionate = this.visibilitaService.impreseSelezionate.concat(imprese);
    this.selezionatiNonHannoVisibilita = (this.visibilitaService.impreseSelezionate.length === 0);
    this.visibilitaService.gridRefresh$.next(true);
  }

  private getFiltroneUrl() {

    const newObjParametriAgenda: ObjParametriAgenda = cloneDeep(this.objParametriAgendaService.getObjParamValue());
    newObjParametriAgenda.Pagina_Provenienza = enum_PagineGiasNG.Pagina_Menu_Profilazione;
    newObjParametriAgenda.Piva = '';

    let paramFiltroRicercaNG = new ParametriFiltroRicercaNG;
    paramFiltroRicercaNG.TipoMostraGestitiChiamante = [Enum_TipoMostra_FiltroRicerca.Aziende];
    paramFiltroRicercaNG.SitoDestinazioneDopoIlRedirect = Enum_SiteRedirector.GiasNG;
    paramFiltroRicercaNG.PaginaDestinazioneDopoIlRedirect = enum_PagineGiasNG.Pagina_Menu_Profilazione;
    paramFiltroRicercaNG.PaginaProvenienza = enum_PagineGiasNG.Pagina_Menu_Profilazione;
    paramFiltroRicercaNG.TipoComportamentoFiltroRicercaNG = Enum_TipoComportamento_FiltroRicerca.SelezionamentoEntita;
    paramFiltroRicercaNG.Piva = '';

    newObjParametriAgenda.GenericObj_string = JSON.stringify(paramFiltroRicercaNG);
    this.objParametriAgendaService.changeObjParametriAgenda(newObjParametriAgenda);

    return this.gestioneRichiesteService.gestionePassaggioStessoSito(enum_PagineGiasNG.Pagina_Filtro_Ricerca);
  }

  private getParametriFiltrone(mostraHeader = false): Array<ParametriAggiuntivi_QueryString> {
    return [
      KeyValuePair.Create(enum_FiltroneParams.Sito_Origine, Enum_SiteRedirector.GiasNG.toString(), true),
      KeyValuePair.Create(enum_FiltroneParams.Pagina_Origine, enum_PagineAgenda_2010.Pagina_DuplicaOperazione.toString(), true),
      KeyValuePair.Create(enum_FiltroneParams.Pagina_Destinazione, enum_PagineGiasNG.Pagina_Menu_Profilazione.toString(), true),
      KeyValuePair.Create(enum_FiltroneParams.Sito_Destinazione, Enum_SiteRedirector.GiasNG.toString(), true),
      KeyValuePair.Create(enum_FiltroneParams.Tipo_Filtrone, enum_TipoFiltrone.Esportatore_Universale_Imprese.toString(), true),
      KeyValuePair.Create(enum_FiltroneParams.Forza_Redirect, '1', true),
      KeyValuePair.Create(enum_FiltroneParams.Codifica_Stampe, enum_CodificaStampe.Nessuna.toString(), true),
      KeyValuePair.Create(enum_FiltroneParams.Categoria, 'azienda', true),
      KeyValuePair.Create(enum_FiltroneParams.BloccaCategoria, '1', true),
      KeyValuePair.Create(enum_FiltroneParams.Filtrino, '1', true),
      //KeyValuePair.Create(enum_FiltroneParams.Dati_Di_Ritorno, '1', true),
      KeyValuePair.Create(enum_FiltroneParams.Mostra_Header_Footer, (mostraHeader ? '' : '1'), true)
    ];
  }

  private caricaImprese() {
    const usernames = this.utenti.map(u => u.UserName);
    this.master.set_isLoading({ isLoading: true });
    forkJoin([
      this.visibilitaService.hannoStessaVisibilita(usernames),
      this.visibilitaService.hannoVisibilitaTotale(usernames)
    ]).pipe(
      take(1),
      map(res => {
        console.log(res);
        this.selezionatiVisibilitaDiversa = !res[0];
        this.selezionatiHannoVisibilitaTotale = res[1].every(u => u.VisibilitaTotale);
        return !this.selezionatiVisibilitaDiversa && !this.selezionatiHannoVisibilitaTotale;
      }),
      switchMap(canLoad => canLoad ? this.caricaImpreseSingoloUtente(usernames[0]) : of([]))
    ).subscribe(res => this.master.set_isLoading({ isLoading: false }));
    // this.visibilitaService.hannoStessaVisibilita(usernames)
    //   .pipe(
    //     take(1),
    //     switchMap(isSame => {
    //       console.log('Users have same visibility?', isSame);
    //       this.selezionatiVisibilitaDiversa = !isSame;
    //       return isSame ? this.visibilitaService.hannoVisibilitaTotale(usernames) : of([]);
    //     }),
    //     map (totalVisibility => {
    //       if (!totalVisibility) {
    //         console.log('Some users have full visibility');
    //         this.selezionatiHannoVisibilitaTotale = false;
    //         return false;
    //       } else if ((totalVisibility as any[]).some(u => u.VisibilitaTotale)) {
    //         this.selezionatiHannoVisibilitaTotale = true;
    //         return false;
    //       }
    //       console.log('full visibility?', totalVisibility);
    //       return true;
    //     }),
    //     switchMap(canLoad => {
    //       return canLoad ? this.caricaImpreseSingoloUtente(usernames[0]) : of([]);
    //     })
    //   ).subscribe(canLoad => this.master.set_isLoading({isLoading:false}));
  }

  private caricaImpreseSingoloUtente(username: string): Observable<any[]> {
    return this.visibilitaService.haVisibilitaTotale(username).pipe(
      take(1),
      tap(visTot => this.selezionatiHannoVisibilitaTotale = visTot),
      switchMap(visTot => visTot ? of([]) : this.visibilitaService.leggiVisibilitaUtente(username, true)),
      tap(data => this.aggiungiImpreseSelezionate(data))
    );
  }

  // private caricaImpresePiuUtenti() {
  //   this.visibilitaService.hannoStessaVisibilita(this.utenti.map(u => u.UserName))
  //     .subscribe(stessaVisibilita => {
  //       if (stessaVisibilita)
  //         this.caricaImpreseSingoloUtente(this.utenti[0].UserName);
  //       else
  //         this.dialog.baseError('prof.VisibilitaNonConforme', 'prof.InfoErroreVisibilitaDiversa');
  //     });
  // }

  private checkWindowToResize() {
    let winRefs = document.getElementsByClassName('k-window ng-star-inserted');
    for (let i = 0; i < winRefs.length; i++) {
      winRefs[i]?.classList.remove('resized');
    }
  }

}
