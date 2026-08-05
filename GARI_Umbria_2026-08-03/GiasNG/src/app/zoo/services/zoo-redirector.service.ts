import { ElementRef, Inject, Injectable } from "@angular/core";
import {
  enum_LAVCOD,
  enum_PagineAgronicaSincro,
  enum_PagineGiasNG,
  enum_TipoSincronizzazione_BDN,
  enum_TipoSincronizzazione_Treatments
} from "../../Model/TipiEnumerativi";
import { enum_PagineAgenda_2010, Enum_SiteRedirector } from "../../Model/siti.enum";
import { Router } from "@angular/router";
import { GestioneRichiesteService } from "../../Service/gestione-richieste.service";
import { ObjParametriAgendaService } from "../../Service/obj-parametri-agenda.service";
import { Enum_DBTypeOperation, LOADING_TOKEN, LoadingService, ObjParametriAgenda } from 'gias-ui-kit';
import { Tipo_Attivita} from 'gias-ui-kit';
import { ZooOperationGridFlatItem } from "../models/zoo-operation-grid-item.model";
import { isArray } from "lodash";
import { GiasDialogService } from "app/Service/gias-dialog.service";
import {filter, from, of, map, forkJoin} from "rxjs";
import {catchError, take} from "rxjs/operators";
import { OperazioniZooClient, TerapieClient } from "app/Service/net-core6-api.service";
import { LAVCOD_TRATTAMENTO_ANTIBUTTERATURA } from "app/Model/CostantiPersonalizzate";

export const OPERATIONS_MODEL_4 = [
  enum_LAVCOD.MACELLAZIONE_ANIMALI,
  enum_LAVCOD.VENDITA_ANIMALI,
  enum_LAVCOD.TRASFERIMENTO_ANIMALI
];
export const OPERAZIONI_ZOO_ALIMENTAZIONE = [
  enum_LAVCOD.ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_MANGIMI,
  enum_LAVCOD.ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_FORAGGI
];

export interface ZooActivityForRedirect {
  Id_Agenda: number;
  Lav_Cod: number;
  Data: Date;
  Piva: string;
}

@Injectable()
export class ZooRedirectorService {

  constructor(
    private router: Router,
    private dialog: GiasDialogService,
    private agenda: ObjParametriAgendaService,
    private gestioneRichieste: GestioneRichiesteService,
    private opZooClient: OperazioniZooClient,
    private therapiesClient: TerapieClient,
    @Inject(LOADING_TOKEN) private loadingService: LoadingService
  ) {
  }

  public redirectToModel4Generation(data: ZooOperationGridFlatItem) {
    if (data.Blocco_Flag === 1) {
      this.dialog.baseError("ImpossibileGenerareModello4", "Modello4OperazioneGiaGenerato");
      return;
    }
    if (!OPERATIONS_MODEL_4.includes(data.Lav_Cod)) {
      this.dialog.baseError("ImpossibileGenerareModello4", "Modello4OperazioneNonSincronizzabile");
      return;
    }
    from(this.gestioneRichieste.gestionePassaggioAltroSito(
      Enum_SiteRedirector.Sito_AgronicaSincronizzatore, enum_PagineAgronicaSincro.SincronizzatoreBDN,
      [
        { key: "tipoSincro", value: enum_TipoSincronizzazione_BDN.Genera_Modello4.toString(), codifica: false },
        { key: "chiave_arr", value: this.getAnimalTransDocParams(data), codifica: true }
      ]
    )).pipe(take(1), filter(r => r !== ""))
      .subscribe((url: string) => window.location.href = url);
  }

  public redirectToSincroBDN(key: string, pageSincro: enum_PagineAgronicaSincro, tipoSincro: enum_TipoSincronizzazione_BDN) {
    if (key == "") {
      this.dialog.baseError("ImpossibileGenerareModello4", "Modello4OperazioneGiaGenerato");
      return;
    }

    const agenda = this.agenda.getObjParamValue();
    agenda.Sito_Provenienza = Enum_SiteRedirector.GiasNG;
    agenda.Pagina_Provenienza = enum_PagineGiasNG.Pagina_Menu_Zoo;

    from(this.gestioneRichieste.gestionePassaggioAltroSito(
      Enum_SiteRedirector.Sito_AgronicaSincronizzatore, pageSincro,
      [
        { key: "tipoSincro", value: tipoSincro.toString(), codifica: false },
        { key: "chiave", value: key, codifica: true }
      ], agenda
    )).pipe(take(1), filter(r => r !== ""))
      .subscribe((url: string) => window.location.href = url);
  }

  public redirectToSincroTreatments(key: string, pageSincro: enum_PagineAgronicaSincro, tipoSincro: enum_TipoSincronizzazione_Treatments) {
    if (key == "") {
      this.dialog.baseError("ImpossibileGenerareModello4", "Modello4OperazioneGiaGenerato");
      return;
    }

    const agenda = this.agenda.getObjParamValue();
    agenda.Sito_Provenienza = Enum_SiteRedirector.GiasNG;
    agenda.Pagina_Provenienza = enum_PagineGiasNG.Pagina_Menu_Zoo;

    from(this.gestioneRichieste.gestionePassaggioAltroSito(
      Enum_SiteRedirector.Sito_AgronicaSincronizzatore, pageSincro,
      [
        { key: "tipoSincro", value: tipoSincro.toString(), codifica: false },
        { key: "chiave", value: key, codifica: true }
      ], agenda
    )).pipe(take(1), filter(r => r !== ""))
      .subscribe((url: string) => window.location.href = url);
  }

  public redirectToModel4Registration(data: ZooOperationGridFlatItem) {
    if (data.Tipo_Accettazione === 1) {
      this.dialog.baseError("ImpossibileRegistrareUscita", "Modello4OperazioneGiaRegistrato");
      return;
    }
    from(this.gestioneRichieste.gestionePassaggioAltroSito(
      Enum_SiteRedirector.Sito_AgronicaSincronizzatore, enum_PagineAgronicaSincro.SincronizzatoreBDN,
      [
        { key: "tipoSincro", value: enum_TipoSincronizzazione_BDN.Registra_Modello4.toString(), codifica: false },
        { key: "chiave_arr", value: this.getAnimalTransDocParams(data), codifica: true }
      ]
    )).pipe(take(1), filter(r => r !== ""))
      .subscribe((url: string) => window.location.href = url);
  }

  public redirectNewOperation(lavCod: number) {
    const agenda = this.agenda.getObjParamValue();
    agenda.Sito_Provenienza = Enum_SiteRedirector.GiasNG;
    agenda.Pagina_Provenienza = enum_PagineGiasNG.Pagina_Menu_Zoo;
    agenda.Data = new Date();
    agenda.Lav_Cod = lavCod;
    agenda.Id_Agenda = 0;
    agenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
    agenda.TipoOperazioneAgenda = Tipo_Attivita.QuadernoDiCampagna;
    agenda.Veg_Cod = -1;
    this.innerRedirect(lavCod, agenda);
  }

  public redirectOpenOperationInfo(activity: ZooActivityForRedirect, elemRef?: ElementRef) {
    if (activity.Lav_Cod == enum_LAVCOD.CURE_MEDICAMENTI_ANIMALI) {
      this.redirectActivityToTreatment(activity, false, elemRef);
    } else {
      let agenda = this.agendaFromActivity(activity, false);
      this.innerRedirect(agenda.Lav_Cod, agenda);
    }
  }

  public redirectOpenOperationEdit(activity: ZooActivityForRedirect, elemRef?: ElementRef) {
    if (activity.Lav_Cod == enum_LAVCOD.CURE_MEDICAMENTI_ANIMALI) {
      this.redirectActivityToTreatment(activity, true, elemRef);
    } else {
      let agenda = this.agendaFromActivity(activity, true);
      this.innerRedirect(agenda.Lav_Cod, agenda);
    }
  }

  public redirectToTherapy(Id_Terapia: number, opType: Enum_DBTypeOperation, elemRef?: ElementRef) {
    this.loadingService.set_isLoading({ isLoading: true, component: elemRef });
    forkJoin([
      from(this.gestioneRichieste.gestionePassaggioStessoSito(enum_PagineGiasNG.Pagina_Terapia_Zoo)),
      this.therapiesClient.terapieGetTerapia(Id_Terapia)
        .pipe(map(r => r.RispostaOK ? r.RispostaStringa as any : null))
    ]).subscribe(([redirectUrl, therapy]) => {
      this.loadingService.set_isLoading({ isLoading: false, component: elemRef });
      if (therapy) {
        const objP = this.agenda.getObjParamValue() as ObjParametriAgenda;
        objP.TipoOperazioneDB = opType;
        objP.GenericObj_string = therapy;
        this.agenda.navigateTo(redirectUrl, {}, objP, false);
      } else {
        this.dialog.baseError("_Errore", "QualcosaEAndatoStorto");
      }
    });
  }

  private getAnimalTransDocParams(activities: ZooOperationGridFlatItem | ZooOperationGridFlatItem[]): string {
    const keys = isArray(activities)
      ? activities.map(a => a.Piva + "_" + a.Id_Agenda)
      : [activities].map(a => a.Piva + "_" + a.Id_Agenda);
    return JSON.stringify(keys);
  }

  /** Gestisce il redirect all'altro sito.
   * 
   * @example
   * ```
   * let activity: ZooActivityForRedirect = ...;
   * this.innerRedirect(activity.Lav_Cod, this.agendaFromActivity(activity, true));
   * ```
   */
  private innerRedirect(lavCod: number, agenda: ObjParametriAgenda) {
    this.gestioneRichieste.gestionePassaggioAltroSito(
      Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
      this.getZooPageFromLavCod(lavCod),
      [], agenda
    ).then((val: string) => {
      if (val.indexOf("http") >= 0) {
        window.location.href = val;
      } else {
        this.router.navigate([val]);
      }
    });
  }

  private redirectActivityToTreatment(activity: ZooActivityForRedirect, isEditing: boolean, elemRef?: ElementRef) {
    this.loadingService.set_isLoading({ isLoading: true, component: elemRef });
    forkJoin([
      from(this.gestioneRichieste.gestionePassaggioStessoSito(enum_PagineGiasNG.Pagina_Trattamento_Zoo)),
      this.opZooClient.operazioniZooGetAttivitaTrattamentoZooFromAgenda(activity.Piva, activity.Id_Agenda)
        .pipe(map(r => r.RispostaOK ? r.RispostaStringa as any : null))
    ]).subscribe(([redirectUrl, attivita]) => {
      this.loadingService.set_isLoading({ isLoading: false, component: elemRef });
      if (attivita) {
        const objP = this.agenda.getObjParamValue() as ObjParametriAgenda;
        objP.TipoOperazioneDB = isEditing ? Enum_DBTypeOperation.Update : Enum_DBTypeOperation.Read;
        objP.GenericObj_string = attivita;
        this.agenda.navigateTo(redirectUrl, {}, objP, false);
      } else {
        this.dialog.baseError("_Errore", "QualcosaEAndatoStorto");
      }
    });
  }

  private agendaFromActivity(activity: ZooActivityForRedirect, isEditing: boolean): ObjParametriAgenda {
    const agenda = this.agenda.getObjParamValue();
    agenda.Data = activity.Data;
    agenda.Id_Agenda = activity.Id_Agenda;
    agenda.Programmazione_Cod = 0;
    agenda.Lav_Cod = activity.Lav_Cod;
    agenda.Veg_Cod = 0;
    agenda.TipoOperazioneDB = isEditing ? Enum_DBTypeOperation.Update : Enum_DBTypeOperation.Read;
    agenda.Sito_Provenienza = Enum_SiteRedirector.GiasNG;
    agenda.Pagina_Provenienza = enum_PagineGiasNG.Pagina_Menu_Zoo;
    return agenda;
  }

  /**
   * @param lavCod
   * @private
   * @UsageNotes unused LAVCODs:
   * - enum_LAVCOD.SOSTITUZIONE_MARCA:
   * - enum_LAVCOD.ANALISI_LATTE_SINGOLA:
   * - enum_LAVCOD.ANALISI_LATTE_MASSA:
   * - enum_LAVCOD.MUNGITURA_PREPARAZIONE:
   * - enum_LAVCOD.MUNGITURA_SECCHIO_POSTA:
   * - enum_LAVCOD.MUNGITURA_GRUPPI_POSTA:
   * - enum_LAVCOD.MUNGITURA_SALA_LATTE:
   * - enum_LAVCOD.MUNGITURA_LAVAGGIO_IMPIANTI:
   * - enum_LAVCOD.MUNGITURA_LAVAGGIO_SALA_LATTE:
   */
  private getZooPageFromLavCod(lavCod: enum_LAVCOD): enum_PagineAgenda_2010 {
    switch (lavCod) {
      case enum_LAVCOD.ALIMENTAZIONE_PULIZIA_IMPIANTI:
      case enum_LAVCOD.ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_FORAGGI:
      case enum_LAVCOD.ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_FORAGGI:
      case enum_LAVCOD.ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_MANGIMI:
      case enum_LAVCOD.ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_MANGIMI:
      case enum_LAVCOD.ALIMENTAZIONE_CONTROLLO_REGOLAZIONE_SISTEMI:
        return enum_PagineAgenda_2010.Pagina_ZooAlimentazione;

      case enum_LAVCOD.SPOSTAMENTI_ZOO:
        return enum_PagineAgenda_2010.Pagina_Zoo_Spostamento;

      case enum_LAVCOD.PESATURA_ANIMALI:
        return enum_PagineAgenda_2010.Pagina_Zoo_Pesatura;

      case enum_LAVCOD.ACQUISTO_ANIMALI:
      case enum_LAVCOD.NASCITA_ANIMALI:
      case enum_LAVCOD.INCREMENTO_CONSISTENZE_ZOO:
        return enum_PagineAgenda_2010.Pagina_Zoo_Carico;

      case enum_LAVCOD.VENDITA_ANIMALI:
      case enum_LAVCOD.DECREMENTO_CONSISTENZE_ZOO:
      case enum_LAVCOD.MORTE_ANIMALI:
      case enum_LAVCOD.MACELLAZIONE_ANIMALI:
      case enum_LAVCOD.TRASFERIMENTO_ANIMALI:
        return enum_PagineAgenda_2010.Pagina_Zoo_Scarico;

      case enum_LAVCOD.CURE_MEDICAMENTI_ANIMALI:
        return enum_PagineAgenda_2010.Pagina_Zoo_Trattamento;

      case enum_LAVCOD.ALTRE_LAVORAZIONI_ZOO:
      default:
        return enum_PagineAgenda_2010.Pagina_ZooAltreLavorazioni;
    }
  }

}
