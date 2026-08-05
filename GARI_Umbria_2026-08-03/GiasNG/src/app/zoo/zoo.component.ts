import { AfterViewInit, Component, OnDestroy, OnInit } from "@angular/core";
import { ZooRedirectorService } from "./services/zoo-redirector.service";
import { GiasMultiSelectTemplateService } from 'gias-ui-kit';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { enum_Security_Attivita } from "../Model/TipiEnumerativi";
import { ObjParametriAgendaService } from "../Service/obj-parametri-agenda.service";
import { FormGroup } from "@angular/forms";
import { ZooOperationsFilters, ZooOperationsFiltersForm } from "./models/zoo-operations-filters.model";
import { ReplaySubject, switchMap } from "rxjs";
import { AGRODATAFINE, AGRODATAINIZIO } from "../Model/CostantiPersonalizzate";
import { ZooFiltersHelperService, StableItems } from "./services/zoo-filters-helper.service";
import { BaseCodeDescr as IBaseCodeDescr, BaseCodeDescrStr as IBaseCodeDescrStr } from "../Service/api.service";
import { CookieService } from "../Service/cookie.service";
import { SelectEvent } from "@progress/kendo-angular-layout";
import { PermessiUtenteService } from "../Service/permessi-utente.service";

@Component({
  standalone: false,
  selector: 'app-zoo',
  templateUrl: './zoo.component.html',
  styleUrls: ['./zoo.component.scss'],
  providers: [
    ZooRedirectorService, GiasDropDownTemplateService, GiasMultiSelectTemplateService
  ]
})
export class ZooComponent implements OnInit, AfterViewInit, OnDestroy {

  protected readonly hasZooReadPermission: boolean;
  protected readonly hasProtocolsReadPermission: boolean;
  protected readonly hasTherapeuticIndicationsReadPermission: boolean;
  protected readonly hasTherapiesReadPermission: boolean;
  protected searchFilters$ = new ReplaySubject<ZooOperationsFilters>(null);
  protected filters: FormGroup<ZooOperationsFiltersForm>;
  protected centers: IBaseCodeDescr[] = [];
  protected stables: StableItems[] = [];
  protected zooOperations: IBaseCodeDescrStr[] = [];

  private _selectedTab: number = 0;

  constructor(
    private cookies: CookieService,
    private permissions: PermessiUtenteService,
    private agenda: ObjParametriAgendaService,
    private zooFiltersHelper: ZooFiltersHelperService,
  ) {
    this.hasZooReadPermission = this.permissions.canReadPermesso(enum_Security_Attivita.Gest_Stalle);
    this.hasProtocolsReadPermission = this.permissions.canReadPermesso(enum_Security_Attivita.ZooProtocolliTerapeutici);
    this.hasTherapeuticIndicationsReadPermission = this.permissions.canReadPermesso(enum_Security_Attivita.ZooIndicazioniTerapeutiche);
    this.hasTherapiesReadPermission = this.permissions.canReadPermesso(enum_Security_Attivita.ZooTerapie);
  }
  ngOnDestroy(): void {
    this.zooFiltersHelper.resetFilters$.next(true);
  }

  public get isImpresaSelezionata(): boolean {
    let isImpSel = this.currentPiva != undefined && this.currentPiva != null && this.currentPiva !== '';
    return isImpSel;
  }

  private get currentPiva(): string {
    const agenda = this.agenda.getObjParamValue();
    return agenda.Piva ?? '';
  }

  ngOnInit() {
    let tab = this.cookies.getCookie('zooTab');
    if (!tab) {
      tab = '0'; // Default to the first tab if no cookie is set
      this.cookies.setCookie({ name: 'zooTab', value: tab.toString(), expireDays: 10 * 365 });
    }
    this._selectedTab = +tab;
    if (this.isImpresaSelezionata){
      this.initFilters();
      this.readSavedFilters();
    }
  }

  ngAfterViewInit() {
    this.applyFilters(true);
  }

  public onTabSelect(event: SelectEvent): void {
    this._selectedTab = event.index;
    this.cookies.setCookie({ name: 'zooTab', value: event.index.toString(), expireDays: 10 * 365 });
    this.readSavedFilters();
    this.applyFilters(false);
  }

  public isTabSelected(index: number): boolean {
    return this._selectedTab === index;
  }

  protected applyFilters(nextValue: boolean) {
    if (this.filters.controls.center.value == 0 && this.filters.controls.stable.value != '0_0') {
      this.filters.controls.center.patchValue(this.getCenterFromStable(this.filters.controls.stable.value));
    }

    let stable = 0;
    if (this.stables.length == 0) {
      if ((this.filters.controls.stable.value as string).indexOf("_") != -1) {
        stable = (this.filters.controls.stable.value as string).split("_").map(x => parseInt(x))[1];
      } else {
        stable = this.filters.controls.stable.value;
      }
    } else {
      stable = this.stables.find(s => s.key == this.filters.controls.stable.value)?.codice ?? 0;
    }

    const newData = new ZooOperationsFilters(
      this.filters.controls.center.value as number,
      stable,
      this.filters.controls.from.value as Date ?? this.zooFiltersHelper.monthStart,
      this.filters.controls.to.value as Date ?? this.zooFiltersHelper.monthEnd,
      this.filters.controls.operations.value as string[],
    );
    console.debug("applyFilters", newData);
    this.searchFilters$.next(newData);
    this.rememberFilters(this.filters);
  }

  private initFilters() {
    this.filters = this.zooFiltersHelper.getFiltersFormGroup();
    this.zooFiltersHelper.loadBusinessCenters(this.currentPiva)
      .subscribe(centers => this.centers = centers);
    this.zooFiltersHelper.loadStables(this.currentPiva, this.filters.controls.center.value)
      .subscribe(stables => this.stables = stables);
    this.zooFiltersHelper.loadZooOperations()
      .subscribe(operations => this.zooOperations = operations);

    this.filters.controls.center.valueChanges
      .pipe(switchMap(center => this.zooFiltersHelper.loadStables(this.currentPiva, center)))
      .subscribe(stables => {
        this.stables = stables;
        if (stables.length == 2){
          this.filters.controls.stable.setValue(stables[1].key);
          return;
        } else {
          this.filters.controls.stable.setValue(stables[0].key);
          return;
        }
      });
  }

  private rememberFilters(form: FormGroup<ZooOperationsFiltersForm>) {
    const newData = form.value;
    const from = newData?.from?.toISOString();
    const to = newData?.to?.toISOString();
    const selected_center = newData.center.toString();
    const _selected_stable = newData.stable ?? '0_0';
    let selected_stable = "0";
    if (_selected_stable.includes('_')) {
      selected_stable = _selected_stable.split('_')[1]; // Estraggo solo il codice stalla
    } else if (!isNaN(_selected_stable as any)) {
      selected_stable = _selected_stable;
    }
    const operations = newData.operations;
    
    let FiltersDateStartName = 'zoo_FiltersDateStart';
    let FiltersDateEndName = 'zoo_FiltersDateEnd';
    let FiltersOperationsName = 'zoo_FiltersOperations';
    let PivaName = 'zooPiva';
    let CenterName = 'zooSenter';
    let StableName = 'zooStable';

    this.cookies.setCookie({ name: FiltersDateStartName, value: from, expireDays: 10 * 365 });
    this.cookies.setCookie({ name: FiltersDateEndName, value: to, expireDays: 10 * 365 });

    //Salvataggio Cookie centri-stalla
    this.cookies.setCookie({ name: PivaName, value: this.currentPiva, expireDays: 10 * 365 });
    this.cookies.setCookie({ name: CenterName, value: selected_center, expireDays: 10 * 365 });
    this.cookies.setCookie({ name: StableName, value: selected_stable, expireDays: 10 * 365 });

    if (operations.length) {
      this.cookies.setCookie({
        name: FiltersOperationsName,
        value: operations.reduce((acc, x) => acc + "|" + x),
        expireDays: 10 * 365
      });
    } else {
      this.cookies.deleteCookie(FiltersOperationsName);
    }
  }

  private readSavedFilters() {

    let FiltersDateStartName = 'zoo_FiltersDateStart';
    let FiltersDateEndName = 'zoo_FiltersDateEnd';
    let FiltersOperationsName = 'zoo_FiltersOperations';
    let PivaName = 'zooPiva';
    let CenterName = 'zooSenter';
    let StableName = 'zooStable';

    const from = this.cookies.getCookie(FiltersDateStartName);
    const to = this.cookies.getCookie(FiltersDateEndName);
    const operations = this.cookies.getCookie(FiltersOperationsName);
    const piva: string = this.cookies.getCookie(PivaName);
    const selected_center: number = parseInt(this.cookies.getCookie(CenterName));
    const selected_stable: string = this.cookies.getCookie(StableName);
    this.filters.controls.operations.patchValue(operations.split("|").filter(x => x !== ''));
    this.filters.controls.from.patchValue(!from ? this.zooFiltersHelper.monthStart : new Date(from));
    this.filters.controls.to.patchValue(!to ? this.zooFiltersHelper.monthEnd : new Date(to));
    if (piva == this.currentPiva) {
      if (selected_center){
        this.filters.controls.center.patchValue(selected_center);
      }
      if (selected_stable){
        this.filters.controls.stable.patchValue(selected_stable);
      }
    }
  }

  private getCenterFromStable(stableKey: string): number {
    if (stableKey != undefined && typeof(stableKey) == 'string' && stableKey.includes('_')) {
      return parseInt(stableKey.split('_')[0]);
    }
    return 0;
  }

}
 