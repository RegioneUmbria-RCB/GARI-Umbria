import { Component } from '@angular/core';
import { SHARED_IMPORTS } from 'app/profilazione/profilazione.module';
import { GiasDropDownTemplateService, GiasDropDownTemplateSComponent } from 'gias-ui-kit';
import { generateGridProviders } from 'gias-kendo-grid';
import { ChecklistGridService } from './checklist-grid.service';


@Component({
  selector: 'app-checklist-settings-grid',
  imports: [SHARED_IMPORTS],
  templateUrl: './checklist-settings-grid.component.html',
  styleUrl: './checklist-settings-grid.component.css',
  providers: [
    ...generateGridProviders(ChecklistGridService, ChecklistSettingsGridComponent),
    GiasDropDownTemplateService
  ]
})
export class ChecklistSettingsGridComponent {

}
