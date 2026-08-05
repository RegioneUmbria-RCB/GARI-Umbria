import { Injectable, Injector, OnDestroy } from "@angular/core";
import { AbstractGridConfigService, CommandsColumnSettings, CommandsDropDownSettings, ConfigTemplate, DropdownListWithForm, EditingMode, GiasKendoGridComponent, GiasMessageService, GridPublicService, HttpAction, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, SelectableSettings } from "gias-kendo-grid";
import { catchError, map, merge, Observable, of, Subject, switchMap, takeUntil } from "rxjs";
import { ProtocolloxIntervento, TerapiaZooFormService } from "../terapia-zoo.service";
import { PrescrizioniClient } from "app/Service/net-core6-api.service";
import { AGRODATAFINE, AGRODATAINIZIO, ConversionService, DropdownListItem } from "gias-ui-kit";
import { ProtocolsGridConfigService } from "./protocols-grid-config.service";
import { CellClickEvent, CellCloseEvent, SelectionEvent } from "@progress/kendo-angular-grid";
import { enum_UdM_Dose } from "app/zoo/models/tipi-enumerativi-zoo";
import { enum_UnitaMisura } from "app/Model/TipiEnumerativi";
import { LeggiPrescrizioni } from "app/zoo/models/leggi-prescrizioni.model";
import { IntervalloTemporale } from "app/Model/anagrafiche/IntervalloTemporale";
import { enum_TipoGrigliaTrattamentiZoo, enum_TipoPrescrizione } from "app/zoo/models/tipo-prescrizione.enum";
import { FormArray, FormGroup } from "@angular/forms";
import { DropDownButton } from "@progress/kendo-angular-buttons";

export class ProtocolsGridServerResult extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

@Injectable()
export class ProtocolsGridService
    extends AbstractGridConfigService<ProtocolsGridServerResult>
    implements OnDestroy {

    editingMode: EditingMode = EditingMode.IN_CELL;
    isEditing: boolean = true;

    loader: LoaderType = LoaderType.SERVICE;

    rowId: string = 'IdRicetta';
    gridId: string = 'ProtocolsGrid';

    signal: Subject<void> = new Subject();

    private isDataLoaded: boolean = false;

    public numericFormat: string;
    public isAbortingEdit: boolean = false;

    public currentProtocolsFormArray: FormArray;

    public Array_AllProtocols: Array<any> = [];

    private getLoadParams(): LeggiPrescrizioni {
        let formValues = this.terapiaFormService.formTerapiaZoo.getRawValue();
        return {
            Piva: this.terapiaFormService.objP_Agenda.Piva,
            Sa_Cod: formValues?.CentroAziendale?.codice ?? 0,
            Sta_Num: formValues?.Stalla?.Sta_Num ?? 0,
            validita: new IntervalloTemporale(AGRODATAINIZIO, AGRODATAFINE),
            Tipo_Cod: enum_TipoPrescrizione.Protocollo_Terapeutico,
            Tipo_Griglia: enum_TipoGrigliaTrattamentiZoo.Protocolli
        } as LeggiPrescrizioni;
    }

    private getObservableForRows(): Observable<any[]> {
        let obs: Observable<any[]>;
        let param = this.getLoadParams();
        obs = this.prescriptions.prescrizioniLeggiPrescrizioni(param)
            .pipe(
                switchMap((resp) => {
                    if (resp.RispostaOK)
                        return of(JSON.parse(resp.RispostaStringa));
                    else
                        return of([]);
                }),
                switchMap((r) => of(this.conversionService.ConversionDateInObject(r)))
            );

        return obs;
    }

    constructor(
        injector: Injector,
        public gridpublicService: GridPublicService,
        public terapiaFormService: TerapiaZooFormService,
        private giasMessageService: GiasMessageService,
        private protocolsGridConfigService: ProtocolsGridConfigService,
        private prescriptions: PrescrizioniClient,
        private conversionService: ConversionService
    ) {
        super(injector, ConfigTemplate.DefaultTemplate);

        this.setProtAltDDLConfig();

        this.resizable.autoFitColumns = true;

        this.cmdColumn = new CommandsColumnSettings({
            editBtn: false,
            infoBtn: false,
            removeBtn: false,
            onDisableInfoBtn: () => false,
            width: 10
        });
        this.cmdDropDown = new CommandsDropDownSettings({
            removeBtn: false, infoBtn: false
        });

        if (!this.terapiaFormService.isInfoMode) {
            this.selectable.selectable = new SelectableSettings({
                checkboxOnly: true,
                enabled: true
            });
            this.selectable.preselectedRows.selectionChangeFn = this.selectionChangeFn;
            this.selectable.columnSettings.showSelectAll = (!this.terapiaFormService.isUpdateMode);
            this.selectable.shouldShowCheckbox = true;
            this.selectable.columnSettings.title = ' ';
        }

        this.groups.groupable.enabled = false;

        this.onCellClick = (event: CellClickEvent) => {
            const isProtAltCell = event.columnIndex > 0 && event.column.field === 'Prot_Alt';
            const isRowNotSelected = !event.dataItem.Selected;

            if (isProtAltCell && isRowNotSelected) event.sender.closeRow(event.rowIndex);
        };

        this.onCellClose = (event: CellCloseEvent) => {
            const protocolsArray = this.currentProtocolsFormArray;
            if (!protocolsArray) return;

            const gridRow = event.dataItem;
            const isProtAltDDLColumn = event.column.field === 'Prot_Alt';

            if (isProtAltDDLColumn && gridRow.Selected) {
                const newProtAltId = event.dataItem.Prot_Alt;
                const currentProtocolId = gridRow.IdRicetta;

                this.updateProtAlt(currentProtocolId, newProtAltId);
                event.dataItem.Prot_Alt_Des = this.getProtAltDisplayName(newProtAltId);
                event.sender.closeCell();
                return;
            }
        };

        this.terapiaFormService.ddlSelectionSubject
        .pipe(
            takeUntil(this.signal)
        )
        .subscribe(() => this.gridpublicService.refresh(true));
    }

    public selectionChangeFn = (event: SelectionEvent, component: GiasKendoGridComponent) => {
        let refreshGrid = false;

        const protocolsArray = this.currentProtocolsFormArray;
        if (!protocolsArray) return;

        event.selectedRows.forEach(row => {
            row.dataItem['selezionato'] = 'S';

            const dataItem = row.dataItem;
            const idProt = dataItem.IdRicetta;
            const isAlreadyInForm = protocolsArray.controls.some(c => c.value.Id === idProt);

            if (!isAlreadyInForm) {
                const newProtocolData: ProtocolloxIntervento = {
                    Id: idProt,
                    Prot_Alt: dataItem.Prot_Alt
                };
                const protocolFormGroup = this.createProtocolloFormGroup(newProtocolData);
                protocolsArray.push(protocolFormGroup);
            }
            refreshGrid = false;
        });

        event.deselectedRows.forEach(row => {
            row.dataItem['selezionato'] = 'N';

            const idToRemove = row.dataItem.IdRicetta;

            this.gridPublicService.getValue().data.rows
                .filter((r: any) => r.IdRicetta === idToRemove)
                .forEach((r: any) => {
                    r.Prot_Alt = null;
                    r.Prot_Alt_Des = '';
            });

            const indexToRemove = protocolsArray.controls.findIndex(c => c.value.Id === idToRemove);
            if (indexToRemove !== -1) protocolsArray.removeAt(indexToRemove);

            refreshGrid = true;
        });

        this.gridPublicService.refresh(refreshGrid);
    };

    calculateNumericFormat(UnitaDiMisura: string | number): void {
        if (UnitaDiMisura == null || UnitaDiMisura == undefined) return;

        this.numericFormat = '#0.00';
        let udm: string = '';

        if (typeof UnitaDiMisura === 'number') {
            switch(UnitaDiMisura) {
                    //case enum_UdM_Dose.mg_su_Kg:
                    case enum_UnitaMisura.Milligrammi:
                        udm = 'mg';
                    break;
                    case enum_UdM_Dose.ml_su_100Kg:
                    case enum_UdM_Dose.ml_su_Capo:
                    case enum_UnitaMisura.Millilitri:
                        udm = 'ml';
                    break;
                    case enum_UdM_Dose.g_su_100Kg:
                        udm = 'g';
                    break;
                    default:
                        udm = '';
            }
        } else {
            udm = UnitaDiMisura;
        }

        this.numericFormat += ' ' + udm;
    }

    read(options?: any): Observable<ProtocolsGridServerResult> {
        this.loadingService.set_isLoading({ isLoading: true, component: this.gridPublicService.gridElRef });

        const protocolsInForm: Array<KendoGridRow> = (this.currentProtocolsFormArray && this.currentProtocolsFormArray.length > 0)
            ? this.currentProtocolsFormArray.controls.map(c => c.value as KendoGridRow)
            : [];

        return this.getObservableForRows()
                .pipe(
                    catchError(() => {
                        this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });
                        return of([]);
                    }),
                    map((responseRows: any[]) => {
                        this.loadingService.set_isLoading({ isLoading: false, component: this.gridPublicService.gridElRef });

                        this.Array_AllProtocols.splice(0, this.Array_AllProtocols.length);
                        const apiProtocols = responseRows.map(p => ({
                            Id: p.IdRicetta,
                            Display: `${p.Denominazione} (${p.Numero})`
                        }));
                        this.Array_AllProtocols.push(...apiProtocols);

                        let mappedRows: Array<KendoGridRow> = responseRows.map((itemOfRows, index) => {
                            const existingProtocol = protocolsInForm.find(p => p['Id'] === itemOfRows['IdRicetta']);

                            let row = { ...itemOfRows };

                            if (existingProtocol) {
                                Object.assign(row, existingProtocol);
                                row['Selected'] = true;
                                row['selezionato'] = 'S';

                                if (row['Prot_Alt']) {
                                    row['Prot_Alt_Des'] = this.getProtAltDisplayName(row['Prot_Alt']);
                                }
                            } else {
                                row['Selected'] = false;
                                row['selezionato'] = 'N';
                            }
                            row['IdProtocollo'] = index + 1;
                            return row;
                        });

                        let finalRows: Array<KendoGridRow> = mappedRows;
                        if (this.terapiaFormService.isInfoMode) {
                            finalRows = mappedRows.filter(row => row['Selected']);
                        }

                        if (responseRows.length > 0) this.isDataLoaded = true;
                        return new ProtocolsGridServerResult(
                            finalRows,
                            this.protocolsGridConfigService.kendoCols,
                            this.protocolsGridConfigService.kendoModelEntries
                        );
                    })
                );
    }

    ngOnDestroy(): void {
        this.signal.next();
        this.signal.complete();
    }

    perform(actionType: HttpAction, items: any, oldRow?: any): Observable<any> {
        throw new Error("Method not implemented.");
    }

    public createProtocolloFormGroup(protocol?: ProtocolloxIntervento): FormGroup {
        const mask = protocol || new ProtocolloxIntervento(0, 0);
        return this.terapiaFormService.fb.group({
            Id: [mask.Id],
            Prot_Alt: [mask.Prot_Alt]
        });
    }

    public setProtAltDDLConfig() {
        const protAltCol = this.protocolsGridConfigService.kendoCols.find(c => c.field === 'Prot_Alt');
        protAltCol.editable = (!this.terapiaFormService.isInfoMode);
        protAltCol.ddl = new DropdownListWithForm('id', 'Prot_Alt', 'name', []);
        protAltCol.ddl.descriptionField = 'Prot_Alt_Des';
        protAltCol.ddl.loadOnEdit = true;
        protAltCol.ddl.loadFunction = this.loadAlternativeProtocols.bind(this);
    }

    private loadAlternativeProtocols(dataItem: any): Observable<DropdownListItem[]> {
        const currentProtocolId = dataItem.IdRicetta;
        const selectedIds = this.currentProtocolsFormArray
            ? this.currentProtocolsFormArray.controls.map(c => c.value.Id)
            : [];

        const items: DropdownListItem[] = this.Array_AllProtocols
            .filter(p => selectedIds.includes(p.Id) && p.Id !== currentProtocolId)
            .map(p => ({
                id: p.Id,
                name: p.Display
            } as DropdownListItem));
        return of(items);
    }

    public getProtAltDisplayName(protAltId: number | null | undefined): string | null {
        if (protAltId === null || protAltId === undefined) return null;
        const protocol = this.Array_AllProtocols.find(p => p.Id === protAltId);
        return protocol ? protocol.Display : null;
    }

    public updateProtAlt(protocolId: number, newProtAltId: number) {
        const protocolsArray = this.currentProtocolsFormArray;
        if (!protocolsArray) return;

        const formIndex = protocolsArray.controls.findIndex(c => c.value.Id === protocolId);
        if (formIndex !== -1) {
            const formGroup = protocolsArray.at(formIndex) as FormGroup;
            const selectedProtocol = this.Array_AllProtocols.find(p => p.Id === newProtAltId);
            const displayName = selectedProtocol ? selectedProtocol.Display : null;

            formGroup.patchValue({
                Prot_Alt: newProtAltId,
                Prot_Alt_Des: displayName
            });
            this.gridPublicService.refresh(false);
        }
    }

}