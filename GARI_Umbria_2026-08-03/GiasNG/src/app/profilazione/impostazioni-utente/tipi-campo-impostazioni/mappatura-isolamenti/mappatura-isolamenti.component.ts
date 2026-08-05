import { Component } from '@angular/core';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import {
  DropdownListItem,
  DropdownListWithForm,
  generateGridProviders,
  GiasKendoGridComponent,
  KendoGridColumn,
  KendoGridModel,
  KendoGridRow,
  ModelEntry
} from 'gias-kendo-grid';
import { SHARED_IMPORTS } from 'app/profilazione/profilazione.module';
import { MappaturaIsolamentiGridService } from './mappatura-isolamenti-grid.service';

@Component({
  selector: 'app-mappatura-isolamenti',
  imports: [SHARED_IMPORTS],
  templateUrl: './mappatura-isolamenti.component.html',
  styleUrl: './mappatura-isolamenti.component.css',
  providers: [
    ...generateGridProviders(MappaturaIsolamentiGridService, MappaturaIsolamentiComponent),
    GiasDropDownTemplateService
  ]
})
export class MappaturaIsolamentiComponent {

}
