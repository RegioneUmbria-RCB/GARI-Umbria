import {Component, OnInit} from '@angular/core';
import {TranslocoService} from "@jsverse/transloco";
import {FormArray, FormControl, FormGroup} from "@angular/forms";
import {BaseCodeDescr} from "../../../../Model/baseClass/baseCodeDescr";
import {ProfilazioneDataShareService} from "../../../services/profilazione-data-share.service";
import {faMagnifyingGlass} from "@fortawesome/free-solid-svg-icons";
import {MasterService} from "../../../../Service/master.service";
import {ProfilazioneUtentiService} from "../../../services/profilazione-utenti.service";
import {GiasDialogService} from "../../../../Service/gias-dialog.service";
import {PermessiUtenteService} from "../../../../Service/permessi-utente.service";
import {enum_Security_Attivita} from "../../../../Model/TipiEnumerativi";
import { enum_TipoControllo } from 'gias-ui-kit';

export enum enum_TipoFiltro {
  CONTAINS = 0,
  STARTS_WITH = 1, // used also as greater-equal for dates
  ENDS_WITH = 2,   // used also as less than-equal for dates
  EQUALS = 3
}

@Component({
  standalone: false,
  selector: 'app-filtri-utente',
  templateUrl: './filtri-utente.component.html',
  styleUrls: ['./filtri-utente.component.css']
})
export class FiltriUtenteComponent implements OnInit {
  protected permessoAmmUtenti = false;
  protected filtersForm: FormGroup;
  protected filterTypesList: BaseCodeDescr[];
  protected readonly faSearch = faMagnifyingGlass;
  protected readonly textControlType = enum_TipoControllo.CASELLA_TESTO;
  protected readonly dateControlType = enum_TipoControllo.CALENDARIO;
  private readonly filterableFields: BaseCodeDescr[] = [
    new BaseCodeDescr(0, 'firstTime'),
    new BaseCodeDescr(enum_TipoControllo.CALENDARIO, 'Data_Creazione'),
    new BaseCodeDescr(enum_TipoControllo.CASELLA_TESTO, 'Username'),
    new BaseCodeDescr(enum_TipoControllo.CASELLA_TESTO, 'Nome'),
    new BaseCodeDescr(enum_TipoControllo.CASELLA_TESTO, 'Cognome'),
    new BaseCodeDescr(enum_TipoControllo.CASELLA_TESTO, 'CodFisc'),
    new BaseCodeDescr(enum_TipoControllo.CASELLA_TESTO, 'Email'),
    new BaseCodeDescr(enum_TipoControllo.CASELLA_TESTO, 'UsernameCommerciale'),
    new BaseCodeDescr(enum_TipoControllo.CASELLA_TESTO, 'Tipologia_Des'),
    new BaseCodeDescr(enum_TipoControllo.CASELLA_TESTO, 'GruppiDes'),
    new BaseCodeDescr(enum_TipoControllo.CALENDARIO, 'Data_Modifica'),
    new BaseCodeDescr(enum_TipoControllo.CALENDARIO, 'UltimoAccesso')
  ];
  private readonly toHide = [
    'firstTime', 'Data_Creazione',
  ];


  constructor(
    private master: MasterService,
    private transloco: TranslocoService,
    private dialog: GiasDialogService,
    private pdsService: ProfilazioneDataShareService,
    private utentiService: ProfilazioneUtentiService,
    private permessi: PermessiUtenteService
  ) {
    this.permessoAmmUtenti = this.permessi.canReadPermesso(enum_Security_Attivita.AmministrazioneUtenti);
  }

  /** Visible filters properties. */
  public get filterableProperties() {
    return (this.filtersForm.get('ctrls') as FormArray).controls.filter(c => !this.toHide.includes(c.value.field));
  }

  public get filters() {
    return this.filtersForm.value;
  }

  protected get panelTitle(): string {
    return this.transloco.translate('prof.FiltriUtente');
  }

  ngOnInit(): void {
    if (this.pdsService.loadUsersFilters) {
      this.filtersForm = this.pdsService.loadUsersFilters;
    } else {
      this.initForm();
      this.pdsService.loadUsersFilters = this.filtersForm;
      this.pdsService.filterService = this;
    }
    this.filterTypesList = [
      new BaseCodeDescr(enum_TipoFiltro.CONTAINS, this.transloco.translate("filterContainsOperator")),
      new BaseCodeDescr(enum_TipoFiltro.STARTS_WITH, this.transloco.translate("filterStartsWithOperator")),
      new BaseCodeDescr(enum_TipoFiltro.ENDS_WITH, this.transloco.translate("filterEndsWithOperator")),
      new BaseCodeDescr(enum_TipoFiltro.EQUALS, this.transloco.translate("filterEqOperator")),
    ];
  }

  search() {
    this.preventIfPendingChanges(() => {
      this.pdsService.removeUserDateFilter();
      this.utentiService.userReloaded$.next(true);
    });
  }

  resetFilters() {
    this.setCtrlValue('Username', enum_TipoFiltro.EQUALS, this.master.objP_utenti.UtenteUsername);
    this.setCtrlValue('firstTime', enum_TipoFiltro.EQUALS, 'true');
  }

  protected getCtrlLabel(ctrl: FormGroup) {
    return this.transloco.translate('prof.UserFilters' + ctrl.get('field').value);
  }

  private initForm() {
    const fArray = new FormArray([]);
    this.filterableFields.forEach((field: BaseCodeDescr) => fArray.push(new FormGroup({
      field: new FormControl(field.descrizione),
      filterType: new FormControl(enum_TipoFiltro.CONTAINS),
      value: new FormControl(''),
      valueAlt: new FormControl(''),
      controlType: new FormControl(field.codice)
    })));
    this.filtersForm = new FormGroup({ctrls: fArray});
    this.resetFilters();
  }

  private setCtrlValue(field: string, filterType: enum_TipoFiltro, value: string) {
    const fArray = this.filtersForm.get('ctrls') as FormArray;
    let absCtrl = fArray.controls.find(ctrl => ctrl.get('field').value === field);
    if (absCtrl) {
      absCtrl.get('filterType').patchValue(filterType);
      absCtrl.get('value').patchValue(value);
    }
  }

  private preventIfPendingChanges(action: Function) {
    if (this.pdsService.usersGrid && this.pdsService.usersGrid.gridServices.incellHasChanges()) {
      this.dialog.baseError('prof.ModificheSospese', 'prof.SalvaModifichePerContinuare');
    } else {
      action.call(null);
    }
  }
}
