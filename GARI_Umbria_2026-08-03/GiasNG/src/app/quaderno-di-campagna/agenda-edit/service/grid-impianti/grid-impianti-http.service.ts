import {Injectable, Injector, OnDestroy} from '@angular/core';
import {forkJoin, from, Observable, of, Subject, Subscription} from 'rxjs';
import {ObjParametriAgendaService} from 'app/Service/obj-parametri-agenda.service';
import {CELL_TYPES, Enum_DBTypeOperation, ObjParametriAgenda, Stati, Tipo_Attivita} from 'gias-ui-kit';
import {
  AbstractGridConfigService,
  CommandsColumnSettings,
  ConfigTemplate,
  CustomColumnSettings,
  DateSettings,
  DropdownListWithForm,
  EditingMode,
  GiasKendoGridComponent,
  GridCustomizations,
  GridPublicService,
  GroupSettings,
  HttpAction,
  KendoGridColumn,
  KendoGridModel,
  KendoGridRow,
  KendoServerResult,
  LoaderType,
  NumericSettings,
  RendererGridEvent,
  SelectableSettings,
  ToolbarSettings
} from 'gias-kendo-grid';
import {CellClickEvent, CellCloseEvent, SelectionEvent} from '@progress/kendo-angular-grid';
import {map, tap} from 'rxjs/operators';
import {
  ImpiantiService,
  IrrigazioneImpianto,
  LeggiImpianto,
  LeggiMacchineIrrigazione
} from 'app/Service/Anagrafica/impianti.service';
import {FormControl, FormGroup, Validators} from '@angular/forms';
import {GridImpiantiService} from './grid-impianti.service';
import {QdCService} from '../qdc.service';
import {TranslocoService} from '@jsverse/transloco';
import {
  CodiciXOperazione,
  GridImpiantoSelezionatoModel
} from '../../quaderno-di-campagna-form/quaderno-di-campagna-form.model';
import {enum_LAVCOD, enum_SEMINA_TIPO, SIMBOLO_M3_HA} from 'app/Model/TipiEnumerativi';
import {Lavorazione} from "../../../../Model/attivita/Lavorazione";
import {FunzioniComuniService} from "../../../../Service/FunzioniComuni.service";
import {Campo} from "../../../../Model/anagrafiche/Campo";
import {AgendaService} from "../../../../Service/Agenda/Agenda.service";
import {UnitaDiMisura} from 'app/Model/metaschema/UnitaDiMisura';
import {
  ConsiglioIrrigazione,
  LeggiConsiglioIrrigazione,
  TipoIrrigazione
} from 'app/Model/attivita/dettagli/DettaglioIrrigazione';
import {AGRODATAFINE, AGRODATAINIZIO} from "../../../../Model/CostantiPersonalizzate";
import {ParcoMacchine} from "../../../../Model/anagrafiche/ParcoMacchine";
import { MisceleService } from '../miscele.service';
import { IrrigazioneAcquaService } from '../irrigazione-acqua.service';
import { ImgBase64Component } from "app/anagrafica/esercizi/imgBase64/img-base64.component";

export class GridImpiantiServerResult extends KendoServerResult {
    constructor(rows: KendoGridRow[], columns: KendoGridColumn[], model: KendoGridModel) {
        super(model, columns, rows);
    }
}

@Injectable()

export class GridImpiantiHttpService extends AbstractGridConfigService<GridImpiantiServerResult> implements OnDestroy {
    gridId: string = "ImpiantiGridId";
    rowId: string = "kendoKey";

    Subs = new Subscription();

    objParametriAgenda: ObjParametriAgenda;

    GridImpianti: GridImpiantiServerResult = new GridImpiantiServerResult(null, null, null);

    //Row della cella cliccata dall'utente
    ClickedRowGridimpianti: any = null;

    //Cella che contiene tutti i Consigli Irrigazione dell'impianto
    ConsigliClickedCell: any[] = [];

    //Cella che contiene tutti i tipi di Irrigazione dell'impianto
    IrrigazioniUtilizzabiliClickedCell: any[] = [];

    loader: LoaderType = LoaderType.SERVICE;
    editingMode: EditingMode = EditingMode.IN_CELL;
    views = new GridCustomizations({ enabled: true });
    impiantiLoaded$ = new Subject<GridImpiantiServerResult>();
    consiglioIrrigazione: ConsiglioIrrigazione[] = [];
    irrigationData = new Map();
    isIrrigazione: boolean = false;
    isFertirrigazione: boolean = false;

    constructor(injector: Injector,
        private impiantiservice: ImpiantiService,
        private objParametriAgendaService: ObjParametriAgendaService,
        private qdcservice: QdCService,
        private gridimpiantiservice: GridImpiantiService,
        private gridpublicService: GridPublicService,
        private translocoService: TranslocoService,
        private funzionicomuniService: FunzioniComuniService,
        private agendaService: AgendaService,
        private misceleservice: MisceleService,
        private irrigazioneAcquaService: IrrigazioneAcquaService) {

        super(injector, ConfigTemplate.DefaultTemplate);

        this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();

        this.toolbar = new ToolbarSettings(false, false);

        this.pagination.gridState.take = 6;

        // Nascondo la colonna Azioni con i bottoni di Info,Modifica e Cancella
        this.cmdColumn = new CommandsColumnSettings({
            editBtn: false,
            infoBtn: false,
            removeBtn: false
        });

        // Setting colonna con checkbox

        //Decido se disabilitare o meno la colonna del checkbox
        //Nel caso di MultiCentro di Ricetta o Brogliaccio in modifica disabilito la colonna checkbox perchè non permetto la
        //selezione/deselezione delle righe degli impianti ma permetto di potere cambiare
        //la superfice trattata
        let disabilita_checkbox: boolean = !this.qdcservice.abilitaGrid;

        if (this.qdcservice.TestataForm.get("MultiCentro").value &&
            this.qdcservice.TestataForm.get("Tipo").value === Tipo_Attivita.Ricetta &&
            (this.qdcservice.TestataForm.get("Stato").value === Stati.Eseguita || this.qdcservice.TestataForm.get("Stato").value === Stati.Da_Eseguire)) {
            disabilita_checkbox = true;
        }

        this.selectable.selectable = new SelectableSettings({ enabled: true, disableAllcheckbox: disabilita_checkbox });

        this.selectable.selectable.checkboxOnly = true;

        this.selectable.shouldShowCheckbox = true;

        this.selectable.preselectedRows.selectionChangeFn = this.selectionChange;

        this.selectable.columnSettings.title = ' ';

        this.selectable.columnSettings.includeInChooser = false;

        this.resizable.autoFitColumns = true;

        this.resizable.isResizable = true;

        this.behavior.createFormGroupFromOutside = true;

        this.columnMenu.kendoGridColumnChooser = true;

        this.behavior.excelSettings.enabled = false;

        this.behavior.pdfSettings.enabled = false;

        this.generalSettings.height = 'auto';

        this.groups = new GroupSettings({ groupable: { enabled: false, showFooter: false } }, this.translocoService);

        let operazioniQdc = this.qdcservice.TestataForm.get("Operazioni").getRawValue();
        if (this.NascondiCheckboxColumn(operazioniQdc, true)) {
            this.selectable.columnSettings.showSelectAll = false;
        } else {
            this.selectable.columnSettings.showSelectAll = true;
        }

        if (operazioniQdc && operazioniQdc.length > 0) {
            this.isFertirrigazione = operazioniQdc.findIndex(o => + o.primaryKey.codice === enum_LAVCOD.FERTIRRIGAZIONE) > -1;
            this.isIrrigazione = operazioniQdc.findIndex(o => + o.primaryKey.codice === enum_LAVCOD.IRRIGAZIONE) > -1;
        } else {
            this.isIrrigazione = false;
            this.isFertirrigazione = false;
        }
        this.customColumn = new CustomColumnSettings({showColumn: this.isIrrigazione, useCustomColumnHeaderTemplate: false, useCustomColumnCellTemplate: true, width: 250});

        //Nascondo il seleziona tutte le colonne se è abilitata l'opzione con frazionamento
        //Sia quando viene cambiato in una sezione di prodotti sia quando viene selezionata una nuova operazione
        this.Subs.add(this.qdcservice.Sezioni_ProdottoFormArray.valueChanges.subscribe((sezioni: Array<any>) => {

            for (let s of sezioni) {
                if (this.NascondiCheckboxColumn([s.Operazione], false)) {
                    this.selectable.columnSettings.showSelectAll = false;

                    break;
                } else {
                    this.selectable.columnSettings.showSelectAll = true;
                }
            }
        }));

        this.Subs.add(this.qdcservice.TestataForm.get("Operazioni").valueChanges.subscribe((operazioni: Lavorazione[]) => {

            let newValueIrrigazione = false;
            let newValueFertirrigazione = false;

            if (this.NascondiCheckboxColumn(operazioni, true)) {
                this.selectable.columnSettings.showSelectAll = false;
            } else {
                this.selectable.columnSettings.showSelectAll = true;
            }
            if (operazioni && operazioni.length > 0) {
                newValueIrrigazione = operazioni.findIndex(o => + o.primaryKey.codice === enum_LAVCOD.IRRIGAZIONE) > -1;
                newValueFertirrigazione = operazioni.findIndex(o => + o.primaryKey.codice === enum_LAVCOD.FERTIRRIGAZIONE) > -1;
            } else {
                newValueIrrigazione = false;
                newValueFertirrigazione = false;
            }

            if (!newValueIrrigazione && this.isIrrigazione){ //l'operazione non è più un'irrigazione
                this.GridImpianti.rows.forEach((row:any) => {
                    this.qdcservice.reimpostaValoriDefaultFertirrigazioneEIrrigazione(row, true);
                    row.UdmDose = SIMBOLO_M3_HA;
                });
            }

            if (!newValueFertirrigazione && this.isFertirrigazione){ //l'operazione non è più una fertirrigazione
                this.GridImpianti.rows.forEach(row => {
                    this.qdcservice.reimpostaValoriDefaultFertirrigazioneEIrrigazione(row, false);
                });
            }

            this.qdcservice.AggiornaFormArrayImpiantiSelezionati();

            this.isIrrigazione = newValueIrrigazione;
            this.isFertirrigazione = newValueFertirrigazione;

            this.customColumn.showColumn = this.isIrrigazione;
            this.customColumn.width = 250;
        }));
    }

    private transformIrrigazioneImpianto(item: IrrigazioneImpianto){
        let codiceStr: string = item.codice;
        let result: any;
        if(codiceStr !== ""){
            let tipoIrrigazione: TipoIrrigazione = null;
            let macchinaIrrigazione: ParcoMacchine = null;
            let codici: string[] = codiceStr.split("_");

            if(codici && codici.length === 2){
                if(item.flagIsMacchina){
                    macchinaIrrigazione = new ParcoMacchine(+codici[1]);
                    macchinaIrrigazione.codice_impianto = item.imp_cod;
                }else if(!item.flagIsMacchina){
                    tipoIrrigazione = new TipoIrrigazione(+codici[0]);
                }

            }
            result = {
                IrrigazioneUtilizzata_Codice: item.codice,
                IrrigazioneUtilizzata_Descrizione: item.descrizione,
                tipoIrrigazione: tipoIrrigazione,
                macchinaIrrigazione: macchinaIrrigazione,
                Efficienza: item.efficienza,
                Portata: item.portata
            };
        }
        return result;
    }

    ngOnDestroy(): void {
        this.Subs.unsubscribe();
    }

    //Se c'è il frazionamento nascondo il CheckboxColumn
    private NascondiCheckboxColumn(Operazioni: Array<Lavorazione>, leggi_impostazione_utente: boolean): boolean {

        let nascondi = false;

        let Opzione_Semina = null;

        if (Operazioni) {
            if (leggi_impostazione_utente) {

                let Operazione_Con_Sementi = Operazioni.find(o => this.qdcservice.Elenco_Operazioni_Sementi.includes(+ o.primaryKey.codice));

                if (Operazione_Con_Sementi) {
                    Opzione_Semina = this.qdcservice.get_Default_Opzione_Semina([Operazione_Con_Sementi]);

                    if (Opzione_Semina && Opzione_Semina.codice === enum_SEMINA_TIPO.Frazionamento_Appezzamenti_perLotto_e_Semina_sui_Singoli_Default) {
                        nascondi = true;
                    }
                }

            } else {

                if (Operazioni.length === 1) {

                    Opzione_Semina = this.qdcservice.getOpzione_Semina_Model(Operazioni[0]);

                    if (Opzione_Semina && Opzione_Semina.codice === enum_SEMINA_TIPO.Frazionamento_Appezzamenti_perLotto_e_Semina_sui_Singoli_Default) {
                        nascondi = true;
                    }

                }

            }
        }

        return nascondi;
    }

    public selectionChange = (event: SelectionEvent, component: GiasKendoGridComponent) => {

        let selectedRows = event.selectedRows.map(e => e.dataItem);

        let deselectedRows = event.deselectedRows.map(e => e.dataItem);

        this.qdcservice.AggiornaRigheSelezionateGriglia(selectedRows, deselectedRows);

    };

    public override onCellClick = (event: CellClickEvent) => {
        //Se la riga non è stata selezionata chiudo la cella
        if (!event.dataItem.Selected) {
            event.sender.closeRow(event.rowIndex);
        }else{
            // Keep a reference to the clicked row and snapshot the previous date values so we can revert if needed
            this.ClickedRowGridimpianti = event.dataItem;
            try {
                // store previous values as non-enumerable just in case
                (event.dataItem as any).__prev_DataFineIrrigazione = event.dataItem.DataFineIrrigazione;
                (event.dataItem as any).__prev_DataInizioIrrigazione = event.dataItem.DataInizioIrrigazione;
            } catch (e) { /* ignore */ }
        }
    };

    public override onCellClose = (event: CellCloseEvent, inputElementRef) => {
        this.gridimpiantiservice.GridImpiantionCellClose(event, inputElementRef,this.ConsigliClickedCell,this.IrrigazioniUtilizzabiliClickedCell);
    };

    perform(actionType: HttpAction, items: any): Observable<any[]> {
        return from([]);
    }

    read(): Observable<GridImpiantiServerResult> {

        if (this.qdcservice.getImpresa_Model().partitaIva === '0') {
            this.GridImpianti = new GridImpiantiServerResult([],
                this.setColumnsGridImpianti(),
                this.setModelGridImpianti()
            );
            return of(this.GridImpianti);
        }

        this.loadingService.set_isLoading({ isLoading: true, message: '', component: this.gridPublicService.gridElRef });

        let Veg_Cod = 0;

        let Dest_Cod = 0;

        if (this.qdcservice.TestataForm.get('Specie').value !== undefined &&
            this.qdcservice.TestataForm.get('Specie').value !== null) {
            if (this.qdcservice.TestataForm.get('Specie').value.classType !== undefined &&
                this.qdcservice.TestataForm.get('Specie').value.classType !== null &&
                this.qdcservice.TestataForm.get('Specie').value.classType === 'DestinazioneUso') {

                Dest_Cod = this.qdcservice.TestataForm.get('Specie').value.codice;
            } else {
                Veg_Cod = this.qdcservice.TestataForm.get('Specie').value.codice;
            }
        }

        let id_agenda_list: number[] = [];

        let ricetta_operazione_cod_list: number[] = [];

        if (this.objParametriAgenda.TipoOperazioneDB !== Enum_DBTypeOperation.Write) {
            if (this.qdcservice.TestataForm.get("Tipo").getRawValue() === Tipo_Attivita.QuadernoDiCampagna) {
                id_agenda_list = (this.qdcservice.TestataForm.get("Codici_Attivita").value as Array<CodiciXOperazione>).filter(co => co.CodiceAttivita !== "" && co.CodiceAttivita !== "0").map(co => + co.CodiceAttivita);
            } else if (this.qdcservice.TestataForm.get("Tipo").getRawValue() === Tipo_Attivita.Ricetta) {
                ricetta_operazione_cod_list = (this.qdcservice.TestataForm.get("Codici_Attivita").value as Array<CodiciXOperazione>).filter(co => co.CodiceOperazioneRicetta !== "" && co.CodiceOperazioneRicetta !== "0").map(co => + co.CodiceOperazioneRicetta);
            }
        }

        //Va bene nella lettura degli impianti passare il centro aziendale perchè viene utilizzato come filtro per gli impianti

        const leggiImpiantoParams = <LeggiImpianto>{
            disciplinare: this.qdcservice.getDisciplinareModelValue(0),
            direttiva_nitrati: this.qdcservice.getDisciplinareModelValue(enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI),
            utilizzoTerreno: this.qdcservice.TestataForm.get('Specie').value,
            centroAziendale: this.qdcservice.TestataForm.get('Centro_Aziendale').value,
            lavorazioni: this.qdcservice.TestataForm.get('Operazioni').getRawValue(),
            campo: (this.qdcservice.TestataForm.get('Campo').value as Campo),
            data: this.qdcservice.TestataForm.get('Data').value,
            consideraTerrenoNudo: this.qdcservice.getCONSIDERATERRENONUDO(),
            dettagliTerrenoNudo: this.qdcservice.getCONSIDERATERRENONUDO(),
            id_agenda_list: id_agenda_list,
            tipo_ricetta: this.qdcservice.TestataForm.get('TipoRicetta').value,
            tipo_operazione_db: this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB,
            veg_cod: Veg_Cod,
            dest_cod: Dest_Cod,
            tipo_attivita: this.qdcservice.TestataForm.get('Tipo').value,
            stato: this.qdcservice.TestataForm.get('Stato').value,
            ricetta_operazione_cod_list: ricetta_operazione_cod_list
        };

        const obs: Promise<any>[] = [
            this.impiantiservice.CaricaImpianti(leggiImpiantoParams)
        ];

        return forkJoin(obs).pipe(map(result => {
            let rows = result[0];
            let Operazioni: Array<Lavorazione> = this.qdcservice.TestataForm.get("Operazioni").getRawValue();

            let operazioneIsFertirrigazione: boolean = false;
            let operazioneIsIrrigazione: boolean = false;

            if (Operazioni && Operazioni.length > 0) {
                operazioneIsFertirrigazione = Operazioni.findIndex(o => + o.primaryKey.codice === enum_LAVCOD.FERTIRRIGAZIONE) > -1;
                operazioneIsIrrigazione = Operazioni.findIndex(o => + o.primaryKey.codice === enum_LAVCOD.IRRIGAZIONE) > -1;
            }

            let ddlUdmAggiornata: boolean = false;
            let udmIrrigazione: string = '';

            if (rows && this.qdcservice.ImpiantiSelezionatiFormArray.getRawValue().length > 0) {
                rows.forEach((r: any) => {
                    //Seleziono sulla grid gli Impianti in base al formArray ImpiantiSelezionati
                    this.qdcservice.ImpiantiSelezionatiFormArray.getRawValue().forEach((i: GridImpiantoSelezionatoModel) => {
                    if (r.PIVA === i.PIVA &&
                      r.SA_COD === i.SA_COD &&
                      r.APPEZZA === i.APPEZZA &&
                      r.ID_REG === i.ID_REG &&
                      r.Progetto_Cod === i.Progetto_Cod) {

                      //Valorizzo anche la Finalità e la Copertura
                      //che mi arrivano lato server. Perchè serviranno per la
                      //lettura dei Prodotti.

                      i.GRFI_COD = r.GRFI_COD;

                      i.Grfi_Des = r.Grfi_Des;

                      i.Cop_Cod = r.Cop_Cod;

                      i.Copertura = r.Copertura;

                      i.Flag_Protetto = r.Flag_Protetto;

                      i.CUL_COD = r.CUL_COD;

                      i.Cul_Des = r.Cul_Des;

                      i.ListaClassiTessitura = r.ListaClassiTessitura;

                      i.Obj_Disciplinare = r.Obj_Disciplinare;

                      i.Regolamento_Cod = r.Regolamento;

                      i.App_Nome = r.App_Nome_Anagrafica;

                      i.Progetto_Des = r.Progetto;

                      i.Stato_Cod = r.Stato_Cod;

                      i.Validita_Inizio_Distinta = r.Validita_Inizio_Distinta_Date;

                      i.Validita_Fine_Distinta = r.Validita_Fine_Distinta_Date;

                      i.Data_Raccolta = r.Data_Raccolta_Date;

                      i.Data_Raccolta_Prevista = r.Data_Raccolta_Prevista_Date;

                      i.CarenzaStr = r.CarenzaStr;

                      i.DataCarenza = r.DataCarenza;

                      i.Sup_Imp = r.Sup_Imp;

                      i.DistBZ_CorpiIdrici = r.DistBZ_CorpiIdrici;

                      i.DistBZ_AreeResPub = r.DistBZ_AreeResPub;

                      i.DistBZ_Allevamenti = r.DistBZ_Allevamenti;

                      i.DistBZ_VegNatNonColt = r.DistBZ_VegNatNonColt;

                      i.SupBZ_Riduzione = r.SupBZ_Riduzione;

                      if (r.N_Max !== "") {
                        i.flag_N_Max = true;
                      } else {
                        i.flag_N_Max = false;
                      }

                      if (r.P_Max !== "") {
                        i.flag_P_Max = true;
                      } else {
                        i.flag_P_Max = false;
                      }

                      if (r.K_Max !== "") {
                        i.flag_K_Max = true;
                      } else {
                        i.flag_K_Max = false;
                      }

                      if (r.Mg_Max !== "") {
                        i.flag_Mg_Max = true;
                      } else {
                        i.flag_Mg_Max = false;
                      }

                      if (r.CU_Max !== "") {
                        i.flag_CU_Max = true;
                      } else {
                        i.flag_CU_Max = false;
                      }

                      i.N_Max = r.N_Max_Decimal;

                      i.P_Max = r.P_Max_Decimal;

                      i.K_Max = r.K_Max_Decimal;

                      i.Mg_Max = r.Mg_Max_Decimal;

                      i.CU_Max = r.CU_Max_Decimal;

                      i.N_Residuo = r.N_Residuo_Decimal;

                      i.P_Residuo = r.P_Residuo_Decimal;

                      i.K_Residuo = r.K_Residuo_Decimal;

                      i.Mg_Residuo = r.Mg_Residuo_Decimal;

                      i.CU_Residuo = r.CU_Residuo_Decimal;

                      i.N_Residuo_Percentuale = r.N_Residuo_Percentuale;

                      i.P_Residuo_Percentuale = r.P_Residuo_Percentuale;

                      i.K_Residuo_Percentuale = r.K_Residuo_Percentuale;

                      i.Mg_Residuo_Percentuale = r.Mg_Residuo_Percentuale;

                      i.CU_Residuo_Percentuale = r.CU_Residuo_Percentuale;

                      //Imposto la Superficie Trattata, la Bufferzone e la Mitigazione Deriva da quella dell'impianto selezionato
                      r.Sup_Imp_help = this.funzionicomuniService.roundNumber(i.Sup_Imp_help,this.qdcservice.get4DecimalNumericSettings().decimals);
                      r.Sup_Riduzione_BufferZone = this.funzionicomuniService.roundNumber(i.Sup_Riduzione_BufferZone,this.qdcservice.get4DecimalNumericSettings().decimals);
                      r.Perc_Riduzione_Deriva = this.funzionicomuniService.roundNumber(i.Perc_Riduzione_Deriva, 2);

                      let aperturaOperazioneDaAnagraficaImpianto = this.qdcservice.Apertura_QdC
                        && this.qdcservice.Apertura_QdC.flag_QdC_Aperta_da_Altra_Pagina
                        && this.qdcservice.Apertura_QdC.flag_Selezione_Impianti;

                      if (!aperturaOperazioneDaAnagraficaImpianto){
                        r.IrrigazioneUtilizzata_Codice = i.IrrigazioneUtilizzata_Codice;
                        r.IrrigazioneUtilizzata_Descrizione = i.IrrigazioneUtilizzata_Descrizione;
                        r.tipoIrrigazione = i.tipoIrrigazione;
                        r.macchinaIrrigazione = i.macchinaIrrigazione;
                        r.UdmDose = i.UdmDose;
                        r.DoseAcquaGiornaliera = i.DoseAcquaGiornaliera;
                        r.OreIrrigazione = i.OreIrrigazione;
                        r.Portata = i.Portata;
                        r.DataInizioIrrigazione = i.DataInizioIrrigazione;
                        r.DataFineIrrigazione = i.DataFineIrrigazione;
                        r.FrequenzaIrrigazioneMedia = i.FrequenzaIrrigazioneMedia;
                        r.Efficienza = i.Efficienza;
                        r.DataConsiglio = i.DataConsiglio;
                        r.Consiglio_Codice = i.Consiglio_Codice;
                        r.Consiglio_Descrizione = i.Consiglio_Descrizione;
                        r.DoseAcquaConsiglio = i.DoseAcquaConsiglio;
                        r.UdmConsiglio = i.UdmConsiglio;
                        r.minDataTurno = i.minDataTurno;
                        r.maxDataTurno = i.maxDataTurno;
                      }

                      if (operazioneIsFertirrigazione){
                          this.irrigazioneAcquaService.calculateFertirrigationValuesFromDoseAcqua(r, r.DoseAcquaGiornaliera, r.Sup_Imp_help, r.DataInizioIrrigazione, r.DataFineIrrigazione, r.Efficienza);
                      } else if (operazioneIsIrrigazione){
                        this.irrigazioneAcquaService.calculateIrrigationWaterValues(r, r.DoseAcquaGiornaliera, r.Sup_Imp_help, r.DataInizioIrrigazione, r.DataFineIrrigazione, r.Efficienza);
                        if (!ddlUdmAggiornata && r.DoseAcquaGiornaliera > 0){ //per le irrigazioni è necessario aggiornare il valore della ddl sopra la griglia
                            udmIrrigazione = r.UdmDose;
                            this.gridimpiantiservice.aggiornaUdmDoseIrrigazioneGridImpiantiComponent(udmIrrigazione);
                            ddlUdmAggiornata = true;
                        }
                      }

                      r.Selected = true;
                    }
                  });

                });

                if (operazioneIsIrrigazione && udmIrrigazione !== ''){
                    rows.forEach(row => {
                        if (row.DoseAcquaGiornaliera === 0){ //il valore di default che proviene dal BE per impianti non irrigati è m3/Ha
                            row.UdmDose = udmIrrigazione;
                        }
                    });
                }
                rows = this.RiordinaRigheGridImpianti(rows);

                this.gridimpiantiservice.EventiPostselectionChangeGridImpianti(rows, false);
            }

            this.GridImpianti.rows = <KendoGridRow[]>(rows);

            //Per le griglie con le viste e che devono essere ricaricate viene fatto questo if per le colonne
            //per evitare che vengano sovrascritte con le colonne che si erano impostate all'inizio

            if (!this.gridPublicService?.giasGridComponent?.columns) {
                this.GridImpianti.columns = this.setColumnsGridImpianti();
            } else {
                this.GridImpianti.columns = this.gridPublicService?.giasGridComponent?.columns;
            }

            if (Operazioni && Operazioni.length > 0) {
                //Nascondo le colonne Sup Riduzione e Mitigazione Deriva se non sto facendo un trattamento

                let index = Operazioni.findIndex(o => this.qdcservice.Elenco_Operazioni_Formulati.includes(+ o.primaryKey.codice));

                if (index === -1) {

                    this.GridImpianti.columns.forEach(c => {
                        if (c.field === "Perc_Riduzione_Deriva" || c.field === "Sup_Riduzione_BufferZone") {
                            c.hidden = true;
                        }
                    });
                } else {
                    this.GridImpianti.columns.forEach(c => {
                        if (c.field === "Perc_Riduzione_Deriva" || c.field === "Sup_Riduzione_BufferZone") {
                            c.hidden = false;
                        }
                    });
                }

                //Mostro la colonna della 'Data utile Prima Raccolta' solo se siamo in una raccolta
                index = Operazioni.findIndex(o => + o.primaryKey.codice === enum_LAVCOD.RILIEVO_PRODUZIONE_DATA_RACCOLTA);

                if (index > -1) {
                    let CarenzaStr = rows.find(r => r.CarenzaStr && r.CarenzaStr !== "");
                    if (CarenzaStr !== undefined) {
                        this.GridImpianti.columns.forEach(c => {
                            if (c.field === "CarenzaStr") {
                                c.hidden = false;
                                c.includeInChooser = true;
                            }
                        });
                    } else {
                        this.GridImpianti.columns.forEach(c => {
                            if (c.field === "CarenzaStr") {
                                c.hidden = true;
                                c.includeInChooser = true;
                            }
                        });
                    }
                } else {
                    this.GridImpianti.columns.forEach(c => {
                        if (c.field === "CarenzaStr") {
                            c.includeInChooser = false;
                            c.hidden = true;
                        }
                    });
                }

                //Mostro la colonna della 'Classe tessitura' solo se siamo in una distribuzione ammendanti
                index = Operazioni.findIndex(o => + o.primaryKey.codice === enum_LAVCOD.DISTRIBUZIONE_AMMENDANTI);

                if (index > -1) {
                    this.GridImpianti.columns.forEach(c => {
                        if (c.field === "Str_ClasseTessitura") {
                            c.hidden = false;
                            c.includeInChooser = true;
                        }
                    });
                } else {
                    this.GridImpianti.columns.forEach(c => {
                        if (c.field === "Str_ClasseTessitura") {
                            c.hidden = true;
                            c.includeInChooser = false;
                        }
                    });
                }



                if (operazioneIsFertirrigazione || operazioneIsIrrigazione) {

                    this.GridImpianti.columns.forEach(c => {
                        if (c.field === "IrrigazioneUtilizzata_Codice" || //|| c.field === "Consiglio_Codice" || c.field === "DataConsiglio" || c.field === "DoseAcquaConsiglio"
                            c.field === "UdmDose" || c.field === "DoseAcquaGiornaliera" || c.field === "OreIrrigazione" ||
                            c.field === "Portata" || c.field === "Efficienza" || c.field === "QtaTotaleAcquaGiornaliera" || c.field === "QtaTotaleAcquaAssorbitaGiornaliera" ||
                            c.field === "DataInizioIrrigazione" || c.field === "DataFineIrrigazione" || c.field === "FrequenzaIrrigazioneMedia" || c.field === "QtaTotaleAcquaUtilizzataPeriodo" || c.field === "QtaTotaleAcquaAssorbitaPeriodo") {

                        c.hidden = false;
                        c.includeInChooser = true;
                        }
                    });

                    if (operazioneIsFertirrigazione){

                        this.GridImpianti.columns.forEach(c => {
                            if (c.field === "Consiglio_Codice" || c.field === "DataConsiglio" || c.field === "DoseAcquaConsiglio"){
                                c.hidden = true;
                                c.includeInChooser = false;
                            }
                        });

                        let doseAcquaIndex = this.GridImpianti.columns.findIndex(c => c.field === "DoseAcquaGiornaliera");
                        if (doseAcquaIndex > -1){
                            this.GridImpianti.columns[doseAcquaIndex].editable = false;
                        }
                        let dataInizioIrrigazioneIndex = this.GridImpianti.columns.findIndex(c => c.field === "DataInizioIrrigazione");
                        if (dataInizioIrrigazioneIndex > -1){
                            this.GridImpianti.columns[dataInizioIrrigazioneIndex].editable = false;
                        }
                    } else if (operazioneIsIrrigazione){

                        if (this.qdcservice.TestataForm.get("Tipo").value === Tipo_Attivita.QuadernoDiCampagna){
                            this.GridImpianti.columns.forEach(c => {
                                if (c.field === "Consiglio_Codice" || c.field === "DataConsiglio" || c.field === "DoseAcquaConsiglio"){
                                    c.hidden = false;
                                    c.includeInChooser = true;
                                }
                            });
                        }

                        let doseAcquaIndex = this.GridImpianti.columns.findIndex(c => c.field === "DoseAcquaGiornaliera");
                        if (doseAcquaIndex > -1){
                            this.GridImpianti.columns[doseAcquaIndex].editable = true;
                        }
                        let dataInizioIrrigazioneIndex = this.GridImpianti.columns.findIndex(c => c.field === "DataInizioIrrigazione");
                        if (dataInizioIrrigazioneIndex > -1){
                            this.GridImpianti.columns[dataInizioIrrigazioneIndex].editable = true;
                        }
                    }
                } else {
                  this.GridImpianti.columns.forEach(c => {
                    if (c.field === "IrrigazioneUtilizzata_Codice" || c.field === "Consiglio_Codice" || c.field === "DataConsiglio" ||
                      c.field === "DoseAcquaConsiglio" || c.field === "UdmDose" || c.field === "DoseAcquaGiornaliera" || c.field === "OreIrrigazione" ||
                      c.field === "Portata" || c.field === "Efficienza" || c.field === "QtaTotaleAcquaGiornaliera" || c.field === "QtaTotaleAcquaAssorbitaGiornaliera" ||
                      c.field === "DataInizioIrrigazione" || c.field === "DataFineIrrigazione" || c.field === "FrequenzaIrrigazioneMedia" || c.field === "QtaTotaleAcquaUtilizzataPeriodo"|| c.field === "QtaTotaleAcquaAssorbitaPeriodo") {

                      c.hidden = true;
                      c.includeInChooser = false;
                    }
                  });
                }
            }

            //Potrebbe succedere che ci sono dei Poligoni nel GIS per quegli impianti ma che non è stata generata la StaticMap
            //(la static Map viene generata solo se è abilitata una chiave sul db dato che ha un costo generarla e se questa chiave è abilitata la Static map viene generata alla creazione del Poligono)
            let GIS = rows.find(r => r.StaticMapBase64String && r.StaticMapBase64String !== "");
            if (GIS !== undefined) {
                this.GridImpianti.columns.forEach(c => {
                    if (c.field === "StaticMapBase64String") {
                        c.hidden = false;
                    }
                });
            } else {
                this.GridImpianti.columns.forEach(c => {
                    if (c.field === "StaticMapBase64String") {
                        c.hidden = true;
                    }
                });
            }

            if (!this.gridPublicService?.giasGridComponent?.model) {
                this.GridImpianti.model = this.setModelGridImpianti();
            } else {
                this.GridImpianti.model = this.gridPublicService?.giasGridComponent?.model;
            }

            this.GridImpianti = new GridImpiantiServerResult(this.GridImpianti.rows,
                this.GridImpianti.columns,
                this.GridImpianti.model
            );

            if (this.qdcservice.Sezioni_ProdottoFormArray.getRawValue() && this.qdcservice.Sezioni_ProdottoFormArray.getRawValue().length > 0 && !this.qdcservice.Mostra_Sezioni_Prodotto) {
                this.qdcservice.Mostra_Sezioni_Prodotto = true;
            }

            if (this.qdcservice.Sezioni_Senza_ProdottoFormArray.getRawValue() && this.qdcservice.Sezioni_Senza_ProdottoFormArray.getRawValue().length > 0 && !this.qdcservice.Mostra_Sezioni_Senza_Prodotto) {
                this.qdcservice.Mostra_Sezioni_Senza_Prodotto = true;
            }

            this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });

            return this.GridImpianti;
        }),tap(x => this.impiantiLoaded$.next(x)));
    }

    defaultConsiglioIrrigazione: any = {
        Consiglio_Codice: 0,
        Consiglio_Descrizione: this.translocoService.translate("Nessuno"),
        DataConsiglio: AGRODATAINIZIO,
        DoseAcquaConsiglio: 0,
        UdmConsiglio: null,
        minDataTurno: null,
        maxDataTurno: null
    };

    public loadConsiglioForImpianto(impianto: any): Observable<ConsiglioIrrigazione[]> {

      this.ConsigliClickedCell = [];

      if(this.ClickedRowGridimpianti){

        const consiglioParam: LeggiConsiglioIrrigazione = {
          data: this.qdcservice.TestataForm.get("Data").getRawValue(),
          piva: this.ClickedRowGridimpianti.PIVA,
          sa_cod: this.ClickedRowGridimpianti.SA_COD,
          appezza: this.ClickedRowGridimpianti.APPEZZA,
          id_reg: this.ClickedRowGridimpianti.ID_REG,
          progetto_cod: this.ClickedRowGridimpianti.Progetto_Cod
        };

        return from(this.impiantiservice.CaricaConsiglio(consiglioParam)).pipe(
          map(result => {

            this.ConsigliClickedCell = result.map((item: ConsiglioIrrigazione) => {
              return{
                Consiglio_Codice: item.codice,
                Consiglio_Descrizione: item.descrizione,
                DataConsiglio: item.dataConsiglio,
                DoseAcquaConsiglio: item.qtaAcqua,
                UdmConsiglio: item.unitaDiMisura,
                minDataTurno: item.minDataTurno,
                maxDataTurno: item.maxDataTurno
              };

            });

            this.ConsigliClickedCell.unshift({
              Consiglio_Codice: 0,
              Consiglio_Descrizione: this.translocoService.translate("Nessuno"),
              DataConsiglio: AGRODATAINIZIO,
              DoseAcquaConsiglio: 0,
              UdmConsiglio: null,
              minDataTurno: null,
              maxDataTurno: null
            });

            return this.ConsigliClickedCell;
          })
        );
      }
    }

    public loadIrrigazioneUtilizzataForImpianto(impianto?: any): Observable<any[]> {

      this.IrrigazioniUtilizzabiliClickedCell = [];

      if(this.IrrigazioniUtilizzabiliClickedCell){

        let leggiMacchine: LeggiMacchineIrrigazione;
        if (impianto && impianto.hasOwnProperty('PIVA') && impianto.hasOwnProperty('SA_COD') && impianto.hasOwnProperty('APPEZZA') && impianto.hasOwnProperty('ID_REG')) {
            leggiMacchine = {
                piva: impianto.PIVA,
                sa_cod: impianto.SA_COD,
                appezza: impianto.APPEZZA,
                id_reg: impianto.ID_REG
            };
        } else {
            leggiMacchine = {
                piva: this.ClickedRowGridimpianti.PIVA,
                sa_cod: this.ClickedRowGridimpianti.SA_COD,
                appezza: this.ClickedRowGridimpianti.APPEZZA,
                id_reg: this.ClickedRowGridimpianti.ID_REG
            };
        }

        return from(this.impiantiservice.Leggi_Macchine_Irrigazione_Impianto(leggiMacchine)).pipe(
          map((Macchine: IrrigazioneImpianto[]) => {

            this.IrrigazioniUtilizzabiliClickedCell = Macchine.map((item: IrrigazioneImpianto) => {

              let CodiceStr: string = item.codice;

              if(CodiceStr !== ""){

                let tipoIrrigazione: TipoIrrigazione = null;

                let macchinaIrrigazione: ParcoMacchine = null;

                let portata: number = item.portata;

                let efficienza: number = item.efficienza;

                let Codici: string[] = CodiceStr.split("_");

                let IrrigazioneUtilizzata_Descrizione: string = "";

                if(Codici && Codici.length === 2){
                  if(item.flagIsMacchina){
                    macchinaIrrigazione = new ParcoMacchine(+Codici[1]);
                    macchinaIrrigazione.codice_impianto = item.imp_cod;
                  }else if(!item.flagIsMacchina){
                    tipoIrrigazione = new TipoIrrigazione(+Codici[0]);
                    efficienza = 100;
                  }

                  IrrigazioneUtilizzata_Descrizione = this.qdcservice.getIrrigazioneUtilizzata_Descrizione(item.flagIsMacchina,item.descrizione);
                }

                return{
                  IrrigazioneUtilizzata_Codice: item.codice,
                  IrrigazioneUtilizzata_Descrizione: IrrigazioneUtilizzata_Descrizione,
                  tipoIrrigazione: tipoIrrigazione,
                  macchinaIrrigazione: macchinaIrrigazione,
                  Efficienza: efficienza,
                  Portata: portata
                };

              }

            });

            return this.IrrigazioniUtilizzabiliClickedCell;
          })
        );
      }
    }

    override applyRendererRules(opts: RendererGridEvent): void {
        const { grid, gridElRef } = { ...opts };
        let rows: [] = grid.data['data'];
        this.qdcservice.GridImpiantiElementRefRows = gridElRef.nativeElement.querySelectorAll('tbody tr');

        this.qdcservice.GestisciColoreRigheGridImpianti();
    }

    setColumnsGridImpianti() {

        const numericsettings=this.qdcservice.get4DecimalNumericSettings();
        numericsettings.autoCorrect = true;

        const percentagesettings = new NumericSettings();
        percentagesettings.min = 0;
        percentagesettings.max = 100;
        percentagesettings.autoCorrect = true;
        percentagesettings.format = "#.##\\%";

        const efficienzaSettings = new NumericSettings();
        efficienzaSettings.min = 0.01;
        efficienzaSettings.max = 100;
        efficienzaSettings.autoCorrect = true;
        efficienzaSettings.format = "#.##\\%";

        const numericsettingDoseAcquaConsiglio = new NumericSettings();
        numericsettingDoseAcquaConsiglio.autoCorrect = false;
        numericsettingDoseAcquaConsiglio.format = "n2";
        numericsettingDoseAcquaConsiglio.decimals = 2;

        const numericSettingsFrequenzaIrrigazioneMedia = new NumericSettings();
        numericSettingsFrequenzaIrrigazioneMedia.autoCorrect = true;
        numericSettingsFrequenzaIrrigazioneMedia.min = 1;
        numericSettingsFrequenzaIrrigazioneMedia.format = "n0";
        numericSettingsFrequenzaIrrigazioneMedia.decimals = 0;
        numericSettingsFrequenzaIrrigazioneMedia.defaultValue = 1;

        const numericSettingsPortata = new NumericSettings();
        numericSettingsPortata.autoCorrect = true;
        numericSettingsPortata.min = 0;
        numericSettingsPortata.format = "n0";
        numericSettingsPortata.decimals = 0;
        numericSettingsPortata.defaultValue = 0;

        const numericSettingsDoseAcquaGiornaliera = new NumericSettings();
        numericSettingsDoseAcquaGiornaliera.autoCorrect = true;
        numericSettingsDoseAcquaGiornaliera.min = 0;
        numericSettingsDoseAcquaGiornaliera.format = "n6";
        numericSettingsDoseAcquaGiornaliera.decimals = 6;
        numericSettingsDoseAcquaGiornaliera.defaultValue = 0;

        const numericSettingsOreIrrigazione = new NumericSettings();
        numericSettingsOreIrrigazione.autoCorrect = true;
        numericSettingsOreIrrigazione.min = 0;
        numericSettingsOreIrrigazione.format = "n6";
        numericSettingsOreIrrigazione.decimals = 6;
        numericSettingsOreIrrigazione.defaultValue = 0;

        const supImpHelpNumericSettings = new NumericSettings({
            defaultValue: 0,
            format: 'n4',
            min: 0,
            step: 0.0001,
            decimals: 4
        });

        const supRiduzioneBufferZoneNumericSettings = new NumericSettings({
            defaultValue: 0,
            format: 'n4',
            min: 0,
            step: 0.0001,
            decimals: 4
        });

        const datesettings = new DateSettings();
        datesettings.format = "dd/MM/yyyy";

        const columns: Array<KendoGridColumn> = [
            new KendoGridColumn({ field: 'StaticMapBase64String', title: this.translocoService.translate('Poligono') }, { resizable: true, filterable: false, editable: false , hidden: true, component: ImgBase64Component }),
            new KendoGridColumn({ field: 'rag_soc', title: this.translocoService.translate('Ragione_Sociale1') }, { hidden: true, resizable: true, editable: false }),
            new KendoGridColumn({ field: 'sa_nome', title: this.translocoService.translate('CentroAziendale') }, { hidden: true, resizable: true, editable: false }),
            new KendoGridColumn({ field: 'Campo_Des', title: this.translocoService.translate('Campo') }, { hidden: true, resizable: true, editable: false }),
            new KendoGridColumn({ field: 'APP_NOME', title: this.translocoService.translate('AppezzamentoAbbr') }, { hidden: true, resizable: true, editable: false, showHTMLAsString: true }),
            new KendoGridColumn({ field: 'CodBioApp', title: this.translocoService.translate('AppBioCod') }, { hidden: true, resizable: true, editable: false }),
            new KendoGridColumn({ field: 'catasto', title: this.translocoService.translate('Catasto') }, { hidden: true, resizable: true, editable: false, showHTMLAsString: true }),
            new KendoGridColumn({ field: 'Fase_Corrente', title: this.translocoService.translate('FaseFenologicaCorrente') }, { hidden: true, resizable: true, editable: false }),
            new KendoGridColumn({ field: 'Codici_Anagrafe_Des', title: this.translocoService.translate('DestinazioneDUso') }, { hidden: true, resizable: true, editable: false }),
            new KendoGridColumn({ field: 'Cul_Des', title: this.translocoService.translate('Varietà') }, { hidden: true, resizable: true, editable: false }),
            new KendoGridColumn({ field: 'Grva_Des', title: this.translocoService.translate('GruppoVarietale') }, { hidden: true, resizable: true, editable: false }),
            new KendoGridColumn({ field: 'Sup_Imp', title: this.translocoService.translate('SuperficieAbbrHa') }, { resizable: true, editable: false, format: '{0:n4}', numeric: numericsettings }),
            new KendoGridColumn({ field: "Sup_Riduzione_BufferZone", title: this.translocoService.translate('SuperficieRiduzioneBuffer') }, { resizable: true, editable: this.qdcservice.abilitaGrid, format: '{0:n4}', numeric: supRiduzioneBufferZoneNumericSettings, includeInChooser: false }),
            new KendoGridColumn({ field: "Perc_Riduzione_Deriva", title: this.translocoService.translate('PercentualeRiduzioneDeriva') }, { resizable: true, editable: this.qdcservice.abilitaGrid, format: '{0:#.##\\%}', numeric: percentagesettings, includeInChooser: false }),
            new KendoGridColumn({ field: 'Sup_Imp_help', title: this.translocoService.translate('SuperficieTrattata') }, { resizable: true, editable: this.qdcservice.abilitaGrid, format: '{0:n4}', numeric: supImpHelpNumericSettings, includeInChooser: false }),
            new KendoGridColumn({ field: 'IrrigazioneUtilizzata_Codice', title: this.translocoService.translate('IrrigazioneUtilizzata') }, { hidden: true, includeInChooser: false, resizable: true, editable: true }),
            new KendoGridColumn({ field: 'Consiglio_Codice', title: this.translocoService.translate('Consiglio') }, { hidden: true,includeInChooser: false, resizable: true, editable: true }),
            new KendoGridColumn({ field: 'DataConsiglio', title: this.translocoService.translate('DataConsiglio') }, { hidden: true,includeInChooser: false, resizable: true, editable: false, format: '{0:dd/MM/yyyy}',date: datesettings }),
            new KendoGridColumn({ field: 'DoseAcquaConsiglio', title: this.translocoService.translate('DoseAcquaConsiglio') }, { hidden: true, includeInChooser: false, resizable: true, editable: false, format: '{0:n2}', numeric: numericsettingDoseAcquaConsiglio }),
            new KendoGridColumn({ field: 'UdmDose', title: this.translocoService.translate('UdmDose') }, { hidden: true, includeInChooser: false, resizable: true, editable: false }),
            new KendoGridColumn({ field: 'DoseAcquaGiornaliera', title: this.translocoService.translate('DoseAcquaGiornaliera') }, { hidden: true, includeInChooser: false, resizable: true, editable: false, format: '{0:n6}', numeric: numericSettingsDoseAcquaGiornaliera }),
            new KendoGridColumn({ field: 'OreIrrigazione', title: this.translocoService.translate('OreIrrigazione') }, { hidden: true, includeInChooser: false, resizable: true, editable: true, format: '{0:n6}', numeric: numericSettingsOreIrrigazione }),
            new KendoGridColumn({ field: 'Portata', title: this.translocoService.translate('Portata') }, { hidden: true, includeInChooser: false, resizable: true, editable: true, format: '{0:n0}', numeric: numericSettingsPortata }),
            new KendoGridColumn({ field: 'Efficienza', title: this.translocoService.translate('Efficienza') }, { hidden: true,includeInChooser: false, resizable: true, editable: true, format: '{0:#.##\\%}', numeric: efficienzaSettings }),
            new KendoGridColumn({ field: 'QtaTotaleAcquaGiornaliera', title: this.translocoService.translate('QtaTotaleAcquaGiornaliera') }, { hidden: true,includeInChooser: false, resizable: true, editable: false, format: '{0:n4}', numeric: numericsettings }),
            new KendoGridColumn({ field: 'QtaTotaleAcquaAssorbitaGiornaliera', title: this.translocoService.translate('QtaTotaleAcquaAssorbitaGiornaliera') }, { hidden: true,includeInChooser: false, resizable: true, editable: false, format: '{0:n4}', numeric: numericsettings }),
            new KendoGridColumn({ field: 'DataInizioIrrigazione', title: this.translocoService.translate('DataInizioIrrigazione') }, { hidden: true,includeInChooser: false, resizable: true, editable: false, validators: [Validators.required] }),
            new KendoGridColumn({ field: 'DataFineIrrigazione', title: this.translocoService.translate('DataFineIrrigazione') }, { hidden: true,includeInChooser: false,resizable: true, editable: true, validators: [Validators.required] }),
            new KendoGridColumn({ field: 'FrequenzaIrrigazioneMedia', title: this.translocoService.translate('FrequenzaIrrigazioneMedia') }, {  hidden: true,includeInChooser: false, resizable: true, editable: true, numeric: numericSettingsFrequenzaIrrigazioneMedia }),
            new KendoGridColumn({ field: 'QtaTotaleAcquaUtilizzataPeriodo', title: this.translocoService.translate('QtaTotaleAcquaUtilizzataPeriodo') }, {  hidden: true,includeInChooser: false,resizable: true, editable: false, format: '{0:n4}', numeric: numericsettings }),
            new KendoGridColumn({ field: 'QtaTotaleAcquaAssorbitaPeriodo', title: this.translocoService.translate('QtaTotaleAcquaAssorbitaPeriodo') }, {  hidden: true,includeInChooser: false, resizable: true, editable: false, format: '{0:n4}', numeric: numericsettings }),
            new KendoGridColumn({ field: 'Disciplinare_Des', title: this.translocoService.translate('Disciplinare') }, { hidden: true, resizable: true, editable: false }),
            new KendoGridColumn({ field: 'Capitolato_Privato_Des', title: this.translocoService.translate('CapitolatoPrivato') }, { hidden: true, resizable: true, editable: false }),
            new KendoGridColumn({ field: 'Grfi_Des', title: this.translocoService.translate('Finalità') }, { hidden: true, resizable: true, editable: false }),
            new KendoGridColumn({ field: 'Validita_Inizio_Date', title: this.translocoService.translate('DataInizioImpianto') }, { hidden: true, resizable: true, editable: false }),
            new KendoGridColumn({ field: 'Data_Semina_Date', title: this.translocoService.translate('DataSemina') }, { hidden: true, resizable: true, editable: false }),
            new KendoGridColumn({ field: 'Data_Fioritura_Date', title: this.translocoService.translate('DataFioritura') }, { hidden: true, resizable: true, editable: false }),
            new KendoGridColumn({ field: 'Data_Fioritura_Prevista_Date', title: this.translocoService.translate('DataFiorituraPrevista') }, { hidden: true, resizable: true, editable: false }),
            new KendoGridColumn({ field: 'Data_Raccolta_Prevista_Date', title: this.translocoService.translate('DataRaccoltaPrevista') }, { hidden: true, resizable: true, editable: false }),
            new KendoGridColumn({ field: 'Progetto', title: this.translocoService.translate('Lotto') }, { hidden: true, resizable: true, editable: false }),
            new KendoGridColumn({ field: 'Copertura', title: this.translocoService.translate('Copertura') }, { hidden: true, resizable: true, editable: false }),
            new KendoGridColumn({ field: 'Data_Raccolta_Date', title: this.translocoService.translate('DataRaccolta') }, { hidden: true, resizable: true, editable: false }),
            new KendoGridColumn({ field: 'Data_Semina_Prevista_Date', title: this.translocoService.translate('DataSeminaPrevista') }, { hidden: true, resizable: true, editable: false }),
            new KendoGridColumn({ field: 'Str_ClasseTessitura', title: this.translocoService.translate('ClasseTessitura') }, { hidden: true, resizable: true, editable: false, includeInChooser: false }),
            new KendoGridColumn({ field: 'N_Massimo', title: this.translocoService.translate('NKgHa') }, { hidden: true, resizable: true, editable: false, showHTMLAsString: true }),
            new KendoGridColumn({ field: 'P_Massimo', title: this.translocoService.translate('PKgHa') }, { hidden: true, resizable: true, editable: false, showHTMLAsString: true }),
            new KendoGridColumn({ field: 'K_Massimo', title: this.translocoService.translate('KKgHa') }, { hidden: true, resizable: true, editable: false, showHTMLAsString: true }),
            new KendoGridColumn({ field: 'Mg_Massimo', title: this.translocoService.translate('MgKgHa') }, { hidden: true, resizable: true, editable: false, showHTMLAsString: true }),
            new KendoGridColumn({ field: 'CU_Massimo', title: this.translocoService.translate('CUKgHa') }, { hidden: true, resizable: true, editable: false, showHTMLAsString: true }),
            new KendoGridColumn({ field: 'SpecieAgea', title: this.translocoService.translate('SpecieAgea') }, { hidden: true, resizable: true, editable: false }),
            new KendoGridColumn({ field: 'CultivarAgea', title: this.translocoService.translate('VarietàAgea') }, { hidden: true, resizable: true, editable: false }),
            new KendoGridColumn({ field: 'TRA_FILA', title: this.translocoService.translate('TraFila') }, { hidden: true, resizable: true, editable: false }),
            new KendoGridColumn({ field: 'SU_FILA', title: this.translocoService.translate('SuFila') }, { hidden: true, resizable: true, editable: false }),
            new KendoGridColumn({ field: 'P_Impianto', title: this.translocoService.translate('NPianteImpianto') }, { hidden: true, resizable: true, editable: false }),
            new KendoGridColumn({ field: 'Foral_Des', title: this.translocoService.translate('FormaAllevamento') }, { hidden: true, resizable: true, editable: false }),
            new KendoGridColumn({ field: 'Port_Des', title: this.translocoService.translate('Portinnesto') }, { hidden: true, resizable: true, editable: false }),
            new KendoGridColumn({ field: 'RifNumerico', title: this.translocoService.translate('AppRifNum') }, { resizable: true, editable: false }),
            new KendoGridColumn({ field: 'CarenzaStr', title: this.translocoService.translate('DataUtilePrimaRaccolta') }, { resizable: true, filterable: true, editable: false, hidden: true, includeInChooser: false }),
            new KendoGridColumn({ field: 'ZVN', title: this.translocoService.translate('ZVN') }, { resizable: true, editable: false })
        ];


        if (this.qdcservice.visualizza_Kpin_BlockName) {
            columns.push(new KendoGridColumn({ field: 'KPIN', title: this.translocoService.translate('KPIN') }, { hidden: true, resizable: true, editable: false }));
            columns.push(new KendoGridColumn({ field: 'Block_Name', title: this.translocoService.translate('BLOCK') }, { hidden: true, resizable: true, editable: false }));
        }

        if (this.qdcservice.visualizza_codici_imp_app_prj) {
            columns.push(new KendoGridColumn({ field: 'Codice_Impianto', title: this.translocoService.translate('CodiceImpianto') }, { hidden: true, resizable: true, editable: false }));
        }

        // Mostro solo le colonne visibili dell'utenti_impostazioni
        if (this.qdcservice.ColonneVisibiliGridImpianti !== undefined &&
            this.qdcservice.ColonneVisibiliGridImpianti !== null &&
            this.qdcservice.ColonneVisibiliGridImpianti.length > 0) {

            columns.forEach(c => {
                this.qdcservice.ColonneVisibiliGridImpianti.forEach(cv => {
                    if (c.title === cv) {
                        c.hidden = false;
                    }
                });
            });
        }

        // Set up dropdowns for the columns before returning
        this.columns = columns;
        this.handleDropdowns();

        return columns;
    }

    private handleDropdowns(): void {
        let index: number = -1;
        index = this.columns.findIndex(s => s.field === 'Consiglio_Codice');

        if (index > -1) {
          this.columns[index].ddl = new DropdownListWithForm('Consiglio_Codice', 'Consiglio_Codice', 'Consiglio_Descrizione', []);
          this.columns[index].ddl.valuePrimitive = true;
          this.columns[index].ddl.loadOnEdit = true;
          this.columns[index].ddl.descriptionField = 'Consiglio_Descrizione';
          this.columns[index].ddl.loadFunction = this.loadConsiglioForImpianto.bind(this);
        }

        index = this.columns.findIndex(s => s.field === 'IrrigazioneUtilizzata_Codice');

        if (index > -1) {
          this.columns[index].ddl = new DropdownListWithForm('IrrigazioneUtilizzata_Codice', 'IrrigazioneUtilizzata_Codice', 'IrrigazioneUtilizzata_Descrizione', []);
          this.columns[index].ddl.valuePrimitive = true;
          this.columns[index].ddl.loadOnEdit = true;
          this.columns[index].ddl.descriptionField = 'IrrigazioneUtilizzata_Descrizione';
          this.columns[index].ddl.loadFunction = this.loadIrrigazioneUtilizzataForImpianto.bind(this);
        }

    }

    setModelGridImpianti() {

        let model: KendoGridModel = {
            PIVA: { type: "string" },
            SA_COD: { type: "number" },
            APPEZZA: { type: "number" },
            ID_REG: { type: "number" },
            rag_soc: { type: "string" },
            sa_nome: { type: "string" },
            Campo_Des: { type: "string" },
            APP_NOME: { type: "string" },
            RifNumerico: { type: "string" },
            CodBioApp: { type: "string" },
            catasto: { type: "string" },
            Fase_Corrente: { type: "string" },
            Codici_Anagrafe_Des: { type: "string" },
            Cul_Des: { type: "string" },
            Grva_Des: { type: "string" },
            Disciplinare: { type: "string" },
            Disciplinare_Cod: { type: "number" },
            Reg_Des: { type: "string" },
            Disciplinare_Des: { type: "string" },
            Capitolato_Privato_Des: { type: "string" },
            Grfi_Des: { type: "string" },
            Validita_Inizio: { type: "string" },
            Validita_Inizio_Date: { type: "date" },
            Validita_Fine: { type: "string" },
            Validita_Fine_Date: { type: "date" },
            Data_Semina: { type: "string" },
            Data_Semina_Date: { type: "date" },
            Data_Fioritura: { type: "string" },
            Data_Fioritura_Date: { type: "date" },
            Data_Fioritura_Prevista: { type: "string" },
            Data_Fioritura_Prevista_Date: { type: "date" },
            Data_Raccolta_Prevista: { type: "string" },
            Data_Raccolta_Prevista_Date: { type: "date" },
            Progetto: { type: "string" },
            Copertura: { type: "string" },
            Flag_Protetto: { type: "number" },
            Data_Raccolta: { type: "string" },
            Data_Raccolta_Date: { type: "date" },
            Data_Semina_Prevista: { type: "string" },
            Data_Semina_Prevista_Date: { type: "date" },
            Str_ClasseTessitura: { type: "string" },
            ListaClassiTessitura: { type: CELL_TYPES.DROPDOWNLIST },
            Obj_Disciplinare: { type: CELL_TYPES.DROPDOWNLIST },
            N_Massimo: { type: "string" },
            P_Massimo: { type: "string" },
            K_Massimo: { type: "string" },
            Mg_Massimo: { type: "string" },
            CU_Massimo: { type: "string" },
            N_Max: { type: "string" },
            P_Max: { type: "string" },
            K_Max: { type: "string" },
            Mg_Max: { type: "string" },
            CU_Max: { type: "string" },
            N_Max_Decimal: { type: "number" },
            P_Max_Decimal: { type: "number" },
            K_Max_Decimal: { type: "number" },
            Mg_Max_Decimal: { type: "number" },
            CU_Max_Decimal: { type: "number" },
            N_Residuo_Decimal: { type: "number" },
            P_Residuo_Decimal: { type: "number" },
            K_Residuo_Decimal: { type: "number" },
            Mg_Residuo_Decimal: { type: "number" },
            CU_Residuo_Decimal: { type: "number" },
            N_Residuo_Percentuale: { type: "number" },
            P_Residuo_Percentuale: { type: "number" },
            K_Residuo_Percentuale: { type: "number" },
            Mg_Residuo_Percentuale: { type: "number" },
            CU_Residuo_Percentuale: { type: "number" },
            SpecieAgea: { type: "string" },
            CultivarAgea: { type: "string" },
            TRA_FILA: { type: "string" },
            SU_FILA: { type: "string" },
            P_Impianto: { type: "string" },
            Foral_Des: { type: "string" },
            Port_Des: { type: "string" },
            Sup_Imp: { type: "number" },
            Sup_Riduzione_BufferZone: { type: "number" },
            Perc_Riduzione_Deriva: { type: "number" },
            SupBZ_Riduzione: { type: "number" },
            Sup_Imp_help: { type: "number" },
            DistBZ_CorpiIdrici: { type: "number" },
            DistBZ_AreeResPub: { type: "number" },
            DistBZ_Allevamenti: { type: "number" },
            DistBZ_VegNatNonColt: { type: "number" },
            Id_ClasseTessitura: { type: "string" },
            Finanziamento: { type: "string" },
            Regolamento: { type: "number" },
            Progetto_Cod: { type: "number" },
            Validita_Inizio_Distinta: { type: "string" },
            Validita_Inizio_Distinta_Date: { type: "date" },
            Validita_Fine_Distinta: { type: "string" },
            Validita_Fine_Distinta_Date: { type: "date" },
            Descrizione_Unica: { type: "string" },
            GisWkt: { type: "string" },
            GisWktGps: { type: "string" },
            GisWktSistemaRiferimento: { type: "string" },
            GisTipoEntita_cod: { type: "number" },
            GisLayerCod: { type: "number" },
            CUL_COD: { type: "number" },
            Veg_Cod: { type: "number" },
            Veg_Des: { type: "string" },
            Campo_Cod: { type: "number" },
            GRFI_COD: { type: "number" },
            Cop_Cod: { type: "number" },
            Validita_inizio_Appezzamento: { type: "string" },
            Validita_inizio_Appezzamento_Date: { type: "date" },
            Validita_fine_Appezzamento: { type: "string" },
            Validita_fine_Appezzamento_Date: { type: "date" },
            kendoKey: { type: "string" },
            App_Nome_Anagrafica: { type: "string" },
            StaticMapBase64String: { type: "custom" },
            CarenzaStr: { type: "string" },
            DataCarenza: { type: "date" },
            ZVN: { type: "string" },
            Progetto_Des: { type: "string" },
            Stato_Cod: { type: "string" },
            Dichiarazione_Non_Utilizzo_Trattamenti: { type: "string" },
            Dichiarazione_Non_Utilizzo_Fertilizzazioni: { type: "string" },
            IrrigazioneUtilizzata_Codice: { type: CELL_TYPES.DROPDOWNLIST },
            IrrigazioneUtilizzata_Descrizione: { type: CELL_TYPES.STRING },
            Consiglio_Codice: { type: CELL_TYPES.DROPDOWNLIST }, // consiglioIrrigazione.codice
            Consiglio_Descrizione: { type: CELL_TYPES.STRING },
            DataConsiglio: { type: "date" }, // consiglioIrrigazione.dataConsiglio
            DoseAcquaConsiglio: { type: "number" }, // consiglioIrrigazione.qtaAcqua
            UdmConsiglio: {type: CELL_TYPES.DROPDOWNLIST},
            minDataTurno: { type: "date" },
            maxDataTurno: { type: "date" },
            UdmDose: { type: "string" }, // unitaDiMisura.simbolo or consiglioIrrigazione.unitaDiMisura.simbolo
            DoseAcquaGiornaliera: { type: "number" }, // Qta Acqua Metro cubo Ettaro
            OreIrrigazione: { type: "number" }, // Ore
            Portata: { type: "number" }, // Portata
            Efficienza: { type: "number" }, // calculated field
            QtaTotaleAcquaGiornaliera: { type: "number" }, // QtaTotale * superficie (daily)
            QtaTotaleAcquaAssorbitaGiornaliera: { type: "number" }, // calculated field
            DataInizioIrrigazione: { type: "date"}, // DataInizio
            DataFineIrrigazione: { type: "date"}, // DataFine
            FrequenzaIrrigazioneMedia: { type: "number" }, // Frequenza
            QtaTotaleAcquaUtilizzataPeriodo: { type: "number" }, // QtaRilevata * superficie (period)
            QtaTotaleAcquaAssorbitaPeriodo: { type: "number" }, // calculated field
            QtaRilevata: { type: "number" },
            QtaTotale: { type: "number" },
            Ore: { type: "number" },
            DataInizio: { type: "date" },
            DataFine: { type: "date" },
            Frequenza: { type: "number" },
            tipoIrrigazione: { type: CELL_TYPES.DROPDOWNLIST },
            macchinaIrrigazione: { type: CELL_TYPES.DROPDOWNLIST },
        };

        if (this.qdcservice.visualizza_Kpin_BlockName) {
            model['KPIN'] = { type: 'string' };
            model['Block_Name'] = { type: 'string' };
        }

        if (this.qdcservice.visualizza_codici_imp_app_prj) {
            model['Codice_Impianto'] = { type: 'string' };
        }

        return model;
    }

    public override onCreateExternalFormGroup = (event: any) => {

        const formGroup = new FormGroup({});

        const field = event.column.field;

        if (field !== undefined &&
            field !== null &&
            field !== '') {

            const row: KendoGridRow = event.dataItem;

            const column: KendoGridColumn = this.GridImpianti.columns.filter(c => c.field === field)[0];

            if (column.editable) {

                const value: string | Date = row[column.field];

                const formState: any = {
                    value: value,
                    disabled: false
                };

                let formCtrl: FormControl = null;

                if (column.field === 'Sup_Imp_help') {
                    column.numeric.max = (row['Sup_Imp'] - row['Sup_Riduzione_BufferZone']);
                }

                formCtrl = new FormControl(formState, column.validators);

                formGroup.addControl(column.field, formCtrl);

                if (column.ddl != null) {
                    const descField = column.ddl.descriptionField;
                    const descValue = row[descField];
                    const descFormState: any = {
                        value: descValue,
                        disabled: false
                    };
                    const descFormControl = new FormControl(descFormState);
                    formGroup.addControl(descField, descFormControl);
                }
            }
        }

        return formGroup;

    };

    private RiordinaRigheGridImpianti(rows: Array<any>): Array<any> {

        //Mostro prima le righe selezionate in modifica
        if (this.objParametriAgenda.TipoOperazioneDB === Enum_DBTypeOperation.Update) {
            let righe_selezionate = rows.filter(r => r.Selected === true);

            if (righe_selezionate.length > 0) {
                rows = rows.filter(r => !r.Selected || r.Selected === false);

                for (let i = 0; i < righe_selezionate.length; i++) {
                    rows.splice(i, 0, righe_selezionate[i]);
                }
            }
        }

        return rows;

    }

    public updateRowsFromDialog(editedImpianti: any[]): void {
        if (!Array.isArray(editedImpianti) || !this.GridImpianti?.rows) return;

        editedImpianti.forEach(edited => {
            // Find the row by unique key (adjust as needed)
            const idx = this.GridImpianti.rows.findIndex(
                r => (r as any).kendoKey === edited.kendoKey
            );
            if (idx > -1) {
                this.GridImpianti.rows[idx] = { ...this.GridImpianti.rows[idx], ...edited };
            }
        });

        // Force change detection by reassigning the array
        this.GridImpianti.rows = [...this.GridImpianti.rows];

        // Refresh the grid UI
        if (this.gridPublicService?.giasGridComponent) {
            this.gridPublicService.giasGridComponent.refresh();
        }
    }

    public LeggiMacchinaIrrigazioneDefault(piva: string, sa_cod: number, appezza: number, id_reg: number): Observable<any> {
        let leggiMacchineIrrigazione: LeggiMacchineIrrigazione = {
            piva: piva,
            sa_cod: sa_cod,
            appezza: appezza,
            id_reg: id_reg
        };
        return this.impiantiservice.LeggiMacchinaIrrigazioneDefault(leggiMacchineIrrigazione).pipe(
            map((response: IrrigazioneImpianto) => this.transformIrrigazioneImpianto(response))
        );
    }
}
