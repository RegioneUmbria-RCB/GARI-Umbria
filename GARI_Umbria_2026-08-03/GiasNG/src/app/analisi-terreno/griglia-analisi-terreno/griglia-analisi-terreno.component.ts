import { Component, Inject, ViewChild, ViewEncapsulation } from "@angular/core";
import { generateGridProviders } from 'gias-kendo-grid';
import { GridAnalisiTerrenoHttpService } from "./service/grid-analisi-terreno-http.service";
import { GRID_HTTP_TOKEN } from 'gias-kendo-grid';
import { GridPublicService } from 'gias-kendo-grid';
import { GiasDialogService } from "app/Service/gias-dialog.service";
import { TranslocoService } from "@jsverse/transloco";
import { GiasKendoGridComponent } from 'gias-kendo-grid';
import { DialogCloseResult, DialogResult } from "@progress/kendo-angular-dialog";
import { HttpAction } from 'gias-kendo-grid';
import { ActivatedRoute, Router } from "@angular/router";
import { enum_AnalisiTerrenoGridCommands } from "../utils";

@Component({
    standalone: false,
    selector: 'app-griglia-analisi-terreno',
    templateUrl: './griglia-analisi-terreno.component.html',
    styleUrls: ['./griglia-analisi-terreno.component.scss'],
    providers: [...generateGridProviders(GridAnalisiTerrenoHttpService,
                                        GrigliaAnalisiTerrenoComponent)],
    encapsulation: ViewEncapsulation.None
})
export class GrigliaAnalisiTerrenoComponent {

    @ViewChild(GiasKendoGridComponent) gridChild: GiasKendoGridComponent;

    constructor(@Inject(GRID_HTTP_TOKEN) public gridAnalisiTerrenoHttpService: GridAnalisiTerrenoHttpService,
                                         private gridpublicService: GridPublicService,
                                         public dialogService: GiasDialogService,
                                         public transloco: TranslocoService,
                                         private route: ActivatedRoute,
                                         private router: Router) {}

    get selected() {
        return this.gridAnalisiTerrenoHttpService.selectedRows;
    }

    set selected(arrCheck: any[]) {
        this.gridAnalisiTerrenoHttpService.selectedRows = arrCheck;
    }

    public async removeSelected() {
        if (!this.selected.length) {
            this.error('SelezionareAlmenoUnOperazione');
            return;
        }

        let prompt= this.selected.length > 1
                    ? this.transloco.translate('MultipleDeletionConfirmation', [this.selected.length])
                    : this.transloco.translate('SoleDeletionConfirmation');

        this.dialogService.dialogMessageObs_Result(
            this.transloco.translate('ActivityDeletion'), prompt
        ).GiasSubscribe((R: DialogResult) => {
            if (R instanceof DialogCloseResult) return;
            if (R['returnObj']) {
                this.gridAnalisiTerrenoHttpService.perform(HttpAction.REMOVE, this.selected.map(item => item.dataItem))
                    .GiasSubscribe(r => {
                        if (r == '') {
                            this.selected = [];
                            this.gridAnalisiTerrenoHttpService.gridpublicService.refresh(true);
                        }
                    });
            }
        })
    }

    onNuovo() {
        this.gridAnalisiTerrenoHttpService.goToEditPage(null, enum_AnalisiTerrenoGridCommands.NUOVA_ANALISI);
    }

    private error(content: string) {
        this.dialogService.baseError( 'Errore', content );
    }
}
