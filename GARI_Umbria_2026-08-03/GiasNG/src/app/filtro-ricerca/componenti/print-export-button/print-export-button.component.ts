import { Component, effect, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { faFileExcel } from "@fortawesome/free-solid-svg-icons";
import { TranslocoService } from "@jsverse/transloco";
import { Enum_TipoComportamento_FiltroRicerca, enum_CodificaStampe } from "app/Model/TipiEnumerativi";
import { Enum_SiteRedirector } from "app/Model/siti.enum";
import { GestioneRichiesteService, KeyValuePair, ParametriAggiuntivi_QueryString } from "app/Service/gestione-richieste.service";
import { GiasIFrameWindowService } from "gias-ui-kit";
import { DialogWindowService } from "app/filtro-ricerca/griglia-filtro-ricerca/service/dialog-window.service";
import { FiltroRicercaService, ProseguiSelezionati, SignalOverflowRows } from "app/filtro-ricerca/griglia-filtro-ricerca/service/filtro-ricerca.service";

enum enum_ActionButton {
    toPrint = 1,
    toExportExcel = 2,
    toOthers = 3,
    toExportPDF = 4
}

@Component({
    standalone: false,
    selector: 'print-export-button',
    templateUrl: './print-export-button.component.html',
    styleUrls: ['./print-export-button.component.scss']
})
export class PrintExportComponent implements OnInit {

    title: string;
    faIcon;
    faExcel = faFileExcel;
    showPrint: boolean = false;
    overflowRows: SignalOverflowRows = new SignalOverflowRows();

    actionType: enum_ActionButton;

    @Output() onPressButton = new EventEmitter<boolean>();
    @Input() buttonType: Enum_TipoComportamento_FiltroRicerca;
    @Input() codificaStampa: enum_CodificaStampe;

    constructor(
        private translocoService: TranslocoService,
        private windowService: GiasIFrameWindowService,
        private service: FiltroRicercaService,
                private gestioneRichieste: GestioneRichiesteService,
                private dialogService: DialogWindowService) {

                    effect(() => {
                        this.overflowRows = this.service.overflowSelectTopRowsSignal();
                    });
                }

    ngOnInit() {
        switch (this.buttonType) {
            case Enum_TipoComportamento_FiltroRicerca.EsportaPdf:
                this.title = this.translocoService.translate("Stampa");
                this.actionType = enum_ActionButton.toPrint;
                this.showPrint = true;
            break;

            case Enum_TipoComportamento_FiltroRicerca.EsportaExcel:
                this.title = this.translocoService.translate("EsportaSuExcel");
                this.actionType = enum_ActionButton.toExportExcel;
                this.faIcon = this.faExcel;
                this.showPrint = false;
                break;

            case Enum_TipoComportamento_FiltroRicerca.SelezionamentoEntita:
                this.title = this.translocoService.translate("Prosegui");
                this.actionType = enum_ActionButton.toOthers;
                this.showPrint = false;
                break;
        }
    }

    onRedirectToStamps(richiesta) {
        let title: string;
        let impiantiStr = '';

        const parametri: ParametriAggiuntivi_QueryString[] = [
            KeyValuePair.Create("lav_cod", '0'),
            KeyValuePair.Create("specie", "-1"),
            KeyValuePair.Create("impianti", impiantiStr),
        ];
        // this.getSelectedRows();

        parametri.push(KeyValuePair.Create("tipo_operazione", 'Stampa-' + richiesta));
        //title = this.transloco.translate(event.actionName);

        if (!richiesta) return;

        this.gestioneRichieste.gestionePassaggioAltroSito(
            Enum_SiteRedirector.Sito_AgronicaStampe_2010,
            richiesta, parametri)
            .then(link => {
                this.windowService.open({
                    title: title,
                    content: link,
                    height: window.innerHeight * 0.9,
                    width: window.innerWidth * 0.9
                });
            });
    }

    checkIfSomethingSelected() {
        if (this.service.selectedRows.length > 0)
            return true;
        else
            return false;
    }

    printReports() {
        if (this.overflowRows.overflow)
            this.dialogService.infoDialog(this.translocoService.translate("NumeroRigheMaggioreDiQuelloImpostato", { n_rows: this.overflowRows.nRows }));
        else
            this.service.toPrintOrExport();
    }

}
