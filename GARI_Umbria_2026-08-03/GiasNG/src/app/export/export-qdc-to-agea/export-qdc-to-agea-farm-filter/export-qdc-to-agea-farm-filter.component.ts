import {AfterViewInit, Component, EventEmitter, Input, OnDestroy, OnInit, Output, SimpleChanges} from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Impresa } from 'app/Model/anagrafiche/Impresa';
import { FarmFilters, ImpreseService } from 'app/Service/Anagrafica/imprese.service';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { catchError, map, of, tap, Observable, debounceTime, switchMap, filter, merge, Subscription, finalize } from 'rxjs';
import { gearIcon } from '@progress/kendo-svg-icons'
import { skip } from 'rxjs';
import { saveAs } from 'file-saver';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';

@Component({
  standalone: false,
  selector: 'app-export-qdc-to-agea-farm-filter',
  templateUrl: './export-qdc-to-agea-farm-filter.component.html',
  styleUrls: ['./export-qdc-to-agea-farm-filter.component.css'],
  providers: [GiasDropDownTemplateService]
})
export class ExportQdcToAgeaFarmFilterComponent implements OnInit, OnDestroy,AfterViewInit {

  gearIcon = gearIcon;

  @Input() impresaSelected: Impresa | null = null;
  @Input() yearControl: FormControl;
  @Output() onImpresaSelected = new EventEmitter<Impresa>();

  form = new FormGroup({
    campaignYear: new FormControl<number>(new Date().getFullYear()),
    withoutBundles: new FormControl<boolean>(false),
    afterDate: new FormControl<Date | null>(null),
    beforeDate: new FormControl<Date | null>(null),
    withoutSubmissions: new FormControl<boolean>(false),
    submissionError: new FormControl<boolean>(false),
    submissionCompleted: new FormControl<boolean>(false),
    loadImpreseFirstTime: new FormControl<boolean>(false)
  });

  loading = false;
  settingsDialogOpen = false;
  data$: Observable<Impresa[]> = this.form.valueChanges
    .pipe(
      debounceTime(100),
      tap(() => this.loading = true),
      switchMap(filters => {
        const payload = { ...filters, withBundles: !filters.withoutBundles } as FarmFilters;
        return this.impreseService.LeggiImpreseConFiltroUtenteHubAgea(payload);
      }),
      catchError(() => of([] as Impresa[])),
      map(farms => {
        const list = farms ?? [] as Impresa[];
        const headerPiva = this.objParametriAgendaService.getObjParamValue()?.Piva;
        const headerRagSoc = this.objParametriAgendaService.getObjParamValue()?.RagSoc;
        if (headerPiva && !list.some(f => f.partitaIva === headerPiva)) {
          const headerImpresa = new Impresa(headerPiva);
          headerImpresa.ragioneSociale = headerRagSoc;
          return [headerImpresa, ...list];
        }
        return list;
      }),
      tap((farms) => {

        let index = farms.findIndex(imp=>imp.partitaIva === this.impresaSelected?.partitaIva);

        let imp: Impresa = null;

        if(index > -1){
          imp = farms[index];
        }else if(farms.length === 1){
          imp = farms[0];
        }

        this.onImpresaSelected.emit(imp);

        this.loading = false
      }),
    );

  formListeners$ = merge(
    this.form.controls.withoutBundles.valueChanges
      .pipe(tap(value => this.toggleControls([
        this.form.controls.afterDate,
        this.form.controls.beforeDate,
        this.form.controls.withoutSubmissions,
        this.form.controls.submissionError,
        this.form.controls.submissionCompleted
      ], value))),

    this.form.controls.withoutSubmissions.valueChanges
      .pipe(tap(value => this.toggleControls([
        this.form.controls.submissionError,
        this.form.controls.submissionCompleted
      ], value))),

    this.form.controls.submissionError.valueChanges
      .pipe(tap(value => this.toggleControls([
        this.form.controls.submissionCompleted
      ], value))),

    this.form.controls.submissionCompleted.valueChanges
      .pipe(tap(value => this.toggleControls([
        this.form.controls.submissionError,
      ], value)))
  );

  private yearSubscription: Subscription;

  constructor(private impreseService: ImpreseService, private objParametriAgendaService: ObjParametriAgendaService) { }

  ngOnInit(): void {
    if (this.yearControl == null)
      throw new Error("Year control cannot be null");

    this.yearSubscription = this.yearControl.valueChanges
      .pipe(
        skip(1),
        tap(value => this.form.controls.campaignYear.setValue(value))
      ).subscribe();
  }

  ngOnDestroy(): void {
    this.yearSubscription?.unsubscribe();
    this.yearSubscription = null;
  }

  exportExcel(): void {
    const filters = this.form.getRawValue();
    const payload = { ...filters, withBundles: !filters.withoutBundles } as FarmFilters;
    this.impreseService
      .exportImpreseExcel(payload)
      .subscribe(resp => {
        let binaryData: Uint8Array = resp.Data;
        let filename = resp.FileName;

        const blob = new Blob([this.base64ToArrayBuffer(binaryData)], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
        saveAs(blob, filename);
    });
  }

  private toggleControls(controls: FormControl[], disable: boolean): void {
    for (const control of controls) {
      if (disable) {
        control.setValue(typeof control.value == 'boolean' ? false : null, { emitEvent: false });
        control.disable({ emitEvent: false });
      } else {
        control.enable({ emitEvent: false });
      }
    }
  }

  private base64ToArrayBuffer(base64) {
    let binaryString = window.atob(base64);
    let binaryLen = binaryString.length;
    let bytes = new Uint8Array(binaryLen);
    for (let i = 0; i < binaryLen; i++) {
       let ascii = binaryString.charCodeAt(i);
       bytes[i] = ascii;
    }
    return bytes;
 }

 ngAfterViewInit() {
   this.form.get("loadImpreseFirstTime").setValue(true,{emitEvent: true});
 }

}
