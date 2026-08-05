import { Component } from '@angular/core';
import { TranslocoModule } from '@jsverse/transloco';
import { WorkflowSettingsGridComponent } from './workflow-settings-grid/workflow-settings-grid.component';
import { ChecklistSettingsGridComponent } from './checklist-settings-grid/checklist-settings-grid.component';

@Component({
  selector: 'app-documentary-management',
  imports: [TranslocoModule, WorkflowSettingsGridComponent, ChecklistSettingsGridComponent],
  templateUrl: './documentary-management.component.html',
  styleUrl: './documentary-management.component.css'
})
export class DocumentaryManagementComponent {

}
