import {Injectable, Injector} from '@angular/core';
import {
  CommandsColumnSettings,
  DettagliColumnSettings,
  GroupSettings,
  ToolbarSettings
} from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import {
  DropdownListWithForm,
  EditingMode,
  GridCustomizations,
  KendoGridColumn,
  KendoGridModel,
  KendoGridRow,
  KendoServerResult,
  LoaderType,
  ModelEntry
} from 'gias-kendo-grid';
import {ConfigTemplate} from 'gias-kendo-grid';
import {AbstractGridConfigService, HttpAction} from 'gias-kendo-grid';
import {GridDataWithFilter, GridPublicService} from 'gias-kendo-grid';
import {from, map, Observable, of, takeUntil} from 'rxjs';
import {QdCService, Redirect_To_GiasNG_Page} from '../qdc.service';
import {TranslocoService} from '@jsverse/transloco';
import {MisceleService} from "../miscele.service";
import {
    enum_ErroreGias_Tipo,
    ErroreGias,
    ErroreGias_Severity, rispostaStandard,
    RispostaStandard
} from "../../../../Service/master.service";
import {Lavorazione} from "../../../../Model/attivita/Lavorazione";
import {FormGroup} from "@angular/forms";
import {cloneDeep} from "lodash";
import {Enum_SiteRedirector} from "../../../../Model/siti.enum";
import {Enum_DBTypeOperation} from 'gias-ui-kit';
import {enum_PagineGiasNG} from "../../../../Model/TipiEnumerativi";
import {GridMacchinaModel} from "../../quaderno-di-campagna-form/quaderno-di-campagna-form.model";
import {ObjParametriAgendaService} from "../../../../Service/obj-parametri-agenda.service";
import {GestioneRichiesteService} from "../../../../Service/gestione-richieste.service";
import {LeggiProfilazione, ProfilazioneService} from "../../../../Service/Agenda/profilazione.service";
import {GiasMessageService} from "../../../../Service/gias-message.service";

export class GridMacchineServerResult extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

@Injectable()

export class GridMacchineHttpService extends AbstractGridConfigService<GridMacchineServerResult>{
    gridId = 'GridMacchine';
    rowId = 'Risorsa_Cod';

    RisorseMacchine: Array<GridMacchinaModel> = [];

    GridMacchineServerResult: GridMacchineServerResult;

    GridMacchine: any= {};

    loader: LoaderType = LoaderType.SERVICE;
    editingMode: EditingMode = EditingMode.IN_LINE;

    views = new GridCustomizations({enabled: false});

    constructor(injector: Injector,
                public gridpublicService: GridPublicService,
                public qdcservice: QdCService,
                private translocoService: TranslocoService,
                private misceleservice: MisceleService,
                private objParametriAgendaService: ObjParametriAgendaService,
                private gestionerichiesteservice: GestioneRichiesteService,
                private profilazioneservice: ProfilazioneService,
                private messageservice: GiasMessageService) {

        super(injector, ConfigTemplate.DefaultTemplate);

        this.dettagliColumn = new DettagliColumnSettings({
          editBtn: true,
          edit: this.Modifica_Anagrafica_Macchina.bind(this),
          title: "ModificaAnagraficaMacchina",
          width: 200
        });

        // Nascondo la colonna Azioni con i bottoni di Info,Modifica e Cancella
        this.cmdColumn = new CommandsColumnSettings({
            editBtn: false,
            infoBtn: false,
            removeBtn: this.qdcservice.abilitaGrid,
            onDisableInfoBtn: () => false,
            showEditBtn: this.qdcservice.Controlla_Permesso_Modifica_Macchina,
            width: 30
        });

        this.toolbar = new ToolbarSettings(
            this.qdcservice.abilitaGrid,
            false,
            'Azioni',
            150,
            '',
            false
        );

        this.resizable.autoFitColumns=false;

        this.resizable.isResizable=true;

        this.selectable.shouldShowCheckbox = false;

        this.columnMenu.kendoGridColumnChooser=true;

        this.behavior.excelSettings.enabled=false;

        this.behavior.pdfSettings.enabled=false;

        this.behavior.showDeletionConfirmation = true;

        this.generalSettings.performOnEdit=false;

        this.generalSettings.height = 'auto';

        this.pagination.pageable = false;

        this.groups = new GroupSettings({groupable: {enabled: false, showFooter: false}}, this.translocoService);

        this.gridPublicService.formGroup.pipe(takeUntil(this.signal)).subscribe( (formGroup: FormGroup)=>{
          if(formGroup) {
            formGroup.get("Risorsa_Cod").valueChanges.pipe(takeUntil(this.signal)).subscribe((newRisorsa_Cod: string)=>{
              //Faccio questo perchè così al cambio delle'elemneto della ddl mi memorizzo
              //nella griglia anche tutti i suoi dati collegati non sonolo il codice e la decsrizione della ddl.
              let macchina = this.RisorseMacchine.find(m=> m.Risorsa_Cod == newRisorsa_Cod);

              if (macchina !== undefined && macchina !== null){

                formGroup.patchValue({
                    Piva: macchina.Piva,
                    Mac_Cod: macchina.Mac_Cod,
                    Taratura_Ugello:  macchina.Taratura_Ugello,
                    Modello:  macchina.Modello,
                    Marca: macchina.Marca,
                    Data_Scadenza_Taratura: macchina.Data_Scadenza_Taratura,
                    Mac_Des: macchina.Mac_Des,
                    Tipo: macchina.Tipo,
                    Dettaglio_1: macchina.Dettaglio_1,
                    Dettaglio_2: macchina.Dettaglio_2,
                    Risorsa_Des: macchina.Risorsa_Des,
                    Validita: macchina.Validita
                });
              }
            });
          }
        });

    }

    perform(actionType: HttpAction, item: any,oldRow:KendoGridRow): Observable<any> {

        let Obs: Observable<any> = null;

        let listerroriGias: ErroreGias[] = [];

        let Operazioni: Array<Lavorazione> = this.qdcservice.TestataForm.get("Operazioni").getRawValue();

        if(!Operazioni || Operazioni.length === 0){
            listerroriGias.push(<ErroreGias>{
                severity: ErroreGias_Severity.Bloccante,
                tipo: enum_ErroreGias_Tipo.Generico,
                messaggio: this.translocoService.translate('SelezionareAlmenoUnOperazione')
            });

            this.qdcservice.gestisci_ErroriGias(listerroriGias,true,true).then();

        }else{
            if(actionType === HttpAction.UPDATE || actionType === HttpAction.CREATE) {

                let riga_doppia = this.qdcservice.Controlla_Righe_Doppie(this.gridPublicService.getValue().data.rows,
                    item,actionType,oldRow);



                if(riga_doppia !== undefined && riga_doppia !== null){

                    let message: string = this.translocoService.translate('NonEConsentitoInserireUnelementogiapresente', { NomeElemento: riga_doppia.Risorsa_Des});

                    listerroriGias.push(<ErroreGias>{
                        severity: ErroreGias_Severity.Bloccante,
                        tipo: enum_ErroreGias_Tipo.Generico,
                        messaggio: message
                    });

                    this.qdcservice.gestisci_ErroriGias(listerroriGias,true,true).then();
                }

            }
        }

        if(listerroriGias.length === 0){

          let rows = this.qdcservice.MacchineFormArray.getRawValue();

          switch(actionType){
            case HttpAction.CREATE:
              rows.push(item);
              break;
            case HttpAction.UPDATE:
              let index = rows.findIndex(r=>r.Risorsa_Cod === oldRow['Risorsa_Cod']);
              if(index > -1){
                rows[index] = item;
              }
              break;
            case HttpAction.REMOVE:
              rows = rows.filter(r=>r.Risorsa_Cod !== item['Risorsa_Cod']);
              break;
          }

          this.qdcservice.AggiornaFormArrayMacchine(rows);

          //Imposto in automatico l'acqua dalla taratura ugello della macchina se cambia il default della macchina o ne
          //viene inserita una nuova (non se viene cancellata)
          this.qdcservice.Imposta_VolumiAcqua(0,null,"").then(risp=>{
            //Triggero il ricalcolo dell'acqua
            if(risp){
              this.misceleservice.calcoloMiscele('changeAcqua_Ha',null,0);
            }
          });

          Obs = of([]);

        }else{
            Obs = of(false);
        }

        return Obs ;
    }

    read(): Observable<GridMacchineServerResult> {

        this.loadingService.set_isLoading({isLoading: true, message: '', component: this.gridPublicService.gridElRef});

        let rows = this.qdcservice.MacchineFormArray.getRawValue();

        return of(this.setGridMacchineServerResult(rows));

    }

    setGridMacchineServerResult(rows): GridMacchineServerResult{

        this.GridMacchine.kendo_rows = <KendoGridRow[]>(rows);

        this.GridMacchine.kendo_columns = this.setColumnsGridMacchine();

        this.GridMacchine.kendo_model = this.setModelGridMacchine();

        this.GridMacchineServerResult = new GridMacchineServerResult (this.GridMacchine.kendo_rows,
                                                                        this.GridMacchine.kendo_columns,
                                                                        this.GridMacchine.kendo_model);

        this.loadingService.set_isLoading({isLoading: false, message: '', component: this.gridPublicService.gridElRef});

        return this.GridMacchineServerResult;
    }

    setColumnsGridMacchine(){

        //Imposto le Dropdown della Griglia delle Macchine
        let ddl_Risorsa = new DropdownListWithForm('Risorsa_Cod', 'Risorsa_Cod', 'Risorsa_Des', []);

        ddl_Risorsa.valuePrimitive = true;

        ddl_Risorsa.descriptionField = 'Risorsa_Des';

        ddl_Risorsa.loadOnEdit = true;

        ddl_Risorsa.loadFunction = this.CaricaddlRisorsaMacchine.bind(this);

        let columns: Array<KendoGridColumn>=[
            new KendoGridColumn({field: 'Risorsa_Cod',title: this.translocoService.translate('Risorsa')},{resizable:true,editable: this.qdcservice.abilitaGrid,ddl: ddl_Risorsa})
        ];


        return columns;
    }

    setModelGridMacchine(){

        const model: KendoGridModel={
            Risorsa_Cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST),
            Risorsa_Des: new ModelEntry(CELL_TYPES.STRING),
            Piva: new ModelEntry(CELL_TYPES.STRING),
            Mac_Cod: new ModelEntry(CELL_TYPES.NUMBER),
            Taratura_Ugello: new ModelEntry(CELL_TYPES.NUMBER),
            Modello: new ModelEntry(CELL_TYPES.STRING),
            Marca: new ModelEntry(CELL_TYPES.DROPDOWNLIST),
            Data_Scadenza_Taratura: new ModelEntry(CELL_TYPES.DATE),
            Mac_Des: new ModelEntry(CELL_TYPES.STRING),
            Tipo: new ModelEntry(CELL_TYPES.DROPDOWNLIST),
            Dettaglio_1: new ModelEntry(CELL_TYPES.DROPDOWNLIST),
            Dettaglio_2: new ModelEntry(CELL_TYPES.DROPDOWNLIST),
            Validita: new ModelEntry(CELL_TYPES.DROPDOWNLIST)
        };



        return model;
    }

    Modifica_Anagrafica_Macchina(data: any, isNew: boolean, rowIndex: number){
      if(this.qdcservice.Controlla_Permesso_Modifica_Macchina(data)){

        this.gridpublicService.giasGridComponent.disableButtonsDuringEditMode(false);

        this.gridpublicService.gridComp.closeRow(rowIndex);

        this.ApriAnagraficaMacchina(data,false);
      }
    }

    public ApriAnagraficaMacchina(data: any,isNew:boolean){
        //Redirect all'Anagrafica della Macchina per
        //le operazioni di nuovo e modifica

        let newObjPageToMemorize =  new Redirect_To_GiasNG_Page();

        let objParametriAgenda = cloneDeep(this.objParametriAgendaService.getObjParamValue());

        newObjPageToMemorize.objParametriAgenda = cloneDeep(objParametriAgenda);

        this.gestionerichiesteservice.ObjPageToMemorize = newObjPageToMemorize;

        let title = "";

        objParametriAgenda.Pagina_Provenienza = this.qdcservice.masterService.getCurrentPageAsValue();

        objParametriAgenda.Sito_Provenienza = Enum_SiteRedirector.GiasNG;

        if(isNew){
            objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
            title = "NuovaMacchina";
        }else{
            objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Update;
            objParametriAgenda.Mac_Cod = + data.Mac_Cod;
            objParametriAgenda.Sa_Cod = + data.Sa_Cod;
            objParametriAgenda.Piva = data.Piva;
            title = "ModificaMacchina";
        }

        this.objParametriAgendaService.changeObjParametriAgenda(objParametriAgenda);

        this.gestionerichiesteservice.gestionePassaggioStessoSito_Aperto_in_Iframe(enum_PagineGiasNG.Pagina_Edit_Macchina,
            [],-1,title).then(GiasIFrameWindowService=>{
            GiasIFrameWindowService.window.window.onDestroy(() => {

                this.objParametriAgendaService.changeObjParametriAgenda(this.gestionerichiesteservice.ObjPageToMemorize.objParametriAgenda);

                this.gestionerichiesteservice.ObjPageToMemorize = null;

                //Aggiorno i dati della griglia con quelli modificati dall'anagrafica
                if(!isNew && data){
                    this.CaricaddlRisorsaMacchine(this).subscribe(risorseMacchine=>{

                        let griData: GridDataWithFilter = cloneDeep(this.qdcservice.GridMacchinePublicService.getValue());

                        let index = griData.data.rows.findIndex(r=>r["Mac_Cod"]=== data.Mac_Cod && r["Piva"]=== data.Piva);

                        if(index > -1){
                            griData.data.rows[index] = risorseMacchine.find(r=>r.Mac_Cod === data.Mac_Cod && r.Piva=== data.Piva);

                            this.qdcservice.AggiornaFormArrayMacchine(griData.data.rows);

                            this.qdcservice.GridMacchinePublicService.refresh(true,griData);
                        }
                    });
                }
            });
        });
    }



    public SalvaDefaultMacchine(){

        if(this.qdcservice.GridMacchinePublicService && this.qdcservice.GridMacchinePublicService.giasGridComponent.inLineModificaIsDisabled){
            return;
        }

        let ArrayDefaultOperatori = this.qdcservice.getDefaulOperatorifromGrid();

        let ArrayDefaultMacchine = this.qdcservice.getDefaultMacchinefromGrid();

        const LeggiProfilazione:LeggiProfilazione = {
            impresa: this.qdcservice.getImpresa_Model(),
            operazioni: this.qdcservice.TestataForm.get('Operazioni').getRawValue(),
            specie: this.qdcservice.TestataForm.get('Specie').value,
            data: this.qdcservice.TestataForm.get('Data').value,
            macchine: ArrayDefaultMacchine,
            contatti: ArrayDefaultOperatori
        };

        const Promise_Risp = this.profilazioneservice.Scrivi_Profilazione_Dati_QdC(LeggiProfilazione);

        Promise_Risp.then((risp:RispostaStandard) => {

            if(risp.RispostaOK){
                this.qdcservice.RicaricaGridMacchine().then();
                this.messageservice.successMessage(this.translocoService.translate('ProfiloMemorizzatoCorrettamente'),false);
            }

        });


    }

    public CaricaddlRisorsaMacchine(dataItem:any): Observable<Array<GridMacchinaModel>> {

        const leggiProfilazione = <LeggiProfilazione>{
            impresa: this.qdcservice.getImpresa_Model(),
            data: this.qdcservice.TestataForm.get('Data').value,
            operazioni: this.qdcservice.TestataForm.get('Operazioni').value
        };


        return this.profilazioneservice.CaricaRisorseMacchine_QdC(leggiProfilazione).pipe(map((risp: rispostaStandard<Array<GridMacchinaModel>>)=>{

            //Escludo tra gli elementi della DDL le macchine che sono già state scelte
            /*let rows:any = this.gridpublicService.getValue().data.rows;

            rows.forEach(r => {
                let index = macchine.findIndex(m=> m.Mac_Cod === r.Mac_Cod && dataItem.Mac_Cod !== m.Mac_Cod);

                if(index > -1)
                    macchine.splice(index, 1);
            });*/

            this.RisorseMacchine = risp.RispostaStringa;
            return this.RisorseMacchine;
        }));

    }

}
