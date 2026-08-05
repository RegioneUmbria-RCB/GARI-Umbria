import { Component, Input, OnInit } from '@angular/core';
import { ExcelExportComponent } from '@progress/kendo-angular-excel-export';
import { Indicatore } from '../indicatori-widget.models';

@Component({
  standalone: false,
  selector: 'app-indicatori-widget-grid',
  templateUrl: './indicatori-widget-grid.component.html',
  styleUrls: ['./indicatori-widget-grid.component.css'],
})
export class IndicatoriWidgetGridComponent implements OnInit {
  @Input() data: Indicatore[] = [];
  @Input() height: number;

  columns: IndicatoriWidgetGridColumn[] = [];
  rows: IndicatoriWidgetGridRow[] = [];

  ngOnInit(): void {
    this.columns = this.computeColumns();
    this.rows = this.computeRows();
  }

  saveExcel(component: ExcelExportComponent): void {
    const options = component.workbookOptions();
    const sheet = options.sheets[0];
    let rd = 0;
    for (const row of sheet.rows) {
      row.cells[3].textAlign = "right";
      if (row.type === "header") {
        for (let c = 0; c < row.cells.length; c++) {
          row.cells[c].background = "#428BCA";
          row.cells[c].verticalAlign = "center";
        }
      } else if (row.type === "data") {
        row.cells[3].color = component.data[rd].colore;
        rd++;
      }
    }

    component.save(options);
  }

  private computeColumns(): IndicatoriWidgetGridColumn[] {
    const model = {}; // Keep track of all columns

    const columns = this.getDefaultColumns();
    for (const c of columns) {
      model[c.field] = 1;
    };

    for (const indicatore of this.data) {
      if (indicatore.Risultato.OutputGridValues == null) {
        continue;
      }

      for (const value of indicatore.Risultato.OutputGridValues) {
        let field = value.field + "_" + value.valueType;

        if (model.hasOwnProperty(field)) {
          continue;
        }

        model[field] = 1;
        const column = { field: field, title: value.title } as IndicatoriWidgetGridColumn;

        if (String(value.valueType).toUpperCase() !== "STRING") {

          column.headerClass = "right-align";

          if (value.outputFormat !== "") {
            column.format = "{0:" + value.outputFormat + "}";
          }
        }

        columns.push(column);
      }
    }

    return columns;
  }

  private computeRows(): IndicatoriWidgetGridRow[] {
    let rows = [];

    for (const indic of this.data) {

      let indic_row: IndicatoriWidgetGridRow = {
        stazione: indic.Stazione,
        modello: indic.Modello,
        settings: indic.DescrParametri,
        rischio: "",
        messaggio: "",
        colore: undefined
      };

      if (indic.Risultato.OutputGridValues != null) {
        for (const value of indic.Risultato.OutputGridValues) {
          let field = value.field + "_" + value.valueType;
          indic_row[field] = value.value;
        }
      }

      if (indic.Risultato.Status === "-1" || indic.Risultato.Status === "2") {
        indic_row.messaggio = indic.Risultato.StatusMsg;
      } else {

        if (indic.Risultato.Status === "3") {
          indic_row.messaggio = indic.Risultato.StatusMsg + " (" + indic.Risultato.Value.toFixed(1) + "%)";
        } else {

          let msg = indic.Risultato.AuxMsg;
          if (indic.Risultato.Status === "1") {

            if (indic.Risultato.AuxMsg === "") {
              msg = indic.Risultato.StatusMsg;
            } else {
              msg = indic.Risultato.StatusMsg + " " + indic.Risultato.AuxMsg;
            }
          }

          indic_row.messaggio = msg;

          let riskAvail = ['Basso', 'Medio', 'Alto', 'Estremo'];
          let nBands = indic.Risultato.Bands.length;
          let b = 0;
          while (b < nBands - 1) {
            if (indic.Risultato.Value < indic.Risultato.Bands[b].Value) {
              indic_row.rischio = riskAvail[Math.min(b, riskAvail.length - 1)];
              indic_row.colore = indic.Risultato.Bands[b].Color;
              b = nBands;
            }
            b++;
          }
          if (b < nBands) {
            indic_row.rischio = riskAvail[Math.min(b, riskAvail.length - 1)];
            indic_row.colore = indic.Risultato.Bands[b].Color;
          }

        }
      }

      rows.push(indic_row);
    }

    return rows;
  }

  private getDefaultColumns(): IndicatoriWidgetGridColumn[] {
    return [
      { field: 'stazione', title: 'Stazione' } as IndicatoriWidgetGridColumn,
      { field: 'modello', title: 'Modello' } as IndicatoriWidgetGridColumn,
      { field: 'settings', title: 'Impostazioni calcolo' } as IndicatoriWidgetGridColumn,
      { field: 'rischio', title: 'Rischio', headerClass: 'right-align' } as IndicatoriWidgetGridColumn,
      { field: 'messaggio', title: 'Messaggio' } as IndicatoriWidgetGridColumn
    ]
  }
}

interface IndicatoriWidgetGridColumn {
  field: string;
  title: string;
  headerClass: string | undefined;
  format: string | undefined;
}

interface IndicatoriWidgetGridRow {
  stazione: string;
  modello: string;
  settings: string;
  rischio: string;
  messaggio: string;
  colore: string | undefined
}
