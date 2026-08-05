import { Injectable, Injector } from '@angular/core';
import {
  CommandsColumnSettings, DettagliColumnSettings,
  GroupSettings,
  ToolbarSettings
} from 'gias-kendo-grid';
import { EditingMode, KendoGridColumn,GridCustomizations, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, ModelEntry, DropdownListWithForm } from 'gias-kendo-grid';
import { ConfigTemplate } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import {GridDataWithFilter, GridPublicService} from 'gias-kendo-grid';
import { from, Observable, of, takeUntil } from 'rxjs';
import { QdCService } from '../qdc.service';
import { LeggiProfilazione, ProfilazioneService } from 'app/Service/Agenda/profilazione.service';
import { TranslocoService } from '@jsverse/transloco';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { GridOperatoreModel } from '../../quaderno-di-campagna-form/quaderno-di-campagna-form.model';
import {
    enum_ErroreGias_Tipo,
    ErroreGias,
    ErroreGias_Severity,
    RispostaStandard
} from "../../../../Service/master.service";
import {Lavorazione} from "../../../../Model/attivita/Lavorazione";
import {FormGroup} from "@angular/forms";
import {cloneDeep} from "lodash";
import {Enum_DBTypeOperation} from 'gias-ui-kit';
import {enum_PagineAgenda_2010, Enum_SiteRedirector} from "../../../../Model/siti.enum";
import {
    GestioneRichiesteService,
    ParametriAggiuntivi_QueryString
} from "../../../../Service/gestione-richieste.service";
import {COD_LEGALE} from "../../../../Model/CostantiPersonalizzate";
import {GiasMessageService} from "../../../../Service/gias-message.service";
import { CELL_TYPES } from 'gias-ui-kit';

export class GridOperatoriServerResult extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

@Injectable()

export class GridOperatoriHttpService extends AbstractGridConfigService<GridOperatoriServerResult>{
    gridId = 'GridOperatori';
    rowId = 'Risorsa_Cod';

    GridOperatoriServerResult: GridOperatoriServerResult;

    GridOperatori: any= {};

    RisorseOperatori: Array<any>;

    loader: LoaderType = LoaderType.SERVICE;
    editingMode: EditingMode = EditingMode.IN_LINE;

    views = new GridCustomizations({enabled: false});

    constructor(injector: Injector,
                public gridpublicService: GridPublicService,
                public qdcservice: QdCService,
                private profilazioneservice: ProfilazioneService,
                private translocoService: TranslocoService,
                private objParametriAgendaService: ObjParametriAgendaService,
                private messageservice: GiasMessageService,
                private gestioneRichiesteService: GestioneRichiesteService) {

        super(injector, ConfigTemplate.DefaultTemplate);

        this.dettagliColumn = new DettagliColumnSettings({
          editBtn: true,
          edit: this.Modifica_Anagrafica_Operatore.bind(this),
          title: "ModificaAnagraficaOperatore",
          width: 200
        });

        // Nascondo la colonna Azioni con i bottoni di Info,Modifica e Cancella
        this.cmdColumn = new CommandsColumnSettings({
            editBtn: false,
            infoBtn: false,
            removeBtn: this.qdcservice.abilitaGrid,
            onDisableInfoBtn: () => false,
            showEditBtn: this.qdcservice.Controlla_Permesso_Modifica_Operatore,
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

        this.generalSettings.performOnEdit = false;

        this.generalSettings.height = 'auto';

        this.pagination.pageable = false;

        this.groups = new GroupSettings({groupable: {enabled: false, showFooter: false}}, this.translocoService);

      this.gridPublicService.formGroup.pipe(takeUntil(this.signal)).subscribe( (formGroup: FormGroup)=>{
        if(formGroup) {
          formGroup.get("Risorsa_Cod").valueChanges.pipe(takeUntil(this.signal)).subscribe((newRisorsa_Cod: string)=>{
            //Faccio questo perchè così al cambio delle'elemneto della ddl mi memorizzo
            //nella griglia anche tutti i suoi dati collegati non sonolo il codice e la decsrizione della ddl.
            let operatore = this.RisorseOperatori.find(m=> m.Risorsa_Cod == newRisorsa_Cod);

            if (operatore !== undefined && operatore !== null && operatore !== ""){

              formGroup.patchValue({
                Piva:  operatore.Piva,
                Cod_Contatto:  operatore.Cod_Contatto,
                Cod_RisUm:  operatore.Cod_RisUm,
                Cod_Rapporto: operatore.Cod_Rapporto,
                Rapporto_Des: operatore.Rapporto_Des,
                Nome: operatore.Nome,
                Cognome: operatore.Cognome,
                Data_Scadenza_Patentino: operatore.Data_Scadenza_Patentino,
                Risorsa_Des: operatore.Risorsa_Des
              });
            }
          });
        }
      });


    }

    perform(actionType: HttpAction, item: any, oldRow: KendoGridRow): Observable<any[]> {

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
          let rows = this.qdcservice.OperatoriFormArray.getRawValue();

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

          this.qdcservice.AggiornaFormArrayOperatori(rows);

          Obs = of([]);
        }else{
          Obs = of(false);
        }

        return Obs;
    }

    read(): Observable<GridOperatoriServerResult> {

        this.loadingService.set_isLoading({isLoading: true, message: '', component: this.gridPublicService.gridElRef});

        let  rows =  this.qdcservice.OperatoriFormArray.getRawValue();

        return of(this.setGridOperatoriServerResult(rows));
    }

    setGridOperatoriServerResult(rows): GridOperatoriServerResult{

        this.GridOperatori.kendo_rows = <KendoGridRow[]>(rows);

        this.GridOperatori.kendo_columns = this.setColumnsGridOperatori();

        this.GridOperatori.kendo_model = this.setModelGridOperatori();

        this.GridOperatoriServerResult = new GridOperatoriServerResult (
            this.GridOperatori.kendo_rows,
            this.GridOperatori.kendo_columns,
            this.GridOperatori.kendo_model
        );

        this.loadingService.set_isLoading({isLoading: false, message: '', component: this.gridPublicService.gridElRef});

        return this.GridOperatoriServerResult;
    }

    setColumnsGridOperatori(){

        //Imposto le Dropdown della Griglia delle Macchine
        let ddl_Risorsa = new DropdownListWithForm('Risorsa_Cod', 'Risorsa_Cod', 'Risorsa_Des', []);

        ddl_Risorsa.valuePrimitive = true;

        ddl_Risorsa.descriptionField = 'Risorsa_Des';

        ddl_Risorsa.loadOnEdit = true;

        ddl_Risorsa.loadFunction = this.CaricaddlRisorsaOperatori.bind(this);

        let columns: Array<KendoGridColumn>=[
            new KendoGridColumn({field: 'Risorsa_Cod',title: this.translocoService.translate('Risorsa')},{resizable:true, editable: this.qdcservice.abilitaGrid,ddl: ddl_Risorsa})
        ];


        return columns;
    }

    setModelGridOperatori(){

        const model: KendoGridModel={
            Piva: new ModelEntry(CELL_TYPES.STRING),
            Cod_Contatto: new ModelEntry(CELL_TYPES.STRING),
            Cod_RisUm: new ModelEntry(CELL_TYPES.NUMBER),
            Risorsa_Cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST),
            Risorsa_Des: new ModelEntry(CELL_TYPES.STRING),
            Cod_Rapporto: new ModelEntry(CELL_TYPES.NUMBER),
            Rapporto_Des: new ModelEntry(CELL_TYPES.STRING),
            Nome: new ModelEntry(CELL_TYPES.STRING),
            Cognome: new ModelEntry(CELL_TYPES.STRING),
            Data_Scadenza_Patentino: new ModelEntry(CELL_TYPES.DATE),
        };

        return model;
    }

    CaricaddlRisorsaOperatori(dataItem: any) {

        const LeggiProfilazione = <LeggiProfilazione>{
            impresa: this.qdcservice.getImpresa_Model(),
            data: this.qdcservice.TestataForm.get('Data').value
        };

        return from(this.profilazioneservice.CaricaRisorseOperatori_QdC(LeggiProfilazione).then((operatori: Array<GridOperatoreModel>)=>{

            //Escludo tra gli elementi della DDL gli operatori che sono già stati scelti
            /*let rows:any = this.gridpublicService.getValue().data.rows;

            rows.forEach(r => {
                let index = operatori.findIndex(o=> o.Cod_RisUm === r.Cod_RisUm && o.Cod_Contatto === r.Cod_Contatto  &&
                                                    dataItem.Cod_RisUm !== o.Cod_RisUm && dataItem.Cod_Contatto !== o.Cod_Contatto);

                if(index > -1)
                    operatori.splice(index, 1);
            });*/

            this.RisorseOperatori = operatori;
            return this.RisorseOperatori;
        }));

    }

    Modifica_Anagrafica_Operatore(data: any, isNew: boolean, rowIndex: number){
      if(this.qdcservice.Controlla_Permesso_Modifica_Operatore(data)){

        this.gridpublicService.giasGridComponent.disableButtonsDuringEditMode(false);

        this.gridpublicService.gridComp.closeRow(rowIndex);

        this.ApriAnagraficaContatto(data,false);
      }
    }

    ApriAnagraficaContatto(data: any, isNew: boolean){
        //Redirect all'Anagrafica della Contatto per
        //le operazioni di nuovo e modifica

        let objParametriAgenda = cloneDeep(this.objParametriAgendaService.getObjParamValue());

        let title = "";

        if(isNew){
            objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
            title = "NuovoOperatore";
        }else{
            objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Update;
            objParametriAgenda.Cod_Contatto = data.Cod_Contatto;
            objParametriAgenda.Piva = data.Piva;
            title = "ModificaOperatore";
        }

        objParametriAgenda.Pagina_Provenienza = this.qdcservice.masterService.getCurrentPageAsValue();
        objParametriAgenda.Sito_Provenienza = Enum_SiteRedirector.GiasNG;

        //Lav_Cod lo passo a 0 perchè viene in realtà considerato solo per le operazioni contabili non di campagna
        let ParametriAggiuntivi: Array<ParametriAggiuntivi_QueryString> =
            [
                {
                    key:"dialog",
                    value: "true",
                    codifica: true
                },
                {
                    key:"tipo_rapporto",
                    value: COD_LEGALE.toString(),
                    codifica: true
                },
                {
                    key:"lav_cod",
                    value: "0",
                    codifica: true
                },
                {
                    key:"orig",
                    value:"",
                    codifica: true
                },
                {
                    key:"apertodaGiasNG",
                    value:"true",
                    codifica: true
                }
            ];

        this.gestioneRichiesteService.gestionePassaggioAltroSito_Aperto_in_Iframe(
            Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
            enum_PagineAgenda_2010.Pagina_Anagrafica_Contatto_New,
            ParametriAggiuntivi,
            objParametriAgenda,
            true,
            -1,
            title,
            false).then(GiasIFrameWindowService => {
            GiasIFrameWindowService.window.window.onDestroy(() => {
                //Aggiorno i dati della griglia con quelli modificati dall'anagrafica
                if(!isNew && data){
                    this.CaricaddlRisorsaOperatori(this).subscribe(risorseOperatori=>{

                        let griData: GridDataWithFilter = cloneDeep(this.qdcservice.GridOperatoriPublicService.getValue());

                        let index = griData.data.rows.findIndex(r=>r["Cod_Contatto"]=== data.Cod_Contatto && r["Piva"]=== data.Piva);

                        if(index > -1){
                            griData.data.rows[index] = risorseOperatori.find(r=>r.Cod_Contatto === data.Cod_Contatto  && r.Piva=== data.Piva);

                            this.qdcservice.AggiornaFormArrayOperatori(griData.data.rows);

                            this.qdcservice.GridOperatoriPublicService.refresh(true,griData);
                        }
                    });
                }
            });
        });

    }

    SalvaDefaultOperatori(){

        if(this.qdcservice.GridOperatoriPublicService && this.qdcservice.GridOperatoriPublicService.giasGridComponent.inLineModificaIsDisabled){
            return;
        }

        let ArrayDefaultOperatori = this.qdcservice.getDefaulOperatorifromGrid();

        let ArrayDefaultMacchine = this.qdcservice.getDefaultMacchinefromGrid();

        const LeggiProfilazione:LeggiProfilazione = {
            impresa: this.qdcservice.getImpresa_Model(),
            operazioni: this.qdcservice.TestataForm.get('Operazioni').getRawValue(),
            specie:  this.qdcservice.TestataForm.get('Specie').value,
            data:  this.qdcservice.TestataForm.get('Data').value,
            macchine:  ArrayDefaultMacchine,
            contatti: ArrayDefaultOperatori
        };

        const Promise_Risp = this.profilazioneservice.Scrivi_Profilazione_Dati_QdC(LeggiProfilazione);

        Promise_Risp.then((risp:RispostaStandard) => {

            if(risp.RispostaOK){
                this.qdcservice.RicaricaGridOperatori().then();
                this.messageservice.successMessage(this.translocoService.translate('ProfiloMemorizzatoCorrettamente'),false);
            }

        });

    }

}
