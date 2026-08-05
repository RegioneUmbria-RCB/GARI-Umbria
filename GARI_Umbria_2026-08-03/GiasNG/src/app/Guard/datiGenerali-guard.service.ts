import { Injectable, isDevMode } from "@angular/core";
import { ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from "@angular/router";
import {
  SACOD_NOFILTRO,
  SESSION_LINK_CORE_API,
  SESSION_LINK_NETCORE,
  SESSION_LINK_QDCA_COMPLIANCE,
  SESSION_LINK_GIAS_BASE,
  SESSION_LINK_NETCORE_DATA_EXCHANGE,
  SESSION_VARIABILISESSIONE,
  SESSION_TIMEZONEOFFSET,
  SESSION_OBJP_SUPER_SERVER,
  SESSION_OBJP_SERVER,
  SESSION_OBJP_UTENTI
} from "app/Model/CostantiPersonalizzate";
import { ImpostazioniAziendeCentriService } from "app/profilazione/services/impostazioni/impostazioni-aziende-centri.service";
import { ConfigurazioneSitiService } from "app/Service/configurazione-siti.service";
import { ConversionService } from "app/Service/conversion.service";
import { MasterService } from "app/Service/master.service";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { PermessiUtenteService } from "app/Service/permessi-utente.service";
import { environment } from "environments/environment";
import { isNumber } from "lodash";
import { SessionStorageService } from "ngx-webstorage";
import { from, lastValueFrom, Observable, take } from "rxjs";

@Injectable({providedIn: 'root'})
export class DatiGeneraliGuard {
  constructor(
    private masterService: MasterService,
    private sessionSt: SessionStorageService,
    private conversionService: ConversionService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private configurazioneSitiService: ConfigurazioneSitiService,
    private permessiUtenteService: PermessiUtenteService,
    private impostazioniaziendecentriservice: ImpostazioniAziendeCentriService
  ) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree | Observable<boolean | UrlTree> | Promise<boolean | UrlTree> {
    return new Promise<boolean>(async (resolve, reject) => {
      if (this.masterService.ObjParametri_Server == null || this.masterService.ObjParametri_Server == undefined || this.masterService.ObjParametri_Server == "") {
        const objPSuperServerSession = this.sessionSt.retrieve('ObjParametri_Super_Server');

        this.masterService.link_API = this.conversionService.ConversionDateInObject(JSON.parse(this.sessionSt.retrieve(SESSION_LINK_CORE_API)));
        this.masterService.link_NetCore6Api = this.conversionService.ConversionDateInObject(JSON.parse(this.sessionSt.retrieve(SESSION_LINK_NETCORE)));
        this.masterService.link_QdCA_Compliance = this.conversionService.ConversionDateInObject(JSON.parse(this.sessionSt.retrieve(SESSION_LINK_QDCA_COMPLIANCE)));
        this.masterService.link_NetCoreDataExchange = this.conversionService.ConversionDateInObject(JSON.parse(this.sessionSt.retrieve(SESSION_LINK_NETCORE_DATA_EXCHANGE)));
        this.masterService.link_GiasBase = this.conversionService.ConversionDateInObject(JSON.parse(this.sessionSt.retrieve(SESSION_LINK_GIAS_BASE)));
        this.masterService.objP_super_server = this.conversionService.ConversionDateInObject(JSON.parse(this.sessionSt.retrieve(SESSION_OBJP_SUPER_SERVER)));
        this.masterService.objP_server = this.conversionService.ConversionDateInObject(JSON.parse(this.sessionSt.retrieve(SESSION_OBJP_SERVER)));
        this.masterService.objP_utenti = this.conversionService.ConversionDateInObject(JSON.parse(this.sessionSt.retrieve(SESSION_OBJP_UTENTI)));
        this.masterService.variabiliInSessione = this.conversionService.ConversionDateInObject(JSON.parse(this.sessionSt.retrieve(SESSION_VARIABILISESSIONE)));
        this.masterService.serverTimeZoneOffset = this.sessionSt.retrieve(SESSION_TIMEZONEOFFSET);

        if (!this.permessiUtenteService.getUtente_PermessiLoaded()){
          const permessiUtente = await lastValueFrom(this.permessiUtenteService.fetchPermessiUtente());
          const confSiti = await lastValueFrom(this.configurazioneSitiService.leggi());
          this.permessiUtenteService.changeUtente_Permessi(permessiUtente);
          this.permessiUtenteService.changeUtente_PermessiLoaded(true);
        }

        let lingua = this.sessionSt.retrieve('lingua')
        if (isNumber(lingua)) {
          if (this.masterService.getCurrentLingua() != lingua) {
            this.masterService.changeLingua_Cod(lingua);
          }
        }
        this.sessionSt.store('lingua', this.masterService.getCurrentLingua());

        // this.sessionSt.store('ObjParametri_Super_Server', this.sessionSt.retrieve('ObjParametri_Super_Server'));
        // this.sessionSt.store('ObjParametri_Server', this.sessionSt.retrieve('ObjParametri_Server'));
        // this.sessionSt.store('ObjParametri_Utenti', this.sessionSt.retrieve('ObjParametri_Utenti'));
        //this.sessionSt.store('link_CoreWS', this.sessionSt.retrieve('link_CoreWS'));
        this.sessionSt.store(SESSION_LINK_CORE_API, this.sessionSt.retrieve(SESSION_LINK_CORE_API));
        this.sessionSt.store(SESSION_LINK_NETCORE, this.sessionSt.retrieve(SESSION_LINK_NETCORE));
        this.sessionSt.store(SESSION_LINK_QDCA_COMPLIANCE, this.sessionSt.retrieve(SESSION_LINK_QDCA_COMPLIANCE));
        this.sessionSt.store(SESSION_LINK_NETCORE_DATA_EXCHANGE, this.sessionSt.retrieve(SESSION_LINK_NETCORE_DATA_EXCHANGE));
        this.sessionSt.store(SESSION_LINK_GIAS_BASE, this.sessionSt.retrieve(SESSION_LINK_GIAS_BASE));
        this.sessionSt.store(SESSION_OBJP_SUPER_SERVER, this.sessionSt.retrieve(SESSION_OBJP_SUPER_SERVER));
        this.sessionSt.store(SESSION_OBJP_SERVER, this.sessionSt.retrieve(SESSION_OBJP_SERVER));
        this.sessionSt.store(SESSION_OBJP_UTENTI, this.sessionSt.retrieve(SESSION_OBJP_UTENTI));
        this.sessionSt.store(SESSION_VARIABILISESSIONE, this.sessionSt.retrieve(SESSION_VARIABILISESSIONE));
        this.sessionSt.store(SESSION_TIMEZONEOFFSET,this.sessionSt.retrieve(SESSION_TIMEZONEOFFSET));

        if (this.sessionSt.retrieve('objParametri_Agenda') !== null) {
          this.objParametriAgendaService.changeObjParametriAgenda(this.conversionService.ConversionDateInObject(JSON.parse(this.sessionSt.retrieve('objParametri_Agenda'))));
        }

        this.impostazioniaziendecentriservice.getImprese_Impostazioni(this.objParametriAgendaService.getObjParamValue().Piva, SACOD_NOFILTRO)
          .pipe(take(1))
          .subscribe(() => {
            this.masterService.changeInitialLoadCompleteSource(true);
            resolve(true);
          });
        return;
      } else {

        const confSiti = await lastValueFrom(this.configurazioneSitiService.leggi());
        // this.sessionSt.store('ObjParametri_Super_Server', JSON.stringify(this.masterService.ObjParametri_Super_Server));
        // this.sessionSt.store('ObjParametri_Server', JSON.stringify(this.masterService.ObjParametri_Server));
        // this.sessionSt.store('ObjParametri_Utenti', JSON.stringify(this.masterService.ObjParametri_Utenti));
        // this.sessionSt.store('link_CoreWS', JSON.stringify(this.masterService.link_CoreWS));
        this.sessionSt.store(SESSION_LINK_CORE_API, JSON.stringify(this.masterService.link_API));
        this.sessionSt.store(SESSION_LINK_NETCORE, JSON.stringify(this.masterService.link_NetCore6Api));
        this.sessionSt.store(SESSION_LINK_NETCORE_DATA_EXCHANGE, JSON.stringify(this.masterService.link_NetCoreDataExchange));
        this.sessionSt.store(SESSION_LINK_QDCA_COMPLIANCE, JSON.stringify(this.masterService.link_QdCA_Compliance));
        this.sessionSt.store(SESSION_LINK_GIAS_BASE, JSON.stringify(this.masterService.link_GiasBase));
        this.sessionSt.store(SESSION_VARIABILISESSIONE, JSON.stringify(this.masterService.variabiliInSessione));
        // this.sessionSt.store('objP_server', JSON.stringify(this.masterService.objP_server));
        // this.sessionSt.store('objP_super_server', JSON.stringify(this.masterService.objP_super_server));
        // this.sessionSt.store('objP_utenti', JSON.stringify(this.masterService.objP_utenti));
        this.sessionSt.store(SESSION_TIMEZONEOFFSET, this.masterService.serverTimeZoneOffset);

        let lingua = this.sessionSt.store('lingua', this.masterService.getCurrentLingua());
        if (isNumber(lingua)) {
          if (this.masterService.getCurrentLingua() != lingua) {
            this.masterService.changeLingua_Cod(lingua);
          }
        }
        this.sessionSt.store('lingua', this.masterService.getCurrentLingua());

        this.masterService.changeInitialLoadCompleteSource(true);
        resolve(true);
        return;
      }
    });
  }

}
