import { Inject, Injectable, TemplateRef } from '@angular/core';
import { WindowService, WindowSettings } from "@progress/kendo-angular-dialog";
import { take, tap } from "rxjs";
import { GIAS_MASTER_SERVICE_TOKEN, IGiasMasterService } from '../utils/gias-master.service';
import { messaggioPostMessage } from '../utils/models';

@Injectable({
  providedIn: 'root'
})
export class GiasWindowsService {

  private templateRef: TemplateRef<any>;

  functionMessageEventListener: any;

  constructor(
    private windowService: WindowService,
    @Inject(GIAS_MASTER_SERVICE_TOKEN) private masterService: IGiasMasterService
  ) { }

  open(settings: WindowSettings, messageEventListener = true) {
    settings.titleBarContent = this.templateRef;
    this.masterService.changeShowBackground(true);
    const windowRef = this.windowService.open(settings);
    windowRef.result.pipe(
      take(1),
      tap((val) => {
        this.masterService.changeShowBackground(false);
        window.removeEventListener('message', this.functionMessageEventListener);
      })
    ).subscribe();

    if (messageEventListener === true) {

      this.functionMessageEventListener = (event => {
        if (event.data.messaggio) {
          if (event.data.messaggio === messaggioPostMessage.chiudiWindowGiasNG) {
            windowRef.close();
          }
        } else {
          if (event.data && event.data === messaggioPostMessage.chiudiWindowGiasNG) {
            windowRef.close();
          }
        }
      }).bind(this);

      window.addEventListener('message', this.functionMessageEventListener);

    }

    return windowRef;
  }
}
