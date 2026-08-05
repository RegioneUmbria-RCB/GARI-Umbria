import {Component, ElementRef, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {Router} from '@angular/router';
import {PermessiUtenteService} from '../Service/permessi-utente.service';
import {SelectEvent} from '@progress/kendo-angular-layout';
import {ProfilazioneDataShareService} from './services/profilazione-data-share.service';
import {map, Subject, takeUntil} from 'rxjs';
import {enum_Security_Attivita, enum_Security_Operazione} from "../Model/TipiEnumerativi";
import {enum_PagineProfilazione, PaginaProfilazioneItem} from "./models/PaginaProfilazione.model";
import {ImpostazioniFormService} from "./services/impostazioni/impostazioni-form.service";
import {GruppiUtentiService} from "./services/gruppi-utenti.service";
import { TipologieUtentiService } from './services/profili-permessi/tipologie-utenti.service';
import {MasterService} from "../Service/master.service";
import {UsersLoaderService} from "./gestione-utenti/utenti/griglia-menu-profilazione/users-loader.service";


@Component({
  standalone: false,
  selector: 'app-menu-profilazione',
  templateUrl: './menu-profilazione.component.html',
  styleUrls: ['./menu-profilazione.component.scss'],
  providers: [
    ProfilazioneDataShareService,
    ImpostazioniFormService,
    TipologieUtentiService,
    GruppiUtentiService,
    UsersLoaderService
  ]
})
export class MenuProfilazioneComponent implements OnInit, OnDestroy {
  @ViewChild('tabstrip') tabstrip;
  @ViewChild('anchor', {static: false})
  public anchor: ElementRef<HTMLElement>;
  public pagineProfilazione: Array<PaginaProfilazioneItem>;
  public show = false;
  protected canAccessPage = false;

  //-------------------------------------------------------------------------

  constructor(
    private router: Router,
    private datashare: ProfilazioneDataShareService,
    private permessiUtente: PermessiUtenteService,
    private master: MasterService
  ) {
    this.datashare.exitProfilazione = new Subject<boolean>();
    this.datashare.initPermissions(permessiUtente);
    this.setPagineProfilazione();
    this.applicaPermessiVisualizzazione();
    this.nascondiMostra_Installazione();

  }

  public get currentPage(): string {
    return this.datashare.currentPage.value;
  }

  ngOnInit() {
    this.computeCurrentPage();
    this.handleRedirect();
  }

  ngOnDestroy(): void {
    this.datashare.exitProfilazione.next(true);
    this.datashare.exitProfilazione.complete();
  }

  public isPageSelected(page: PaginaProfilazioneItem) {
    return this.datashare.currentPage.value === page.pagina;
  }

  public onTabSelect(e: SelectEvent) {
    this.datashare.currentPage.next(this.pagineProfilazione[e.index]?.pagina);
  }

  private computeCurrentPage() {
    let currentPage = this.pagineProfilazione.find(p => p.url === this.router.url.substring(1));
    this.datashare.currentPage.next(currentPage.pagina);
  }

  private nascondiMostra_Installazione() {
    this.permessiUtente.controllaEsistenzaTabellaCliente_Permessi().pipe(map(tabellaEsiste => {
      if(!tabellaEsiste)
        this.removeTab(enum_PagineProfilazione.CLIENTE_PERMESSI);
    })).subscribe();
  }

  private handleRedirect() {
    let prevPage = this.datashare.currentPage.value;
    this.datashare.currentPage.pipe(takeUntil(this.datashare.exitProfilazione))
      .subscribe(page => {
        if (page !== prevPage) {
          prevPage = page;
          let currentPage = this.pagineProfilazione.find(p => p.pagina === page);
          this.router.navigate([currentPage.url]);
        }
      });
  }

  /**
   * @private
   * @history
   * (07/05/2024): Il tab "Impostazioni Utente" è diventato "Impostazioni Superuser".
   * Se non si è superuser, per modificare le proprie (o altrui) impostazioni è necessario passare per l'apposito
   * pulsante nella griglia utenti.
   */
  private applicaPermessiVisualizzazione() {
    this.canAccessPage = this.permessiUtente.getPermesso(enum_Security_Attivita.Profilazione_NG, enum_Security_Operazione.Lettura);
    if (!this.datashare.userPermissions.canReadGroups)
      this.removeTab(enum_PagineProfilazione.GRUPPI);
    if (!this.datashare.userPermissions.canReadProfiles)
      this.removeTab(enum_PagineProfilazione.PROFILI_PERMESSI);
    if (this.cannotRead(enum_Security_Attivita.Gest_UtentiImpostazioni) || !this.master.isSuperuser())
      this.removeTab(enum_PagineProfilazione.UTENTI_IMPOSTAZIONI);
    if (this.cannotRead(enum_Security_Attivita.ImpostazioniImprese_NG))
      this.removeTab(enum_PagineProfilazione.IMPRESE_IMPOSTAZIONI);
    if (!this.datashare.userPermissions.canReadAllUsers)
      this.removeTab(enum_PagineProfilazione.VISIBILITA_UTENTI);
    if (!this.master.isSuperuser())
      this.removeTab(enum_PagineProfilazione.CLIENTE_PERMESSI);
  }

  private removeTab(page: enum_PagineProfilazione) {
    let i = this.pagineProfilazione.findIndex(tab => tab.pagina === page);
    if (i >= 0)
      this.pagineProfilazione.splice(i, 1);
  }

  private cannotRead(permission: enum_Security_Attivita) {
    return !this.permessiUtente.getPermesso(permission, enum_Security_Operazione.Lettura);
  }

  private setPagineProfilazione() {
    this.pagineProfilazione = [];
    this.pagineProfilazione.push(new PaginaProfilazioneItem('Utenti', enum_PagineProfilazione.UTENTI, 'Profilazione/Utenti'));
    this.pagineProfilazione.push(new PaginaProfilazioneItem('Gruppi', enum_PagineProfilazione.GRUPPI, 'Profilazione/Gruppi'));
    this.pagineProfilazione.push(new PaginaProfilazioneItem('prof.Profili', enum_PagineProfilazione.PROFILI_PERMESSI, 'Profilazione/Profili-Permessi'));
    this.pagineProfilazione.push(new PaginaProfilazioneItem('prof.ImpostazioniSU', enum_PagineProfilazione.UTENTI_IMPOSTAZIONI, 'Profilazione/Impostazioni-Utente'));
    this.pagineProfilazione.push(new PaginaProfilazioneItem('prof.ImpostazioniImprese', enum_PagineProfilazione.IMPRESE_IMPOSTAZIONI, 'Profilazione/Impostazioni-Aziende-Centri'));
    this.pagineProfilazione.push(new PaginaProfilazioneItem('RicercaVisibilita', enum_PagineProfilazione.VISIBILITA_UTENTI, 'Profilazione/Visibilita'));
    this.pagineProfilazione.push(new PaginaProfilazioneItem('prof.Installazione', enum_PagineProfilazione.CLIENTE_PERMESSI, 'Profilazione/Cliente-Permessi'));
  }

}
