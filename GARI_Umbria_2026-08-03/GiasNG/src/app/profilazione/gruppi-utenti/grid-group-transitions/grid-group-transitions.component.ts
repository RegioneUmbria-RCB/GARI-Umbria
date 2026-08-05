import { Component } from '@angular/core';
import {generateGridProviders} from 'gias-kendo-grid';
import {GridGroupTransitionsService} from "./grid-group-transitions.service";
import { GruppiUtentiService } from 'app/profilazione/services/gruppi-utenti.service';
import { isNullOrUndefined } from 'app/Service/utils';

@Component({
  standalone: false,
  selector: 'app-grid-group-transitions',
  templateUrl: './grid-group-transitions.component.html',
  styleUrls: ['./grid-group-transitions.component.css'],
  providers: [
    ...generateGridProviders(GridGroupTransitionsService, GridGroupTransitionsComponent)
  ]
})
export class GridGroupTransitionsComponent {
  public gruppoSelected: Boolean = false;

  constructor(private gruppiService: GruppiUtentiService) {
    this.gruppiService.gruppo$.subscribe(gruppo => {this.gruppoSelected = !isNullOrUndefined(gruppo)});
  }

  protected get currGruppoDesc(): string {
    if (this.gruppiService.gruppo$?.value) {
      return this.gruppiService.gruppo$.value.descrizione;
    } else return '';
  }
}
