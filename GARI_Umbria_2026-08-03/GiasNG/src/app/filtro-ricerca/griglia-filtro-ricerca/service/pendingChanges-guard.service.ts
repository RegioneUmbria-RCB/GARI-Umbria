
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { DialogWindowService } from './dialog-window.service';

export interface ComponentCanDeactivate {
  canDeactivate: () => boolean | Promise<boolean> | Observable<boolean>;
}

@Injectable()
export class PendingChangesGuard  {

    constructor(
                private dialogService: DialogWindowService
    ) {}

  canDeactivate(component: ComponentCanDeactivate): boolean | Promise<boolean> | Observable<boolean> {
    // if there are no pending changes, just allow deactivation; else confirm first
    return component.canDeactivate()
      ? true
      : // NOTE: this warning message will only be shown when navigating elsewhere within your angular app;
        // when navigating away from your angular app, the browser will show a generic warning message
        // see http://stackoverflow.com/a/42207299/7307355
        //confirm('WARNING: You have unsaved changes. Press Cancel to go back and save these changes, or OK to lose these changes.');
        this.dialogService.waitDialogResult();
  }
}