import { Injectable, Injector } from "@angular/core";
import { AbstractGridConfigService, ConfigTemplate, EditingMode, HttpAction, KendoGridColumn, LoaderType, KendoGridModel, AgrSelectableSettings, ToolbarSettings, CommandsColumnSettings, ResizableSettings, DateSettings, DateTimeSettings, KendoGridRow, NumericSettings } from "gias-kendo-grid";
import { from, Observable, of } from "rxjs";
import { TurniIrriguaServerResult } from "../model/turni-irrigua.model";
import { TranslocoService } from "@jsverse/transloco";
import { AGRODATAINIZIO, AGRODATAFINE, CELL_TYPES, Dialog_Type, Enum_DBTypeOperation, GiasDialogService } from 'gias-ui-kit';
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { AbstractControl, ControlContainer, ValidatorFn } from "@angular/forms";
import { RateoTempo } from "app/Model/anagrafiche/ParcoMacchine";
import { RateiTempoService } from "./turni-irrigua.service";
import { CustomDateTimePickerComponent } from "../components/custom-datetimepicker.component";
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';

@Injectable()
export class TurniIrriguaGridService extends AbstractGridConfigService<TurniIrriguaServerResult> {
    gridId = 'TurniIrriguaGrid';
    editingMode: EditingMode = EditingMode.IN_LINE;
    loader: LoaderType = LoaderType.SERVICE;
    rowId = 'Rateo_Cod';

    private objParametriAgenda;
    private canWriteRateiIrrigui: boolean;
    private edit: boolean;

    private rateiTempo: any[] = [];

    constructor(
        injector: Injector,
        private objP: ObjParametriAgendaService,
        public translocoService: TranslocoService,
        private controlContainer: ControlContainer,
        private giasDialogService: GiasDialogService,
        private rateiTempoService: RateiTempoService,
        private permessiUtenteService: PermessiUtenteService
    ) {
        super(injector, ConfigTemplate.DefaultTemplate);
        this.objParametriAgenda = this.objP.getObjParamValue();
        this.canWriteRateiIrrigui = this.permessiUtenteService.canWritePermesso(enum_Security_Attivita.AnagraficaGestioneRateiIrriguiMacchine);
        this.edit = this.canWriteRateiIrrigui && this.objParametriAgenda.TipoOperazioneDB != Enum_DBTypeOperation.Read;
        this.handleCustomizations();
        this.rateiTempoService.rateiTempoSource.subscribe(() => {
          console.log("refresh");
          this.gridPublicService.refresh(true);
        });

    }

    handleCustomizations(): void {
        this.behavior.saveExternalChanges = true;
        this.generalSettings.performOnEdit = true;
        this.selectable = new AgrSelectableSettings();
        this.selectable.selectable.checkboxOnly = false;
        this.toolbar = new ToolbarSettings();
        this.toolbar.newItem = this.edit;
        this.toolbar.resetChanges = false;
        this.cmdColumn = new CommandsColumnSettings(
            {
                editBtn: this.edit,
                infoBtn: false,
                removeBtn: this.edit,
                onDisableInfoBtn: () => false
            });
        this.resizable = new ResizableSettings();
        this.resizable.autoFitColumns = true;
        this.resizable.autoFitColumns = false;
        this.resizable.isResizable = true;
        this.views.enabled = false;
        this.columnMenu.columnMenu = false;
        this.groups.groupable.enabled = false;
    }

    read(options?: any): Observable<TurniIrriguaServerResult> {
        this.rateiTempo = this.controlContainer.control.parent.value.rateiTempo;
        this.rateiTempoService.init(this.rateiTempo);
        const rows = this.rateiTempo.map(comp => ({
            Rateo_Cod: comp.Rateo_Cod,
            DataInizio: comp.DataInizio,
            DataFine: comp.DataFine,
            OraInizio: comp.OraInizio,
            OraFine: comp.OraFine,
            Rotazione: comp.Rotazione
        } as KendoGridRow));
        return of(new TurniIrriguaServerResult(
            this.getModelGridTurniIrrigua(),
            this.getColumnsGridTurniIrrigua(),
            rows
        ));
    }

    perform(actionType: HttpAction, items: any, oldRow?: any): Observable<any> {
            switch(actionType) {
              case HttpAction.CREATE:
                items.OraInizio = items.OraInizio ?? new Date();
                items.OraFine = items.OraFine ?? new Date();
                if(this.rateiTempo.filter(r => this.isRatioEqual(items, r)).length > 0) {
                  this.giasDialogService.alertMessage(this.transloco.translate("macchina.RateoGiaPresente"), null, Dialog_Type.error)
                  break;
                }
                this.rateiTempo.push(new RateoTempo(items.Rateo_Cod, items.DataInizio, items.DataFine, items.OraInizio, items.OraFine, items.Rotazione, AGRODATAINIZIO, AGRODATAFINE));
                break;
                case HttpAction.UPDATE:
                    items.OraInizio = items.OraInizio ?? oldRow.OraInizio;
                    items.OraFine = items.OraFine ?? oldRow.OraFine;
                    this.rateiTempo.forEach(comp => { 
                        if(comp.Rateo_Cod == items.Rateo_Cod && this.isRatioEqual(oldRow, comp)){
                            this.rateiTempo[this.rateiTempo.indexOf(comp)] = new RateoTempo(items.Rateo_Cod, items.DataInizio, items.DataFine, items.OraInizio, items.OraFine, items.Rotazione, AGRODATAINIZIO, AGRODATAFINE)
                        }
                    });
                    this.rateiTempoService.setRateiTempo(this.rateiTempo);
                    return of(null);
                break;
                case HttpAction.REMOVE:
                    this.rateiTempo.forEach(comp => {
                        if(comp.Rateo_Cod == items.Rateo_Cod && this.isRatioEqual(items, comp)) {
                            this.rateiTempo.splice(this.rateiTempo.indexOf(comp), 1);
                        }
                    });
                break;
            }
            this.rateiTempoService.setRateiTempo(this.rateiTempo);
            return from([]);
    }

    private isRatioEqual(items: RateoTempo, other: RateoTempo): boolean {
       return other.DataInizio == items.DataInizio
        && other.DataFine == items.DataFine
        && other.OraInizio.getHours() == items.OraInizio.getHours()
        && other.OraInizio.getMinutes() == items.OraInizio.getMinutes()
        && other.OraFine.getHours() == items.OraFine.getHours()
        && other.OraFine.getMinutes() == items.OraFine.getMinutes()
        && other.Rotazione == items.Rotazione;
    }

    private getColumnsGridTurniIrrigua(): Array<KendoGridColumn> {
        return [
            new KendoGridColumn({field: 'Rateo_Cod', title: this.translocoService.translate('Codice')}, {hidden: true, resizable: true, editable: false, numeric: { defaultValue: 0 } /*, validators: [Validators.required] */}),
            new KendoGridColumn({field: 'DataInizio', title: this.translocoService.translate('GiornoInizio')},
                {/* hidden: true, */ resizable: true, editable: this.edit, date: new DateSettings({
                        defaultValue: new Date(),
                        min: AGRODATAINIZIO,
                        max: AGRODATAFINE,
                        format: 'dd/MM',
                        placeholder: ''
                    }), validators: [this.checkDates]
                }
            ),
            new KendoGridColumn({field: 'DataFine', title: this.translocoService.translate('GiornoFine')},
                {/* hidden: false, */ resizable:true, editable: this.edit, date: new DateSettings({
                        defaultValue: new Date(),
                        min: AGRODATAINIZIO,
                        max: AGRODATAFINE,
                        format: 'dd/MM',
                        placeholder: ''
                    }), validators: [this.checkDates]
                }
            ),
            new KendoGridColumn({field: 'OraInizio', title: this.translocoService.translate('OraInizio')}, {/* hidden: false, */ resizable:true, editable: this.edit, component: CustomDateTimePickerComponent}),
            new KendoGridColumn({field: 'OraFine', title: this.translocoService.translate('OraFine')}, {/* hidden: false, */ resizable: true, editable: this.edit, component: CustomDateTimePickerComponent}),
            new KendoGridColumn({field: 'Rotazione', title: this.translocoService.translate('Rotazione')}, {/* hidden: false, */ resizable:true, editable: this.edit, numeric: new NumericSettings({ format: '#', decimals: 0, })})
        ];
    }

    private getModelGridTurniIrrigua(){
        return {
            Rateo_Cod: { editable: false, type: CELL_TYPES.NUMBER },
            DataInizio: { editable: true, type: CELL_TYPES.DATE, format: 'dd/MM'/*, type: CELL_TYPES.CUSTOM */},
            DataFine: { editable: true, type: CELL_TYPES.DATE, format: 'dd/MM'/*, type: CELL_TYPES.CUSTOM */},
            OraInizio: { editable: true, /* type: CELL_TYPES.DATETIME, format: 'HH:mm', */ type: CELL_TYPES.CUSTOM,},
            OraFine: { editable: true, /* type: CELL_TYPES.DATETIME, format: 'HH:mm', */ type: CELL_TYPES.CUSTOM,},
            Rotazione: { editable: false, type: CELL_TYPES.NUMBER }
        };
    }

    private checkDates: ValidatorFn = (control: AbstractControl) => {
        const dataInizio = control?.parent?.get('DataInizio')?.value;
        const dataFine = control.parent?.get('DataFine')?.value;
        if (dataFine && dataInizio && new Date(dataFine) < new Date(dataInizio)) {
            // this.giasMessageService.errorMessage(this.transloco.translate('DataInizioNonDeveEssereMaggioreDiDataFine'));
            return { invalidDateRange: true };
        }
        // console.log(control);
        control?.parent?.get('DataInizio')?.setErrors(null);
        control?.parent?.get('DataFine')?.setErrors(null);
        return null; // Validazione riuscita
    };

}