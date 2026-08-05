import { Injectable } from "@angular/core";
import { Validators } from "@angular/forms";
import { TranslocoService } from "@jsverse/transloco";
import { BooleanSettings, KendoGridColumn, ModelEntry, NumericSettings } from "gias-kendo-grid";
import { CELL_TYPES } from "gias-ui-kit";

export class ProtocolGridItem {
    chiave: number;
    Matricola: string;
    Sta_Num: number;
    Sta_Des: string;
    Raggruppamento_Cod: number;
    Raggruppamento_Des: string;
    Raz_Des: string;
    Stato_Des: string;
    Validita_Inizio: Date;
    Validita_Fine: Date;
    Incremento_Teorico_Calcolato: number;
    Peso_Arrotondato: number;
    Quantita: number;

    constructor(chiave, matricola, sta_num, sta_des, raggruppamento_Cod, raggruppamento_Des, raz_des, stato_des, validita_inizio, validita_fine, incremento_teorico_calcolato, peso_arrotondato, quantita) {
        this.chiave = chiave;
        this.Matricola = matricola;
        this.Raggruppamento_Cod = raggruppamento_Cod;
        this.Raggruppamento_Des = raggruppamento_Des;
        this.Sta_Num = sta_num;
        this.Sta_Des = sta_des;
        this.Raz_Des = raz_des;
        this.Stato_Des = stato_des;
        this.Validita_Inizio = validita_inizio;
        this.Validita_Fine = validita_fine;
        this.Peso_Arrotondato = peso_arrotondato;
        this.Incremento_Teorico_Calcolato = incremento_teorico_calcolato;
        this.Quantita = quantita;
    }
}

@Injectable()
export class ProtocolsGridConfigService {

    constructor(
        public transloco: TranslocoService
    ) {}

    kendoCols: KendoGridColumn[] = [
        new KendoGridColumn(
            { field: 'IdRicetta', title: this.transloco.translate('ID') },
            { resizable: true, filterable: false, editable: false, width: 135, hidden: true }
        ),
        new KendoGridColumn(
            { field: 'selezionato', title: this.transloco.translate('S/N') },
            { resizable: true, filterable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
            { field: 'DataEmissione', title: this.transloco.translate('DataEmissione') },
            { resizable: true, filterable: true, editable: false, width: 150 }
        ),
        new KendoGridColumn(
            { field: 'Numero', title: this.transloco.translate('Numero') },
            { resizable: true, filterable: true, editable: false, width: 150 }
        ),
        new KendoGridColumn(
            { field: 'Denominazione', title: this.transloco.translate('Denominazione') },
            { resizable: true, filterable: true, editable: false, width: 250 }
        ),
        new KendoGridColumn(
            { field: 'Prot_Alt', title: this.transloco.translate('ProtocolloAlternativo') },
            { resizable: true, filterable: true, editable: true, width: 250 }
        ),
        new KendoGridColumn(
            { field: 'Note', title: this.transloco.translate('Note') },
            { resizable: true, filterable: true, editable: false, width: 250 }
        ),
        new KendoGridColumn(
            { field: 'rag_soc', title: this.transloco.translate('RagioneSociale') },
            { resizable: true, filterable: true, editable: false, width: 250 }
        ),
        new KendoGridColumn(
            { field: 'sa_nome', title: this.transloco.translate('CentroAziendale') },
            { resizable: true, filterable: true, editable: false, width: 250 }
        ),
        new KendoGridColumn(
            { field: 'STA_DES', title: this.transloco.translate('Stalla') },
            { resizable: true, filterable: true, editable: false, width: 250 }
        ),
        new KendoGridColumn(
            { field: 'Posologia', title: this.transloco.translate('Posologia') },
            { resizable: true, filterable: true, editable: false, width: 200 }
        ),
        new KendoGridColumn(
            { field: 'ProprietarioIdFiscale', title: this.transloco.translate('Proprietario') },
            { resizable: true, filterable: true, editable: false, width: 150 }
        ),
        new KendoGridColumn(
            { field: 'VeterinarioIdFiscale', title: this.transloco.translate('Veterinario') },
            { resizable: true, filterable: true, editable: false, width: 150 }
        ),
        new KendoGridColumn(
            { field: 'DetentoreIdFiscale', title: this.transloco.translate('Detentore') },
            { resizable: true, filterable: true, editable: false, width: 150 }
        ),
        new KendoGridColumn(
            { field: 'Username_Creazione', title: this.transloco.translate('UtenteCreazione') },
            { resizable: true, filterable: true, editable: false, width: 150 }
        ),
        new KendoGridColumn(
            { field: 'Username_Modifica', title: this.transloco.translate('UtenteModifica') },
            { resizable: true, filterable: true, editable: false, width: 150 }
        ),
        new KendoGridColumn(
            { field: 'Numero_Somm', title: this.transloco.translate('NSomministrazioni') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 150,
                format: '{0:n0}',
                numeric: new NumericSettings({ defaultValue: 0, min: 0, format: 'n0', step: 1, decimals: 0 })
            }
        ),
        new KendoGridColumn(
            { field: 'Intervallo_Somm', title: this.transloco.translate('IntSomministrazioni') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 150,
                format: '{0:n0}',
                numeric: new NumericSettings({ defaultValue: 0, min: 0, format: 'n0', step: 1, decimals: 0 })
            }
        ),
        new KendoGridColumn(
            { field: 'Qta_Dose', title: this.transloco.translate('DoseProdotto.text') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                width: 150,
                format: '{0:n2}',
                numeric: new NumericSettings({ defaultValue: 0, min: 0, format: 'n2', step: 0.01, decimals: 2 })
            }
        ),
        new KendoGridColumn(
            { field: 'Udm_Dose', title: this.transloco.translate('Unita_Misura') },
            { resizable: true, filterable: true, editable: false, width: 150, validators: [Validators.required] }
        ),
        new KendoGridColumn(
            { field: 'Arrotondamento_Peso', title: this.transloco.translate('Arrotondamento_Peso') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                format: '{0:n0}',
                numeric: new NumericSettings({ defaultValue: 0, min: 0, format: 'n0', step: 1, decimals: 0 })
            }
        ),
        new KendoGridColumn(
            { field: 'Massivo', title: this.transloco.translate('Massivo') },
            {
                resizable: true,
                filterable: true,
                editable: false,
                hidden: false,
                boolean: new BooleanSettings({ defaultValue: false, leftLabel: this.transloco.translate('No'), rightLabel: this.transloco.translate('Sì') })
            }
        ),
    ];

    kendoModelEntries = {
        IdRicetta: new ModelEntry(CELL_TYPES.NUMBER, false),
        selezionato: new ModelEntry(CELL_TYPES.STRING, false),
        Numero: new ModelEntry(CELL_TYPES.STRING, false),
        Denominazione: new ModelEntry(CELL_TYPES.STRING, false),
        Prot_Alt: new ModelEntry(CELL_TYPES.DROPDOWNLIST, true),
        Prot_Alt_Des: new ModelEntry(CELL_TYPES.STRING, true),
        ProprietarioIdFiscale: new ModelEntry(CELL_TYPES.STRING, false),
        VeterinarioIdFiscale: new ModelEntry(CELL_TYPES.STRING, false),
        DetentoreIdFiscale: new ModelEntry(CELL_TYPES.STRING, false),
        DataEmissione: new ModelEntry(CELL_TYPES.DATE, false),
        Note: new ModelEntry(CELL_TYPES.STRING, false),
        rag_soc: new ModelEntry(CELL_TYPES.STRING, false),
        sa_nome: new ModelEntry(CELL_TYPES.STRING, false),
        STA_DES: new ModelEntry(CELL_TYPES.STRING, false),
        Posologia: new ModelEntry(CELL_TYPES.STRING, false),
        Udm_Dose: new ModelEntry(CELL_TYPES.STRING, false),
        Username_Creazione: new ModelEntry(CELL_TYPES.STRING, false),
        Username_Modifica: new ModelEntry(CELL_TYPES.STRING, false),
        Numero_Somm: new ModelEntry(CELL_TYPES.NUMBER, false),
        Intervallo_Somm: new ModelEntry(CELL_TYPES.NUMBER, false),
        Qta_Dose: new ModelEntry(CELL_TYPES.NUMBER, false),
        Arrotondamento_Peso: new ModelEntry(CELL_TYPES.NUMBER, false),
        Massivo: new ModelEntry(CELL_TYPES.BOOLEAN, false)
    };
}