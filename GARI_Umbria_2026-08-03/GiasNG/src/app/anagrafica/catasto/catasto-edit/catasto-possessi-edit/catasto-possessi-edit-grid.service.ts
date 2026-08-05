import { Inject, Injectable, Injector } from '@angular/core';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import {combineLatest, from, Observable, tap} from 'rxjs';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import {  DateSettings, DropdownListItem, DropdownListWithForm, EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { ConfigTemplate } from 'gias-kendo-grid';
import { PossessiService, PossessoParticellaId } from './Possessi.service';
import {filter, map, takeUntil} from 'rxjs/operators';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { TitoloDiPossesso } from 'app/Model/metaschema/TitoloDiPossesso';
import { ExcelSettings, PDFSettings, SelectableSettings, ToolbarSettings } from 'gias-kendo-grid';
import { IntervalloTemporale } from 'app/Model/anagrafiche/IntervalloTemporale';
import { Validators } from '@angular/forms';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { CatastoFactoryService, CATASTO_SERVICE_TOKEN } from 'app/Service/ServiceFactory/catasto.factory.service';
import { take } from 'rxjs/operators';
import { TranslocoService } from '@jsverse/transloco';
import { ObjParametriAgenda } from 'gias-ui-kit';


export class PossessiParticelleKendo extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

@Injectable({
    providedIn:'root'
})

export class PossessiParticelleService extends AbstractGridConfigService<PossessiParticelleKendo>{
    gridId = 'PossessiParticelle';
    rowId = 'id';

    edit: boolean;
    hasPossessi: boolean;
    objParametriAgenda: ObjParametriAgenda;
    toolbar = new ToolbarSettings(true, false);

    loader: LoaderType = LoaderType.SERVICE;
    editingMode: EditingMode = EditingMode.IN_CELL;

    kendoColumns: Array<KendoGridColumn>;

    kendoModel: KendoGridModel = {
        codice: {
            editable: false,
            type: CELL_TYPES.STRING
        },
        id: {
            editable: false,
            type: CELL_TYPES.NUMBER
        },
        Superficie: {
            editable: true,
            type: CELL_TYPES.NUMBER,
            validators: [Validators.required]
        },
        CodiceParticella: {
            editable: true,
            type: CELL_TYPES.STRING
        },
        TitoloPossesso: {
            editable: true,
            type: CELL_TYPES.DROPDOWNLIST,
            validators: [Validators.required]
        },
        Validita_Inizio: {
            editable: true,
            type: CELL_TYPES.DATE,
            validators: [Validators.required]
        },
        Validita_Fine: {
            editable: true,
            type: CELL_TYPES.DATE,
            validators: [Validators.required]
        }
    };

    constructor(injector: Injector,
        private possessiParticelleService: PossessiService,
        private ObjParametriAgendaService: ObjParametriAgendaService,
        private translocoService: TranslocoService,
        @Inject(CATASTO_SERVICE_TOKEN) private catastoService: CatastoFactoryService) {


        super(injector, ConfigTemplate.DefaultTemplate);

        this.objParametriAgenda = this.ObjParametriAgendaService.getObjParamValue();
        this.edit = true;


        if (this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Read) {
            this.edit = false;
        }

        // Nascondo la colonna Azioni con i bottoni di Info,Modifica e Cancella
        this.cmdColumn.editBtn = false;
        this.cmdColumn.infoBtn = false;
        this.toolbar.newItem = this.edit;
        // this.resizable.autoFitColumns = true;
        this.resizable.isResizable = true;

        // Setting generali
        this.columnMenu.columnMenu = false;
        this.columnMenu.filterable = false;
        this.behavior.saveExternalChanges = true;
        this.behavior.excelSettings = new ExcelSettings({enabled: false});
        this.behavior.pdfSettings = new PDFSettings({enabled: false});
        this.generalSettings.reordable = false;
        this.generalSettings.performOnEdit = true;
        this.groups.groupable.enabled = false;

        this.pagination.pageable = false;

        this.selectable.selectable = new SelectableSettings({});
        this.views.enabled = false;

        this.possessiParticelleService.possessoParticellaSource.GiasSubscribe((val) => {
            this.gridPublicService.refresh(true);
        })

      this.gridPublicService.changeDetected.pipe(
        takeUntil(this.signal),
        tap((e) => { console.log(e) }),
        filter((e:any) => {
          if(e.action == 'add'){
            return e;
          }
        }),
        tap((event) => {
          const fb = this.gridPublicService.formGroup.getValue();
          if (this.possessiParticelleService.particellaVal?.particella?.Area > 0){
            fb.controls['Superficie'].setValue(this.possessiParticelleService.particellaVal?.particella?.Area);
          }
        })
      ).subscribe()

    }

    perform(actionType: HttpAction, items: any): Observable<any[]> {

        const catastoApp = this.possessiParticelleService.getPossessoParticella();

        switch (actionType) {
            case HttpAction.CREATE:
                var possessoParticellaId = new PossessoParticellaId();
                possessoParticellaId.Area = items[0].Superficie;
                possessoParticellaId.codice = 0;
                possessoParticellaId.validita = new IntervalloTemporale(items[0].Validita_Inizio, items[0].Validita_Fine);
                //possessoParticellaId.titolo_Di_Possesso = new TitoloDiPossesso(parseInt(items[0].TitoloPossesso));
                possessoParticellaId.titolo_Di_Possesso = {
                    codice: items[0].TitoloPossesso.id,
                    descrizione: items[0].TitoloPossesso.name
                }
                possessoParticellaId.codice_particella = items[0].CodiceParticella;
                const maxID = Math.max(...catastoApp.map(obj => { return obj.id }));
                possessoParticellaId.id = maxID + 1;
                items[0].id = possessoParticellaId.id;
                items[0].codice = possessoParticellaId.codice;
                catastoApp.push(possessoParticellaId);
                break;
            case HttpAction.REMOVE:
                var index = catastoApp.findIndex((el) => el.id == parseInt(items[0].id));
                if (index != -1) {
                    catastoApp.splice(index, 1);
                }
                break;
            case HttpAction.UPDATE:
                var index = catastoApp.findIndex((el) => el.id == parseInt(items[0].id));
                var el = catastoApp.find((el) => el.id == parseInt(items[0].id));
                el.Area = items[0].Superficie;
                el.codice = items[0].codice;
                el.validita = new IntervalloTemporale(items[0].Validita_Inizio, items[0].Validita_Fine);
                el.titolo_Di_Possesso = {
                    codice: items[0].TitoloPossesso.id,
                    descrizione: items[0].TitoloPossesso.name
                }
                el.codice_particella = items[0].CodiceParticella;
                catastoApp[index] = el;
                break;
        }

        this.possessiParticelleService.setPossessoParticella(catastoApp);


        return from([this.possessiParticelleService.getPossessoParticella()]);
    }


    read(): Observable<PossessiParticelleKendo> {

        let tabellaObs = this.possessiParticelleService.possessoParticellaSource
        let possessiObs = this.catastoService.checkPossessi(this.catastoService.getParticellaEdit())
        if (this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Write) {
            return tabellaObs.pipe(take(1), map((data) => {
                const rows: KendoGridRow[] = new Array<KendoGridRow>();
                let id = 0;
                for (const possesso of data) {
                    const row: KendoGridRow = {
                        id: String(possesso.id),
                        codice: possesso.codice,
                        Superficie: possesso.Area,
                        TitoloPossesso: possesso.titolo_Di_Possesso.codice,
                        Validita_Inizio: possesso.validita.inizio,
                        Validita_Fine: possesso.validita.fine,
                        CodiceParticella: possesso.codice_particella
                    };
                    rows.push(row);
                    id++;
                }
                this.kendoColumns = this.kendoGridColumnInitalizer(false)
                this.handleDropdowns(this.catastoService.titoli_Di_Possesso);
                const p = new PossessiParticelleKendo(rows, this.kendoColumns, this.kendoModel);
                return p;
            }));
        } else {
            return combineLatest([possessiObs, tabellaObs]).pipe(take(1),map((data) => {
            this.hasPossessi = data[0]
            const rows: KendoGridRow[] = new Array<KendoGridRow>();
            let id = 0;
            for (const possesso of data[1]) {
                const row: KendoGridRow = {
                    id: possesso.id,
                    codice: possesso.codice,
                    Superficie: possesso.Area,
                    TitoloPossesso: possesso.titolo_Di_Possesso.codice,
                    Validita_Inizio: possesso.validita.inizio,
                    Validita_Fine: possesso.validita.fine,
                    CodiceParticella: possesso.codice_particella
                };
                rows.push(row);
                id++;
            }
            this.kendoColumns = this.kendoGridColumnInitalizer(this.hasPossessi)
            this.handleDropdowns(this.catastoService.titoli_Di_Possesso);
            const p = new PossessiParticelleKendo(rows, this.kendoColumns, this.kendoModel);
            return p;
            }));
        }
    }

    kendoGridColumnInitalizer(bloccato: boolean): KendoGridColumn[]{
        this.cmdColumn.removeBtn = this.edit &&!bloccato;
        return [
            // new KendoGridColumn(
            //     { field: "codice", title: "codice" }, { resizable: true, editable: false}
            // ),
            // new KendoGridColumn(
            //     { field: "id", title: "id" }, { resizable: true, editable: false}
            // ),
            new KendoGridColumn(
                { field: 'TitoloPossesso', title: this.translocoService.translate('TitoloDiPossesso') },
                { resizable: true, editable: this.edit && !bloccato, validators: [Validators.required], width: 135 }
            ),
            new KendoGridColumn(
                { field: 'Validita_Inizio', title: this.translocoService.translate('ValiditàInizio') },
                { resizable: true, editable: this.edit && !bloccato, date: new DateSettings({ defaultValue: AGRODATAINIZIO }), validators: [Validators.required], width: 135 }
            ),
            new KendoGridColumn(
                { field: 'Validita_Fine', title: this.translocoService.translate('ValiditàFine') },
                { resizable: true, editable: this.edit && !bloccato, date: new DateSettings({ defaultValue: AGRODATAFINE }), validators: [Validators.required], width: 135 }
            ),
            new KendoGridColumn(
                { field: 'Superficie', title: this.translocoService.translate('SuperficieCondottaAbbr') },
                { resizable: true, numeric: { defaultValue: 0, min: 0, format: 'n4' }, editable: this.edit && !bloccato, validators: [Validators.required], width: 135 }
            ),
            new KendoGridColumn(
                { field: 'CodiceParticella', title: this.translocoService.translate('CodiceParticella') },
                { resizable: true, editable: this.edit, width: 135 }
            )
        ];
    }

    handleDropdowns(ddlist: TitoloDiPossesso[]): void {
        const col = this.kendoColumns.find(s => s.field === 'TitoloPossesso');

        const data: DropdownListItem[] = ddlist.map(cod => new DropdownListItem(cod.codice, cod.descrizione));

        col.ddl = new DropdownListWithForm('codice', 'TitoloPossesso', 'descrizione', data);
        col.ddl.valuePrimitive = false;
    }
}
