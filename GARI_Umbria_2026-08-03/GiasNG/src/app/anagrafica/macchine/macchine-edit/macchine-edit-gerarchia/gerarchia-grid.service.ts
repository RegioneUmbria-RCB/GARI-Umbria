import { Inject, Injectable, Injector } from '@angular/core';
import { ControlContainer, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TranslocoService } from '@jsverse/transloco';
import { MacchinaGerarchia, ParcoMacchine, enum_TipoLegame } from 'app/Model/anagrafiche/ParcoMacchine';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { enum_UnitaMisura } from 'app/Model/TipiEnumerativi';
import { Dialog_Type, GiasDialogService } from 'app/Service/gias-dialog.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { MacchineFactoryService, MACCHINE_SERVICE_TOKEN } from 'app/Service/ServiceFactory/macchine.factory.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { AgrSelectableSettings, BehaviorSettings, CommandsColumnSettings, GeneralSettings, ResizableSettings, ToolbarSettings } from 'gias-kendo-grid';
import {  DateSettings, DropdownListItem, DropdownListWithForm, EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, LoaderType } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { ConfigTemplate } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { forkJoin, from, map, Observable, of, Subscription } from 'rxjs';
import { MacchinaKendoServerResult } from '../../macchine.model';
import { GerarchiaService } from './gerarchia.service';
import {IntervalloTemporale} from '../../../../Model/anagrafiche/IntervalloTemporale';


@Injectable()
export class MacchineEditGerarchiaGridService extends AbstractGridConfigService<MacchinaKendoServerResult> {
  gridId = 'MacchineEditGerarchiaGridService';

  editingMode: EditingMode = EditingMode.IN_CELL;
  loader: LoaderType = LoaderType.SERVICE;
  rowId = 'codice';
  generalSettings = new GeneralSettings;
  gerarchiaColumns: KendoGridColumn[];
  behavior = new BehaviorSettings({});
  private objParametriAgenda;
  private edit;

  gerarchia: any[] = [];

  gerarchiaMacchinaParentForm: FormGroup = this.fb.group({});
  gerarchiaMacchinaParentFormSub: Subscription;

  constructor(
      injector: Injector,
      @Inject(MACCHINE_SERVICE_TOKEN) private macchineService: MacchineFactoryService,
      private fb: FormBuilder,
      private objP: ObjParametriAgendaService,
      private translocoService: TranslocoService,
      private controlContainer: ControlContainer,
      private gerarchiaService: GerarchiaService,
      private giasDialogService: GiasDialogService){
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

        this.gerarchiaService.gerarchiaSource.subscribe((val) => {
          console.log("refresh")
          this.gridPublicService.refresh(true);
        })



  }



  read(): Observable<MacchinaKendoServerResult> {

    this.gerarchia = this.controlContainer.control.parent.value.gerarchiaFigli;

      const columns = this.setColumnsGridGerarchia();
      const model = this.setModelGridGerarchia();

      this.handleDropdowns(columns);

      let rows = []
      this.gerarchia.forEach(comp => {
        rows.push({ID: comp.ID,
          codice: comp.codice,
          Componente_Cod: comp.macchina.codice,
          Componente_Des: comp.macchina.descrizione != "" ? comp.macchina.descrizione : comp.macchina.dettaglio_1.descrizione,
          Legame_Cod: comp.legame.codice,
          Legame_Des: enum_TipoLegame[comp.legame.codice],
          DescrizioneLegame: comp.desclegame,
          UdM_Cod: comp.udm.codice,
          UdM_Des: enum_UnitaMisura[comp.udm.codice],
          Qta: comp.qta,
          validita_inizio: comp.validita_inizio,
          validita_fine: comp.validita_fine
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

  setColumnsGridGerarchia(){
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
                numeric: { defaultValue: 0 }
                //validators: [Validators.required]
            }
          ),
          new KendoGridColumn(
              {
                  field: 'Componente_Cod',
                  title: this.translocoService.translate('Componente')
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
                field: 'Legame_Cod',
                title: this.translocoService.translate('Tipo Legame')
            },
            {
                hidden: false,
                resizable:true,
                editable: this.edit,
                numeric: { defaultValue: 0 },
                validators: [Validators.required]
            }
          ),
          new KendoGridColumn(
            {
                field: 'DescrizioneLegame',
                title: this.translocoService.translate('Descrizione Legame')},
            {
                hidden: false,
                resizable:true,
                editable: this.edit
            }
          ),
          new KendoGridColumn(
            {
                field: 'UdM_Cod',
                title: this.translocoService.translate('Unità di Misura')
            },
            {
                hidden: false,
                resizable:true,
                editable: this.edit,
            }
          ),
          new KendoGridColumn(
            {
                field: 'Qta',
                title: this.translocoService.translate('Quantità')
            },
            {
                hidden: false,
                resizable:true,
                editable: this.edit,
                numeric: { defaultValue: 0 },
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
                date: new DateSettings({ defaultValue: AGRODATAINIZIO }),
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
                date: new DateSettings({ defaultValue: AGRODATAFINE }),
                validators: [Validators.required]
            }
          )
      ];

      return columns;
  }

  setModelGridGerarchia(){
    this.cmdColumn.removeBtn = this.edit;
      const model: KendoGridModel={
          ID:{
            editable: false,
            type: CELL_TYPES.NUMBER
          },
          codice:{
              editable: false,
              type: CELL_TYPES.NUMBER
          },
          Componente_Des:{
              editable: true,
              type: CELL_TYPES.STRING
          },
          Componente_Cod:{
            editable: true,
            type: CELL_TYPES.DROPDOWNLIST
          },
          Legame_Des:{
              editable: true,
              type: CELL_TYPES.STRING
          },
          Legame_Cod:{
            editable: true,
            type: CELL_TYPES.DROPDOWNLIST
          },
          DescrizioneLegame:{
              editable: true,
              type: CELL_TYPES.STRING
          },
          UdM_Cod:{
            editable: true,
            type: CELL_TYPES.DROPDOWNLIST
          },
          UdM_Des:{
            editable: true,
            type: CELL_TYPES.STRING
          },
          Qta:{
            editable: true,
            type: CELL_TYPES.NUMBER
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
      const colComp = columns.find(s => s.field === 'Componente_Cod');
      const dataComp: DropdownListItem[] = [];
      colComp .ddl = new DropdownListWithForm('codice', 'Componente_Cod', 'descrizione', dataComp);
      colComp .ddl.valuePrimitive = false;
      colComp .ddl.descriptionField = 'Componente_Des';
      colComp .ddl.loadOnEdit = true;
      colComp .ddl.loadFunction = this.caricaDropdownComp.bind(this);

      const colLeg = columns.find(s => s.field === 'Legame_Cod');
      const dataLeg: DropdownListItem[] = [];
      colLeg.ddl = new DropdownListWithForm('codice', 'Legame_Cod', 'descrizione', dataLeg);
      colLeg.ddl.valuePrimitive = true;
      colLeg.ddl.descriptionField = 'Legame_Des';
      colLeg.ddl.loadOnEdit = true;
      colLeg.ddl.loadFunction = this.caricaDropdownLeg.bind(this);

      const colUdM = columns.find(s => s.field === 'UdM_Cod');
      const dataUdM: DropdownListItem[] = [];
      colUdM.ddl = new DropdownListWithForm('codice', 'UdM_Cod', 'descrizione', dataUdM);
      colUdM.ddl.valuePrimitive = true;
      colUdM.ddl.descriptionField = 'UdM_Des';
      colUdM.ddl.loadOnEdit = true;
      colUdM.ddl.loadFunction = this.caricaDropdownUdM.bind(this);
  }

  perform(actionType: HttpAction, rows: Array<any>): Observable<KendoGridRow[]> {
    switch(actionType) {
      case HttpAction.CREATE:
        if(this.gerarchia.filter(t => { return t.macchina.codice === rows[0].Componente_Cod.id && ((rows[0].validita_inizio <= t.validita_fine) && (rows[0].validita_fine >= t.validita_inizio))}).length != 0) {
          this.giasDialogService.alertMessage(this.transloco.translate("DateMacchineSovrapposizione"), null, Dialog_Type.error)
          break;
        }
        let maxcod = 0
        this.gerarchia.forEach(t => {
          if (t.codice > maxcod) {
            maxcod = t.codice
          }
        })
        this.gerarchia.push(new MacchinaGerarchia(0, maxcod + 1, rows[0].Componente_Cod.data, rows[0].Legame_Cod, rows[0].Legame_Des, rows[0].DescrizioneLegame, rows[0].UdM_Cod, rows[0].UdM_Des, rows[0].Qta, new IntervalloTemporale(rows[0].validita_inizio, rows[0].validita_fine)))
        break;
    case HttpAction.UPDATE:
      this.gerarchia.forEach(comp => {
        if(comp.codice == rows[0].codice){
            this.gerarchia[this.gerarchia.indexOf(comp)] = new MacchinaGerarchia(rows[0].ID, rows[0].codice, {codice: rows[0].Componente_Cod.id, descrizione: rows[0].Componente_Cod.name != null ? rows[0].Componente_Cod.name : rows[0].Componente_Des} as ParcoMacchine, rows[0].Legame_Cod, rows[0].Legame_Des, rows[0].DescrizioneLegame, rows[0].UdM_Cod, rows[0].UdM_Des, rows[0].Qta, new IntervalloTemporale(rows[0].validita_inizio, rows[0].validita_fine))
        }
    });
      break;
    case HttpAction.REMOVE:
        this.gerarchia.forEach(comp => {
            if(comp.codice == rows[0].codice){
                this.gerarchia.splice(this.gerarchia.indexOf(comp), 1)
            }
        });
      break;
    }
    this.gerarchiaService.setGerarchia(this.gerarchia);
    return from([]);
  }

  caricaDropdownComp(): Observable<DropdownListItem[]> {
    let macchine = this.macchineService.leggiMacchine(this.objP.getObjParamValue())
    let ddlitems = []
    return forkJoin({macchine}).pipe(map(result => {
      let kendoMacchine = <any>result.macchine.RispostaStringa;
      let macchineKendo = <Array<any>>(kendoMacchine.kendo_rows);
      macchineKendo.forEach(macchina => {
        if (macchina.Mac_Cod != this.controlContainer.control.parent.value.codice) {
          ddlitems.push({codice: macchina.Mac_Cod, descrizione: macchina.Macchina != "" ? macchina.Macchina : macchina.tipologia, tipo: {codice: "", descrizione: macchina.tipologia}} as ParcoMacchine);
        }
      });
      return ddlitems;
    }));
  }

  caricaDropdownLeg(): Observable<DropdownListItem[]> {
    let ddlitems = []
    ddlitems.push({codice:enum_TipoLegame.Gerarchia, descrizione: this.transloco.translate("Gerarchia")});
    return of(ddlitems);
  }

  caricaDropdownUdM(): Observable<DropdownListItem[]> {
    let ddlitems = []
    ddlitems.push({codice:enum_UnitaMisura.Numero, descrizione: this.transloco.translate("Numero")}, {codice:enum_UnitaMisura.Metri, descrizione: this.transloco.translate("Metri")});
    return of(ddlitems);
  }
}

