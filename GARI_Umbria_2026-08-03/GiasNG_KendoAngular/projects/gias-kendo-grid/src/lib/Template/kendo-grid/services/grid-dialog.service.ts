import { Inject, Injectable } from "@angular/core";
import { TranslocoService } from "@jsverse/transloco";
import { DialogCloseResult, DialogService } from "@progress/kendo-angular-dialog";
import { lastValueFrom } from 'rxjs';
import { DeletionMode, RemoveMultipleRowsParams } from "../models/configuration.model";
import { GRID_HTTP_TOKEN, KendoGridRow, KendoServerResult } from "../models/grid.model";
import { AbstractGridConfigService } from "./grid-config.service";
import { GridPublicService } from "./grid-public.service";
import { GiasDialogService } from "gias-ui-kit";

@Injectable({ providedIn: "root" })
export class GridDialogService extends GiasDialogService {
  ts: TranslocoService;
  constructor(private gridPublicService: GridPublicService,
    kendoDialogService: DialogService,
    @Inject(GRID_HTTP_TOKEN)
    public conf: AbstractGridConfigService<KendoServerResult>,
    translocoService: TranslocoService) {
    super(kendoDialogService, translocoService);
    this.ts = translocoService;
  }

  public async showDialog(callback: (row) => void,
    row: KendoGridRow) {

    let msg = await this.conf.getRemoveMultipleRowsMessage(this.getSelectedRows(row));
    if (msg != '' && msg != undefined) {
      const risposta = await lastValueFrom(this.dialogMessageObs_Result(
        '',
        msg,
        [
          { text: this.ts.translate('giasgrid.Conferma'), primary: true, returnObj: true },
          { text: this.ts.translate('giasgrid.Annulla'), returnObj: false }
        ],
        undefined,
        undefined,
        e => e instanceof DialogCloseResult, 3
      ));
      if (risposta['returnObj'])
        callback(row);
    } else {
      callback(row);
    }

  }

  private getSelectedRows(row: KendoGridRow): RemoveMultipleRowsParams {
    let itemsToRemove = new RemoveMultipleRowsParams();
    itemsToRemove.data = new Array<KendoGridRow>();

    if (this.conf.behavior.deletionMode === DeletionMode.HandleSingleRowDeletionOnly) {
      itemsToRemove.data.push(row);
      return itemsToRemove;
    }

    this.gridPublicService.value.data.rows.forEach(row => {
      if (row['Selected']) {
        itemsToRemove.data.push(row);
      }
    });

    if (itemsToRemove.data.find((r) => r['chiave'] == row['chiave']) == undefined) {
      itemsToRemove.data.push(row);
    }

    return itemsToRemove;
  }

}
