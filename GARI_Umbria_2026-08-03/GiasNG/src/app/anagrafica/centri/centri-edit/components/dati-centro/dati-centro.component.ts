import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { FormArray, FormGroup, FormGroupDirective } from '@angular/forms';
import { RubricaVociConChiave } from 'app/Model/anagrafiche/RubricaVoci';
import { ImpreseFactoryService, IMPRESE_SERVICE_TOKEN } from 'app/Service/ServiceFactory/imprese.factory.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { GRID_HTTP_TOKEN, KendoServerResult } from 'gias-kendo-grid';
import { AbstractGridConfigService } from 'gias-kendo-grid';
import { GridPublicService } from 'gias-kendo-grid';
import { generateGridProviders } from 'gias-kendo-grid';
import { from, map, Subject, take, takeUntil, tap, Observable, catchError, of } from 'rxjs';
import { EditCentroStore } from '../../services/centri-store.service';
import { RubricaConfigService } from '../../services/grids/rubrica-config.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { Comune, Provincia } from 'app/Model/MetaschemaModel';
import { CodiciNazioniISO3166 } from 'app/Model/metaschema/CodiciNazioniISO3166';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { TranslocoService } from '@jsverse/transloco';
import { MasterService } from 'app/Service/master.service';
import { ConfigurazioneSitiService } from 'app/Service/configurazione-siti.service';
import { HttpClient } from '@angular/common/http';
import {GiasIstatService} from '../../../../../Service/istat/gias-istat.service';

@Component({
  standalone: false,
  selector: 'dati-centro',
  templateUrl: './dati-centro.component.html',
  styleUrls: ['./dati-centro.component.scss'],
  providers: [...generateGridProviders(RubricaConfigService, DatiCentroComponent)]
})
export class DatiCentroComponent implements OnInit, OnDestroy {
  @Input() isFormDisabled: boolean;

  datiFormGroup: FormGroup;
  istatComune: FormGroup;
  indirizzo: FormGroup;

  provincie: Provincia[];
  comune: Comune[];
  stati: CodiciNazioniISO3166[];

  italiaSelezionata: boolean = true;
  apiLoaded: Observable<boolean>;

  get indirizzi() {
    let arr = this.datiFormGroup.controls['indirizzi'] as FormArray;
    return arr;
  }

  private signal: Subject<void> = new Subject();
  private messaggioErrore: string;
  private messaggiCampoNonVuoto: string;
  private firstChange: boolean = true;
  private geocoder: google.maps.Geocoder;

  constructor(
    public store: EditCentroStore,
    private rootFormGroup: FormGroupDirective,
    @Inject(GRID_HTTP_TOKEN) public conf: AbstractGridConfigService<KendoServerResult>,
    private objParametriAgendaService: ObjParametriAgendaService,
    private istatService: GiasIstatService,
    @Inject(IMPRESE_SERVICE_TOKEN) private impreseService: ImpreseFactoryService,
    rubricaPubService: GridPublicService,
    private giasDialogService: GiasDialogService,
    private translocoService: TranslocoService,
    private masterService: MasterService,
    private configurazioneSitiService: ConfigurazioneSitiService,
    httpClient: HttpClient
  ) {
    this.messaggiCampoNonVuoto = this.translocoService.translate('IlCampoNonPuòEssereVuoto');

    let gridService = conf as RubricaConfigService;
    this.store.rubricaPubService = rubricaPubService;

    store.rubricaDataReady.pipe(take(1)).subscribe(s => {
      gridService.rubricaReady.next({ rubricaVoci: (s.rubrica as RubricaVociConChiave[]) });
    });

    if (this.masterService.gmapsApiLoaded == false) {
      this.configurazioneSitiService.currentConfigurazione_Siti
        .pipe(take(1))
        .subscribe(cs => {
          let urlWithApiKey = '';
          let csGoogleMaps = cs.filter(cs_ => cs_.Chiave === 'googlemaps');

          if (csGoogleMaps.length > 0) {
            urlWithApiKey = csGoogleMaps[0].Valore;
          }

          let url: string;
          if (urlWithApiKey !== '') {
            url = urlWithApiKey + ',places';
          } else {
            url = 'https://maps.googleapis.com/maps/api/js?libraries=drawing,geometry,places';
          }

          this.apiLoaded = httpClient.jsonp(url, 'callback')
            .pipe(
              map(() => true),
              tap(() => this.masterService.gmapsApiLoaded = true),
              catchError(() => of(false)),
            );
        });
    } else {
      this.apiLoaded = of(true);
    }
  }

  ngOnInit(): void {
    this.datiFormGroup = this.rootFormGroup.form as FormGroup;
    this.indirizzo = (this.datiFormGroup.get('indirizzi') as FormArray).at(0).get('indirizzo') as FormGroup;
    this.istatComune = this.indirizzo.get('istatComune') as FormGroup;

    this.indirizzo.valueChanges.pipe(takeUntil(this.signal)).subscribe(indirizzo => {
      let a = 0; //Commento per funzione vuota SonarQube
    });

    if (this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB === Enum_DBTypeOperation.Write) {
      this.initializeFields();
    }

    this.indirizzo.controls["stato"].valueChanges.subscribe((value) => {
      this.onCountryChange(value);
    });

    this.istatComune.controls['prov'].valueChanges.subscribe((value: Provincia) => {
      this.onProvChange(value);
    });

    this.istatComune.controls['com'].valueChanges.subscribe((value: Comune) => {
      this.onComChange(value);
    });
  }

  ngOnDestroy(): void {
    this.signal.next();
    this.signal.complete();
  }

  onClickGeolocalizza() {
    const titolo: string = this.translocoService.translate('centro.Geolocalizzazione');
    this.validazioneCampiGeolocalizza();

    if (this.messaggioErrore === null && this.masterService.gmapsApiLoaded === false) {
      this.messaggioErrore = this.translocoService.translate('GeolocalizzazioneNonSupportata');
    }

    if (this.messaggioErrore !== null) {
      this.giasDialogService.baseError(titolo, this.messaggioErrore, false);
    } else {
      const indirizzo = this.componiIndirizzoCentro();
      this.geocoder = new google.maps.Geocoder();

      this.geocoder.geocode(
        {'address': indirizzo},
        function(results, status) {
          if (status === 'OK') {
            const lat = results[0].geometry.location.lat();
            const lng = results[0].geometry.location.lng();
            this.proponiCoordinateCentro(lat,lng);
          } else {
            let messaggio = this.translocoService.translate('GeolocalizzazioneNonAvvenuta');
            let errore = this.translocoService.translate("Errore_");
            let messaggioErroreGeolocalizza: string = `<strong>${messaggio}</strong><br>${errore}: ${status}`;
            this.giasDialogService.baseError(titolo, messaggioErroreGeolocalizza, false);
          }
        }.bind(this)
      );
    }
  }

  private initializeFields() {
    const objP = this.objParametriAgendaService.getObjParamValue();
    from(this.impreseService.leggiImpresa(objP)).pipe(
      take(1),
      map((val) => {
        return val.RispostaStringa;
      }),
      tap((impresa) => {
        const indirizzo = impresa.indirizzi[0].indirizzo;

        const prov: string = indirizzo.istatComune.prov;
        const com: string = indirizzo.istatComune.com;

        this.istatService.leggiProvincie('').then(lp => {
          if(!this.store.inModifica) {
            this.indirizzo.get('istatComune.prov').setValue(prov);
            this.indirizzo.get('istatComune.com').setValue(com);
          }
        });
      })
    ).subscribe();
  }

  private validazioneCampiGeolocalizza() {
    this.messaggioErrore = null;

    if (this.indirizzo.controls['via'].value === '') {
      const nomeCampo = this.translocoService.translate('centro.Via');
      this.componiMessaggioErroreGeolocalizza(nomeCampo);
    }

    if (this.indirizzo.controls['cap'].value === '') {
      const nomeCampo = this.translocoService.translate('centro.CAP');
      this.componiMessaggioErroreGeolocalizza(nomeCampo);
    }
  }

  private componiMessaggioErroreGeolocalizza(nomeCampo: string) {
    if (this.messaggioErrore === null) {
      this.messaggioErrore = `${this.messaggiCampoNonVuoto} : ${nomeCampo}`
    } else {
      this.messaggioErrore += `, ${nomeCampo}`
    }
  }

  private componiIndirizzoCentro(): string {
    const via = this.indirizzo.controls['via'].value;
    const frazione = this.indirizzo.controls['frazione'].value;
    const desProvincia = this.istatComune.controls["comuni_prov"].value;
    const objComune = this.istatComune.value;
    const cap = this.indirizzo.controls['cap'].value;
    const objStato: CodiciNazioniISO3166 = <CodiciNazioniISO3166>this.indirizzo.controls['stato'].value;
    let indirizzoCentro: string;

    if (this.italiaSelezionata) {
      indirizzoCentro = `${via},${cap},${desProvincia}`;

      if (objComune.descrizione !== '') {
        indirizzoCentro += `,${objComune.localita}`
      }

      if (objStato.descrizione !== '') {
        indirizzoCentro += `,${objStato.descrizione}`
      }
    } else {
      indirizzoCentro = `${via},${frazione},${objStato.descrizione}`;
    }

    return indirizzoCentro;
  }

  private proponiCoordinateCentro(lat: number, lng: number): void {
    this.datiFormGroup.controls['lat'].setValue(lat);
    this.datiFormGroup.controls['lng'].setValue(lng);
  }

  private onCountryChange(value): void {
    from(this.istatService.leggiStati()).pipe(take(1), map(s => {
      this.stati = s;
      const country: CodiciNazioniISO3166 = this.stati.find(s => s.codice == value.codice);
      const prefix: string = value.codice == 'KE' ? value.codice : '';

      if(!this.firstChange || country.gestioneGerarchia != 1) {
        this.istatComune.controls["com"].setValue(value.codice == 'IT' ? '000' : prefix + '000', { emitEvent: false });
        this.istatComune.controls["comuni_prov"].setValue('', { emitEvent: false });
        this.istatComune.controls["prov"].setValue(value.codice == 'IT' ? '000' : prefix + '000', { emitEvent: false });
        this.indirizzo.controls['cap'].setValue('00000', { emitEvent: false });
      }

      if (country.gestioneGerarchia != 1) {
        this.italiaSelezionata = false;
        this.firstChange = false;
        return;
      } else {
        this.firstChange = false;
        const com = this.istatComune.get('com').value;
        const prov = this.istatComune.get('prov').value;

        from(this.istatService.leggiProvincie(value.codice)).pipe(take(1), map(pp => {
          this.provincie = pp;
          let provincia: Provincia = this.provincie.find(p => p.Istat_Prov == prov);

          from(this.istatService.leggiComuni(provincia.Istat_Prov)).pipe(
            take(1),
            tap(c => {
              this.comune = c;
              this.istatComune.controls['com'].setValue(this.comune.find(c => c.codice == com), {emitEvent: false});
              this.istatComune.controls['prov'].setValue(provincia.Istat_Prov, {emitEvent: false});
            })).subscribe();
        })).subscribe();

        if (!this.store.inModifica) {
          this.indirizzo.get('istatComune.prov').setValue(prov);
          this.indirizzo.get('istatComune.com').setValue(com);
        }
      }

      this.italiaSelezionata = true;
    })).subscribe();
  }

  private onProvChange(value: Provincia): void {
    if (value.Istat_Prov) {
      from(this.istatService.leggiComuni(value.Istat_Prov)).pipe(take(1), map(c => {
        this.comune = c;
        this.istatComune.controls['com'].setValue(value.comuneDefault, {emitEvent: false});
        this.istatComune.controls['comuni_prov'].setValue(value.Provincia_Des, {emitEvent: false});
        this.istatComune.controls['prov'].setValue(value.Istat_Prov, {emitEvent: false});
        this.istatComune.controls['localita'].setValue(this.comune.find(c => c.codice == value.comuneDefault).descrizione, {emitEvent: false});

        from(this.istatService.leggiCAP(value.Istat_Prov, value.comuneDefault)).pipe(take(1), map(c => {
          this.istatComune.controls['cap'].setValue(c, {emitEvent: false});
          this.indirizzo.controls['cap'].setValue(c, {emitEvent: false});
        })).subscribe();
      })).subscribe();
    }
  }

  private onComChange(value: Comune | string): void {
    const comCod: string = typeof value === 'string' ? value : value?.codice;

    if (!!comCod && comCod !== '') {
      this.istatComune.controls['com'].setValue(comCod, {emitEvent: false});
      this.istatComune.controls['localita'].setValue(value['descrizione'] ?? '', {emitEvent: false});

      from(this.istatService.leggiCAP(this.istatComune.get('prov').value, comCod)).pipe(take(1), map(c => {
        this.istatComune.controls['cap'].setValue(c, {emitEvent: false});
        this.indirizzo.controls['cap'].setValue(c, {emitEvent: false});
      })).subscribe();
    } else {
      this.indirizzo.controls['cap'].setValue('00000');
    }
  }
}
