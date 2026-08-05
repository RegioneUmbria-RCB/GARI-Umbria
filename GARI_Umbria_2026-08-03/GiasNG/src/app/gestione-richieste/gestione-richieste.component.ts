import { Component, Inject } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { GestioneRichiesteService, warmUpGiasNG_Response } from '../Service/gestione-richieste.service';
import { PermessiUtenteService } from '../Service/permessi-utente.service';
import { MasterService } from '../Service/master.service';
import { ObjParametriAgendaService } from '../Service/obj-parametri-agenda.service';
import { IntlService } from '@progress/kendo-angular-intl';
import { SessionStorageService } from 'ngx-webstorage';
import { catchError, from, map, of, switchMap, take, tap } from 'rxjs';
import {
  AGRODATAFINE,
  AGRODATAINIZIO,
  timeZones,
  SESSION_LINK_CORE_API,
  SESSION_LINK_NETCORE,
  SESSION_LINK_QDCA_COMPLIANCE,
  SESSION_LINK_GIAS_BASE,
  SESSION_LINK_NETCORE_DATA_EXCHANGE,
  SESSION_VARIABILISESSIONE,
  SESSION_LINGUA,
  SESSION_TIMEZONEOFFSET,
  SESSION_OBJP_SUPER_SERVER,
  SESSION_OBJP_SERVER,
  SESSION_OBJP_UTENTI
} from 'app/Model/CostantiPersonalizzate';
import { NETCORE6_API_BASE_URL } from "../Service/net-core6-api.service";
import { QdCAC_API_BASE_URL } from "../Service/qdca-compliance-api.service";
import { ImpostazioniAziendeCentriService } from 'app/profilazione/services/impostazioni/impostazioni-aziende-centri.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { Configurazione_Siti } from 'app/Service/configurazione-siti.service';
import {ConfigurazioneSitiService} from "app/Service/configurazione-siti.service";

@Component({
  standalone: false,
  selector: 'gias-gestione-richieste',
  templateUrl: './gestione-richieste.component.html',
  styleUrls: ['./gestione-richieste.component.scss']
})
/** GestioneRichieste component*/
export class GestioneRichiesteComponent {
  /** GestioneRichieste ctor */
  objSuperServer: string;
  objServer: string;
  objUtenti: string;
  unid: string;
  linkcorews: string;
  linkcoreapi: string;
  linknetcore: string;
  linkQdcaCompliance: string;
  linkNetCoreDataExchange: string;
  linkGiasBase: string;
  objParametri_Agenda: ObjParametriAgenda;

  private pako;

  constructor(private router: Router,
    private route: ActivatedRoute,
    private masterService: MasterService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private gestioneRichiesteService: GestioneRichiesteService,
    private permessiUtenteService: PermessiUtenteService,
    private sessionSt: SessionStorageService,
    private configurazioneSitiService: ConfigurazioneSitiService,
    private intlService: IntlService,
    private impostazioniaziendecentriService: ImpostazioniAziendeCentriService,
    @Inject(NETCORE6_API_BASE_URL) private baseNetCoreUrl?: string,
    @Inject(QdCAC_API_BASE_URL) private baseQdCAComplianceUrl?: string,
  ) {
    this.pako = require('pako');
  }

  async ngOnInit() {
    //console.log("gestioneRichieste isback", this.gestioneRichiesteService.isBack);
    if (!this.gestioneRichiesteService.isBack) {
      this.route.queryParams.pipe(
        take(1),
        switchMap((params) => {
          try {
            this.masterService.set_isLoading({ isLoading: true, message: '' });
            this.objSuperServer = params.objss;
            this.objServer = params.objs;
            this.objUtenti = params.obju;

            this.unid = params.unid;
            //this.linkcorews = params.lcore;
            this.linkcoreapi = params.lapi;
            this.linkGiasBase = params.lgb;
            this.linknetcore = params.lnca;
            this.linkQdcaCompliance = params.lqdcc;
            this.linkNetCoreDataExchange = params.lncdex;

            this.masterService.serverTimeZoneOffset = timeZones[this.fromBinary((atob(params.tz)))];
            const linkCoreApi: string = this.formatDynamicLink(this.fromBinary(atob(this.linkcoreapi)));
            this.masterService.link_API = linkCoreApi;

            if (params.lnca == null || params.lnca == undefined || params.lnca == "") {
              this.masterService.link_NetCore6Api = this.baseNetCoreUrl;
            } else {
              this.masterService.link_NetCore6Api = this.formatDynamicLinkGiasBase(this.fromBinary(atob(this.linknetcore)));
            }

            this.masterService.link_GiasBase = this.formatDynamicLinkGiasBase(this.fromBinary(atob(this.linkGiasBase)));
            this.masterService.link_NetCore6Api = this.formatDynamicLinkGiasBase(this.fromBinary(atob(this.linknetcore)));
            this.masterService.link_QdCA_Compliance = this.formatDynamicLinkGiasBase(this.fromBinary(atob(this.linkQdcaCompliance)));
            if (this.linkNetCoreDataExchange) {
              this.masterService.link_NetCoreDataExchange = this.formatDynamicLinkGiasBase(this.fromBinary(atob(this.linkNetCoreDataExchange)));
            }

            from(this.gestioneRichiesteService.warmUpEFGiasNG()).pipe(
              take(1)
            ).subscribe((val) => {
              //console.log('warmUpEFGiasNG', val);
            })
          } catch (e) {
            console.error(e);
          }

          return from(this.gestioneRichiesteService.warmUpGiasNG(this.unid));
        }),
        switchMap((warmUpResult: warmUpGiasNG_Response) => {
          return from(this.configurazioneSitiService.leggi()).pipe(
            tap((configurazioneSiti: Configurazione_Siti[]) => {
              this.setSiti(configurazioneSiti);
            }),
            map(() => warmUpResult)
          );
        }),
        tap((r: warmUpGiasNG_Response) => {
          try {
            this.permessiUtenteService.changeUtente_Permessi(r.utente);
            this.impostazioniaziendecentriService.changeImprese_Impostazioni(r.impresa_impostazioni);
            this.masterService.variabiliInSessione = r.VariabiliInSessione;
            this.masterService.objP_super_server = r.objP_super_server;
            this.masterService.objP_server = r.objP_server;
            this.masterService.objP_utenti = r.objP_utenti;
            //this.masterService.link_GiasBase = window.location.protocol + '//' + window.location.hostname + '/' + 'GiasBase';

            try {
              r.objParametri_Agenda.Validita_Inizio = this.intlService.parseDate(<string><unknown>r.objParametri_Agenda.Validita_Inizio);
            } catch (e) {
              r.objParametri_Agenda.Validita_Inizio = AGRODATAINIZIO;
            }

            try {
              r.objParametri_Agenda.Validita_Fine = this.intlService.parseDate(<string><unknown>r.objParametri_Agenda.Validita_Fine);
            } catch (e) {
              r.objParametri_Agenda.Validita_Fine = AGRODATAFINE;
            }

            this.objParametri_Agenda = r.objParametri_Agenda;
            this.objParametriAgendaService.changeObjParametriAgenda(this.objParametri_Agenda);

            if (this.masterService.getCurrentLingua() != r.Lingua_Cod) {
              this.masterService.changeLingua_Cod(r.Lingua_Cod);
            }

            // this.sessionSt.store('ObjParametri_Super_Server', JSON.stringify(this.masterService.ObjParametri_Super_Server));
            // this.sessionSt.store('ObjParametri_Server', JSON.stringify(this.masterService.ObjParametri_Server));
            // this.sessionSt.store('ObjParametri_Utenti', JSON.stringify(this.masterService.ObjParametri_Utenti));
            // this.sessionSt.store('link_CoreWS', JSON.stringify(this.masterService.link_CoreWS));
            this.sessionSt.store(SESSION_LINK_CORE_API, JSON.stringify(this.masterService.link_API));
            this.sessionSt.store(SESSION_LINK_NETCORE, JSON.stringify(this.masterService.link_NetCore6Api));
            this.sessionSt.store(SESSION_LINK_QDCA_COMPLIANCE, JSON.stringify(this.masterService.link_QdCA_Compliance));
            this.sessionSt.store(SESSION_LINK_NETCORE_DATA_EXCHANGE, JSON.stringify(this.masterService.link_NetCoreDataExchange));
            this.sessionSt.store(SESSION_LINK_GIAS_BASE, JSON.stringify(this.masterService.link_GiasBase));


            this.sessionSt.store(SESSION_VARIABILISESSIONE, JSON.stringify(r.VariabiliInSessione));
            // this.sessionSt.store('link', JSON.stringify(r.link));
            this.sessionSt.store(SESSION_OBJP_SERVER, JSON.stringify(r.objP_server));
            this.sessionSt.store(SESSION_OBJP_SUPER_SERVER, JSON.stringify(r.objP_super_server));
            this.sessionSt.store(SESSION_OBJP_UTENTI, JSON.stringify(r.objP_utenti));
            this.sessionSt.store(SESSION_LINGUA, this.masterService.getCurrentLingua());
            this.sessionSt.store(SESSION_TIMEZONEOFFSET, this.masterService.serverTimeZoneOffset);
          } catch (e) {
            console.error(e);
          } finally {
            this.masterService.changeInitialLoadCompleteSource(true);
            this.masterService.set_isLoading({ isLoading: false, message: '' });
          }
          this.gestioneRedirect();
        }),
        catchError((err) => {
          console.error(err);
          this.masterService.changeInitialLoadCompleteSource(true);
          this.masterService.set_isLoading({ isLoading: false, message: '' });
          return of(null);
        })
      ).subscribe();
    } else {
      this.gestioneRichiesteService.isBack = false;
    }
  }
  fromBinary(binary) {
    const bytes = new Uint8Array(binary.length);
    for (let i = 0; i < bytes.length; i++) {
      bytes[i] = binary.charCodeAt(i);
    }
    const charCodes = new Uint16Array(bytes.buffer);
    let result = '';
    for (let i = 0; i < charCodes.length; i++) {
      result += String.fromCharCode(charCodes[i]);
    }
    return result;
  }

  setSiti(configurazioneSiti: Configurazione_Siti[]) {
    let sitoNetCoreDataExchange = this.getSito('GiasOnline_NetCoreDataExchange_API', configurazioneSiti);
    if (sitoNetCoreDataExchange && sitoNetCoreDataExchange != "") {
      this.masterService.link_NetCoreDataExchange = sitoNetCoreDataExchange;
    }
  }

  getSito(chiave: string, configurazioneSiti: Configurazione_Siti[]): string {
    let valore = configurazioneSiti.find((val) => { return val.Chiave == chiave })?.Valore;
    if (!valore) {
      return "";
    }
    if (valore.indexOf("http") == 0) {
      return valore;
    } else {
      let lanToWebSiteBasePath = configurazioneSiti.find((val) => { return val.Chiave == 'LanToWebSiteBasePath' })?.Valore;
      let url = lanToWebSiteBasePath + valore;
      return url;
    }
    return "";
  }

  gestioneRedirect() {
    this.gestioneRichiesteService.getPathFromPagina_Richiesta(this.objParametri_Agenda.Pagina_Richiesta).then((val) => {

      this.router.navigate([val], { queryParams: this.getParams(this.objParametri_Agenda.QueryStringFiltrino) });
    })
  }

  getParams(queryString: string): Params {
    queryString = queryString.replace("?", "");
    let arrParams = queryString.split("&");
    let params: Params = {};
    arrParams.forEach((strParam) => {
      let paramName = strParam.split("=")[0];
      let paramValue = strParam.split("=")[1];
      params[paramName] = paramValue;
    })

    return params;
  }

  formatDynamicLink(link: string): string {
    if (link[0] == '/') {
      //console.log(window.location);
      return window.location.origin + link;
      // return window.location.protocol + '//' + window.location.host + link
    } else {
      return link;
    }
  }

  formatDynamicLinkGiasBase(link: string): string {
    if (link[0] == '/') {
      //console.log(window.location);
      return window.location.protocol + '//' + window.location.hostname + link;
      // return window.location.protocol + '//' + window.location.host + link
    } else {
      return link;
    }
  }

}
