import { AfterViewInit, Component, Inject, OnInit } from '@angular/core';
import { AnagraficheRilieviAvversitaGridConfig } from './anagrafiche-rilievi-avversita-grid-config.service';
import { generateGridProviders } from 'gias-kendo-grid';
import { GRID_HTTP_TOKEN } from 'gias-kendo-grid';

@Component({
  standalone: false,
  selector: 'app-anagrafiche-rilievi-avversita',
  templateUrl: './anagrafiche-rilievi-avversita.component.html',
  styleUrls: ['./anagrafiche-rilievi-avversita.component.scss'],
  providers: [
    ...generateGridProviders(AnagraficheRilieviAvversitaGridConfig, AnagraficheRilieviAvversitaComponent)
  ]
})
export class AnagraficheRilieviAvversitaComponent implements OnInit, AfterViewInit {

  showGrid: boolean = false;
  constructor(@Inject(GRID_HTTP_TOKEN) private anagraficheRilieviAvversitaGridConfig: AnagraficheRilieviAvversitaGridConfig) { }

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    setTimeout(() => this.showGrid = true, 0);
  }

}
