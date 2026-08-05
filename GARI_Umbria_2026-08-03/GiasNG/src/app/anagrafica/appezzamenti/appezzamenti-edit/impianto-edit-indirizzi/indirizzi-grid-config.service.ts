import { Injectable, Injector } from '@angular/core';
import { from, map, Observable, of, Subscription, take, tap } from 'rxjs';
import { DropdownListWithForm, EditingMode, KendoGridColumn, KendoGridRow, LoaderType, ModelEntry } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { CELL_TYPES, ObjParametriAgenda } from 'gias-ui-kit';
import { BehaviorSettings, GeneralSettings, CommandsColumnSettings, ToolbarSettings, AgrSelectableSettings, ResizableSettings } from 'gias-kendo-grid';
import { GridPublicService } from 'gias-kendo-grid';
import { ConfigTemplate } from 'gias-kendo-grid';
import { IndirizziAppezzamentoModel, IndirizziServerResult } from '../../appezzamenti.model';
import { SMARTPHONE_WIDTH } from 'app/Model/CostantiPersonalizzate';
import { IndirizziAppezzamentoDataService } from './indirizzi-data.service';
import { IndirizziAppezzamentoService } from './indirizzi.service';
import { Comune } from 'app/Model/MetaschemaModel';
import { CentriAziendaliService } from 'app/Service/Anagrafica/centri.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { IndirizziParentFormDataService } from './indirizzi-parent-from.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { TranslocoService } from '@jsverse/transloco';
import { capValidatorAppezzamenti } from 'gias-ui-kit';
import {GiasIstatService} from '../../../../Service/istat/gias-istat.service';

@Injectable()
export class IndirizziConfigService extends AbstractGridConfigService<IndirizziServerResult> {
  gridId = 'IdirizziAppezzamento';
  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_CELL;
  behavior: BehaviorSettings;
  rowId = 'chiave';
  generalSettings = new GeneralSettings();

  private objParametriAgenda: ObjParametriAgenda;

  private IndirizziServerResult: IndirizziServerResult;
  private gridIndirizziAppezzamento: any = {};

  private indirizziParentForm: FormGroup = this.fb.group({});
  private indirizziParentFormSub: Subscription;

  private edit: boolean;

  private indirizziColumns: KendoGridColumn[] = [
    new KendoGridColumn(
      { field: 'cap', title: this.transloco.translate('CAP') },
      {
        resizable: true,
        filterable: true,
        editable: true,
        width: 135,
        validators: [capValidatorAppezzamenti()]
      }
    ),
    new KendoGridColumn(
      { field: 'prov', title: this.transloco.translate('Provincia') },
      {
        resizable: true,
        filterable: true,
        editable: true,
        width: 135
      }
    ),
    new KendoGridColumn(
      { field: 'com', title: this.transloco.translate('Comune') },
      {
        resizable: true,
        filterable: true,
        editable: true,
        width: 135
      }
    ),
    new KendoGridColumn(
      { field: 'frazione', title: this.transloco.translate('Frazione') },
      {
        resizable: true,
        filterable: true,
        editable: true,
        width: 135
      }
    ),
    new KendoGridColumn(
      { field: 'via', title: this.transloco.translate('Via') },
      {
        resizable: true,
        filterable: true,
        editable: true,
        width: 135
      }
    ),
    new KendoGridColumn(
      { field: 'codice_stato', title: this.transloco.translate('Stato') },
      {
        resizable: true,
        filterable: true,
        editable: true,
        width: 135
      }
    ),
    new KendoGridColumn(
      { field: 'note', title: this.transloco.translate('Note') },
      {
        resizable: true,
        filterable: true,
        editable: true,
        width: 135
      }
    )
  ];

  private indirizziModel: IndirizziAppezzamentoModel = {
    chiave: new ModelEntry(CELL_TYPES.NUMBER, false),
    codice: new ModelEntry(CELL_TYPES.NUMBER, false),
    cap: new ModelEntry(CELL_TYPES.STRING, false),
    frazione: new ModelEntry(CELL_TYPES.STRING, false),

    prov: new ModelEntry(CELL_TYPES.DROPDOWNLIST, true),
    PROVINCIA: new ModelEntry(CELL_TYPES.STRING, true),
    com: new ModelEntry(CELL_TYPES.DROPDOWNLIST, true),
    COMUNE: new ModelEntry(CELL_TYPES.STRING, true),
    codice_stato: new ModelEntry(CELL_TYPES.DROPDOWNLIST, false),
    descrizione_stato: new ModelEntry(CELL_TYPES.STRING, false),

    note: new ModelEntry(CELL_TYPES.STRING, false),
    via: new ModelEntry(CELL_TYPES.STRING, false),
    tipo_indirizzo: new ModelEntry(CELL_TYPES.STRING, false)
  }

  constructor(
    injector: Injector,
    private fb: FormBuilder,
    private indirizziAppezzamentoDataSevice: IndirizziAppezzamentoDataService,
    private indirizziAppezzamentoService: IndirizziAppezzamentoService,
    private indirizziParentFormDataService: IndirizziParentFormDataService,
    private istatService: GiasIstatService,
    private gridPublicSerivce: GridPublicService,
    private centriService: CentriAziendaliService,
    private objParametriAgendaService: ObjParametriAgendaService,
    protected transloco: TranslocoService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);

    this.objParametriAgenda = objParametriAgendaService.getObjParamValue();
    this.resizable = new ResizableSettings();
    this.resizable.autoFitColumns = true;

    const objParametriAgenda = this.objParametriAgendaService.getObjParamValue();

    this.edit = objParametriAgenda.TipoOperazioneDB != Enum_DBTypeOperation.Read;

    this.behavior.saveExternalChanges = this.edit;
    this.generalSettings.performOnEdit = this.edit;

    this.handleCustomizations();

    this.indirizziParentFormSub = this.indirizziParentFormDataService.indirizziParentForm$
      .subscribe(i => {
        this.indirizziParentForm = this.indirizziParentFormDataService.indirizziParentForm;
      });

    this.gridPublicService.changeDetected.pipe(
      tap((event: any) => {
        const fb = this.gridPublicService.formGroup.getValue();
        let centroAziendaleSelezionato = this.indirizziParentFormDataService.centroSelezionato;

        if (event?.action === 'add') {
          if (this.indirizziParentForm.controls['catastoAppezzamento'].value.length > 0) {
            let part = this.indirizziParentForm.controls['catastoAppezzamento'].value[0].particella;
            let prov = part.Prov;
            let com = part.Com;

            this.istatService.leggiStati().then((stati) => {
              let stato = stati.find(s => s.codice === 'IT');
              let col = this.indirizziColumns.find(s => s.field === 'codice_stato');
              fb.controls['codice_stato'].setValue(stato.codice);
              fb.controls['descrizione_stato'].setValue(stato.descrizione, { emitEvent: false });
              col.ddl.reload.next(true);

              this.istatService.leggiProvincie('').then((province) => {
                let pro = province.find(s => s.Istat_Prov === prov);
                let col = this.indirizziColumns.find(s => s.field === 'prov');
                fb.controls['prov'].setValue(pro.Istat_Prov, { emitEvent: false });
                fb.controls['PROVINCIA'].setValue(pro.Provincia_Des, { emitEvent: false });
                col.ddl.reload.next(true);

                this.istatService.leggiComuni(prov).then((comuni) => {
                  let comune = comuni.find(s => s.codice === com);
                  let col = this.indirizziColumns.find(s => s.field === 'com');
                  fb.controls['com'].setValue(comune.codice);
                  fb.controls['COMUNE'].setValue(comune.descrizione, { emitEvent: false });
                  col.ddl.reload.next(true);
                });
              });
            });
          } else {
            let centroCodice: number = 0;
            let centroPiva: string = '';

            if (centroAziendaleSelezionato.primaryKey.codice === 0 || centroAziendaleSelezionato.primaryKey.partitaIva === '') {
              const objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
              centroCodice = objParametriAgenda.Sa_Cod;
              centroPiva = objParametriAgenda.Piva;
            } else {
              centroCodice = centroAziendaleSelezionato.primaryKey.codice;
              centroPiva = centroAziendaleSelezionato.primaryKey.partitaIva;
            }

            this.centriService.leggiCentroAziendale(centroPiva, centroCodice).pipe(
              take(1),
              tap((vals) => {
                if (vals.RispostaStringa.indirizzi.length > 0) {
                  let ind = vals.RispostaStringa.indirizzi[0];

                  this.istatService.leggiStati().then((stati) => {
                    let stato = stati.find(s => s.codice === ind.indirizzo.stato.codice);
                    let col = this.indirizziColumns.find(s => s.field === 'codice_stato');
                    fb.controls['codice_stato'].setValue(stato.codice);
                    fb.controls['descrizione_stato'].setValue(stato.descrizione, { emitEvent: false });
                    col.ddl.reload.next(true);

                    col = this.indirizziColumns.find(s => s.field === 'prov');
                    fb.controls['prov'].setValue(ind.indirizzo.istatComune.prov, { emitEvent: false });
                    fb.controls['PROVINCIA'].setValue(ind.indirizzo.istatComune.comuni_prov, { emitEvent: false });
                    col.ddl.reload.next(true);

                    col = this.indirizziColumns.find(s => s.field === 'com');
                    fb.controls['com'].setValue(ind.indirizzo.istatComune.com, { emitEvent: false });
                    fb.controls['COMUNE'].setValue(ind.indirizzo.istatComune.localita, { emitEvent: false });
                    col.ddl.reload.next(true);

                    fb.controls['cap'].setValue(ind.indirizzo.cap, { emitEvent: false });
                    fb.controls['frazione'].setValue(ind.indirizzo.frazione, { emitEvent: false });
                    fb.controls['via'].setValue(ind.indirizzo.via, { emitEvent: false });
                    fb.controls['note'].setValue(ind.indirizzo.note, { emitEvent: false });
                    fb.controls['tipo_indirizzo'].setValue(ind.tipo_Indirizzo, { emitEvent: false });
                  });
                }
              })
            ).GiasSubscribe(() => {
              let a = 0; //Commento per funzione vuota SonarQube
            });
          }
        }
      })
    ).GiasSubscribe((event: any) => {
      let a = 0; //Commento per funzione vuota SonarQube
    });

    this.gridPublicSerivce.formGroup.GiasSubscribe(fg => {
      if (fg != undefined) {
        from(this.istatService.leggiStati()).pipe(take(1), map(s => {
          let stato = fg.controls["codice_stato"].value;
          let gestioneGerarchia = 1;

          if (!!stato) {
            gestioneGerarchia = s.filter(t => t.codice == stato)[0].gestioneGerarchia;
          }

          if (gestioneGerarchia != 1) {
            fg.controls['com'].disable({ emitEvent: false });
            fg.controls['prov'].disable({ emitEvent: false });
            fg.controls["prov"].setValue('000', { emitEvent: false });
            fg.controls["PROVINCIA"].setValue('', { emitEvent: false });
            fg.controls["com"].setValue('000', { emitEvent: false });
            fg.controls["COMUNE"].setValue('', { emitEvent: false });
          }

          fg.controls["codice_stato"].valueChanges.GiasSubscribe(() => {
            let stato = fg.controls["codice_stato"].value;
            gestioneGerarchia = s.filter(t => t.codice == stato)[0].gestioneGerarchia;
            if (gestioneGerarchia != 1) {
              let col = this.indirizziColumns.find(s => s.field === 'prov');
              fg.controls["prov"].setValue('000', { emitEvent: false });
              fg.controls["PROVINCIA"].setValue('', { emitEvent: false });
              fg.controls['com'].disable({ emitEvent: false });
              fg.controls['prov'].disable({ emitEvent: false });
              col.ddl.reload.next(true);

              col = this.indirizziColumns.find(s => s.field === 'com');
              fg.controls["com"].setValue('000', { emitEvent: false });
              fg.controls["COMUNE"].setValue('', { emitEvent: false });
              col.ddl.reload.next(true);
            } else {
              let col = this.indirizziColumns.find(s => s.field === 'prov');
              fg.controls['com'].enable({ emitEvent: false });
              fg.controls['prov'].enable({ emitEvent: false });
              fg.controls["prov"].setValue(stato == 'IT' ? '000' : stato + '000', { emitEvent: false });
              fg.controls["PROVINCIA"].setValue('', { emitEvent: false });
              fg.controls["com"].setValue(stato == 'IT' ? '000' : stato + '000', { emitEvent: false });
              fg.controls["COMUNE"].setValue('', { emitEvent: false });
            }

            fg.controls['cap'].updateValueAndValidity({ onlySelf: true, emitEvent: false });
          });
        })).subscribe();

        fg.controls["prov"].valueChanges.GiasSubscribe(() => {
          let prov = fg.controls["prov"].value;
          if (prov != null && prov != "") {
            const formVal = fg.getRawValue();
            if (formVal.Codice <= 0) {
              fg.controls["com"].enable({ emitEvent: false });
            }
            from(this.istatService.leggiProvincie(fg.controls['codice_stato'].value)).pipe(take(1), map(p => {
              let comdef = p.filter(t => t.Istat_Prov == fg.controls['prov'].value)[0].comuneDefault;
              this.istatService.leggiComuni(prov).then((comuni) => {
                let com = comuni.find(s => s.codice === comdef);
                let col = this.indirizziColumns.find(s => s.field === 'com');
                fg.controls["com"].setValue(com.codice, { emitEvent: false });
                fg.controls["COMUNE"].setValue(com.descrizione, { emitEvent: false });
                col.ddl.reload.next(true);
              })
            })).subscribe();
          }
        });

        fg.controls["com"].valueChanges.GiasSubscribe(() => {
          let com = fg.controls["com"].value;
          let prov = fg.controls["prov"].value;
          let stato = fg.controls["codice_stato"].value;
          if (com != '' && com != '000' && prov != '' && prov != '000' && (stato == 'IT' || stato == '')) {
            this.istatService.leggiCAP(prov, com).then((cap) => {
              fg.controls['cap'].setValue(cap);
            })
          } else {
            fg.controls['cap'].setValue('00000');
          }
        });
      }
    })

  }

  read(): Observable<IndirizziServerResult> {
    for (let i = 0; i < this.indirizziColumns.length; i++) {
      this.indirizziColumns[i].editable = this.edit;
    }

    this.getRows();
    this.gridIndirizziAppezzamento.kendo_columns = this.indirizziColumns;
    this.gridIndirizziAppezzamento.kendo_model = this.indirizziModel;

    this.IndirizziServerResult = new IndirizziServerResult(this.gridIndirizziAppezzamento.kendo_rows,
      this.gridIndirizziAppezzamento.kendo_columns,
      this.gridIndirizziAppezzamento.kendo_model
    );

    let id = 0;
    this.gridIndirizziAppezzamento.kendo_rows.forEach((row) => {
      row[this.rowId] = id;
      id++;
    });

    this.handleDropdowns();

    return of(this.IndirizziServerResult);
  }

  private getRows() {
    let rows = this.indirizziAppezzamentoDataSevice.indirizziAppezzamento;
    let kendoRows = new Array();
    rows.forEach(r => {
      let row: any = {};
      row.chiave = r.chiave;
      row.cap = r.indirizzo.cap;
      row.codice = r.indirizzo.codice;
      row.frazione = r.indirizzo.frazione;
      row.com = r.indirizzo.istatComune.com;
      row.COMUNE = r.indirizzo.istatComune.localita;
      row.prov = r.indirizzo.istatComune.prov;
      row.PROVINCIA = r.indirizzo.istatComune.comuni_prov;
      row.note = r.indirizzo.note;
      row.codice_stato = r.indirizzo.stato.codice;
      row.descrizione_stato = r.indirizzo.stato.descrizione;
      row.via = r.indirizzo.via;
      row.tipo_indirizzo = r.tipo_Indirizzo;
      kendoRows.push(row);
    });
    this.gridIndirizziAppezzamento.kendo_rows = kendoRows;
  }

  perform(actionType: HttpAction, rows: Array<any>): Observable<KendoGridRow[]> {
    rows.forEach((row) => {
      this.indirizziAppezzamentoService.perform(actionType, row);
    });

    return of(rows);
  }

  handleCustomizations(): void {
    this.selectable = new AgrSelectableSettings();
    this.selectable.selectable.checkboxOnly = false;
    this.selectable.selectable.enabled = true;
    this.selectable.shouldShowCheckbox = false;

    this.behavior.pdfSettings.enabled = false;
    this.behavior.excelSettings.enabled = false;
    this.groups.groupable.enabled = false;
    this.views.enabled = false;

    this.toolbar = new ToolbarSettings(this.edit, false);

    this.cmdColumn = new CommandsColumnSettings({
      editBtn: false,
      infoBtn: false,
      removeBtn: this.edit,
      onDisableInfoBtn: () => false
    });

    this.resizable.autoFitColumns = false;
    this.resizable.isResizable = true;

    if (window.innerWidth < SMARTPHONE_WIDTH) {
      this.toolbar.newItem = false;
      this.cmdColumn.editBtn = false;
      this.groups.groupable.enabled = false;
      this.views.enabled = false;
    }
  }

  private handleDropdowns() {
    // Provincia
    let col = this.indirizziColumns.find(s => s.field === 'prov');
    let data = [];
    col.ddl = new DropdownListWithForm('Istat_Prov', 'prov', 'Provincia_Des', data);
    col.ddl.valuePrimitive = true;
    col.ddl.loadOnEdit = true;
    col.ddl.descriptionField = 'PROVINCIA';
    col.ddl.loadFunction = this.caricaProvincie.bind(this);

    // Comune
    col = this.indirizziColumns.find(s => s.field === 'com');
    data = [];
    col.ddl = new DropdownListWithForm('codice', 'com', 'descrizione', data);
    col.ddl.valuePrimitive = true;
    col.ddl.loadOnEdit = true;
    col.ddl.descriptionField = 'COMUNE';
    col.ddl.loadFunction = this.caricaComuni.bind(this);

    // Stato
    col = this.indirizziColumns.find(s => s.field === 'codice_stato');
    data = [];
    col.ddl = new DropdownListWithForm('codice', 'codice_stato', 'descrizione', data);
    col.ddl.valuePrimitive = true;
    col.ddl.loadOnEdit = true;
    col.ddl.descriptionField = 'descrizione_stato';
    col.ddl.loadFunction = this.caricaStati.bind(this);
  }

  caricaProvincie(dataItem: any) {
    return from(this.istatService.leggiProvincie(dataItem.codice_stato));
  }

  caricaComuni(dataItem: any): Observable<Comune[]> {
    if (dataItem.prov !== null && dataItem.prov != "") {
      return from(this.istatService.leggiComuni(dataItem.prov));
    } else {
      return from([]);
    }
  }

  caricaStati(dataItem: any) {
    return from(this.istatService.leggiStati());
  }
}
