import {Component, Input, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {generateGridProviders, GiasKendoGridComponent} from 'gias-kendo-grid';
import {LettureContatoriGridConfigService} from "./letture-contatori-aziendali-grid.service";
import {Subject, switchMap, takeUntil } from 'rxjs';
import { LettureContatoriFilters } from 'app/domanda-irrigua/pages/letture-contatori/letture-contatori.component';

@Component({
  standalone: false,
  selector: 'app-letture-contatori-aziendali-grid',
  templateUrl: './letture-contatori-aziendali-grid.component.html',
  styleUrls: ['./letture-contatori-aziendali-grid.component.css'],
  providers: [
    ...generateGridProviders(LettureContatoriGridConfigService, LettureContatoriAziendaliGridComponent),
  ]
})
export class LettureContatoriAziendaliGridComponent implements OnInit, OnDestroy {
  @ViewChild('LettureContatoriAziendaliGrid') grid: GiasKendoGridComponent;
  @Input() searchFilters$: Subject<LettureContatoriFilters>;

  private signal = new Subject<void>();

  constructor() { }

  ngOnInit(): void {
    this.searchFilters$.pipe(
      takeUntil(this.signal),
      switchMap(filters => this.grid.config.read(filters))
    ).subscribe(() => this.grid.publicService.refresh(true));
  }

  ngOnDestroy(): void {
    this.signal.next();
    this.signal.complete();
  }
}
