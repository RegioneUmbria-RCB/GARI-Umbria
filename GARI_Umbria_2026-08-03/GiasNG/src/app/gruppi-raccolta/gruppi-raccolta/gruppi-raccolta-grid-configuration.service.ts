import {Injectable, Injector} from '@angular/core';
import {AbstractGridConfigService, HttpAction} from 'gias-kendo-grid';
import {GruppiRaccoltaKendoServerResult, KendoGruppiRaccoltaModel} from './gruppi-raccolta.model';
import { EditingMode, KendoGridColumn, LoaderType, ModelEntry} from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import {forkJoin, map, Observable} from 'rxjs';
import {ConfigTemplate} from 'gias-kendo-grid';
import {AGRODATAFINE, AGRODATAINIZIO, SMARTPHONE_WIDTH} from '../../Model/CostantiPersonalizzate';
import {TranslocoService} from '@jsverse/transloco';
import {
    AgrSelectableSettings,
    CommandsColumnSettings,
    ToolbarSettings
} from 'gias-kendo-grid';
import {ObjParametriAgendaService} from '../../Service/obj-parametri-agenda.service';
import {GruppiRaccoltaGridEventsService} from './gruppi-raccolta-grid-events.service';
import {GruppiRaccoltaService} from '../../Service/GruppiRaccolta/gruppi-raccolta.service';
import {MasterService} from '../../Service/master.service';
import {GruppoRaccolta} from '../../Model/metaschema/GruppoRaccolta';
import {IntervalloTemporale} from '../../Model/anagrafiche/IntervalloTemporale';
import {AbstractControl, Validators} from '@angular/forms';
import { ObjParametriAgenda } from 'gias-ui-kit';

export function startdateValidator(control: AbstractControl) {
    if (control.value > control.parent?.controls['Validita_Fine'].value) {
        return { 'startDate': true };
    }
    return null;
}

export function endDateValidator(control: AbstractControl) {
    if (control.value < control.parent?.controls['Validita_Inizio'].value) {
        return { 'endDate': true };
    }
    return null;
}

@Injectable()
export class GruppiRaccoltaGridConfigurationService extends AbstractGridConfigService<GruppiRaccoltaKendoServerResult> {

    private objParametriAgenda: ObjParametriAgenda;

    constructor(
        injector: Injector,
        private translocoService: TranslocoService,
        private objParametriAgendaService: ObjParametriAgendaService,
        private gruppiRaccoltaGridEventsService: GruppiRaccoltaGridEventsService,
        private gruppiRaccoltaServivce: GruppiRaccoltaService,
        private masterService: MasterService
    ) {
        super(injector, ConfigTemplate.DefaultTemplate);

        this.gruppiRaccoltaGridEventsService.loadingService = this.loadingService;
        this.gruppiRaccoltaGridEventsService.component = this.gridPublicService.gridElRef;
        this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();

        this.handleCustomizations();
    }

    editingMode: EditingMode = EditingMode.IN_LINE;
    gridId: string = 'gruppi-raccolta';
    loader: LoaderType = LoaderType.SERVICE;
    rowId: string = 'GruppoRaccolta_Cod';

    private readonly gridColumns: Array<KendoGridColumn> = [
        new KendoGridColumn(
            {field: 'GruppoRaccolta_Des', title: this.translocoService.translate('Descrizione')},
            {resizable: true, editable: true, width: 135, validators: [Validators.required]}
        ),
        new KendoGridColumn(
            {field: 'Validita_Inizio', title: this.translocoService.translate('Validita_Inizio')},
            {
                resizable: true,
                editable: true,
                date: {defaultValue: AGRODATAINIZIO},
                media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
                width: 135,
                validators: [startdateValidator]
            }
        ),
        new KendoGridColumn(
            {field: 'Validita_Fine', title: this.translocoService.translate('Validita_Fine')},
            {
                resizable: true,
                editable: true,
                date: {defaultValue: AGRODATAFINE},
                media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',
                width: 135,
                validators: [endDateValidator]
            }
        ),
    ];

    private readonly gridModel: KendoGruppiRaccoltaModel = {
        GruppoRaccolta_Cod: new ModelEntry(CELL_TYPES.NUMBER, false),
        GruppoRaccolta_Des: new ModelEntry(CELL_TYPES.STRING, false),
        Validita_Inizio: new ModelEntry(CELL_TYPES.DATE, false),
        Validita_Fine: new ModelEntry(CELL_TYPES.DATE, false),
        Data_Creazione: new ModelEntry(CELL_TYPES.DATE, false),
        Utente_Creazione: new ModelEntry(CELL_TYPES.STRING, false),
        Data_Modifica: new ModelEntry(CELL_TYPES.DATE, false),
        Utente_Modifica: new ModelEntry(CELL_TYPES.STRING, false)
    }

    perform(actionType: HttpAction, item: any): Observable<any[]> {
        const gr: GruppoRaccolta = this.prepareGruppoRaccolta(item);
        if(actionType === HttpAction.UPDATE) {
            return this.gruppiRaccoltaGridEventsService.updateGruppoRaccolta(gr);
        } else if(actionType === HttpAction.CREATE) {
            return this.gruppiRaccoltaGridEventsService.writeGruppoRaccolta(gr);
        } else if(actionType === HttpAction.REMOVE) {
            return this.gruppiRaccoltaGridEventsService.deleteGruppoRaccolta(gr);
        }
    }

    read(options?: any): Observable<GruppiRaccoltaKendoServerResult> {
        console.log('read gruppi raccolta');
        this.loadingService.set_isLoading({ isLoading: true, message: '', component: this.gridPublicService.gridElRef });
        this.gruppiRaccoltaGridEventsService.component = this.gridPublicService.gridElRef;

        let objParams: ObjParametriAgenda = this.objParametriAgendaService.getObjParamValue();
        objParams.Data = new Date();

        return forkJoin([this.gruppiRaccoltaServivce.leggiGruppiRaccolta(objParams)]).pipe(
            map((res) => {

                let rows = res[0].flatMap(gr => {
                    return {
                        GruppoRaccolta_Cod: gr.codice,
                        GruppoRaccolta_Des: gr.descrizione,
                        Validita_Inizio: gr.validita.inizio,
                        Validita_Fine: gr.validita.fine
                    }
                });

                const tableData: GruppiRaccoltaKendoServerResult = new GruppiRaccoltaKendoServerResult(this.gridModel,
                    this.gridColumns,
                    rows
                );

                this.loadingService.set_isLoading({isLoading: false, message: '', component: this.gridPublicService.gridElRef});
                return tableData;
            })
        );
    }

    handleCustomizations(): void {
        this.selectable = new AgrSelectableSettings();
        this.selectable.selectable.checkboxOnly = true;
        this.selectable.selectable.enabled = true;
        this.selectable.shouldShowCheckbox = true;

        this.toolbar = new ToolbarSettings();
        this.toolbar.newItem = true;
        this.toolbar.resetChanges = false;

        this.cmdColumn = new CommandsColumnSettings({
            editBtn: true,
            removeBtn: true,
        });

        this.resizable.autoFitColumns = false;
        this.resizable.isResizable = true;

        this.groups.groupable.enabled = false;

        if (window.innerWidth < SMARTPHONE_WIDTH) {
            this.toolbar.newItem = false;
            this.cmdColumn.editBtn = false;
            this.groups.groupable.enabled = false;
            this.views.enabled = false;
        }
    }

    private prepareGruppoRaccolta(item: any): GruppoRaccolta {
        let grupporaccolta: GruppoRaccolta = new GruppoRaccolta(item.GruppoRaccolta_Cod ? item.GruppoRaccolta_Cod : 0);
        grupporaccolta.descrizione = item.GruppoRaccolta_Des;
        grupporaccolta.validita = new IntervalloTemporale(item.Validita_Inizio, item.Validita_Fine);
        return grupporaccolta;
    }

}
