import {Component, input, OnInit} from '@angular/core';
import {generateGridProvidersAnagrafica} from '../services/providers';
import {ProvideAnagraficaTreeDeps} from '../../kendo-tree/utility/providers';
import {GiasKendoGridModule, HttpAction, KendoGridColumn, KendoGridModel} from 'gias-kendo-grid';
import {map, Observable} from 'rxjs';
import {LightGridDataService, LightGridServerResult, LightGridService} from './light-grid.service';

@Component({
  standalone: true,
  selector: 'app-light-grid',
  imports: [
    GiasKendoGridModule
  ],
  templateUrl: './light-grid.component.html',
  styleUrl: './light-grid.component.css',
  providers: [
    ...generateGridProvidersAnagrafica(LightGridService, LightGridComponent),
    ...ProvideAnagraficaTreeDeps(),
    LightGridDataService
  ]
})
export class LightGridComponent<T extends unknown[]> implements OnInit {
  data = input.required<T>();
  model = input.required<KendoGridModel>();
  columns = input.required<KendoGridColumn[]>();
  perform = input.required<(action: HttpAction, items: any) => Observable<any[]>>();

  class = input<string>('');

  constructor(
    private dataService: LightGridDataService<T>
  ) {  }

  ngOnInit(): void {
    this.dataService.model = this.model();
    this.dataService.columns = this.columns();
    this.dataService.data.next(this.data());
    this.dataService.read = (options) => this.dataService.data.pipe(
      map((v) => new LightGridServerResult(this.dataService.columns, this.dataService.model, v))
    );
    this.dataService.perform = (action, items) => undefined;
  }
}
