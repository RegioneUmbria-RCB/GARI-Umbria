import {Component, input, OnInit} from '@angular/core';
import {KeyValue} from '@angular/common';
import {HttpAction, KendoGridColumn, KendoGridModel, ModelEntry} from 'gias-kendo-grid';
import {TranslocoService} from '@jsverse/transloco';
import {LightGridComponent} from '../../../../Utility/Template/kendo-grid/light-grid/light-grid.component';
import {CELL_TYPES} from 'gias-ui-kit';

@Component({
  standalone: true,
  selector: 'app-agea-codes',
  imports: [
    LightGridComponent
  ],
  templateUrl: './agea-codes.component.html',
  styleUrl: './agea-codes.component.css',
  providers: []
})
export class AgeaCodesComponent implements OnInit{
  codes = input.required<KeyValue<string, string>[]>();

  gridModel: KendoGridModel = {
    key: new ModelEntry(CELL_TYPES.STRING, false),
    value: new ModelEntry(CELL_TYPES.STRING, false)
  };

  gridColumns: KendoGridColumn[] = [
    new KendoGridColumn(
      {field: 'key', title: this.transloco.translate('Codice')},
      {resizable: true, editable: false, width: 120}
    ),
    new KendoGridColumn(
      {field: 'value', title: this.transloco.translate('Valore')},
      {resizable: true, editable: false, width: 120}
    ),
  ];

  constructor(
    private transloco: TranslocoService
  ) {  }

  ngOnInit(): void {
  }

  perform = (action: HttpAction, items: any) => undefined;
}
