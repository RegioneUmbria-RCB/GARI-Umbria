import {Injectable, Injector, OnDestroy, Optional} from '@angular/core';
import {FormArray, FormGroup} from '@angular/forms';
import {TranslocoService} from '@jsverse/transloco';
import {Lavorazione} from 'app/Model/attivita/Lavorazione';
import {FERTILIZZANTI, FORMULATI, SEMENTI} from 'app/Model/CostantiPersonalizzate';
import {UnitaDiMisura} from 'app/Model/metaschema/UnitaDiMisura';
import {enum_LAVCOD, enum_TipoMezzo, enum_TipoOperazioneDB} from 'app/Model/TipiEnumerativi';
import {ObjParametriAgendaService} from 'app/Service/obj-parametri-agenda.service';
import {
    CommandsColumnSettings,
    DettagliColumnSettings,
    GroupSettings,
    SelectableSettings
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
import {UtilityFunctions} from 'app/Utility/UtilityFunctions';
import {BehaviorSubject, from, lastValueFrom, Observable, of, Subscription} from 'rxjs';
import {CalcoloSuperficiService} from '../calcolo-superfici.service';
import {GridImpiantiService} from '../grid-impianti/grid-impianti.service';
import {MisceleService} from '../miscele.service';
import {QdCProdottiService} from '../prodotti.service';
import {QdCService} from '../qdc.service';
import {QdCUnitadiMisuraService} from '../unita-di-misura.service';
import {GridDosiProdottiService} from './grid-dosi-prodotti.service';
import {GridDosiProdottiControlliService} from "./grid-dosi-prodotti-controlli.service";
import {QdCDettagliFertilizzantiService} from "../prodotti/dettagli-fertilizzanti.service";
import {QdCDettagliFormulatiService} from "../prodotti/dettagli-formulati.service";
import {QdCDettagliSementiService} from "../prodotti/dettagli-sementi.service";
import {QdCFertilizzantiService} from "../prodotti/fertilizzanti.service";

export class GridDosiProdottiServerResult extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

export class GridDosiProdotti_ddl_UdM{
    simbolo: string;
    codice: string;
    flagTipoDose: number;
    unitadimisura: UnitaDiMisura
}

@Injectable()
export class GridDosiProdottiHttpService extends AbstractGridConfigService<GridDosiProdottiServerResult> implements OnDestroy {
    gridId = 'GridDosiProdotti';
    rowId = 'DosiProdottiGridrowId';

    GridDosiProdottiServerResult: GridDosiProdottiServerResult;

    Categoria_Magazzino: number;

    Operazione: Lavorazione;

    Array_UdM_Grid: Array<GridDosiProdotti_ddl_UdM>=null;

    Subs:Subscription = new Subscription();

    ID_RigaDoseProdottoDaModificare: number = -1;

    GridDosiProdotti: any= {};

    loader: LoaderType = LoaderType.SERVICE;
    editingMode: EditingMode = EditingMode.IN_LINE;

    views = new GridCustomizations({enabled: false});

    //Se sono in modalità edit completo nascondo i pulsanti della grid
    EditCompleto: BehaviorSubject<any> = new BehaviorSubject<any>(null);

    constructor(
        injector: Injector,
        public qdcservice: QdCService,
        private prodottiservice: QdCProdottiService,
        private objParametriAgendaService: ObjParametriAgendaService,
        private translocoService: TranslocoService,
        private misceleService: MisceleService,
        private unitadimisuraservice: QdCUnitadiMisuraService,
        private griddosiprodottiservice: GridDosiProdottiService,
        @Optional() private qdcfertilizzantiservice: QdCFertilizzantiService,
        @Optional() private qdcdettagliformulatiservice: QdCDettagliFormulatiService,
        @Optional() private qdcdettaglifertilizzantiservice: QdCDettagliFertilizzantiService,
        @Optional() private qdcdettaglisementiservice: QdCDettagliSementiService,
        private gridcontrolliservice: GridDosiProdottiControlliService
    ) {

        super(injector, ConfigTemplate.DefaultTemplate);

        this.handleCustomization();


        this.Subs.add(this.gridPublicService.formGroup.subscribe(async (formGroup: FormGroup)=>{

            if(formGroup) {

                formGroup.patchValue({
                    Riga_Salvata: false
                },{emitEvent: false});


                //Disabilito N_Utile perchè non è editabile ma la colonna la dovevo rendere editabile se no non viene ricaricato
                //il valore a video
                if(+(formGroup.get("Operazione").value as Lavorazione).primaryKey.codice === enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI)
                    formGroup.get("N_Utile").disable({ emitEvent: false });

                const flagTipoDose = (formGroup.get('flagTipoDose').value as number);

                this.disableHlHaInBaseAUdm(flagTipoDose, formGroup);

                const udm = (formGroup.get('Udm_Cod') as FormGroup);

                this.unitadimisuraservice._unita_grid = (await lastValueFrom(this.loadUdms(formGroup.getRawValue()))).find(udm_grid=> { return udm_grid.codice === udm.getRawValue();}).unitadimisura;

                this.Subs.add(udm.valueChanges.subscribe((value:string)=>{

                    let val: GridDosiProdotti_ddl_UdM = this.Array_UdM_Grid.find( udm => { return udm.codice === value; });

                    if(val.flagTipoDose === enum_TipoMezzo.Ettaro){
                        this.misceleService.dose_ha_change(this.qdcservice.QdCForm, formGroup,this.qdcservice.mostraAcqua(),this.qdcservice.getOpzione_Semina_Model(this.Operazione),formGroup.get("Sup_Calcolata").value);
                    }else if(val.flagTipoDose === enum_TipoMezzo.Ettolitro){
                        this.misceleService.dose_hl_change(this.qdcservice.QdCForm, formGroup,this.qdcservice.mostraAcqua());
                    }

                    formGroup.patchValue({
                        UdM_Indicata: new UnitaDiMisura(val.unitadimisura.codice,val.simbolo,val.unitadimisura.descrizione),
                        UdM: val.unitadimisura,
                        UdM_Magazzino: this.griddosiprodottiservice.getUnitaDiMisura(val.unitadimisura)
                    },{emitEvent: false});

                    this.unitadimisuraservice.changeUdM(val.unitadimisura,formGroup,true);

                    this.disableHlHaInBaseAUdm(val.flagTipoDose, formGroup);

                    this.unitadimisuraservice._unita_grid = val.unitadimisura;
                }));

                const doseHa = (formGroup.get('Dose_Ha') as FormGroup);

                this.Subs.add(doseHa?.valueChanges?.subscribe((value) => {
                    this.misceleService.dose_ha_change(this.qdcservice.QdCForm, formGroup,this.qdcservice.mostraAcqua(),this.qdcservice.getOpzione_Semina_Model(this.Operazione),formGroup.get("Sup_Calcolata").value);
                }));

                const doseHl = (formGroup.get('Dose_Hl') as FormGroup);

                this.Subs.add(doseHl?.valueChanges?.subscribe((value) => {
                    this.misceleService.dose_hl_change(this.qdcservice.QdCForm, formGroup,this.qdcservice.mostraAcqua());
                }));

                const qtaTotale = (formGroup.get('DoseTot_Ha') as FormGroup);

                this.Subs.add(qtaTotale?.valueChanges?.subscribe((value) => {
                    this.misceleService.dose_tot_ha_change(this.qdcservice.QdCForm, formGroup,this.qdcservice.mostraAcqua(),this.qdcservice.getOpzione_Semina_Model(this.Operazione),formGroup.get("Sup_Calcolata").value);
                }));

                const supCalcolata = (formGroup.get('Sup_Calcolata') as FormGroup);

                this.Subs.add(supCalcolata?.valueChanges?.subscribe((value) => {
                    this.misceleService.sup_calcolata_change(formGroup,value);
                }));

                const N = (formGroup.get('N') as FormGroup);

                this.Subs.add(N?.valueChanges?.subscribe((value) => {
                    this.qdcdettaglifertilizzantiservice.changeN(formGroup);
                }));

            }

        }));

    }

    private handleCustomization() {
        // Nascondo la colonna Azioni con i bottoni di Info,Modifica e Cancella
        this.cmdColumn = new CommandsColumnSettings({
            editBtnTitle: 'qdc.ModificaQuantitaProdotto',
            editBtn: this.qdcservice.abilitaGrid,
            infoBtn: false,
            removeBtn: this.qdcservice.abilitaGrid,
            onDisableInfoBtn: () => false,
            onDisableRemoveBtn: (row)=> this.DisabilitaBottoni(row),
            onDisableEditBtn: (row)=> this.DisabilitaBottoni(row),
        });

        this.dettagliColumn = new DettagliColumnSettings({
            editBtn: this.qdcservice.abilitaGrid,
            edit: this.AbilitaEditCompleto.bind(this),
            title: "qdc.ModificaCompleta",
            onDisableEditBtn: (row)=> this.DisabilitaBottoni(row)
        })

        // Setting colonna con checkbox
        this.selectable.selectable = new SelectableSettings({enabled: false});
        this.selectable.shouldShowCheckbox = false;

        this.resizable.autoFitColumns=false;
        this.resizable.isResizable=true;

        this.columnMenu.kendoGridColumnChooser=false;

        this.behavior.excelSettings.enabled=false;
        this.behavior.pdfSettings.enabled=false;

        this.pagination.pageable = false;

        this.groups = new GroupSettings({groupable: {enabled: false, showFooter: false}}, this.translocoService);
    }

    public DisabilitaBottoni(row: KendoGridRow){

        let disabilita = true;

        if(this.qdcservice.abilitaGrid){
            if(!this.EditCompleto.getValue())
                disabilita = false;
        }

        return disabilita;
    }

    private AbilitaEditCompleto(dataitem,isNew,rowIndex) {
        this.EditCompleto.next({ Dataitem: dataitem, RowIndex: rowIndex});
    }

    ngOnDestroy(): void {
        this.Subs.unsubscribe();
    }

    private onRemoveGridDosiProdotti(dataItem) {

        const DosiProdotti = this.prodottiservice.ProdottiForm.get('DosiProdotti') as any as FormArray;
        const rowIndex = DosiProdotti.getRawValue().findIndex(d=>d.DosiProdottiGridrowId === dataItem.DosiProdottiGridrowId);

        this.qdcservice.AggiornaFormArrayGridDosiProdotti(dataItem,rowIndex,this.Operazione,enum_TipoOperazioneDB.Cancellazione,null);

        this.prodottiservice.EventiPostRemoveGridDosiProdotti();

    }

    disableHlHaInBaseAUdm(hlha: number, formGroup: FormGroup) {
        if(hlha === enum_TipoMezzo.Ettolitro) {
            formGroup.get('Dose_Ha').disable({ emitEvent: false });
            formGroup.get('Dose_Hl').enable({ emitEvent: false });
        } else if(hlha === enum_TipoMezzo.Ettaro) {
            formGroup.get('Dose_Hl').disable({ emitEvent: false });
            formGroup.get('Dose_Ha').enable({ emitEvent: false });
        }
    }

    perform(actionType: HttpAction, item: any): Observable<any> {

        let Obs: Observable<any> = of([]);

        if(actionType === HttpAction.UPDATE || actionType === HttpAction.CREATE) {

            let tipo_operazione_db = enum_TipoOperazioneDB.Scrittura;

            if(actionType === HttpAction.UPDATE){
                tipo_operazione_db = enum_TipoOperazioneDB.Modifica;
                this.unitadimisuraservice._unita_grid = item.UdM;
            }

            Obs = from(new Promise( (resolve, reject) => {

                let DosiProdotti: Array<any> = this.prodottiservice.ProdottiForm.get('DosiProdotti').getRawValue();

                this.gridcontrolliservice.ControllaSeInserireDoseProdotto(DosiProdotti,item,this.prodottiservice.ProdottiForm,tipo_operazione_db, item.DosiProdottiGridrowId,
                                                                            this.qdcdettagliformulatiservice?.doseConsentitaDiserbo,
                                                                            this.qdcdettagliformulatiservice?.Array_Soglie_Avversita,this.qdcdettagliformulatiservice?.Array_Dosi).then(errori=>{
                    if(errori.length === 0){
                        const index = (DosiProdotti).findIndex(prodotto => prodotto[this.rowId] === item[this.rowId]);

                        item.Riga_Salvata = true;

                        this.qdcservice.AggiornaFormArrayGridDosiProdotti(item,index,this.Operazione,tipo_operazione_db,null);

                        resolve([]);
                    }else {

                        resolve(null);
                    }
                })
            }));

        }else if(actionType === HttpAction.REMOVE) {
            this.onRemoveGridDosiProdotti(item);
        }

        return Obs;

    }

    read(): Observable<GridDosiProdottiServerResult> {
        this.loadingService.set_isLoading({isLoading: true, message: '', component: this.gridPublicService.gridElRef});

        this.Operazione = this.prodottiservice.ProdottiForm.get("Operazione").value;

        this.Categoria_Magazzino = this.prodottiservice.ProdottiForm.get("Categoria_Magazzino").value;

        this.gridId = this.gridId + "_" + this.Operazione.primaryKey.codice;

        this.GridDosiProdotti.kendo_rows = this.prodottiservice.ProdottiForm.get('DosiProdotti').getRawValue();
        this.GridDosiProdotti.kendo_columns = this.setColumnsGridDosiProdotti();
        this.GridDosiProdotti.kendo_model = this.setModelGridDosiProdotti();

        this.GridDosiProdottiServerResult = new GridDosiProdottiServerResult (this.GridDosiProdotti.kendo_rows,
            this.GridDosiProdotti.kendo_columns,
            this.GridDosiProdotti.kendo_model
        );

        this.loadingService.set_isLoading({isLoading: false, message: '', component: this.gridPublicService.gridElRef});

        return of(this.GridDosiProdottiServerResult);
    }

    loadUdms(dataItem) {

        //Le udm vengono caricate quando viene aperto l'edit in line della righa perchè mi serve avere l'ultima unita di misura della ddl

        let Obs: Observable<any> = null;

            Obs =  from(this.prodottiservice.getArray_UdM(dataItem,false).then((udms: Array<UnitaDiMisura>)=>{

                let Array_hl_ha: Array<string> = ['ha'];

                //Aggiungo hl solo se l'operazione lo permette
                if(this.qdcservice.mostraDose_Hl(this.Operazione))
                    Array_hl_ha.unshift('hl');

                const result = cartesian(udms, Array_hl_ha);
                this.Array_UdM_Grid = result.map(
                    (entry, index) =>
                    {

                        const udm:UnitaDiMisura = entry[0];

                        let isHl = false;

                        if(this.qdcservice.mostraDose_Hl(this.Operazione))
                            isHl = index % 2 === 0;

                        const codice = udm.codice + (isHl ? "_" + enum_TipoMezzo.Ettolitro:
                            "_" + enum_TipoMezzo.Ettaro);

                        return <GridDosiProdotti_ddl_UdM>{
                            simbolo: '[' + udm.simbolo + "/" + entry[1] + ']',
                            codice: codice,
                            unitadimisura: udm,
                            flagTipoDose: (isHl ? enum_TipoMezzo.Ettolitro: enum_TipoMezzo.Ettaro)
                        };
                    });

                return this.Array_UdM_Grid;

            }));

        return  Obs;

    }

    setColumnsGridDosiProdotti(){

        const numericsettings=this.qdcservice.getProductNumericSettings(null);

        numericsettings.autoCorrect = true;

        let ddl_UdM = new DropdownListWithForm('codice', 'Udm_Cod', 'simbolo', []);
        ddl_UdM.valuePrimitive = true;
        ddl_UdM.loadOnEdit = true;
        ddl_UdM.descriptionField = 'Udm_Des';
        ddl_UdM.loadFunction = this.loadUdms.bind(this);

        let columns: Array<KendoGridColumn>=[
            new KendoGridColumn({field: 'Av_Des',title: this.translocoService.translate('qdc.lbl_avversitaResource1.Text')},{resizable:true,editable: false,width: 150}),
            new KendoGridColumn({field: 'Soglia_Des',title: this.translocoService.translate('qdc.RichiestaSogliaGiustif')},{resizable:true,editable: false,width: 150}),
            new KendoGridColumn({field: 'Fr_Cod',title: this.translocoService.translate('qdc.BoundFieldResource11.HeaderText')},{resizable:true,editable: false,width: 100}),
            new KendoGridColumn({field: 'Fr_Des',title: this.translocoService.translate('qdc.BoundFieldResource12.HeaderText')},{resizable:true,editable: false,width: 150}),
            new KendoGridColumn({field: 'Efficienza',title: this.translocoService.translate('qdc.lblEfficienzaResource1.Text')},{resizable:true,editable: false,format: '{0:n4}',numeric: numericsettings,includeInChooser: false,width: 100}),
            new KendoGridColumn({field: 'N',title: this.translocoService.translate('qdc.lbl_NResource1.Text')},{resizable:true,editable: false,format: '{0:n4}',numeric: numericsettings,includeInChooser: false,width: 90}),
            new KendoGridColumn({field: 'N_Utile',title: this.translocoService.translate('qdc.lblNUtileResource1.Text')},{resizable:true,editable: false,format: '{0:n4}',numeric: numericsettings,includeInChooser: false,width: 90}),
            new KendoGridColumn({field: 'P',title: this.translocoService.translate('qdc.lbl_P2O5Resource1.Text')},{resizable:true,editable: false,format: '{0:n4}',numeric: numericsettings,includeInChooser: false,width: 90}),
            new KendoGridColumn({field: 'K',title: this.translocoService.translate('qdc.lblK2OResource1.Text')},{resizable:true,editable: false,format: '{0:n4}',numeric: numericsettings,includeInChooser: false,width: 90}),
            new KendoGridColumn({field: 'Cu',title: this.translocoService.translate('qdc.lblCuResource1.Text')},{resizable:true,editable: false,format: '{0:n4}',numeric: numericsettings,includeInChooser: false,width: 90}),
            new KendoGridColumn({field: 'Carenza',title: this.translocoService.translate('qdc.BoundFieldResource15.HeaderText')},{resizable:true,editable: false,format: '{0:n}',numeric: numericsettings,includeInChooser: false,width: 100}),
            new KendoGridColumn({field: 'Prima_Raccolta',title: this.translocoService.translate('qdc.BoundFieldResource16.HeaderText')},{resizable:true,editable: false,width: 100}),
            new KendoGridColumn({field: 'strBuffer',title: this.translocoService.translate('qdc.Buffer_Zone_m')},{resizable:true,editable: false,width: 100}),
            new KendoGridColumn({field: 'Dose_Etichetta_Des',title: this.translocoService.translate('qdc.BoundFieldResource13.HeaderText')},{resizable:true,editable: false,showHTMLAsString:true,width: 200}),
            new KendoGridColumn({field: 'Cod_Articolo',title: this.translocoService.translate('CodiceArticolo')},{resizable:true,editable: false,width: 100}),
            new KendoGridColumn({field: 'Lotto',title: this.translocoService.translate('Lotto2')},{resizable:true,editable: false,width: 100}),
            new KendoGridColumn({field: 'Udm_Cod',title: this.translocoService.translate('RisorsaUDM')},{resizable:true,editable: this.qdcservice.abilitaGrid,ddl: ddl_UdM,includeInChooser: false,width: 130}),
            new KendoGridColumn({field: 'Dose_Ha',title: this.translocoService.translate('qdc.BoundFieldResource21.HeaderText')},{resizable:true,editable: this.qdcservice.abilitaGrid,format: '{0:n4}',numeric: numericsettings,includeInChooser: false,width: 130}),
            new KendoGridColumn({field: 'Dose_Hl',title: this.translocoService.translate('qdc.BoundFieldResource22.HeaderText')},{resizable:true,editable: this.qdcservice.abilitaGrid,format: '{0:n4}',numeric: numericsettings,includeInChooser: false,width: 130}),
            new KendoGridColumn({field: 'DoseTot_Ha',title: this.translocoService.translate('qdc.BoundFieldResource23.HeaderText')},{resizable:true,editable: this.qdcservice.abilitaGrid,format: '{0:n4}',numeric: numericsettings,includeInChooser: false,width: 130}),
            new KendoGridColumn({field: 'Sup_Calcolata',title: this.translocoService.translate('qdc.Sup_Calcolata_ha')},{resizable:true,editable: this.qdcservice.abilitaGrid,hidden:false, includeInChooser:false,format: '{0:n4}',numeric: numericsettings,width: 100}),
            new KendoGridColumn({field: 'Fabbricato_Des',title: this.translocoService.translate('Magazzino')},{resizable:true,editable: false,width: 150}),
            new KendoGridColumn({field: 'Des_Rif',title: this.translocoService.translate('qdc.Riferimento')},{resizable:true,editable: false,width: 100})
        ];

        // TODO Cambio le colonne in base al Lav_Cod
        // Da controllare nella trattamenti-2 cercando GridView_Dosi.Columns

        switch(this.Categoria_Magazzino){
            case FORMULATI:
                let Formulato_index = columns.findIndex(((column: KendoGridColumn) => column.field === 'Fr_Des'));

                columns[Formulato_index].title=this.translocoService.translate('qdc.BoundFieldResource12.HeaderText');

                columns = columns.filter(
                    function(column: KendoGridColumn) {
                        const field = column.field;
                        if(field !== 'Efficienza' &&
                            field !== 'N' &&
                            field !== 'N_Utile' &&
                            field !== 'P' &&
                            field !== 'K' &&
                            field !== 'MgO' &&
                            field !== 'Cu' &&
                            field !== 'Cod_Articolo' &&
                            field !== 'Sup_Calcolata'){
                            return column;
                        }
                    }
                );

                if (this.qdcservice.GestioneLotti_Formulati === 0) {
                    columns = columns.filter(
                        function(column: KendoGridColumn) {
                            const field = column.field;
                            if(field !== 'Lotto'){
                                return column;
                            }
                        }
                    );
                }

                switch(+ this.Operazione.primaryKey.codice){

                    case enum_LAVCOD.TRATTAMENTO_ANTIPARASSITARIO:
                        break;

                    case enum_LAVCOD.DISSECCAMENTO:
                        columns = columns.filter(
                            function(column: KendoGridColumn) {
                                const field = column.field;
                                if(field !== 'Av_Des' &&
                                    field !== 'Soglia_Des'){
                                    return column;
                                }
                            }
                        );
                        break;

                    case enum_LAVCOD.TRATTAMENTO_FITOREGOLATORE:
                        columns = columns.filter(
                            function(column: KendoGridColumn) {
                                const field = column.field;
                                if(field !== 'Av_Des' &&
                                    field !== 'Soglia_Des'){
                                    return column;
                                }
                            }
                        );
                        break;

                    case enum_LAVCOD.DISERBO:
                        columns = columns.filter(
                            function(column: KendoGridColumn) {
                                const field = column.field;
                                if(field !== 'Soglia_Des'){
                                    return column;
                                }
                            }
                        );

                        const index = columns.findIndex(((column: KendoGridColumn) => column.field === 'Av_Des'));

                        columns[index].title=this.translocoService.translate('Infestanti') + "/" + this.translocoService.translate('GruppiInfestanti');
                        break;

                    case enum_LAVCOD.GEODISINFESTAZIONE:
                    case enum_LAVCOD.CONCIA_SEME:
                    case enum_LAVCOD.CONFUSIONE_SESSUALE:
                    case enum_LAVCOD.DISORIENTAMENTO_SESSUALE:
                        break;

                }
                break;
            case FERTILIZZANTI:
                let Fertilizzante_index = columns.findIndex(((column: KendoGridColumn) => column.field === 'Fr_Des'));

                columns[Fertilizzante_index].title=this.translocoService.translate('qdc.Fertilizzante');

                columns = columns.filter(
                    function(column: KendoGridColumn) {
                        const field = column.field;
                        if(field !== 'Fr_Cod' &&
                            field !== 'Av_Des' &&
                            field !== 'Soglia_Des' &&
                            field !== 'Dosi_Etichetta' &&
                            field !== 'Dose_Etichetta' &&
                            field !== 'Dose_Etichetta_Des' &&
                            field !== 'Carenza' &&
                            field !== 'Prima_Raccolta' &&
                            field !== 'strBuffer' &&
                            field !== 'Cod_Articolo'&&
                            field !== 'Sup_Calcolata'){
                            return column;
                        }
                    }
                );

                if (this.qdcservice.GestioneLotti_Fertilizzanti === 0) {
                    columns = columns.filter(
                        function(column: KendoGridColumn) {
                            const field = column.field;
                            if(field !== 'Lotto'){
                                return column;
                            }
                        }
                    );
                }

                switch(+ this.Operazione.primaryKey.codice){

                    case enum_LAVCOD.DISTRIBUZIONE_CONCIME:
                    case enum_LAVCOD.SARCHIATURA_CONCIMAZIONE:
                    case enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI:
                        columns = columns.filter(
                            function(column: KendoGridColumn) {
                                const field = column.field;
                                if(field !== 'Dose_Hl'){
                                    return column;
                                }
                            }
                        );
                        break;

                }

                //Abilito l'edit in line per N,P,K,Cu se sono in una DISTRIBUZIONE_AMMENDANTI
                if(+ this.Operazione.primaryKey.codice === enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI){
                    columns[columns.findIndex(c=>c.field === "N")].editable = this.qdcservice.abilitaGrid;

                    //Abilito anche N_utile per mostrare il valore cambiato ma l'utente non lo potrà modificare
                    columns[columns.findIndex(c=>c.field === "N_Utile")].editable = this.qdcservice.abilitaGrid;
                    columns[columns.findIndex(c=>c.field === "P")].editable = this.qdcservice.abilitaGrid;
                    columns[columns.findIndex(c=>c.field === "K")].editable = this.qdcservice.abilitaGrid;
                    columns[columns.findIndex(c=>c.field === "Cu")].editable = this.qdcservice.abilitaGrid;
                }

                break;
            case SEMENTI:
                // Sistemo la griglia con i prodotti/sementi

                columns = columns.filter(
                    function(column: KendoGridColumn) {
                        const field = column.field;
                        if(field !== 'Dose_Hl'){
                            return column;
                        }
                    }
                );

                if(!this.qdcdettaglisementiservice?.Mostra_Sup_Calcolata()){
                    columns = columns.filter(
                        function(column: KendoGridColumn) {
                            const field = column.field;
                            if(field !== 'Sup_Calcolata'){
                                return column;
                            }
                        }
                    );
                }

                let Semente_index = columns.findIndex(((column: KendoGridColumn) => column.field === 'Fr_Des'));

                columns[Semente_index].title=this.translocoService.translate('qdc.Semente_Piantina');

                columns = columns.filter(
                    function(column: KendoGridColumn) {
                        const field = column.field;
                        if(field !== 'Fr_Cod' &&
                            field !== 'Av_Des' &&
                            field !== 'Soglia_Des' &&
                            field !== 'Dose_Etichetta' &&
                            field !== 'Dosi_Etichetta' &&
                            field !== 'Dose_Etichetta_Des' &&
                            field !== 'Carenza' &&
                            field !== 'Prima_Raccolta' &&
                            field !== 'strBuffer' &&
                            field !== 'Efficienza' &&
                            field !== 'N' &&
                            field !== 'N_Utile' &&
                            field !== 'P' &&
                            field !== 'K' &&
                            field !== 'MgO' &&
                            field !== 'Cu'
                        ){
                            return column;
                        }
                    }
                );
            break;
        }

        return columns;
    }

    setModelGridDosiProdotti()
    {

        var model: KendoGridModel = {};

        model.Categoria_Magazzino = { type: 'number' };
        model.DosiProdottiGridrowId = { type: 'number' };
        model.Prodotto = new ModelEntry(CELL_TYPES.DROPDOWNLIST);
        model.UdM = new ModelEntry(CELL_TYPES.DROPDOWNLIST);
        model.Operazione = new ModelEntry(CELL_TYPES.DROPDOWNLIST);
        model.UdM_Magazzino = new ModelEntry(CELL_TYPES.DROPDOWNLIST);
        model.UdM_Indicata = new ModelEntry(CELL_TYPES.DROPDOWNLIST);
        model.Magazzino_del_Prodotto_Selezionato = new ModelEntry(CELL_TYPES.DROPDOWNLIST);
        model.Fr_Cod = {type: 'number'};
        model.Fr_Des = {type: 'string'};
        model.Udm_Cod =  new ModelEntry(CELL_TYPES.DROPDOWNLIST);
        model.Udm_Cod_Trasformato = {type: 'number'};
        model.Udm_Des = new ModelEntry(CELL_TYPES.STRING);
        //Dose_Ha corrisponde a Dose_Ha_Reale, Dose_Ha e Dose
        //della Trattamenti_2
        model.Dose_Ha = {type: 'number'};
        //Dose_Hl corrisponde a Dose_Hl_Reale e Dose_Hl
        //della Trattamenti_2
        model.Dose_Hl = {type: 'number'};
        //DoseTot_Ha corrisponde a Dose_Tot_Reale e Qta_Tot
        //della Trattamenti_2
        model.DoseTot_Ha = {type: 'number'};
        model.Ha_Hl = {type: 'number'};
        model.Dose_QtaTot = {type: 'number'};
        model.Dose_Max = {type: 'number'};
        model.Dose_Etichetta_Max = {type: 'string'};
        model.Dose_Etichetta_Value = {type: 'string'};
        model.Limite_Numero_Trattamenti = {type: 'number'};
        model.Limite_Numero_Trattamenti_UDM_SIM = {type: 'string'};
        model.Limite_Numero_Trattamenti_UDM_COD = {type: 'number'};
        model.IntervalloTrattamenti_Min = {type: 'number'};
        model.IntervalloTrattamenti_Max = {type: 'number'};
        model.strPA_COD = {type: 'string'};
        model.strPA_COD_Pesi = {type: 'string'};
        model.strCLTOSS_COD = {type: 'string'};
        model.For_Veg_Cod = {type: 'string'};
        model.Dose_Fittizia = {type: 'number'};
        model.Giacenza = {type: 'number'};
        model.Lotto = {type: 'string'};
        model.Mat_Regolamento = {type: 'number'};
        model.Mat_Veg_Cod = {type: 'number'};
        model.Mat_Cul_Cod = {type: 'number'};
        model.Sup_Calcolata = {type: 'number'};
        model.Piva = {type: 'string'};
        model.Sa_Cod = {type: 'number'};
        model.Fabbricato_Cod = {type: 'number'};
        model.Fabbricato_Des = {type: 'string'};
        model.Piva_Rif = {type: 'string'};
        model.Sa_Cod_Rif = {type: 'number'};
        model.ID_Agenda_Rif = {type: 'number'};
        model.ID_Mov_Rif = {type: 'number'};
        model.ID_Mov_Det_Rif = {type: 'number'};
        model.Lav_Cod_Rif = {type: 'number'};
        model.Cau_Mov_Rif = {type: 'string'};
        model.Des_Rif = {type: 'string'};
        model.Qta_Rif = {type: 'number'};
        model.flagTipoDose = {type: 'number'};
        model.flagDoseQuantitaTotale = {type: 'number'};
        model.Dose_Acqua = {type: 'number'};
        model.MgO = {type: 'number'};
        model.Visualizza_Solo_Prodotti_in_Giacenza = {type: 'boolean'};
        model.Riga_Salvata = {type: 'boolean'};

        switch(this.Categoria_Magazzino){
            case FORMULATI:
                model.Avversita = new ModelEntry(CELL_TYPES.DROPDOWNLIST);
                model.Av_Cod = {type: 'number'};
                model.Av_Gru = {type: 'string'};
                model.Av_Des = {type: 'string'};
                model.Soglia_Avversita = new ModelEntry(CELL_TYPES.DROPDOWNLIST);
                model.Soglia_Value = {type: 'number'};
                model.Soglia_Des = {type: 'string'};
                model.Dose_Etichetta = new ModelEntry(CELL_TYPES.DROPDOWNLIST);
                model.Dosi_Etichetta = new ModelEntry(CELL_TYPES.DROPDOWNLIST);
                model.Dose_Etichetta_Des = {type: 'string'}; //Nuovo
                model.BufferZone = new ModelEntry(CELL_TYPES.DROPDOWNLIST);
                model.BufferMin = {type: 'number'};
                model.BufferMax = {type: 'number'};
                model.strBuffer = {type: 'string'};
                model.Carenza = {type: 'number'};
                model.Prima_Raccolta = {type: 'string'};
                model.Polverulento = {type: 'number'};
                model.EpocaDPI = new ModelEntry(CELL_TYPES.DROPDOWNLIST);
                break;
            case FERTILIZZANTI:
                model.Efficienza = {type: 'number'};
                model.N = {type: 'number'};
                model.N_Utile = {type: 'number'};
                model.P = {type: 'number'};
                model.K = {type: 'number'};
                model.Cu = {type: 'number'};
                model.EpocaFertilizzazione = new ModelEntry(CELL_TYPES.DROPDOWNLIST);
                model.Utilizza_Direttiva_Nitrati = {type: 'boolean'};
                model.Direttiva_Nitrati = new ModelEntry(CELL_TYPES.DROPDOWNLIST);
                break;
            case SEMENTI:
                model.Cod_Articolo = {type: 'string'};
                model.Opzioni_Semina = new ModelEntry(CELL_TYPES.DROPDOWNLIST);
                break;
        }


        return model;
    }

}

const cartesian =
  (...a) => a.reduce((a, b) => a.flatMap(d => b.map(e => [d, e].flat())));
