import { Injectable, OnDestroy, signal } from "@angular/core";
import { FormArray, FormBuilder, FormControl, FormGroup } from "@angular/forms";
import { TranslocoService } from "@jsverse/transloco";
import { DialogResult } from "@progress/kendo-angular-dialog";
import { RemoveTagEvent } from "@progress/kendo-angular-dropdowns";
import { GroupResult, groupBy } from "@progress/kendo-data-query";
import { Enum_ColonnaData_FiltroRicerca, Enum_Entita_FiltroRicerca, Enum_FiltroDestinazioneUso_FiltroRicerca, Enum_FiltroOperatoreLogico_FiltroRicerca, Enum_FiltroOperazioniSelezionate_FiltroRicerca, Enum_FiltroPoligoni_FiltroRicerca, Enum_FiltroRipartoCatasto_FiltroRicerca, Enum_TipoConfronto_FiltroRicerca, Enum_TipoMostra_FiltroRicerca, enum_Security_Attivita, enum_TipoImpresaGerarchia } from "app/Model/TipiEnumerativi";
import { DialogBooleanResult, Dialog_Type, GiasDialogService } from "app/Service/gias-dialog.service";
import { VariabiliInSessione_NG } from "app/Service/master.service";
import { AnagrafeClient, BudgetClient, Cultivar_GestioneFiltroUtente_Leggi_IN, GisClient, LeggiBudgetTestate, LeggiGruppiVarietali_IN, LeggiOperazioni_IN, LeggiServiziStati_IN, LeggiServizi_IN, LeggiSpecieVegetali_IN, MetaschemaClient, RispostaStandard, UtentiClient } from "app/Service/net-core6-api.service";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { PermessiUtenteService } from "app/Service/permessi-utente.service";
import { GiiasMultiselectTemplateSComponent } from 'gias-ui-kit';
import { AGRODATAFINE, AGRODATAINIZIO } from "app/Model/CostantiPersonalizzate";
import {FunzioniComuniService} from "../../../Service/FunzioniComuni.service";
import { ObjParametriAgenda } from 'gias-ui-kit';
import { FormFiltriTemporali, FormFiltroTemporale, FormFiltroTemporaleBreve, FormFiltroRicercaConfig, FormFiltriAziende, FormFiltriPianoColturale, FormFiltriMovimenti, FormCaricaDati } from "app/filtro-ricerca/utils";
import { BehaviorSubject, forkJoin, lastValueFrom, map, of } from "rxjs";
import { PivaRealeService } from "app/anagrafica/imprese/piva-reale.service";

export class ProseguiSelezionati {

    TipoMostra: Enum_TipoMostra_FiltroRicerca;
    Chiavi: string[];
    ParametriFiltro: any;
    JsonRichiesta: string;
    VariabiliInSessione: VariabiliInSessione_NG;
}

export class SignalOverflowRows {
    overflow: boolean;
    nRows: number;

    constructor(overflow: boolean = false, nRows: number = 0) {
        this.overflow = overflow;
        this.nRows = nRows;
    }
}

@Injectable({ providedIn: 'root' })
export class FiltroRicercaService implements OnDestroy {

    public tab_Carica_Dati_Visibility = {
        ImpreseReferenti_Visibile: false,
        LegaleRappresentante_Visibile: false,
        IndirizzoAzienda_Visibile: false,
        IndirizzoCentroAziendale_Visibile: false,
        IndirizziPianoColturale_Visibile: false,
        EstremiCatastaliCentroAziendale_Visibile: false,
        DatiCatastaliAppezzamento_Visibile: false,
        AltriDatiAzienda_Visibile: false,
        AltriDatiCentro_Visibile: false,
        AltriDatiPianoColturale_Visibile: false,
        AltriDatiFabbricato_Visibile: false,
        CatastoCampo_Visibile: false,
        AltriDatiCampo_Visibile: false,
        GISImpianto_Visibile: false,
        Servizi_Visibile: false
    };

    ListaFiltroDestinazioniDuso: any = Object.entries(Enum_FiltroDestinazioneUso_FiltroRicerca)
        .filter(([key]) => isNaN(Number(key)))
        .map(([key, value]) => ({ nome: (+value != Enum_FiltroDestinazioneUso_FiltroRicerca.Tutto) ? this.transloco.translate(key) : '', id: +value }));

    ListeCategorieEsito: any = Object.entries(Enum_TipoMostra_FiltroRicerca)
        .filter(([key, value]) => isNaN(Number(key)))
        .map(([key, value]) => ({ nomeCat: this.transloco.translate(key), idCat: +value, visible: true }));

    ListaPoligoniGIS: any = Object.entries(Enum_FiltroPoligoni_FiltroRicerca)
        .filter(([key]) => isNaN(Number(key)))
        .map(([key, value]) => ({ Descrizione: (+value != Enum_FiltroPoligoni_FiltroRicerca.Tutto) ? this.transloco.translate(key) : '', Value: +value }));

    ListaRipartoCatastale: any = Object.entries(Enum_FiltroRipartoCatasto_FiltroRicerca)
        .filter(([key]) => isNaN(Number(key)))
        .map(([key, value]) => ({ Descrizione: (+value != Enum_FiltroRipartoCatasto_FiltroRicerca.Tutto) ? this.transloco.translate(key) : '', Value: +value }));

    ListaFiltroOperazioniSelezionate: any = Object.entries(Enum_FiltroOperazioniSelezionate_FiltroRicerca)
        .filter(([key, value]) => isNaN(Number(key)))
        .map(([key, value]) => ({ Descrizione: (+value != Enum_FiltroOperazioniSelezionate_FiltroRicerca.Tutto) ? this.transloco.translate(key) : '', Value: +value }));

    ListaFiltroOperatoreLogicoZone: any = Object.entries(Enum_FiltroOperatoreLogico_FiltroRicerca)
        .filter(([key]) => isNaN(Number(key)))
        .map(([key, value]) => ({ Descrizione: (+value == Enum_FiltroOperatoreLogico_FiltroRicerca.AND_TutteLeCondizioniTrue) ? this.transloco.translate('AND_FiltroOperatoreLogicoZone') : this.transloco.translate('OR_FiltroOperatoreLogicoZone'), Value: +value }));

    ListaFiltroOperatoreLogicoTemporali: any = Object.entries(Enum_FiltroOperatoreLogico_FiltroRicerca)
        .filter(([key]) => isNaN(Number(key)))
        .map(([key, value]) => ({ Descrizione: (+value == Enum_FiltroOperatoreLogico_FiltroRicerca.AND_TutteLeCondizioniTrue) ? this.transloco.translate('AND_FiltroOperatoreLogicoTemporali') : this.transloco.translate('OR_FiltroOperatoreLogicoTemporali'), Value: +value }));

    ListaEntita: any = Object.entries(Enum_Entita_FiltroRicerca)
        .filter(([key, value]) => isNaN(Number(key)) && (value == Enum_Entita_FiltroRicerca.Appezzamento || value == Enum_Entita_FiltroRicerca.Impianto || value == Enum_Entita_FiltroRicerca.Esercizio))
        .map(([key, value]) => ({ nome: this.transloco.translate(key), id: +value }));

    ListaValidita: any = Object.entries(Enum_ColonnaData_FiltroRicerca)
        .filter(([key]) => isNaN(Number(key)))
        .map(([key, value]) => ({ nome: this.transloco.translate(key), id: +value }));

    ListaTipoConfronto: any = Object.entries(Enum_TipoConfronto_FiltroRicerca)
        .filter(([key]) => isNaN(Number(key)))
        .map(([key, value]) => ({
            nome: this.estraiDescrizione_TipoConfronto(+value),
            id: +value
        }));

    ListaTipiImpresa: any = Object.entries(enum_TipoImpresaGerarchia)
        .filter(([key]) => isNaN(Number(key)))
        .map(([key, value]) => ({ nome: this.transloco.translate(key), id: +value }));

    estraiDescrizione_TipoConfronto(value: number) {
        switch (value) {
            case Enum_TipoConfronto_FiltroRicerca.Maggiore:
                return this.transloco.translate("gt")
            case Enum_TipoConfronto_FiltroRicerca.MaggioreUguale:
                return this.transloco.translate("gte")
            case Enum_TipoConfronto_FiltroRicerca.Minore:
                return this.transloco.translate("lt")
            case Enum_TipoConfronto_FiltroRicerca.MinoreUguale:
                return this.transloco.translate("lte")
            case Enum_TipoConfronto_FiltroRicerca.CompresoFra:
                return this.transloco.translate("CompresoFra")
        }
    }

    ListaZone: any[];
    ListaDestinazioniDuso: any[] = new Array();
    ListaGruppiVegetali: any[] = new Array();
    ListaUtilizzoTerreno: any[] = new Array();
    ListaContributiACA: any[] = new Array();
    ListaServizi: any[];
    ListaAnomalie: any[];

    ListaCoopImpreseReferenti: any[];

    defaultServizio = { Servizio_Cod: 0, Servizio_Des: "" };

    formFilters = CreateFormGroup(this.fb, this.InitializeFormFiltroRicerca());
    onLoad: boolean = true;
    permessoBudget: boolean = false;

    gridIDstring: BehaviorSubject<string> = new BehaviorSubject<string>(null);

    inizioCampagna;
    fineCampagna;

    ListaSpecieCompleta = new Array();
    ListaGruppiVarietali = new Array();
    ListaOperazioni = new Array();
    ListaStatiPratica = new Array();
    ListaBudgets = new Array();

    showIconUserSpecie: boolean = false;
    showIconUserOperazioni: boolean = false;
    displayNoViews: boolean = false;
    applyLoadCacheViewFirstTime: boolean = true;
    showSetDefaultFiltersButton: boolean = true;
    showContributiACA: boolean = false;

    filtroJSONBeforeChanges: any;

    selectedRows: any[] = [];

    paramAgenda: ObjParametriAgenda;
    paramFiltroRicercaNG: any;

    public overflowSelectTopRowsSignal = signal<SignalOverflowRows>({overflow: false, nRows: 0});

    destinazioniUsoObs;
    gruppiVegetaliObs;
    utilizzoTerrenoObs;
    specieObs;

    constructor(private fb: FormBuilder,
        private transloco: TranslocoService,
        private formFieldsService: MetaschemaClient,
        private anagrafeService: AnagrafeClient,
        private dialogService: GiasDialogService,
        private budgetService: BudgetClient,
        private permessiUtenteService: PermessiUtenteService,
        private objParametriAgendaService: ObjParametriAgendaService,
        private annataAgrariaService: UtentiClient,
        private gisService: GisClient,
        private funzionicomuniservice: FunzioniComuniService,
        private pivaRealeService: PivaRealeService) {

        //inizializzo tutto ciò che è utile al form dei filtri
        this.permessoBudget = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Budget_Gestione_Anagrafiche_CdG, 0);

        if (!this.permessoBudget)
            this.ListeCategorieEsito.splice(this.ListeCategorieEsito.findIndex(item => item.idCat == Enum_TipoMostra_FiltroRicerca.PianoColturaleBudget), 1);

        this.annataAgrariaService.utentiGetAnnataAgraria().subscribe(r => {
            let objRisp = JSON.parse(r.RispostaStringa);
            this.inizioCampagna = objRisp.inizioCampagna;
            this.fineCampagna = objRisp.fineCampagna;
        });


        //carico le DDL

        this.anagrafeService.anagrafeGetImpresePadre().subscribe(r => {
            this.ListaCoopImpreseReferenti = JSON.parse(r.RispostaStringa);
        });

        this.anagrafeService.anagrafeGetZone(0).subscribe(r => {
            this.ListaZone = JSON.parse(r.RispostaStringa);
        });

        this.anagrafeService.anagrafeGetCodiciDestinazioniDUso().subscribe(r => {
            this.ListaDestinazioniDuso = JSON.parse(r.RispostaStringa);
        });

        this.formFieldsService.metaschemaGetGruppiVegetaliFiltroUtente().subscribe(r => {
            this.ListaGruppiVegetali = JSON.parse(r.RispostaStringa);
        });

        this.formFieldsService.metaschemaGetMetodiDiProduzione().subscribe(r => {
            this.ListaUtilizzoTerreno = JSON.parse(r.RispostaStringa);
            this.ListaUtilizzoTerreno.forEach(item => item.Descrizione = this.transloco.translate(item.Descrizione));
        });

        this.anagrafeService.anagrafeGetContributiACA().subscribe(r => {

            let arrayResponse = JSON.parse(r.RispostaStringa);
            if (arrayResponse.length > 0) {
                this.showContributiACA = true;
                this.ListaContributiACA = arrayResponse;
            }

        });

        const paramServizi = {
            Servizio_Cod: 0,
            Servizio_Des: ""
        } as LeggiServizi_IN;

        this.formFieldsService.metaschemaGetServizi(paramServizi).subscribe(r => {
            this.ListaServizi = JSON.parse(r.RispostaStringa);
            this.ListaServizi.unshift(this.defaultServizio);
        });

        this.gisService.gisGetGISProcessingAlgorithmsCleaningAlgorithm().subscribe(r => {
            this.ListaAnomalie = JSON.parse(r.RispostaStringa);
        });

        let paramSpecie = {
            Veg_Cod: 0,
            Gru_Cod: 0,
            LetteraIniziale: "",
            StringaCerca: ""
        } as LeggiSpecieVegetali_IN;

        this.formFieldsService.metaschemaGetSpecieFiltroUtente(paramSpecie).subscribe(r => {
            let objResp = JSON.parse(r.RispostaStringa);
            this.ListaSpecieCompleta = objResp.DataTable;
            this.showIconUserSpecie = objResp.VisibilitaApplicata;

            //se ho caricato una specie all'apertura del Filtrone
            const arrSpecie = this.formFilters.get('FiltriPianoColturale').get('Specie').getRawValue().map(item => item.Veg_Cod);
            if (arrSpecie.length > 0)
                this.formFilters.get('FiltriPianoColturale').get('Specie').patchValue(this.ListaSpecieCompleta.filter(item => arrSpecie.includes(item.Veg_Cod)));
        });

    }

    //funzione scritta perché è possibile che un chiamante arrivi con parametri da impostare sui filtri e potrebbero non essere stati ancora caricati
    //alcuni array funzionali
    waitResultBeforeFillParametriFiltroRicerca() {

        this.paramAgenda = this.objParametriAgendaService.getObjParamValue();

        this.paramFiltroRicercaNG = (this.paramAgenda.GenericObj_string !== '') ? JSON.parse(this.paramAgenda.GenericObj_string) : '';

        if (this.ListaSpecieCompleta.length == 0) {

            forkJoin({
                specie: this.specieObs
              }).subscribe({

                next: (result) => {
                                    let objResp = JSON.parse((result.specie as RispostaStandard).RispostaStringa);
                                    this.ListaSpecieCompleta = objResp.DataTable;
                                    this.showIconUserSpecie = objResp.VisibilitaApplicata;

                                    if (this.paramFiltroRicercaNG !== '')
                                        this.setFormFieldsForChiamante(this.paramFiltroRicercaNG);
                },

                error: (error) => {
                                    console.error('Errore durante le chiamate HTTP:', error);
                }

            });

        } else {
                if (this.paramFiltroRicercaNG !== '')
                    this.setFormFieldsForChiamante(this.paramFiltroRicercaNG);
        }

    }

    resetFormArrayFiltriTemporali() {
        let filtriData = this.formFilters.get('FiltriTemporali').get('FiltriData') as FormArray;
        //sbianco i filtriData

        while (filtriData.length !== 0) {
            filtriData.removeAt(0);
        }

        filtriData.push(this.fb.group({
            Entita: [null],
            ColonnaData: [null],
            TipoConfronto: [null],
            ModalitaFiltroData: [false],
            Date: this.fb.group({
                inizio: [null],
                fine: [null]
            })
        }));
    }

    prepareJSONforView(): string {
        const formValues = this.formFilters.value;

        return JSON.stringify(formValues);
    }

    loadListaBudgets() {
        const param = {
            Piva: this.objParametriAgendaService.getObjParamValue().Piva,
            Id_Budget: 0,
            IncludiPubblici: true
        } as LeggiBudgetTestate;

        this.budgetService.budgetGetTestateBudget(param).subscribe(r => {
            this.ListaBudgets = JSON.parse(r.RispostaStringa);
        });
    }

    //chiamata per popolare la multiselect delle Specie
    openMultiSelectSpecie(multiEl: GiiasMultiselectTemplateSComponent) {

        multiEl.loading = true;

        //controllo il gruppoVegetale
        let arrGruppoVegetale = this.formFilters.get('FiltriPianoColturale').get('GruppoVegetale').value.map(item => item['Gru_Cod']);
            this.fillListItemsSpecie(multiEl, arrGruppoVegetale);
    }

    fillListItemsSpecie(multiEl, arrGruppoVegetale) {

        if (arrGruppoVegetale.length > 0) // filtro per Gru_Cod
            multiEl.listItems = groupBy(this.ListaSpecieCompleta.filter(item => arrGruppoVegetale.includes(item.Gru_Cod)), [{ field: 'Gru_Des' }]);
        else
            multiEl.listItems = groupBy(this.ListaSpecieCompleta, [{ field: 'Gru_Des' }]);

        multiEl.listItemsNoFiltered = multiEl.listItems;
        multiEl.loading = false;
    }

    onRemoveItemSpecie(removedItem) {

        let arrVarieta = (this.formFilters.get('FiltriPianoColturale').get('Varieta')) ? this.formFilters.get('FiltriPianoColturale').get('Varieta').value : new Array();

        if (arrVarieta) {
            this.formFilters.get('FiltriPianoColturale').get('Varieta').patchValue(arrVarieta.filter(item => item.Veg_Cod !== removedItem.dataItem.Veg_Cod));
        }

    }

    onRemoveAll(values, formGroup, multiToClear: string[]) {
        if (values.length == 0) {
            multiToClear.forEach(item => {
                this.formFilters.get(formGroup).get(item).patchValue([]);
            });
        }
    }

    fillFormArray(objViewJSON) {

        if (objViewJSON && objViewJSON.length > 0) {

            let filtriData = this.formFilters.get('FiltriTemporali').get('FiltriData') as FormArray;

            //sbianco i filtriData
            filtriData.clear();

            objViewJSON.forEach(item => {

                const itemGroup = this.fb.group({
                    Entita: item.Entita,
                    ColonnaData: item.ColonnaData,
                    TipoConfronto: item.TipoConfronto,
                    ModalitaFiltroData: item.ModalitaFiltroData,
                    Date: this.fb.group({
                        inizio: (item.ModalitaFiltroData) ? ((item.TipoConfronto?.id == Enum_TipoConfronto_FiltroRicerca.CompresoFra) ? new Date(this.inizioCampagna) : new Date()) : item.Date?.inizio,
                        fine: (item.ModalitaFiltroData && item.TipoConfronto?.id == Enum_TipoConfronto_FiltroRicerca.CompresoFra) ? new Date(this.fineCampagna) : item.Date?.fine,
                    })
                });

                filtriData.push(itemGroup);
            });
        }
    }

    clearFormArray() {
        let formGroupTemporali = this.formFilters.get('FiltriTemporali') as FormGroup;
        formGroupTemporali.setControl('FiltriData', new FormArray([this.fb.group(
            {
                Entita: [null],
                ColonnaData: [null],
                TipoConfronto: [null],
                ModalitaFiltroData: [false],
                Date: this.fb.group({
                    inizio: [null],
                    fine: [null]
                })
            }
        )]));

        this.showSetDefaultFiltersButton = true;
    }

    //chiamata per popolare la multiselect delle Varietà
    openMultiSelectVarieta(multiEl: GiiasMultiselectTemplateSComponent) {

        //se abbiamo già delle specie selezionate, prendiamo quelle
        let arrVegCod = (this.formFilters.get('FiltriPianoColturale').get('Specie')) ? this.formFilters.get('FiltriPianoColturale').get('Specie').value.map(item => item['Veg_Cod']) : new Array();

        if (arrVegCod.length == 0) {
            multiEl.listItems = arrVegCod;
        } else {

            multiEl.loading = true;

            let params = {
                Veg_Cod_List: arrVegCod,
                Cul_Cod: 0,
                Cerca_CulDes: "",
                ControllaLaVisibilitaDelleSpecie: false
            } as Cultivar_GestioneFiltroUtente_Leggi_IN;

            this.formFieldsService.metaschemaGetCultivar(params).subscribe(r => {
                let listaVarieta = JSON.parse(r.RispostaStringa);
                let groupedData: GroupResult[] = groupBy(listaVarieta, [
                    { field: "Veg_des" },
                ]);
                multiEl.listItems = groupedData;
                multiEl.listItemsNoFiltered = multiEl.listItems;
                multiEl.loading = false;
            });
        }
    }

    setFiltriTemporaliMenu() {

        let filtriTemporaliObj = new FormFiltriTemporali();
        filtriTemporaliObj.FiltriData = new Array<FormFiltroTemporale>;

        filtriTemporaliObj.FiltriData.push(new FormFiltroTemporale(this.ListaEntita.find(item => item.id == Enum_Entita_FiltroRicerca.Esercizio),
                                                                    this.ListaValidita.find(item => item.id == Enum_ColonnaData_FiltroRicerca.ValiditaInizio),
                                                                    this.ListaTipoConfronto.find(item => item.id == Enum_TipoConfronto_FiltroRicerca.MinoreUguale),
                                                                    true,
                                                                    new FormFiltroTemporaleBreve(AGRODATAINIZIO, AGRODATAFINE)));

        filtriTemporaliObj.FiltriData.push(new FormFiltroTemporale(this.ListaEntita.find(item => item.id == Enum_Entita_FiltroRicerca.Esercizio),
                                                                    this.ListaValidita.find(item => item.id == Enum_ColonnaData_FiltroRicerca.ValiditaFine),
                                                                    this.ListaTipoConfronto.find(item => item.id == Enum_TipoConfronto_FiltroRicerca.MaggioreUguale),
                                                                    true,
                                                                    new FormFiltroTemporaleBreve(AGRODATAINIZIO, AGRODATAFINE)));

        filtriTemporaliObj.OperatoreLogico = this.ListaFiltroOperatoreLogicoTemporali.find(item => item.Value == Enum_FiltroOperatoreLogico_FiltroRicerca.AND_TutteLeCondizioniTrue);
        //this.formService.formFilters.get('FiltriTemporali').patchValue(formObj.FiltriTemporali, { emitEvent: false });

        this.fillFormArray(filtriTemporaliObj.FiltriData);
        this.formFilters.get('FiltriTemporali').get('OperatoreLogico').patchValue(this.ListaFiltroOperatoreLogicoTemporali.find(item => item.Value == Enum_FiltroOperatoreLogico_FiltroRicerca.AND_TutteLeCondizioniTrue));
        //this.formFilters.get('FiltriTemporali').patchValue(filtriTemporaliObj);
        this.showSetDefaultFiltersButton = false;
    }

    //setta i campi di un oggetto FormFiltroRicercaConfig e lo restituisce, in modo da poterli poi patchare al Form vero e proprio
    private getFormFieldsObj(paramFiltro): FormFiltroRicercaConfig {
        
        let formObj = new FormFiltroRicercaConfig();

        if (paramFiltro.Piva) {
            formObj.FiltriAziende = new FormFiltriAziende();
            formObj.FiltriAziende.Piva = paramFiltro.Piva;
        }

        if (paramFiltro.FiltriPianoColturale) {
            formObj.FiltriPianoColturale = new FormFiltriPianoColturale();

            formObj.FiltriPianoColturale.Specie = new Array();
            paramFiltro.FiltriPianoColturale.Specie.forEach(item => {
                formObj.FiltriPianoColturale.Specie.push({"Veg_Cod": item, "Veg_Des": ''})
            });
        }

        if (paramFiltro.FiltriMovimenti) {
          formObj.FiltriMovimenti = new FormFiltriMovimenti();
          formObj.FiltriMovimenti.FiltroOperazioni = this.ListaFiltroOperazioniSelezionate.find(item => item.Value == paramFiltro.FiltriMovimenti.FiltroOperazioni);
          formObj.FiltriMovimenti.DataMovimento = new FormFiltroTemporaleBreve(paramFiltro.FiltriMovimenti.DataMovimento.inizio, paramFiltro.FiltriMovimenti.DataMovimento.fine);
        }

        if (paramFiltro.FiltriTemporali) {
          formObj.FiltriTemporali = new FormFiltriTemporali();
          formObj.FiltriTemporali.FiltriData = new Array<FormFiltroTemporale>;

          paramFiltro.FiltriTemporali.FiltriData.forEach(elemFiltriData => {
                formObj.FiltriTemporali.FiltriData.push(new FormFiltroTemporale(this.ListaEntita.find(item => item.id == elemFiltriData.Entita),
                                                                                this.ListaValidita.find(item => item.id == elemFiltriData.ColonnaData),
                                                                                this.ListaTipoConfronto.find(item => item.id == elemFiltriData.TipoConfronto),
                                                                                elemFiltriData.ModalitaFiltroData,
                                                                                new FormFiltroTemporaleBreve(elemFiltriData.Date.inizio, elemFiltriData.Date.fine)));
          });

          formObj.FiltriTemporali.OperatoreLogico = this.ListaFiltroOperatoreLogicoTemporali.find(item => item.Value == paramFiltro.FiltriTemporali.OperatoreLogicoFiltriTemporali);

          this.fillFormArray(formObj.FiltriTemporali.FiltriData);
        }

        if (paramFiltro.CaricaDatiAggiuntivi) {
            formObj.CaricaDati = structuredClone(paramFiltro.CaricaDatiAggiuntivi as FormCaricaDati);
        }

        return formObj;
    }

    async setFormFieldsForChiamante(paramFiltro) {
        let pivaReale = await lastValueFrom(this.pivaRealeService.getPivaReale(paramFiltro.Piva));
        paramFiltro.Piva = pivaReale;
        this.formFilters.patchValue(this.getFormFieldsObj(paramFiltro), { emitEvent: false });
    }

    openMultiSelectGruppoVarietale(multiEl: GiiasMultiselectTemplateSComponent) {

        let arrVegCod = this.formFilters.get('FiltriPianoColturale').get('Specie').value.map(item => item['Veg_Cod']);

        if (arrVegCod.length == 0) {
            multiEl.listItems = arrVegCod;
        } else {
            if (this.ListaGruppiVarietali.length == 0) {

                let params = {
                    Veg_Cod_List: []
                } as LeggiGruppiVarietali_IN;

                this.formFieldsService.metaschemaGetGruppiVarietali(params).subscribe(r => {
                    this.ListaGruppiVarietali = JSON.parse(r.RispostaStringa);
                    multiEl.listItems = groupBy(this.ListaGruppiVarietali.filter(item => arrVegCod.includes(item.Veg_Cod)), [{ field: 'Veg_Des' }]);
                    multiEl.loading = false;
                });

            } else {
                multiEl.listItems = groupBy(this.ListaGruppiVarietali.filter(item => arrVegCod.includes(item.Veg_Cod)), [{ field: 'Veg_Des' }]);
                multiEl.loading = false;
            }
        }
    }

    onRemoveItemGruppoVegetale(removedItem) {
        let arrSpecie = this.formFilters.get('FiltriPianoColturale').get('Specie').value;

        if (arrSpecie) {
            this.formFilters.get('FiltriPianoColturale').get('Specie').patchValue(arrSpecie.filter(item => item.Gru_Cod !== removedItem.dataItem.Gru_Cod));

            let specieToRemove = arrSpecie.filter(item => item.Gru_Cod == removedItem.dataItem.Gru_Cod);

            specieToRemove.forEach(item => {
                let param = new RemoveTagEvent(item);
                this.onRemoveItemSpecie(param);
            });
        }
    }

    openMultiSelectZone(multiEl: GiiasMultiselectTemplateSComponent) {

        multiEl.loading = true;

        if (!multiEl.listItems) {
            this.anagrafeService.anagrafeGetZone(0).subscribe(r => {
                multiEl.listItems = JSON.parse(r.RispostaStringa);
                multiEl.loading = false;
            });
        } else
            multiEl.loading = false;

    }

    openMultiSelectDestinazioneUso(multiEl: GiiasMultiselectTemplateSComponent) {

        multiEl.loading = true;

        if (!multiEl.listItems) {
            this.anagrafeService.anagrafeGetCodiciDestinazioniDUso().subscribe(r => {
                multiEl.listItems = JSON.parse(r.RispostaStringa);
                multiEl.loading = false;
            });
        } else
            multiEl.loading = false;
    }

    fillListaStatiPratica(servizioCod: number) {

        const param = {
            Servizio_Cod: servizioCod,
            Stato_Cod: 0,
            Stato_Des: ""
        } as LeggiServiziStati_IN;

        this.formFieldsService.metaschemaGetServiziStati(param).subscribe(r => {
            this.ListaStatiPratica = JSON.parse(r.RispostaStringa);
        });

    }

    openMultiSelectTipoOperazioni(multiEl: GiiasMultiselectTemplateSComponent) {

        multiEl.loading = true;

        if (this.ListaOperazioni.length == 0) {

            let param = {
                GruppoOperazioni: [1, 2, 3, 4],
                Operazioni: []
            } as LeggiOperazioni_IN;

            this.formFieldsService.metaschemaGetOperazioniFiltroUtente(param).subscribe(r => {
                let objResp = JSON.parse(r.RispostaStringa);
                this.ListaOperazioni = objResp.DataTable;
                this.showIconUserOperazioni = objResp.VisibilitaApplicata;
                this.fillMultiElTipoOperazioni(multiEl);
                multiEl.loading = false;
            });

        } else {

            if (!multiEl.listItems) {
                this.fillMultiElTipoOperazioni(multiEl);
            }
            multiEl.loading = false;
        }

    }

    getTipoMostraString(tipoMostra: Enum_TipoMostra_FiltroRicerca): string {

        let tipo;

        switch(tipoMostra) {
            case Enum_TipoMostra_FiltroRicerca.Aziende:
                tipo = "azienda";
            break;
            case Enum_TipoMostra_FiltroRicerca.CentriAziendali:
                tipo = "centro";
            break;
            case Enum_TipoMostra_FiltroRicerca.Campi:
                tipo = "campo";
            break;
            case Enum_TipoMostra_FiltroRicerca.Appezzamenti:
                tipo = "appezza";
            break;
            case Enum_TipoMostra_FiltroRicerca.Impianti:
                tipo = "impianto";
            break;
            case Enum_TipoMostra_FiltroRicerca.Esercizi:
                tipo = "esercizio";
            break;
            case Enum_TipoMostra_FiltroRicerca.Movimenti:
                tipo = "movimento";
            break;
            case Enum_TipoMostra_FiltroRicerca.Fabbricati:
                tipo = "fabbricato";
            break;
        }

        return tipo;
    }

    toPrintOrExport() {

        let ri = {
            tipo: this.getTipoMostraString(this.formFilters.get('CategoriaEsito').value.idCat),
            chiavi: this.selectedRows.map(item => item.dataItem.chiave),
            tipoEntita: this.formFilters.get('CategoriaEsito').value.idCat,
            parametri: [['precedente', AGRODATAINIZIO], ['successivo', AGRODATAFINE]]
        }

        let objPostMessage = {
            messaggio: "chiudiWindowGiasNG",
            contesto: 3, // enum contestoPostMessage.FiltroneImpianti (Angular)
            inData: ri
        }

        window.parent.postMessage(objPostMessage, this.funzionicomuniservice.getOrigins());
    }

    fillMultiElTipoOperazioni(multiEl) {
        let unique = [];
        this.ListaOperazioni.map(item => ({
            GRU_COD: item.GRU_COD,
            GRU_DES: item.GRU_DES
        })).forEach(item => {
            const found = unique.find(element => element.GRU_COD == item.GRU_COD);
            if (!found)
                unique.push(item);
        });
        multiEl.listItems = unique;
    }

    onRemoveItemTipoOperazioni(removedItem) {

        let arrOperazioni = (this.formFilters.get('FiltriMovimenti').get('Operazioni')) ? this.formFilters.get('FiltriMovimenti').get('Operazioni').value : new Array();

        if (arrOperazioni.length > 0) {
            this.formFilters.get('FiltriMovimenti').get('Operazioni').patchValue(arrOperazioni.filter(item => item.GRU_COD !== removedItem.dataItem.GRU_COD));
        }

    }


    openMultiSelectOperazioni(multiEl: GiiasMultiselectTemplateSComponent) {

        let arrTipoOperazioni = (this.formFilters.get('FiltriMovimenti').get('GruppoOperazioni')) ? this.formFilters.get('FiltriMovimenti').get('GruppoOperazioni').value.map(item => item['GRU_COD']) : new Array();

        multiEl.loading = true;

        if (this.ListaOperazioni.length == 0) {

            let param = {
                GruppoOperazioni: [1, 2, 3, 4],
                Operazioni: []
            } as LeggiOperazioni_IN;

            this.formFieldsService.metaschemaGetOperazioniFiltroUtente(param).subscribe(r => {
                let objResp = JSON.parse(r.RispostaStringa);
                this.ListaOperazioni = objResp.DataTable;
                this.showIconUserOperazioni = objResp.VisibilitaApplicata;
                this.fillMultiElOperazioni(multiEl, arrTipoOperazioni);
                multiEl.loading = false;
            });

        } else {
            this.fillMultiElOperazioni(multiEl, arrTipoOperazioni);
            multiEl.loading = false;
        }
    }

    fillMultiElOperazioni(multiEl, arrTipoOperazioni) {
        if (arrTipoOperazioni.length > 0)
            multiEl.listItems = groupBy(this.ListaOperazioni.filter(item => arrTipoOperazioni.includes(item.GRU_COD)), [{ field: 'GRU_DES' }]);
        else
            multiEl.listItems = groupBy(this.ListaOperazioni, [{ field: 'GRU_DES' }]);

        multiEl.listItemsNoFiltered = multiEl.listItems;
    }

    createDialogWindow(title: string, content: string, preventAction: (p: DialogBooleanResult) => boolean = () => { return false }): Promise<DialogResult> {
        return new Promise((resolve, reject) => {
            let dialog = this.dialogService.dialogMessageRef(
                title,
                content,
                [
                    { text: this.transloco.translate('Conferma'), primary: true, returnObj: true },
                    { text: this.transloco.translate('Annulla'), returnObj: false }
                ], 'auto', 'auto',
                preventAction,
                Dialog_Type.info
            );

            if (typeof content == 'string')
                dialog.content.location.nativeElement.style.whiteSpace = 'pre-line';
            dialog.result.subscribe(res => resolve(res))
        })
    }

    setOverflowSelectTopRows(value: SignalOverflowRows): void {
        this.overflowSelectTopRowsSignal.set(value);
    }

    fieldsNotCompareToDefault = ['CategoriaEsito', 'CaricaDati', 'TipoFiltroInizio', 'TipoFiltroFine', 'FiltroZone', 'FiltriTemporali', 'Budget'];

    hasChangedValues(currentValues: any, defaultValues: any = this.InitializeFormFiltroRicerca()): boolean {
        for (const key in currentValues) {
            if (currentValues.hasOwnProperty(key) && this.fieldsNotCompareToDefault.indexOf(key) === -1) {
                if (typeof currentValues[key] === 'object' && !(currentValues[key] instanceof Date) && currentValues[key] !== null) {
                    if (defaultValues[key] == null || this.hasChangedValues(currentValues[key], defaultValues[key])) {
                        return true;
                    }
                } else if (currentValues[key] instanceof Date && defaultValues[key] instanceof Date) {
                    if (currentValues[key].getTime() !== defaultValues[key].getTime()) {
                        return true;
                    }
                } else {
                    if (currentValues[key] !== defaultValues[key] && currentValues[key] !== '' && typeof currentValues[key] !== 'undefined') {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    get listaTipiMostra() {
        return this.ListeCategorieEsito.filter(item => item.visible);
    }

    set_tab_Carica_Dati_Visibility() {
        let CategoriaEsito = this.formFilters.get("CategoriaEsito").getRawValue();

        if (CategoriaEsito) {

            this.tab_Carica_Dati_Visibility.ImpreseReferenti_Visibile = true;
            this.tab_Carica_Dati_Visibility.LegaleRappresentante_Visibile = true;
            this.tab_Carica_Dati_Visibility.AltriDatiAzienda_Visibile = true;

            switch (CategoriaEsito.idCat) {
                case Enum_TipoMostra_FiltroRicerca.Aziende:
                    this.tab_Carica_Dati_Visibility.IndirizzoAzienda_Visibile = true;
                    this.formFilters.get("CaricaDati").get("IndirizzoAzienda").patchValue(true);
                    this.tab_Carica_Dati_Visibility.IndirizzoCentroAziendale_Visibile = false;
                    this.formFilters.get("CaricaDati").get("IndirizzoCentroAziendale").patchValue(false);
                    this.tab_Carica_Dati_Visibility.IndirizziPianoColturale_Visibile = false;
                    this.tab_Carica_Dati_Visibility.EstremiCatastaliCentroAziendale_Visibile = false;
                    this.tab_Carica_Dati_Visibility.AltriDatiCentro_Visibile = false;
                    this.tab_Carica_Dati_Visibility.AltriDatiPianoColturale_Visibile = false;
                    this.tab_Carica_Dati_Visibility.DatiCatastaliAppezzamento_Visibile = false;
                    this.tab_Carica_Dati_Visibility.AltriDatiFabbricato_Visibile = false;
                    this.tab_Carica_Dati_Visibility.CatastoCampo_Visibile = false;
                    this.tab_Carica_Dati_Visibility.AltriDatiCampo_Visibile = false;
                    this.tab_Carica_Dati_Visibility.GISImpianto_Visibile = false;
                    this.tab_Carica_Dati_Visibility.Servizi_Visibile = true;
                    break;
                case Enum_TipoMostra_FiltroRicerca.CentriAziendali:
                    this.tab_Carica_Dati_Visibility.IndirizzoAzienda_Visibile = true;
                    this.formFilters.get("CaricaDati").get("IndirizzoAzienda").patchValue(false);
                    this.tab_Carica_Dati_Visibility.IndirizzoCentroAziendale_Visibile = true;
                    this.formFilters.get("CaricaDati").get("IndirizzoCentroAziendale").patchValue(true);
                    this.tab_Carica_Dati_Visibility.IndirizziPianoColturale_Visibile = false;
                    this.tab_Carica_Dati_Visibility.EstremiCatastaliCentroAziendale_Visibile = true;
                    this.tab_Carica_Dati_Visibility.AltriDatiCentro_Visibile = true;
                    this.tab_Carica_Dati_Visibility.AltriDatiPianoColturale_Visibile = false;
                    this.tab_Carica_Dati_Visibility.DatiCatastaliAppezzamento_Visibile = false;
                    this.tab_Carica_Dati_Visibility.AltriDatiFabbricato_Visibile = false;
                    this.tab_Carica_Dati_Visibility.CatastoCampo_Visibile = false;
                    this.tab_Carica_Dati_Visibility.AltriDatiCampo_Visibile = false;
                    this.tab_Carica_Dati_Visibility.GISImpianto_Visibile = false;
                    this.tab_Carica_Dati_Visibility.Servizi_Visibile = false;
                    break;
                case Enum_TipoMostra_FiltroRicerca.Campi:
                    this.tab_Carica_Dati_Visibility.IndirizzoAzienda_Visibile = true;
                    this.formFilters.get("CaricaDati").get("IndirizzoAzienda").patchValue(false);
                    this.tab_Carica_Dati_Visibility.IndirizzoCentroAziendale_Visibile = true;
                    this.formFilters.get("CaricaDati").get("IndirizzoCentroAziendale").patchValue(false);
                    this.tab_Carica_Dati_Visibility.IndirizziPianoColturale_Visibile = false;
                    this.tab_Carica_Dati_Visibility.EstremiCatastaliCentroAziendale_Visibile = false;
                    this.tab_Carica_Dati_Visibility.AltriDatiCentro_Visibile = true;
                    this.tab_Carica_Dati_Visibility.AltriDatiPianoColturale_Visibile = false;
                    this.tab_Carica_Dati_Visibility.DatiCatastaliAppezzamento_Visibile = false;
                    this.tab_Carica_Dati_Visibility.AltriDatiFabbricato_Visibile = false;
                    this.tab_Carica_Dati_Visibility.CatastoCampo_Visibile = true;
                    this.tab_Carica_Dati_Visibility.AltriDatiCampo_Visibile = true;
                    this.tab_Carica_Dati_Visibility.GISImpianto_Visibile = false;
                    this.tab_Carica_Dati_Visibility.Servizi_Visibile = false;
                    break;
                case Enum_TipoMostra_FiltroRicerca.Appezzamenti:
                    this.tab_Carica_Dati_Visibility.IndirizzoAzienda_Visibile = true;
                    this.formFilters.get("CaricaDati").get("IndirizzoAzienda").patchValue(false);
                    this.tab_Carica_Dati_Visibility.IndirizzoCentroAziendale_Visibile = true;
                    this.formFilters.get("CaricaDati").get("IndirizzoCentroAziendale").patchValue(false);
                    this.tab_Carica_Dati_Visibility.IndirizziPianoColturale_Visibile = true;
                    this.tab_Carica_Dati_Visibility.EstremiCatastaliCentroAziendale_Visibile = false;
                    this.tab_Carica_Dati_Visibility.AltriDatiCentro_Visibile = true;
                    this.tab_Carica_Dati_Visibility.AltriDatiPianoColturale_Visibile = true;
                    this.tab_Carica_Dati_Visibility.DatiCatastaliAppezzamento_Visibile = true;
                    this.tab_Carica_Dati_Visibility.AltriDatiFabbricato_Visibile = false;
                    this.tab_Carica_Dati_Visibility.CatastoCampo_Visibile = false;
                    this.tab_Carica_Dati_Visibility.AltriDatiCampo_Visibile = true;
                    this.tab_Carica_Dati_Visibility.GISImpianto_Visibile = false;
                    this.tab_Carica_Dati_Visibility.Servizi_Visibile = false;
                    break;              
                case Enum_TipoMostra_FiltroRicerca.Esercizi:
                    this.tab_Carica_Dati_Visibility.IndirizzoAzienda_Visibile = true;
                    this.formFilters.get("CaricaDati").get("IndirizzoAzienda").patchValue(false);
                    this.tab_Carica_Dati_Visibility.IndirizzoCentroAziendale_Visibile = true;
                    this.formFilters.get("CaricaDati").get("IndirizzoCentroAziendale").patchValue(false);
                    this.tab_Carica_Dati_Visibility.IndirizziPianoColturale_Visibile = true;
                    this.tab_Carica_Dati_Visibility.EstremiCatastaliCentroAziendale_Visibile = false;
                    this.tab_Carica_Dati_Visibility.AltriDatiCentro_Visibile = true;
                    this.tab_Carica_Dati_Visibility.AltriDatiPianoColturale_Visibile = true;
                    this.tab_Carica_Dati_Visibility.DatiCatastaliAppezzamento_Visibile = true;
                    this.tab_Carica_Dati_Visibility.AltriDatiFabbricato_Visibile = false;
                    this.tab_Carica_Dati_Visibility.CatastoCampo_Visibile = false;
                    this.tab_Carica_Dati_Visibility.AltriDatiCampo_Visibile = true;
                    this.tab_Carica_Dati_Visibility.GISImpianto_Visibile = false;
                    this.tab_Carica_Dati_Visibility.Servizi_Visibile = false;
                    break;
                case Enum_TipoMostra_FiltroRicerca.Impianti:
                case Enum_TipoMostra_FiltroRicerca.Movimenti:
                case Enum_TipoMostra_FiltroRicerca.PianoColturale:
                    this.tab_Carica_Dati_Visibility.IndirizzoAzienda_Visibile = true;
                    this.formFilters.get("CaricaDati").get("IndirizzoAzienda").patchValue(false);
                    this.tab_Carica_Dati_Visibility.IndirizzoCentroAziendale_Visibile = true;
                    this.formFilters.get("CaricaDati").get("IndirizzoCentroAziendale").patchValue(false);
                    this.tab_Carica_Dati_Visibility.IndirizziPianoColturale_Visibile = true;
                    this.tab_Carica_Dati_Visibility.EstremiCatastaliCentroAziendale_Visibile = false;
                    this.tab_Carica_Dati_Visibility.AltriDatiCentro_Visibile = true;
                    this.tab_Carica_Dati_Visibility.AltriDatiPianoColturale_Visibile = true;
                    this.tab_Carica_Dati_Visibility.DatiCatastaliAppezzamento_Visibile = true;
                    this.tab_Carica_Dati_Visibility.AltriDatiFabbricato_Visibile = false;
                    this.tab_Carica_Dati_Visibility.CatastoCampo_Visibile = false;
                    this.tab_Carica_Dati_Visibility.AltriDatiCampo_Visibile = true;
                    this.tab_Carica_Dati_Visibility.GISImpianto_Visibile = true;
                    this.tab_Carica_Dati_Visibility.Servizi_Visibile = false;
                    break;
                case Enum_TipoMostra_FiltroRicerca.PianoColturaleBudget:
                    this.tab_Carica_Dati_Visibility.IndirizzoAzienda_Visibile = true;
                    this.formFilters.get("CaricaDati").get("IndirizzoAzienda").patchValue(false);
                    this.tab_Carica_Dati_Visibility.IndirizzoCentroAziendale_Visibile = true;
                    this.formFilters.get("CaricaDati").get("IndirizzoCentroAziendale").patchValue(false);
                    this.tab_Carica_Dati_Visibility.IndirizziPianoColturale_Visibile = true;
                    this.tab_Carica_Dati_Visibility.EstremiCatastaliCentroAziendale_Visibile = false;
                    this.tab_Carica_Dati_Visibility.AltriDatiCentro_Visibile = true;
                    this.tab_Carica_Dati_Visibility.AltriDatiPianoColturale_Visibile = true;
                    this.tab_Carica_Dati_Visibility.DatiCatastaliAppezzamento_Visibile = true;
                    this.tab_Carica_Dati_Visibility.AltriDatiFabbricato_Visibile = false;
                    this.tab_Carica_Dati_Visibility.CatastoCampo_Visibile = false;
                    this.tab_Carica_Dati_Visibility.AltriDatiCampo_Visibile = true;
                    this.tab_Carica_Dati_Visibility.GISImpianto_Visibile = false;
                    this.tab_Carica_Dati_Visibility.Servizi_Visibile = false;
                    break;
                case Enum_TipoMostra_FiltroRicerca.Fabbricati:
                    this.tab_Carica_Dati_Visibility.IndirizzoAzienda_Visibile = true;
                    this.formFilters.get("CaricaDati").get("IndirizzoAzienda").patchValue(false);
                    this.tab_Carica_Dati_Visibility.IndirizzoCentroAziendale_Visibile = true;
                    this.formFilters.get("CaricaDati").get("IndirizzoCentroAziendale").patchValue(false);
                    this.tab_Carica_Dati_Visibility.IndirizziPianoColturale_Visibile = false;
                    this.tab_Carica_Dati_Visibility.EstremiCatastaliCentroAziendale_Visibile = false;
                    this.tab_Carica_Dati_Visibility.AltriDatiCentro_Visibile = true;
                    this.tab_Carica_Dati_Visibility.AltriDatiPianoColturale_Visibile = false;
                    this.tab_Carica_Dati_Visibility.DatiCatastaliAppezzamento_Visibile = false;
                    this.tab_Carica_Dati_Visibility.AltriDatiFabbricato_Visibile = true;
                    this.tab_Carica_Dati_Visibility.CatastoCampo_Visibile = false;
                    this.tab_Carica_Dati_Visibility.AltriDatiCampo_Visibile = false;
                    this.tab_Carica_Dati_Visibility.GISImpianto_Visibile = false;
                    this.tab_Carica_Dati_Visibility.Servizi_Visibile = false;
                    break;                
            }
        }
    }

    ngOnDestroy(): void {
        throw new Error("Method not implemented.");
    }

    InitializeFormFiltroRicerca(vals?: Partial<FormFiltroRicercaConfig>): FormFiltroRicercaConfig {

        return {
            FiltriAziende: {
                ImpreseReferenti: vals?.FiltriAziende?.ImpreseReferenti ?? new Array(),
                TipiImpresa: vals?.FiltriAziende?.TipiImpresa ?? new Array(),
                RagioneSociale: vals?.FiltriAziende?.RagioneSociale ?? null,
                CUAA: vals?.FiltriAziende?.CUAA ?? null,
                Piva: vals?.FiltriAziende?.Piva ?? null,
                Stati: vals?.FiltriAziende?.Stati ?? new Array(),
                Regioni: vals?.FiltriAziende?.Regioni ?? new Array(),
                Province: vals?.FiltriAziende?.Province ?? new Array(),
                Comuni: vals?.FiltriAziende?.Comuni ?? new Array(),
                Zone: vals?.FiltriAziende?.Comuni ?? new Array(),
                FiltroZone: vals?.FiltriAziende?.FiltroZone ?? this.ListaFiltroOperatoreLogicoZone.find(item => item.Value == Enum_FiltroOperatoreLogico_FiltroRicerca.AND_TutteLeCondizioniTrue),
            },

            FiltriCentriAziendali: {
                CentroAziendale: vals?.FiltriCentriAziendali?.CentroAziendale ?? null,
                Stati: vals?.FiltriCentriAziendali?.Stati ?? new Array(),
                Regioni: vals?.FiltriCentriAziendali?.Regioni ?? new Array(),
                Province: vals?.FiltriCentriAziendali?.Province ?? new Array(),
                Comuni: vals?.FiltriCentriAziendali?.Comuni ?? new Array(),
            },

            FiltriPianoColturale: {
                FiltroDestinazioneUso: vals?.FiltriPianoColturale?.FiltroDestinazioneUso ?? (vals?.CategoriaEsito?.idCat == this.ListeCategorieEsito.find(item => item.idCat == Enum_TipoMostra_FiltroRicerca.PianoColturale)?.idCat ||
                                                                                            vals?.CategoriaEsito?.idCat == this.ListeCategorieEsito.find(item => item.idCat == Enum_TipoMostra_FiltroRicerca.PianoColturaleBudget)?.idCat ||
                                                                                            vals?.CategoriaEsito?.idCat == this.ListeCategorieEsito.find(item => item.idCat == Enum_TipoMostra_FiltroRicerca.Esercizi)?.idCat ||
                                                                                            vals?.CategoriaEsito?.idCat == this.ListeCategorieEsito.find(item => item.idCat == Enum_TipoMostra_FiltroRicerca.Appezzamenti)?.idCat ||
                                                                                            vals?.CategoriaEsito?.idCat == this.ListeCategorieEsito.find(item => item.idCat == Enum_TipoMostra_FiltroRicerca.Impianti)?.idCat) ? this.ListaFiltroDestinazioniDuso.find(item => item.id == Enum_FiltroDestinazioneUso_FiltroRicerca.EscludiDestinazioniUso) : this.ListaFiltroDestinazioniDuso.find(item => item.id == Enum_FiltroDestinazioneUso_FiltroRicerca.Tutto),
                DestinazioniUso: vals?.FiltriPianoColturale?.DestinazioniUso ?? new Array(),
                GruppoVegetale: vals?.FiltriPianoColturale?.GruppoVegetale ?? new Array(),
                Specie: vals?.FiltriPianoColturale?.Specie ?? new Array(),
                Varieta: vals?.FiltriPianoColturale?.Varieta ?? new Array(),
                TipologiaVarietale: vals?.FiltriPianoColturale?.TipologiaVarietale ?? new Array(),
                UtilizzoTerreno: vals?.FiltriPianoColturale?.UtilizzoTerreno ?? new Array(),
                ContributiACA: vals?.FiltriPianoColturale?.ContributiACA ?? new Array(),
                Lotto: vals?.FiltriPianoColturale?.Lotto ?? '',
                Progetto: vals?.FiltriPianoColturale?.Progetto ?? ''
            },

            FiltriTemporali: {
                FiltriData: vals?.FiltriTemporali?.FiltriData ?? [{ Entita: null, ColonnaData: null, TipoConfronto: null, ModalitaFiltroData: false, Date: { inizio: null, fine: null } }],
                OperatoreLogico: vals?.FiltriTemporali?.OperatoreLogico ?? this.ListaFiltroOperatoreLogicoTemporali.find(item => item.Value == Enum_FiltroOperatoreLogico_FiltroRicerca.AND_TutteLeCondizioniTrue)
            },

            FiltriMovimenti: {
                FiltroOperazioni: vals?.FiltriMovimenti?.FiltroOperazioni ?? this.ListaFiltroOperazioniSelezionate.find(item => item.Value == Enum_FiltroOperazioniSelezionate_FiltroRicerca.Tutto),
                GruppoOperazioni: vals?.FiltriMovimenti?.GruppoOperazioni ?? new Array(),
                Operazioni: vals?.FiltriMovimenti?.Operazioni ?? new Array(),
                DataMovimento: {
                    inizio: vals?.FiltriMovimenti?.DataMovimento?.inizio ?? null,
                    fine: vals?.FiltriMovimenti?.DataMovimento?.fine ?? null,
                }
            },

            FiltriServizi: {
                Servizio: vals?.FiltriServizi?.Servizio ?? this.defaultServizio,
                StatiPratica: vals?.FiltriServizi?.StatiPratica ?? new Array(),
                Data: vals?.FiltriServizi?.Data ?? null,
            },

            FiltriCatasto: {
                FiltroRipartoCatastale: vals?.FiltriCatasto?.FiltroRipartoCatastale ?? this.ListaRipartoCatastale.find(item => item.Value == Enum_FiltroRipartoCatasto_FiltroRicerca.Tutto)
            },

            FiltriGIS: {
                FiltroPoligoni: vals?.FiltriGIS?.FiltroPoligoni ?? this.ListaPoligoniGIS.find(item => item.Value == Enum_FiltroPoligoni_FiltroRicerca.Tutto),
                Anomalie: vals?.FiltriGIS?.Anomalie ?? new Array(),
            },

            CaricaDati: {
                ImpreseReferenti: vals?.CaricaDati?.ImpreseReferenti ?? false,
                DatiIscrizioneLibroSoci: {
                    NumeroIscrizione: vals?.CaricaDati?.DatiIscrizioneLibroSoci?.NumeroIscrizione ?? false,
                    DataIscrizione: vals?.CaricaDati?.DatiIscrizioneLibroSoci?.DataIscrizione ?? false
                },
                LegaleRappresentante: vals?.CaricaDati?.LegaleRappresentante ?? false,
                IndirizzoAzienda: vals?.CaricaDati?.IndirizzoAzienda ?? true,
                IndirizzoCentroAziendale: vals?.CaricaDati?.IndirizzoCentroAziendale ?? true,
                IndirizziPianoColturale: vals?.CaricaDati?.IndirizziPianoColturale ?? false,
                AltriDatiAzienda: vals?.CaricaDati?.AltriDatiAzienda ?? false,
                AltriDatiCentro: vals?.CaricaDati?.AltriDatiCentro ?? false,
                CatastoCampo: vals?.CaricaDati?.CatastoCampo ?? false,
                AltriDatiCampo: vals?.CaricaDati?.AltriDatiCampo ?? false,
                AltriDatiPianoColturale: vals?.CaricaDati?.AltriDatiPianoColturale ?? false,
                AltriDatiFabbricato: vals?.CaricaDati?.AltriDatiFabbricato ?? false,
                Servizi: vals?.CaricaDati?.Servizi ?? false,
                CodiciAzienda: vals?.CaricaDati?.CodiciAzienda ?? new Array(),
                CodiciCentroAziendale: vals?.CaricaDati?.CodiciCentroAziendale ?? new Array(),
                CodiciCampo: vals?.CaricaDati?.CodiciCampo ?? new Array(),
                CodiciPianoColturale: vals?.CaricaDati?.CodiciPianoColturale ?? new Array(),
                CodiciFabbricato: vals?.CaricaDati?.CodiciFabbricato ?? new Array(),
                CatastoAppezzamento: vals?.CaricaDati?.CatastoAppezzamento ?? false,
                GISImpianto: vals?.CaricaDati?.GISImpianto ?? false,
                ContributiACA: vals?.CaricaDati?.ContributiACA ?? false,
                CatastoCentroAziendale: vals?.CaricaDati?.CatastoCentroAziendale ?? false,
            },

            CategoriaEsito: vals?.CategoriaEsito ?? this.ListeCategorieEsito.find(item => item.idCat == Enum_TipoMostra_FiltroRicerca.PianoColturale),
            Budget: vals?.Budget ?? null

        };
    }

}

function CreateFormGroup(fb: FormBuilder, mask: FormFiltroRicercaConfig) {

    return fb.group({

        FiltriAziende: fb.group({
            ImpreseReferenti: new FormControl(mask.FiltriAziende.ImpreseReferenti),
            TipiImpresa: new FormControl(mask.FiltriAziende.TipiImpresa),
            RagioneSociale: new FormControl(mask.FiltriAziende.RagioneSociale),
            CUAA: new FormControl(mask.FiltriAziende.CUAA),
            Piva: new FormControl(mask.FiltriAziende.Piva),
            Stati: new FormControl(mask.FiltriAziende.Stati),
            Regioni: new FormControl(mask.FiltriAziende.Regioni),
            Province: new FormControl(mask.FiltriAziende.Province),
            Comuni: new FormControl(mask.FiltriAziende.Comuni),
            Zone: new FormControl(mask.FiltriAziende.Zone),
            FiltroZone: new FormControl(mask.FiltriAziende.FiltroZone)
        }),

        FiltriCentriAziendali: fb.group({
            CentroAziendale: new FormControl(mask.FiltriCentriAziendali.CentroAziendale),
            Stati: new FormControl(mask.FiltriCentriAziendali.Stati),
            Regioni: new FormControl(mask.FiltriCentriAziendali.Regioni),
            Province: new FormControl(mask.FiltriCentriAziendali.Province),
            Comuni: new FormControl(mask.FiltriCentriAziendali.Comuni),
        }),

        FiltriPianoColturale: fb.group({
            FiltroDestinazioneUso: new FormControl(mask.FiltriPianoColturale.FiltroDestinazioneUso),
            DestinazioniUso: new FormControl(mask.FiltriPianoColturale.DestinazioniUso),
            GruppoVegetale: new FormControl(mask.FiltriPianoColturale.GruppoVegetale),
            Specie: new FormControl(mask.FiltriPianoColturale.Specie),
            Varieta: new FormControl(mask.FiltriPianoColturale.Varieta),
            TipologiaVarietale: new FormControl(mask.FiltriPianoColturale.TipologiaVarietale),
            ContributiACA: new FormControl(mask.FiltriPianoColturale.ContributiACA),
            UtilizzoTerreno: new FormControl(mask.FiltriPianoColturale.UtilizzoTerreno),
            Lotto: new FormControl(mask.FiltriPianoColturale.Lotto),
            Progetto: new FormControl(mask.FiltriPianoColturale.Progetto),
        }),

        FiltriTemporali: fb.group({
            FiltriData: new FormArray([fb.group(
                {
                    Entita: [null],
                    ColonnaData: [null],
                    TipoConfronto: [null],
                    ModalitaFiltroData: [false],
                    Date: fb.group({
                        inizio: [null],
                        fine: [null]
                    })
                }
            )]),
            OperatoreLogico: new FormControl(mask.FiltriTemporali.OperatoreLogico)
        }),

        FiltriMovimenti: fb.group({
            FiltroOperazioni: new FormControl(mask.FiltriMovimenti.FiltroOperazioni),
            GruppoOperazioni: new FormControl({ value: mask.FiltriMovimenti.GruppoOperazioni, disabled: true}),
            Operazioni: new FormControl({ value: mask.FiltriMovimenti.Operazioni, disabled: true}),
            DataMovimento: fb.group({
                inizio: new FormControl({ value: mask.FiltriMovimenti.DataMovimento.inizio, disabled: true}),
                fine: new FormControl({ value: mask.FiltriMovimenti.DataMovimento.fine, disabled: true})
            }),
        }),

        FiltriServizi: fb.group({
            Servizio: new FormControl(mask.FiltriServizi.Servizio),
            StatiPratica: new FormControl(mask.FiltriServizi.StatiPratica),
            Data: new FormControl(mask.FiltriServizi.Data),
        }),

        FiltriCatasto: fb.group({
            FiltroRipartoCatastale: new FormControl(mask.FiltriCatasto.FiltroRipartoCatastale)
        }),

        FiltriGIS: fb.group({
            FiltroPoligoni: new FormControl(mask.FiltriGIS.FiltroPoligoni),
            Anomalie: new FormControl(mask.FiltriGIS.Anomalie),
        }),

        CaricaDati: fb.group({
            ImpreseReferenti: new FormControl(mask.CaricaDati.ImpreseReferenti),
            DatiIscrizioneLibroSoci: fb.group({
                NumeroIscrizione: new FormControl(mask.CaricaDati.DatiIscrizioneLibroSoci.NumeroIscrizione),
                DataIscrizione: new FormControl(mask.CaricaDati.DatiIscrizioneLibroSoci.DataIscrizione)
            }),
            LegaleRappresentante: new FormControl(mask.CaricaDati.LegaleRappresentante),
            IndirizzoAzienda: new FormControl(mask.CaricaDati.IndirizzoAzienda),
            IndirizzoCentroAziendale: new FormControl(mask.CaricaDati.IndirizzoCentroAziendale),
            IndirizziPianoColturale: new FormControl(mask.CaricaDati.IndirizziPianoColturale),
            Servizi: new FormControl(mask.CaricaDati.Servizi),
            AltriDatiAzienda: new FormControl(mask.CaricaDati.AltriDatiAzienda),
            AltriDatiCentro: new FormControl(mask.CaricaDati.AltriDatiCentro),
            CatastoCampo: new FormControl(mask.CaricaDati.CatastoCampo),
            AltriDatiCampo: new FormControl(mask.CaricaDati.AltriDatiCampo),
            AltriDatiPianoColturale: new FormControl(mask.CaricaDati.AltriDatiPianoColturale),
            AltriDatiFabbricato: new FormControl(mask.CaricaDati.AltriDatiFabbricato),
            CodiciAzienda: new FormControl(mask.CaricaDati.CodiciAzienda),
            CodiciCentroAziendale: new FormControl(mask.CaricaDati.CodiciCentroAziendale),
            CodiciCampo: new FormControl(mask.CaricaDati.CodiciPianoColturale),
            CodiciPianoColturale: new FormControl(mask.CaricaDati.CodiciPianoColturale),
            CodiciFabbricato: new FormControl(mask.CaricaDati.CodiciPianoColturale),
            CatastoAppezzamento: new FormControl(mask.CaricaDati.CatastoAppezzamento),
            GISImpianto: new FormControl(mask.CaricaDati.GISImpianto),
            ContributiACA: new FormControl(mask.CaricaDati.ContributiACA),
            CatastoCentroAziendale: new FormControl(mask.CaricaDati.CatastoCentroAziendale)
        }),

        CategoriaEsito: new FormControl(null),
        Budget: new FormControl(mask.Budget)

    });
}
