import { Inject, Injectable, Injector } from '@angular/core';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { forkJoin, from, Observable } from 'rxjs';

import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service'; 
import { ObjParametriAgenda } from 'gias-ui-kit';
import {  DropdownListWithForm, DropdownListItem, EditingMode, GridInfoCommandEvent, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, ModelEntry } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { map } from 'rxjs/operators';
import { MasterService, RispostaStandard } from 'app/Service/master.service';
import { ConfigTemplate } from 'gias-kendo-grid';
import { AddEvent, CellClickEvent, EditEvent } from '@progress/kendo-angular-grid';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { IntlService } from '@progress/kendo-angular-intl';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { AggregateSettings, AgrSelectableSettings, CommandsColumnSettings, DettagliColumnSettings, ToolbarSettings } from 'gias-kendo-grid';
import { enum_TipoOperazioneDB } from 'app/Model/TipiEnumerativi';
import { Router } from '@angular/router';
import { AppezzamentiEventsService } from './appezzamenti-events.service';
import { AppezzamentiServerResult, AppezzamentoModel } from './appezzamenti.model';
import { ImpiantiFactoryService, IMPIANTI_SERVICE_TOKEN } from 'app/Service/ServiceFactory/impianti.factory.service';
import { TranslocoService } from '@jsverse/transloco';





@Injectable({
    providedIn:'root'
})

export class AppezzamentiHttpService extends AbstractGridConfigService<AppezzamentiServerResult>{
    gridId = 'AppezzamentiHttpService';
    rowId = 'chiave';

    readonly STRING: string       = CELL_TYPES.STRING;
    readonly DATE: string         = CELL_TYPES.DATE;
    readonly DROPDOWNLIST: string = CELL_TYPES.DROPDOWNLIST;
    readonly NUMERIC: string      = CELL_TYPES.NUMBER;

    objParametriAgenda: ObjParametriAgenda;

    GridAppezzamentiServerResult: AppezzamentiServerResult;

    GridAppezzamentiRows: KendoGridRow[];

    GridAppezzamentiModel: KendoGridModel;

    GridAppezzamentiColumns: KendoGridColumn[];

    Array_Particelle_Per_Appezzamento: Array<any>=null;

    Array_Validita_Appezzamenti: Array<any>=null;

    loader: LoaderType = LoaderType.SERVICE;
    editingMode: EditingMode = EditingMode.IN_LINE;

    kendoColumns: KendoGridColumn[] = [
        new KendoGridColumn({field: 'sa_nome', title: this.transloco.translate('Centro')},{resizable:true,editable: false,width: 150}),
        new KendoGridColumn({field: 'Campo_Des', title: this.transloco.translate('Campo')},{resizable:true,editable: false,width: 150}),
        new KendoGridColumn({field: 'APP_NOME',title: this.transloco.translate('Nome')},{resizable:true,editable: true,width: 150,validators:[Validators.required]}),
        new KendoGridColumn({field: 'utilizzo',title: this.transloco.translate('Utilizzo')},{resizable:true,editable: true,width: 150}),
        new KendoGridColumn({field: 'rif_alfanumerico',title: this.transloco.translate('RiferimentoAppezzamentoAbbr')},{hidden: true,resizable:true,editable: false,width: 150}),
        new KendoGridColumn({field: 'cod_biologico',title: this.transloco.translate('AppBioCod')},{hidden: true,resizable:true,editable: false,width: 150}),
        new KendoGridColumn({field: 'cod_kpin',title: this.transloco.translate('KPIN')},{hidden: true,resizable:true,editable: false,width: 100}),
        new KendoGridColumn({field: 'cod_block',title: this.transloco.translate('BLOCK')},{hidden: true,resizable:true,editable: false,width: 100}),
        new KendoGridColumn({field: 'SUP_APP',title: this.transloco.translate('SuperficieAppezzamentoAbbr')},{resizable:true,editable: true,width: 150,numeric:{ min: 0 },validators:[Validators.required]}),

        new KendoGridColumn({field: 'Validita_Inizio',title: this.transloco.translate('InizioValidità')},{resizable:true,editable: true,width: 150,date:{},validators:[Validators.required]}),
        new KendoGridColumn({field: 'Validita_Fine',title: this.transloco.translate('FineValidità')},{resizable:true,editable: true,width: 150,date:{},validators:[Validators.required]}),
        new KendoGridColumn({field: 'Data_Modifica',title: this.transloco.translate('DataModifica')},{resizable:true,editable: false,width: 150}),
        new KendoGridColumn({field: 'utente_modifica',title: this.transloco.translate('UtenteModifica')},{resizable:true,editable: false,width: 150}),
        new KendoGridColumn({field: 'Data_Creazione',title: this.transloco.translate('DataCreazione')},{resizable:true,editable: false,width: 150}),
        new KendoGridColumn({field: 'utente_creazione',title: this.transloco.translate('UtenteCreazione')},{resizable:true,editable: false,width: 150}),
        new KendoGridColumn({field: 'isola',title: this.transloco.translate('Isola')},{resizable:true,editable: false,width: 150}),
        new KendoGridColumn({field: 'MetodoProduzione_Cod',title: this.transloco.translate('MetodoProduzione')},{resizable:true,editable: true,width: 150}),
        new KendoGridColumn({field: 'Blk_Flag_Des',title: this.transloco.translate('Bloccato')},{hidden: true,resizable:true,editable: false,width: 150})
    ];

    kendoModel: AppezzamentoModel = {
        sa_nome: new ModelEntry(CELL_TYPES.STRING, false),
        Campo_Des: new ModelEntry(CELL_TYPES.STRING, false),
        APP_NOME: new ModelEntry(CELL_TYPES.STRING, false),
        utilizzo: new ModelEntry(CELL_TYPES.STRING, false),
        rif_alfanumerico: new ModelEntry(CELL_TYPES.STRING, false),
        cod_biologico: new ModelEntry(CELL_TYPES.STRING, false),
        cod_kpin: new ModelEntry(CELL_TYPES.STRING, false),
        cod_block: new ModelEntry(CELL_TYPES.STRING, false),
        SUP_APP: new ModelEntry(CELL_TYPES.NUMBER, false),
        Validita_Inizio: new ModelEntry(CELL_TYPES.DATE, false),
        Validita_Fine: new ModelEntry(CELL_TYPES.DATE, false),
        Data_Modifica: new ModelEntry(CELL_TYPES.DATETIME, false),
        utente_modifica: new ModelEntry(CELL_TYPES.STRING, false),
        Data_Creazione: new ModelEntry(CELL_TYPES.DATETIME, false),
        utente_creazione: new ModelEntry(CELL_TYPES.STRING, false),
        isola: new ModelEntry(CELL_TYPES.STRING, false),
        MetodoProduzione_Cod:{type: CELL_TYPES.DROPDOWNLIST},
        MetodoProduzione_Des: new ModelEntry(CELL_TYPES.STRING, false),
        Blk_Flag: new ModelEntry(CELL_TYPES.NUMBER, false),
        Blk_Flag_Des: new ModelEntry(CELL_TYPES.STRING, false)
    };

    constructor(injector: Injector,
        @Inject(IMPIANTI_SERVICE_TOKEN) private appezzamentiService: ImpiantiFactoryService,
        private ObjParametriAgendaService: ObjParametriAgendaService,
        private intlService: IntlService,
        private masterService: MasterService,
        private appezzamentiEventsService: AppezzamentiEventsService,
        private router: Router,
        protected transloco: TranslocoService) // non togliere perché è necessario al funzionamento della funzione editColumn.edit
        {

        super(injector,ConfigTemplate.DefaultTemplate);

        this.objParametriAgenda = this.ObjParametriAgendaService.getObjParamValue();

        this.toolbar = new ToolbarSettings(true, true);

        this.cmdColumn = new CommandsColumnSettings({
            editBtn: false,
            infoBtn: false,
            removeBtn: true
        });

        this.dettagliColumn = new DettagliColumnSettings({
            editBtn: true,
            edit: this.editAppezzamento.bind(this)
        })

        this.resizable.autoFitColumns=true;

        this.resizable.isResizable=true;

        this.behavior.createFormGroupFromOutside=true;
        this.toolbar.resetChanges = false;
        this.toolbar.newItem = true;
        this.behavior.excelSettings.enabled = true;
        this.behavior.pdfSettings.enabled = false;
        this.groups.groupable.enabled = false;

        this.selectable = new AgrSelectableSettings();
        this.selectable.selectable.checkboxOnly = false;

    }


    perform(actionType: HttpAction, items: any): Observable<any[]> {
        return from([]);
    }

    read(): Observable<AppezzamentiServerResult> {

        //this.GridAppezzamentiColumns=this.setColumnsGridAppezzamenti();
        this.GridAppezzamentiColumns=this.kendoColumns;

        this.setDropdownLists(this.GridAppezzamentiColumns.find((c) => {
            if(c.field === 'MetodoProduzione_Cod') {
                return c;
            }
        }));

        //this.GridAppezzamentiModel=this.setModelGridAppezzamenti();
        this.GridAppezzamentiModel=this.kendoModel;

        /** Letture observable (obs):
     * 0. Lettura per controllare se disabilitare o meno la colonna Sup.[ha]
     * 1. Lettura per controllare i periodi temporali di Validità che deve avere l'appezzamento
     * 2. Lettura per caricare le righe della griglia degli appezzamenti
    **/

        const objAgenda=new ObjParametriAgenda;

        objAgenda.Piva=this.objParametriAgenda.Piva;

        this.masterService.set_isLoading({ isLoading: true, message: 'Loading' });

        const obs: Promise<string>[] = [ this.appezzamentiService.LeggiParticelle_Per_Appezzamento(objAgenda),
            this.appezzamentiService.Controlla_Validita_Appezzamenti(objAgenda),
            this.appezzamentiService.leggiAppezzamenti(objAgenda) ];

        return forkJoin(obs).pipe(map(result => {

            this.masterService.set_isLoading({ isLoading: false, message: 'Loading' });

            this.Array_Particelle_Per_Appezzamento=JSON.parse(result[0]);

            this.Array_Validita_Appezzamenti=JSON.parse(result[1]);

            this.GridAppezzamentiRows = <KendoGridRow[]>(JSON.parse(result[2]));

            this.GridAppezzamentiServerResult = new AppezzamentiServerResult(this.GridAppezzamentiRows,
                this.GridAppezzamentiColumns,
                this.GridAppezzamentiModel);

            return this.GridAppezzamentiServerResult;
        }));
    }

    editAppezzamentoInLine(event: EditEvent){
      let a = 0; //Commento per funzione vuota SonarQube
    }

    public override onCreateExternalFormGroup = (event: any) => {

        const formGroup = new FormGroup({});

        let disabilitacolonnaSuperficie=false;

        let Data_Operazione_Max_Validita_inizio=AGRODATAFINE;

        let Data_Operazione_Min_Validita_fine=AGRODATAINIZIO;

        if(event?.action === 'edit'){

            for(var x=0; x< this.Array_Particelle_Per_Appezzamento.length;x++){
                if(event.dataItem.Piva === this.Array_Particelle_Per_Appezzamento[x].PIVA &&
                    event.dataItem.Sa_Cod === this.Array_Particelle_Per_Appezzamento[x].SA_COD &&
                    event.dataItem.Appezza === this.Array_Particelle_Per_Appezzamento[x].APPEZZA){

                    disabilitacolonnaSuperficie=true;
                    break;
                }
            }

            for(var x=0; x< this.Array_Validita_Appezzamenti.length;x++){
                if(event.dataItem.Piva === this.Array_Validita_Appezzamenti[x].Piva &&
                    event.dataItem.Sa_Cod === this.Array_Validita_Appezzamenti[x].Sa_Cod &&
                    event.dataItem.Appezza === this.Array_Validita_Appezzamenti[x].Appezza){

                    Data_Operazione_Max_Validita_inizio=this.intlService.parseDate(this.Array_Validita_Appezzamenti[x].Data_Operazione_Max_Validita_inizio);
                    Data_Operazione_Min_Validita_fine=this.intlService.parseDate(this.Array_Validita_Appezzamenti[x].Data_Operazione_Min_Validita_fine);
                    break;
                }
            }

        }

        const row: KendoGridRow=event.dataItem;

        this.GridAppezzamentiColumns.forEach((s: KendoGridColumn) => {
            let value: string | Date = row[s.field];
            if(this.GridAppezzamentiModel[s.field]?.type === this.DATE) {
                value = this.intlService.parseDate(<string>row[s.field]);
            }

            if(this.GridAppezzamentiModel[s.field]?.type === this.DROPDOWNLIST && !s.ddl.valuePrimitive) {
                value = row[s.ddl.formControlValue];
            }

            if(Data_Operazione_Max_Validita_inizio !== AGRODATAFINE && s.field==='Validita_Inizio'){
                s.date.max = Data_Operazione_Max_Validita_inizio;
            }

            if(Data_Operazione_Min_Validita_fine !== AGRODATAINIZIO && s.field==='Validita_Fine'){
                s.date.min = Data_Operazione_Min_Validita_fine;
            }

            const formState: any={
                value:value,
                disabled: (disabilitacolonnaSuperficie && s.field==='SUP_APP') ? true : false
            };

            const formCtrl = new FormControl(formState, s.validators);
            formGroup.addControl(s.field, formCtrl);
        });

        return formGroup;

    };

    private setDropdownLists(column: KendoGridColumn) {
        if(column.field === 'MetodoProduzione_Cod') {
            const data: DropdownListItem[] = this.appezzamentiService.getArray_Metodo_Produzione().map(m => new DropdownListItem(m.MetodoProduzione_Cod, m.MetodoProduzione_Des));

            column.ddl = new DropdownListWithForm('ddl_Metodo_Produzione', 'MetodoProduzione_Cod', 'MetodoProduzione_Des', data, null);
            column.ddl.valuePrimitive = false;
        }
    }

    public editAppezzamento(el) {
        this.appezzamentiEventsService.onTemplateBtnClick(el);
    }

}











// TODO
        /*       "field": "sup_app",
  "title": Traduzione(menuBSAnagraficaResx, "SuperficieAbbr", "Sup.") + " [ha]",
  "filterable": {
      operators: {
          number: {
              eq: Traduzione(menuBSAnagraficaResx, "UgualeA", "Uguale a"),
              gte: Traduzione(menuBSAnagraficaResx, "MaggioreDi", "Maggiore di"),
              lte: Traduzione(menuBSAnagraficaResx, "MinoreDi", "Minore di")
          }
      }
  },
  "format": "{0:n4}",
  "footerTemplate": Traduzione(menuBSAnagraficaResx, "Totale", "Totale") + ": #: kendo.toString(sum, \"n4\") # ",
  "width": "150px"
   {
        "field": "Validita_Inizio",
        "title": Traduzione(menuBSAnagraficaResx, "InizioValidità", "Inizio Validità"),
        filterable: {
            ui: "datepicker"
        },
        template: '#= (kendo.toString(Validita_Inizio, "dd/MM/yyyy" ) == "01/01/1900") ? "" : kendo.toString(Validita_Inizio, "dd/MM/yyyy" ) #',
        "width": "150px"
    },
    {
        "field": "Validita_Fine",
        "title": Traduzione(menuBSAnagraficaResx, "FineValidità", "Fine Validità"),
        filterable: {
            ui: "datepicker"
        },
        template: '#= (kendo.toString(Validita_Fine, "dd/MM/yyyy" ) == "31/12/2100") ? "" : kendo.toString(Validita_Fine, "dd/MM/yyyy" ) #',
        "width": "150px"
    },*/
