import {AfterViewInit, Component, HostListener, OnDestroy, OnInit, QueryList, ViewChild, ViewChildren} from "@angular/core";
import { GiasDropDownTemplateService, ObjParametriAgenda } from 'gias-ui-kit';
import { Enum_Entita_FiltroRicerca, Enum_FiltroDestinazioneUso_FiltroRicerca, Enum_FiltroOperazioniSelezionate_FiltroRicerca, Enum_FiltroPoligoni_FiltroRicerca, Enum_TipoComportamento_FiltroRicerca, Enum_TipoMostra_FiltroRicerca, enum_CodificaStampe } from "app/Model/TipiEnumerativi";
import { FiltroRicercaService } from "./griglia-filtro-ricerca/service/filtro-ricerca.service";
import { GrigliaFiltroRicercaComponent } from "./griglia-filtro-ricerca/griglia-filtro-ricerca.component";
import { GiasMultiSelectTemplateService } from 'gias-ui-kit';
import { TranslocoService } from "@jsverse/transloco";
import { combineLatestWith, lastValueFrom, Observable, skip, Subscription} from "rxjs";
import {
  AnagrafeClient,
  RispostaStandard
} from "app/Service/net-core6-api.service";
import { faEraser, faFilter, faMagnifyingGlass, faSliders, faUser, faXmark } from "@fortawesome/free-solid-svg-icons";
import {cloneDeep} from "lodash";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { DrawerComponent, TabStripComponent, TabStripTabComponent } from "@progress/kendo-angular-layout";
import { FiltriTemporaliComponent } from "./componenti/filtri-temporali/filtri-temporali.component";
import { TreeAziendeService } from "./service/tree-aziende.service";
import { ActivatedRoute } from "@angular/router";
import { Enum_Type_Button_Clear } from "./utils";
import { ConversionService } from "app/Service/conversion.service";
import { FiltroJSONService } from 'gias-kendo-grid';
import { CodiciComponent } from "./componenti/codici/codici.component";
import {GiasMessageService} from 'app/Service/gias-message.service';
import { AGRODATAFINE, AGRODATAINIZIO } from "app/Model/CostantiPersonalizzate";
import { PivaRealeService } from 'app/anagrafica/imprese/piva-reale.service'; 

@Component({
    standalone: false,
    selector: 'app-filtro-ricerca',
    templateUrl: './filtro-ricerca.component.html',
    styleUrls: ['./filtro-ricerca.component.scss'],
    providers: [GiasDropDownTemplateService,
                GiasMultiSelectTemplateService
                ]
})
export class FiltroRicercaComponent implements OnDestroy, OnInit, AfterViewInit  {

    @ViewChild('grigliaFiltro') grigliaFiltro: GrigliaFiltroRicercaComponent;
    @ViewChild('tabstrip') tabstrip: TabStripComponent;
    @ViewChild('tabstripDati') tabstripDati: TabStripComponent;
    @ViewChild('filtriTemporali') filtriTemporali: FiltriTemporaliComponent;
    @ViewChildren(TabStripTabComponent) public tabs: QueryList<TabStripTabComponent>;
    @ViewChild('drawer') drawer: DrawerComponent;
    @ViewChildren(CodiciComponent) codiciComponent: QueryList<CodiciComponent>;

    displayButtonFilters: boolean = true;
    displayFilters: boolean = false;
    drawerWidth: number;

    shouldApply: boolean = false;

    // displayButtonDati: boolean = true;
    displayDati: boolean = false;
    displayPrintExportButton: boolean = false;
    displayDDLTipoFiltroOperazioni: boolean = true;

    showGrid: boolean = true;
    showSpecieAndVarieta: boolean = true;
    showAnomalieGIS: boolean = true;
    showMovimenti: boolean = true;
    modBudget: boolean = false;
    showDestinazioniUso: boolean = true;
    withPadding: boolean = true;
    withMarginTop: boolean = true;
    disableButtonAnnataAgraria: boolean = true;

    showImpostaAzienda: boolean;

    faSearch = faMagnifyingGlass;
    faClear = faEraser;
    faUser = faUser;
    faFilter = faFilter;
    faSliders = faSliders;
    faClose = faXmark;

    ListaCodiciAziende = [];
    ListaCodiciCentroAziendale = [];
    ListaCodiciCampo = [];
    ListaCodiciPianoColturale = [];
    ListaCodiciFabbricato = [];

    modalitaFiltroneConBottone = [Enum_TipoComportamento_FiltroRicerca.SelezionamentoEntita, Enum_TipoComportamento_FiltroRicerca.EsportaPdf, Enum_TipoComportamento_FiltroRicerca.EsportaExcel];

    buttonTypeChiamanti: Enum_TipoComportamento_FiltroRicerca;
    codificaStampa: enum_CodificaStampe;

    paramAgenda: ObjParametriAgenda;
    paramFiltroRicercaNG: any;

    inputAziendaSelected: any;

    DDLObs = new Subscription();

    public Enum_Type_Button_Clear = Enum_Type_Button_Clear;

    constructor(
                public formService: FiltroRicercaService,
                private transloco: TranslocoService,
                private ddlService: GiasDropDownTemplateService,
                private codiciService: AnagrafeClient,
                private objParametriAgendaService: ObjParametriAgendaService,
                private filtroJSONService: FiltroJSONService,
                private route: ActivatedRoute,
                public treeAziendeService: TreeAziendeService,
                private giasMessageService: GiasMessageService,
                private conversionService: ConversionService,
                private pivaRealeService: PivaRealeService) {

                    this.paramAgenda = this.objParametriAgendaService.getObjParamValue();

                    this.formService.onLoad = true;
                    this.filtroJSONService.isFirstApplyView = true;

                    //resetto il form, potrei arrivare una seconda volta sul Filtrone
                    this.formService.formFilters.patchValue(this.formService.InitializeFormFiltroRicerca(), { emitEvent: false });
                    this.formService.resetFormArrayFiltriTemporali();
                    this.expandPanelTree(false);

                    //carico i codici
                    this.loadCodici();

                    this.manageFromMenuOrChiamante();

                    this.formService.gridIDstring.next(this.formService.formFilters.get("CategoriaEsito").value.idCat);

                    this.formService.loadListaBudgets();

                    this.DDLObs.add(this.ddlService.currentDropDownValueObject.pipe(skip(1)).subscribe(async ddlElem=>{
                        switch(ddlElem.FormControlName){
                            case 'CategoriaEsito':

                                //devo sbiancare il form
                                this.formService.formFilters.patchValue(this.formService.InitializeFormFiltroRicerca({ CategoriaEsito: ddlElem.Value }), { emitEvent: false });
                                this.formService.resetFormArrayFiltriTemporali();
                                if (this.filtriTemporali)
                                  this.filtriTemporali.initializeFiltriTemporali();

                                this.showDestinazioniUso = true;
                                //se arrivo da chiamante e ho i filtri valorizzati, rimetto quelli
                                if (this.paramFiltroRicercaNG)
                                  this.formService.setFormFieldsForChiamante(this.paramFiltroRicercaNG);
                                else  //se arrivo da menu, metto i filtri temporali che avevo
                                  if (ddlElem.Value.idCat == Enum_TipoMostra_FiltroRicerca.PianoColturale || ddlElem.Value.idCat == Enum_TipoMostra_FiltroRicerca.PianoColturaleBudget) {
                                      this.showDestinazioniUso = false;
                                      this.formService.setFiltriTemporaliMenu();
                                  } else
                                      this.formService.showSetDefaultFiltersButton = true;

                                this.showMovimenti = true;
                                this.modBudget = false;
                                this.displayDDLTipoFiltroOperazioni = true;

                                //sostituisco il blocco commentato sotto con 
                                this.manageTipoMostra(ddlElem.Value.idCat);

                                this.formService.onLoad = true;

                                this.filtroJSONService.isFirstApplyView = true;
                                this.formService.gridIDstring.next(ddlElem.Value.idCat);
                                this.formService.set_tab_Carica_Dati_Visibility();

                            break;

                            case 'FiltroDestinazioneUso':
                                switch (ddlElem.Value.id) {

                                  case Enum_FiltroDestinazioneUso_FiltroRicerca.Tutto:
                                    this.showSpecieAndVarieta = true;
                                    this.showDestinazioniUso = true;
                                  break;

                                  case Enum_FiltroDestinazioneUso_FiltroRicerca.EscludiDestinazioniUso:
                                    this.showSpecieAndVarieta = true;
                                    this.showDestinazioniUso = false;
                                    this.formService.formFilters.get('FiltriPianoColturale').get('DestinazioniUso').patchValue([], { emitEvent: false });
                                  break;

                                  case Enum_FiltroDestinazioneUso_FiltroRicerca.SoloDestinazioniUso:
                                    this.showSpecieAndVarieta = false;
                                    this.showDestinazioniUso = true;
                                    this.formService.formFilters.get('FiltriPianoColturale').get('GruppoVegetale').patchValue([], { emitEvent: false });
                                    this.formService.formFilters.get('FiltriPianoColturale').get('Specie').patchValue([], { emitEvent: false });
                                    this.formService.formFilters.get('FiltriPianoColturale').get('TipologiaVarietale').patchValue([], { emitEvent: false });
                                    this.formService.formFilters.get('FiltriPianoColturale').get('Varieta').patchValue([], { emitEvent: false });
                                  break;

                                }
                            break;

                            case 'FiltroPoligoni':
                                if (ddlElem.Value.Value == Enum_FiltroPoligoni_FiltroRicerca.SenzaPoligoni)
                                  this.showAnomalieGIS = false;
                                else
                                  this.showAnomalieGIS = true;
                            break;

                            case 'Servizio':
                                this.formService.formFilters.get('FiltriServizi').get('StatiPratica').patchValue([]);
                                if (ddlElem.Value !== this.formService.defaultServizio)
                                  this.formService.fillListaStatiPratica(ddlElem.Value.Servizio_Cod);
                                else
                                  this.formService.ListaStatiPratica = [];
                            break;

                            case 'Budget':
                                this.loadCodici(ddlElem.Value.Id_Budget);
                                this.loadCodiciPianoColturale(ddlElem.Value.Id_Budget);
                            break;

                            case 'FiltroZone':
                                console.log(ddlElem.Value);
                            break;

                            case 'FiltroOperazioni':
                                this.toggleFieldsFiltriMovimenti(ddlElem.Value.Value);
                            break;
                        }
                    }));

                    this.DDLObs.add(this.formService.formFilters.get('CaricaDati').get('DatiIscrizioneLibroSoci').valueChanges.subscribe(val => {
                      if (val.DataIscrizione || val.NumeroIscrizione)
                        this.formService.formFilters.get('CaricaDati').get('ImpreseReferenti').patchValue(true);
                    }));

                    this.DDLObs.add(this.formService.formFilters.get('CaricaDati').get('ImpreseReferenti').valueChanges.subscribe(val => {
                      if (!val)
                        this.formService.formFilters.get('CaricaDati').get('DatiIscrizioneLibroSoci').patchValue({ NumeroIscrizione: false, DataIscrizione: false });  
                    }));
    }

    private manageTipoMostra(idCat: Enum_TipoMostra_FiltroRicerca) {
      
      switch (idCat) {
        case Enum_TipoMostra_FiltroRicerca.PianoColturaleBudget:
          this.modBudget = true;
          let arrInUso = this.formService.ListaBudgets.filter(item => item.In_Uso);
          if (arrInUso.length > 0) {
            let firstBudget = arrInUso.pop();
            this.formService.formFilters.get('Budget').patchValue(firstBudget);
            this.loadCodici(firstBudget.Id_Budget);
            this.loadCodiciPianoColturale(firstBudget.Id_Budget);
          }

          this.toggleFieldsFiltriMovimenti(Enum_FiltroOperazioniSelezionate_FiltroRicerca.Tutto);
          this.selectPreviousAvailableTab();

        break;

        case Enum_TipoMostra_FiltroRicerca.Movimenti:
          this.displayDDLTipoFiltroOperazioni = false;
          
          this.toggleFieldsFiltriMovimenti(Enum_FiltroOperazioniSelezionate_FiltroRicerca.ConOperazioni);
        break;

        case Enum_TipoMostra_FiltroRicerca.Fabbricati:
          this.toggleFieldsFiltriMovimenti(Enum_FiltroOperazioniSelezionate_FiltroRicerca.Tutto);
          this.showMovimenti= false;
          this.selectPreviousAvailableTab();
        break;

        case Enum_TipoMostra_FiltroRicerca.Appezzamenti:
        case Enum_TipoMostra_FiltroRicerca.Impianti:
        case Enum_TipoMostra_FiltroRicerca.Esercizi:
          this.toggleFieldsFiltriMovimenti(Enum_FiltroOperazioniSelezionate_FiltroRicerca.Tutto);
          this.showDestinazioniUso = false;
          this.loadCodiciPianoColturale();
        break;

        default:
          this.toggleFieldsFiltriMovimenti(Enum_FiltroOperazioniSelezionate_FiltroRicerca.Tutto);
        break;
      }
    }

    private toggleFieldsFiltriMovimenti(param) {
      if (param == Enum_FiltroOperazioniSelezionate_FiltroRicerca.Tutto) {
          this.formService.formFilters.get('FiltriMovimenti').patchValue(this.formService.InitializeFormFiltroRicerca()['FiltriMovimenti']);
          this.formService.formFilters.get('FiltriMovimenti').get('GruppoOperazioni').disable();
          this.formService.formFilters.get('FiltriMovimenti').get('Operazioni').disable();
          this.formService.formFilters.get('FiltriMovimenti').get('DataMovimento').get('inizio').disable();
          this.formService.formFilters.get('FiltriMovimenti').get('DataMovimento').get('fine').disable();
          this.disableButtonAnnataAgraria = true;
      } else {
          this.formService.formFilters.get('FiltriMovimenti').get('GruppoOperazioni').enable();
          this.formService.formFilters.get('FiltriMovimenti').get('Operazioni').enable();
          this.formService.formFilters.get('FiltriMovimenti').get('DataMovimento').get('inizio').enable();
          this.formService.formFilters.get('FiltriMovimenti').get('DataMovimento').get('fine').enable();
          this.disableButtonAnnataAgraria = false;
      }
    }

    selectPreviousAvailableTab() {
      let disabledIndex = this.tabs.toArray().findIndex(item => item.selected && !item.disabled);

      if (disabledIndex > -1) {
        const tabArray = this.tabs.toArray();
        for (let i = 0; i < tabArray.length && i !== disabledIndex; i++) {
          if (!tabArray[i].disabled) {
            this.tabstrip.selectTab(i);
            break;
          }
        }
      }
    }

    loadCodiciPianoColturale(budget: number = 0) {

      this.codiciService.anagrafeGetCodiciAnagrafeUsatixEntita({ entita: Enum_Entita_FiltroRicerca.Appezzamento, idBudget: budget}).pipe(
        combineLatestWith(
        this.codiciService.anagrafeGetCodiciAnagrafeUsatixEntita({ entita: Enum_Entita_FiltroRicerca.Impianto , idBudget: budget}),
        this.codiciService.anagrafeGetCodiciAnagrafeUsatixEntita({ entita: Enum_Entita_FiltroRicerca.Esercizio , idBudget: budget}))).subscribe((r: Array<RispostaStandard>)=>{

        let CodiciAppezzamento: Array<any> = JSON.parse(r[0].RispostaStringa).map(item => {
          return { ...item, checked: false, tipoCodice: Enum_Entita_FiltroRicerca.Appezzamento, master: false}
        });

        if(CodiciAppezzamento && CodiciAppezzamento.length > 0){
          let CodiceAppezzamentoMaster = cloneDeep(CodiciAppezzamento[0]);

          CodiceAppezzamentoMaster.master = true;

          CodiceAppezzamentoMaster.codice = 0;

          CodiceAppezzamentoMaster.descrizione = this.transloco.translate("Appezzamento");

          CodiciAppezzamento.unshift(CodiceAppezzamentoMaster);
        }

        let CodiciImpianto: Array<any>  = JSON.parse(r[1].RispostaStringa).map(item => {
          return { ...item, checked: false, tipoCodice: Enum_Entita_FiltroRicerca.Impianto, master: false}
        });

        if(CodiciImpianto && CodiciImpianto.length > 0){
          let CodiceImpiantoMaster = cloneDeep(CodiciImpianto[0]);

          CodiceImpiantoMaster.master = true;

          CodiceImpiantoMaster.codice = 0;

          CodiceImpiantoMaster.descrizione = this.transloco.translate("Impianto");

          CodiciImpianto.unshift(CodiceImpiantoMaster);
        }

        let CodiciEsercizio: Array<any> = JSON.parse(r[2].RispostaStringa).map(item => {
          return { ...item, checked: false, tipoCodice: Enum_Entita_FiltroRicerca.Esercizio, master: false}
        });

        if(CodiciEsercizio && CodiciEsercizio.length > 0) {
          let CodiciEsercizioMaster = cloneDeep(CodiciEsercizio[0]);

          CodiciEsercizioMaster.master = true;

          CodiciEsercizioMaster.codice = 0;

          CodiciEsercizioMaster.descrizione = this.transloco.translate("Esercizio");

          CodiciEsercizio.unshift(CodiciEsercizioMaster);
        }

        if (this.formService.formFilters.get("CategoriaEsito").value.idCat == Enum_Entita_FiltroRicerca.Appezzamento)
          this.ListaCodiciPianoColturale = CodiciAppezzamento;
        else
          if (this.formService.formFilters.get("CategoriaEsito").value.idCat == Enum_Entita_FiltroRicerca.Impianto)
            this.ListaCodiciPianoColturale = CodiciAppezzamento.concat(CodiciImpianto);
          else
            this.ListaCodiciPianoColturale = CodiciAppezzamento.concat(CodiciImpianto,CodiciEsercizio);
      });
    }

    manageFromMenuOrChiamante() {

        if (this.paramAgenda.GenericObj_string == '') { //arrivo da menù
          this.formService.displayNoViews = false;
          this.showImpostaAzienda = false;

          //carico l'albero delle aziende
          this.treeAziendeService.getTreeAziende();

          //rimuovo se sono da Menù Esercizi, Appezzamenti, Impianti
          this.formService.ListeCategorieEsito.forEach(item => {
            item.visible = true;
            if (item.idCat == Enum_TipoMostra_FiltroRicerca.Appezzamenti || item.idCat == Enum_TipoMostra_FiltroRicerca.Esercizi || item.idCat == Enum_TipoMostra_FiltroRicerca.Impianti)
              item.visible = false;
          });

          this.formService.formFilters.patchValue(this.formService.InitializeFormFiltroRicerca({ CategoriaEsito: this.formService.ListeCategorieEsito.find(item => item.idCat == Enum_TipoMostra_FiltroRicerca.PianoColturale)}), { emitEvent: false });
          this.formService.formFilters.get("CategoriaEsito").enable();
          this.showDestinazioniUso = false;

          this.formService.setFiltriTemporaliMenu();

        } else {  //arrivo da qualche chiamante

          this.showImpostaAzienda = true;
          this.paramFiltroRicercaNG = this.conversionService.ConversionDateInObject(JSON.parse(this.paramAgenda.GenericObj_string));

          if (this.paramFiltroRicercaNG.TipoComportamentoFiltroRicercaNG !== Enum_TipoComportamento_FiltroRicerca.RicercaAvanzataAzienda) {
            this.formService.displayNoViews = false;
            this.formService.applyLoadCacheViewFirstTime = false;
            //carico l'albero delle aziende
            this.treeAziendeService.getTreeAziende();
          } else {
            this.formService.displayNoViews = true;
            this.expandPanelTree(true);
          }

          this.formService.ListeCategorieEsito.forEach(item => {
              if (this.paramFiltroRicercaNG.TipoMostraGestitiChiamante.indexOf(item.idCat) > -1)
                item.visible = true;
              else
                item.visible = false;
          });

          this.formService.setFormFieldsForChiamante(this.paramFiltroRicercaNG);

          this.formService.formFilters.get("CategoriaEsito").patchValue(this.formService.ListeCategorieEsito.find(item => item.visible));

          //se me ne viene passato solo uno, blocco anche la DDL
          if (this.formService.ListeCategorieEsito.filter(item => item.visible).length == 1)
            this.formService.formFilters.get("CategoriaEsito").disable();

          this.manageTipoMostra(this.formService.formFilters.get("CategoriaEsito").value.idCat);

          if ( this.formService.formFilters.get("CategoriaEsito").value.idCat == Enum_TipoMostra_FiltroRicerca.PianoColturale 
                || this.formService.formFilters.get("CategoriaEsito").value.idCat == Enum_TipoMostra_FiltroRicerca.PianoColturaleBudget
                || this.formService.formFilters.get("CategoriaEsito").value.idCat == Enum_TipoMostra_FiltroRicerca.Appezzamenti 
                || this.formService.formFilters.get("CategoriaEsito").value.idCat == Enum_TipoMostra_FiltroRicerca.Esercizi 
                || this.formService.formFilters.get("CategoriaEsito").value.idCat == Enum_TipoMostra_FiltroRicerca.Impianti) {
  
                this.formService.formFilters.get("FiltriPianoColturale").get("FiltroDestinazioneUso").patchValue(this.formService.ListaFiltroDestinazioniDuso.find(item => item.id == Enum_FiltroDestinazioneUso_FiltroRicerca.EscludiDestinazioniUso));
                this.showDestinazioniUso = false;
          }

          //occorre distinguere in questo caso da quale chiamante sto arrivando, perché, ad esempio, se arrivo dalle stampe devo far vedere il bottone
          if (this.modalitaFiltroneConBottone.includes(this.paramFiltroRicercaNG.TipoComportamentoFiltroRicercaNG)) {
            this.displayPrintExportButton = true;
            this.showImpostaAzienda = false;
            this.buttonTypeChiamanti = this.paramFiltroRicercaNG.TipoComportamentoFiltroRicercaNG;
            this.codificaStampa = this.paramFiltroRicercaNG.CodificaStampe;
          }
        }

    }

    loadCodici(budget: number = 0) {
        this.codiciService.anagrafeGetCodiciAnagrafeUsatixEntita({entita: Enum_Entita_FiltroRicerca.Azienda, idBudget: budget}).subscribe(r => {
            this.ListaCodiciAziende = JSON.parse(r.RispostaStringa);
            this.ListaCodiciAziende = this.ListaCodiciAziende.map(item => {
                return { ...item, checked: false, tipoCodice: Enum_Entita_FiltroRicerca.Azienda, master: false}
            });
        });

        this.codiciService.anagrafeGetCodiciAnagrafeUsatixEntita({entita: Enum_Entita_FiltroRicerca.CentroAziendale, idBudget: budget}).subscribe(r => {
            this.ListaCodiciCentroAziendale = JSON.parse(r.RispostaStringa);
            this.ListaCodiciCentroAziendale = this.ListaCodiciCentroAziendale.map(item => {
                return { ...item, checked: false, tipoCodice: Enum_Entita_FiltroRicerca.CentroAziendale, master: false}
            });
        });

        this.codiciService.anagrafeGetCodiciAnagrafeUsatixEntita({entita: Enum_Entita_FiltroRicerca.Campo, idBudget: budget}).subscribe(r => {
          this.ListaCodiciCampo = JSON.parse(r.RispostaStringa);
          this.ListaCodiciCampo = this.ListaCodiciCampo.map(item => {
            return { ...item, checked: false, tipoCodice: Enum_Entita_FiltroRicerca.Campo, master: false}
          });
        });

        this.codiciService.anagrafeGetCodiciAnagrafeUsatixEntita({entita: Enum_Entita_FiltroRicerca.Fabbricato, idBudget: budget}).subscribe(r => {
          this.ListaCodiciFabbricato = JSON.parse(r.RispostaStringa);
          this.ListaCodiciFabbricato = this.ListaCodiciFabbricato.map(item => {
            return { ...item, checked: false, tipoCodice: Enum_Entita_FiltroRicerca.Fabbricato, master: false}
          });
        });
    }

    getNameForDataTemplate(name: string, valueOfRadio): string {
        return this.transloco.translate(name) + " " + this.transloco.translate((valueOfRadio == "0")? "PrecedenteAl" : "SuccessivaAl");
    }

    ngAfterViewInit() {
        this.grigliaFiltro.filtroColture.next(this.formService.InitializeFormFiltroRicerca()['FiltriPianoColturale'].FiltroDestinazioneUso = this.formService.ListaFiltroDestinazioniDuso.find(item => item.id == Enum_FiltroDestinazioneUso_FiltroRicerca.Tutto));
        if (!this.paramFiltroRicercaNG || (this.paramFiltroRicercaNG.TipoComportamentoFiltroRicercaNG !== Enum_TipoComportamento_FiltroRicerca.RicercaAvanzataAzienda))
          this.onOpenFilters();
    }

    async setPivaRagSocOnFilter(dataItem: any) {
      let pivaReale = await lastValueFrom(this.pivaRealeService.getPivaReale(dataItem.Piva));
      this.formService.formFilters.get('FiltriAziende').get('Piva').patchValue(pivaReale);
      this.formService.formFilters.get('FiltriAziende').get('RagioneSociale').patchValue(dataItem.Text);
      this.expandPanelTree(false);
      this.onOpenFilters();
    }

    hasChanges(formGroupToCheck) {
      return this.formService.hasChangedValues(this.formService.formFilters.get(formGroupToCheck).getRawValue(), this.formService.InitializeFormFiltroRicerca({ CategoriaEsito: this.formService.formFilters.get("CategoriaEsito").value })[formGroupToCheck]);
    }

    private async waitDialogResult() {

      let msg = this.transloco.translate("ConfermaRiletturaGriglia", { });

      let dialogResponse = await this.formService.createDialogWindow(
        '', msg
      );

      if (dialogResponse['returnObj']) {
        this.formService.onLoad = false;
        return true;
      }

      this.formService.onLoad = true;
      return true;

    }

    reloadGrid() {

        if (this.filtriTemporali)
          this.filtriTemporali.initializeFiltriTemporali();

        this.showGrid = false;
        setTimeout(() => this.showGrid = true, 0);
    }

    onClearPartialFilters(param: string) {
        if (param == 'FiltriPianoColturale') {
          this.showSpecieAndVarieta = true;
          // this.onSwitchBudgetChange(false);
        }

        this.formService.formFilters.get(param).patchValue(this.formService.InitializeFormFiltroRicerca({ CategoriaEsito: this.formService.formFilters.get("CategoriaEsito").value })[param], { emitEvent: false });

        if (param == 'CaricaDati')
          this.formService.set_tab_Carica_Dati_Visibility();

        if (param == 'FiltriTemporali') {
          this.formService.clearFormArray();
          if (this.filtriTemporali)
            this.filtriTemporali.initializeFiltriTemporali();
        }
    }

    onClearFilters() {  //devo pulire tutti i filtri, tranne la sezione dei Dati Aggiuntivi
        this.onClearPartialFilters('FiltriAziende');
        this.onClearPartialFilters('FiltriCentriAziendali');
        this.onClearPartialFilters('FiltriPianoColturale');
        this.onClearPartialFilters('FiltriTemporali');
        this.onClearPartialFilters('FiltriMovimenti');
        this.onClearPartialFilters('FiltriServizi');
        this.onClearPartialFilters('FiltriCatasto');
        this.onClearPartialFilters('FiltriGIS');
    }

    onClearDatiAggiuntivi(groupDatiAggiuntivi: number) {
      let partialDefaultValues = this.getPartialDefaultValuesCaricaDati(groupDatiAggiuntivi);
      this.formService.formFilters.get("CaricaDati").patchValue(partialDefaultValues);

      //pulisco la lista dei codici
      switch(groupDatiAggiuntivi) {
        case Enum_Entita_FiltroRicerca.Azienda:
            this.codiciComponent.find(item => item.tipoCodici == Enum_Entita_FiltroRicerca.Azienda)?.DeselectAll();
        break;

        case Enum_Entita_FiltroRicerca.CentroAziendale:
          this.codiciComponent.find(item => item.tipoCodici == Enum_Entita_FiltroRicerca.CentroAziendale)?.DeselectAll();
        break;

        case Enum_Entita_FiltroRicerca.Campo:
          this.codiciComponent.find(item => item.tipoCodici == Enum_Entita_FiltroRicerca.Campo)?.DeselectAll();
        break;

        case (Enum_Entita_FiltroRicerca.Appezzamento + Enum_Entita_FiltroRicerca.Impianto + Enum_Entita_FiltroRicerca.Esercizio):
          this.codiciComponent.find(item => item.tipoCodici == Enum_Entita_FiltroRicerca.Appezzamento + Enum_Entita_FiltroRicerca.Impianto + Enum_Entita_FiltroRicerca.Esercizio)?.DeselectAll();
        break;

        case Enum_Entita_FiltroRicerca.Fabbricato:
          this.codiciComponent.find(item => item.tipoCodici == Enum_Entita_FiltroRicerca.Fabbricato)?.DeselectAll();
        break;
      }
    }

    hasChangesDatiAggiuntivi(groupDatiAggiuntivi: number): boolean {
      let partialDefaultValues = this.getPartialDefaultValuesCaricaDati(groupDatiAggiuntivi);
      return JSON.stringify(this.formService.formFilters.get("CaricaDati").getRawValue()) !== JSON.stringify(partialDefaultValues);
    }

    getPartialDefaultValuesCaricaDati(groupDatiAggiuntivi: number) {

      let datAggiuntivi = this.formService.formFilters.get("CaricaDati").getRawValue();

      switch(groupDatiAggiuntivi) {
        case Enum_Entita_FiltroRicerca.Azienda:
          datAggiuntivi.ImpreseReferenti = false;
          datAggiuntivi.LegaleRappresentante = false;
          datAggiuntivi.IndirizzoAzienda = (this.formService.formFilters.get("CategoriaEsito").value.idCat == Enum_TipoMostra_FiltroRicerca.Aziende) ? true : false;
          datAggiuntivi.Servizi = false;
          datAggiuntivi.AltriDatiAzienda = false;
        break;

        case Enum_Entita_FiltroRicerca.CentroAziendale:
          datAggiuntivi.IndirizzoCentroAziendale = (this.formService.formFilters.get("CategoriaEsito").value.idCat == Enum_TipoMostra_FiltroRicerca.CentriAziendali) ? true : false;
          datAggiuntivi.CatastoCentroAziendale = false;
          datAggiuntivi.AltriDatiCentro = false;
        break;

        case Enum_Entita_FiltroRicerca.Campo:
          datAggiuntivi.CatastoCampo = false;
          datAggiuntivi.AltriDatiCampo = false;
        break;

        case (Enum_Entita_FiltroRicerca.Appezzamento + Enum_Entita_FiltroRicerca.Impianto + Enum_Entita_FiltroRicerca.Esercizio):
          datAggiuntivi.IndirizziPianoColturale = false;
          datAggiuntivi.CatastoAppezzamento = false;
          datAggiuntivi.GISImpianto = false;
          datAggiuntivi.AltriDatiPianoColturale = false;
        break;

        case Enum_Entita_FiltroRicerca.Fabbricato:
          datAggiuntivi.AltriDatiFabbricato = false;
        break;
      }

      return datAggiuntivi;
    }

    private checkIfFormIsValid(obj: {errorMessage: string}): boolean {

        const formValues = this.formService.formFilters.getRawValue();
        const categoria = formValues.CategoriaEsito;

        switch (categoria.idCat) {

            case Enum_TipoMostra_FiltroRicerca.Movimenti:
                const validitaInizio = formValues.FiltriMovimenti.DataMovimento?.inizio ?? AGRODATAINIZIO;
                const validitaFine = formValues.FiltriMovimenti.DataMovimento?.fine ?? AGRODATAFINE; 

                let formFiltriMovimenti = this.formService.formFilters.get('FiltriMovimenti').get('DataMovimento');
                
                if (validitaFine && validitaInizio) {
                    const dataFine = new Date(validitaFine);
                    const dataInizio =  new Date(validitaInizio);

                    const diffInMillis = dataFine.getTime() - dataInizio.getTime();
                    const daysDiff = diffInMillis / (1000 * 60 * 60 * 24);

                    if (daysDiff < 0 || validitaInizio == AGRODATAINIZIO || validitaFine == AGRODATAFINE) {
                      
                      if (daysDiff < 0) 
                        obj.errorMessage = 'DateSuiMovimentiIncoerenti';
                      else
                        obj.errorMessage = 'ImpostareIntervalloEstrazioneMovimenti';

                      formFiltriMovimenti.get('inizio').setErrors({ invalidDateRange: true });
                      formFiltriMovimenti.get('fine').setErrors({ invalidDateRange: true });
                      return false;
                    }
                }

                formFiltriMovimenti.get('inizio').setErrors(null);
                formFiltriMovimenti.get('fine').setErrors(null);
                return true;
            break;

            default:
                return true;
            break;

        }

    }

    onSubmit() {

        let objMessageError: { errorMessage: string } = { errorMessage: "" };
        
        if (!this.checkIfFormIsValid(objMessageError)) {
            this.giasMessageService.errorMessage(this.transloco.translate(objMessageError.errorMessage));
            return;
        }

        this.showGrid = true;
        if (this.grigliaFiltro) {
            //this.grigliaFiltro.refreshGriglia();
            this.formService.onLoad = false;
            this.reloadGrid();
        }
        this.onCancel();
    }

    ngOnInit() {
      this.formService.set_tab_Carica_Dati_Visibility();

      if (this.paramFiltroRicercaNG) {
        switch (this.formService.formFilters.get('CategoriaEsito').value.idCat) {
          case Enum_TipoMostra_FiltroRicerca.Aziende:
            this.formService.formFilters.get('CaricaDati').get('ImpreseReferenti').patchValue(true);
          break;
        }

        this.route.queryParams.subscribe(params => {
          if (params['seFrame'] == '1') {
            this.withPadding = false;
            this.withMarginTop = false;
          }
        });
      }

      this.loadCodiciPianoColturale();
    }

    @HostListener('window:beforeunload')
    canDeactivate(): Observable<boolean> | boolean {
      // insert logic to check if there are pending changes here;
      // returning true will navigate without confirmation
      // returning false will show a confirm dialog before navigating away
      if (this.formService.filtroJSONBeforeChanges !== undefined && JSON.stringify(this.formService.formFilters.getRawValue()) !== JSON.stringify(this.formService.filtroJSONBeforeChanges)) {
        return false;
      } else {
        return true;
      }
    }

    onCancel() {
      if (this.displayFilters)
        this.onCancelFilters();
      else
        this.onCancelDati();
    }

    onCancelFilters() {

      this.displayButtonFilters = true;
      this.displayFilters = false;

      this.shouldApply = false;

      if (!!this.drawer.expanded) {
          this.drawer.toggle();
      }
    }

    onOpenFilters() {
      this.displayButtonFilters = false;
      this.displayFilters = true;

      this.drawer.toggle();
    }

    onCancelDati() {
      this.displayButtonFilters = true;
      this.displayDati = false;

      this.shouldApply = false;

      if (!!this.drawer.expanded) {
          this.drawer.toggle();
      }
    }

    onOpenDati() {
      this.displayDati = true;
    }

    onSelectImpresa(dataItem: any) {
      //preparo l'oggetto da passare alla griglia
      let objToSelectAzienda = {Piva: null, RagioneSociale: null};

      objToSelectAzienda.Piva = dataItem.Piva;
      objToSelectAzienda.RagioneSociale = dataItem.Text;

      this.inputAziendaSelected = objToSelectAzienda;
    }

    setImpresaReferente(piva: any) {
      let arrImpreseReferenti = this.formService.formFilters.get('FiltriAziende').get('ImpreseReferenti').value;
      if (arrImpreseReferenti.find(item => item.Piva == piva) == undefined) {
        arrImpreseReferenti.push(this.formService.ListaCoopImpreseReferenti.find(item => item.Piva == piva));
        this.formService.formFilters.get('FiltriAziende').get('ImpreseReferenti').patchValue(arrImpreseReferenti);
        this.expandPanelTree(false);
        this.onOpenFilters();
      }
    }

    ngOnDestroy(): void {
        this.DDLObs.unsubscribe();
    }

    expandPanelTree(param: boolean) {
      this.treeAziendeService.expander.next(param);
    }

    showButtonReimposta() {
      if (this.tabstripDati)
        return this.tabstripDati.tabs.length > 1;
    }

    printReports() {

    }

    onClickSetAnnataAgraria() {
      this.formService.formFilters.get('FiltriMovimenti').get('DataMovimento').get('inizio').patchValue(new Date(this.formService.inizioCampagna), { emitEvent: false });
      this.formService.formFilters.get('FiltriMovimenti').get('DataMovimento').get('fine').patchValue(new Date(this.formService.fineCampagna), { emitEvent: false });
    }

  protected readonly Enum_Entita_FiltroRicerca = Enum_Entita_FiltroRicerca;
}
