import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { CellClickEvent, CellCloseEvent } from '@progress/kendo-angular-grid';
import { Keys } from '@progress/kendo-angular-common';
import { Traduzioni_In } from 'app/Service/net-core6-api.service';
import { Gis_Traduzione } from 'app/Service/api.service';

@Component({
  standalone: false,
  selector: 'gis-translations-grid',
  templateUrl: './gis-translations-grid.component.html',
  styleUrls: ['./gis-translations-grid.component.css']
})
export class GISTranslationsGridComponent implements OnChanges {

  @Input() translations: Gis_Traduzione[];
  @Input() loading: boolean;
  @Output() updated = new EventEmitter<Gis_Traduzione[]>();

  constructor(private formBuilder: FormBuilder) { }

  ngOnChanges(changes: SimpleChanges): void {
  }

  cellClickHandler(args: CellClickEvent): void {
    if (!args.isEdited) {
      args.sender.editCell(
        args.rowIndex,
        args.columnIndex,

        this.formBuilder.group({
          Lingua_Cod: args.dataItem.Lingua_Cod,
          Lingua_Des: args.dataItem.Lingua_Des,
          Traduzione: args.dataItem.Traduzione
        })
      );
    }
  }

  cellCloseHandler(args: CellCloseEvent): void {
    const { formGroup, dataItem } = args;

    if (!formGroup.valid) {
      // prevent closing the edited cell if there are invalid values.
      args.preventDefault();
    } else if (formGroup.dirty) {
      if (args.originalEvent && args.originalEvent.keyCode === Keys.Escape) {
        return;
      }

      this.translations.find(t => t.Lingua_Cod == dataItem.Lingua_Cod).Traduzione = formGroup.getRawValue().Traduzione;
    }
  }

  save(): void {
    // adding a timeout to fire the cellCloseHandler
    setTimeout(() => this.updated.emit(this.translations), 100);
  }
}
