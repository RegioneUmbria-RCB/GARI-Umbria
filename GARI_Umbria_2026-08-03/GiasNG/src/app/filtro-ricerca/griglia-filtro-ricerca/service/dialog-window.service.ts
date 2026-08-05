import { Injectable } from "@angular/core";
import { TranslocoService } from "@jsverse/transloco";
import { Location } from '@angular/common';
import { DialogResult } from "@progress/kendo-angular-dialog";
import { DialogBooleanResult, Dialog_Type, GiasDialogService } from "app/Service/gias-dialog.service";
import { Router } from "@angular/router";

@Injectable()
export class DialogWindowService {

    constructor(
                private transloco: TranslocoService,
                private location: Location,
                private router: Router,
                private dialogService: GiasDialogService
    ) {}

    async waitDialogResult() {

        let msg = this.transloco.translate("ConfermaEsciSenzaSalvareLeModifiche", { });

        let dialogResponse = await this.createDialogWindow(
          '', msg
        );

        if (dialogResponse['returnObj']) {
          return true;
        }

        this.location.go(this.router.url);
        return false;

    }

    createDialogWindow(title: string, content: string, preventAction: (p: DialogBooleanResult) => boolean = ()=>{return false}): Promise<DialogResult> {
        return new Promise((resolve, reject) => {
            let dialog = this.dialogService.dialogMessageRef(
                title,
                content,
                [
                    { text: this.transloco.translate('Conferma'), primary: true, returnObj: true },
                    { text: this.transloco.translate('Annulla'), returnObj: false }
                ], 'auto', 'auto',
                preventAction,
                Dialog_Type.info
            );

            if(typeof content == 'string')
                dialog.content.location.nativeElement.style.whiteSpace = 'pre-line';
            dialog.result.subscribe(res => resolve(res))
        })
    }

    async infoDialog(msg: string) {

        let dialogResponse = await this.createDialogWindowInfo(
          '', msg
        );

        if (dialogResponse['returnObj']) {
          return true;
        }

    }

    createDialogWindowInfo(title: string, content: string, preventAction: (p: DialogBooleanResult) => boolean = ()=>{return false}): Promise<DialogResult> {
        return new Promise((resolve, reject) => {
            let dialog = this.dialogService.dialogMessageRef(
                title,
                content,
                [
                    { text: this.transloco.translate('Ok'), primary: true, returnObj: true }
                ], 'auto', 'auto',
                preventAction,
                Dialog_Type.info
            );

            if(typeof content == 'string')
                dialog.content.location.nativeElement.style.whiteSpace = 'pre-line';
            dialog.result.subscribe(res => resolve(res))
        })
    }

}
