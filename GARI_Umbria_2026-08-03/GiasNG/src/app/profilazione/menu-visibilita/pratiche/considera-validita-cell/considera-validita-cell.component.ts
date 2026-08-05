import { Component, Input } from '@angular/core';
import { CustomComponent } from 'gias-kendo-grid';
import { PraticaSelezionabile } from 'app/profilazione/models/pratiche-visibilita.model';

@Component({
  standalone: false,
  selector: 'app-considera-validita-cell',
  template: `
    <gias-grid-boolean
      [defaultValue]="input.ConsideraValiditaTemporale"
      [editable]="edit && input.Selected"
      [useCheckbox]="true"
      (valueChange)="onValueChange($event)">
    </gias-grid-boolean>
  `,
})
export class ConsideraValiditaCellComponent implements CustomComponent {
  @Input() input: PraticaSelezionabile;
  @Input() edit: boolean;
  @Input() field: string;

  onValueChange(value: boolean): void {
    this.input.ConsideraValiditaTemporale = value;
  }
}
