import { AfterViewInit, Component, OnInit, Inject } from '@angular/core';
import { AnagraficheIndiciMaturitaGridConfig } from './anagrafiche-indici-maturita-grid-config.service';
import { generateGridProviders } from 'gias-kendo-grid';
import { GRID_HTTP_TOKEN } from 'gias-kendo-grid';


@Component({
  standalone: false,
  selector: 'app-anagrafiche-indici-maturita',
  templateUrl: './anagrafiche-indici-maturita.component.html',
  styleUrls: ['./anagrafiche-indici-maturita.component.scss'],
  providers: [
    ...generateGridProviders(AnagraficheIndiciMaturitaGridConfig, AnagraficheIndiciMaturitaComponent)
  ]
})
export class AnagraficheIndiciMaturitaComponent implements OnInit, AfterViewInit {
  
  showGrid: boolean = false;
  constructor(@Inject(GRID_HTTP_TOKEN) private anagraficheIndiciMaturitaGridConfig: AnagraficheIndiciMaturitaGridConfig) { }

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    setTimeout(() => this.showGrid = true, 0);
  }

}
