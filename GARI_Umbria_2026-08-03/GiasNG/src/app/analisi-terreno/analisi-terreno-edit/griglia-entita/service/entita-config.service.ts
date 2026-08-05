import { Injectable } from "@angular/core";
import { TranslocoService } from "@jsverse/transloco";
import { SMARTPHONE_WIDTH } from "app/Model/CostantiPersonalizzate";
import {  KendoGridColumn, ModelEntry } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { KendoCampiModel } from "app/anagrafica/campi/campi.model";

@Injectable()
export class EntitaConfigService {

    constructor(
        public translocoService: TranslocoService
    ) {}

    kendoColumnsCampi: KendoGridColumn[] = [
        new KendoGridColumn(
          {field: 'Descrizione', title: this.translocoService.translate('Descrizione')},{ resizable:true, filterable:false, editable: false, media: '(max-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
        ),
        new KendoGridColumn(
          {field: 'sa_nome', title: this.translocoService.translate('Centro')},{ resizable:true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Campo', title: this.translocoService.translate('Campo') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Validita_Inizio', title: this.translocoService.translate('Validita_Inizio') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Validita_Fine', title: this.translocoService.translate('Validita_Fine') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Gru_Des', title: this.translocoService.translate('OrientamentoColturale') }, { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135  }
        ),
        new KendoGridColumn(
          { field: 'Veg_Des', title: this.translocoService.translate('Specie') }, { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
        ),
        new KendoGridColumn(
          { field: 'rif_alfanumerico', title: this.translocoService.translate('RifAlfanumerico') }, { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Superficie_Totale', title: this.translocoService.translate('SuperficieTotaleAbbr') }, { resizable: true, editable: false, format: 'n4', numeric: { defaultValue: 0, min: 0, format: 'n4' }, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Superficie_Biologico', title: this.translocoService.translate('SuperficieBiologicoAbbr') }, { resizable: true, editable: false, format: 'n4', numeric: { defaultValue: 0, min: 0, format: 'n4' }, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Superficie_Convenzionale', title: this.translocoService.translate('SuperficieConvenzionaleAbbr') }, { resizable: true, editable: false, format: 'n4', numeric: { defaultValue: 0, min: 0, format: 'n4' }, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Superficie_Catastale', title: this.translocoService.translate('SuperficieCatastaleAbbr') }, { resizable: true, editable: false, format: 'n4', numeric: { defaultValue: 0, min: 0, format: 'n4' }, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
        ),
        new KendoGridColumn(
          { field: 'sup_contratto', title: this.translocoService.translate('SuperficieContrattoAbbr') }, { resizable: true, editable: false, numeric: { defaultValue: 0, min: 0, format: 'n4' }, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
        ),
        new KendoGridColumn(
          { field: 'filiera', title: this.translocoService.translate('Filiera') }, { resizable: true, editable: false, numeric: { defaultValue: 0, min: 0, format: 'n4' }, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
        ),
        // new KendoGridColumn(
        //   { field: 'Data_Creazione', title: this.translocoService.translate('DataCreazione') }, { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
        // ),
        // new KendoGridColumn(
        //   { field: 'Utente_Creazione', title: this.translocoService.translate('UtenteCreazione') }, { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
        // ),
        // new KendoGridColumn(
        //   { field: 'Data_Modifica', title: this.translocoService.translate('DataModifica') }, { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
        // ),
        // new KendoGridColumn(
        //   { field: 'Utente_Modifica', title: this.translocoService.translate('UtenteModifica') }, { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
        // ),
        // new KendoGridColumn(
        //   { field: 'Attivo', title: this.translocoService.translate('Attivo') }, { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
        // )
    ];
    
    kendoModelCampi= {
        sa_cod: new ModelEntry(CELL_TYPES.STRING),
        sa_nome: new ModelEntry(CELL_TYPES.STRING, false),
        chiave: new ModelEntry(CELL_TYPES.STRING, false),
        Campo: new ModelEntry(CELL_TYPES.STRING, false),
        Campo_Des: new ModelEntry(CELL_TYPES.STRING, false),
        Campo_Cod: new ModelEntry(CELL_TYPES.NUMBER, false),
        Validita_Inizio: new ModelEntry(CELL_TYPES.DATE, false),
        Validita_Fine: new ModelEntry(CELL_TYPES.DATE, false),
        Gru_Cod: new ModelEntry(CELL_TYPES.STRING, false),
        Veg_Cod: new ModelEntry(CELL_TYPES.STRING, false),
        // Utente_Modifica: new ModelEntry(CELL_TYPES.STRING, false),
        // Data_Modifica: new ModelEntry(CELL_TYPES.DATETIME, false),
        // Utente_Creazione: new ModelEntry(CELL_TYPES.STRING, false),
        // Data_Creazione: new ModelEntry(CELL_TYPES.DATETIME, false),
        Gru_Des: new ModelEntry(CELL_TYPES.STRING, false),
        Veg_Des: new ModelEntry(CELL_TYPES.STRING, false),
        PIVA: new ModelEntry(CELL_TYPES.STRING, false),
        Superficie_Totale: new ModelEntry(CELL_TYPES.NUMBER, false),
        Superficie_Convenzionale: new ModelEntry(CELL_TYPES.NUMBER, false),
        Superficie_Biologico: new ModelEntry(CELL_TYPES.NUMBER, false),
        Superficie_Conversione: new ModelEntry(CELL_TYPES.NUMBER, false),
        Superficie_Catastale: new ModelEntry(CELL_TYPES.NUMBER, false),
        rif_alfanumerico: new ModelEntry(CELL_TYPES.STRING, false),
        sup_contratto: new ModelEntry(CELL_TYPES.NUMBER, false),
        filiera: new ModelEntry(CELL_TYPES.NUMBER, false),
        // Attivo: new ModelEntry(CELL_TYPES.STRING, false),
        Descrizione: new ModelEntry(CELL_TYPES.CUSTOM, false),
        // ---------------------------------------------------------------------
        Ribaltato: new ModelEntry(CELL_TYPES.STRING, false),
        ribaltatoDes: new ModelEntry(CELL_TYPES.STRING, false),
        Data_Ribaltamento: new ModelEntry(CELL_TYPES.DATE, false),
    };

    kendoColumnsCentri = [
        new KendoGridColumn(
          { field: 'sa_nome', title: this.translocoService.translate('Descrizione') },
          { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'ind_des', title: this.translocoService.translate('Indirizzo') },
          { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
        ),
        new KendoGridColumn(
          { field: 'frz_des', title: this.translocoService.translate('Frazione') },
          { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
        ),
        new KendoGridColumn(
          { field: 'CAP', title: this.translocoService.translate('CAP') },
          { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Stato_Cod', title: this.translocoService.translate('Stato') },
          { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
        ),
        new KendoGridColumn(
          { field: 'note', title: this.translocoService.translate('Note') },
          { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Superficie_Catastale', title: this.translocoService.translate('SuperficieCatastaleAbbr') },
          { resizable: true, editable: false, width: 135, format: 'n4', numeric: { defaultValue: 0, min: 0, format: 'n4' } }
        ),
        new KendoGridColumn(
          { field: 'Superficie_Convenzionale', title: this.translocoService.translate('SuperficieConvenzionaleAbbr') },
          { resizable: true, editable: false, width: 135, format: 'n4', numeric: { defaultValue: 0, min: 0, format: 'n4' }, }
        ),
        new KendoGridColumn(
          { field: 'Superficie_Conversione', title: this.translocoService.translate('SuperficieConversioneAbbr') },
          { resizable: true, editable: false, width: 135, format: 'n4', numeric: { defaultValue: 0, min: 0, format: 'n4' }, }
        ),
        new KendoGridColumn(
          { field: 'Superficie_Biologico', title: this.translocoService.translate('SuperficieBiologicoAbbr') },
          { resizable: true, editable: false, width: 135, format: 'n4', numeric: { defaultValue: 0, min: 0, format: 'n4' }, }
        ),
        new KendoGridColumn(
          { field: 'Superficie_Totale', title: this.translocoService.translate('SuperficieTotaleAbbr') },
          { resizable: true, editable: false, width: 135, format: 'n4', numeric: { defaultValue: 0, min: 0, format: 'n4' }, }
        ),
        new KendoGridColumn(
          { field: 'Validita_Inizio', title: this.translocoService.translate('Validita_Inizio') },
          { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Validita_Fine', title: this.translocoService.translate('Validita_Fine') },
          { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
        ),
        // new KendoGridColumn(
        //   { field: 'Data_Creazione', title: this.translocoService.translate('DataCreazione') },
        //   { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
        // ),
        // new KendoGridColumn(
        //   { field: 'Utente_Creazione', title: this.translocoService.translate('UtenteCreazione') },
        //   { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
        // ),
        // new KendoGridColumn(
        //   { field: 'Data_Modifica', title: this.translocoService.translate('DataModifica') },
        //   { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
        // ),
        // new KendoGridColumn(
        //   { field: 'Utente_Modifica', title: this.translocoService.translate('UtenteModifica') },
        //   { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
        // ),
        new KendoGridColumn(
          { field: 'IndirizzoCompleto', title: this.translocoService.translate('Indirizzo') },
          { resizable: true, editable: false, media: '(max-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
        ),
    ]
  
    kendoModelCentri = {
        chiave: new ModelEntry(CELL_TYPES.STRING, false),
        sa_cod: new ModelEntry(CELL_TYPES.STRING, false),
        sa_nome: new ModelEntry(CELL_TYPES.STRING, false),
        Piva: new ModelEntry(CELL_TYPES.STRING, false),
        Rag_Soc: new ModelEntry(CELL_TYPES.STRING, false),
        cod_indirizzo: new ModelEntry(CELL_TYPES.STRING, false),
        ind_des: new ModelEntry(CELL_TYPES.STRING, false),
        frz_des: new ModelEntry(CELL_TYPES.STRING, false),
        CAP: new ModelEntry(CELL_TYPES.STRING, false),
        Stato2: new ModelEntry(CELL_TYPES.STRING, false),
        Stato_Cod: new ModelEntry(CELL_TYPES.STRING, false),
        note: new ModelEntry(CELL_TYPES.STRING, false),
        com_des: new ModelEntry(CELL_TYPES.STRING, false),
        pro_cod: new ModelEntry(CELL_TYPES.STRING, false),
        CodiceOperatoreBio: new ModelEntry(CELL_TYPES.STRING, false),
        tipoAttivitaCod: new ModelEntry(CELL_TYPES.STRING, false),
        Superficie_Catastale: new ModelEntry(CELL_TYPES.NUMBER, false),
        Superficie_Convenzionale: new ModelEntry(CELL_TYPES.NUMBER, false),
        Superficie_Conversione: new ModelEntry(CELL_TYPES.NUMBER, false),
        Superficie_Biologico: new ModelEntry(CELL_TYPES.NUMBER, false),
        Superficie_Totale: new ModelEntry(CELL_TYPES.NUMBER, false),
        Validita_Inizio: new ModelEntry(CELL_TYPES.DATE, false),
        Validita_Fine: new ModelEntry(CELL_TYPES.DATE, false),
        // Data_Creazione: new ModelEntry(CELL_TYPES.DATETIME, false),
        // Data_Modifica: new ModelEntry(CELL_TYPES.DATETIME, false),
        // Utente_Creazione: new ModelEntry(CELL_TYPES.STRING, false),
        // Utente_Modifica: new ModelEntry(CELL_TYPES.STRING, false),
        IndirizzoCompleto: new ModelEntry(CELL_TYPES.STRING, false),
    }

    kendoModelImpianti = {
        PIVA: new ModelEntry(CELL_TYPES.STRING, false),
        SA_COD: {type: CELL_TYPES.STRING},
        sa_nome: new ModelEntry(CELL_TYPES.STRING, false),
        APPEZZA: new ModelEntry(CELL_TYPES.NUMBER, false),
        id_Reg: new ModelEntry(CELL_TYPES.NUMBER, false),
        app_nome: new ModelEntry(CELL_TYPES.STRING, false),
        veg_des: new ModelEntry(CELL_TYPES.STRING, false),
        cul_des: new ModelEntry(CELL_TYPES.STRING, false),
        Grfi_Des: new ModelEntry(CELL_TYPES.STRING, false),
        grva_des: new ModelEntry(CELL_TYPES.STRING, false),
        dettSpeciePersonalizzatoDes: new ModelEntry(CELL_TYPES.STRING, false),
        sup_imp: new ModelEntry(CELL_TYPES.NUMBER, false),
        Sup_Int_Alt: new ModelEntry(CELL_TYPES.NUMBER, false),
        Udm_Des_Alt: new ModelEntry(CELL_TYPES.STRING, false),
        Validita_Inizio_Impianto: new ModelEntry(CELL_TYPES.DATE, true),
        Validita_Fine_Impianto: new ModelEntry(CELL_TYPES.DATE, true),
        Data_Inizio_Impianto: new ModelEntry(CELL_TYPES.DATE, true),
        Data_Inizio_Produzione: new ModelEntry(CELL_TYPES.DATE, true),
        Campo_Des: new ModelEntry(CELL_TYPES.STRING, false),
        Codice_Impianto: new ModelEntry(CELL_TYPES.STRING, false),
        port_des: new ModelEntry(CELL_TYPES.STRING, false),
        foral_des: new ModelEntry(CELL_TYPES.STRING, false),
        Cop_Des: new ModelEntry(CELL_TYPES.STRING, false),
        Data_Inizio_Portinnesto: new ModelEntry(CELL_TYPES.DATE, true),
        Reg_Des: new ModelEntry(CELL_TYPES.STRING, false),
        RegolamentoDisciplinare_Des: new ModelEntry(CELL_TYPES.STRING, false),
        Tipologia_Des: new ModelEntry(CELL_TYPES.STRING, false),
        fase_des: new ModelEntry(CELL_TYPES.STRING, false),
        Utilizzo: new ModelEntry(CELL_TYPES.STRING, false),
        Destinazione_Uso_Des: new ModelEntry(CELL_TYPES.STRING, false),
        descrizione: new ModelEntry(CELL_TYPES.CUSTOM, false),
        Metodo_Produzione_Des: new ModelEntry(CELL_TYPES.STRING, false),
        SupBZ_Riduzione: new ModelEntry(CELL_TYPES.NUMBER, false),
        DistBZ_CorpiIdrici: new ModelEntry(CELL_TYPES.NUMBER, false),
        DistBZ_AreeResPub: new ModelEntry(CELL_TYPES.NUMBER, false),
        DistBZ_Allevamenti: new ModelEntry(CELL_TYPES.NUMBER, false),
        DistBZ_VegNatNonColt: new ModelEntry(CELL_TYPES.NUMBER, false)
    };
    
    kendoColumnsImpianti: KendoGridColumn[] = [
        new KendoGridColumn(
          { field: 'sa_nome', title: this.translocoService.translate('Centro') },
          { resizable: true, editable: false, width: 120 }
        ),
        new KendoGridColumn(
          { field: 'Campo_Des', title: this.translocoService.translate('Campo') },
          { resizable: true, editable: false, width: 120 }
        ),
        new KendoGridColumn(
          { field: 'app_nome', title: this.translocoService.translate('Appezzamento') },
          { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 140 }
        ),
        new KendoGridColumn(
          { field: 'Metodo_Produzione_Des', title: this.translocoService.translate('MetodoDiProduzione') },
          { resizable: true, editable: false, width: 120 }
        ),
        new KendoGridColumn(
          { field: 'veg_des', title: this.translocoService.translate('Specie') },
          { resizable: true, editable: false, width: 120 }
        ),
        new KendoGridColumn(
          { field: 'cul_des', title: this.translocoService.translate('Varietà') },
          { resizable: true, editable: false, width: 120 }
        ),
        new KendoGridColumn(
          { field: 'Destinazione_Uso_Des', title: this.translocoService.translate('Utilizzo') },
          { resizable: true, editable: false, width: 120 }
        ),
        new KendoGridColumn(
          { field: 'Grfi_Des', title: this.translocoService.translate('Finalità') },
          { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 120 }
        ),
        new KendoGridColumn(
          { field: 'grva_des', title: this.translocoService.translate('TipologiaVarietale') },
          { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 150 }
        ),
        new KendoGridColumn(
          { field: 'dettSpeciePersonalizzatoDes', title: this.translocoService.translate('DettaglioVarietaPersonalizzato') },
          { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 150 }
        ),
        new KendoGridColumn(
          { field: 'sup_imp', title: this.translocoService.translate('Superficie') },
          {
            resizable: true,
            editable: false,
            numeric: { defaultValue: 0, min: 0, format: 'n4' },
            width: 120
          }
        ),
        new KendoGridColumn(
          { field: 'Sup_Int_Alt', title: this.translocoService.translate('SupAlternativa') },
          { resizable: true, editable: false, numeric: { defaultValue: 0, min: 0, format: 'n4' }, width: 120 }
        ),
        new KendoGridColumn(
          { field: 'Udm_Des_Alt', title: this.translocoService.translate('UnitaMisuraAlternativa') },
          { resizable: true, editable: false, width: 120 }
        ),
        new KendoGridColumn(
          { field: 'Validita_Inizio_Impianto', title: this.translocoService.translate('Validita_Inizio_Impianto') },
          { resizable: true, editable: false, width: 165 }
        ),
        new KendoGridColumn(
          { field: 'Validita_Fine_Impianto', title: this.translocoService.translate('Validita_Fine_Impianto') },
          { resizable: true, editable: false, width: 165 }
        ),
        new KendoGridColumn(
          { field: 'Data_Inizio_Impianto', title: this.translocoService.translate('DataInizioImpianto') },
          { resizable: true, editable: false, width: 165 }
        ),
        new KendoGridColumn(
          { field: 'Data_Inizio_Produzione', title: this.translocoService.translate('DataInizioProduzione') },
          { resizable: true, editable: false, width: 165 }
        ),
        new KendoGridColumn(
          { field: 'Codice_Impianto', title: this.translocoService.translate('CodiceImpianto') },
          { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 140 }
        ),
        new KendoGridColumn(
          { field: 'port_des', title: this.translocoService.translate('Portinnesto') },
          { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 130 }
        ),
        new KendoGridColumn(
          { field: 'foral_des', title: this.translocoService.translate('FormaAllevamento') },
          { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 160 }
        ),
        new KendoGridColumn(
          { field: 'Cop_Des', title: this.translocoService.translate('Copertura') },
          { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 130 }
        ),
        new KendoGridColumn(
          { field: 'Data_Inizio_Portinnesto', title: this.translocoService.translate('DataInizioPortinnesto') },
          { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 165 }
        ),
        new KendoGridColumn(
          { field: 'RegolamentoDisciplinare_Des', title: this.translocoService.translate('Vincolo') },
          { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Tipologia_Des', title: this.translocoService.translate('Tipologia') },
          { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
        ),
        new KendoGridColumn(
          { field: 'fase_des', title: this.translocoService.translate('Stato') },
          { resizable: true, editable: false, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }
        ),
        new KendoGridColumn(
          { field: 'SupBZ_Riduzione', title: this.translocoService.translate('SupBZRiduzione') },
          { resizable: true, editable: false, numeric: { defaultValue: 0, min: 0, format: 'n2' }, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 165 }
        ),
        new KendoGridColumn(
          { field: 'DistBZ_CorpiIdrici', title: this.translocoService.translate('DistBZCorpiIdrici') },
          { resizable: true, editable: false, numeric: { defaultValue: 0, min: 0, format: 'n2' }, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 165 }
        ),
        new KendoGridColumn(
          { field: 'DistBZ_AreeResPub', title: this.translocoService.translate('DistBZAreeResPub') },
          { resizable: true, editable: false, numeric: { defaultValue: 0, min: 0, format: 'n2' }, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 165 }
        ),
        new KendoGridColumn(
          { field: 'DistBZ_Allevamenti', title: this.translocoService.translate('DistBZAllevamenti') },
          { resizable: true, editable: false, numeric: { defaultValue: 0, min: 0, format: 'n2' }, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 165 }
        ),
        new KendoGridColumn(
          { field: 'DistBZ_VegNatNonColt', title: this.translocoService.translate('DistBZVegNatNonColt') },
          { resizable: true, editable: false, numeric: { defaultValue: 0, min: 0, format: 'n2' }, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 165 }
        )
    ];

    kendoColumnsAppezzamenti = [
        new KendoGridColumn({field: 'sa_nome', title: this.translocoService.translate('Centro')},{resizable:true,editable: false,width: 150}),
        new KendoGridColumn({field: 'Campo_Des', title: this.translocoService.translate('Campo')},{resizable:true,editable: false,width: 150}),
        new KendoGridColumn({field: 'APP_NOME',title: this.translocoService.translate('Nome')},{resizable:true,editable: false,width: 150}),
        new KendoGridColumn({field: 'utilizzo',title: this.translocoService.translate('Utilizzo')},{resizable:true,editable: false,width: 150}),
        new KendoGridColumn({field: 'rif_alfanumerico',title: this.translocoService.translate('RiferimentoAppezzamentoAbbr')},{hidden: true,resizable:true,editable: false,width: 150}),
        new KendoGridColumn({field: 'cod_biologico',title: this.translocoService.translate('AppBioCod')},{hidden: true,resizable:true,editable: false,width: 150}),
        new KendoGridColumn({field: 'cod_kpin',title: this.translocoService.translate('KPIN')},{hidden: true,resizable:true,editable: false,width: 100}),
        new KendoGridColumn({field: 'cod_block',title: this.translocoService.translate('BLOCK')},{hidden: true,resizable:true,editable: false,width: 100}),
        new KendoGridColumn({field: 'SUP_APP',title: this.translocoService.translate('SuperficieAppezzamentoAbbr')},{resizable:true,editable: false,width: 150,numeric:{ min: 0 }}),

        new KendoGridColumn({field: 'Validita_Inizio',title: this.translocoService.translate('InizioValidità')},{resizable:true,editable: false,width: 150}),
        new KendoGridColumn({field: 'Validita_Fine',title: this.translocoService.translate('FineValidità')},{resizable:true,editable: false,width: 150}),
        // new KendoGridColumn({field: 'Data_Modifica',title: this.translocoService.translate('DataModifica')},{resizable:true,editable: false,width: 150}),
        // new KendoGridColumn({field: 'utente_modifica',title: this.translocoService.translate('UtenteModifica')},{resizable:true,editable: false,width: 150}),
        // new KendoGridColumn({field: 'Data_Creazione',title: this.translocoService.translate('DataCreazione')},{resizable:true,editable: false,width: 150}),
        // new KendoGridColumn({field: 'utente_creazione',title: this.translocoService.translate('UtenteCreazione')},{resizable:true,editable: false,width: 150}),
        new KendoGridColumn({field: 'isola',title: this.translocoService.translate('Isola')},{resizable:true,editable: false,width: 150}),
        new KendoGridColumn({field: 'MetodoProduzione_Des',title: this.translocoService.translate('MetodoProduzione')},{resizable:true,editable: false,width: 150}),
        new KendoGridColumn({field: 'Blk_Flag_Des',title: this.translocoService.translate('Bloccato')},{hidden: true,resizable:true,editable: false,width: 150})
    ];

    kendoModelAppezzamenti = {
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
        // Data_Modifica: new ModelEntry(CELL_TYPES.DATETIME, false),
        // utente_modifica: new ModelEntry(CELL_TYPES.STRING, false),
        // Data_Creazione: new ModelEntry(CELL_TYPES.DATETIME, false),
        // utente_creazione: new ModelEntry(CELL_TYPES.STRING, false),
        isola: new ModelEntry(CELL_TYPES.STRING, false),
        // MetodoProduzione_Cod:{type: CELL_TYPES.STRING},
        MetodoProduzione_Des: new ModelEntry(CELL_TYPES.STRING, false),
        Blk_Flag: new ModelEntry(CELL_TYPES.NUMBER, false),
        Blk_Flag_Des: new ModelEntry(CELL_TYPES.STRING, false)
    };

}