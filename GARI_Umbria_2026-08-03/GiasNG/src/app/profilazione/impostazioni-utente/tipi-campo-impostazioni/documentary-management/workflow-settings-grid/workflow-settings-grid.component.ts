import { Component } from '@angular/core';
import { SHARED_IMPORTS } from 'app/profilazione/profilazione.module';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { generateGridProviders } from 'gias-kendo-grid';
import { WorkflowGridService } from './workflow-grid.service';

@Component({
  selector: 'app-workflow-settings-grid',
  imports: [SHARED_IMPORTS],
  templateUrl: './workflow-settings-grid.component.html',
  styleUrl: './workflow-settings-grid.component.css',
  providers: [
    ...generateGridProviders(WorkflowGridService, WorkflowSettingsGridComponent),
    GiasDropDownTemplateService
  ]
})
export class WorkflowSettingsGridComponent {

}
