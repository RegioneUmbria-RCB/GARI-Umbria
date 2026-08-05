import { Injectable } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { GridPublicService } from 'gias-kendo-grid';
import { IQueryParamsService } from 'gias-kendo-grid';
import { Subject } from 'rxjs';
import { take, takeUntil } from 'rxjs/operators';


@Injectable()
export class TreeGridService implements IQueryParamsService {
    pubSrviceCtx: GridPublicService;
    signal: Subject<void>;

    constructor(private route: ActivatedRoute) { }

    public execute(gridPub$: GridPublicService) {
        this.pubSrviceCtx = gridPub$;


        this.manageTreeSelection();

    }

    init(signal: Subject<void>) {
        this.signal = signal;
    }

    public gridSelection(ids: string[]) {
        this.pubSrviceCtx.selection.setSelected.next({ keys: ids, usePartialMatch: true, resetPreviousSelection: true });
    }

    private manageTreeSelection() {
    // Redirect to another page. Attempt to update using query params.
    // Automatic unsubscribe.

        this.route.queryParams.pipe(takeUntil(this.signal))
            .subscribe((qp: Params) => {
                const selected = qp['gridDataId'] as string;
                if(selected) {
                    this.pubSrviceCtx.selection.setSelected.next({
                        keys: [selected],
                        usePartialMatch: true,
                        resetPreviousSelection: true
                    });
                }
            });
    }



}
