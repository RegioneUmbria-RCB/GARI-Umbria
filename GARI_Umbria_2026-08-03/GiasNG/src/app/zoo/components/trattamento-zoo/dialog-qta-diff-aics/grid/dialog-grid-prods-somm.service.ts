import { cloneDeep } from "lodash";
import { BehaviorSubject, Observable, of, Subject } from "rxjs";
import { Injectable, Injector, OnDestroy } from "@angular/core";
import { DialogGridProdsSommConfigService } from "./dialog-grid-prods-somm-config.service";
import { AbstractGridConfigService, CommandsColumnSettings, ConfigTemplate, EditingMode, GridPublicService, HttpAction, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, RendererGridEvent } from "gias-kendo-grid";

export interface DialogValidation {
  isValid: boolean;
  messageKey: string;
  messageParams?: any;
}

export interface ProdottoDistribuito {
  // Eredita tutti i campi da 'prodotti'
  [key: string]: any;
  // Aggiunge il campo chiave
  QtaToUse: number;
}

export class DialogGridProdsSommServerResult extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

@Injectable()
export class DialogGridProdsSommService
    extends AbstractGridConfigService<DialogGridProdsSommServerResult>
    implements OnDestroy {

    loader: LoaderType = LoaderType.SERVICE;
    editingMode: EditingMode = EditingMode.IN_CELL;

    rowId: string = 'Chiave';
    gridId: string = 'DialogSommProdsId';

    public reqQuantity: number = 0;
    public gridRows: ProdottoDistribuito[] = [];

    signal: Subject<void> = new Subject();

    proCod: number = 0;
    codiceAIC: string[] = [];

    private dataSubject = new BehaviorSubject<ProdottoDistribuito[]>([]);
    public data$: Observable<ProdottoDistribuito[]> = this.dataSubject.asObservable();

    constructor(
        injector: Injector,
        public gridPublicService: GridPublicService,
        private prodottoConfigService: DialogGridProdsSommConfigService
    ) {
        super(injector, ConfigTemplate.DefaultTemplate);

        this.resizable.autoFitColumns = true;
        this.groups.groupable.enabled = false;

        this.cmdColumn = new CommandsColumnSettings({
            editBtn: false,
            infoBtn: false,
            removeBtn: false
        });
        this.editingMode = EditingMode.IN_CELL;

        this.onCellClose = (args: any) => {
            // Check if the form is valid (e.g., didn't violate min/max)
            // and if it was actually changed ('dirty')
            if (args.formGroup && args.formGroup.valid && args.formGroup.dirty) {
                // 1. Get the new values from the form
                const updatedValues = args.formGroup.value;

                // 2. Find the row in our gridRows array
                const editedRow = this.gridRows.find(row => row[this.rowId] === args.dataItem[this.rowId]);

                if (editedRow) {
                    // 3. Update the values on that row
                    Object.assign(editedRow, updatedValues);

                    // 4. Push the *entire* updated array into the dataSubject
                    // This is what tells your dialog component to update!
                    this.dataSubject.next([...this.gridRows]);
                }
            }
        };

        this.setupGridConfig();

        this.data$.subscribe(rows => this.gridRows = rows);
    }

    private setupGridConfig(): void {
        this.columns = this.prodottoConfigService.kendoColums;
        this.model = this.prodottoConfigService.kendoModel;
    }

    read(options?: any): Observable<DialogGridProdsSommServerResult> {
        return of(new DialogGridProdsSommServerResult(
            this.gridRows,
            this.columns,
            this.model
        ));
    }

    perform(actionType: HttpAction, items: any, oldRow?: any): Observable<any> {
        if (actionType === HttpAction.UPDATE) {
            const updatedRow = items as ProdottoDistribuito;
            const index = this.gridRows.findIndex(row => row[this.rowId] === updatedRow[this.rowId]);

            if (index !== -1) {
                this.gridRows[index] = updatedRow;
                this.dataSubject.next(this.gridRows);
                return of(updatedRow);
            }
        }
        return of(null);
    }

    applyRendererRules(opts: RendererGridEvent): void {}

    onRowClass: any = () => null;

    public setInitialData(rows: KendoGridRow[], totalQty: number): void {
        this.reqQuantity = totalQty;
        this.gridRows = cloneDeep(rows).map((row: KendoGridRow, index: number) => ({
            ...row,
            Chiave: index,
            QtaToUse: 0
        }));

        let quantitaRimanente = Number(totalQty) || 0;
        const rowsByRecentExpiry = [...this.gridRows].sort((a, b) => {
            const aDate = this.getExpiryDate(a);
            const bDate = this.getExpiryDate(b);

            if (aDate && bDate) {
                return bDate.getTime() - aDate.getTime();
            }
            if (aDate && !bDate) {
                return -1;
            }
            if (!aDate && bDate) {
                return 1;
            }
            return (a.Lotto ?? '').toString().localeCompare((b.Lotto ?? '').toString());
        });

        for (const row of rowsByRecentExpiry) {
            if (quantitaRimanente <= 0) {
                break;
            }

            const disponibile = Number(row.Qta) || 0;
            const toUse = Math.min(disponibile, quantitaRimanente);
            row.QtaToUse = toUse;
            quantitaRimanente = Number((quantitaRimanente - toUse).toFixed(4));
        }

        this.dataSubject.next(this.gridRows);

        setTimeout(() => {
            this.gridPublicService.refresh();
        }, 0);
    }

    public validate(totalEditedQta, totalRequired): DialogValidation {
        // 1. Controlla il totale
        if (totalEditedQta !== totalRequired) {
            return {
                isValid: false,
                messageKey: 'zoo.LaQuantitaDistribuitaNonCorrisponde',
                messageParams: { required: totalRequired, actual: totalEditedQta }
            };
        }

        // 2. Controlla che QtaToUse non superi la Giacenza (Qta)
        const overStock = this.gridRows.find(item => (item.QtaToUse || 0) > item.Qta);
        if (overStock) {
            return {
                isValid: false,
                messageKey: 'zoo.LaQuantitaDaSomministrareEccedeLaGiacenza',
                messageParams: { lotto: overStock.Lotto }
            };
        }

        // 3. Controlla che sia stata usata almeno una riga
        if (totalEditedQta <= 0) {
            return {
                isValid: false,
                messageKey: 'zoo.LaQuantitaDaSomministrareNonPuòEssereZero',
            };
        }

        return { isValid: true, messageKey: null };
    }

    public getData(): ProdottoDistribuito[] {
        return this.gridRows.filter(item => item.QtaToUse > 0);
    }

    public updateRowsAndNotify(updatedRows: ProdottoDistribuito[]): void {
        this.gridRows = updatedRows;
        this.dataSubject.next(this.gridRows);
    }

    private getExpiryDate(row: ProdottoDistribuito): Date | null {
        const rawDate = row['DataScadenza']
            ?? row['Data_Scadenza']
            ?? row['dataScadenza']
            ?? row['Scadenza']
            ?? row['scadenza'];

        if (!rawDate) {
            return null;
        }

        const parsed = rawDate instanceof Date ? rawDate : new Date(rawDate);
        return isNaN(parsed.getTime()) ? null : parsed;
    }
}