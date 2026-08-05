import { GridDataResult } from '@progress/kendo-angular-grid';
import { GiasKendoGridComponent } from '../kendo-grid.component';
import { ScrollingMode } from '../models/configuration.model';
import { KendoServerResult } from '../models/grid.model';
import { ScrollableScrollingService } from '../useCases/scrollableScrolling.service';
import { VirtualScrollingService } from '../useCases/virtualScrolling.service';

export interface IScrollingMode {
    // The following is used only by the virtual scrolling service
    rowHeight: number;
    get pageSize(): number;
    get totalCount(): number;

    loadData(kData: KendoServerResult): GridDataResult;
    isVirtual(): this is VirtualScrollingService;

}

class ScrollingFactoryOpts {
    mode: ScrollingMode;
    componentContext: GiasKendoGridComponent;
}

export class ScrollingModeFactory {
    public static Create(opts: ScrollingFactoryOpts) {
        switch (opts.mode) {
            case ScrollingMode.Scrollable:
                return new ScrollableScrollingService({
                    pagination: opts.componentContext.pagination
                });
            case ScrollingMode.Virtual:
                return new VirtualScrollingService({
                    pagination: opts.componentContext.pagination,
                    generalSettings: opts.componentContext.generalSettings,
                    gridPrivate: opts.componentContext.privateService
                });
        }
        return null;
    }
}

