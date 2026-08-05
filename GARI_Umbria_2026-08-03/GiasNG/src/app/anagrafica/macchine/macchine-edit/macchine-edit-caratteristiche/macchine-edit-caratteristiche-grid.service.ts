import { Inject, Injectable, Injector } from '@angular/core';
import { AbstractControl, ControlContainer, FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TranslocoService } from '@jsverse/transloco';
import { Caratteristica } from 'app/Model/anagrafiche/ParcoMacchine';
import { BaseCodeDescr } from 'app/Model/baseClass/baseCodeDescr';
import { CoreWS_Generic } from 'app/Model/CoreWS/CoreWS_Generic';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { AjaxAgronicaService } from 'app/Service/ajax-agronica.service';
import { UnitaDiMisura } from 'app/Service/api.service';
import { MasterService } from 'app/Service/master.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { MacchineFactoryService, MACCHINE_SERVICE_TOKEN } from 'app/Service/ServiceFactory/macchine.factory.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { AgrSelectableSettings, BehaviorSettings, CommandsColumnSettings, GeneralSettings, ResizableSettings, ToolbarSettings } from 'gias-kendo-grid';
import {  DateSettings, DropdownListItem, DropdownListWithForm, EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, LoaderType } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { ConfigTemplate } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { from, map, Observable, of, Subscription, take } from 'rxjs';
import { MacchinaKendoServerResult } from '../../macchine.model';
import { CostiMacchinaDataService } from '../costi-macchina-edit/costi-macchina-data.service';
import { forbiddenPriceValidator } from '../costi-macchina-edit/costi-macchina-edit-grid-config.service';
import { CostiMacchinaEditService } from '../costi-macchina-edit/costi-macchina-edit.service';
import { CostiMacchinaParentFormService } from '../costi-macchina-edit/costi-macchina-parent-form.service';
import { CaratteristicheService } from './caratteristiche.service';
import {IntervalloTemporale} from '../../../../Model/anagrafiche/IntervalloTemporale';

@Injectable()
export class MacchinaEditCaratteristicheConfigService extends AbstractGridConfigService<MacchinaKendoServerResult> {
  gridId = 'MacchineEditCaratteristicheConfigService';

  editingMode: EditingMode = EditingMode.IN_CELL;
  loader: LoaderType = LoaderType.SERVICE;
  rowId = 'codice';
  generalSettings = new GeneralSettings;
  caratteristicheColumns: KendoGridColumn[];
  behavior = new BehaviorSettings({});
  private objParametriAgenda;
  private edit;

  caratteristiche: Caratteristica[] = [];

  caratteristicheMacchinaParentForm: FormGroup = this.fb.group({});
  caratteristicheMacchinaParentFormSub: Subscription;

  constructor(
      injector: Injector,
      @Inject(MACCHINE_SERVICE_TOKEN) private macchineService: MacchineFactoryService,
      private fb: FormBuilder,
      private objP: ObjParametriAgendaService,
      private translocoService: TranslocoService,
      private controlContainer: ControlContainer,
      private caratteristicheService: CaratteristicheService){
      super(injector, ConfigTemplate.DefaultTemplate);

      this.objParametriAgenda = this.objP.getObjParamValue();
      this.edit = true;

      if (this.objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Read) {
        this.edit = false;
      }


      this.resizable = new ResizableSettings();
      this.resizable.autoFitColumns = true;

      this.behavior.saveExternalChanges = true;

      this.generalSettings.performOnEdit = true;
      this.handleCustomizations();

        this.caratteristicheService.caratteristicheSource.subscribe((val) => {
          console.log("refresh")
          this.gridPublicService.refresh(true);
        })



  }



  read(): Observable<MacchinaKendoServerResult> {

    this.caratteristiche = this.controlContainer.control.parent.value.caratteristiche;

      const columns = this.setColumnsGridCaratteristiche();
      const model = this.setModelGridCaratteristiche();

      this.handleDropdowns(columns);

      let rows = []
      this.caratteristiche.forEach(car => {
        rows.push({codice: car.codice,
          Caratteristica_Cod: car.caratteristica.codice,
          Caratteristica_Des: car.caratteristica.descrizione,
          valore: car.valore,
          validita_inizio: car.validita_inizio,
          validita_fine: car.validita_fine
        } as KendoGridRow)
      })

      const result: MacchinaKendoServerResult = new MacchinaKendoServerResult(model, columns, rows);
      return of(result)

  }

  handleCustomizations(): void {
      this.selectable = new AgrSelectableSettings();
      this.selectable.selectable.checkboxOnly = false;

      this.toolbar = new ToolbarSettings();
      this.toolbar.newItem = this.edit;
      this.toolbar.resetChanges = false;

      this.cmdColumn = new CommandsColumnSettings(
          {
              editBtn: false,
              infoBtn: false,
              removeBtn: true,
              onDisableInfoBtn: () => false
          });

      this.resizable.autoFitColumns = false;
      this.resizable.isResizable = true;
      this.views.enabled = false;
      this.columnMenu.columnMenu = false;
      this.groups.groupable.enabled = false;
  }

  setColumnsGridCaratteristiche(){
      const columns: Array<KendoGridColumn>=[
          new KendoGridColumn(
            {
                field: 'codice',
                title: this.translocoService.translate('Codice')
            },
            {
                hidden: true,
                resizable:true,
                editable: this.edit,
                numeric: { },
                validators: [Validators.required]
            }
          ),
          new KendoGridColumn(
              {
                  field: 'Caratteristica_Cod',
                  title: this.translocoService.translate('Caratteristica')
              },
              {
                  hidden: false,
                  resizable:true,
                  editable: this.edit,
                  validators: [Validators.required]
              }
          ),
          new KendoGridColumn(
            {
                field: 'valore',
                title: this.translocoService.translate('Valore')},
            {
                hidden: false,
                resizable:true,
                editable: this.edit,
                validators: [Validators.required]
            }
          ),
          new KendoGridColumn(
            {
                field: 'validita_inizio',
                title: this.translocoService.translate('ValiditàInizio')},
            {
                hidden: false,
                resizable:true,
                editable: this.edit,
                date: { defaultValue : AGRODATAINIZIO},
                validators: [Validators.required]
            }
          ),
          new KendoGridColumn(
            {
                field: 'validita_fine',
                title: this.translocoService.translate('ValiditàFine')},
            {
                hidden: false,
                resizable:true,
                editable: this.edit,
                date: { defaultValue : AGRODATAFINE},
                validators: [Validators.required]
            }
          )
      ];

      return columns;
  }

  setModelGridCaratteristiche(){
    this.cmdColumn.removeBtn = this.edit;
      const model: KendoGridModel={
          codice:{
              editable: false,
              type: CELL_TYPES.NUMBER
          },
          Caratteristica_Des:{
              editable: true,
              type: CELL_TYPES.STRING
          },
          Caratteristica_Cod:{
            editable: true,
            type: CELL_TYPES.DROPDOWNLIST
          },
          valore:{
              editable: true,
              type: CELL_TYPES.STRING
          },
          flag_cancellazione: {
              editable: true,
              type: CELL_TYPES.BOOLEAN
          },
          validita_inizio:{
            editable: true,
            type: CELL_TYPES.DATE
          },
          validita_fine:{
            editable: true,
            type: CELL_TYPES.DATE
          }
      };
      return model;
  }

  handleDropdowns(columns: KendoGridColumn[]): void {
      const col = columns.find(s => s.field === 'Caratteristica_Cod');
      const data: DropdownListItem[] = [];
      col.ddl = new DropdownListWithForm('codice', 'Caratteristica_Cod', 'descrizione', data);
      col.ddl.valuePrimitive = true;
      col.ddl.descriptionField = 'Caratteristica_Des';
      col.ddl.loadOnEdit = true;
      col.ddl.loadFunction = this.caricaDropdown.bind(this);
  }

  perform(actionType: HttpAction, rows: Array<any>): Observable<KendoGridRow[]> {
    switch(actionType) {
      case HttpAction.CREATE:
        let maxcod = 0
        this.caratteristiche.forEach(t => {
          if (t.codice > maxcod) {
            maxcod = t.codice
          }
        })
        this.caratteristiche.push(new Caratteristica(maxcod + 1, rows[0].Caratteristica_Cod, rows[0].Caratteristica_Des, rows[0].valore, new IntervalloTemporale(rows[0].validita_inizio, rows[0].validita_fine)));
        break;
    case HttpAction.UPDATE:
      this.caratteristiche.forEach(car => {
        if(car.codice == rows[0].codice){
            this.caratteristiche[this.caratteristiche.indexOf(car)] = new Caratteristica(rows[0].codice, rows[0].Caratteristica_Cod, rows[0].Caratteristica_Des, rows[0].valore, new IntervalloTemporale(rows[0].validita_inizio, rows[0].validita_fine))
        }
    });
      break;
    case HttpAction.REMOVE:
        this.caratteristiche.forEach(car => {
            if(car.codice == rows[0].codice){
                this.caratteristiche.splice(this.caratteristiche.indexOf(car), 1)
            }
        });
      break;
    }
    this.caratteristicheService.setCaratteristiche(this.caratteristiche);
    return from([]);
  }

  caricaDropdown(): Observable<DropdownListItem[]> {
    let detParent = this.controlContainer.control.parent.value;
    let class_code = detParent.tipo.codice +
                      (detParent.dettaglio_1 ? (detParent.dettaglio_1.codice != "" ? "." + detParent.dettaglio_1.codice : "") : "") +
                      (detParent.dettaglio_2 ? (detParent.dettaglio_2.codice != "" ? "." + detParent.dettaglio_2.codice : "") : "")

    return this.caratteristicheService.getCaratteristicheDisponibili(class_code);
  }
}

