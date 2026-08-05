import { KendoWindowsService, WindowArgs, WindowTypes } from 'app/Service';
import { Command } from '../GIS-layer-window-toolbar.component';
import { DEFAULT_TOP_POSITION } from 'app/GIS/GIS-toolbar/gis-toolbar.service';
import { TranslocoService } from '@jsverse/transloco';

export class OpenBookmarksCommand implements Command {
  constructor(
    private kendoWindowsService: KendoWindowsService,
    private translocoService: TranslocoService
  ) { }

  do(): void {
    if (this.kendoWindowsService.getOpenState(WindowTypes.BookmarksWindow)) {
      this.kendoWindowsService.close(WindowTypes.BookmarksWindow);
      return;
    }

    const title = this.translocoService.translate('GestionePreferiti');
    const args = this.kendoWindowsService.getWindowArgs(WindowTypes.BookmarksWindow)
      ?? new WindowArgs(WindowTypes.BookmarksWindow, true, title, undefined, 700, undefined, undefined, DEFAULT_TOP_POSITION, true, true, true, false, false, false, true);
    this.kendoWindowsService.open(WindowTypes.BookmarksWindow, args);
  }
}
