import { Component, Inject, ViewChild } from "@angular/core";
import { GRID_HTTP_TOKEN, generateGridProviders } from "gias-kendo-grid";
import { NumericTextBoxComponent } from "@progress/kendo-angular-inputs";
import { GridCapiAnimaliService } from "./service/grid-capi-animali.service";
import { enum_TipoPrescrizione } from "app/zoo/models/tipo-prescrizione.enum";

@Component({
    standalone: false,
    selector: 'app-capi-animali',
    templateUrl: './grid-capi-animali.component.html',
    styleUrls: ['./grid-capi-animali.component.scss'],
    providers: [
        ...generateGridProviders(
            GridCapiAnimaliService,
            GridCapiAnimaliComponent
        )
    ]
})
export class GridCapiAnimaliComponent {

    @ViewChild('totaleFarmacoDaSomministrare') totaleFarmacoDaSomministrare!: NumericTextBoxComponent;

    protected qtaApplied = true;
    public numericFormat: string = '#.0000';
    selectedAnimalsCount: number = 0;

    constructor(
        @Inject(GRID_HTTP_TOKEN) protected gridService: GridCapiAnimaliService
    ) {
        this.gridService.calculateNumericFormat(this.gridService.trattamentoFormService.dettagliProtocollo.udmDose);

        this.gridService.gridpublicService
            .subscribe(grid => {
                if (grid?.data)
                    this.selectedAnimalsCount = (!this.gridService.trattamentoFormService.isInfoMode)
                        ? (grid.data?.rows.filter((row: any) => row.Selected).length ?? 0)
                        : (grid.data?.rows.length ?? 0);
        });
    }

    public get gridSelectedRows(): Array<any> {
        return this.gridService.gridpublicService.
            getValue()?.data?.rows?.
            filter((row: any) => row.Selected) ?? [];
    }

    public get isTotQtaDisabled(): boolean {
        return this.gridService.trattamentoFormService.isInfoMode
            || this.gridSelectedRows.length == 0
            // || this.gridService.trattamentoFormService.checkIsVeterinaryOrIndTerapeuticPrescription()
            // || !this.gridService.isTotEditable
        ;
    }

    onKeyDown(event: KeyboardEvent): void {
        if (event.key === 'Enter') {
            event.preventDefault();
            // this.gridService.modificaTotEDistribuisci();
        }
    }

    onBlur(): void {
        if (!this.qtaApplied) {
            this.gridService.modificaTotEDistribuisci();
            this.qtaApplied = true;
        }
    }

    onClickModificaTotEDistribuisci() {
        this.gridService.isTotEditable = true;
        setTimeout(() => this.totaleFarmacoDaSomministrare.focus(), 0);
    }

    onCellClick(event: any) {
        if (this.checkIfCellIsNotEditable(event))
            event.sender.closeRow(event.rowIndex);
    }

    checkIfCellIsNotEditable(event: any): boolean {
        return !event.dataItem.Selected
            // || this.gridService.trattamentoFormService.withDiffAICs
            || this.gridService.trattamentoFormService.checkIfIsEditingFromVeterinaryPrescripton();
    }

    /**
     * Conferma l'eventuale modifica IN_CELL attiva: prima esegue il blur sull'input
     * focalizzato (in modo che il ControlValueAccessor del NumericTextBox propaghi
     * il valore digitato al FormControl Angular), poi chiude programmaticamente la
     * cella della Kendo Grid. Questo garantisce che i dati della riga siano
     * aggiornati prima che un salvataggio esterno li legga.
     */
    public commitCurrentEdit(): void {
        const activeEl = document.activeElement as HTMLElement;
        if (activeEl instanceof HTMLInputElement) {
            activeEl.blur();
        }
        this.gridService.gridpublicService.gridComp?.closeCell();
    }

}