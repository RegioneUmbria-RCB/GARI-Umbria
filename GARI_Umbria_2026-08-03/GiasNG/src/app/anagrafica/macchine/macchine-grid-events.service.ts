import { ElementRef, Inject, Injectable } from '@angular/core';
import { KendoGridColumn, KendoGridRow } from 'gias-kendo-grid';
import { from, map, Observable, of, Subscription, switchMap } from 'rxjs';
import { KendoMacchineModel } from './macchine.model';
import { EditEvent } from '@progress/kendo-angular-grid';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ParcoMacchine } from 'app/Model/anagrafiche/ParcoMacchine';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { enum_TipoOperazioneDB } from 'app/Model/TipiEnumerativi';
import { rispostaStandard, RispostaStandard } from 'app/Service/master.service';
import { Macchine } from 'app/Model/metaschema/Macchine';
import { MacchineDettaglio1 } from 'app/Model/metaschema/MacchineDettaglio1';
import { MacchineDettaglio2 } from 'app/Model/metaschema/MacchineDettaglio2';
import { Marca } from 'app/Model/MetaschemaModel';
import { toInteger } from 'lodash';
import { IntervalloTemporale } from 'app/Model/anagrafiche/IntervalloTemporale';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { CentroAziendale } from 'app/Model/anagrafiche/CentroAziendale';
import { LoadingService, ObjParametriAgenda } from 'gias-ui-kit';
import { MacchineFactoryService, MACCHINE_SERVICE_TOKEN } from 'app/Service/ServiceFactory/macchine.factory.service';
import { TranslocoService } from '@jsverse/transloco';
import { Contatto } from 'app/Model/anagrafiche/Contatto';
import { GiasDialogService } from "../../Service/gias-dialog.service";
import { MacchineCodificaAgea } from '../../Model/metaschema/CodificheAgea';
import {AGRODATAINIZIO} from '../../Model/CostantiPersonalizzate';


@Injectable()
export class MacchineGridEventsService {

  KendoMacchine: any;
  macchineRows: any[];
  macchineColumns: KendoGridColumn[];
  macchineModel: KendoMacchineModel[];
  objParametriAgenda: ObjParametriAgenda;
  editSubscription: Subscription;
  tableDataSub: Subscription;
  ddlSub: Subscription;

  loadingService: LoadingService;
  component: ElementRef<any>;

  constructor(
    @Inject(MACCHINE_SERVICE_TOKEN) private macchineService: MacchineFactoryService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private giasMessageService: GiasMessageService,
    private giasDialogService: GiasDialogService,
    private router: Router,
    private route: ActivatedRoute,
    private translocoService: TranslocoService
  ) { }

  infoMacchinaAngular(event: EditEvent) {
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();

    this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Read;

    this.objParametriAgenda.Mac_Cod = parseInt((<string>(<any>event.dataItem).chiave).split("_")[2]);

    this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);
    //console.log(this.objParametriAgendaService.getObjParamValue());
    //this.router.navigate(['/Anagrafica/Macchine/Macchine-Edit']);
    this.router.navigate(['Macchine-Edit'], { relativeTo: this.route });
  }

  modificaMacchinaAngular(row: KendoGridRow, add_operation: boolean, rowIndex: number) {
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();

    if (add_operation) {
      this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
    } else {
      if (row['Piva'] != this.objParametriAgenda.Piva) {
        //this.giasMessageService.errorMessage(this.translocoService.translate("MacchinaAssociataAltraAzienda"));
        this.giasDialogService.baseError("", this.translocoService.translate('MacchinaAssociataAltraAzienda'), false);
        return;
      }
      this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Update;
      this.objParametriAgenda.Mac_Cod = toInteger(row['chiave'].split('_')[2]);
      this.objParametriAgenda.Sa_Cod = toInteger(row['chiave'].split('_')[1]);
      //this.objParametriAgenda.Piva = (<string>(<any>row).chiave).split("_")[0];
    }

    this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);

    //this.router.navigate(['/Anagrafica/Macchine/Macchine-Edit']);
    this.router.navigate(['Macchine-Edit'], { relativeTo: this.route });
  }

  eliminaMacchinaAngular(item: any): Observable<RispostaStandard> {
    const macchina: ParcoMacchine = this.prepareDeleteParcoMacchine(item);

    this.loadingService.set_isLoading({ isLoading: true, component: this.component });
    return from(this.macchineService.aggiornaMacchina(macchina, enum_TipoOperazioneDB.Cancellazione));
  }

  updateMacchina(item: any): Observable<any> {
    let objPAgenda = new ObjParametriAgenda;
    objPAgenda.Mac_Cod = item.Mac_Cod;
    objPAgenda.Piva = "";
    return this.macchineService.leggiMacchina(objPAgenda).pipe(
      map((r) => {
        if (r.RispostaOK) {
          return r.RispostaStringa
        }
      }),
      map((macchinaRead) => {
        this.loadingService.set_isLoading({ isLoading: true, component: this.component });
        return this.prepareParcoMacchine(item, macchinaRead);
      }),
      switchMap((macchina) => {
        return this.macchineService.aggiornaMacchina(macchina, enum_TipoOperazioneDB.Modifica)
      }),
      switchMap((resp) => {
        this.loadingService.set_isLoading({ isLoading: false, component: this.component });
        if (resp.RispostaOK) {
          this.giasMessageService.successMessage(this.translocoService.translate('MacchinaSalvataCorrettamente'));
        }
        return of([]);
      })
    )


    // this.loadingService.set_isLoading({ isLoading: true, component: this.component });
    // return from(this.macchineService.aggiornaMacchina(macchina, enum_TipoOperazioneDB.Modifica)).pipe(
    //   switchMap((resp) => {
    //     this.loadingService.set_isLoading({ isLoading: false, component: this.component });
    //     if (resp.RispostaOK) {
    //       this.giasMessageService.successMessage(this.translocoService.translate('MacchinaSalvataCorrettamente'));
    //     }
    //     return of([]);
    //   })
    // );
  }

  scriviMacchina(item: any): Observable<any> {
    const macchina: ParcoMacchine = this.prepareParcoMacchine(item);

    this.loadingService.set_isLoading({ isLoading: true, component: this.component });
    return from(this.macchineService.aggiornaMacchina(macchina, enum_TipoOperazioneDB.Scrittura)).pipe(
      switchMap((resp) => {
        this.loadingService.set_isLoading({ isLoading: false, component: this.component });
        if (resp.RispostaOK) {
          this.giasMessageService.successMessage(this.translocoService.translate('MacchinaSalvataCorrettamente'));
        } else {
          this.giasMessageService.errorMessage(this.translocoService.translate('ErroreCreazioneMacchina'));
        }
        return of([]);
      })
    );
  }

  prepareParcoMacchine(item: any, macchinaInput: ParcoMacchine = null): ParcoMacchine {
    let macchina = new ParcoMacchine();
    if (macchinaInput != null) {
      macchina = macchinaInput;
    }
    const objAgenda = this.objParametriAgendaService.getObjParamValue();
    if (item.Piva) {
      macchina.partitaIva = item.Piva;
    } else {
      macchina.partitaIva = objAgenda.Piva;
    }

    if (item.Mac_Cod) {
      macchina.codice = item.Mac_Cod;
    } else {
      macchina.codice = 0;
    }

    macchina.tipo = new Macchine();
    macchina.dettaglio_1 = new MacchineDettaglio1();
    macchina.dettaglio_2 = new MacchineDettaglio2();

    if (item.CLASS_CODE.length > 5) {
      macchina.tipo.codice = item.CLASS_CODE.split('.')[0];
      macchina.dettaglio_1.codice = item.CLASS_CODE.split('.')[1];
      macchina.dettaglio_2.codice = item.CLASS_CODE.split('.')[2];
      macchina.dettaglio_2.descrizione = item.tipologia;
    } else if (item.CLASS_CODE.length > 2) {
      macchina.tipo.codice = item.CLASS_CODE.split('.')[0];
      macchina.dettaglio_1.codice = item.CLASS_CODE.split('.')[1];
      macchina.dettaglio_1.descrizione = item.tipologia;
    } else {
      macchina.tipo.codice = item.CLASS_CODE;
      macchina.tipo.descrizione = item.tipologia;
    }

    macchina.ageaCod = new MacchineCodificaAgea(item.Agea_Cod);

    macchina.descrizione = item.Macchina;
    macchina.codice_stringa = item.Codice;
    macchina.contatto = new Contatto();
    macchina.contatto.primaryKey = { partitaIva: "", codice: item.Cod_Contatto };
    macchina.marca = new Marca();
    macchina.marca.descrizione = "";
    macchina.marca.codice = 0;
    if (item.Ditta_Cod != null && item.Ditta_Cod != undefined) {
      macchina.marca.descrizione = item.Ditta_Des;
      macchina.marca.codice = item.Ditta_Cod;
    }
    macchina.modello = item.Modello;
    macchina.targa = item.Targa;
    macchina.telaio = item.Telaio;

    macchina.data_Ultima_Manutenzione = item.Ultima_Manutenzione ?? AGRODATAINIZIO;
    macchina.data_Ultima_Revisione = item.Ultima_Revisione ?? AGRODATAINIZIO;

    if (item.Sa_Cod.id == -1) {
      macchina.visibilitaPubblica = true;
    } else {
      macchina.visibilitaPubblica = false;
    }

    macchina.centroPK = new CentroAziendale.PK(item.Sa_Cod.id, macchina.partitaIva);
    macchina.validita = new IntervalloTemporale(item.Validita_Inizio, item.Validita_Fine);
    macchina.caratteristiche = item.caratteristiche;

    return macchina;
  }

  prepareDeleteParcoMacchine(item): ParcoMacchine {
    const macchina = new ParcoMacchine();
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();

    macchina.partitaIva = item.Piva;
    macchina.codice = parseInt((<string>(<any>item).chiave).split("_")[2]);
    macchina.descrizione = item.Macchina;

    return macchina;
  }

  private isMacchinaMovimentata(macchina: ParcoMacchine): Observable<rispostaStandard<boolean>> {
    // const parametri: CoreWSRequest<ParcoMacchine> = {
    //   objP_super_server: this.masterService.ObjParametri_Super_Server,
    //   objP_server: this.masterService.ObjParametri_Server,
    //   objP_utenti: this.masterService.ObjParametri_Utenti,
    //   InData: agenda
    // };
    return this.macchineService.isMacchinaMovimentata(macchina);
  }
}
