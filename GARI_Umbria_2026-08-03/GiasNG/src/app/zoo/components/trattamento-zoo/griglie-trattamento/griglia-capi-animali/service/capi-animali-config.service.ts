import { Injectable } from "@angular/core";
import { Validators } from "@angular/forms";
import { TranslocoService } from "@jsverse/transloco";
import { LoadMoreButtonTemplateDirective } from "@progress/kendo-angular-treeview";
import { Progetto } from "app/Model/attivita/centri_di_costo/Progetto";
import { KendoGridColumn, ModelEntry, NumericSettings } from "gias-kendo-grid";
import { CELL_TYPES } from "gias-ui-kit";

export class CapoAnimale {
    selezionato: string;
    chiave: number;
    Matricola: string;
    Raggruppamento_Des: string;
    STA_NUM: number;
    STA_DES: string;
    RAZ_DES: string;
    Sesso: string;
    Stato_Des: string;
    Validita_Inizio: Date;
    Validita_Fine: Date;
    Incremento_Teorico_Calcolato: number;
    Peso_Arrotondato: number;
    OnAntibiotico: boolean;
    OnAntinfiammatorio: boolean;
    Quantita: number;

    constructor(chiave, selezionato,matricola, raggruppamento_Des, sta_num, sta_des, raz_des, sesso, stato_des, validita_inizio, validita_fine, incremento_teorico_calcolato, peso_arrotondato, quantita) {
        this.chiave = chiave;
        this.selezionato = selezionato;
        this.Matricola = matricola;
        this.Raggruppamento_Des = raggruppamento_Des;
        this.STA_NUM = sta_num;
        this.STA_DES = sta_des;
        this.RAZ_DES = raz_des;
        this.Sesso = sesso;
        this.Stato_Des = stato_des;
        this.Validita_Inizio = validita_inizio;
        this.Validita_Fine = validita_fine;
        this.Peso_Arrotondato = peso_arrotondato;
        this.Incremento_Teorico_Calcolato = incremento_teorico_calcolato;
        this.Quantita = quantita;
    }
}


@Injectable()
export class CapiAnimaliConfigService { 

    constructor(
        public translocoService: TranslocoService
    ) {}

    kendoColumnsCapiAnimali: KendoGridColumn[] = [
        new KendoGridColumn(
          {field: 'chiave', title: this.translocoService.translate('Chiave')},{ resizable:true, filterable:false, editable: false, width: 135, hidden: true }
        ),
        new KendoGridColumn(
          {field: 'selezionato', title: this.translocoService.translate('S/N')},{ resizable:true, filterable:true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          {field: 'Matricola', title: this.translocoService.translate('Matricola')},{ resizable:true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          {field: 'Matricola_Breve', title: this.translocoService.translate('MatricolaBreve')},{ resizable:true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          {field: 'Matricola_Breve4', title: this.translocoService.translate('MatricolaBreve4')},{ resizable:true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          {field: 'Progetto', title: this.translocoService.translate('Progetto')},{ resizable:true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          {field: 'Tipo_Des', title: this.translocoService.translate('Tipo')},{ resizable:true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Raggruppamento_Des', title: this.translocoService.translate('Raggruppamento') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'STA_NUM', title: this.translocoService.translate('StallaNumero') }, { resizable: true, editable: false, width: 135, hidden: true }
        ),
        new KendoGridColumn(
          { field: 'STA_DES', title: this.translocoService.translate('StallaDes') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'RAZ_DES', title: this.translocoService.translate('Razza') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Sesso', title: this.translocoService.translate('Sesso') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Progetto_Nome', title: this.translocoService.translate('zoo.DistintaNome') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Codice_Distinta', title: this.translocoService.translate('Lotto2') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Metodo_Produzione', title: this.translocoService.translate('MetodoProduzione') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Mat_Madre', title: this.translocoService.translate('zoo.MatricolaMadre') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'RazDes_Madre', title: this.translocoService.translate('zoo.RazDesMadre') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Mat_Padre', title: this.translocoService.translate('zoo.MatricolaPadre') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'RazDes_Padre', title: this.translocoService.translate('zoo.RazDesPadre') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Lotto_Fornitore', title: this.translocoService.translate('LottoFornitore') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'CF_PROPRIETARIO', title: this.translocoService.translate('zoo.CFproprietario') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'CF_DETENTORE', title: this.translocoService.translate('zoo.CFdetentore') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Stalla_Svezzamento', title: this.translocoService.translate('zoo.StallaSvezzamento') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Certificato', title: this.translocoService.translate('Certificato') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Modello4_Ingresso_Numero', title: this.translocoService.translate('zoo.Modello4IngressoNum') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Modello4_Ingresso', title: this.translocoService.translate('zoo.Modello4Ingresso') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Modello4_Uscita_Numero', title: this.translocoService.translate('zoo.Modello4UscitaNum') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Modello4_Uscita', title: this.translocoService.translate('zoo.Modello4Uscita') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Codice_Azienda_Uscita', title: this.translocoService.translate('zoo.CodiceAziendaUscita') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Codice_Azienda_Fornitore', title: this.translocoService.translate('zoo.CodiceAziendaFornitore') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'CF_FornProv', title: this.translocoService.translate('zoo.CFfornitoreProvenienza') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'RagSoc_FornProv', title: this.translocoService.translate('zoo.RagSocFornProv') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'CF_FornFatt', title: this.translocoService.translate('zoo.CFfornitoreFatturazione') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'RagSoc_FornFatt', title: this.translocoService.translate('zoo.RagSocFornFatt') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Data_DDT_Ingresso', title: this.translocoService.translate('DataDDTIngresso') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Data_DDT_Uscita', title: this.translocoService.translate('DataDDTUscita') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Validato', title: this.translocoService.translate('Validato') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Anomalie', title: this.translocoService.translate('Anomalie') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Anomalie_Note', title: this.translocoService.translate('AnomalieNote') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'giorni_in_stalla', title: this.translocoService.translate('zoo.GiorniInStalla') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Eta_Giorni_TOTALI', title: this.translocoService.translate('zoo.EtaGiorniTotali') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'eta_mesi', title: this.translocoService.translate('zoo.EtaMesi') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'eta_giorni', title: this.translocoService.translate('zoo.EtaGiorni') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Stato_Des', title: this.translocoService.translate('Stato') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Validita_Inizio', title: this.translocoService.translate('Validita_Inizio') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Validita_Fine', title: this.translocoService.translate('Validita_Fine') }, { resizable: true, editable: false, width: 135, hidden: true }
        ),
        new KendoGridColumn(
          { field: 'Dat_Nascita', title: this.translocoService.translate('DataNascita') }, { resizable: true, editable: false, width: 135, hidden: true }
        ),
        new KendoGridColumn(
          { field: 'Incremento_Teorico_Calcolato', title: this.translocoService.translate('zoo.Peso_Stimato') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Peso_Arrotondato', title: this.translocoService.translate('zoo.Peso_Arrotondato') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'OnAntibiotico', title: this.translocoService.translate('Antibiotico') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'OnAntinfiammatorio', title: this.translocoService.translate('Antinfiammatorio') }, { resizable: true, editable: false, width: 135 }
        ),
        new KendoGridColumn(
          { field: 'Quantita', title: this.translocoService.translate('Quantita') }, 
          { resizable: true, editable: true, width: 135, 
            validators: [Validators.required, 
                         Validators.min(1),
                         (control) => {
                          const value = control.value;
                          if (value % 1 !== 0) {
                            return { notInteger: true };
                          }
                          return null;
                        }], 
            numeric: new NumericSettings({
                  defaultValue: 0,
                  format: 'n2',
                  decimals: 2,
                  min: 0
            })
          }
        ),
    ];
    
    kendoModelCapiAnimali= {
        chiave: new ModelEntry(CELL_TYPES.STRING),
        selezionato: new ModelEntry(CELL_TYPES.STRING, false),
        Matricola: new ModelEntry(CELL_TYPES.STRING, false),
        Matricola_Breve: new ModelEntry(CELL_TYPES.STRING, false),
        Matricola_Breve4: new ModelEntry(CELL_TYPES.STRING, false),
        Progetto: new ModelEntry(CELL_TYPES.STRING, false),
        Tipo_Des: new ModelEntry(CELL_TYPES.STRING, false),
        Raggruppamento_Des: new ModelEntry(CELL_TYPES.STRING, false),
        STA_NUM: new ModelEntry(CELL_TYPES.NUMBER, false),
        STA_DES: new ModelEntry(CELL_TYPES.STRING, false),
        RAZ_DES: new ModelEntry(CELL_TYPES.STRING, false),
        Sesso: new ModelEntry(CELL_TYPES.STRING, false),
        Progetto_Nome: new ModelEntry(CELL_TYPES.STRING, false),
        Codice_Distinta: new ModelEntry(CELL_TYPES.STRING, false),
        Metodo_Produzione: new ModelEntry(CELL_TYPES.STRING, false),
        Mat_Madre: new ModelEntry(CELL_TYPES.STRING, false),
        RazDes_Madre: new ModelEntry(CELL_TYPES.STRING, false),
        Mat_Padre: new ModelEntry(CELL_TYPES.STRING, false),
        RazDes_Padre: new ModelEntry(CELL_TYPES.STRING, false),
        Lotto_Fornitore: new ModelEntry(CELL_TYPES.STRING, false),
        CF_PROPRIETARIO: new ModelEntry(CELL_TYPES.STRING, false),
        CF_DETENTORE: new ModelEntry(CELL_TYPES.STRING, false),
        Stalla_Svezzamento: new ModelEntry(CELL_TYPES.STRING, false),
        Certificato: new ModelEntry(CELL_TYPES.STRING, false),
        Modello4_Ingresso_Numero: new ModelEntry(CELL_TYPES.STRING, false),
        Modello4_Ingresso: new ModelEntry(CELL_TYPES.STRING, false),
        Dat_Nascita: new ModelEntry(CELL_TYPES.DATE, false),
        Modello4_Uscita_Numero: new ModelEntry(CELL_TYPES.STRING, false),
        Modello4_Uscita: new ModelEntry(CELL_TYPES.STRING, false),
        Codice_Azienda_Uscita: new ModelEntry(CELL_TYPES.STRING, false),
        Codice_Azienda_Fornitore: new ModelEntry(CELL_TYPES.STRING, false),
        CF_FornProv: new ModelEntry(CELL_TYPES.STRING, false),
        RagSoc_FornProv: new ModelEntry(CELL_TYPES.STRING, false),
        CF_FornFatt: new ModelEntry(CELL_TYPES.STRING, false),
        RagSoc_FornFatt: new ModelEntry(CELL_TYPES.STRING, false),
        Data_DDT_Ingresso: new ModelEntry(CELL_TYPES.DATE, false),
        Data_DDT_Uscita: new ModelEntry(CELL_TYPES.DATE, false),
        Validato: new ModelEntry(CELL_TYPES.STRING, false),
        Anomalie: new ModelEntry(CELL_TYPES.STRING, false),
        Anomalie_Note: new ModelEntry(CELL_TYPES.STRING, false),
        giorni_in_stalla: new ModelEntry(CELL_TYPES.NUMBER, false),
        Eta_Giorni_TOTALI: new ModelEntry(CELL_TYPES.NUMBER, false),
        eta_mesi: new ModelEntry(CELL_TYPES.NUMBER, false),
        eta_giorni: new ModelEntry(CELL_TYPES.NUMBER, false),
        Stato_Des: new ModelEntry(CELL_TYPES.STRING, false),
        Validita_Inizio: new ModelEntry(CELL_TYPES.DATE, false),
        Validita_Fine: new ModelEntry(CELL_TYPES.DATE, false),
        Incremento_Teorico_Calcolato: new ModelEntry(CELL_TYPES.NUMBER, false),
        Peso_Arrotondato: new ModelEntry(CELL_TYPES.NUMBER, false),
        OnAntibiotico: new ModelEntry(CELL_TYPES.BOOLEAN, false),
        OnAntinfiammatorio: new ModelEntry(CELL_TYPES.BOOLEAN, false),
        Quantita: new ModelEntry(CELL_TYPES.NUMBER, true)
    };

}