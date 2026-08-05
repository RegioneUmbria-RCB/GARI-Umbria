import {AbstractGridConfigService, GridCommandItem, HttpAction} from 'gias-kendo-grid';
import {DestroyRef, inject, Inject, Injectable, Injector, Renderer2, RendererFactory2} from '@angular/core';
import { CELL_TYPES } from 'gias-ui-kit';
import {
  DropdownListItem,
  DropdownListWithForm,
  EditingMode,
  KendoGridColumn,
  KendoGridModel,
  KendoGridRow,
  KendoServerResult,
  LoaderType,
  ModelEntry,
  RendererGridEvent
} from 'gias-kendo-grid';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { ConfigTemplate } from 'gias-kendo-grid';
import {
  AggregateSettings,
  CommandsColumnSettings,
  CommandsDropDownEvents,
  CommandsDropDownSettings,
  CustomColumnSettings,
  RemoveMultipleRowsParams,
  ToolbarSettings
} from 'gias-kendo-grid';
import {
  auditTime,
  BehaviorSubject,
  combineLatest,
  filter,
  forkJoin,
  from,
  merge,
  Observable,
  of,
  scan,
  startWith,
  Subject,
  Subscription,
  take,
  withLatestFrom
} from 'rxjs';
import {catchError, debounceTime, map, skip, switchMap, takeUntil, tap} from 'rxjs/operators';
import {SpecieVegetaliService} from 'app/Service/Metaschema/specie-vegetali.service';
import {VarietaService} from 'app/Service/Metaschema/varieta.service';
import {DestinazioneUsoService} from 'app/Service/Metaschema/destinazioneUso.service';
import {GruppoFinalitaService} from 'app/Service/Metaschema/finalita.service';
import {CentriAziendaliService, LeggiCentriAziendali, LeggiIndirizziCentro} from 'app/Service/Anagrafica/centri.service';
import {Impresa} from 'app/Model/anagrafiche/Impresa';
import {EserciziEventsService} from './esercizi-events.service';
import {AGRODATAFINE, AGRODATAINIZIO, SMARTPHONE_WIDTH} from 'app/Model/CostantiPersonalizzate';
import {rispostaStandard} from 'app/Service/master.service';
import {KendoEserciziModel} from './eserciziGrid.model';
import {faCalendar, faClose, faLayerGroup, faPlusSquare,} from '@fortawesome/free-solid-svg-icons';
import {GruppoVarietaleService} from 'app/Service/Metaschema/gruppoVarietale.service';
import {FormaAllevamentoService} from 'app/Service/Metaschema/formaAllevamento.service';
import {PortinnestoService} from 'app/Service/Metaschema/portinnesto.service';
import {CoperturaService} from 'app/Service/Metaschema/copertura.service';
import {cloneDeep, isArray, isNumber, toInteger} from 'lodash';
import {CentroAziendale} from 'app/Model/anagrafiche/CentroAziendale';
import {PermessiUtenteService} from 'app/Service/permessi-utente.service';
import {enum_PagineGiasNG, enum_Security_Attivita, enum_UnitaMisura} from 'app/Model/TipiEnumerativi';
import {Appezzamento} from 'app/Model/anagrafiche/Appezzamento';
import {GiasMessageService} from 'app/Service/gias-message.service';
import {Impianto} from 'app/Model/anagrafiche/Impianto';
import {FormGroup, Validators} from '@angular/forms';
import { Varieta } from 'app/Model/metaschema/utilizzi/Varieta';
import { DestinazioneUso } from 'app/Model/metaschema/utilizzi/DestinazioneUso';
import { AppezzamentoEditService } from '../appezzamenti/appezzamenti-edit/appezzamento-edit.service';
import { Esercizio } from 'app/Model/anagrafiche/Esercizio';
import { BaseCodeDescr } from 'app/Model/baseClass/baseCodeDescr';
import { AnagraficaService, FilterData } from '../anagrafica.service';
import { ImgBase64Component } from './imgBase64/img-base64.component';
import { ImpiantoShortDescriptionComponent } from './ImpiantoShortDescription/impianto-short-description.component';
import { TranslocoService } from '@jsverse/transloco';
import { enum_Impostazioni_Utenti } from 'app/Model/Impostazioni_Utenti.enum';
import { ImpreseFactoryService, IMPRESE_SERVICE_TOKEN } from 'app/Service/ServiceFactory/imprese.factory.service';
import { CampiFactoryService, CAMPI_SERVICE_TOKEN } from 'app/Service/ServiceFactory/campi.factory.service';
import { ImpiantiFactoryService, IMPIANTI_SERVICE_TOKEN } from 'app/Service/ServiceFactory/impianti.factory.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { Regolamenti } from 'app/Model/metaschema/Regolamenti';
import { NuovaOperazioneComponent } from './nuova-operazione/nuova-operazione.component';
import { DialogRef, DialogService } from '@progress/kendo-angular-dialog';
import { NuovaOperazioneService } from './nuova-operazione/nuova-operazione.service';
import { BudgetService } from 'app/Service/Budget/budget.service';
import {GiasDialogService} from "../../Service/gias-dialog.service";
import { EserciziCopiaSpostaAppezzamentiService } from './esercizi-copiaSpostaAppezzamenti/esercizi-copia-sposta-appezzamenti.service';
import {VincoliService} from "../../Service/DPI/vincoli.service";
import {IntervalloTemporale} from "../../Model/anagrafiche/IntervalloTemporale";
import { Vincolo } from "../../Model/metaschema/Vincoli";
import {PianoConcimazioneService} from "../../Service/Metaschema/pianoConcimazione.service";
import {Specie} from "../../Model/metaschema/utilizzi/Specie";
import {GruppoFinalita} from "../../Model/metaschema/utilizzi/GruppoFinalita";
import {FinalitaPianoConcimazione} from "../../Model/metaschema/FinalitaPianoConcimazione";
import {FiltroCalcoloNPK} from "../../Model/filtri/filtroCalcoloNPK";
import {RegolamentoConcimazione} from "../../Model/metaschema/RegolamentoConcimazione";
import {FaseCicloColturale} from "../../Model/metaschema/fase";
import {Disciplinare} from "../../Model/metaschema/Disciplinari";
import { ContattiService } from 'app/Service/Anagrafica/contatti.service';
import { Contatto } from 'app/Model/anagrafiche/Contatto';
import { ImpresaPadre } from 'app/Model/anagrafiche/ImpresaPadre';
import { LeggiProdotti, ProdottiService } from 'app/Service/Anagrafica/prodotti.service';
import { Prodotto } from 'app/Model/attivita/risorse/Prodotto';
import {GruppiRaccoltaService} from '../../Service/GruppiRaccolta/gruppi-raccolta.service';
import {GruppoRaccolta} from 'app/Model/metaschema/GruppoRaccolta';
import {UnitaDiMisura_Alternativa} from 'app/Service/api.service';
import {ConversioniService} from 'app/Service/Metaschema/conversioni.service';
import {FiltroSpecieFinalita} from 'app/Model/filtri/filtroSpecieFinalita';
import {UtilizzoTerreno} from '../../Model/metaschema/utilizzi/UtilizzoTerreno';
import {ImpiantiService} from 'app/Service/Anagrafica/impianti.service';
import {IndirizzoAssociato} from '../../Model/anagrafiche/addresses/IndirizzoAssociato';
import {BaseCodeDescrStr} from '../../Model/baseClass/baseCodeDescrStr';
import {SeminaTrapiantoService} from '../../Service/Metaschema/seminaTrapianto.service';
import {SharedDataService} from '../../GIS/services/shared-data.service';
import { DatiPrevisionaliColtureRequest } from 'app/Model/anagrafiche/DatiPrevisionaliColture';
import {UtilityFunctions} from "../../Utility/UtilityFunctions";
import {SelectionEvent} from "@progress/kendo-angular-grid";
import {GiasKendoGridComponent} from 'gias-kendo-grid';
import {UnitaDiMisura} from '../../Model/metaschema/UnitaDiMisura';
import {UnitaDiMisuraService} from '../../Service/Metaschema/UnitaDiMisura.service';
import {CodificaInfoAggiuntiveService} from '../../Service/Codifiche/codifica_InfoAggiuntive.service';
import {NotificationRef} from '@progress/kendo-angular-notification';
import {OnCloseEsercizioComponent} from './on-close-esercizio/on-close-esercizio.component';
import {Utente_Impostazioni} from '../../Model/utente/utente_impostazioni';
import {enum_TipoPermesso} from '../../profilazione/models/profilazione.model';
import { ImpiantiAgendaNG, ObjParametriAgenda, Tipo_Attivita } from 'gias-ui-kit';
import {handleDdlConfig} from '../../Utility/Template/kendo-grid/grid-dropdown';
import { ConfigurazioneSitiService, EnumChiaviConfigurazioneSiti } from 'app/Service/configurazione-siti.service';
import { SementieriService } from 'app/Service/sementieri.service';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';

enum GridEserciziActions {
  ELENCO_OPERAZIONI = -1,
  NUOVA_OPERAZIONE = -2,
  CATASTO = -3,
  CHIUSURA = -4
}

export class EserciziServerResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}

@Injectable()
export class EserciziHttpService extends AbstractGridConfigService<EserciziServerResult> {
  gridId: string = 'EserciziHttpService';
  loader: LoaderType = LoaderType.SERVICE;

  editingMode: EditingMode = EditingMode.IN_LINE;
  rowId: string = 'chiave';
  cmdColumn: CommandsColumnSettings = new CommandsColumnSettings({editBtn: false, infoBtn: false, removeBtn: false});

  aggregates: AggregateSettings = new AggregateSettings({
    enabled: true,
    descriptors: [
      {field: 'sup_imp', aggregate: 'sum', format: 'n4'}
    ]
  });

  private eserciziServerResult: EserciziServerResult;

  private trasformatoSelected: boolean = false;
  private appezzamentiSelezionati: Map<Appezzamento, string> = new Map<Appezzamento, string>();

  private currentEditAppezza: BehaviorSubject<Appezzamento> = new BehaviorSubject<Appezzamento>(null);

  private eserciziModel: KendoEserciziModel = {
    chiave: new ModelEntry(CELL_TYPES.STRING, false),
    PIVA: new ModelEntry(CELL_TYPES.STRING, false),
    SA_COD: {type: CELL_TYPES.DROPDOWNLIST},
    sa_nome: new ModelEntry(CELL_TYPES.STRING, false),
    APPEZZA: new ModelEntry(CELL_TYPES.NUMBER, false),
    id_Reg: new ModelEntry(CELL_TYPES.NUMBER, false),
    app_nome: new ModelEntry(CELL_TYPES.STRING, false),
    veg_cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    veg_des: new ModelEntry(CELL_TYPES.STRING, false),
    cul_cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    cul_des: new ModelEntry(CELL_TYPES.STRING, false),
    GRFI_COD: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    Grfi_Des: new ModelEntry(CELL_TYPES.STRING, false),
    GRVA_Cod_VEG: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    grva_des: new ModelEntry(CELL_TYPES.STRING, false),
    dettSpeciePersonalizzatoCod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    dettSpeciePersonalizzatoDes: new ModelEntry(CELL_TYPES.STRING, false),
    sup_imp: new ModelEntry(CELL_TYPES.NUMBER, false),
    Sup_Int_Alt: new ModelEntry(CELL_TYPES.NUMBER, false),
    Udm_Cod_Alt: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    Udm_Des_Alt: new ModelEntry(CELL_TYPES.STRING, false),
    Validita_Inizio_Impianto: new ModelEntry(CELL_TYPES.DATE, true),
    Data_Inizio_Impianto: new ModelEntry(CELL_TYPES.DATE, true),
    Data_Inizio_Produzione: new ModelEntry(CELL_TYPES.DATE, true),
    Validita_Inizio: new ModelEntry(CELL_TYPES.DATE, true),
    Validita_Fine: new ModelEntry(CELL_TYPES.DATE, true),
    Data_Modifica: new ModelEntry(CELL_TYPES.DATETIME, true),
    utente_modifica: new ModelEntry(CELL_TYPES.STRING, false),
    Data_Creazione: new ModelEntry(CELL_TYPES.DATETIME, true),
    utente_creazione: new ModelEntry(CELL_TYPES.STRING, false),
    Campo_Cod: new ModelEntry(CELL_TYPES.NUMBER, false),
    Campo_Des: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    Codice_Impianto: new ModelEntry(CELL_TYPES.STRING, false),
    algoritmoCodifica: new ModelEntry(CELL_TYPES.STRING, false),
    tra_fila_m: new ModelEntry(CELL_TYPES.NUMBER, false),
    su_fila_m: new ModelEntry(CELL_TYPES.NUMBER, false),
    interbina: new ModelEntry(CELL_TYPES.NUMBER, false),
    germinabilita: new ModelEntry(CELL_TYPES.NUMBER, false),
    pianteHa: new ModelEntry(CELL_TYPES.NUMBER, false),
    pianteImpianto: new ModelEntry(CELL_TYPES.NUMBER, false),
    port_cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    port_des: new ModelEntry(CELL_TYPES.STRING, false),
    foral_cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    foral_des: new ModelEntry(CELL_TYPES.STRING, false),
    Setup_Cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    cop_cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    Cop_Des: new ModelEntry(CELL_TYPES.STRING, false),
    COVER: new ModelEntry(CELL_TYPES.NUMBER, false),
    MONITORATO: new ModelEntry(CELL_TYPES.NUMBER, false),
    Blk_Flag: new ModelEntry(CELL_TYPES.NUMBER, false),
    Data_Inizio_Portinnesto: new ModelEntry(CELL_TYPES.DATE, true),
    Progetto_Cod: new ModelEntry(CELL_TYPES.NUMBER, false),
    Progetto_Nome: new ModelEntry(CELL_TYPES.STRING, false),
    Progetto_Des: new ModelEntry(CELL_TYPES.STRING, false),
    Regolamento_Cod: new ModelEntry(CELL_TYPES.NUMBER, false),
    Reg_Des: new ModelEntry(CELL_TYPES.STRING, false),
    RegolamentoDisciplinare_Cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    RegolamentoDisciplinare_Des: new ModelEntry(CELL_TYPES.STRING, false),
    Tipologia_Cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    Tipologia_Des: new ModelEntry(CELL_TYPES.STRING, false),
    Stato_Impianto: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    fase_des: new ModelEntry(CELL_TYPES.STRING, false),
    PROV: new ModelEntry(CELL_TYPES.STRING, false),
    PROVINCIA: new ModelEntry(CELL_TYPES.STRING, false),
    Com: new ModelEntry(CELL_TYPES.STRING, false),
    Localita: new ModelEntry(CELL_TYPES.STRING, false),
    SEZIONE: new ModelEntry(CELL_TYPES.STRING, false),
    FOGLIO: new ModelEntry(CELL_TYPES.NUMBER, false),
    NUMERO: new ModelEntry(CELL_TYPES.NUMBER, false),
    SUBALTERNO: new ModelEntry(CELL_TYPES.STRING, false),
    Resa: new ModelEntry(CELL_TYPES.NUMBER, false),
    ResaTotalePrevista: new ModelEntry(CELL_TYPES.NUMBER, false),
    DataTrapiantoSemina: new ModelEntry(CELL_TYPES.DATE, false),
    SettimanaTrapiantoSemina: new ModelEntry(CELL_TYPES.NUMBER, false),
    DataRaccolta: new ModelEntry(CELL_TYPES.DATE, false),
    SettimanaRaccolta: new ModelEntry(CELL_TYPES.NUMBER, false),
    DataFioritura: new ModelEntry(CELL_TYPES.DATE, false),
    cod_kpin: new ModelEntry(CELL_TYPES.STRING, false),
    cod_block: new ModelEntry(CELL_TYPES.STRING, false),
    cod_grower: new ModelEntry(CELL_TYPES.STRING, false),
    Distinta_Chiusa: new ModelEntry(CELL_TYPES.STRING, false),
    codiceImpiantoRibaltato: new ModelEntry(CELL_TYPES.STRING, false),
    replicaGiasRagioneSociale: new ModelEntry(CELL_TYPES.STRING, false),
    replicaGiasPiva: new ModelEntry(CELL_TYPES.STRING, false),
    Utilizzo: new ModelEntry(CELL_TYPES.STRING, false),
    Destinazione_Uso_Cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    Destinazione_Uso_Des: new ModelEntry(CELL_TYPES.STRING, false),
    Attivo: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    Bloccato: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    StaticMapBase64String: new ModelEntry(CELL_TYPES.CUSTOM, false),
    GisPresente: new ModelEntry(CELL_TYPES.STRING, false),
    descrizione: new ModelEntry(CELL_TYPES.CUSTOM, false),
    ZVN: new ModelEntry(CELL_TYPES.STRING, false),
    Metodo_Produzione_Cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false), // TODO DARIO
    Metodo_Produzione_Des: new ModelEntry(CELL_TYPES.STRING, false),
    Mat_Cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    Mat_Des: new ModelEntry(CELL_TYPES.STRING, false),
    SupBZ_Riduzione: new ModelEntry(CELL_TYPES.NUMBER, false),
    DistBZ_CorpiIdrici: new ModelEntry(CELL_TYPES.NUMBER, false),
    DistBZ_AreeResPub: new ModelEntry(CELL_TYPES.NUMBER, false),
    DistBZ_Allevamenti: new ModelEntry(CELL_TYPES.NUMBER, false),
    DistBZ_VegNatNonColt: new ModelEntry(CELL_TYPES.NUMBER, false),
    N: new ModelEntry(CELL_TYPES.NUMBER, true),
    P: new ModelEntry(CELL_TYPES.NUMBER, true),
    K: new ModelEntry(CELL_TYPES.NUMBER, true),
    Mg: new ModelEntry(CELL_TYPES.NUMBER, true),
    organismo_Referente: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    organismo_Referente_des: new ModelEntry(CELL_TYPES.STRING, false),
    GruppoRaccolta_Cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    GruppoRaccolta_Des: new ModelEntry(CELL_TYPES.STRING, false),
    Ribaltato: new ModelEntry(CELL_TYPES.STRING, false),
    ribaltatoDes: new ModelEntry(CELL_TYPES.STRING, false),
    Data_Ribaltamento: new ModelEntry(CELL_TYPES.DATE, false),
    OrdiniCollegati: new ModelEntry(CELL_TYPES.NUMBER, false),
    LinkedMachines: new ModelEntry(CELL_TYPES.STRING, false),
    LinkedACAContributes: new ModelEntry(CELL_TYPES.STRING, false),
    Slope: new ModelEntry(CELL_TYPES.NUMBER, false),
    Clay: new ModelEntry(CELL_TYPES.NUMBER, false),
    Sand: new ModelEntry(CELL_TYPES.NUMBER, false),
    Slit: new ModelEntry(CELL_TYPES.NUMBER, false),
    WeavingClass: new ModelEntry(CELL_TYPES.STRING, false),
    Agea_codiBarrScheVali: new ModelEntry(CELL_TYPES.STRING, false),
    Agea_idAppezzamentoOrig: new ModelEntry(CELL_TYPES.STRING, false),
    Agea_identificativoAppezzamento: new ModelEntry(CELL_TYPES.STRING, false),
    Agea_identificativoIsola: new ModelEntry(CELL_TYPES.STRING, false),
    Agea_identificativoPianoColtivazione: new ModelEntry(CELL_TYPES.STRING, false),
    Agea_idSchedaValidazione: new ModelEntry(CELL_TYPES.STRING, false),
    Agea_idColt: new ModelEntry(CELL_TYPES.STRING, false),
    flagCessata: new ModelEntry(CELL_TYPES.STRING, false)
  };

  private eserciziColumns: KendoGridColumn[] = [
    new KendoGridColumn(
      {field: 'SA_COD', title: this.translocoService.translate('Centro')},
      {resizable: true, editable: true, validators: [Validators.required], width: 120}
    ),
    new KendoGridColumn(
      {field: 'Campo_Cod', title: this.translocoService.translate('Campo')},
      {resizable: true, editable: false, hidden: true}
    ),
    new KendoGridColumn(
      {field: 'Campo_Des', title: this.translocoService.translate('Campo')},
      {resizable: true, editable: true, width: 120}
    ),
    new KendoGridColumn(
      {field: 'app_nome', title: this.translocoService.translate('Appezzamento')},
      {resizable: true, editable: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 140}
    ),
    new KendoGridColumn(
      {field: 'Metodo_Produzione_Cod', title: this.translocoService.translate('MetodoDiProduzione')},
      {resizable: true, editable: true, width: 120}
    ),
    new KendoGridColumn(
      {field: 'veg_cod', title: this.translocoService.translate('Specie')},
      {resizable: true, editable: true, width: 120}
    ),
    new KendoGridColumn(
      {field: 'cul_cod', title: this.translocoService.translate('Varietà')},
      {resizable: true, editable: true, width: 120}
    ),
    new KendoGridColumn(
      {field: 'Destinazione_Uso_Cod', title: this.translocoService.translate('Utilizzo')},
      {resizable: true, editable: true, width: 120}
    ),
    new KendoGridColumn(
      {field: 'GRFI_COD', title: this.translocoService.translate('Finalità')},
      {resizable: true, editable: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 120}
    ),
    new KendoGridColumn(
      {field: 'GRVA_Cod_VEG', title: this.translocoService.translate('TipologiaVarietale')},
      {resizable: true, editable: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 150}
    ),
    new KendoGridColumn(
      {field: 'dettSpeciePersonalizzatoCod', title: this.translocoService.translate('DettaglioVarietaPersonalizzato')},
      {resizable: true, editable: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 150}
    ),
    new KendoGridColumn(
      {field: 'sup_imp', title: this.translocoService.translate('Superficie')},
      {
        resizable: true,
        editable: true,
        numeric: {defaultValue: 0, min: 0, format: 'n4'},
        validators: [Validators.required, Validators.min(0.0001)],
        width: 120
      }
    ),
    new KendoGridColumn(
      {field: 'Sup_Int_Alt', title: this.translocoService.translate('SupAlternativa')},
      {resizable: true, editable: true, numeric: {defaultValue: 0, min: 0, format: 'n4'}, width: 120}
    ),
    new KendoGridColumn(
      {field: 'Udm_Cod_Alt', title: this.translocoService.translate('UnitaMisuraAlternativa')},
      {resizable: true, editable: true, width: 120}
    ),
    new KendoGridColumn(
      {field: 'Validita_Inizio_Impianto', title: this.translocoService.translate('Validita_Inizio_Impianto')},
      {resizable: true, editable: true, date: {defaultValue: AGRODATAINIZIO}, width: 165}
    ),
    new KendoGridColumn(
      {field: 'Data_Inizio_Impianto', title: this.translocoService.translate('DataInizioImpianto')},
      {resizable: true, editable: true, date: {defaultValue: AGRODATAINIZIO}, width: 165}
    ),
    new KendoGridColumn(
      {field: 'Data_Inizio_Produzione', title: this.translocoService.translate('DataInizioProduzione')},
      {resizable: true, editable: true, date: {defaultValue: AGRODATAINIZIO}, width: 165}
    ),
    new KendoGridColumn(
      {field: 'Validita_Inizio', title: this.translocoService.translate('InizioEsercizio')},
      {resizable: true, editable: true, date: {defaultValue: AGRODATAINIZIO}, width: 165}
    ),
    new KendoGridColumn(
      {field: 'Validita_Fine', title: this.translocoService.translate('FineEsercizio')},
      {resizable: true, editable: true, date: {defaultValue: AGRODATAFINE}, width: 165}
    ),
    new KendoGridColumn(
      {field: 'Codice_Impianto', title: this.translocoService.translate('CodiceImpianto')},
      {resizable: true, editable: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 140}
    ),
    new KendoGridColumn(
      {field: 'tra_fila_m', title: this.translocoService.translate('TraFila')},
      {
        resizable: true,
        editable: true,
        numeric: {defaultValue: 0, min: 0, format: 'n2'},
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 100
      }
    ),
    new KendoGridColumn(
      {field: 'su_fila_m', title: this.translocoService.translate('SuFila')},
      {
        resizable: true,
        editable: true,
        numeric: {defaultValue: 0, min: 0, format: 'n2'},
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 100
      }
    ),
    new KendoGridColumn(
      {field: 'Setup_Cod', title: this.translocoService.translate('SeminaTrapianto')},
      {resizable: true, editable: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 100}
    ),
    new KendoGridColumn(
      {field: 'interbina', title: this.translocoService.translate('interbina')},
      {
        resizable: true,
        editable: true,
        numeric: {defaultValue: 0, min: 0, format: 'n2'},
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 100
      }
    ),
    new KendoGridColumn(
      {field: 'germinabilita', title: this.translocoService.translate('germinabilita')},
      {
        resizable: true,
        editable: true,
        numeric: {defaultValue: 100, min: 0, format: 'n2'},
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 100
      }
    ),
    new KendoGridColumn(
      {field: 'pianteHa', title: this.translocoService.translate('piante_Ha')},
      {
        resizable: true,
        editable: true,
        numeric: {defaultValue: 0, min: 0, format: 'n2'},
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 100
      }
    ),
    new KendoGridColumn(
      {field: 'pianteImpianto', title: this.translocoService.translate('piante_Impianto')},
      {
        resizable: true,
        editable: true,
        numeric: {defaultValue: 0, min: 0, format: 'n0'},
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 100
      }
    ),
    new KendoGridColumn(
      {field: 'port_cod', title: this.translocoService.translate('Portinnesto')},
      {resizable: true, editable: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 130}
    ),
    new KendoGridColumn(
      {field: 'foral_cod', title: this.translocoService.translate('FormaAllevamento')},
      {resizable: true, editable: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 160}
    ),
    new KendoGridColumn(
      {field: 'cop_cod', title: this.translocoService.translate('Copertura')},
      {resizable: true, editable: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 130}
    ),
    new KendoGridColumn(
      {field: 'Data_Inizio_Portinnesto', title: this.translocoService.translate('DataInizioPortinnesto')},
      {resizable: true, editable: true, date: {defaultValue: AGRODATAINIZIO}, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 165}
    ),
    new KendoGridColumn(
      {field: 'Progetto_Nome', title: this.translocoService.translate('Lotto2')},
      {resizable: true, editable: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135}
    ),
    new KendoGridColumn(
      {field: 'Progetto_Des', title: this.translocoService.translate('Descrizione')},
      {resizable: true, editable: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135}
    ),
    new KendoGridColumn(
      {field: 'RegolamentoDisciplinare_Cod', title: this.translocoService.translate('Vincolo')},
      {resizable: true, editable: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135}
    ),
    new KendoGridColumn(
      {field: 'Mat_Cod', title: this.translocoService.translate('Prodotto')},
      {resizable: true, editable: true, width: 120}
    ),
    new KendoGridColumn(
      {field: 'Tipologia_Cod', title: this.translocoService.translate('Tipologia')},
      {resizable: true, editable: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135}
    ),
    new KendoGridColumn(
      {field: 'Stato_Impianto', title: this.translocoService.translate('Stato')},
      {resizable: true, editable: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135}
    ),
    new KendoGridColumn(
      {field: 'PROV', title: this.translocoService.translate('ProvinciaAbbr')},
      {resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 100}
    ),
    new KendoGridColumn(
      {field: 'PROVINCIA', title: this.translocoService.translate('Provincia')},
      {resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135}
    ),
    new KendoGridColumn(
      {field: 'Com', title: this.translocoService.translate('ComuneAbbr')},
      {resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 100}
    ),
    new KendoGridColumn(
      {field: 'Localita', title: this.translocoService.translate('Comune')},
      {resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135}
    ),
    new KendoGridColumn(
      {field: 'SEZIONE', title: this.translocoService.translate('SezioneAbbr')},
      {resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 90}
    ),
    new KendoGridColumn(
      {field: 'FOGLIO', title: this.translocoService.translate('FoglioAbbr')},
      {
        resizable: true,
        editable: false,
        numeric: {defaultValue: 0, min: 0, format: 'n0'},
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 90
      }
    ),
    new KendoGridColumn(
      {field: 'NUMERO', title: this.translocoService.translate('NumeroAbbr')},
      {
        resizable: true,
        editable: false,
        numeric: {defaultValue: 0, min: 0, format: 'n0'},
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 90
      }
    ),
    new KendoGridColumn(
      {field: 'SUBALTERNO', title: this.translocoService.translate('SubalternoAbbr')},
      {resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 90}
    ),
    new KendoGridColumn(
      {field: 'ZVN', title: this.translocoService.translate('ZVN')},
      {resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 90}
    ),
    new KendoGridColumn(
      {field: 'Resa', title: this.translocoService.translate('ResaPrevistaPerHa')},
      {
        resizable: true,
        editable: true,
        numeric: {defaultValue: 0, min: 0, format: 'n4'},
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 100
      }
    ),
    new KendoGridColumn(
      {field: 'ResaTotalePrevista', title: this.translocoService.translate('ResaTotalePrevista')},
      {
        resizable: true,
        editable: true,
        numeric: {defaultValue: 0, min: 0, format: 'n4'},
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 100
      }
    ),
    new KendoGridColumn(
      {field: 'DataTrapiantoSemina', title: this.translocoService.translate('DataTrapiantoSemina')},
      {resizable: true, editable: true, date: {defaultValue: AGRODATAINIZIO}, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 100}
    ),
    new KendoGridColumn(
      {field: 'SettimanaTrapiantoSemina', title: this.translocoService.translate('SettimanaTrapiantoSemina')},
      {resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 100}
    ),
    new KendoGridColumn(
      {field: 'DataRaccolta', title: this.translocoService.translate('DataRaccolta')},
      {resizable: true, editable: true, date: {defaultValue: AGRODATAFINE}, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 100}
    ),
    new KendoGridColumn(
      {field: 'SettimanaRaccolta', title: this.translocoService.translate('SettimanaRaccolta')},
      {resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 100}
    ),
    new KendoGridColumn(
      {field: 'DataFioritura', title: this.translocoService.translate('DataFioritura')},
      {resizable: true, editable: true, date: {defaultValue: AGRODATAINIZIO}, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 100}
    ),
    new KendoGridColumn(
      {field: 'Distinta_Chiusa', title: this.translocoService.translate('EsercizioChiuso')},
      {resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 110}
    ),
    new KendoGridColumn(
      {field: 'Attivo', title: this.translocoService.translate('Attivo')},
      {resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 90}
    ),
    new KendoGridColumn(
      {field: 'Bloccato', title: this.translocoService.translate('Bloccato')},
      {resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 100}
    ),
    new KendoGridColumn(
      {field: 'SupBZ_Riduzione', title: this.translocoService.translate('SupBZRiduzione')},
      {
        resizable: true,
        editable: true,
        numeric: {defaultValue: 0, min: 0, format: 'n2'},
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 165
      }
    ),
    new KendoGridColumn(
      {field: 'DistBZ_CorpiIdrici', title: this.translocoService.translate('DistBZCorpiIdrici')},
      {
        resizable: true,
        editable: true,
        numeric: {defaultValue: 0, min: 0, format: 'n2'},
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 165
      }
    ),
    new KendoGridColumn(
      {field: 'DistBZ_AreeResPub', title: this.translocoService.translate('DistBZAreeResPub')},
      {
        resizable: true,
        editable: true,
        numeric: {defaultValue: 0, min: 0, format: 'n2'},
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 165
      }
    ),
    new KendoGridColumn(
      {field: 'DistBZ_Allevamenti', title: this.translocoService.translate('DistBZAllevamenti')},
      {
        resizable: true,
        editable: true,
        numeric: {defaultValue: 0, min: 0, format: 'n2'},
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 165
      }
    ),
    new KendoGridColumn(
      {field: 'DistBZ_VegNatNonColt', title: this.translocoService.translate('DistBZVegNatNonColt')},
      {
        resizable: true,
        editable: true,
        numeric: {defaultValue: 0, min: 0, format: 'n2'},
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 165
      }
    ),
    new KendoGridColumn(
      {field: 'N', title: this.translocoService.translate('n')},
      {
        resizable: true,
        editable: true,
        numeric: {defaultValue: 0, min: 0, format: 'n2'},
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 165
      }
    ),
    new KendoGridColumn(
      {field: 'P', title: this.translocoService.translate('p2o5')},
      {
        resizable: true,
        editable: true,
        numeric: {defaultValue: 0, min: 0, format: 'n2'},
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 165
      }
    ),
    new KendoGridColumn(
      {field: 'K', title: this.translocoService.translate('k2o')},
      {
        resizable: true,
        editable: true,
        numeric: {defaultValue: 0, min: 0, format: 'n2'},
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 165
      }
    ),
    new KendoGridColumn(
      {field: 'Mg', title: this.translocoService.translate('mgo')},
      {
        resizable: true,
        editable: true,
        numeric: {defaultValue: 0, min: 0, format: 'n2'},
        media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
        width: 165
      }
    ),
    new KendoGridColumn(
      {field: 'organismo_Referente', title: this.translocoService.translate('OrganismoReferente')},
      {resizable: true, editable: true, width: 165}
    ),
    new KendoGridColumn(
      {field: 'Data_Modifica', title: this.translocoService.translate('DataModifica')},
      {resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135}
    ),
    new KendoGridColumn(
      {field: 'utente_modifica', title: this.translocoService.translate('UtenteModifica')},
      {resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 145}
    ),
    new KendoGridColumn(
      {field: 'Data_Creazione', title: this.translocoService.translate('DataCreazione')},
      {resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 145}
    ),
    new KendoGridColumn(
      {field: 'utente_creazione', title: this.translocoService.translate('UtenteCreazione')},
      {resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 155}
    ),
    new KendoGridColumn(
      {field: 'GruppoRaccolta_Cod', title: this.translocoService.translate('GruppoRaccolta')},
      {resizable: true, editable: true, width: 165}
    ),
    new KendoGridColumn(
      {field: 'LinkedACAContributes', title: this.translocoService.translate('ContributiACA')},
      {resizable: true, editable: false, width: 165}
    ),
    new KendoGridColumn(
      {field: 'Slope', title: this.translocoService.translate('SlopePercentage')},
      {resizable: true, editable: false, width: 165, numeric: {defaultValue: 0, min: 0, format: 'n2'}}
    ),
    new KendoGridColumn(
      {field: 'Clay', title: this.translocoService.translate('PercArgilla')},
      {resizable: true, editable: false, width: 165, numeric: {defaultValue: 0, min: 0, max: 100, format: 'n2'}}
    ),
    new KendoGridColumn(
      {field: 'Sand', title: this.translocoService.translate('PercSabbia')},
      {resizable: true, editable: false, width: 165, numeric: {defaultValue: 0, min: 0, max: 100, format: 'n2'}}
    ),
    new KendoGridColumn(
      {field: 'Slit', title: this.translocoService.translate('PercLimo')},
      {resizable: true, editable: false, width: 165, numeric: {defaultValue: 0, min: 0, max: 100, format: 'n2'}}
    ),
    new KendoGridColumn(
      {field: 'WeavingClass', title: this.translocoService.translate('ClasseTessitura')},
      {resizable: true, editable: false, width: 165}
    ),
    new KendoGridColumn(
      {field: 'Agea_idAppezzamentoOrig', title: this.translocoService.translate('Agea_idAppezzamentoOrig')},
      {resizable: true, editable: false, width: 165, hidden: true}
    ),
    new KendoGridColumn(
      {field: 'Agea_codiBarrScheVali', title: this.translocoService.translate('Agea_codiBarrScheVali')},
      {resizable: true, editable: false, width: 165, hidden: true}
    ),
    new KendoGridColumn(
      {field: 'Agea_identificativoAppezzamento', title: this.translocoService.translate('Agea_identificativoAppezzamento')},
      {resizable: true, editable: false, width: 165, hidden: true}
    ),
    new KendoGridColumn(
      {field: 'Agea_identificativoIsola', title: this.translocoService.translate('Agea_identificativoIsola')},
      {resizable: true, editable: false, width: 165, hidden: true}
    ),
    new KendoGridColumn(
      {field: 'Agea_identificativoPianoColtivazione', title: this.translocoService.translate('Agea_identificativoPianoColtivazione')},
      {resizable: true, editable: false, width: 165, hidden: true}
    ),
    new KendoGridColumn(
      {field: 'Agea_idSchedaValidazione', title: this.translocoService.translate('Agea_idSchedaValidazione')},
      {resizable: true, editable: false, width: 165, hidden: true}
    ),
    new KendoGridColumn(
      {field: 'Agea_idColt', title: this.translocoService.translate('Agea_idColt')},
      {resizable: true, editable: false, width: 165, hidden: true}
    ),
    new KendoGridColumn(
      {field: 'flagCessata', title: this.translocoService.translate('Annullato')},
      {resizable: true, editable: false, width: 165, hidden: true}
    )
  ];

  private lastAction: string = '';
  private currentTrasformatiValue: Prodotto[];
  private currentUdmAlt: UnitaDiMisura_Alternativa[];

  private datiPrevisionaliSubs: Subscription[] = [];
  private datiPrevisionaliSignal$: Subject<void> = new Subject<void>();

  private _destroyRef: DestroyRef = inject(DestroyRef);

  private readonly objParametriAgenda: ObjParametriAgenda;
  private readonly renderer: Renderer2;
  private readonly STATO_INPRODUZIONE: number = 102;

  constructor(
    injector: Injector,
    @Inject(IMPIANTI_SERVICE_TOKEN) private appezzamentiService: ImpiantiFactoryService,
    private appezzamentoEditService: AppezzamentoEditService,
    private objParametriAgendaService: ObjParametriAgendaService,
    private centriService: CentriAziendaliService,
    @Inject(CAMPI_SERVICE_TOKEN) private campiService: CampiFactoryService,
    private specieVegetaliService: SpecieVegetaliService,
    private varietaService: VarietaService,
    private destinazioniUso: DestinazioneUsoService,
    private gruppoFinalitaService: GruppoFinalitaService,
    private eserciziEventsService: EserciziEventsService,
    private gruppoVarietaleService: GruppoVarietaleService,
    private formaAllevamentoService: FormaAllevamentoService,
    private portinnestoService: PortinnestoService,
    private coperturaService: CoperturaService,
    private rendererFactory: RendererFactory2,
    private permessiUtenteService: PermessiUtenteService,
    private anagraficaService: AnagraficaService,
    private giasMessageService: GiasMessageService,
    private giasDialogService: GiasDialogService,
    private pianoConcimazioneService: PianoConcimazioneService,
    private vincoliService: VincoliService,
    private translocoService: TranslocoService,
    private nuovaOperazioneService: NuovaOperazioneService,
    private budgetService: BudgetService,
    private dialogService: DialogService,
    private copiaSpostaService: EserciziCopiaSpostaAppezzamentiService,
    private contattiService: ContattiService,
    private prodottiService: ProdottiService,
    private conversioniService: ConversioniService,
    @Inject(IMPRESE_SERVICE_TOKEN) private impreseService: ImpreseFactoryService,
    private gruppiRaccoltaservice: GruppiRaccoltaService,
    private seminaTrapiantoService: SeminaTrapiantoService,
    private impiantiService: ImpiantiService,
    private sharedDataService: SharedDataService,
    private unitaMisuraService: UnitaDiMisuraService,
    private codificaInfoAggiuntiveService: CodificaInfoAggiuntiveService,
    private configurazioneSitiService: ConfigurazioneSitiService,
    private sementieriService: SementieriService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);

    this.renderer = this.rendererFactory.createRenderer(null, null);

    this.configurazioneSitiService
      .leggiChiave(EnumChiaviConfigurazioneSiti.Is_Sementieri)
      .pipe(
        take(1),
        map(configurazioneChiave => configurazioneChiave?.Valore?.toLowerCase() === 'true')
      ) // we just care about the sementieri server, not the active sportello
      .subscribe(isSementieri => this.handleCustomizations(isSementieri));

    this.handleCommands();

    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();

    this.gridPublicService.changeDetected.pipe(
      takeUntilDestroyed(this._destroyRef),
      switchMap((event: any) => {
        this.lastAction = event.action;
        this.trasformatoSelected = false;
        this.disableFormControls(event.action);

        if (event.action == 'edit') {
          return this.onGridEditAction(event);
        } else if (event.action == 'add') {
          return this.onGridAddAction(event);
        } else if (event.action == 'cancel') {
          return of(null);
        } else {
          return of(null);
        }
      }),
      switchMap((vals) => {
        if (!!vals) {
          const appezzamento: Appezzamento = vals[1];
          const impianto = appezzamento.impianti.find(imp => vals[0].dataItem.id_Reg == imp.primaryKey.codice);
          let obs = this.appezzamentiService.verifica_OperazioniAgenda(impianto).pipe(
            map((r) => {
              if (r.RispostaOK) {
                const movimentazioni: boolean = r.RispostaStringa['movimentazioni'];
                const trattamentiConcimazioni: boolean = r.RispostaStringa['trattamentiConcimazioni'];
                return {
                  movimentazioni: movimentazioni,
                  trattamentiConcimazioni: trattamentiConcimazioni
                };
              }
            })
          );
          return forkJoin([of(appezzamento), obs]);
        } else {
          return this.impreseService.impresaBiologica(this.objParametriAgenda.Piva);
        }
      }),
      tap((vals) => {
        let appezza;
        const fb = this.gridPublicService.formGroup.getValue();
        if (!isArray(vals) && fb != undefined) {
          appezza = null;
          if (vals) {
            fb.controls['Metodo_Produzione_Cod'].setValue(3);
            fb.controls['Metodo_Produzione_Des'].setValue('Biologico');
            this.leggiVincoloDaCodice('4').pipe(
              take(1),
              tap((vincoloBio) => {
                fb.controls['RegolamentoDisciplinare_Cod'].setValue(vincoloBio.codice);
                fb.controls['RegolamentoDisciplinare_Des'].setValue(vincoloBio.descrizione);
              })
            ).subscribe();
          } else {
            fb.controls['Metodo_Produzione_Cod'].setValue(1);
            fb.controls['Metodo_Produzione_Des'].setValue('Integrato');
          }
          let col;
          col = this.eserciziColumns.find(s => s.field === 'Metodo_Produzione_Cod');
          col.ddl.reload.next(true);
        } else {
          appezza = vals[0];
        }

        if (fb == undefined) {
          return;
        }

        this.currentEditAppezza.next(appezza);
        if (!!appezza) {
          if (appezza.impianti.length > 1) {
            fb.controls['sup_imp'].disable();
            fb.controls['Sup_Int_Alt'].disable();
            fb.controls['Udm_Cod_Alt'].disable();

            fb.controls['Validita_Inizio_Impianto'].disable();
            fb.controls['Validita_Inizio'].disable();
            fb.controls['Validita_Fine'].disable();
          } else if (appezza.impianti.length == 1) {
            if (appezza.impianti[0].esercizi.length > 1) {
              fb.controls['Validita_Inizio_Impianto'].disable();
              fb.controls['Validita_Inizio'].disable();
              fb.controls['Validita_Fine'].disable();
            } else if (appezza.impianti[0].esercizi.length == 1) {

            }
          }

          if (appezza.catastoAppezzamento?.length > 1) {
            fb.controls['sup_imp'].disable();
            fb.controls['Sup_Int_Alt'].disable();
            fb.controls['Udm_Cod_Alt'].disable();
          }

          const impiantoSelezionato = appezza.impianti.find((el) => el.primaryKey.codice == fb.controls['id_Reg'].value);
          if (impiantoSelezionato) {
            if (impiantoSelezionato.utilizzoTerreno.classType == 'Varieta') {
              fb.controls['cul_cod'].enable({emitEvent: false});
              fb.controls['GRVA_Cod_VEG'].enable({emitEvent: false});
              fb.controls['foral_cod'].enable({emitEvent: false});
              fb.controls['port_cod'].enable({emitEvent: false});
              fb.controls['cop_cod'].enable({emitEvent: false});
              fb.controls['tra_fila_m'].enable({emitEvent: false});
              fb.controls['su_fila_m'].enable({emitEvent: false});
              fb.controls['GRFI_COD'].enable({emitEvent: false});
            } else {
              fb.controls['cul_cod'].disable({emitEvent: false});
              fb.controls['GRVA_Cod_VEG'].disable({emitEvent: false});
              fb.controls['foral_cod'].disable({emitEvent: false});
              fb.controls['port_cod'].disable({emitEvent: false});
              fb.controls['cop_cod'].disable({emitEvent: false});
              fb.controls['tra_fila_m'].disable({emitEvent: false});
              fb.controls['su_fila_m'].disable({emitEvent: false});
              fb.controls['GRFI_COD'].disable({emitEvent: false});
            }

            if (!(vals[1].movimentazioni)) {
              fb.controls['veg_cod'].enable({emitEvent: false});
              fb.controls['Destinazione_Uso_Cod'].enable({emitEvent: false});
            }

            if (!(vals[1].trattamentiConcimazioni)) {
              fb.controls['GRFI_COD'].enable({emitEvent: false});
            }

            if (impiantoSelezionato.algoritmoCodifica !== '') {
              fb.controls['Codice_Impianto'].disable({emitEvent: false});
              fb.controls['Progetto_Nome'].disable({emitEvent: false});
            }

          }
        } else {
          // const currentYear: number = new Date().getFullYear();
          fb.controls['SA_COD'].enable();
          // fb.controls['Validita_Inizio_Impianto'].setValue(new Date(currentYear, 0, 1));
          // fb.controls['Validita_Inizio'].setValue(new Date(currentYear, 0, 1));
          // fb.controls['Validita_Fine'].setValue(new Date(currentYear, 11, 31));
          fb.controls['StaticMapBase64String'].setValue('');
          fb.get('Data_Inizio_Produzione').setValue(fb.get('Validita_Inizio').value);

          fb.controls['veg_cod'].setValue(0);
          fb.controls['cul_cod'].setValue(0);
          fb.controls['Destinazione_Uso_Cod'].setValue(0);

          this.caricaCentri(fb.getRawValue()).pipe(
            take(1),
            tap((centres) => {
              if (centres.length == 1) {
                let col;
                col = this.eserciziColumns.find(s => s.field === 'SA_COD');
                fb.controls['SA_COD'].setValue(centres[0].codice);
                fb.controls['sa_nome'].setValue(centres[0].descrizione);
                col.ddl.reload.next(true);
              }
            })
          ).subscribe();
        }
      })
    ).subscribe();

    this.copiaSpostaService.getRicaricaImpianti().pipe(
      map(r => {
        if (r) {
          this.refreshGrid(true);
        }
      })
    ).subscribe();

    this.gridPublicService.formGroup.pipe(
      takeUntilDestroyed(this._destroyRef),
      switchMap((fb) => {
        return !!fb ? fb.controls['Destinazione_Uso_Cod'].valueChanges : of(null);
      }),
      tap((val) => {
        if (val) {
          this.destinazioneUsoCodChanged(this.gridPublicService.formGroup.getValue(), val);
        }
      })
    ).subscribe();

    this.gridPublicService.formGroup.pipe(
      takeUntilDestroyed(this._destroyRef),
      switchMap((fb) => {
        return !!fb ? fb.controls['SA_COD'].valueChanges : of(null);
      }),
      tap((val) => {
        if (val) {
          this.centroChanged(this.gridPublicService.formGroup.getValue(), val);
        }
      })
    ).subscribe();

    this.gridPublicService.formGroup.pipe(
      takeUntilDestroyed(this._destroyRef),
      switchMap((fb) => {
        return !!fb ? fb.controls['veg_cod'].valueChanges : of(null);
      }),
      tap((val) => {
        if (val) {
          this.vegCodChanged(this.gridPublicService.formGroup.getValue(), val);
        }
      })
    ).subscribe();

    this.gridPublicService.formGroup.pipe(
      takeUntil(this.signal),
      switchMap((fb) => {
        return !!fb ? fb.controls['tra_fila_m'].valueChanges : of(null);
      }),
      tap((val) => {
        if (val) {
          this.handleCalculateSupOrPlants(this.gridPublicService.formGroup.getValue());
        }
      })
    ).subscribe();

    this.gridPublicService.formGroup.pipe(
      takeUntil(this.signal),
      switchMap((fb) => {
        return !!fb ? fb.controls['su_fila_m'].valueChanges : of(null);
      }),
      tap((val) => {
        if (val) {
          this.handleCalculateSupOrPlants(this.gridPublicService.formGroup.getValue());
        }
      })
    ).subscribe();

    this.gridPublicService.formGroup.pipe(
      takeUntil(this.signal),
      switchMap((fb) => {
        return !!fb ? fb.controls['interbina'].valueChanges : of(null);
      }),
      tap((val) => {
        if (val) {
          this.handleCalculateSupOrPlants(this.gridPublicService.formGroup.getValue());
        }
      })
    ).subscribe();

    this.gridPublicService.formGroup.pipe(
      takeUntil(this.signal),
      switchMap((fb) => {
        return !!fb ? fb.controls['germinabilita'].valueChanges : of(null);
      }),
      tap((val) => {
        if (val) {
          this.handleCalculateSupOrPlants(this.gridPublicService.formGroup.getValue());
        }
      })
    ).subscribe();

    this.gridPublicService.formGroup.pipe(
      takeUntil(this.signal),
      switchMap((fb) => {
        return !!fb ? fb.controls['sup_imp'].valueChanges : of(null);
      }),
      tap((val) => {
        if (val) {
          this.handleCalculateSupOrPlants(this.gridPublicService.formGroup.getValue());
        }
      })
    ).subscribe();

    this.gridPublicService.formGroup.pipe(
      takeUntilDestroyed(this._destroyRef),
      takeUntil(this.signal),
      switchMap((fb) => {
        return !!fb ? fb.controls['pianteHa'].valueChanges : of(null);
      }),
      tap((val) => {
        if (val) {
          this.handleCalculateSupOrPlants(this.gridPublicService.formGroup.getValue());
        }
      })
    ).subscribe();

    this.gridPublicService.formGroup.pipe(
      takeUntil(this.signal),
      switchMap((fb) => {
        return !!fb ? fb.controls['pianteImpianto'].valueChanges : of(null);
      }),
      tap((val) => {
        if (val) {
          this.handleCalculateSupOrPlants(this.gridPublicService.formGroup.getValue());
        }
      })
    ).subscribe();

    this.gridPublicService.formGroup.pipe(
      takeUntil(this.signal),
      switchMap((fb) => {
        return !!fb ? fb.controls['Tipologia_Cod'].valueChanges : of(null);
      }),
      tap((val) => {
        if (val != null && this.gridPublicService?.formGroup?.getValue()?.getRawValue()?.veg_cod > 0) {
          this.impostaNPK();
        }
      })
    ).subscribe();

    this.gridPublicService.formGroup.pipe(
      takeUntil(this.signal),
      switchMap((fb) => {
        return !!fb ? fb.controls['Stato_Impianto'].valueChanges : of(null);
      }),
      tap((val) => {
        if (val && this.gridPublicService?.formGroup?.getValue()?.getRawValue()?.veg_cod > 0) {
          this.impostaNPK();
        }
      })
    ).subscribe();

    this.gridPublicService.formGroup.pipe(
      takeUntil(this.signal),
      switchMap((fb) => {
        return !!fb ? fb.controls['RegolamentoDisciplinare_Cod'].valueChanges : of(null);
      }),
      tap((val) => {
        if (val) {
          this.impostaTipologiaDefault();
          this.impostaStatoImpiantoDefault();
        }
      })
    ).subscribe();

    this.gridPublicService.formGroup.pipe(
      takeUntil(this.signal),
      switchMap((fb) => {
        return !!fb ? fb.controls['Mat_Cod'].valueChanges : of(null);
      }),
      tap((val) => {
        if (val) {
          this.onTrasformatoChanged(val);
        }
      })
    ).subscribe();

    this.gridPublicService.formGroup.pipe(
      takeUntil(this.signal),
      switchMap((fb) => {
        if (fb != undefined) {
          const supAltControl = fb.controls['Sup_Int_Alt'];
          const udmControl = fb.controls['Udm_Cod_Alt'];
          return combineLatest([
            supAltControl.valueChanges.pipe(startWith(supAltControl.value)),
            udmControl.valueChanges.pipe(startWith(udmControl.value)),
          ]);
        } else {
          return of([null, null]);
        }
      }),
      withLatestFrom(this.caricaUnitaDiMisuraAlt()),
      tap(([[supAlt, udmAlt], _]) => {
        if (supAlt != null && udmAlt != null && udmAlt != 0) {
          this.onSupAltChanged(supAlt, udmAlt);
        }
      })
    ).subscribe();

    this.gridPublicService.formGroup.pipe(
      takeUntil(this.signal),
      switchMap((fb) => {
        if (fb != undefined) {
          const supAltControl = fb.controls['Sup_Int_Alt'];
          return supAltControl.valueChanges.pipe(
            filter(() => supAltControl.touched),
            startWith(supAltControl.value),
            debounceTime(100),
            tap((supAlt: number) => {
              const control = fb.controls['sup_imp'];
              if (supAlt == null || supAlt == 0) {
                control?.enable();
                return;
              }

              control?.disable();
            })
          );
        } else {
          return of(null);
        }
      }),
    ).subscribe();

    this.gridPublicService.formGroup.pipe(
      takeUntil(this.signal),
      switchMap((fb) => {
        return !!fb ? fb.controls['Metodo_Produzione_Cod'].valueChanges : of(null);
      }),
      tap((val) => {
        if (val == 3) {
          this.leggiVincoloDaCodice('4').pipe(
            take(1),
            tap((vincoloBio) => {
              let fb = this.gridPublicService.formGroup.getValue();
              fb.controls['RegolamentoDisciplinare_Cod'].setValue(vincoloBio.codice);
              fb.controls['RegolamentoDisciplinare_Des'].setValue(vincoloBio.descrizione);
            })
          ).subscribe();
        }
      })
    ).subscribe();

    this.gridPublicService.formGroup.pipe(
      takeUntil(this.signal),
      switchMap((fb) => {
        if (fb != undefined) {
          const supImp = fb.controls['sup_imp'];
          const resa = fb.controls['Resa'];
          return combineLatest([
            supImp.valueChanges.pipe(startWith(supImp.value)),
            resa.valueChanges.pipe(startWith(resa.value)),
          ]);
        } else {
          return of([null, null]);
        }
      }),
      tap(([supImp, resa]) => {
        let fb = this.gridPublicService.formGroup.getValue();
        fb?.controls['ResaTotalePrevista'].setValue(supImp * resa);
      })
    ).subscribe();

    this.handleImpostazioniSuperUser();
    this.handleUserSettings();

    if (this.isBudget()) {
      this.handleIsBudget();
    }
  }

  perform(actionType: HttpAction, items: any): Observable<any> {
    return this.performAppezzamento(actionType, items);
  }

  read(): Observable<EserciziServerResult> {
    this.msgWarningDelete();
    const objAgenda: ObjParametriAgenda = cloneDeep(this.objParametriAgendaService.getObjParamValue());
    objAgenda.Appezza = 0;
    objAgenda.Id_Reg = 0;
    objAgenda.Progetto_Cod = 0;
    objAgenda.Data = AGRODATAINIZIO;

    let data: FilterData = this.anagraficaService.filterData.getValue();
    if (data.filter) {
      objAgenda.Data = data.data;
    }
    if (this.loadingService) {
      this.loadingService.set_isLoading({isLoading: true, message: '', component: this.gridPublicService.gridElRef});
    }

    return this.appezzamentiService.leggi(objAgenda).pipe(
      catchError((err) => {
        this.loadingService.set_isLoading({isLoading: false, message: '', component: this.gridPublicService.gridElRef});
        return of();
      }),
      map(result => {
        if (this.loadingService) {
          this.loadingService.set_isLoading({isLoading: false, message: '', component: this.gridPublicService.gridElRef});
        }
        if (result.length > 0 && result[0].StaticMapBase64String != null) {
          result.forEach((e) => {
            if (e.StaticMapBase64String != '') {
              e.GisPresente = 'SI';
            } else {
              e.GisPresente = 'NO';
            }
          });
          if (this.eserciziColumns.filter(el => el.field == 'StaticMapBase64String').length == 0) {
            this.eserciziColumns.splice(0, 0, new KendoGridColumn(
              {field: 'StaticMapBase64String', title: this.translocoService.translate('GIS')},
              {resizable: true, filterable: false, width: 170, editable: false, component: ImgBase64Component}
            ));
          }
        }

        result.forEach(e => {
          switch (typeof e.Ribaltato) {
            case 'number':
              if (e.Ribaltato == 0) {
                e.ribaltatoDes = this.translocoService.translate('No');
              } else {
                e.ribaltatoDes = this.translocoService.translate('Si');
              }
              break;
            case 'string':
              if (e.Ribaltato.trim().toLowerCase() == 'true') {
                e.ribaltatoDes = this.translocoService.translate('Si');
              } else {
                e.ribaltatoDes = this.translocoService.translate('No');
              }
              break;
            default:
              e.ribaltatoDes = this.translocoService.translate('No');
          }
        });

        result = result.map(e => {
          e.flagCessata = Number.parseInt(e?.flagCessata ?? '0') === 1 ? this.transloco.translate('Si') : this.transloco.translate('No');
          return e;
        });

        result.forEach(r => {
          r.LinkedMachines = (<string>r.LinkedMachines)?.trim().split('  ').join(' , ') ?? '';
          r.LinkedACAContributes = (<string>r.LinkedACAContributes)?.trim().split('  ').join(' , ') ?? '';
        });

        if (this.eserciziColumns.filter(el => el.field == 'descrizione').length == 0) {
          this.eserciziColumns.splice(0, 0, new KendoGridColumn(
            {
              field: 'descrizione',
              title: this.translocoService.translate('Descrizione')
            },
            {
              resizable: true,
              filterable: false,
              editable: false,
              media: '(max-width: ' + SMARTPHONE_WIDTH + 'px)',
              component: ImpiantoShortDescriptionComponent
            }
          ));
        }

        this.handleDropdowns();
        this.eserciziServerResult = new EserciziServerResult(result, this.eserciziColumns, this.eserciziModel);
        return this.eserciziServerResult;
      })
    );
  }

  override async getRemoveMultipleRowsMessage(opts: RemoveMultipleRowsParams) {
    let msg: string;
    msg = this.translocoService.translate('anagrafica.Delete_Msg');
    opts.data.forEach(r => {
      msg = msg.concat('\n' + r['Campo_Des'] + '  ' + r['app_nome'] + '  ' + r['Utilizzo']);
    });

    return msg;
  }

  override applyRendererRules(opts: RendererGridEvent): void {
    const {grid, gridElRef} = {...opts};
    let a = gridElRef;
    let visibleRows: [] = gridElRef.nativeElement.querySelectorAll('tbody tr');
    grid.view.forEach((el, index) => {
      const row = visibleRows[index] as HTMLElement;
      if (el.Attivo === 0) {
        this.renderer.addClass(row, 'nonAttivo');
      }
      if (el.Attivo === 1) {
        this.renderer.removeClass(row, 'nonAttivo');
      }
      if (el.Bloccato === 1) {
        // this.renderer.addClass(row, 'bloccato');
        UtilityFunctions.setStyle(this.renderer, row, '.editBtn', 'display', 'none');
        UtilityFunctions.setStyle(this.renderer, row, '#btnDelete', 'display', 'none');
      } else {
        //this.renderer.removeClass(visibleRows[index], 'bloccato');
        UtilityFunctions.setStyle(this.renderer, row, '.editBtn', 'display', 'block');
        UtilityFunctions.setStyle(this.renderer, row, '#btnDelete', 'display', 'block');
      }
    });
    this.anagraficaService.applicaFiltri();
  }

  private disableFormControls(action: string): void {
    const fb = this.gridPublicService.formGroup.getValue();
    fb?.enable({emitEvent: false});

    switch (action) {
      case 'edit':
        fb.controls['veg_cod'].disable({emitEvent: false});
        fb.controls['Destinazione_Uso_Cod'].disable({emitEvent: false});
        fb.controls['GRFI_COD'].disable({emitEvent: false});
        fb.controls['Validita_Inizio_Impianto'].disable({emitEvent: false});
        fb.controls['ResaTotalePrevista'].disable({emitEvent: false});
        break;
      case 'add':
        fb.controls['ResaTotalePrevista'].disable({emitEvent: false});
        break;
    }
  }

  private handleCalculateSupOrPlants(fb: FormGroup): void {
    if (this.lastAction == 'add' && !fb.get('sup_imp').dirty) {
      let calcoloSup = this.calcolaSuperficie(fb);
      const superficie: number = calcoloSup.superficie;
      const pianteHa: number = calcoloSup.numeroPianteHa;
      if (superficie > 0) {
        fb.controls['sup_imp'].setValue(superficie);
        fb.controls['pianteHa'].setValue(pianteHa);
      }
    } else if (fb.get('sup_imp').dirty || fb.get('tra_fila_m').dirty || fb.get('su_fila_m').dirty || fb.get('interbina').dirty || fb.get('germinabilita').dirty) {
      this.calculatePlants(fb);
    } else if (fb.get('pianteImpianto').dirty && !fb.get('pianteHa').dirty && fb.get('sup_imp').getRawValue() > 0) {
      let pianteImpianto: number = fb.get('pianteImpianto').getRawValue();
      let sup: number = fb.get('sup_imp').getRawValue();
      if (pianteImpianto > 0 && sup > 0) {
        let pHa = parseFloat((pianteImpianto / sup).toFixed(2));
        fb.controls['pianteHa'].setValue(pHa);
      }
    }
  }

  private calculatePlants(fb: FormGroup): void {
    const distanza_suFila = fb.get('su_fila_m').value;
    const distanza_traFila = fb.get('tra_fila_m').value;
    const interbina = fb.get('interbina').value;
    const germinabilita = fb.get('germinabilita').value;
    const superficie = fb.get('sup_imp').value;

    let numeroPiante: number = 0;

    if (
      distanza_suFila == 0 ||
      distanza_traFila == 0
    ) {
      return;
    }

    let denominatore: number;
    if (interbina > 0) {
      denominatore = Math.abs(distanza_traFila - interbina) * distanza_suFila;
    } else {
      denominatore = distanza_suFila * distanza_traFila;
    }

    let PianteHa: number;
    if (germinabilita != 0 && germinabilita != undefined) {
      PianteHa = 10000 / denominatore * (germinabilita / 100);
    } else {
      PianteHa = 10000 / denominatore;
    }

    if (PianteHa == Infinity) {
      fb.get('pianteHa').setValue(null);
      fb.get('pianteImpianto').setValue(null);
      return;
    }

    numeroPiante = PianteHa * superficie;

    fb.get('pianteHa').setValue(parseFloat(PianteHa.toFixed(0)), {emitEvent: false});
    fb.get('pianteImpianto').setValue(parseFloat(numeroPiante.toFixed(0)), {emitEvent: false});
  }

  private calcolaSuperficie(fb: FormGroup): { superficie: number, numeroPianteHa: number } {
    const distanza_suFila = fb.get('su_fila_m').value;
    const distanza_traFila = fb.get('tra_fila_m').value;
    const interbina = fb.get('interbina').value;
    const germinabilita = fb.get('germinabilita').value;
    const numeroPiante = fb.get('pianteImpianto').value;

    // this.EsercizioEditForm.controls['piante_Ha'].setValue(piante_ha, { emitEvent: false });
    // this.EsercizioEditForm.controls['piante_Impianto'].setValue(piante_impianto, { emitEvent: false });

    let flag = true;
    // Controllo se i campi richiesti sono stati riempiti
    if (distanza_suFila == 0 || distanza_traFila == 0) {
      flag = false;
    }

    if (numeroPiante == undefined || numeroPiante == 0) {
      flag = false;
    }

    if (flag) {

      let denominatore;

      if (interbina > 0) {
        denominatore = Math.abs(distanza_traFila - interbina) * distanza_suFila;
      } else {
        denominatore = distanza_suFila * distanza_traFila;
      }

      let PianteHa: number;
      if (germinabilita != 0 && germinabilita != undefined && germinabilita != null) {
        PianteHa = 10000 / denominatore * (germinabilita / 100);
      } else {
        PianteHa = 10000 / denominatore;
      }

      if (PianteHa == Infinity) {
        return;
      }
      const superficie = numeroPiante / PianteHa;

      return {
        superficie: parseFloat(superficie.toFixed(4)),
        numeroPianteHa: parseFloat(PianteHa.toFixed(4)),
      };
    } else {
      return {
        superficie: 0,
        numeroPianteHa: 0
      };
    }
  }

  private destinazioneUsoCodChanged(fb: FormGroup, val: any): void {
    if (val != undefined && val != 0) {
      let col;
      let colVeg_Cod;
      colVeg_Cod = this.eserciziColumns.find(s => s.field === 'veg_cod');

      fb.controls['veg_cod'].setValue(0);
      fb.controls['veg_des'].setValue('');
      fb.controls['cul_cod'].setValue(0);
      fb.controls['cul_des'].setValue('');
      fb.controls['N'].setValue(0);
      fb.controls['P'].setValue(0);
      fb.controls['K'].setValue(0);
      fb.controls['Mg'].setValue(0);
      this.impostaStatoImpiantoDefault();
      this.impostaTipologiaDefault();

      let colFinalita = this.eserciziColumns.find(s => s.field === 'GRFI_COD');
      fb.controls['GRFI_COD'].setValue(0);
      fb.controls['Grfi_Des'].setValue('');
      colFinalita.ddl.reload.next(true);


      let colRegolamentoDisciplinare = this.eserciziColumns.find(s => s.field === 'RegolamentoDisciplinare_Cod');
      fb.controls['RegolamentoDisciplinare_Cod'].setValue('1');
      fb.controls['RegolamentoDisciplinare_Des'].setValue('Nessuno');
      colRegolamentoDisciplinare.ddl.reload.next(true);


      col = this.eserciziColumns.find(s => s.field === 'port_cod');
      fb.controls['port_cod'].setValue(0);
      fb.controls['port_des'].setValue('');
      col.ddl.reload.next(true);


      col = this.eserciziColumns.find(s => s.field === 'foral_cod');
      fb.controls['foral_cod'].setValue(0);
      fb.controls['foral_des'].setValue('');
      col.ddl.reload.next(true);

      col = this.eserciziColumns.find(s => s.field === 'GRVA_Cod_VEG');
      fb.controls['GRVA_Cod_VEG'].setValue(0);
      fb.controls['grva_des'].setValue('');
      col.ddl.reload.next(true);

      col = this.eserciziColumns.find(s => s.field === 'cop_cod');
      fb.controls['cop_cod'].setValue(0);
      fb.controls['Cop_Des'].setValue('');
      col.ddl.reload.next(true);

      colVeg_Cod.ddl.reload.next(true);
    }
  }

  private centroChanged(fb: FormGroup, val: any): void {
    if (val != undefined && val != 0) {
      let col;
      let colVeg_Cod;
      colVeg_Cod = this.eserciziColumns.find(s => s.field === 'Campo_Cod');

      fb.controls['Campo_Cod'].setValue(0);
      fb.controls['Campo_Des'].setValue('');

      colVeg_Cod.ddl.reload.next(true);
    }
  }

  private vegCodChanged(fb: FormGroup, val: any): void {
    let col;
    col = this.eserciziColumns.find(s => s.field === 'GRVA_Cod_VEG');
    fb.controls['GRVA_Cod_VEG'].setValue(0);
    fb.controls['grva_des'].setValue('');
    col.ddl.reload.next(true);

    col = this.eserciziColumns.find(s => s.field === 'foral_cod');
    fb.controls['foral_cod'].setValue(0);
    fb.controls['foral_des'].setValue('');
    col.ddl.reload.next(true);

    col = this.eserciziColumns.find(s => s.field === 'port_cod');
    fb.controls['port_cod'].setValue(0);
    fb.controls['port_des'].setValue('');
    col.ddl.reload.next(true);

    col = this.eserciziColumns.find(s => s.field === 'tra_fila_m');
    fb.controls['tra_fila_m'].setValue(0);

    col = this.eserciziColumns.find(s => s.field === 'su_fila_m');
    fb.controls['su_fila_m'].setValue(0);

    col = this.eserciziColumns.find(s => s.field === 'cop_cod');
    setTimeout(() => {
      fb.controls['cop_cod'].setValue(0);
      fb.controls['Cop_Des'].setValue('');
    }, 10);
    col.ddl.reload.next(true);


    this.caricaPortinnesto(fb.getRawValue()).pipe(
      take(1),
      tap((vals) => {
        col = this.eserciziColumns.find(s => s.field === 'port_cod');
        fb.controls['port_cod'].setValue(0);
        fb.controls['port_des'].setValue('');
        col.ddl.reload.next(true);
      })
    ).subscribe();


    col = this.eserciziColumns.find(s => s.field === 'foral_cod');
    setTimeout(() => {
      fb.controls['foral_cod'].setValue(0);
      fb.controls['foral_des'].setValue('');
    }, 10);
    col.ddl.reload.next(true);

    col = this.eserciziColumns.find(s => s.field === 'GRVA_Cod_VEG');
    setTimeout(() => {
      fb.controls['GRVA_Cod_VEG'].setValue(0);
      fb.controls['grva_des'].setValue('');
    }, 10);
    col.ddl.reload.next(true);

    if (this.lastAction == 'add' && val > 0) {
      fb.controls['Resa'].setValue(0);

      this.impreseService.impresaBiologica(this.objParametriAgenda.Piva).pipe(
        take(1),
        tap((bio) => {
          if (bio) {
            fb.controls['RegolamentoDisciplinare_Cod'].setValue('4');
            col = this.eserciziColumns.find(s => s.field === 'RegolamentoDisciplinare_Cod');
            col.ddl.reload.next(true);
          }
        })
      ).subscribe();
    }

    if (val != undefined && val != 0) {

      col = this.eserciziColumns.find(s => s.field === 'Destinazione_Uso_Cod');
      fb.controls['Destinazione_Uso_Cod'].setValue(0);
      fb.controls['Destinazione_Uso_Des'].setValue('');
      col.ddl.reload.next(true);

      let colVarieta = this.eserciziColumns.find(s => s.field === 'cul_cod');

      if (!this.trasformatoSelected) {
        this.varietaService.leggiVarietaAltreAsObs({codice: val, descrizione: ''}).subscribe((resp) => {
          fb.controls['cul_cod'].setValue(resp.codice);
          fb.controls['cul_des'].setValue(resp.descrizione);
          colVarieta.ddl.reload.next(true);
        });
      }

      let colFinalita = this.eserciziColumns.find(s => s.field === 'GRFI_COD');

      this.impostaStatoImpiantoDefault();
      this.impostaTipologiaDefault();
      this.caricaVincoliSpecieChanged();
      if (!this.trasformatoSelected || fb.controls['GRFI_COD']?.value == 0 || fb.controls['GRFI_COD']?.value == null || fb.controls['GRFI_COD']?.value == undefined) {
        this.gruppoFinalitaService.leggi({codice: val, descrizione: ''}, this.sharedDataService?.getCfgSementiAsValue()).then((resp) => {
          if (resp.length > 0) {
            let finalita: BaseCodeDescr;
            if (this.permessiUtenteService.getImpostazione_Utente(201)) {
              finalita = resp.find(el => el.codice == parseInt(this.permessiUtenteService.getImpostazione_Utente(201).Valore));
            }
            if (finalita) {
              fb.controls['GRFI_COD'].setValue(finalita.codice);
              fb.controls['Grfi_Des'].setValue(finalita.descrizione);
            } else {
              fb.controls['GRFI_COD'].setValue(resp[0].codice);
              fb.controls['Grfi_Des'].setValue(resp[0].descrizione);
            }
            colFinalita.ddl.reload.next(true);
          }

        });
      }

      if (!this.trasformatoSelected) {
        fb.controls['Mat_Cod'].setValue(0);
        fb.controls['Mat_Des'].setValue('');
      }

      this.trasformatoSelected = false;
      fb.controls['cul_cod'].enable({emitEvent: false});
      fb.controls['GRVA_Cod_VEG'].enable({emitEvent: false});
      fb.controls['foral_cod'].enable({emitEvent: false});
      fb.controls['port_cod'].enable({emitEvent: false});
      fb.controls['cop_cod'].enable({emitEvent: false});
      fb.controls['tra_fila_m'].enable({emitEvent: false});
      fb.controls['su_fila_m'].enable({emitEvent: false});
      fb.controls['GRFI_COD'].enable({emitEvent: false});

    } else {
      col = this.eserciziColumns.find(s => s.field === 'cul_cod');
      fb.controls['cul_cod'].setValue(0);
      fb.controls['cul_des'].setValue('');
      col.ddl.reload.next(true);

      fb.controls['cul_cod'].disable({emitEvent: false});
      fb.controls['GRVA_Cod_VEG'].disable({emitEvent: false});
      fb.controls['foral_cod'].disable({emitEvent: false});
      fb.controls['port_cod'].disable({emitEvent: false});
      fb.controls['cop_cod'].disable({emitEvent: false});
      fb.controls['tra_fila_m'].disable({emitEvent: false});
      fb.controls['su_fila_m'].disable({emitEvent: false});
      fb.controls['GRFI_COD'].disable({emitEvent: false});
      fb.controls['Mat_Cod'].setValue(0);
      fb.controls['Mat_Des'].setValue('');
    }

    this.eserciziColumns.find(s => s.field === 'Mat_Cod').ddl.reload.next(true);
  }

  private caricaVincoliSpecieChanged() {
    let fb = this.gridPublicService.formGroup.getValue();
    let dataItem = fb.getRawValue();
    if (dataItem.Metodo_Produzione_Cod == 3) {
      this.impostaVincoloBio();
    }
  }

  private impostaVincoloBio() {
    let col = this.eserciziColumns.find(s => s.field === 'RegolamentoDisciplinare_Cod');
    this.gridPublicService.formGroup.getValue().controls['RegolamentoDisciplinare_Cod'].setValue('4');
    col.ddl.reload.next(true);
  }

  private handleCustomizations(isSementieri: boolean): void {
    let showCustomColumn: boolean = this.isBudget();
    this.customColumn = new CustomColumnSettings({
      title: '',
      showColumn: showCustomColumn,
      // action: (data) => {
      //   this.redirectToPlanning(data)
      // },
      useCustomColumnCellTemplate: true
      //btnIcon: 'k-i-xi-tools-plan-management',
      //btnTooltip: this.translocoService.translate('RichiestaMaterialeVivaistico')
    });

    const permessoEdit: boolean = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_Impianto, 2);
    const permessoRemove: boolean = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_Impianto, 2);
    const permessoInfo: boolean = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_Impianto, 0);
    const permessoAgendaRead: boolean = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Agenda_AccessoMenu_NG, 0) && this.budgetService.getBudget().activeBudget === false;

    this.toolbar = new ToolbarSettings();
    this.toolbar.newItem = permessoEdit && !isSementieri;
    this.toolbar.resetChanges = false;

    this.cmdColumn = new CommandsColumnSettings({
      editBtn: permessoEdit && !isSementieri,
      removeBtn: permessoRemove && !isSementieri,
    });

    this.cmdDropDown = new CommandsDropDownSettings({
      fullEditBtn: permessoEdit && !isSementieri,
      infoBtn: permessoInfo,
    });

    if (permessoAgendaRead) {
      this.cmdDropDown.addCommand(
        new GridCommandItem(
          'ListaAttivita', GridEserciziActions.ELENCO_OPERAZIONI,
          '', faCalendar
        ));
    }

    if (permessoAgendaRead && !isSementieri) {
      this.cmdDropDown.addCommand(new GridCommandItem(
        'NuovaAttività', GridEserciziActions.NUOVA_OPERAZIONE,
        '', faPlusSquare
      ));
    }

    this.cmdDropDown.addCommand(
      new GridCommandItem(
        this.translocoService.translate('Catasto'),
        GridEserciziActions.CATASTO,
        '',
        faLayerGroup
      )
    );

    if (this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_Impianto, 2) && !isSementieri) {
      this.cmdDropDown.addCommand(
        new GridCommandItem(
          this.translocoService.translate('ChiusuraEsercizio'),
          GridEserciziActions.CHIUSURA,
          '',
          faClose
        )
      );
    }

    this.gridPublicService.openCommands.subscribe(dataItem => {
      if (!dataItem) {
        return;
      }
      if (dataItem['Bloccato'] || isSementieri) {
        this.cmdDropDown.removeCommand(CommandsDropDownEvents.FULL_EDIT);
        this.cmdDropDown.removeCommand(CommandsDropDownEvents.INLINE_EDIT);
        this.cmdDropDown.removeCommand(CommandsDropDownEvents.REMOVE);
      }
    });

    this.resizable.autoFitColumns = false;
    this.selectable.columnSettings.showSelectAll = true;
    this.selectable.shouldShowCheckbox = true;
    this.selectable.columnSettings.title = ' ';
    this.resizable.isResizable = true;
    this.selectable.selectable.checkboxOnly = true;
    this.selectable.selectable.enabled = true;
    this.selectable.preselectedRows.selectionChangeFn = this.selectionChangeFn;
    this.views.enabled = true;
    this.groups.groupable.enabled = false;

    if (window.innerWidth < SMARTPHONE_WIDTH) {
      this.groups.groupable.enabled = false;
      this.views.enabled = false;
      this.cmdColumn.editBtn = false;
      this.toolbar.newItem = false;
    }
  }

  private selectionChangeFn = (event: SelectionEvent, component: GiasKendoGridComponent) => {
    console.log(event);
    // let righeBloccate: any[] = event.selectedRows.filter(((x) => x.dataItem.Bloccato == true));
    // righeBloccate.forEach((x) => { x.dataItem.Selected = false; })
  };

  //#region "DDLs"

  private handleDropdowns(): void {
    let col: KendoGridColumn;
    let data: any[] = [];

    handleDdlConfig(this.eserciziColumns, 'SA_COD', 'descrizione', 'codice', 'sa_nome', this.caricaCentri.bind(this));
    handleDdlConfig(this.eserciziColumns, 'veg_cod', 'descrizione', 'codice', 'veg_des', this.caricaSpecieVegetali.bind(this));
    handleDdlConfig(this.eserciziColumns, 'cul_cod', 'descrizione', 'codice', 'cul_des', this.caricaCultivar.bind(this));
    handleDdlConfig(this.eserciziColumns, 'Destinazione_Uso_Cod', 'descrizione', 'codice', 'Destinazione_Uso_Des', this.caricaDestinazioniUso.bind(this));
    handleDdlConfig(this.eserciziColumns, 'GRFI_COD', 'descrizione', 'codice', 'Grfi_Des', this.caricaFinalita.bind(this));
    handleDdlConfig(this.eserciziColumns, 'Campo_Des', 'descrizione', 'codice', 'Campo_Des', this.caricaCampi.bind(this));
    handleDdlConfig(this.eserciziColumns, 'cop_cod', 'descrizione', 'codice', 'Cop_Des', this.caricaCopertura.bind(this));
    handleDdlConfig(this.eserciziColumns, 'port_cod', 'descrizione', 'codice', 'port_des', this.caricaPortinnesto.bind(this));
    handleDdlConfig(this.eserciziColumns, 'foral_cod', 'descrizione', 'codice', 'foral_des', this.caricaFormaAllevamento.bind(this));
    handleDdlConfig(this.eserciziColumns, 'GRVA_Cod_VEG', 'descrizione', 'codice', 'grva_des', this.caricaGruppoVarietale.bind(this));
    handleDdlConfig(this.eserciziColumns, 'RegolamentoDisciplinare_Cod', 'descrizione', 'codice', 'RegolamentoDisciplinare_Des', this.caricaVincoli.bind(this));
    handleDdlConfig(this.eserciziColumns, 'Tipologia_Cod', 'descrizione', 'codice', 'Tipologia_Des', this.caricaTipologie.bind(this));
    handleDdlConfig(this.eserciziColumns, 'Stato_Impianto', 'descrizione', 'codice', 'fase_des', this.caricaStatoImpianto.bind(this));
    handleDdlConfig(this.eserciziColumns, 'Metodo_Produzione_Cod', 'descrizione', 'codice', 'Metodo_Produzione_Des', this.caricaMetodoProduzione.bind(this));
    handleDdlConfig(this.eserciziColumns, 'organismo_Referente', 'descrizione', 'codice', 'organismo_Referente_des', this.caricaOrganismoReferente.bind(this));
    handleDdlConfig(this.eserciziColumns, 'Mat_Cod', 'descrizione', 'codice', 'Mat_Des', this.caricaTrasformati.bind(this));
    handleDdlConfig(this.eserciziColumns, 'Udm_Cod_Alt', 'descrizione', 'codice', 'Udm_Des_Alt', this.caricaUnitaDiMisuraAlt.bind(this));
    handleDdlConfig(this.eserciziColumns, 'GruppoRaccolta_Cod', 'descrizione', 'codice', 'GruppoRaccolta_Des', this.caricaGruppiRaccolta.bind(this));
    handleDdlConfig(this.eserciziColumns, 'dettSpeciePersonalizzatoCod', 'descrizione', 'codice', 'dettSpeciePersonalizzatoDes', this.loadDettaglioSpeciePersonalizzato.bind(this));

    // Attivo
    col = this.eserciziColumns.find(s => s.field === 'Attivo');
    data = [
      new DropdownListItem(0, 'No'),
      new DropdownListItem(1, 'Sì')
    ];

    col.ddl = new DropdownListWithForm('codice', 'Attivo', 'descrizione', data);
    col.ddl.valuePrimitive = false;

    // Bloccato
    col = this.eserciziColumns.find(s => s.field === 'Bloccato');
    data = [
      new DropdownListItem(0, 'No'),
      new DropdownListItem(1, 'Sì')
    ];

    col.ddl = new DropdownListWithForm('codice', 'Bloccato', 'descrizione', data);
    col.ddl.valuePrimitive = false;
    col = this.eserciziColumns.find(s => s.field === 'Setup_Cod');
    let ddlElements: DropdownListItem[] = this.seminaTrapiantoService.SeminaTrapiantoValues.map((el) => {
      return new DropdownListItem(el.codice, el.descrizione);
    });
    col.ddl = new DropdownListWithForm('codice', 'Setup_Cod', 'descrizione', ddlElements);
    col.ddl.valuePrimitive = false;
  }

  private caricaSpecieVegetali(dataItem: any) {
    return from(this.specieVegetaliService.leggi_FiltroUtente()).pipe(map((val) => {
      if (!(val.findIndex((sp) => {
        return sp.codice == 0;
      }) >= 0)) {
        val.push({codice: 0, descrizione: ''});
      }
      return val;
    }));
  }

  private caricaCultivar(dataItem: any): Observable<Varieta[]> {
    if (dataItem.veg_cod != null && dataItem.veg_cod != 0 && dataItem.veg_cod != 'null') {
      return this.varietaService.leggiAsObs({codice: dataItem.veg_cod, descrizione: ''});
    } else {
      return of([]);
    }
  }

  private caricaFinalita(dataItem: any) {
    if (dataItem.veg_cod != null && dataItem.veg_cod != 0) {
      return from(this.gruppoFinalitaService.leggi({
        codice: dataItem.veg_cod,
        descrizione: ''
      }, this.sharedDataService?.getCfgSementiAsValue()));
    } else {
      return of([{codice: 0, descrizione: ''}]);
    }
  }

  private caricaDestinazioniUso(dataItem: any) {
    return from(this.destinazioniUso.leggi()).pipe(
      map((val) => {
        if (!(val.findIndex((sp) => {
          return sp.codice == 0;
        }) >= 0)) {
          val.push({codice: 0, descrizione: '', classType: 'DestinazioneUso'});
        }
        return val;
      }));
  }

  private caricaCentri(dataItem: any): Observable<BaseCodeDescr[]> {
    let impresa = new Impresa;
    impresa.partitaIva = this.objParametriAgenda.Piva;
    return from(this.centriService.leggiCentriAziendaliModelloQdC(<LeggiCentriAziendali>{
      impresa: impresa,
      data: dataItem.Validita_Inizio
    }, false)).pipe(
      map((vals) => {
        return vals.map(el => {
          return {codice: el.primaryKey.codice, descrizione: el.nome};
        });
      })
    );
  }

  private caricaCampi(dataItem: any) {
    if (dataItem.SA_COD != undefined) {
      let centro = new CentroAziendale(new CentroAziendale.PK(dataItem.SA_COD, this.objParametriAgenda.Piva));
      return from(this.campiService.LeggiCampi({centro: centro, data: dataItem.Validita_Inizio}, false, '').then(vals => {
        let values = vals.map(el => {
          return {codice: el.descrizione, descrizione: el.descrizione};
        });
        return [{codice: 0, descrizione: ''}].concat(values);
      }));
    } else {
      return of([]);
    }
  }

  private caricaCopertura(dataItem: any) {
    if (dataItem.veg_cod != null && dataItem.veg_cod != 0) {
      return from(this.coperturaService.leggi({codice: toInteger(dataItem.veg_cod), descrizione: dataItem.veg_des}));
    } else {
      return of([{codice: 0, descrizione: ''}]);
    }
  }

  private caricaPortinnesto(dataItem: any) {
    if (dataItem.veg_cod != null && dataItem.veg_cod != 0) {
      return from(this.portinnestoService.leggi({codice: toInteger(dataItem.veg_cod), descrizione: dataItem.veg_des}));
    } else {
      return of([{codice: 0, descrizione: ''}]);
    }
  }

  private caricaFormaAllevamento(dataItem: any) {
    if (dataItem.veg_cod != null && dataItem.veg_cod != 0) {
      return from(this.formaAllevamentoService.leggi({codice: toInteger(dataItem.veg_cod), descrizione: dataItem.veg_des}));
    } else {
      return of([{codice: 0, descrizione: ''}]);
    }
  }

  private caricaGruppoVarietale(dataItem: any) {
    if (dataItem.veg_cod != null && dataItem.veg_cod != 0) {
      return from(this.gruppoVarietaleService.leggi({codice: toInteger(dataItem.veg_cod), descrizione: dataItem.veg_des}));
    } else {
      return of([{codice: 0, descrizione: ''}]);
    }
  }

  private caricaVincoli(dataItem: any): Observable<Vincolo[]> {
    if (dataItem.veg_cod != null && dataItem.veg_cod != 0) {
      let validita: IntervalloTemporale = new IntervalloTemporale(dataItem.Validita_Inizio, dataItem.Validita_Fine);
      return this.vincoliService.leggiVincoli(validita);
    } else {
      let vincoloDefault = new Vincolo('1');
      let disciplinare: Disciplinare = new Disciplinare('0');
      let regolamento: Regolamenti = new Regolamenti(1);
      vincoloDefault.descrizione = 'Nessuno';
      vincoloDefault.disciplinare = disciplinare;
      vincoloDefault.regolamento = regolamento;
      return of([vincoloDefault]);
    }
  }

  private caricaStatoImpianto(dataItem: any): Observable<FaseCicloColturale[]> {
    let specie: Specie = {
      codice: dataItem.veg_cod,
      descrizione: dataItem.veg_des
    };
    let finalita: GruppoFinalita = {
      specieCod: dataItem.veg_cod,
      codice: dataItem.GRFI_COD,
      descrizione: dataItem.Grfi_Des
    };
    let filtro: FiltroSpecieFinalita = {
      specie: specie,
      finalita: finalita
    };

    return this.gruppoFinalitaService.Leggi_FasiCicloColturalexSpecie(filtro).pipe(
      switchMap((fasi) => {
        if (specie && specie.codice > 0) {
          let fase102 = fasi.find((el) => {
            return el.codice == this.STATO_INPRODUZIONE;
          });
          if (!fase102) {
            fasi.push({
              codice: this.STATO_INPRODUZIONE,
              descrizione: 'In produzione',
              disciplinarePubblicoPrivato: true
            });
          }
        }
        return of(fasi);
      })
    );
  }

  private caricaMetodoProduzione(dataItem: any) {
    return of(this.appezzamentiService.getArray_Metodo_Produzione());
  }

  private caricaGruppiRaccolta(dataItem: any) {
    let objParams = this.objParametriAgendaService.getObjParamValue();
    objParams.Data = new Date();
    return from(this.gruppiRaccoltaservice.leggiGruppiRaccoltaValidi(objParams)).pipe(map(el => {
      el.push(new GruppoRaccolta(0));
      return el.map(r => {
        return {codice: r.codice, descrizione: r.descrizione};
      });
    }));
  }

  private caricaUnitaDiMisuraAlt() {
    return (this.currentUdmAlt != null && this.currentUdmAlt.length <= 1)
      ? of(this.currentUdmAlt)
      : this.conversioniService.leggi(true).pipe(
        tap(value => this.currentUdmAlt = value)
      );
  }

  private caricaTipologie(dataItem: any): Observable<FinalitaPianoConcimazione[]> {
    if (dataItem.veg_cod != null && dataItem.veg_cod != 0 &&
      dataItem.RegolamentoDisciplinare_Cod != null && dataItem.RegolamentoDisciplinare_Cod != '' &&
      dataItem.GRFI_COD != null && dataItem.GRFI_COD != 0) {

      if (dataItem.RegolamentoDisciplinare_Cod == '4' || dataItem.RegolamentoDisciplinare_Cod == '1') {
        return of([{codice: 0, descrizione: ''}]);
      }

      let specie: Specie = {
        codice: dataItem.veg_cod,
        descrizione: ''
      };
      let finalita: GruppoFinalita = {
        specieCod: 0,
        codice: dataItem.GRFI_COD,
        descrizione: ''
      };

      let disciplinare_cod = (<string>dataItem.RegolamentoDisciplinare_Cod).split('_')[1];
      let validita: IntervalloTemporale = new IntervalloTemporale();
      return this.vincoliService.leggiVincoli(validita).pipe(
        switchMap((vincoli: Vincolo[]) => {
          let vincoloSelected = vincoli.find((el) => {
            return el.codice == dataItem.RegolamentoDisciplinare_Cod;
          });
          //vincoloSelected.disciplinare.regolamentoConcimazione.codice
          let regolamento: Regolamenti = {
            codice: vincoloSelected.disciplinare.regolamentoConcimazione.codice,
            descrizione: ''
          };
          return this.pianoConcimazioneService.leggiRegolamentoConcimazionexImpianto(regolamento, specie, finalita);
        })
      );

    } else {
      return of([{codice: 0, descrizione: ''}]);
    }
  }

  private caricaOrganismoReferente(dataItem: any) {
    const piva = this.objParametriAgendaService.getObjParamValue().Piva;
    let impresa = new Impresa();
    impresa.partitaIva = piva;
    return from(this.contattiService.LeggiOrganismiReferenti(impresa)).pipe(map(el => {
      return el.map(r => {
        let descrizione;
        if (r.ragione_Sociale != null && r.ragione_Sociale != '') {
          descrizione = r.ragione_Sociale;
        } else {
          descrizione = r.cognome + ' ' + r.nome;
        }
        return {codice: r.primaryKey.codice, descrizione: descrizione};
      });
    }));
  }

  //#endregion

  private impostaStatoImpiantoDefault() {
    let fb = this.gridPublicService.formGroup.getValue();
    let dataItem = fb.getRawValue();
    let col = this.eserciziColumns.find(s => s.field === 'Stato_Impianto');
    this.caricaStatoImpianto(dataItem).pipe(
      take(1),
      tap((fasi) => {
        if (fasi.length == 0) {
          fb.controls['Stato_Impianto'].setValue(0);
          fb.controls['fase_des'].setValue('');
          return;
        }
        let fase102 = fasi.find((el) => {
          return el.codice == this.STATO_INPRODUZIONE;
        });
        if (!fase102) {
          if (fb.controls['Stato_Impianto'].getRawValue() != fasi[0].codice) {
            fb.controls['Stato_Impianto'].setValue(fasi[0].codice);
            fb.controls['fase_des'].setValue(fasi[0].descrizione);
          }
        } else {
          if (fb.controls['Stato_Impianto'].getRawValue() != fase102.codice) {
            fb.controls['Stato_Impianto'].setValue(fase102.codice);
            fb.controls['fase_des'].setValue(fase102.descrizione);
          }
        }
        col.ddl.reload.next(true);
      })
    ).subscribe();
  }

  private impostaNPK() {
    let dataItem = this.gridPublicService.formGroup.getValue().getRawValue();
    this.leggiVincoloDaCodice(dataItem.RegolamentoDisciplinare_Cod).pipe(
      take(1),
      switchMap((vincolo: Vincolo) => {
        if (vincolo && vincolo.disciplinare && vincolo.disciplinare.regolamentoConcimazione && vincolo.disciplinare.regolamentoConcimazione.codice > 0) {
          let regolamento: RegolamentoConcimazione = {
            codice: vincolo.disciplinare.regolamentoConcimazione.codice,
            descrizione: '',
            tipo: 0
          };
          let specie: Specie = {
            codice: dataItem.veg_cod,
            descrizione: ''
          };
          let finalita: FinalitaPianoConcimazione = {
            codice: dataItem.Tipologia_Cod,
            descrizione: ''
          };
          let stato: FaseCicloColturale = {
            codice: dataItem.Stato_Impianto,
            descrizione: '',
            disciplinarePubblicoPrivato: true
          };

          let filtroCalcoloNPK: FiltroCalcoloNPK = {
            regolamento: regolamento,
            specie: specie,
            finalita: finalita,
            stato: stato
          };
          return this.pianoConcimazioneService.calcoloNPKModello(filtroCalcoloNPK);
        } else if (this.lastAction == 'add' && vincolo && vincolo.regolamento && vincolo.regolamento.codice == 4) {
          return of({
            n: 0,
            p2o5: 0,
            k2o: 0,
            mgo: 0
          });
        }
        return of(null);
      }),
      tap((resp) => {
        if (resp) {
          let fb = this.gridPublicService.formGroup.getValue();
          if (resp.n) {
            fb.controls['N'].setValue(resp.n);
          }

          if (resp.p2o5) {
            fb.controls['P'].setValue(resp.p2o5);
          }

          if (resp.k2o) {
            fb.controls['K'].setValue(resp.k2o);
          }

          if (resp.mgo) {
            fb.controls['Mg'].setValue(resp.mgo);
          }

        }
      })
    ).subscribe();
  }

  private impostaTipologiaDefault() {
    let fb = this.gridPublicService.formGroup.getValue();
    let dataItem = fb.getRawValue();
    this.caricaTipologie(dataItem).pipe(
      take(1),
      tap((val: FinalitaPianoConcimazione[]) => {
        if (val && val.length > 0) {
          fb.controls['Tipologia_Cod'].setValue(val[0].codice);
          fb.controls['Tipologia_Des'].setValue(val[0].descrizione);
        } else {
          fb.controls['Tipologia_Cod'].setValue(0);
          fb.controls['Tipologia_Des'].setValue('');
        }
        let col;
        col = this.eserciziColumns.find(s => s.field === 'Tipologia_Cod');
        col.ddl.reload.next(true);
      })
    ).subscribe();
  }

  private onTrasformatoChanged(trasformatoCod: number): void {
    this.trasformatoSelected = true;
    const fb = this.gridPublicService.formGroup.getValue();
    const dataItem = fb.getRawValue();

    const item = this.currentTrasformatiValue.find(t => t.codice == trasformatoCod);
    if (item == null) {
      return;
    }

    if (item.finalita?.codice != null && item.finalita.codice > 0 && fb.controls['GRFI_COD'].touched == false) {
      fb.controls['GRFI_COD'].setValue(item.finalita.codice);
      fb.controls['Grfi_Des'].setValue(item.finalita.descrizione);
      this.gruppoFinalitaService.leggi({
        codice: item.varieta.specie.codice,
        descrizione: ''
      }, this.sharedDataService?.getCfgSementiAsValue()).then(() => {
        fb.controls['GRFI_COD'].setValue(item.finalita.codice);
        fb.controls['Grfi_Des'].setValue(item.finalita.descrizione);
        this.eserciziColumns.find(s => s.field === 'GRFI_COD').ddl.reload.next(true);
      });
    } else {
      fb.controls['GRFI_COD'].setValue(0);
      fb.controls['Grfi_Des'].setValue('');
    }

    if (item.varieta?.codice != null && (dataItem.cul_cod == null || dataItem.cul_cod == 0 || dataItem.cul_des.toLowerCase() == 'altre' || dataItem.cul_cod != 0)) {
      fb.controls['veg_cod'].setValue(item.varieta.specie.codice);
      fb.controls['veg_des'].setValue(item.varieta.specie.descrizione);
      this.eserciziColumns.find(s => s.field === 'veg_cod').ddl.reload.next(true);
      fb.controls['cul_cod'].setValue(item.varieta.codice);
      fb.controls['cul_des'].setValue(item.varieta.descrizione);
      this.eserciziColumns.find(s => s.field === 'cul_cod').ddl.reload.next(true);
    }

    if (item.regolamento?.codice != null && item.regolamento?.codice != 0 && dataItem.Regolamento_Cod != item.regolamento?.codice) {
      fb.controls['Regolamento_Cod'].setValue(item.regolamento.codice);
      fb.controls['Reg_Des'].setValue(item.regolamento.descrizione);
      if (item.regolamento?.codice == 4) {
        fb.controls['Metodo_Produzione_Cod'].setValue(3);
        fb.controls['Metodo_Produzione_Des'].setValue('Biologico');
        this.leggiVincoloDaCodice('4').pipe(
          take(1),
          tap((vincoloBio) => {
            fb.controls['RegolamentoDisciplinare_Cod'].setValue(vincoloBio.codice);
            fb.controls['RegolamentoDisciplinare_Des'].setValue(vincoloBio.descrizione);
            let col = this.eserciziColumns.find(s => s.field === 'RegolamentoDisciplinare_Cod');
            col.ddl.reload.next(true);
          })
        ).subscribe();
      } else {
        if (fb.controls['Metodo_Produzione_Cod'].value == 3) {
          fb.controls['Metodo_Produzione_Cod'].setValue(1);
          fb.controls['Metodo_Produzione_Des'].setValue('Integrato');
        }
        this.leggiVincoloDaCodice('1').pipe(
          take(1),
          tap((vincolo) => {
            fb.controls['RegolamentoDisciplinare_Cod'].setValue(vincolo.codice);
            fb.controls['RegolamentoDisciplinare_Des'].setValue(vincolo.descrizione);
            let col = this.eserciziColumns.find(s => s.field === 'RegolamentoDisciplinare_Cod');
            col.ddl.reload.next(true);
          })
        ).subscribe();
      }

    }
  }

  private onSupAltChanged(supAlt: number, udmAltCode: number): void {
    const fb = this.gridPublicService.formGroup.getValue();

    const item = this.currentUdmAlt?.find(t => t.codice == udmAltCode);
    if (item == null) {
      return;
    }

    const value = Math.round(supAlt * item.tassoConversione * 1000) / 1000;
    const superficieControl = fb.controls['sup_imp'];
    superficieControl?.setValue(value);
    superficieControl?.markAsDirty();
    superficieControl?.markAsTouched();
  }

  private leggiVincoloDaCodice(codice: string): Observable<Vincolo> {
    let validita: IntervalloTemporale = new IntervalloTemporale();
    return this.vincoliService.leggiVincoli(validita).pipe(
      switchMap((vincoli: Vincolo[]) => {
        let vincoloSelected = vincoli.find((el) => {
          return el.codice == codice;
        });
        return of(vincoloSelected);
      })
    );
  }

  private loadDettaglioSpeciePersonalizzato(dataItem: any) {
    let objParams = this.objParametriAgendaService.getObjParamValue();
    objParams.Data = new Date();
    return from(this.codificaInfoAggiuntiveService.leggiDettaglioVarietaPersonalizzato());
  }

  private caricaTrasformati(dataItem: any) {
    const impresa = new Impresa();
    impresa.partitaIva = this.objParametriAgenda.Piva;
    let cul_cod_sel = dataItem.cul_cod;
    if (dataItem?.cul_des?.toLowerCase() == 'altre') {
      cul_cod_sel = 0;
    }
    const payload = {
      specie: dataItem.veg_cod != null ? {codice: dataItem.veg_cod} : null,
      varieta: cul_cod_sel != null ? {codice: cul_cod_sel} : null,
      finalita: dataItem.GRFI_COD != null ? {codice: dataItem.GRFI_COD} : null,
      regolamento: dataItem.Regolamento_Cod != null ? {codice: dataItem.Regolamento_Cod} : null,
      impresa: impresa
    } as LeggiProdotti;
    return this.prodottiService.Leggi_Trasformati_Vegetali_Anagrafica_With_Default(payload).pipe(tap(value => this.currentTrasformatiValue = value));
  }

  private performAppezzamento(actionType: HttpAction, item: any): Observable<any> {
    const DBAppezza = this.currentEditAppezza.getValue();
    const fb = this.gridPublicService.formGroup.getValue();
    switch (actionType) {
      case HttpAction.CREATE:
        return this.sementieriService.isSementieriSportello().pipe(
          switchMap(isSementieri =>this.createAppezza(item, isSementieri))
        );
      case HttpAction.UPDATE:
        return this.updateAppezza(DBAppezza, item, fb);
      case HttpAction.REMOVE:
        let ribaltato: boolean = item?.Ribaltato == 'true';
        return this.removeAppezza(item.PIVA, item.SA_COD, item.APPEZZA, item.id_Reg, item.Progetto_Cod, ribaltato);
    }
  }

  private prepareAppezzamento(item: any, isSementieri: boolean): Appezzamento {
    const objPAgenda: ObjParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    const validitySettingInterpretation: Utente_Impostazioni = this.permessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.UTENTE_InterpretazioneInizioFineAnnataAgraria);
    const newAppezzaFb: FormGroup = this.appezzamentoEditService.getAppezzamentoForm();
    let newAppezza: Appezzamento = <Appezzamento>(<any>newAppezzaFb.getRawValue());

    newAppezza.impianti = [];
    newAppezza.impianti[0] = <Impianto>(<any>this.appezzamentoEditService.getImpiantoForm(
      0,
      newAppezza.primaryKey,
      item.Validita_Inizio_Impianto,
      item.Validita_Fine,
      isSementieri
    ).getRawValue());

    newAppezza.impianti[0].esercizi = [];
    newAppezza.impianti[0].esercizi[0] = <any>this.appezzamentoEditService.getEsercizioForm(
      0,
      newAppezza.impianti[0].primaryKey,
      item.Validita_Inizio,
      item.Validita_Fine
    ).getRawValue();

    newAppezza.primaryKey.centroAziendalePK.partitaIva = objPAgenda.Piva;
    newAppezza.primaryKey.centroAziendalePK.codice = item.SA_COD;
    newAppezza.primaryKey.codice = 0;
    newAppezza.descrizione = item.app_nome;
    newAppezza.superficie = item.sup_imp;
    newAppezza.validita.inizio = item.Validita_Inizio_Impianto ?? AGRODATAINIZIO;
    newAppezza.validita.fine = (validitySettingInterpretation?.Valore === '0' ? item.Validita_Fine : AGRODATAFINE) ?? AGRODATAFINE;
    newAppezza.supBZ_Riduzione = item.SupBZ_Riduzione;
    newAppezza.distBZ_Allevamenti = item.DistBZ_Allevamenti;
    newAppezza.distBZ_CorpiIdrici = item.DistBZ_CorpiIdrici;
    newAppezza.distBZ_AreeResPub = item.DistBZ_AreeResPub;
    newAppezza.distBZ_VegNatNonColt = item.DistBZ_VegNatNonColt;

    newAppezza.campoPK = {
      codice: item.Campo_Cod,
      centroAziendalePK: {
        codice: item.SA_COD,
        partitaIva: objPAgenda.Piva
      }
    };

    newAppezza.metodo_Produzione.codice = item.Metodo_Produzione_Cod;
    newAppezza.metodo_Produzione.descrizione = item.Metodo_Produzione_Des;

    let impianto: Impianto = newAppezza.impianti[0];
    if (isNumber(item.cul_cod) && item.cul_cod > 0) {
      const varieta: Varieta = {
        codice: item.cul_cod,
        descrizione: item.cul_des,
        specie: {
          codice: item.veg_cod,
          descrizione: item.veg_des,
        },
        classType: 'Varieta'
      };
      impianto.utilizzoTerreno = varieta;
    } else if (isNumber(item.Destinazione_Uso_Cod) && item.Destinazione_Uso_Cod > 0) {
      const destinazioneUso: DestinazioneUso = {
        codice: item.Destinazione_Uso_Cod,
        descrizione: item.Destinazione_Uso_Des,
        classType: 'DestinazioneUso'
      };
      impianto.utilizzoTerreno = destinazioneUso;
    } else {
      return undefined;
    }
    impianto.superficie = item.sup_imp;

    impianto.superficieAlternativa = item.Sup_Int_Alt ?? 0;
    impianto.unitaMisuraAlternativa = {codice: 0, descrizione: ''};
    if (item.Udm_Cod_Alt) {
      impianto.unitaMisuraAlternativa = {codice: item.Udm_Cod_Alt, descrizione: ''};
    }

    impianto.validita.inizio = item.Validita_Inizio_Impianto ?? AGRODATAINIZIO;
    impianto.validita.fine =
      (['0', '1'].some(v => v === validitySettingInterpretation?.Valore) ? item.Validita_Fine : AGRODATAFINE) ?? AGRODATAFINE;

    if (isNumber(item.GRFI_COD)) {
      impianto.gruppoFinalita.codice = item.GRFI_COD;
      impianto.gruppoFinalita.descrizione = item.Grfi_Des;
    }

    impianto.seminaTrapianto = {codice: item.Setup_Cod?.id, descrizione: ''};
    impianto.codiceImpianto = item.Codice_Impianto;
    impianto.su_Fila_M = item.su_fila_m;
    impianto.tra_Fila_M = item.tra_fila_m;
    impianto.data_Inizio_Portinnesto = item.Data_Inizio_Portinnesto ?? impianto.validita.inizio;
    impianto.data_Inizio_Impianto = item.Data_Inizio_Impianto ?? impianto.validita.inizio;
    impianto.data_Inizio_Produzione = item.Data_Inizio_Produzione ?? impianto.validita.inizio;
    if (isNumber(item.interbina)) {
      impianto.interbina = item.interbina;
    }
    if (isNumber(item.germinabilita)) {
      impianto.germinabilita = item.germinabilita;
    }
    if (isNumber(item.port_cod)) {
      impianto.portinnesto.codice = item.port_cod;
    }
    if (isNumber(item.foral_cod)) {
      impianto.formaAllevamento.codice = item.foral_cod;
    }
    if (isNumber(item.cop_cod)) {
      impianto.copertura.codice = item.cop_cod;
    }
    if (isNumber(item.GRVA_Cod_VEG)) {
      impianto.gruppoVarietale.codice = item.GRVA_Cod_VEG;
    }
    if (item.dettSpeciePersonalizzatoCod != undefined) {
      impianto.dettaglio_varieta_personalizzato.codice = item.dettSpeciePersonalizzatoCod;
    }

    let esercizio = impianto.esercizi[0];
    esercizio.validita.inizio = item.Validita_Inizio ?? AGRODATAINIZIO;
    esercizio.validita.fine = item.Validita_Fine ?? AGRODATAFINE;
    esercizio.lotto = item.Progetto_Nome;
    esercizio.descrizione = item.Progetto_Des;
    esercizio.resa_prevista = item.Resa;
    esercizio.data_Semina_Trapianto_Prevista = item.DataTrapiantoSemina ?? impianto.validita.inizio;
    esercizio.data_Fioritura_Prevista = item.DataFioritura ?? impianto.validita.inizio;
    esercizio.data_Raccolta_Prevista = item.DataRaccolta ?? impianto.validita.fine;

    esercizio.apportiMassimiMacroelementi.tipologia.codice = item.Tipologia_Cod;
    esercizio.apportiMassimiMacroelementi.fase.codice = item.Stato_Impianto;

    esercizio.apportiMassimiMacroelementi.n = item.N;
    esercizio.apportiMassimiMacroelementi.p2o5 = item.P;
    esercizio.apportiMassimiMacroelementi.k2o = item.K;
    esercizio.apportiMassimiMacroelementi.mgo = item.Mg;

    if (item.Mat_Cod != null) {
      esercizio.prodotto = {codice: item.Mat_Cod, descrizione: item.Mat_Des} as Prodotto;
    } else {
      esercizio.prodotto = {codice: 0, descrizione: ''} as Prodotto;
    }
    esercizio.piante_Impianto = item.pianteImpianto;
    esercizio.gruppoRaccolta = new BaseCodeDescr(item.GruppoRaccolta_Cod ?? 0, item.GruppoRaccolta_Des);

    if (isNumber(item.pianteHa)) {
      esercizio.piante_Ha = item.pianteHa;
    }
    if (item.Distinta_Chiusa == 1) {
      esercizio.esercizio_Chiuso = true;
    } else {
      esercizio.esercizio_Chiuso = false;
    }

    if (item.organismo_Referente != '') {
      let orgRef_contatto = new Contatto();
      orgRef_contatto.primaryKey = {codice: item.organismo_Referente, partitaIva: ''};
      esercizio.organismo_Referente = orgRef_contatto;
    }

    esercizio.apportiMassimiMacroelementi.tipologia = new FinalitaPianoConcimazione(item.Tipologia_Cod);
    return newAppezza;
  }

  private createAppezza(item: any, isSementieri: boolean): Observable<any> {
    let newAppezza: Appezzamento = this.prepareAppezzamento(item, isSementieri);

    if (newAppezza == undefined) {
      this.giasMessageService.errorMessage(this.translocoService.translate('ImpossibileSalvareImpiantoScegliDestinazione'));
      return of(false);
    }

    this.loadingService.set_isLoading({isLoading: true, message: '', component: this.gridPublicService.gridElRef});
    return this.caricaVincoli(item).pipe(
      switchMap((vincoli: Vincolo[]) => {
        let vincoloSelezionato = vincoli.find((el) => {
          return el.codice == item.RegolamentoDisciplinare_Cod;
        });
        if (vincoloSelezionato != undefined) {
          newAppezza.impianti[0].esercizi[0].vincolo = vincoloSelezionato;
        }
        return of(newAppezza);
      }),
      switchMap((app) => {
        return this.appezzamentiService.ScriviAppezzamento(app);
      }),
      tap((resp) => {
        if (resp.RispostaOK == true) {
          this.giasMessageService.successMessage(this.translocoService.translate('Impianto') + ' ' + newAppezza.descrizione + ' ' + this.translocoService.translate('Modificato') + '.');
        } else {
          if (resp.Errore != '') {
            this.giasMessageService.errorMessage(this.translocoService.translate('ErroreSalvataggio') + ' ' + this.translocoService.translate('Impianto') + newAppezza.descrizione + ': ' + resp.Errore);
          }
        }
      })
    );
  }

  private readIndirizzoCentroAziendale(sa_cod: number): Observable<rispostaStandard<IndirizzoAssociato[]>> {
    let params: LeggiIndirizziCentro = new LeggiIndirizziCentro(this.objParametriAgenda.Piva, sa_cod);
    return this.centriService.readCentreAddresses(params);
  }

  private takeAddress(indirizzi: IndirizzoAssociato[]): IndirizzoAssociato {
    let indirizzo: IndirizzoAssociato = indirizzi.find(i =>
      i?.indirizzo?.istatComune?.reg != '' &&
      i?.indirizzo?.istatComune?.reg != '000' &&
      i?.indirizzo?.istatComune?.prov != '' &&
      i?.indirizzo?.istatComune?.prov != '000'
    );
    return indirizzo ?? indirizzi[0];
  }

  private prepareDatiColtureRequestParams(indirizzo: IndirizzoAssociato): DatiPrevisionaliColtureRequest {
    const fb = this.gridPublicService.formGroup.getValue();

    let params: DatiPrevisionaliColtureRequest = new DatiPrevisionaliColtureRequest();
    params.vegCod = new BaseCodeDescr(fb.controls['veg_cod'].value ?? 0);
    params.culCod = new BaseCodeDescr(fb.controls['cul_cod'].value ?? 0);
    params.grfiCod = new BaseCodeDescr(fb.controls['GRFI_COD'].value ?? 0);
    params.grvaCod = new BaseCodeDescr(fb.controls['GRVA_Cod_VEG'].value ?? 0);
    params.validitaInizio = fb.controls['Validita_Inizio'].value;
    params.validitaFine = fb.controls['Validita_Fine'].value;
    params.portCod = new BaseCodeDescr(fb.controls['port_cod'].value ?? 0);
    params.foralCod = new BaseCodeDescr(fb.controls['foral_cod'].value ?? 0);
    params.regCod = new BaseCodeDescr(fb.controls['RegolamentoDisciplinare_Cod'].value == '4' ? 4 : 1);
    params.statoCod = new BaseCodeDescr(fb.controls['Stato_Impianto'].value ?? 0);
    params.reg = new BaseCodeDescrStr(indirizzo.indirizzo.istatComune.reg ?? '000');
    params.prov = new BaseCodeDescrStr(indirizzo.indirizzo.istatComune.prov ?? '000');
    params.codiceStato = new BaseCodeDescrStr(indirizzo.indirizzo.stato.codice ?? '00');
    params.parametroCod = new BaseCodeDescr(4);
    params.dettSpeciePersonalizzatoCod = new BaseCodeDescrStr(fb.controls['dettSpeciePersonalizzatoCod'].value ?? '');

    return params;
  }

  private updateAppezza(DBAppezza: Appezzamento, item: any, fb: FormGroup): Observable<any> {
    let modificaSuperficie = true;
    let modificaValidita = true;
    if (DBAppezza.impianti.length > 1) {
      modificaSuperficie = false;
      modificaValidita = false;
    }

    if (DBAppezza.impianti[0].esercizi.length > 1) {
      modificaValidita = false;
    }

    if (DBAppezza.catastoAppezzamento?.length > 1) {
      modificaSuperficie = false;
    }

    DBAppezza.descrizione = item.app_nome;
    if (modificaSuperficie && fb.controls['sup_imp'].dirty) {
      DBAppezza.superficie = item.sup_imp;
      if (DBAppezza.catastoAppezzamento.length == 1) {
        DBAppezza.catastoAppezzamento[0].area = item.sup_imp;
      }
    }

    if (modificaSuperficie && fb.controls['sup_imp'].dirty) {
      DBAppezza.superficie = item.sup_imp;
      if (DBAppezza.catastoAppezzamento.length == 1) {
        DBAppezza.catastoAppezzamento[0].area = item.sup_imp;
      }
    }

    if (modificaValidita && fb.controls['Validita_Inizio'].dirty) {
      DBAppezza.validita.inizio = item.Validita_Inizio;
    }

    if (modificaValidita && fb.controls['Validita_Fine'].dirty) {
      DBAppezza.validita.fine = item.Validita_Fine;
    }

    DBAppezza.metodo_Produzione.codice = item.Metodo_Produzione_Cod;
    DBAppezza.metodo_Produzione.descrizione = item.Metodo_Produzione_Des;
    DBAppezza.distBZ_AreeResPub = item.DistBZ_AreeResPub;
    DBAppezza.distBZ_Allevamenti = item.DistBZ_Allevamenti;
    DBAppezza.distBZ_CorpiIdrici = item.DistBZ_CorpiIdrici;
    DBAppezza.distBZ_VegNatNonColt = item.DistBZ_VegNatNonColt;
    DBAppezza.supBZ_Riduzione = item.SupBZ_Riduzione;

    let DBimpianto = DBAppezza.impianti.find((el) => el.primaryKey.codice == item.id_Reg);
    if (DBimpianto) {
      let cambiatoUtilizzo = false;
      switch (DBimpianto.utilizzoTerreno.classType) {
        case 'Varieta':
          if (item.veg_cod == 0) {
            //CAMBIATO UTILIZZO
            const utilizzo: DestinazioneUso = {
              codice: item.Destinazione_Uso_Cod,
              descrizione: item.Destinazione_Uso_Des,
              classType: 'DestinazioneUso'
            };
            DBimpianto.utilizzoTerreno = utilizzo;
            cambiatoUtilizzo = true;
          } else if ((<Varieta>DBimpianto.utilizzoTerreno).specie.codice != item.veg_cod) {
            //CAMBIATA SPECIE
            const utilizzo: Varieta = {
              codice: item.cul_cod,
              descrizione: item.cul_des,
              specie: {
                codice: item.veg_cod,
                descrizione: item.veg_des
              },
              classType: 'Varieta'
            };
            DBimpianto.utilizzoTerreno = utilizzo;
            cambiatoUtilizzo = true;
          } else {
            (<Varieta>DBimpianto.utilizzoTerreno).codice = item.cul_cod;
            (<Varieta>DBimpianto.utilizzoTerreno).descrizione = item.cul_des;
          }
          break;
        case 'DestinazioneUso':
          if (item.veg_cod != 0) {
            //CAMBIATO UTILIZZO
            const utilizzo: Varieta = {
              codice: item.cul_cod,
              descrizione: item.cul_des,
              specie: {
                codice: item.veg_cod,
                descrizione: item.veg_des
              },
              classType: 'Varieta'
            };
            DBimpianto.utilizzoTerreno = utilizzo;
            cambiatoUtilizzo = true;
          } else {
            (<DestinazioneUso>DBimpianto.utilizzoTerreno).codice = item.Destinazione_Uso_Cod;
            (<DestinazioneUso>DBimpianto.utilizzoTerreno).descrizione = item.Destinazione_Uso_Des;
          }
          break;
      }

      if (cambiatoUtilizzo) {
        DBimpianto.gruppoVarietale = {codice: 0, descrizione: ''};
        DBimpianto.irrigazione = {codice: 0, descrizione: ''};
        DBimpianto.formaAllevamento = {codice: 0, descrizione: ''};
        DBimpianto.portinnesto = {codice: 0, descrizione: ''};
        DBimpianto.seminaTrapianto = {codice: '', descrizione: ''};
        DBimpianto.provenienzaSeme = {codice: 0, descrizione: ''};
        DBimpianto.tecnicaConduzioneTraFila = {codice: 0, descrizione: ''};
        DBimpianto.tecnicaConduzioneSuFila = {codice: 0, descrizione: ''};
        DBimpianto.copertura = {codice: 0, descrizione: ''};
        for (let i = 0; i < DBimpianto.esercizi.length; i++) {
          if (item.veg_cod == 0) {

            DBimpianto.esercizi[i].piante_Impianto = 0;
            DBimpianto.esercizi[i].piante_Ha_Femmine = 0;
            DBimpianto.esercizi[i].piante_Ha_Impianto_Femmine = 0;
            DBimpianto.esercizi[i].piante_Ha_Maschi = 0;
            DBimpianto.esercizi[i].piante_Ha_Impianto_Maschi = 0;

            DBimpianto.esercizi[i].data_Semina_Trapianto_Prevista = AGRODATAINIZIO;
            DBimpianto.esercizi[i].data_Raccolta_Prevista = AGRODATAINIZIO;
            DBimpianto.esercizi[i].data_Fioritura_Prevista = AGRODATAINIZIO;
            DBimpianto.esercizi[i].resa_effettiva = 0;
            DBimpianto.esercizi[i].prodotto = {codice: 0, descrizione: ''} as Prodotto;

            DBimpianto.esercizi[i].regolamento = {codice: 0, descrizione: ''};
            DBimpianto.esercizi[i].disciplinare = {
              codice: '0',
              descrizione: '',
              disciplinarePubblicoPrivato: 0,
              regolamentoConcimazione: {codice: 0, descrizione: '', tipo: 0},
              gruppoFinalita: {codice: 0, descrizione: '', specieCod: 0},
              raggruppamentiColturaliDPI: {codice: 0, descrizione: ''},
              flagProtetto: 0,
              idTr: 0,
              validita: new IntervalloTemporale()
            };

          }
          DBimpianto.esercizi[i].apportiMassimiMacroelementi.pianoConcimazione = {codice: 0, descrizione: '', tipo: 0};
          DBimpianto.esercizi[i].apportiMassimiMacroelementi.tipologia = {codice: 0, descrizione: ''};
          DBimpianto.esercizi[i].apportiMassimiMacroelementi.fase = {codice: 0, descrizione: '', disciplinarePubblicoPrivato: true};
          DBimpianto.esercizi[i].apportiMassimiMacroelementi.n = 0;
          DBimpianto.esercizi[i].apportiMassimiMacroelementi.p2o5 = 0;
          DBimpianto.esercizi[i].apportiMassimiMacroelementi.k2o = 0;
          DBimpianto.esercizi[i].apportiMassimiMacroelementi.mgo = 0;
        }
      }

      DBimpianto.codiceImpianto = item.Codice_Impianto;
      DBimpianto.seminaTrapianto = {codice: item.Setup_Cod?.id, descrizione: ''};
      DBimpianto.su_Fila_M = item.su_fila_m;
      DBimpianto.tra_Fila_M = item.tra_fila_m;

      if (modificaSuperficie && fb.controls['sup_imp'].dirty) {
        DBimpianto.superficie = item.sup_imp;
      }

      //if (modificaSuperficie && fb.controls['Sup_Int_Alt'].dirty) {
      DBimpianto.superficieAlternativa = item.Sup_Int_Alt ?? 0;
      //}

      //if (modificaSuperficie && fb.controls['Udm_Cod_Alt'].dirty) {
      const udm = this.currentUdmAlt?.find(x => x.codice == item.Udm_Cod_Alt);
      DBimpianto.unitaMisuraAlternativa = udm ?? {
        codice: 0,
        descrizione: '',
        simbolo: '',
        tassoConversione: 0
      } as UnitaDiMisura_Alternativa;
      //}

      if (modificaValidita && fb.controls['Validita_Inizio'].dirty && fb.controls['Validita_Inizio'].value < DBimpianto.validita.inizio) {
        DBimpianto.validita.inizio = item.Validita_Inizio;
      }

      if (modificaValidita && fb.controls['Validita_Fine'].dirty) {
        DBimpianto.validita.fine = item.Validita_Fine;
      }

      if (isNumber(item.GRFI_COD)) {
        DBimpianto.gruppoFinalita.codice = item.GRFI_COD;
        DBimpianto.gruppoFinalita.descrizione = item.Grfi_Des;
      }

      if (isNumber(item.interbina)) {
        DBimpianto.interbina = item.interbina;
      }
      if (isNumber(item.germinabilita)) {
        DBimpianto.germinabilita = item.germinabilita;
      }
      if (isNumber(item.port_cod)) {
        DBimpianto.portinnesto.codice = item.port_cod;
      }
      if (isNumber(item.foral_cod)) {
        DBimpianto.formaAllevamento.codice = item.foral_cod;
      }
      if (isNumber(item.cop_cod)) {
        DBimpianto.copertura.codice = item.cop_cod;
      }
      if (isNumber(item.GRVA_Cod_VEG)) {
        DBimpianto.gruppoVarietale.codice = item.GRVA_Cod_VEG;
      }
      if (item.dettSpeciePersonalizzatoCod != undefined) {
        DBimpianto.dettaglio_varieta_personalizzato.codice = item.dettSpeciePersonalizzatoCod;
      }

      DBimpianto.data_Inizio_Portinnesto = item.Data_Inizio_Portinnesto ?? DBimpianto.validita.inizio;

      DBimpianto.data_Inizio_Impianto = item.Data_Inizio_Impianto ?? DBimpianto.validita.inizio;

      DBimpianto.data_Inizio_Produzione = item.Data_Inizio_Produzione ?? DBimpianto.validita.inizio;

      let DBEsercizio: Esercizio = DBimpianto.esercizi.find((el) => el.codice == item.Progetto_Cod);
      if (DBEsercizio) {
        if (modificaValidita && fb.controls['Validita_Inizio'].dirty) {
          DBEsercizio.validita.inizio = item.Validita_Inizio;
        }

        if (modificaValidita && fb.controls['Validita_Fine'].dirty) {
          DBEsercizio.validita.fine = item.Validita_Fine;
        }

        //DBEsercizio.regolamento.codice = item.Regolamento_Cod;
        DBEsercizio.regolamento.descrizione = item.Reg_Des;

        DBEsercizio.lotto = item.Progetto_Nome;
        DBEsercizio.descrizione = item.Progetto_Des;
        DBEsercizio.resa_prevista = item.Resa;

        DBEsercizio.prodotto = {codice: item.Mat_Cod, descrizione: item.Mat_Des} as Prodotto;

        DBEsercizio.apportiMassimiMacroelementi.n = item.N;
        DBEsercizio.apportiMassimiMacroelementi.p2o5 = item.P;
        DBEsercizio.apportiMassimiMacroelementi.k2o = item.K;
        DBEsercizio.apportiMassimiMacroelementi.mgo = item.Mg;

        DBEsercizio.apportiMassimiMacroelementi.tipologia.codice = item.Tipologia_Cod;
        DBEsercizio.apportiMassimiMacroelementi.fase.codice = item.Stato_Impianto;

        if (isNumber(item.pianteHa)) {
          DBEsercizio.piante_Ha = item.pianteHa;
        }

        if (isNumber(item.pianteImpianto)) {
          DBEsercizio.piante_Impianto = item.pianteImpianto;
        }

        DBEsercizio.apportiMassimiMacroelementi.tipologia = new FinalitaPianoConcimazione(item.Tipologia_Cod);
        DBEsercizio.piante_Impianto = item.pianteImpianto;
        DBEsercizio.gruppoRaccolta = new BaseCodeDescr(item.GruppoRaccolta_Cod ?? 0, item.GruppoRaccolta_Des);

        DBEsercizio.data_Semina_Trapianto_Prevista = item.DataTrapiantoSemina ?? DBEsercizio.validita.inizio;
        DBEsercizio.data_Fioritura_Prevista = item.DataFioritura ?? DBEsercizio.validita.inizio;
        DBEsercizio.data_Raccolta_Prevista = item.DataRaccolta ?? DBEsercizio.validita.fine;

        if (item.Distinta_Chiusa == 1) {
          DBEsercizio.esercizio_Chiuso = true;
        } else {
          DBEsercizio.esercizio_Chiuso = false;
        }

        if (item.organismo_Referente != '') {
          let orgRef_contatto = new Contatto();
          orgRef_contatto.primaryKey = {codice: item.organismo_Referente, partitaIva: ''};
          DBEsercizio.organismo_Referente = orgRef_contatto;
        }
      }
    }
    this.loadingService.set_isLoading({isLoading: true, message: '', component: this.gridPublicService.gridElRef});
    return this.caricaVincoli(item).pipe(
      switchMap((vincoli: Vincolo[]) => {
        let vincoloSelezionato = vincoli.find((el) => {
          return el.codice == item.RegolamentoDisciplinare_Cod;
        });
        if (vincoloSelezionato != undefined) {
          let impianto = DBAppezza.impianti.find((el) => el.primaryKey.codice == item.id_Reg);
          if (impianto) {
            let DBEsercizio: Esercizio = impianto.esercizi.find((el) => el.codice == item.Progetto_Cod);
            if (DBEsercizio) {
              DBEsercizio.vincolo = vincoloSelezionato;
            }
          }
        }
        return of(DBAppezza);
      }),
      switchMap(() => {
        return this.appezzamentiService.ScriviAppezzamento(DBAppezza);
      }),
      tap((resp) => {
        if (resp.RispostaOK == true) {
          this.giasMessageService.successMessage(this.translocoService.translate('Impianto') + ' ' + DBAppezza.descrizione + ' ' + this.translocoService.translate('Modificato') + '.');
        } else {
          this.giasMessageService.errorMessage(this.translocoService.translate('ErroreSalvataggio') + ' ' + this.translocoService.translate('Impianto') + DBAppezza.descrizione + ': ' + resp.Errore);
        }
      })
    );
  }

  private removeAppezza(
    Piva: string,
    Sa_Cod: number,
    Appezza: number,
    Id_Reg: number,
    Progetto_Cod: Number,
    ribaltato: boolean = false
  ): Observable<any> {

    let objPAgenda: ObjParametriAgenda = new ObjParametriAgenda();
    objPAgenda.Piva = Piva;
    objPAgenda.Sa_Cod = Sa_Cod;
    objPAgenda.Appezza = Appezza;
    this.loadingService.set_isLoading({isLoading: true, message: '', component: this.gridPublicService.gridElRef});

    return this.appezzamentiService.leggiAppezzamentoObs(
      objPAgenda,
      AGRODATAINIZIO, false, true, true, true, true, false
    ).pipe(
      switchMap((resp) => {
        let DBAppezza = resp;
        //Caso 1 appezzamento -> 1 impianto
        if (DBAppezza.impianti.length == 1) {
          DBAppezza.flag_cancellazione = true;
        } else {
          //Caso 1 appezzamento -> n impianti
          let impIndex = DBAppezza.impianti.findIndex((el) => el.primaryKey.codice == Id_Reg);
          if (impIndex > -1) {
            DBAppezza.impianti.splice(impIndex, 1);
          }
        }
        return of(DBAppezza);
      }),
      switchMap(DBAppezza => {
        let dialog: Observable<any> = of(null);
        if (ribaltato && this.isBudget()) {
          let actions = [
            {text: this.translocoService.translate('Si'), primary: true, returnObj: true},
            {text: this.translocoService.translate('No'), returnObj: false}
          ];
          dialog = this.giasDialogService.dialogMessageObs_Result('', this.getImpiantoRibaltatoMsg(DBAppezza), actions);
        }
        return forkJoin([
          of(DBAppezza),
          dialog
        ]);
      }),
      switchMap(resp => {
        return this.appezzamentiService.ScriviAppezzamento_Obs(resp[0], false, resp[1]?.returnObj ?? false);
      }),
      switchMap((resp) => {
        this.addAppezzamentoSelezionato(resp.RispostaStringa, resp.ErroriGias[0]?.messaggio ?? '');
        this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);
        return of([]);
      })
    );
  }

  private getImpiantoRibaltatoMsg(appezza: Appezzamento): string {
    let msg: string = appezza.descrizione;
    if (appezza.impianti[0].utilizzoTerreno.classType == 'Varieta') {
      msg = msg.concat(' '.concat((<Varieta>appezza.impianti[0].utilizzoTerreno).specie.descrizione));
    } else {
      msg = msg.concat(' '.concat((<UtilizzoTerreno>appezza.impianti[0].utilizzoTerreno).descrizione));
    }
    msg = msg.concat(' - '.concat(appezza.impianti[0].utilizzoTerreno.descrizione));
    msg = msg.concat('\n'.concat(this.translocoService.translate('EliminaImpiantoRibaltato')));
    return msg;
  }

  private addAppezzamentoSelezionato(appezzamento: Appezzamento, error: string): void {
    this.appezzamentiSelezionati.set(appezzamento, error);
  }

  private getAppezzamentiSelezionati() {
    return this.appezzamentiSelezionati;
  }

  private clearSelezionati(): void {
    this.appezzamentiSelezionati = new Map<Appezzamento, string>();
  }

  private msgWarningDelete(): void {
    let msgFailure = this.translocoService.translate('ErroreCancellazioneAppezzamenti');
    let msgSuccess = this.translocoService.translate('SuccessoCancellazioneAppezzamenti');
    let displaySuccess = false;
    let displayFailure = false;
    this.getAppezzamentiSelezionati().forEach((error, app) => {
      if (error == '') {
        msgSuccess = msgSuccess.concat('\n' + app['descrizione']);
        displaySuccess = true;
      } else {
        msgFailure = msgFailure.concat('\n' + app['descrizione'] + ': ' + error);
        displayFailure = true;
      }
    });
    if (displaySuccess) {
      this.giasMessageService.successMessage(msgSuccess);
    }
    if (displayFailure) {
      this.giasDialogService.baseError('', msgFailure);
    }
    this.clearSelezionati();
  }

  private handleCommands(): void {
    this.gridPublicService.commandEvent.subscribe(ev => {
      if (!ev) {
        return;
      }
      switch (ev.command.action) {
        case CommandsDropDownEvents.FULL_EDIT:
          this.eserciziEventsService.onTemplateBtnClick(ev.dataItem);
          break;
        case GridEserciziActions.ELENCO_OPERAZIONI:
          this.onListaOperazioni(ev.dataItem);
          break;
        case GridEserciziActions.NUOVA_OPERAZIONE:
          this.onNuovaOperazione(ev.dataItem);
          break;
        case GridEserciziActions.CATASTO:
          this.onCatasto(ev.dataItem);
          break;
        case GridEserciziActions.CHIUSURA:
          this.onCloseEx(ev.dataItem);
          break;
      }
    });
  }

  private onListaOperazioni(dataItem): void {
    this.objParametriAgenda.Sa_Cod = parseInt(dataItem.SA_COD);
    this.objParametriAgenda.SaNome = dataItem.sa_nome;
    this.objParametriAgenda.Appezza = parseInt(dataItem.APPEZZA);
    this.objParametriAgenda.Id_Reg = parseInt(dataItem.id_Reg);
    this.objParametriAgenda.Veg_Cod = parseInt(dataItem.veg_cod);
    this.objParametriAgenda.Id_Cod = parseInt(dataItem.Destinazione_Uso_Cod);

    if (dataItem.Campo_Cod != null && dataItem.Campo_Cod != undefined && isNumber(parseInt(dataItem.Campo_Cod))) {
      this.objParametriAgenda.Campo_Cod = parseInt(dataItem.Campo_Cod);
    } else {
      this.objParametriAgenda.Campo_Cod = 0;
    }

    this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
    this.objParametriAgenda.Data = dataItem.Validita_Inizio;
    this.objParametriAgenda.Pagina_Provenienza = enum_PagineGiasNG.Pagina_Menu_Anagrafica_Impianti;

    if (isNaN(this.objParametriAgenda.Veg_Cod)) {
      this.objParametriAgenda.Veg_Cod = 0;
    }

    if (isNaN(this.objParametriAgenda.Id_Cod)) {
      this.objParametriAgenda.Id_Cod = 0;
    }

    let impianto: ImpiantiAgendaNG = new ImpiantiAgendaNG();
    impianto.Piva = this.objParametriAgenda.Piva;
    impianto.Sa_Cod = this.objParametriAgenda.Sa_Cod;
    impianto.Appezza = this.objParametriAgenda.Appezza;
    impianto.Id_Reg = this.objParametriAgenda.Id_Reg;
    this.objParametriAgenda.Impianti = [impianto];
    this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);
    this.eserciziEventsService.onListaOperazioni();
  }

  private onNuovaOperazione(dataItem): void {
    this.objParametriAgenda.Sa_Cod = parseInt(dataItem.SA_COD);
    this.objParametriAgenda.SaNome = dataItem.sa_nome;
    this.objParametriAgenda.Appezza = parseInt(dataItem.APPEZZA);
    this.objParametriAgenda.Id_Reg = parseInt(dataItem.id_Reg);
    if (dataItem.Campo_Cod != null && dataItem.Campo_Cod != undefined && isNumber(parseInt(dataItem.Campo_Cod))) {
      this.objParametriAgenda.Campo_Cod = parseInt(dataItem.Campo_Cod);
    } else {
      this.objParametriAgenda.Campo_Cod = 0;
    }
    this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
    this.objParametriAgenda.TipoOperazioneAgenda = Tipo_Attivita.QuadernoDiCampagna;
    this.objParametriAgenda.Pagina_Provenienza = enum_PagineGiasNG.Pagina_Menu_Anagrafica_Impianti;

    if (dataItem.veg_cod && +dataItem.veg_cod > 0) {
      this.objParametriAgenda.Veg_Cod = +dataItem.veg_cod;
      this.objParametriAgenda.Veg_Des = dataItem.veg_des;
      this.objParametriAgenda.Id_Cod = 0;
      this.objParametriAgenda.Id_Des = '';

    } else if (dataItem.Destinazione_Uso_Cod && +dataItem.Destinazione_Uso_Cod > 0) {
      this.objParametriAgenda.Id_Cod = +dataItem.Destinazione_Uso_Cod;
      this.objParametriAgenda.Id_Des = dataItem.Destinazione_Uso_Des;
      this.objParametriAgenda.Veg_Cod = 0;
      this.objParametriAgenda.Veg_Des = '';
    }

    let impianto = new ImpiantiAgendaNG();
    impianto.Piva = this.objParametriAgenda.Piva;
    impianto.Sa_Cod = this.objParametriAgenda.Sa_Cod;
    impianto.Appezza = this.objParametriAgenda.Appezza;
    impianto.Progetto_Cod = dataItem.Progetto_Cod;
    impianto.Id_Reg = this.objParametriAgenda.Id_Reg;
    impianto.Sup_Imp = dataItem.sup_imp;

    this.objParametriAgenda.Impianti = [impianto];

    const Validita_Inizio: Date = dataItem.Validita_Inizio;
    const Validita_Fine: Date = dataItem.Validita_Fine;

    let data = new Date();
    if (Validita_Inizio < data && Validita_Fine > data) {
      this.objParametriAgenda.Data = data;
    } else if (Validita_Inizio > data) {
      this.objParametriAgenda.Data = Validita_Inizio;
    } else if (Validita_Fine < data) {
      this.objParametriAgenda.Data = Validita_Fine;
    }

    const dialog: DialogRef = this.dialogService.open({
      title: this.translocoService.translate('SelezionaOperazione'),
      content: NuovaOperazioneComponent,
      width: 400,
      actions: [
        {text: this.translocoService.translate('CreaOperazione'), primary: true},
        {text: this.translocoService.translate('Annulla')}
      ]
    });

    dialog.result.GiasSubscribe((result) => {
      if ((<any>result).primary == true) {
        const NuovaOperazione = this.nuovaOperazioneService.operazioneSelezionataSubject.getValue();
        if (NuovaOperazione != null && NuovaOperazione != undefined && NuovaOperazione.codice != 0) {
          this.objParametriAgenda.Lav_Cod = NuovaOperazione.codice;
          this.objParametriAgenda.Lav_Des = NuovaOperazione.descrizione;
          this.objParametriAgendaService.changeObjParametriAgenda(this.objParametriAgenda);
          this.eserciziEventsService.onNuovaAttivita();
        }
      }
    });
  }

  private onCatasto(dataItem): void {
    const impianto: Impianto = new Impianto({
      codice: dataItem.id_Reg,
      appezzamentoPK: {
        codice: dataItem.APPEZZA,
        centroAziendalePK: {
          codice: dataItem.SA_COD,
          partitaIva: dataItem.PIVA
        }
      }
    });
    this.eserciziEventsService.onCatasto(impianto, dataItem);
  }

  private onCloseEx(dataItem: any): void {
    if (!this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_Impianto, 2))
      return;

    let actions: any[];

    // if exercice is already closed, can't do it again
    if (!!dataItem.Distinta_Chiusa && dataItem.Distinta_Chiusa.toLowerCase().trim() === 'si') {
      actions = [{text: this.translocoService.translate('Ok'), returnObj: false}];
    } else {
      actions = [
        {text: this.translocoService.translate('Conferma'), primary: true, returnObj: true},
        {text: this.translocoService.translate('Annulla'), returnObj: false}
      ];
    }

    const dialog: DialogRef = this.giasDialogService.dialogMessageRef(
      this.translocoService.translate('ChiusuraEsercizio'),
      OnCloseEsercizioComponent,
      actions
    );

    const esercizio: Esercizio = new Esercizio(dataItem.Progetto_Cod, dataItem.app_nome);
    esercizio.impiantoPK = {
      codice: dataItem.id_Reg,
      appezzamentoPK: {
        codice: dataItem.APPEZZA,
        centroAziendalePK: {
          codice: dataItem.SA_COD,
          partitaIva: dataItem.PIVA
        }
      }
    };

    const dialogInstance = dialog.content.instance;
    dialogInstance.dataItem = dataItem;

    dialog.result.pipe().subscribe(resp => {
      if (resp['returnObj']) {
        esercizio.esercizio_Chiuso = dialogInstance.chiusuraForm.controls['chiusura'].value ?? false;
        esercizio.esercizioReplica = dialogInstance.replicaForm.controls['replica'].value ?? false;

        this.callClosePlant(esercizio);
      }
    });
  }

  private callClosePlant(esercizio: Esercizio): void {
    this.loadingService.set_isLoading({isLoading: true, message: '', component: this.gridPublicService.gridElRef});

    this.appezzamentiService.closePlant(esercizio).pipe(
      catchError((a,c) => {
        this.giasMessageService.errorMessage('ErroreDuranteOperazione_', false, true);
        this.loadingService.set_isLoading({isLoading: false, message: '', component: this.gridPublicService.gridElRef});
        return of(null);
      }),
      take(1)
    ).subscribe(r => {
      if (r.RispostaOK) {
        this.giasMessageService.successMessage('SalvataggioAvvenutoConSuccesso', false, true);
        this.refreshGrid(true);
      } else {
        this.giasMessageService.errorMessage('ErroreDuranteOperazione_', false, true);
      }

      this.loadingService.set_isLoading({isLoading: false, message: '', component: this.gridPublicService.gridElRef});
    });
  }

  private isBudget(): boolean {
    return this.budgetService?.isBudget();
  }

  private setGruppoRaccoltaDefault(): void {
    this.gruppiRaccoltaservice.leggiGruppoRaccoltaImpresa(this.objParametriAgendaService.getObjParamValue()).subscribe(
      gr => {
        const fb = this.gridPublicService.formGroup.getValue();
        fb.get('GruppoRaccolta_Cod').setValue(gr.codice);
        fb.get('GruppoRaccolta_Des').setValue(gr.descrizione);
        this.eserciziColumns.find(s => s.field === 'GruppoRaccolta_Cod').ddl.reload.next(true);
      }
    );
  }

  private refreshGrid(forceRefresh: boolean): void {
    this.gridPublicService.refresh(forceRefresh);
  }

  private setSubsForDatiPrevisionaliColture(): void {
    const fb = this.gridPublicService.formGroup.getValue();

    this.datiPrevisionaliSubs.push(fb.controls['SA_COD'].valueChanges.pipe(takeUntil(this.signal)).subscribe(() => this.callUpdateResaPrevista()));
    this.datiPrevisionaliSubs.push(fb.controls['veg_cod'].valueChanges.pipe(takeUntil(this.signal)).subscribe(() => this.callUpdateResaPrevista()));
    this.datiPrevisionaliSubs.push(fb.controls['cul_cod'].valueChanges.pipe(takeUntil(this.signal)).subscribe(() => this.callUpdateResaPrevista()));
    this.datiPrevisionaliSubs.push(fb.controls['GRFI_COD'].valueChanges.pipe(takeUntil(this.signal)).subscribe(() => this.callUpdateResaPrevista()));
    this.datiPrevisionaliSubs.push(fb.controls['GRVA_Cod_VEG'].valueChanges.pipe(takeUntil(this.signal)).subscribe(() => this.callUpdateResaPrevista()));
    this.datiPrevisionaliSubs.push(fb.controls['Validita_Inizio'].valueChanges.pipe(takeUntil(this.signal)).subscribe(() => this.callUpdateResaPrevista()));
    this.datiPrevisionaliSubs.push(fb.controls['Validita_Fine'].valueChanges.pipe(takeUntil(this.signal)).subscribe(() => this.callUpdateResaPrevista()));
    this.datiPrevisionaliSubs.push(fb.controls['port_cod'].valueChanges.pipe(takeUntil(this.signal)).subscribe(() => this.callUpdateResaPrevista()));
    this.datiPrevisionaliSubs.push(fb.controls['foral_cod'].valueChanges.pipe(takeUntil(this.signal)).subscribe(() => this.callUpdateResaPrevista()));
    this.datiPrevisionaliSubs.push(fb.controls['RegolamentoDisciplinare_Cod'].valueChanges.pipe(takeUntil(this.signal)).subscribe(() => this.callUpdateResaPrevista()));
    this.datiPrevisionaliSubs.push(fb.controls['Stato_Impianto'].valueChanges.pipe(takeUntil(this.signal)).subscribe(() => this.callUpdateResaPrevista()));
    this.datiPrevisionaliSubs.push(fb.controls['Destinazione_Uso_Cod'].valueChanges.pipe(takeUntil(this.signal)).subscribe(() => this.callUpdateResaPrevista()));
    this.datiPrevisionaliSubs.push(fb.controls['dettSpeciePersonalizzatoCod'].valueChanges.pipe(takeUntil(this.signal)).subscribe(() => this.callUpdateResaPrevista()));

    this.datiPrevisionaliSubs.push(this.datiPrevisionaliSignal$.pipe(debounceTime(300)).subscribe(() => this.updateResaPrevista(this.gridPublicService.formGroup.getValue())));
  }

  private setSubsForDataInizioProduzione(): void {
    const fb = this.gridPublicService.formGroup.getValue();
    merge(
      fb.get('Stato_Impianto').valueChanges.pipe(takeUntil(this.signal)),
      fb.get('Validita_Inizio').valueChanges.pipe(takeUntil(this.signal))
    ).subscribe(val => {
      if (fb.get('Stato_Impianto').value === this.STATO_INPRODUZIONE) {
        const date: Date = fb.get('Validita_Inizio').value;
        fb.get('Data_Inizio_Produzione').setValue(new Date(date.getFullYear(), 0, 1));
      }
    });
  }

  private callUpdateResaPrevista(): void {
    this.datiPrevisionaliSignal$.next();
  }

  private updateResaPrevista(fb: FormGroup): void {
    let saCod = fb.controls['SA_COD'].value;
    let vegCod = fb.controls['veg_cod'].value;

    if (isNumber(vegCod) && vegCod > 0 && isNumber(saCod) && saCod > 0) {
      this.readIndirizzoCentroAziendale(saCod).pipe(
        take(1),
        switchMap((resp) => {
          let params: DatiPrevisionaliColtureRequest = this.prepareDatiColtureRequestParams(
            this.takeAddress(resp.RispostaStringa)
          );
          return this.impiantiService.readDatiPrevisionaliColture(params);
        }),
        switchMap((resa) => {
          if (resa.RispostaOK) {
            if (resa.RispostaStringa.udm.codice !== 0) {
              fb.controls['Resa'].setValue(this.unitaMisuraService.Converti(<UnitaDiMisura>resa.RispostaStringa.udm, resa.RispostaStringa.valore, new UnitaDiMisura(enum_UnitaMisura.KG__HA, '')) ?? resa.RispostaStringa.valore);
            } else {
              fb.controls['Resa'].setValue(resa.RispostaStringa.valore);
            }
          }
          return of(null);
        }),
        catchError((a,c) => {
          return of(null);
        })
      ).subscribe();
    }
  }

  private handleImpostazioniSuperUser(): void {
    if (this.permessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.SuperUser_KPIN_BlockName)?.Valore == '1') {
      this.eserciziColumns.push(new KendoGridColumn(
        {field: 'cod_kpin', title: this.translocoService.translate('KPIN')},
        {resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 120}
      ));

      this.eserciziColumns.push(new KendoGridColumn(
        {field: 'cod_block', title: this.translocoService.translate('BLOCK')},
        {resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 120}
      ));

      this.eserciziColumns.push(new KendoGridColumn(
        {field: 'cod_grower', title: this.translocoService.translate('GROWER')},
        {resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 120}
      ));
    }
  }

  private handleIsBudget(): void {
    this.eserciziColumns.push(
      new KendoGridColumn(
        {field: 'ribaltatoDes', title: this.translocoService.translate('Ribaltato')},
        {resizable: true, editable: false, width: 165}
      )
    );
    this.eserciziColumns.push(
      new KendoGridColumn(
        {field: 'Data_Ribaltamento', title: this.translocoService.translate('DataRibaltamento')},
        {resizable: true, editable: false, width: 165}
      )
    );
  }

  private handleUserSettings() {
    if (this.permessiUtenteService.getPermesso(enum_Security_Attivita.GestioneAssociazioneAppezzamentiXParcoMacchine, enum_TipoPermesso.LETTURA)) {
      this.eserciziColumns.push(
        new KendoGridColumn(
          {field: 'LinkedMachines', title: this.translocoService.translate('LinkedWaterCounters')},
          {resizable: true, editable: false, width: 165}
        )
      );
    }
  }

  private onGridEditAction(event: any) {
    let objPAgenda: ObjParametriAgenda = new ObjParametriAgenda();
    objPAgenda.Piva = event.dataItem.PIVA;
    objPAgenda.Sa_Cod = parseInt(event.dataItem.SA_COD);
    objPAgenda.Appezza = parseInt(event.dataItem.APPEZZA);

    const fb = this.gridPublicService.formGroup.getValue();
    fb.controls['SA_COD'].disable({emitEvent: false});
    fb.controls['Campo_Des'].disable({emitEvent: false});

    this.setSubsForDataInizioProduzione();

    fb.controls['sup_imp'].valueChanges.pipe(
      skip(1),
      auditTime(1000),
      scan((a, c) => this.onEditSupImp(a, c), undefined)
    ).subscribe(val => {});

    return forkJoin([
      of(event),
      this.appezzamentiService.leggiAppezzamento(objPAgenda,
        AGRODATAINIZIO,
        false,
        true,
        true,
        true,
        true,
        false,
        false
      )
    ]);
  }

  private onGridAddAction(event: any) {
    const fb = this.gridPublicService.formGroup.getValue();
    if (this.permessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.SUPERUSER_COD_ORG_REFERENTE_DA_PADRE)?.Valore == '1') {
      from(this.impreseService.leggiImpresa(this.objParametriAgendaService.getObjParamValue()))
        .pipe(
          take(1),
          map(impresa => {
            let padriImpresa = impresa.RispostaStringa.impresaPadre;
            if (padriImpresa.length == 1) {
              let azienda: ImpresaPadre = padriImpresa[0];
              let col;
              col = this.eserciziColumns.find(s => s.field === 'organismo_Referente');
              fb.controls['organismo_Referente'].patchValue(azienda.partitaIva);
              fb.controls['organismo_Referente_des'].patchValue(azienda.ragioneSociale);
              col.ddl.reload.next(true);
            }
          })
        ).subscribe();
    }

    fb.controls['dettSpeciePersonalizzatoCod'].setValue('0');

    this.setCodiceImpianto();
    this.setGruppoRaccoltaDefault();
    this.setSubsForDatiPrevisionaliColture();
    this.setSubsForDataInizioProduzione();
    this.setValidityDefault();

    return of(null);
  }

  private onEditSupImp(previousNotification: NotificationRef, value: number): NotificationRef {
    const superficieGis: number = Number.parseFloat(this.currentEditAppezza?.getValue()?.superficieGis.toFixed(4));

    if (!!superficieGis && superficieGis !== 0) {
      const diff: number = Number.parseFloat((value - superficieGis).toFixed(4));
      const percentage: number = Number.parseFloat(((diff / superficieGis) * 100).toFixed(4));

      const prefix: string = `${this.transloco.translate('SuperficiePoligonoAssociato')} ${superficieGis} [Ha],  `;
      const diffStr: string = `${this.transloco.translate('Differenza')}: ${diff > 0 ? '+' : ''}${diff} [Ha],  `;
      const percStr: string = `${this.transloco.translate('DifferenzaPercentuale')}: ${diff > 0 ? '+' : ''}${percentage} %`;

      if (diff >= 0.0001) {
        previousNotification?.hide();
        return this.giasMessageService.warningMessage(prefix + diffStr + percStr, false, false, undefined, 10000);
      }
    }
  }

  private setCodiceImpianto(): void {
    const fb = this.gridPublicService.formGroup.getValue();

    this.appezzamentiService.getCodiceImpianto(
      this.objParametriAgenda.Piva,
      new Date().getFullYear()
    ).pipe(
      take(1)
    ).subscribe(r => {
      fb.controls['Codice_Impianto'].setValue(r.codiceImpianto);
      fb.controls['algoritmoCodifica'].setValue(r.algoritmoCodifica);

      if (r.codiceImpianto !== '' && r.algoritmoCodifica !== '') {
        fb.controls['Codice_Impianto'].disable();
      }
    });
  }

  private setValidityDefault(): void {
    let fb: FormGroup = this.gridPublicService.formGroup.getValue();
    const settings: IntervalloTemporale = this.extractValidityFromSetting();

    fb.controls['Validita_Inizio'].setValue(settings?.inizio ?? new Date());
    fb.controls['Validita_Fine'].setValue(settings?.fine ?? AGRODATAFINE);
    fb.controls['Validita_Inizio_Impianto'].setValue(settings?.inizio ?? new Date());

    fb.controls['Validita_Inizio'].markAsTouched();
    fb.controls['Validita_Fine'].markAsTouched();
    fb.controls['Validita_Inizio_Impianto'].markAsTouched();

    fb.controls['Validita_Inizio'].markAsDirty();
    fb.controls['Validita_Fine'].markAsDirty();
    fb.controls['Validita_Inizio_Impianto'].markAsDirty();
  }

  private extractValidityFromSetting(): IntervalloTemporale {
    // questo codice è ripetuto nel file appezzamento-global-edit.component, appena si ha tempo correggere
    const setting: Utente_Impostazioni = this.permessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.UTENTE_InizioFineAnnataAgraria);

    // default values, if no setting is found
    let endYear: number = new Date().getFullYear();
    let endMonth: number = 11;
    let endDay: number = 31;
    let startYear: number = new Date().getFullYear();
    let startMonth: number = 0;
    let startDay: number = 1;

    if (setting != undefined) {
      let startSet: string = setting.Valore.trim().slice(0, 4);
      let endSet: string = setting.Valore.trim().slice(4);

      startDay = Number.parseInt(startSet.slice(0, 2));
      startMonth = Number.parseInt(startSet.slice(2)) - 1;

      endDay = Number.parseInt(endSet.slice(0, 2));
      endMonth = Number.parseInt(endSet.slice(2)) - 1;

      const today: Date = new Date();
      if (startMonth < today.getMonth() || (today.getMonth() === startMonth && startDay < today.getDate())) {
        startYear = today.getFullYear();
      } else {
        startYear = today.getFullYear() - 1;
      }

      // controllo se la differenza è minore di 0, ossia l'annata agrazia inizia e finisce nello stesso anno solare
      if (endMonth > startMonth) {
        endYear = startYear;
      } else {
        endYear = startYear + 1;
      }
    }

    const start: Date = new Date(startYear, startMonth, startDay);
    const end: Date = new Date(endYear, endMonth, endDay);
    return new IntervalloTemporale(start, end);
  }
}
