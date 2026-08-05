import { Overlay, OverlayRef } from '@angular/cdk/overlay';
import { ComponentPortal } from '@angular/cdk/portal';
import { ComponentRef, ElementRef, Inject, Injectable } from '@angular/core';
import { TranslocoPipe, TranslocoService } from '@jsverse/transloco';
import { CoreWSRequest } from 'app/Model/AppConfig';
import { CoreWS_GenericObjP } from 'app/Model/CoreWS/CoreWS_GenericObjP';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { FooterModel } from '../Master/footer/footer.component';
import { HeaderModel } from '../Master/header/header.component';
import { CustomMessagesService } from './kendo-messages.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { MessageService } from '@progress/kendo-angular-l10n';
import { DOCUMENT } from '@angular/common';
import { GiasMessageService } from './gias-message.service';
import { AlberoMenu } from "./api.service";
import { cloneDeep } from "lodash";
import { IGiasMasterService, DynamicOverlay, GiasWaitFrameComponent } from 'gias-ui-kit';

export enum enum_ErroreGias_Tipo {
  Generico = 1,
  NonGestito = 999
}

export enum ErroreGias_Severity {
  Bloccante = 0, //Errore in basso a dx rosso, l'utente non può procedere (sia errori gestiti che exceptions)
  Warning = 1, //Pop-up di n warning accodati con possibilità per l'utente di proseguire ("Si desidera proseguire?"   -   Annulla/Prosegui)
  Info = 2,
  WarningBloccante = 3 //Pop-up di n errori accodati senza possibilità per l'utente di proseguire ("Non è possibile proseguire" - OK --> l'utente DEVE sistemare dei dati, NON può procedere altrimenti )
}

export class ErroreGias {
  severity: ErroreGias_Severity;
  messaggio: string;
  ex: string;
  tipo: enum_ErroreGias_Tipo;
}

export class RispostaStandard {
  Sessione: boolean;
  RispostaOK: boolean;
  RispostaConferma: boolean;
  ParametroDue: boolean;
  ParametroDue_stringa: string;
  Tipo: string;

  RispostaCompressa: Uint8Array;
  Compressa: boolean;

  Errore: string;
  RispostaStringa: string;

  ErroriGias: ErroreGias[];
}

export class rispostaStandard<T> {
  Sessione: boolean;
  RispostaOK: boolean;
  RispostaConferma: boolean;
  ParametroDue: boolean;
  ParametroDue_stringa: string;
  Tipo: string;
  RispostaCompressa: Uint8Array;
  Compressa: boolean;

  Errore: string;
  RispostaStringa: T;

  ErroriGias: ErroreGias[];

  constructor(res?: T) {
    this.RispostaStringa = res;
  }
}

export class LoadingObject {
  isLoading = false;
  message?: string = '';
  component?: ElementRef<any>;
  zIndex?: number = 1000;
}

export enum enum_InputType {
  Gias_Classic = 0,
  Angular_Material = 1,
  Due_righe = 2
}

export class ErrorMsg {
  show: boolean;
  msg: string;
  errorNumber: number;
}

export class WindowErrorMsg {
  show: boolean;
  msg: string;
  showWarningIcon?: boolean;
  TitleBarMessage?: string;
  errorNumber: number;
}

export class AgronicaCoreParametri_NG {
  PivaSuperUser: string;
  SuperUserUsername: string;
  UsernameOperazione: string;
  UtenteUsername: string;
  UtenteCodFiscale: string;
  FinestraTemporaleInizio: Date;
  FinestraTemporaleFine: Date;
  Lingua_Cod: number;
}

export class Imprese_Impostazioni {
  constructor(
    public Piva?: string,
    public Impostazione_Cod?: number,
    public Valore: string = '',
    public Sa_Cod: number = 0
  ) { }
}

export class AgronicaLink_NG {
  linkGiasBase: string;
  linkAgronicaCoreAPI: string
}

export class VariabiliInSessione_NG {
  cn_server: number;
}

// This timeout is used to delay the apprearence of the wait-frame component
const SHOW_LOADER_TIMEOUT: number = 400;

@Injectable()
export class MasterService implements IGiasMasterService {

  private headerModel: HeaderModel = null;
  private footerModel: FooterModel = null;
  private fullLoad = false;
  private showBackground = false;
  private InputType: enum_InputType = enum_InputType.Due_righe;
  private erroMsg: ErrorMsg = { show: false, msg: '', errorNumber: -1 };

  private isLoading = false;
  private message = '';

  private loadingObject: LoadingObject = { isLoading: false, message: '' };

  public ObjParametri_Server = null;
  public ObjParametri_Utenti = null;
  public ObjParametri_Super_Server = null;
  private _serverTimeZoneOffset: string = "";
  public _link_GiasBase = ''; //http://localhost/GiasBase/
  public _link_CoreWS = null;
  public _link_NetCore6Api = "https://localhost:5011";
  public _link_QdCA_Compliance = "https://localhost:5021";
  public _link_NetCoreDataExchange = "https://localhost:5031";
  public Lingua_Cod: number = 1;

  public headerLoadedSource = new Subject<void>();

  public gmapsApiLoaded: boolean = false;

  set link_CoreWS(value) {
    this._link_CoreWS = value;
  }

  get link_CoreWS() {
    return this._link_CoreWS;
  }

  get link_NetCore6Api() {
    return this._link_NetCore6Api
  }

  set link_NetCore6Api(value) {
    this._link_NetCore6Api = value;
  }

  get linkNetCoreDataExchange() {
    return this._link_NetCoreDataExchange;
  }

  get link_QdCA_Compliance() {
    return this._link_QdCA_Compliance;
  }

  set link_QdCA_Compliance(value) {
    this._link_QdCA_Compliance = value;
  }

  get link_NetCoreDataExchange() {
    return this._link_NetCoreDataExchange;
  }

  set link_NetCoreDataExchange(value) {
    this._link_NetCoreDataExchange = value;
  }

  set serverTimeZoneOffset(value) {
    if (value == null) {
      //console.log('qua');
    }
    this._serverTimeZoneOffset = value;
  }

  get serverTimeZoneOffset() {
    return this._serverTimeZoneOffset;
  }

  private _InfoAlberoMenu: AlberoMenu = null;

  set InfoAlberoMenu(alberoMenu: AlberoMenu) {
    this._InfoAlberoMenu = alberoMenu;
  }

  get InfoAlberoMenu() {
    return this._InfoAlberoMenu;
  }

  private _link_API = "https://localhost:5011";

  set link_API(value) {
    this._link_API = value;
  }

  get link_API() {
    return this._link_API;
  }

  set link_GiasBase(value) {
    this._link_GiasBase = value;
  }

  get link_GiasBase() {
    return this._link_GiasBase;
  }

  public objP_server: AgronicaCoreParametri_NG = null;
  public objP_super_server: AgronicaCoreParametri_NG = null;
  public objP_utenti: AgronicaCoreParametri_NG = null;

  private _variabiliInSessione: VariabiliInSessione_NG = null;
  get variabiliInSessione(): VariabiliInSessione_NG {
    return this._variabiliInSessione;
  }

  set variabiliInSessione(value: VariabiliInSessione_NG) {
    this._variabiliInSessione = value;
  }


  private headerModelSource = new BehaviorSubject(this.headerModel);
  currentHeader: Observable<HeaderModel> = this.headerModelSource.asObservable();

  private FullLoadSource = new BehaviorSubject(this.fullLoad);
  currentFullLoad: Observable<boolean> = this.FullLoadSource.asObservable();

  private isLoadingSource = new BehaviorSubject(this.loadingObject);
  currentIsLoading: Observable<LoadingObject> = this.isLoadingSource.asObservable();

  private InputTypeSource = new BehaviorSubject(this.InputType);
  currentInputType: Observable<enum_InputType> = this.InputTypeSource.asObservable();

  private ErrorMsgTypeSource = new BehaviorSubject(this.erroMsg);
  currentErrorMsgType: Observable<ErrorMsg> = this.ErrorMsgTypeSource.asObservable();

  private WindowErrorMsgSource = new BehaviorSubject(this.erroMsg);
  currentWindowErrorMsgSource: Observable<WindowErrorMsg> = this.WindowErrorMsgSource.asObservable();

  private initialLoadComplete = new BehaviorSubject<boolean>(false);
  initialLoadCompleteSource: Observable<boolean> = this.initialLoadComplete.asObservable();

  private footerModelSource = new BehaviorSubject(this.footerModel);
  currentFooter = this.footerModelSource.asObservable();

  private showBackgroundSource = new BehaviorSubject(this.showBackground);
  currentShowBackground = this.showBackgroundSource.asObservable();

  private lingua_CodSource = new BehaviorSubject(this.Lingua_Cod);
  currentLingua_Cod: Observable<number> = this.lingua_CodSource.asObservable();

  private isRefreshing = new BehaviorSubject<boolean>(false);
  isRefreshingSource: Observable<boolean> = this.isRefreshing.asObservable();

  private overlayRef: OverlayRef = null;
  private overlayComponentRef: OverlayRef = null;
  private isAlreadyLoading = false;

  private currentPage = new BehaviorSubject<number>(0);
  currentPageSource: Observable<number> = this.currentPage.asObservable();

  constructor(private overlay: Overlay,
    private translocopipe: TranslocoPipe,
    private translocoService: TranslocoService,
    private giasMessageService: GiasMessageService,
    @Inject(DOCUMENT) private document: Document,
    public kendoMessages: MessageService,
    private dynamicOverlay: DynamicOverlay) {

    this.isLoadingSource.subscribe(loadingParam => this.handleLoading(loadingParam));

    this.lingua_CodSource.subscribe((val) => {
      const lingua = MasterService.getLangCode(val);
      this.changeLanguage(lingua);
      this.changeKendoWidgetsLanguage(lingua);
    });

  }

  public static getLangCode(lingua: number): string {
    switch (lingua) {
      case 1:
        return 'it';
      case 2:
        return 'en';
      case 3:
        return 'fr';
      case 5:
        return 'pt';
      default:
        return 'it';
    }
  }


  /**
   * Stabilisce se un utente è superuser o meno.
   * @param usernameOperazione piva o CF dell'utente interessato, se non viene passato si fa riferimento all'utente che ha eseguito il login
   * @return True se l'utente interessato è il SuperUser, False altrimenti
   */
  public isSuperuser(usernameOperazione?: string): boolean {
    if (!usernameOperazione)
      usernameOperazione = this.objP_utenti.UsernameOperazione;
    return this.objP_utenti.PivaSuperUser === usernameOperazione;
  }

  /**
   * Checks if a username is the superuser username.
   * @param username username to check, if not provided the current user username will be used
   * @return True it is superuser username, False otherwise
   */
  public isSuperuserUsername(username?: string): boolean {
    if (!username)
      username = this.objP_utenti.UtenteUsername;
    return this.objP_server.SuperUserUsername === username;
  }

  public getCurrentUserUsername(): string {
    return this.objP_utenti.UtenteUsername;
  }

  changeLanguage(ln: string) {
    this.translocoService.setActiveLang(ln);
    this.changeKendoWidgetsLanguage(ln)
    this.document.documentElement.lang = ln;
  }

  changeKendoWidgetsLanguage(ln: string) {
    const svc = <CustomMessagesService>this.kendoMessages;

    svc.language = ln;
  }

  private handleLoading(loadingParam: LoadingObject) {
    if (loadingParam) {
      if (loadingParam.isLoading) {
        setTimeout(() => {
          this.handleShowLoading(loadingParam);
        }, SHOW_LOADER_TIMEOUT);
      } else {
        this.handleHideLoading(loadingParam)
      }
    }
  }


  private handleShowLoading(loadingParam: LoadingObject) {
    const isLoading = this.get_isLoading();

    if (isLoading.isLoading && !this.isAlreadyLoading) {
      this.isAlreadyLoading = true;
      const spinnerOverlayPortal = new ComponentPortal(GiasWaitFrameComponent);

      let component: ComponentRef<GiasWaitFrameComponent>;
      if (!loadingParam.component) {
        if (!this.overlayRef) {
          this.overlayRef = this.overlay.create();
        }
        component = this.overlayRef.attach(spinnerOverlayPortal);
        if (loadingParam.zIndex) {
          this.overlayRef.overlayElement.style.zIndex = loadingParam.zIndex.toString();
          this.overlayRef.overlayElement.parentNode.parentNode['style'].zIndex = loadingParam.zIndex.toString();
        }
      } else {
        this.overlayComponentRef = this.dynamicOverlay.createWithDefaultConfig(loadingParam.component.nativeElement);
        component = this.overlayComponentRef.attach(spinnerOverlayPortal);
      }

      if (isLoading.message && isLoading.message != '') {
        component.instance.message = isLoading.message;
      }
    }
  }


  private handleHideLoading(loadingParam: LoadingObject) {
    this.isAlreadyLoading = false;
    if (this.overlayRef != null) {
      this.overlayRef.detach();
      this.overlayRef.dispose();
      this.overlayRef = null;
    }

    if (this.overlayComponentRef != null) {
      this.overlayComponentRef.detach();
      this.overlayComponentRef.dispose();
      this.overlayComponentRef = null;
    }
  }

  changeHeader(upd_headerModel: HeaderModel) {
    this.headerModelSource.next(upd_headerModel);
  }

  getHeader(): HeaderModel {
    return this.headerModelSource.getValue();
  }

  changeFooter(upd_footerModel: FooterModel) {
    this.footerModelSource.next(upd_footerModel);
  }

  getFooter(): FooterModel {
    return this.footerModelSource.getValue();
  }

  changeShowBackground(upd_ShowBackground: boolean) {
    this.showBackgroundSource.next(upd_ShowBackground);
  }

  getShowBackground(): boolean {
    return this.showBackgroundSource.getValue();
  }

  set_FullLoad(fullLoadValue: boolean) {
    this.FullLoadSource.next(fullLoadValue);
  }

  set_isLoading(loadingObject: LoadingObject) {
    this.isLoadingSource.next(loadingObject);
  }

  get_isLoading(): LoadingObject {
    return this.isLoadingSource.getValue();
  }

  changeInputType(upd_InputType: enum_InputType) {
    this.InputTypeSource.next(upd_InputType);
  }

  getInputType(): enum_InputType {
    return this.InputTypeSource.getValue();
  }

  changeErrorMsgType(upd_errorMsg: ErrorMsg) {
    this.ErrorMsgTypeSource.next(upd_errorMsg);
  }

  changeWindowErrorMsg(upd_errorMsg: WindowErrorMsg) {

    if (upd_errorMsg.showWarningIcon === undefined ||
      upd_errorMsg.showWarningIcon === null) {
      upd_errorMsg.showWarningIcon = true;
    }

    if (upd_errorMsg.TitleBarMessage === undefined ||
      upd_errorMsg.TitleBarMessage === null) {
      upd_errorMsg.TitleBarMessage = this.translocopipe.transform('SieVerificatounProblema');
    }

    this.WindowErrorMsgSource.next(upd_errorMsg);
  }

  public InitialLoadCompleteValue(): boolean {
    return cloneDeep(this.initialLoadComplete.getValue());
  }

  changeInitialLoadCompleteSource(value: boolean) {
    this.initialLoadComplete.next(value);
  }

  changeIsRefreshing(value: boolean) {
    this.isRefreshing.next(value);
  }

  getIsRefreshing(): boolean {
    return this.isRefreshing.getValue();
  }

  getErrorMsgType(): WindowErrorMsg {
    return this.ErrorMsgTypeSource.getValue();
  }

  getWindowErrorMsg(): ErrorMsg {
    return this.WindowErrorMsgSource.getValue();
  }

  getParametri(): Parametri {
    return {
      objP_super_server: this.ObjParametri_Super_Server,
      objP_server: this.ObjParametri_Server,
      objP_utenti: this.ObjParametri_Utenti,

    };
  }

  getCoreWSGenericObjP(): CoreWS_GenericObjP {
    const coreWS_G = new CoreWS_GenericObjP();

    coreWS_G.objP_server = this.ObjParametri_Server;
    coreWS_G.objP_utenti = this.ObjParametri_Utenti;
    coreWS_G.objP_super_server = this.ObjParametri_Super_Server;

    return coreWS_G;
  }

  getCoreWSRequest(objParametriAgenda: ObjParametriAgenda): CoreWSRequest<ObjParametriAgenda> {
    const parametri: CoreWSRequest<ObjParametriAgenda> = {
      objP_super_server: this.ObjParametri_Super_Server,
      objP_server: this.ObjParametri_Server,
      objP_utenti: this.ObjParametri_Utenti,
      InData: objParametriAgenda
    };
    return parametri;
  }

  getCoreWSScrittura<T>(objParametriAgenda: ObjParametriAgenda, data: T): CoreWSRequest<T> {
    const parametri: CoreWSRequest<T> = {
      objP_super_server: this.ObjParametri_Super_Server,
      objP_server: this.ObjParametri_Server,
      objP_utenti: this.ObjParametri_Utenti,
      InData: data
    };
    return parametri;
  }

  setCompanyHeader(Rag_Soc: string) {
    const header = this.getHeader();
    header.company = Rag_Soc;
    this.changeHeader(header);
  }

  changeCurrentPage(upd_CurrentPage: number) {
    this.currentPage.next(upd_CurrentPage);
  }

  getCurrentPageAsValue(): number {
    return this.currentPage.getValue();
  }

  public getCurrentPageAsObs(): Observable<number> {
    return this.currentPage.asObservable();
  }

  changeLingua_Cod(lingua: number) {
    this.lingua_CodSource.next(lingua);
  }

  getCurrentLingua(): number {
    return this.lingua_CodSource.getValue();
  }

  handleErrori_Gestiti(errori: ErroreGias[]) {
    const bloccanti = errori.filter((e) => e.severity == ErroreGias_Severity.Bloccante);
    bloccanti.forEach((e) => {
      this.giasMessageService.errorMessage(e.messaggio);
      //this.giasDialogService.alertMessage(e.messaggio);
    });
    // const warning = errori.filter((e) => e.severity == ErroreGias_Severity.Bloccante);
    // const info = errori.filter((e) => e.severity == ErroreGias_Severity.Bloccante);
  }

}

export class Parametri {
  objP_super_server: string;
  objP_server: string;
  objP_utenti: string;
}
