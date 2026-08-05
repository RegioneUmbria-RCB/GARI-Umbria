import { Component, DestroyRef, inject, Input, OnDestroy, OnInit, SecurityContext, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import {
  Observable, audit, combineLatest, debounceTime, interval, merge, scan, skip, startWith, Subject, Subscription, take,
  takeUntil, tap, forkJoin, map, filter, distinctUntilChanged, fromEvent
} from 'rxjs';
import { GiasDropDownTemplateService, ObjParametriAgenda } from 'gias-ui-kit';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { Specie } from 'app/Model/metaschema/utilizzi/Specie';
import { GruppoFinalita } from 'app/Model/metaschema/utilizzi/GruppoFinalita';
import { AGRODATAFINE, AGRODATAINIZIO } from 'app/Model/CostantiPersonalizzate';
import { GruppoFinalitaService } from 'app/Service/Metaschema/finalita.service';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { GiasDropDownTemplateSComponent } from 'gias-ui-kit';
import { GruppoVarietaleService } from 'app/Service/Metaschema/gruppoVarietale.service';
import { UtilizzoTerreno } from 'app/Model/metaschema/utilizzi/UtilizzoTerreno';
import { Varieta } from 'app/Model/metaschema/utilizzi/Varieta';
import { FormaAllevamentoService } from 'app/Service/Metaschema/formaAllevamento.service';
import { PortinnestoService } from 'app/Service/Metaschema/portinnesto.service';
import { CoperturaService } from 'app/Service/Metaschema/copertura.service';
import { IrrigazioneService } from 'app/Service/Metaschema/irrigazione.service';
import { SeminaTrapiantoService } from 'app/Service/Metaschema/seminaTrapianto.service';
import { ProvenienzaSemeService } from 'app/Service/Metaschema/provenienzaSeme.service';
import { ConduzioneService } from 'app/Service/Metaschema/conduzioneTraSuFila.service';
import { CodificaInfoAggiuntiveService } from 'app/Service/Codifiche/codifica_InfoAggiuntive.service';
import { BaseCodeDescr } from 'app/Model/baseClass/baseCodeDescr';
import { Esercizio } from 'app/Model/anagrafiche/Esercizio';
import { DatePipe, KeyValue } from '@angular/common';
import { GiasMessageService } from "../../../Service/gias-message.service";
import { Impianto } from 'app/Model/anagrafiche/Impianto';
import { DestinazioneUso } from "../../../Model/metaschema/utilizzi/DestinazioneUso";
import { TabStripComponent } from '@progress/kendo-angular-layout';
import { DropDownFilterSettings } from "@progress/kendo-angular-dropdowns";
import { TranslocoService } from '@jsverse/transloco';
import { SharedDataService } from '../../../GIS/services/shared-data.service';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { ConversioniService } from 'app/Service/Metaschema/conversioni.service';
import { UnitaDiMisura_Alternativa } from 'app/Service/api.service';
import { NotificationRef } from '@progress/kendo-angular-notification';
import { cloneDeep } from 'lodash';
import { Irrigazione, MacchinaIrrigazione } from 'app/Model/metaschema/Irrigazione';
import { GroupResult, groupBy } from "@progress/kendo-data-query";
import { ParcoMacchine } from "app/Model/anagrafiche/ParcoMacchine";
import { IntervalloTemporale } from '../../../Model/anagrafiche/IntervalloTemporale';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  standalone: false,
  selector: 'app-impianto-edit',
  templateUrl: './impianto-edit.component.html',
  styleUrls: ['./impianto-edit.component.css'],
  providers: [GiasDropDownTemplateService]
})
export class ImpiantoEditComponent implements OnInit, OnDestroy {
  @Input() appezzamentoEditForm: FormGroup;
  @Input() impiantoEditForm: FormGroup = this.fb.group({});
  @Input() maxSup: number;

  @ViewChild('tabstripEsercizi') public stripEsercizi: TabStripComponent;

  superficieBlur$: Subject<void> = new Subject<void>();

  AGRODATA_INIZIO: Date = AGRODATAINIZIO;
  AGRODATA_FINE: Date = AGRODATAFINE;

  imagePath: SafeResourceUrl;
  imageExist: boolean = false;

  public readonly defaultItem: { codice: 0, descrizione: '' };
  public impIrrigationData: Irrigazione[] | GroupResult[] = [];
  private impIrrigationSourceData: Irrigazione[] | GroupResult[] = [];

  edit: boolean = true;

  private lastUtilizzoTerreno: UtilizzoTerreno;
  private specie: Specie;

  private signal$: Subject<void> = new Subject();
  private caricamentoFormCompletato: Subject<void> = new Subject();

  private objParametriAgenda: ObjParametriAgenda;
  private subDDL: Subscription;
  private destroyRef = inject(DestroyRef);

  private readonly FASE_INPRODUZIONE: number = 102;

  constructor(
    private gruppoFinalitaService: GruppoFinalitaService,
    private fb: FormBuilder,
    private objParametriAgendaService: ObjParametriAgendaService,
    private fcService: FunzioniComuniService,
    private irrrigazioneService: IrrigazioneService,
    private gruppoVarietaleService: GruppoVarietaleService,
    private formaAllevamentoService: FormaAllevamentoService,
    private portinnestoService: PortinnestoService,
    private datePipe: DatePipe,
    private seminaTrapiantoService: SeminaTrapiantoService,
    private provenienzaSemeService: ProvenienzaSemeService,
    private conduzioneService: ConduzioneService,
    private codificaInfoAggiuntiveService: CodificaInfoAggiuntiveService,
    private coperturaService: CoperturaService,
    private _sanitizer: DomSanitizer,
    private giasMessageService: GiasMessageService,
    private transloco: TranslocoService,
    private permessiUtenteService: PermessiUtenteService,
    private sharedDataService: SharedDataService,
    private conversioniService: ConversioniService
  ) { }

  ngOnInit() {
    this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    this.piante_calcola();
    this.initImpiantoForm();
  }

  ngOnDestroy() {
    this.signal$.next();
    this.signal$.complete();
    if (this.subDDL) {
      this.subDDL.unsubscribe();
    }
  }

  getFormArrayEsercizi() {
    return <FormArray>this.impiantoEditForm.get('esercizi');
  }

  onAddEsercizio(): void {
    const arrFormEsercizi: FormArray<any> = this.impiantoEditForm.controls['esercizi'] as FormArray;
    const lastFormEsercizio: FormGroup<any> = arrFormEsercizi.controls[arrFormEsercizi.length - 1] as FormGroup;
    const deepCopy: FormGroup<any> = this.fcService.cloneAbstractControl(lastFormEsercizio) as FormGroup;
    const validita_fine: Date = deepCopy.controls['validita'].value.fine as Date;

    if (validita_fine.toUTCString() == AGRODATAFINE.toUTCString()) {
      this.giasMessageService.warningMessage(this.transloco.translate('PerInserireEsercizio'), true);
    } else {
      this.setNewEsercizioData(deepCopy, validita_fine);
      deepCopy.controls['codici'].setValue([]);
      deepCopy.controls['lotto'].setValue('');
      deepCopy.controls['codice'].setValue(0);
      deepCopy.controls['esercizio_Chiuso'].setValue(false); // new ex. can't be created already closed

      arrFormEsercizi.push(deepCopy);

      setTimeout(() => {
        this.stripEsercizi.selectTab(arrFormEsercizi.length - 1);
      }, 100);
    }
  }

  setNewEsercizioData(esercizio: FormGroup, dataFine: Date) {
    let validita_inizio_new: Date = new Date(dataFine);
    let validita_fine_new: Date = new Date(dataFine);

    validita_inizio_new.setDate(dataFine.getDate() + 1);
    validita_fine_new.setFullYear(dataFine.getFullYear() + 1);

    (esercizio.controls['validita'] as FormGroup).controls['inizio'].setValue(validita_inizio_new);
    (esercizio.controls['validita'] as FormGroup).controls['fine'].setValue(validita_fine_new);
  }

  onRemoveEsercizioTabStrip(): void {
    let ArrTabs = this.stripEsercizi.tabs.toArray();
    let selectedIndex = ArrTabs.findIndex((el) => el.selected == true);
    const arrFormEsercizi = this.impiantoEditForm.controls['esercizi'] as FormArray;
    arrFormEsercizi.removeAt(selectedIndex);
    if (ArrTabs.length > selectedIndex && selectedIndex > 0) {
      this.stripEsercizi.selectTab(selectedIndex - 1);
    } else if (selectedIndex == 0) {
      this.stripEsercizi.selectTab(1);
    } else {
      this.stripEsercizi.selectTab(0);
    }

    this.updateDataInizioProduzione(undefined);
  }

  async openDdl(ddlEl: GiasDropDownTemplateSComponent): Promise<void> {
    switch (ddlEl.giasFormControlName) {
      case 'gruppoFinalita':
        ddlEl.listItems = await this.gruppoFinalitaService.leggi(this.specie, this.sharedDataService?.getCfgSementiAsValue());
        break;
      case 'gruppoVarietale':
        ddlEl.listItems = await this.gruppoVarietaleService.leggi(this.specie);
        break;
      case 'irrigazione':
        // ddlEl.loading = true;
        this.readIrrigationMachines()
          .pipe(
            take(1),
            map(irr => irr.map(x => {
              if (!!x.value) {
                x.value = '---  ' + this.transloco.translate(x.value) + '  ---';
              }
              return x;
            })),
          ).subscribe(irrigazioni => {
            this.impIrrigationSourceData = irrigazioni;
            this.impIrrigationData = this.impIrrigationSourceData;
          });
        break;
      case 'formaAllevamento':
        ddlEl.listItems = await this.formaAllevamentoService.leggi(this.specie);
        break;
      case 'portinnesto':
        ddlEl.listItems = await this.portinnestoService.leggi(this.specie);
        break;
      case 'seminaTrapianto':
        ddlEl.listItems = await this.seminaTrapiantoService.leggi(this.specie);
        break;
      case 'tecnicaConduzioneTraFila':
        ddlEl.listItems = await this.conduzioneService.leggiConduzioneTra(this.specie);
        break;
      case 'tecnicaConduzioneSuFila':
        ddlEl.listItems = await this.conduzioneService.leggiConduzioneSu(this.specie);
        break;
      case 'dettaglio_varieta_personalizzato':
        ddlEl.listItems = await this.codificaInfoAggiuntiveService.leggiDettaglioVarietaPersonalizzato();
        break;
      case 'codiceZona':
        ddlEl.listItems = await this.codificaInfoAggiuntiveService.leggiProdotti(this.objParametriAgenda.Piva);
        break;
      case 'provenienzaSeme':
        ddlEl.listItems = await this.provenienzaSemeService.leggi(this.specie);
        break;
      case 'copertura':
        ddlEl.listItems = await this.coperturaService.leggi(this.specie);
        break;
      case 'unitaMisuraAlternativa':
        ddlEl.listItems = await this.conversioniService.leggi_async(true);
        break;
    }
  }

  descrizioneEsercizio(esercizio: Esercizio): string {
    let impianto: Impianto = this.impiantoEditForm.getRawValue();
    let impiantoDescr: string;

    if (impianto.utilizzoTerreno.classType == "Varieta") {
      impiantoDescr = (<Varieta>impianto.utilizzoTerreno).specie.descrizione + " - " + (<Varieta>impianto.utilizzoTerreno).descrizione + " ";
    } else {
      impiantoDescr = (<DestinazioneUso>impianto.utilizzoTerreno).descrizione + " ";
    }

    if (impianto.utilizzoTerreno.codice == 0) {
      impiantoDescr = "";
    }

    let dataInizioStr = '...';
    if (this.datePipe.transform(esercizio.validita.inizio, 'longDate') != this.datePipe.transform(AGRODATAINIZIO, 'longDate')) {
      dataInizioStr = this.datePipe.transform(esercizio.validita.inizio, 'shortDate');
    }
    let dataFineStr = '...';
    if (this.datePipe.transform(esercizio.validita.fine, 'longDate') != this.datePipe.transform(AGRODATAFINE, 'longDate')) {
      dataFineStr = this.datePipe.transform(esercizio.validita.fine, 'shortDate');
    }

    return dataInizioStr + ' - ' + dataFineStr;
  }

  specieSelezionata(): boolean {
    const val: UtilizzoTerreno = this.impiantoEditForm.controls['utilizzoTerreno'].value;
    return val.classType == 'Varieta' && (<Varieta>val).specie.codice > 0;
  }

  updateDataInizioProduzione(event: { fase: number, id: number, validitaInizio: Date; }): void {
    const dataInizioProduzione: Date = this.impiantoEditForm.controls['data_Inizio_Produzione'].value;
    const eserciziInProduzione: any[] = this.getFormArrayEsercizi().value.filter(e => e.apportiMassimiMacroelementi.fase.codice === this.FASE_INPRODUZIONE);

    if (event?.fase === this.FASE_INPRODUZIONE && event?.validitaInizio < dataInizioProduzione) {
      this.impiantoEditForm.controls['data_Inizio_Produzione'].setValue(event.validitaInizio);
    } else if (eserciziInProduzione.length > 0) {
      const dates = eserciziInProduzione.flatMap(e => e.validita.inizio)
        .sort((a: Date, b: Date) => a.getTime() - b.getTime());

      let date: Date = dates.shift();
      let dateCopy = cloneDeep(date);

      this.impiantoEditForm.controls['data_Inizio_Produzione'].setValue(dateCopy ?? new Date());
    }
  }

  disableCodiceImpianto(): boolean {
    return !!this.impiantoEditForm && this.impiantoEditForm?.controls['codiceImpianto']?.value !== '' && this.impiantoEditForm?.controls['algoritmoCodifica']?.value !== '';
  }

  ageaCodes(): KeyValue<string, string>[] {
    return [
      {
        key: this.transloco.translate('Agea_idColt'),
        value: this.impiantoEditForm.controls['Agea_idColt'].value ?? ''
      }
    ];
  }

  private initImpiantoForm() {
    this.disabilitacontrolli();
    this.impiantoEditForm.controls['superficie'].updateValueAndValidity();

    if (this.impiantoEditForm.controls['utilizzoTerreno'].value &&
      this.impiantoEditForm.controls['utilizzoTerreno'].value.classType == 'Varieta'
    ) {
      this.specie = this.impiantoEditForm.controls['utilizzoTerreno'].value.specie;
    }

    if (this.impiantoEditForm.controls['immagineBase64'].value != null &&
      this.impiantoEditForm.controls['immagineBase64'].value != undefined &&
      this.impiantoEditForm.controls['immagineBase64'].value != ''
    ) {
      this.imagePath = this._sanitizer.sanitize(
        SecurityContext.URL,
        'data:image/jpg;base64,' + this.impiantoEditForm.controls['immagineBase64'].value
      );
      this.imageExist = true;
    }
    this.loadIrrigation();

    this.handleImpiantoFormChanges();
    this.caricamentoFormCompletato.pipe(takeUntil(this.signal$))
      .subscribe(() => this.onCaricamentoFormCompletato());

    interval(0).pipe(take(1)).subscribe(() => this.caricamentoFormCompletato.next());
    this.handelAltControls();
  }

  private loadIrrigation() {
    if (this.impiantoEditForm.value.macchineIrrigazione.length > 0) {
      const macCod = this.impiantoEditForm.value.macchineIrrigazione[0].codice;
      this.impiantoEditForm.controls['flagImpiantoIsMacchina'].setValue(true);
      this.impiantoEditForm.controls['usaMacchineIrrigazioneAnagrafica'].setValue(true);

      this.readIrrigationMachines()
        .pipe(
          filter(irrigazioni => irrigazioni.some(i => !!i['aggregates'])),
          map((irrigazioni: Irrigazione[] | GroupResult[]) => {
            const aggr = irrigazioni as GroupResult[];
            // Sometimes valueis already translated, sometimes it is not
            // const macchineIrrigazione = aggr.find(x => x.value == '---  appezzamento.MacchineIrrigazione  ---' ||
            //    x.value == '---  ' + this.transloco.translate(`appezzamento.MacchineIrrigazione`) + '  ---');
            const macchineIrrigazione = aggr.find(x => x.value == 'MacchineIrrigazione');
            return macchineIrrigazione ? macchineIrrigazione.items as Irrigazione[] : [];
          })
        )
        .subscribe((irrigazioni: Irrigazione[]) => {
          const found = irrigazioni.find(x => x['codice'] === macCod);
          if (found) {
            this.impiantoEditForm.controls['irrigazione'].setValue(found);
            this.impiantoEditForm.controls['macchineIrrigazione'].setValue([]); // will be reset at save
          }
        });
    }
  }

  /**
   * Filters the irrigation source data by a case-insensitive match against the `descrizione` field
   * and stores the result in `this.impIrrigationData`.
   *
   * Notes and edge cases:
   * - Matching is performed using `toLowerCase()` / `indexOf(...) !== -1`, so an empty `value`
   *   will match all items.
   * - The method expects every item to have a string `descrizione` property. If `descrizione` is
   *   `null`/`undefined` or not a string, a runtime error may occur.
   * - If `this.impIrrigationSourceData` is `undefined`, not an array, or contains unexpected types,
   *   the method may throw or produce incorrect results. Callers should ensure the source data
   *   conforms to the expected shapes (either `Irrigazione[]` or `GroupResult[]`).
   *
   * Side effects:
   * - Updates `this.impIrrigationData` with the filtered results.
   *
   * @param value - The substring to filter on (case-insensitive). An empty string will match all items.
   */
  public handleImpIrrigazioneFilter(value: string) {
    const matchValue = (str: string) => str.toLowerCase().indexOf(value.toLocaleLowerCase()) != -1;

    if (this.impIrrigationSourceData.some(x => !!x.codice) && this.impIrrigationSourceData.some(x => !!x.descrizione)) {
      this.impIrrigationData = (this.impIrrigationSourceData as Irrigazione[])
        .filter(i => matchValue(i.descrizione));
    } else {
      this.impIrrigationData = (this.impIrrigationSourceData as GroupResult[]).map(agg => {
        let items = agg.items.filter((i: Irrigazione) => matchValue(i.descrizione));
        const cpy: GroupResult = {
          items: items,
          aggregates: agg.aggregates,
          field: agg.field,
          value: agg.value
        };
        return cpy;
      }).filter(agg => agg.items.length > 0);
    }
  }

  private handleImpiantoFormChanges() {
    this.impiantoEditForm.controls['utilizzoTerreno'].valueChanges.pipe(takeUntil(this.signal$))
      .subscribe((val: UtilizzoTerreno) => this.onChangeUtilizzoTerreno(val));

    merge(
      this.impiantoEditForm.controls['su_Fila_M'].valueChanges,
      this.impiantoEditForm.controls['tra_Fila_M'].valueChanges,
      this.impiantoEditForm.controls['interbina'].valueChanges,
      this.impiantoEditForm.controls['germinabilita'].valueChanges,
      this.impiantoEditForm.controls['superficie'].valueChanges
    ).pipe(takeUntil(this.signal$))
      .subscribe(() => this.piante_calcola());

    this.impiantoEditForm.controls['superficie'].valueChanges.pipe(
      takeUntil(this.signal$),
      skip(1),
      audit(ev => this.superficieBlur$),
      scan((a, c) => this.onEditSupImp(a, c), undefined)
    ).subscribe();

    this.impiantoEditForm.controls['gruppoVarietale'].valueChanges.pipe(takeUntil(this.signal$))
      .subscribe((el: BaseCodeDescr) => this.onChangeGruppoVarietale(el));
    this.impiantoEditForm.controls['impianto_Ibrido'].valueChanges.pipe(takeUntil(this.signal$))
      .subscribe((val: boolean) => this.onChangeImpiantoIbrido(val));
    this.impiantoEditForm.controls['irrigazione'].valueChanges.pipe(takeUntil(this.signal$))
      .subscribe((val: Irrigazione) => this.onChangeIrrigazione(val));

    this.impiantoEditForm.get('validita').valueChanges.pipe(
      takeUntilDestroyed(this.destroyRef),
      filter((v: IntervalloTemporale, _) => v?.inizio == undefined || v?.fine == undefined)
    ).subscribe((v: IntervalloTemporale) => {
      const plotValidity: IntervalloTemporale = this.appezzamentoEditForm.get('validita').getRawValue();
      this.impiantoEditForm.get('validita').setValue({ inizio: v?.inizio ?? plotValidity.inizio, fine: v?.fine ?? plotValidity.fine });
    });

    this.impiantoEditForm.get('validita').valueChanges.pipe(
      takeUntilDestroyed(this.destroyRef),
      filter(() => this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB == Enum_DBTypeOperation.Write),
      filter(v => v?.inizio != undefined && v?.fine != undefined),
      distinctUntilChanged()
    ).subscribe(v => {
      const exercises: FormArray = <FormArray>this.impiantoEditForm.controls['esercizi'];
      if (exercises.length === 1) {
        const ex: FormGroup = exercises.at(0) as FormGroup;
        if (!ex.get('validita').touched) {
          ex.get('validita').setValue(v);
        }
      }
    });

    this.edit = this.objParametriAgenda.TipoOperazioneDB !== Enum_DBTypeOperation.Read;
  }

  private handelAltControls() {
    const unitaMisuraAlternativaControl = this.impiantoEditForm.get('unitaMisuraAlternativa');
    const superficieAlternativaControl = this.impiantoEditForm.get('superficieAlternativa');
    if (unitaMisuraAlternativaControl == null || superficieAlternativaControl == null) return;

    const altUdm$ = unitaMisuraAlternativaControl.valueChanges.pipe(startWith(unitaMisuraAlternativaControl.value));
    const altSup$ = superficieAlternativaControl.valueChanges.pipe(startWith(superficieAlternativaControl.value));

    combineLatest([altUdm$, altSup$]).pipe(
      takeUntil(this.signal$),
      debounceTime(250),
      filter(([udm, sup]: [UnitaDiMisura_Alternativa, number]) => !!(udm?.codice))
    ).subscribe(([udm, sup]: [UnitaDiMisura_Alternativa, number]) => {
      // round result to 4th decimal
      const newValue = Math.round(sup * udm.tassoConversione * 1000) / 1000;

      const superficieImpiantoForm = this.impiantoEditForm.controls['superficie'];
      superficieImpiantoForm?.setValue(newValue);
      superficieImpiantoForm?.markAsTouched();

      const superficieAppezzamentoForm = this.appezzamentoEditForm?.controls['superficie'];
      const idAppezzamentoForm = this.appezzamentoEditForm?.controls['primaryKey']?.value;
      if (superficieAppezzamentoForm != null && superficieAppezzamentoForm.untouched && idAppezzamentoForm != null && idAppezzamentoForm.codice == 0) {
        superficieAppezzamentoForm.setValue(newValue);
      }
    });

    altSup$.pipe(takeUntil(this.signal$)).subscribe((value: number) => {
      if (value == null || value == 0) {
        this.impiantoEditForm.controls['superficie'].enable();
      } else {
        this.impiantoEditForm.controls['superficie'].disable();
      }
    });
  }

  private readIrrigationMachines(): Observable<Irrigazione[] | GroupResult[]> {
    if (this.impiantoEditForm.value.usaMacchineIrrigazioneAnagrafica) {
      const getCategory = (x: Irrigazione | MacchinaIrrigazione) => {
        let key = x instanceof MacchinaIrrigazione ? 'MacchineIrrigazione' : 'ImpIrrigazione';
        // return '---  ' + this.transloco.translate(`${key}`) + '  ---';
        return key;
      };

      return forkJoin([
        this.irrrigazioneService.leggi(this.specie),
        this.irrrigazioneService.leggiMacchineIrrigazione(this.objParametriAgenda.Piva)
      ]).pipe(
        take(1),
        map((results) => {
          const irrigazioni: Irrigazione[] = results[0];
          const macchineIrrigazione = results[1]
            .map(x => new MacchinaIrrigazione(
              x.Mac_Cod,
              [x.tipologia, x.Ditta_Des ?? '', x.Modello, x.macchina].filter(str => str != '').join(' - ')
            ));
          let data = [...macchineIrrigazione, ...irrigazioni]
            .map(x => ({
              codice: x.codice,
              descrizione: x.descrizione,
              category: getCategory(x)
            }));
          return groupBy(data, [{ field: "category" },]) as GroupResult[];
          //.sort((a, b) => a.descrizione.localeCompare(b.descrizione));
        })
      );
    } else {
      return this.irrrigazioneService.leggi(this.specie);
    }
  }

  private onChangeIrrigazione(irrigazione: Irrigazione | MacchinaIrrigazione) {
    if (irrigazione['category'] == 'MacchineIrrigazione') {
      this.impiantoEditForm.controls['flagImpiantoIsMacchina'].setValue(true);
    } else {
      this.impiantoEditForm.controls['flagImpiantoIsMacchina'].setValue(false);
    }
  }

  private async specieChanged(specie: Specie) {
    this.specie = specie;

    if (this.specie.codice == 0) {
      this.impiantoEditForm.get('gruppoFinalita').setValue({ codice: 0, descrizione: "" });
    } else {
      const finalita_arr = await this.gruppoFinalitaService.leggi(specie, this.sharedDataService?.getCfgSementiAsValue());
      let finalita: GruppoFinalita = { codice: 0, descrizione: '', specieCod: this.specie.codice };
      if (finalita_arr.length > 0) {
        let defaultFinalita: GruppoFinalita;
        if (this.permessiUtenteService.getImpostazione_Utente(201)) {
          defaultFinalita = finalita_arr.find(el => el.codice == parseInt(this.permessiUtenteService.getImpostazione_Utente(201).Valore))
        }
        if (defaultFinalita) {
          finalita = defaultFinalita;
        } else {
          finalita = finalita_arr[0];
        }
      }
      this.impiantoEditForm.get('gruppoFinalita').setValue({ codice: finalita.codice, descrizione: finalita.descrizione });

    }

    this.impiantoEditForm.get('gruppoVarietale').setValue({ codice: 0, descrizione: '' });
    this.impiantoEditForm.get('irrigazione').setValue({ codice: 0, descrizione: '' });
    this.impiantoEditForm.get('formaAllevamento').setValue({ codice: 0, descrizione: '' });
    this.impiantoEditForm.get('portinnesto').setValue({ codice: 0, descrizione: '' });
    this.impiantoEditForm.get('seminaTrapianto').setValue({ codice: 0, descrizione: '' });
    this.impiantoEditForm.get('provenienzaSeme').setValue({ codice: 0, descrizione: '' });
    this.impiantoEditForm.get('tecnicaConduzioneTraFila').setValue({ codice: 0, descrizione: '' });
    this.impiantoEditForm.get('tecnicaConduzioneSuFila').setValue({ codice: 0, descrizione: '' });
    this.impiantoEditForm.get('dettaglio_varieta_personalizzato').setValue({ codice: 0, descrizione: '' });
    this.impiantoEditForm.get('codiceZona').setValue({ codice: '', descrizione: '' });
    this.impiantoEditForm.get('copertura').setValue({ codice: 0, descrizione: '' });
  }

  private piante_calcola(): void {
    const distanza_suFila = this.impiantoEditForm.get('su_Fila_M').value;
    const distanza_traFila = this.impiantoEditForm.get('tra_Fila_M').value;
    const interbina = this.impiantoEditForm.get('interbina').value;
    const germinabilita = this.impiantoEditForm.get('germinabilita').value;
    const superficie = this.impiantoEditForm.get('superficie').value;

    let flag = true;
    // Controllo se i campi richiesti sono stati riempiti
    if (distanza_suFila == 0) {
      flag = false;
    }
    if (distanza_traFila == 0) {
      flag = false;
    }

    if (flag) {

      let denominatore: number;

      if (interbina > 0) {
        denominatore = Math.abs(distanza_traFila - interbina) * distanza_suFila;
      } else {
        denominatore = distanza_suFila * distanza_traFila;
      }

      let PianteHa: number;
      if (germinabilita != 0 && germinabilita != undefined && germinabilita != null) {
        PianteHa = 10000 / denominatore * (germinabilita / 100);
      } else {
        PianteHa = 10000 / denominatore;
      }

      if (PianteHa == Infinity) {
        this.impiantoEditForm.get('piante_Ha')?.setValue(null);
        this.impiantoEditForm.get('piante_Impianto')?.setValue(null);
        return;
      }
      const PianteImpianto: number = PianteHa * superficie;

      this.impiantoEditForm.get('piante_Ha')?.setValue(parseFloat(PianteHa.toFixed(0)));
      this.impiantoEditForm.get('piante_Impianto')?.setValue(parseFloat(PianteImpianto.toFixed(0)));
    } else {
      this.impiantoEditForm.get('piante_Ha')?.setValue(null);
      this.impiantoEditForm.get('piante_Impianto')?.setValue(null);
    }
  }

  private disabilitacontrolli() {
    // Se sono in Lettura disabilito tutti i controlli
    if (this.objParametriAgenda.TipoOperazioneDB === Enum_DBTypeOperation.Read) {
      this.impiantoEditForm.disable();
    }

    if (this.impiantoEditForm.controls['codiceImpianto'].value !== '' && this.impiantoEditForm.controls['algoritmoCodifica'].value !== '') {
      this.impiantoEditForm.controls['codiceImpianto'].disable();
    }
  }

  private onChangeUtilizzoTerreno(utilizzoTerreno: UtilizzoTerreno) {
    if (this.lastUtilizzoTerreno == null) {
      this.lastUtilizzoTerreno = utilizzoTerreno;
      return;
    }
    if (this.lastUtilizzoTerreno.classType != utilizzoTerreno.classType) {
      if (utilizzoTerreno.classType == 'Varieta') {
        this.specieChanged((<Varieta>utilizzoTerreno).specie);
      } else {
        this.specieChanged({ codice: 0, descrizione: '' });
      }
    } else if (utilizzoTerreno.classType == 'Varieta') {
      if ((<Varieta>this.lastUtilizzoTerreno).specie.codice != (<Varieta>utilizzoTerreno).specie.codice) {
        this.specieChanged((<Varieta>utilizzoTerreno).specie);
      }
    }
    this.lastUtilizzoTerreno = utilizzoTerreno;
  }

  private onChangeGruppoVarietale(el: BaseCodeDescr) {
    if (el.codice < 0) {
      this.impiantoEditForm.controls['impianto_Ibrido'].setValue(true);
    } else {
      this.impiantoEditForm.controls['impianto_Ibrido'].setValue(false);
    }
  }

  private onChangeImpiantoIbrido(val: boolean) {
    if (val == false) {
      this.impiantoEditForm.controls['codBMBDBT_F'].setValue('');
      this.impiantoEditForm.controls['genetica_F'].setValue('');
      this.impiantoEditForm.controls['offType_F'].setValue('');
      this.impiantoEditForm.controls['distanzaSuFila_F'].setValue(0);
      this.impiantoEditForm.controls['distanzaTraFila_F'].setValue(0);
    }
  }

  private onCaricamentoFormCompletato() {
    let impianto: Impianto = this.impiantoEditForm.getRawValue();
    let index = impianto.esercizi.findIndex((imp: Esercizio) => imp.codice == this.objParametriAgenda.Progetto_Cod);
    if (index >= 0) {
      this.stripEsercizi.selectTab(index);
    } else {
      this.stripEsercizi.selectTab(0);
    }
  }

  private onEditSupImp(previousNotification: NotificationRef, value: number): NotificationRef {
    const result: { sup: number, readFromAppezzamento: boolean } = this.extractSuperficieGis();

    if (!!result.sup && result.sup !== 0) {
      let prefix: string;
      const diff: number = Number.parseFloat((value - result.sup).toFixed(4));
      const percentage: number = Number.parseFloat(((diff / result.sup) * 100).toFixed(4));

      if (result.readFromAppezzamento) {
        prefix = `${this.transloco.translate('SuperficiePoligonoAppezzamentoAssociato')} ${result.sup} [Ha],  `;
      } else {
        prefix = `${this.transloco.translate('SuperficiePoligonoAssociato')} ${result.sup} [Ha],  `;
      }

      const diffStr: string = `${this.transloco.translate('Differenza')}: ${diff > 0 ? '+' : ''}${diff} [Ha],  `;
      const percStr: string = `${this.transloco.translate('DifferenzaPercentuale')}: ${diff > 0 ? '+' : ''}${percentage} %`;
      if (diff >= 0.0001) {
        previousNotification?.hide();
        return this.giasMessageService.warningMessage(prefix + diffStr + percStr, false, false, undefined, 10000);
      }
    }
  }

  /** Extract the associated polygon surface and tells if it was read from the Impianto or from the Appezzamento */
  private extractSuperficieGis(): { sup: number, readFromAppezzamento: boolean } {
    let superficieGis: number = Number.parseFloat(this.impiantoEditForm?.controls['superficieGis']?.value?.toFixed(4));

    // check if implant doesn't have a polygon associated or if it is a new one
    const readFromAppezzamento: boolean = superficieGis === 0 || this.impiantoEditForm?.controls['primaryKey']?.value?.codice === 0;
    if (readFromAppezzamento) {
      superficieGis = Number.parseFloat(this.appezzamentoEditForm?.controls['superficieGis']?.value?.toFixed(4));
    }

    return { sup: superficieGis, readFromAppezzamento: readFromAppezzamento };
  }
}
