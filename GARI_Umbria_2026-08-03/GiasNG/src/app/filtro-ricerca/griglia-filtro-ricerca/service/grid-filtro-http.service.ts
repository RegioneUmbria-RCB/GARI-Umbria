import { Injectable, Injector, OnDestroy } from "@angular/core";
import { TranslocoService } from "@jsverse/transloco";
import {
    FiltroRicercaClient,
    CriteriRicerca_IN,
    CodiceAnagrafeBase
} from "app/Service/net-core6-api.service";
import { AbstractGridConfigService, CommandsColumnSettings, CommandsDropDownSettings, ConfigTemplate, CustomColumnSettings, DettagliColumnSettings, EditingMode, GridCustomizations, GroupSettings, HttpAction, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, ScrollingMode, SelectableSettings, ToolbarSettings } from 'gias-kendo-grid';
import { BehaviorSubject, Observable, catchError, from, map, of, skip, take, takeUntil } from "rxjs";
import { FiltroRicercaService, SignalOverflowRows } from "./filtro-ricerca.service";
import { PermessiUtenteService } from "app/Service/permessi-utente.service";
import { Enum_Entita_FiltroRicerca, enum_Security_Attivita, Enum_TipoComportamento_FiltroRicerca } from "app/Model/TipiEnumerativi";
import { AGRODATAFINE, AGRODATAINIZIO } from "app/Model/CostantiPersonalizzate";
import { ConversionService } from "app/Service/conversion.service";
import { FiltroJSONService } from 'gias-kendo-grid';
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { GestioneRichiesteService } from "app/Service/gestione-richieste.service";
import { TreeAziendeService } from "app/filtro-ricerca/service/tree-aziende.service";
import { SelectionEvent } from "@progress/kendo-angular-grid";
import { GiasKendoGridComponent } from 'gias-kendo-grid';
import { DialogWindowService } from "./dialog-window.service";
import { faCheckCircle } from "@fortawesome/free-solid-svg-icons";

export class GridFiltroRicercaServerResult extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

@Injectable()
export class GridFiltroRicercaHttpService extends AbstractGridConfigService<GridFiltroRicercaServerResult> implements OnDestroy {

    gridId = 'FiltroRicercaGridId';
    rowId = 'chiave';

    GridFiltroServerResult: GridFiltroRicercaServerResult;

    GridFiltro: any = {};

    loader: LoaderType = LoaderType.SERVICE;
    editingMode: EditingMode = EditingMode.IN_PAGE;

    views = new GridCustomizations({ enabled: true });

    permessoGridViste;

    paramAgenda;
    paramFiltroRicercaNG: any;

    changeViewToApply = new BehaviorSubject<boolean>(null);
    applyViewFiltroJSONObs;
    afterSaveView;
    gridIDchangeObs;
    valueChangeForm;

    filtroColture;

    distinctPiva: any[];

    constructor(injector: Injector,
        private translocoService: TranslocoService,
        private filtroRicercaService: FiltroRicercaClient,
        private filtroRicercaFormService: FiltroRicercaService,
        private permessiUtenteService: PermessiUtenteService,
        private conversionService: ConversionService,
        private filtroJSONService: FiltroJSONService,
        private treeAziendeService: TreeAziendeService,
        private objParametriAgendaService: ObjParametriAgendaService,
        private gestioneRichiesteService: GestioneRichiesteService,
        private dialogService: DialogWindowService) {

        super(injector, ConfigTemplate.DefaultTemplate);

        this.distinctPiva = new Array();

        this.cmdDropDown = new CommandsDropDownSettings({
            fullEditBtn: false, removeBtn: false, infoBtn: false
        });

        //se arrivo da Ricerca Avanzata, disattivo le viste
        this.views.enabled = !this.filtroRicercaFormService.displayNoViews;

        //serve a far in modo che non sia possibile salvare le personalizzazioni della vista su quella di default
        this.views.isSaveDisabledOnDefaultView = true;

        this.paramAgenda = this.objParametriAgendaService.getObjParamValue();

        this.paramFiltroRicercaNG = (this.paramAgenda.GenericObj_string !== '') ? JSON.parse(this.paramAgenda.GenericObj_string) : '';

        this.gestioneChiamanti();

        this.cmdColumn = new CommandsColumnSettings({
            editBtn: false,
            infoBtn: false,
            removeBtn: false,
            onDisableInfoBtn: () => false,
            width: 10
        });

        this.toolbar = new ToolbarSettings(
            false,
            false,
            'Azioni',
            150,
            '',
            false
        );

        this.resizable.autoFitColumns = true;

        this.resizable.isResizable = true;

        this.columnMenu.kendoGridColumnChooser = true;

        this.behavior.excelSettings.enabled = true;

        this.behavior.pdfSettings.enabled = false;

        this.generalSettings.performOnEdit = false;

        this.generalSettings.height = 'auto';

        // this.pagination.pageable = {
        //     buttonCount: 10,
        //     info: true,
        //     type: 'numeric',
        //     // pageSizes: this.rowOptions,
        //     previousNext: true
        // };

        this.pagination.gridState.take = 10;
        this.pagination.navigable = true;

        // DCA20240822: griglia scrollabile senza paginazione, per ora commentata perché manca il numero di row
        // this.pagination.scrollingType = ScrollingMode.Virtual;
        // this.pagination.virtualScrolling = { rowHeight: 36, viewportHeight: 450 };

        this.behavior.showDeletionConfirmation = true;

        this.groups = new GroupSettings({ groupable: { enabled: true, showFooter: false } }, this.translocoService);

        this.permessoGridViste = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Visibilita_Viste_Grid, 0);

        //gestione salvataggio Viste/report
        this.gridPublicService.thereIsAForm = true;

        //gestione delle view in base al filtroJSON salvato
        this.applyViewFiltroJSONObs = this.gridPublicService.applyViewFiltroJSON.pipe(skip(1)).subscribe((view) => {
            this.filtroRicercaFormService.onLoad = (!view.FiltroJSON || view.Predefinita);  //essendo impostato isSaveDisabledOnDefaultView a true, se la vista è di default non deve caricare i filtri salvati
                                                                                            // e lancio la query con l'onLoad a true
            if (!view.Predefinita) {
                let filtroJSON = this.conversionService.ConversionDateInObject((view.FiltroJSON)? JSON.parse(view.FiltroJSON) : this.filtroRicercaFormService.InitializeFormFiltroRicerca({ CategoriaEsito: this.filtroRicercaFormService.formFilters.get("CategoriaEsito").value }));
                this.filtroRicercaFormService.set_tab_Carica_Dati_Visibility();
                this.filtroRicercaFormService.formFilters.patchValue(filtroJSON, { emitEvent: false });
                this.filtroRicercaFormService.fillFormArray(filtroJSON.FiltriTemporali?.FiltriData);

                // mi salvo il valore iniziale del filtroJSON, mi servirà per capire se 
                //l'utente ha fatto delle modifiche ai filtri nel mentre che stava sulla pagina
                this.filtroRicercaFormService.filtroJSONBeforeChanges = this.filtroRicercaFormService.formFilters.getRawValue();
            }

            this.changeViewToApply.next(false);
        });

        this.afterSaveView = this.gridPublicService.afterSaveView.pipe(skip(1)).subscribe((filtroJSON) => {
            this.filtroRicercaFormService.filtroJSONBeforeChanges = (filtroJSON !== '') ? this.conversionService.ConversionDateInObject(JSON.parse(filtroJSON)) : this.filtroRicercaFormService.filtroJSONBeforeChanges;
        });

        this.gridPublicService.filtroJSONstring = JSON.stringify(this.filtroRicercaFormService.formFilters.getRawValue());

        this.valueChangeForm = this.filtroRicercaFormService.formFilters.valueChanges.subscribe((value) => {
            this.gridPublicService.filtroJSONstring = JSON.stringify(this.filtroRicercaFormService.formFilters.getRawValue());
        });

        this.gridIDchangeObs = this.filtroRicercaFormService.gridIDstring.subscribe((gridIdstring) => {
            this.changeViewToApply.next(true);
            this.gridPublicService.keyViewString.next(gridIdstring);
        });

    }

    gestioneChiamanti() {

        let showAdditionalColumn = (this.paramFiltroRicercaNG !== '') ? true : false;

        if (showAdditionalColumn) {

            let showColumnImpostaAzienda = (this.paramFiltroRicercaNG.TipoComportamentoFiltroRicercaNG == Enum_TipoComportamento_FiltroRicerca.RicercaAvanzataAzienda) ? true : false;

            this.customColumn = new CustomColumnSettings({
                showColumn: showColumnImpostaAzienda,
                action: this.selectAzienda.bind(this),
                btnIcon: '',
                fontawesomeIcon: faCheckCircle,
                title: this.transloco.translate('Comandi'),
                btnTooltip: this.transloco.translate('ImpostaAziendaCorrente')
            });

            if (!showColumnImpostaAzienda) {                            //se non sono nel caso della Ricerca avanzata, metto le checkbox
                this.selectable.selectable = new SelectableSettings({
                    checkboxOnly: true,
                    enabled: true,
                });

                this.selectable.preselectedRows.selectionChangeFn = this.selectionChangeFn;

                this.selectable.preselectedRows.selectedRows = [];
                // this.selectable.columnSettings.showSelectAll = (this.reportMultiSelectGridOrogel.includes(paramFiltroRicercaNG.CodificaStampe)) ? true : false;
                this.selectable.columnSettings.showSelectAll = true;
                this.selectable.shouldShowCheckbox = true;
                this.selectable.columnSettings.title = ' ';
            }
        }
    }

    public selectionChangeFn = (event: SelectionEvent, component: GiasKendoGridComponent) => {
        const selectedRows = event.selectedRows;
        const deselectedRows = event.deselectedRows;

        if (selectedRows.length > 0) {

            //verifico in che contesto ci troviamo
            let permessi = this.paramFiltroRicercaNG.BlocchiSelezionexStampa;
            let msg: string = '';

            if (permessi !== undefined) {
                if (permessi.SingolaAzienda && (selectedRows.some(elemSelected => this.filtroRicercaFormService.selectedRows.some(elemetSave => elemetSave.dataItem.Piva !== elemSelected.dataItem.Piva)) || selectedRows.some(elemSelected => elemSelected.dataItem.Piva !== selectedRows[selectedRows.length - 1].dataItem.Piva)))
                    msg += ' ' + this.transloco.translate("PivaUnica");
                if (permessi.SingoloCentro && (selectedRows.some(elemSelected => this.filtroRicercaFormService.selectedRows.some(elemetSave => elemetSave.dataItem.Sa_Cod !== elemSelected.dataItem.Sa_Cod)) || selectedRows.some(elemSelected => elemSelected.dataItem.Sa_Cod !== selectedRows[selectedRows.length - 1].dataItem.Sa_Cod)))
                    msg += '\n ' + this.transloco.translate("SaCodUnico");
                if (permessi.SingolaSpecie && (selectedRows.some(elemSelected => this.filtroRicercaFormService.selectedRows.some(elemetSave => elemetSave.dataItem.Veg_Cod !== elemSelected.dataItem.Veg_Cod)) || selectedRows.some(elemSelected => elemSelected.dataItem.Veg_Cod !== selectedRows[selectedRows.length - 1].dataItem.Veg_Cod)))
                    msg += '\n ' + this.transloco.translate("VegCodUnico");
            }

            if (msg == '')
                selectedRows.forEach(item => {
                    if (!this.filtroRicercaFormService.selectedRows.some(saveItem => saveItem.index === item.index))
                        this.filtroRicercaFormService.selectedRows.push(item);
                });
            else {
                let rows = this.gridPublicService.getValue().data.rows.filter(row => selectedRows.some(item => item.dataItem.chiave === row['chiave']));

                rows.forEach(row => row['Selected'] = false);

                this.dialogService.infoDialog(msg);
            }
        } else
            this.filtroRicercaFormService.selectedRows = this.filtroRicercaFormService.selectedRows.filter(item => !deselectedRows.some(deselectedItem => deselectedItem.index === item.index));
    }

    selectAzienda(dataItem) {
        if (dataItem) {
            this.paramAgenda.Piva = dataItem.Piva;
            this.paramAgenda.RagSoc = dataItem.RagioneSociale;
            this.objParametriAgendaService.changeObjParametriAgenda(this.paramAgenda);
            let paramFiltroRicercaNG = (this.paramAgenda.GenericObj_string !== '') ? JSON.parse(this.paramAgenda.GenericObj_string) : '';

            if (paramFiltroRicercaNG !== '' && paramFiltroRicercaNG.SitoDestinazioneDopoIlRedirect !== 0 && paramFiltroRicercaNG.PaginaDestinazioneDopoIlRedirect !== 0)
                this.gestioneRichiesteService.gestionePassaggioAltroSito((paramFiltroRicercaNG !== '' && paramFiltroRicercaNG.SitoDestinazioneDopoIlRedirect !== undefined) ? paramFiltroRicercaNG.SitoDestinazioneDopoIlRedirect : this.paramAgenda.Sito_Provenienza, (paramFiltroRicercaNG !== '' && paramFiltroRicercaNG.PaginaDestinazioneDopoIlRedirect !== undefined) ? paramFiltroRicercaNG.PaginaDestinazioneDopoIlRedirect : this.paramAgenda.Pagina_Provenienza).then(resp => window.location.href = resp);
            else
                from(this.gestioneRichiesteService.gestioneRedirect()).pipe(
                    take(1)
                ).subscribe(
                    (val: string) => {
                        const href = val + '&IDSezione=' + paramFiltroRicercaNG.IDSezione;
                        window.location.href = href
                    }
                );
        }
    }

    getParamsForGrid(): CriteriRicerca_IN {

        let arrSpecie;
        let arrVarieta;

        let arrZone = this.filtroRicercaFormService.formFilters.get('FiltriAziende').get('Zone').value.map(item => item['Zona_Cod']);

        if (this.filtroRicercaFormService.formFilters.get("FiltriPianoColturale").get("Specie"))
            arrSpecie = this.filtroRicercaFormService.formFilters.get("FiltriPianoColturale").get("Specie").value;
        else
            arrSpecie = [];

        if (this.filtroRicercaFormService.formFilters.get("FiltriPianoColturale").get("Varieta"))
            arrVarieta = this.filtroRicercaFormService.formFilters.get("FiltriPianoColturale").get("Varieta").value;
        else
            arrVarieta = [];

        let CodiciAzienda: Array<CodiceAnagrafeBase> = (this.filtroRicercaFormService.formFilters.get('CaricaDati').get("AltriDatiAzienda").value) ? (this.filtroRicercaFormService.formFilters.get("CaricaDati").get("CodiciAzienda").getRawValue().filter(c => !c.master && c.checked) as Array<CodiceAnagrafeBase>) : [];

        let CodiciCentroAziendale: Array<CodiceAnagrafeBase> = (this.filtroRicercaFormService.formFilters.get('CaricaDati').get("AltriDatiCentro").value) ? (this.filtroRicercaFormService.formFilters.get("CaricaDati").get("CodiciCentroAziendale").getRawValue().filter(c => !c.master && c.checked) as Array<CodiceAnagrafeBase>) : [];

        let CodiciFabbricato: Array<CodiceAnagrafeBase> = (this.filtroRicercaFormService.formFilters.get('CaricaDati').get("AltriDatiAzienda").value) ? (this.filtroRicercaFormService.formFilters.get("CaricaDati").get("CodiciFabbricato").getRawValue().filter(c => !c.master && c.checked) as Array<CodiceAnagrafeBase>) : [];

        let CodiciCampo: Array<CodiceAnagrafeBase> = (this.filtroRicercaFormService.formFilters.get('CaricaDati').get("AltriDatiFabbricato").value) ? (this.filtroRicercaFormService.formFilters.get("CaricaDati").get("CodiciCampo").getRawValue().filter(c => !c.master && c.checked) as Array<CodiceAnagrafeBase>) : [];

        let CodiciAppezzamento: Array<CodiceAnagrafeBase> = (this.filtroRicercaFormService.formFilters.get('CaricaDati').get("AltriDatiPianoColturale").value) ? (this.filtroRicercaFormService.formFilters.get("CaricaDati").get("CodiciPianoColturale").getRawValue().filter(c => !c.master && c.checked && c.tipoCodice === Enum_Entita_FiltroRicerca.Appezzamento) as Array<CodiceAnagrafeBase>) : [];

        let CodiciImpianto: Array<CodiceAnagrafeBase> = (this.filtroRicercaFormService.formFilters.get('CaricaDati').get("AltriDatiPianoColturale").value) ? (this.filtroRicercaFormService.formFilters.get("CaricaDati").get("CodiciPianoColturale").getRawValue().filter(c => !c.master && c.checked && c.tipoCodice === Enum_Entita_FiltroRicerca.Impianto) as Array<CodiceAnagrafeBase>) : [];

        let CodiciEsercizio: Array<CodiceAnagrafeBase> = (this.filtroRicercaFormService.formFilters.get('CaricaDati').get("AltriDatiPianoColturale").value) ? (this.filtroRicercaFormService.formFilters.get("CaricaDati").get("CodiciPianoColturale").getRawValue().filter(c => !c.master && c.checked && c.tipoCodice === Enum_Entita_FiltroRicerca.Esercizio) as Array<CodiceAnagrafeBase>) : [];

        return {
            TipoMostra: this.filtroRicercaFormService.formFilters.get("CategoriaEsito") ? +this.filtroRicercaFormService.formFilters.get("CategoriaEsito").value['idCat'] : 0,
            OnLoad: this.filtroRicercaFormService.onLoad,
            Id_Budget: this.filtroRicercaFormService.formFilters.get('Budget').value ? this.filtroRicercaFormService.formFilters.get('Budget').value['Id_Budget'] : 0,

            FiltriAziende: {
                ImpreseReferenti: this.filtroRicercaFormService.formFilters.get("FiltriAziende").get("ImpreseReferenti").value.map(item => item['Piva']),
                TipiImpresa: this.filtroRicercaFormService.formFilters.get("FiltriAziende").get("TipiImpresa").value.map(item => item['id']),
                RagioneSociale: this.filtroRicercaFormService.formFilters.get("FiltriAziende").get("RagioneSociale") ? this.filtroRicercaFormService.formFilters.get("FiltriAziende").get("RagioneSociale").value : "",
                CUAA: this.filtroRicercaFormService.formFilters.get("FiltriAziende").get("CUAA") ? this.filtroRicercaFormService.formFilters.get("FiltriAziende").get("CUAA").value : "",
                Piva: this.filtroRicercaFormService.formFilters.get("FiltriAziende").get("Piva") ? this.filtroRicercaFormService.formFilters.get("FiltriAziende").get("Piva").value : "",
                Stati: this.filtroRicercaFormService.formFilters.get("FiltriAziende").get("Stati").value.map(item => item['Codice']),
                Regioni: this.filtroRicercaFormService.formFilters.get("FiltriAziende").get("Regioni").value.map(item => item['REG']),
                Province: this.filtroRicercaFormService.formFilters.get("FiltriAziende").get("Province").value.map(item => item['PROV']),
                Comuni: this.filtroRicercaFormService.formFilters.get("FiltriAziende").get("Comuni").value.map(item => item['PROV'] + "_" + item['COM_LOCALITA']),
                OperatoreLogicoZone: this.filtroRicercaFormService.formFilters.get("FiltriAziende").get("FiltroZone").value['Value'],
                Zone: arrZone
            },
            FiltriCentriAziendali: {
                CentroAziendale: this.filtroRicercaFormService.formFilters.get("FiltriCentriAziendali").get("CentroAziendale") ? this.filtroRicercaFormService.formFilters.get("FiltriCentriAziendali").get("CentroAziendale").value : "",
                Stati: this.filtroRicercaFormService.formFilters.get("FiltriCentriAziendali").get("Stati").value.map(item => item['Codice']),
                Regioni: this.filtroRicercaFormService.formFilters.get("FiltriCentriAziendali").get("Regioni").value.map(item => item['REG']),
                Province: this.filtroRicercaFormService.formFilters.get("FiltriCentriAziendali").get("Province").value.map(item => item['PROV']),
                Comuni: this.filtroRicercaFormService.formFilters.get("FiltriCentriAziendali").get("Comuni").value.map(item => item['PROV'] + "_" + item['COM_LOCALITA']),
            },
            FiltriPianoColturale: {
                FiltroDestinazioneUso: this.filtroRicercaFormService.formFilters.get("FiltriPianoColturale").get("FiltroDestinazioneUso")?.value?.id ?? 0,
                DestinazioniUso: this.filtroRicercaFormService.formFilters.get("FiltriPianoColturale").get("DestinazioniUso")?.value.map(item => item['Codice']),
                UtilizzoTerreno: this.filtroRicercaFormService.formFilters.get("FiltriPianoColturale").get("UtilizzoTerreno")?.value.map(item => item['Value']),
                GruppoVegetale: this.filtroRicercaFormService.formFilters.get("FiltriPianoColturale").get("GruppoVegetale").value.map(item => item['Gru_Cod']),
                Specie: arrSpecie ? arrSpecie.map(item => item.Veg_Cod) : arrSpecie,
                Varieta: arrVarieta ? arrVarieta.map(item => item.Cul_Cod) : arrVarieta,
                TipologiaVarietale: this.filtroRicercaFormService.formFilters.get("FiltriPianoColturale").get("TipologiaVarietale").value.map(item => item['Grva_Cod']),
                ContributiACA: this.filtroRicercaFormService.formFilters.get("FiltriPianoColturale").get("ContributiACA").value.map(item => item['code']),
                Lotto: this.filtroRicercaFormService.formFilters.get("FiltriPianoColturale").get("Lotto").value,
                Progetto: this.filtroRicercaFormService.formFilters.get("FiltriPianoColturale").get("Progetto").value,
            },
            FiltriMovimenti: {
                GruppoOperazioni: this.filtroRicercaFormService.formFilters.get("FiltriMovimenti").get("GruppoOperazioni").value.map(item => item['GRU_COD']),
                Operazioni: this.filtroRicercaFormService.formFilters.get("FiltriMovimenti").get("Operazioni").value.map(item => item['LAV_COD']),
                FiltroOperazioni: this.filtroRicercaFormService.formFilters.get("FiltriMovimenti").get("FiltroOperazioni").value ? this.filtroRicercaFormService.formFilters.get("FiltriMovimenti").get("FiltroOperazioni").value['Value'] : -1,
                DataMovimento: {
                    inizio: this.filtroRicercaFormService.formFilters.get("FiltriMovimenti").get("DataMovimento").get("inizio").value ?? AGRODATAINIZIO,
                    fine: this.filtroRicercaFormService.formFilters.get("FiltriMovimenti").get("DataMovimento").get("fine").value ?? AGRODATAFINE
                },
            },
            FiltriTemporali: {
                FiltriData: this.filtroRicercaFormService.formFilters.get("FiltriTemporali")
                            .get("FiltriData")
                            ?.getRawValue()
                            ?.reduce((acc, item) => {
                                const entita = item.Entita?.id ?? -1;
                                const colonnaData = item.ColonnaData?.id ?? -1;
                                const tipoConfronto = item.TipoConfronto?.id ?? -1;
                        
                                // Filtra direttamente durante la riduzione
                                if (entita !== -1 && colonnaData !== -1 && tipoConfronto !== -1) {
                                    acc.push({
                                        Entita: entita,
                                        ColonnaData: colonnaData,
                                        TipoConfronto: tipoConfronto,
                                        ModalitaFiltroData: item.ModalitaFiltroData?.valueOf() ? 1 : 0,
                                        Date: {
                                            inizio: item.Date?.inizio ?? AGRODATAINIZIO,
                                            fine: item.Date?.fine ?? AGRODATAFINE,
                                        },
                                    });
                                }
                        
                                return acc;
                            }, []),
                OperatoreLogicoFiltriTemporali: this.filtroRicercaFormService.formFilters.get("FiltriTemporali").get("OperatoreLogico").value['Value']
            },

            FiltriServizi: {
                Servizio: this.filtroRicercaFormService.formFilters.get("FiltriServizi").get("Servizio")?.value?.Servizio_Cod ?? -1,
                StatiPratica: this.filtroRicercaFormService.formFilters.get("FiltriServizi").get("StatiPratica").value.map(item => item['Stato_Cod']),
                Data: this.filtroRicercaFormService.formFilters.get("FiltriServizi").get("Data").value ?? AGRODATAINIZIO,
            },

            FiltriCatasto: {
                FiltroRipartoCatastale: this.filtroRicercaFormService.formFilters.get("FiltriCatasto").get("FiltroRipartoCatastale")?.value?.Value ?? -1,
            },

            FiltriGIS: {
                FiltroPoligoni: this.filtroRicercaFormService.formFilters.get("FiltriGIS").get("FiltroPoligoni").value['Value'],
                Anomalie: this.filtroRicercaFormService.formFilters.get("FiltriGIS").get("Anomalie").value.map(item => item['Algoritmo'])
            },

            CaricaDati: {
                ImpreseReferenti: this.filtroRicercaFormService.formFilters.get("CaricaDati").get("ImpreseReferenti").value,
                DatiIscrizioneLibroSoci: {
                    NumeroIscrizione: this.filtroRicercaFormService.formFilters.get("CaricaDati").get("DatiIscrizioneLibroSoci").get("NumeroIscrizione").value,
                    DataIscrizione: this.filtroRicercaFormService.formFilters.get("CaricaDati").get("DatiIscrizioneLibroSoci").get("DataIscrizione").value
                },
                LegaleRappresentante: this.filtroRicercaFormService.formFilters.get("CaricaDati").get("LegaleRappresentante").value,
                IndirizzoAzienda: this.filtroRicercaFormService.formFilters.get("CaricaDati").get("IndirizzoAzienda").value,
                IndirizzoCentroAziendale: this.filtroRicercaFormService.formFilters.get("CaricaDati").get("IndirizzoCentroAziendale").value,
                IndirizziPianoColturale: this.filtroRicercaFormService.formFilters.get("CaricaDati").get("IndirizziPianoColturale").value,
                Servizi: this.filtroRicercaFormService.formFilters.get("CaricaDati").get("Servizi").value,
                CatastoCampo: this.filtroRicercaFormService.formFilters.get("CaricaDati").get("CatastoCampo").value,
                CodiciAzienda: CodiciAzienda,
                CodiciCentroAziendale: CodiciCentroAziendale,
                CodiciAppezzamento: CodiciAppezzamento,
                CodiciImpianto: CodiciImpianto,
                CodiciEsercizio: CodiciEsercizio,
                CodiciCampo: CodiciCampo,
                CodiciFabbricato: CodiciFabbricato,
                CatastoAppezzamento: this.filtroRicercaFormService.formFilters.get("CaricaDati").get("CatastoAppezzamento").value,
                GISImpianto: this.filtroRicercaFormService.formFilters.get("CaricaDati").get("GISImpianto").value,
                ContributiACA: this.filtroRicercaFormService.formFilters.get("CaricaDati").get("ContributiACA").value,
                CatastoCentroAziendale: this.filtroRicercaFormService.formFilters.get("CaricaDati").get("CatastoCentroAziendale").value,
            }
        };

    }

    read(options?: any): Observable<GridFiltroRicercaServerResult> {

        this.loadingService.set_isLoading({ isLoading: true, component: this.gridPublicService.gridElRef });

        if ((this.filtroRicercaFormService.onLoad && !this.filtroRicercaFormService.hasChangedValues(this.filtroRicercaFormService.formFilters.getRawValue(), this.filtroRicercaFormService.InitializeFormFiltroRicerca({ CategoriaEsito: this.filtroRicercaFormService.formFilters.get("CategoriaEsito").value }))))
            this.filtroRicercaFormService.onLoad = true;
        else
            this.filtroRicercaFormService.onLoad = false;

        if (!this.filtroRicercaFormService.applyLoadCacheViewFirstTime)
            this.setFalse_applyLoadCacheFirstTime();

        //se faccio una ricerca, resetto le righe selezionate
        this.filtroRicercaFormService.selectedRows = [];

        let param = this.getParamsForGrid();
        return this.filtroRicercaService.filtroRicercaGetResult(param).pipe(catchError((err) => {
            this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });
            return of();
        }), map(r => {
            
            this.loadingService.set_isLoading({ isLoading: false, component: this.gridPublicService.gridElRef });

            //ripristino la cache
            this.filtroRicercaFormService.applyLoadCacheViewFirstTime = true;

            let objResult = r.RispostaStringa;
            
            let gridVisiteServerResult = new GridFiltroRicercaServerResult(
                this.setRowGrid(objResult.result),
                this.setColumnsGrid(objResult.kendoColumns),
                this.setKendoModelGrid(objResult.kendoColumns)
            );

            //verifichiamo se c'è stato overflow del numero delle righe da mostrare in griglia
            //in quanto è strata aggiunta una chiave in tabella che segnala quante righe debbano essere mostrate
            //in caso, facciamo apparire una kendo-dialog con il messaggio

            this.filtroRicercaFormService.setOverflowSelectTopRows(new SignalOverflowRows(objResult.overflowSelectTopRows, objResult.result.length));

            if (objResult.overflowSelectTopRows) {
                this.dialogService.infoDialog(this.transloco.translate('NumeroRigheMaggioreDiQuelloImpostato', { n_rows: objResult.result.length }));
            }

            return gridVisiteServerResult;
        }));
    }

    perform(actionType: HttpAction, items: any, oldRow?: any): Observable<any> {
        throw new Error("Method not implemented.");
    }

    setRowGrid(responseRows) {
        this.distinctPiva = [...new Set(responseRows.map(item => item.Piva))];
        this.treeAziendeService.pivaFilterSubject.next(this.distinctPiva);

        return responseRows;
    }

    setColumnsGrid(columnsName): Array<KendoGridColumn> {
        let columns: Array<KendoGridColumn> = [];
        for (let itemOfColumns of columnsName) {
            if (itemOfColumns.Display)
                columns.push(
                    new KendoGridColumn({ field: itemOfColumns.Field, title: (itemOfColumns.TranslateTitle) ? this.translocoService.translate(itemOfColumns.Title) : itemOfColumns.Title }, { resizable: true, editable: false, hidden: itemOfColumns.Hidden })
                );
        }

        return columns;
    }

    ngOnDestroy(): void {
        this.applyViewFiltroJSONObs.unsubscribe();
        this.afterSaveView.unsubscribe();
        this.gridIDchangeObs.unsubscribe();
        this.valueChangeForm.unsubscribe();
    }

    setKendoModelGrid(arrayModel): KendoGridModel {

        let gridModel = new KendoGridModel();

        arrayModel.forEach((item) => {
            gridModel[item.Field] = { editable: false, type: item.DataType };
        });

        return gridModel;
    }

    getPermessoGridViste() {
        return this.permessoGridViste;
    }

    setTrueisFirstApplyView() {
        this.filtroJSONService.isFirstApplyView = true;
    }

    setFalse_applyLoadCacheFirstTime() {
        this.filtroJSONService.applyLoadCacheFirstTime = false;
    }
}
