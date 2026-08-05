import {AfterViewInit, Component, DestroyRef, forwardRef, inject, Input, input, OnInit, output} from '@angular/core';
import {
  ControlContainer,
  FormControl,
  FormGroup,
  FormGroupDirective,
  NG_VALUE_ACCESSOR, Validators
} from '@angular/forms';
import {catchError, combineLatest, map, of, ReplaySubject, take, tap} from 'rxjs';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {CodiciNazioniISO3166} from '../../../../Model/metaschema/CodiciNazioniISO3166';
import {Comune, Provincia} from '../../../../Model/MetaschemaModel';
import {GiasIstatService} from '../../../../Service/istat/gias-istat.service';
import {GiasDropDownTemplateSComponent, GiiasMultiselectTemplateSComponent} from 'gias-ui-kit';
import {LatLng} from '../../../../Model/GIS/Utility';
import {GiasDialogService} from '../../../../Service/gias-dialog.service';
import {TranslocoService} from '@jsverse/transloco';
import {MasterService} from '../../../../Service/master.service';
import {ConfigurazioneSitiService} from '../../../../Service/configurazione-siti.service';
import {HttpClient} from '@angular/common/http';

class AddressFG {
  country: FormControl<CodiciNazioniISO3166> = new FormControl<CodiciNazioniISO3166>(new CodiciNazioniISO3166(
    '','','','',0)
  );
  province: FormControl<Provincia> = new FormControl<Provincia>(new Provincia());
  city: FormControl<Comune> = new FormControl<Comune>(new Comune());
  fraction: FormControl<string> = new FormControl<string>('');
  street: FormControl<string> = new FormControl<string>('');
  zipCode: FormControl<string> = new FormControl<string>('');
  note: FormControl<string> = new FormControl<string>('');
  lat: FormControl<number> = new FormControl<number>(0, [Validators.min(-90), Validators.max(90)]);
  lng: FormControl<number> = new FormControl<number>(0, [Validators.min(-180), Validators.max(180)]);
}

@Component({
  standalone: false,
  selector: 'gias-address-template',
  templateUrl: './gias-address-template.component.html',
  styleUrl: './gias-address-template.component.css',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => GiasAddressTemplateComponent),
      multi: true
    }
  ],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }]
})
export class GiasAddressTemplateComponent implements OnInit, AfterViewInit {
  readonly city = input<Comune>();
  readonly province = input<Provincia>();
  readonly country = input<CodiciNazioniISO3166>();
  readonly zipCode = input<string>();
  readonly street = input<string>();
  readonly fraction = input<string>();
  readonly note = input<string>();
  readonly geolocation = input<LatLng>();
  @Input() showGeolocation: (...args) => boolean = () => false;

  cityOut = output<Comune>();
  provinceOut = output<Provincia>();
  countryOut = output<CodiciNazioniISO3166>();
  zipCodeOut = output<string>();
  streetOut = output<string>();
  fractionOut = output<string>();
  noteOut = output<string>();
  geolocationOut = output<LatLng>();

  protected addressForm: FormGroup<AddressFG> = new FormGroup<AddressFG>(new AddressFG());
  protected countries$: ReplaySubject<CodiciNazioniISO3166[]> = new ReplaySubject<CodiciNazioniISO3166[]>(1);
  protected province$: ReplaySubject<Provincia[]> = new ReplaySubject<Provincia[]>(1);
  protected cities$: ReplaySubject<Comune[]> = new ReplaySubject<Comune[]>(1);

  private _destroyRef = inject(DestroyRef);

  constructor(
    private configurazioneSitiService: ConfigurazioneSitiService,
    private giasDialogService: GiasDialogService,
    private istatService: GiasIstatService,
    private masterService: MasterService,
    private transloco: TranslocoService,
    private httpClient: HttpClient
  ) {  }

  ngOnInit(): void {
    this.addressForm.controls.note.valueChanges.pipe(takeUntilDestroyed(this._destroyRef)).subscribe((v) => {
      this.noteOut.emit(v);
    });
    this.addressForm.controls.zipCode.valueChanges.pipe(takeUntilDestroyed(this._destroyRef)).subscribe((v) => {
      this.zipCodeOut.emit(v);
    });
    this.addressForm.controls.street.valueChanges.pipe(takeUntilDestroyed(this._destroyRef)).subscribe((v) => {
      this.streetOut.emit(v);
    });
    this.addressForm.controls.fraction.valueChanges.pipe(takeUntilDestroyed(this._destroyRef)).subscribe((v) => {
      this.fractionOut.emit(v);
    });
    this.addressForm.controls.country.valueChanges.pipe(takeUntilDestroyed(this._destroyRef)).subscribe((v) => {
      this.onCountryChance(v);
      this.countryOut.emit(v);
    });
    this.addressForm.controls.province.valueChanges.pipe(takeUntilDestroyed(this._destroyRef)).subscribe((v) => {
      this.onProvinceChance(v);
      this.provinceOut.emit(v);
    });
    this.addressForm.controls.city.valueChanges.pipe(takeUntilDestroyed(this._destroyRef)).subscribe((v) => {
      this.onCityChange(v);
      this.cityOut.emit(v);
    });
    combineLatest([this.addressForm.controls.lat.valueChanges, this.addressForm.controls.lng.valueChanges]).pipe(
      takeUntilDestroyed(this._destroyRef)
    ).subscribe((g: number[]) => {
      this.geolocationOut.emit({lat: g[0], lng: g[1], isValid: true});
    });

    this.addressForm.controls.note.setValue(this.note(), {emitEvent: false})
    this.addressForm.controls.zipCode.setValue(this.zipCode(), {emitEvent: false});
    this.addressForm.controls.street.setValue(this.street(), {emitEvent: false});
    this.addressForm.controls.fraction.setValue(this.fraction(), {emitEvent: false});
    this.addressForm.controls.country.setValue(this.country(), {emitEvent: false});
    this.addressForm.controls.province.setValue(this.province(), {emitEvent: false});
    this.addressForm.controls.city.setValue(this.city(), {emitEvent: false});
    this.addressForm.controls.lat.setValue(this.geolocation().lat);
    this.addressForm.controls.lng.setValue(this.geolocation().lng);
  }

  ngAfterViewInit(): void {
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

          this.httpClient.jsonp(url, 'callback')
            .pipe(
              take(1),
              map((v) => {
                console.log(v);
                return true;
              }),
              tap((v) => {
                console.log(v);
                this.masterService.gmapsApiLoaded = true;
              }),
              catchError((e) => {
                console.log(e);
                return of(false);
              }),
            ).subscribe();
        });
    }
  }

  showProvince(): boolean {
    return this.addressForm.controls.country.value?.gestioneGerarchia === 1;
  }

  showCity(): boolean {
    return this.addressForm.controls.country.value?.gestioneGerarchia === 1;
  }

  onCountryChance(v): void {
    const country: CodiciNazioniISO3166 = this.addressForm.controls.country.value;
    this.resetProvince();
    this.resetCity();

    if (!(country.gestioneGerarchia === 0)) {
      this.province$.next([]);
      this.cities$.next([]);

      this.istatService.readProvinces(this.addressForm.controls.country.value.codice).pipe(
        take(1)
      ).subscribe(r => this.province$.next(r));
    }
  }

  onProvinceChance(v): void {
    this.istatService.readCities(this.addressForm.controls.province.value.Istat_Prov).pipe(
      take(1)
    ).subscribe(r => {
      this.cities$.next(r);
      const defaultCity: Comune = r.find(c => c.codice === this.addressForm.controls.province.value.comuneDefault) ?? new Comune();
      this.addressForm.controls.city.setValue(defaultCity ?? new Comune());
      this.cityOut.emit(defaultCity);
    });
  }

  onCityChange(v): void {
    this.istatService.readZipCode(
      this.addressForm.controls.province.value.Istat_Prov,
      this.addressForm.controls.city.value.codice
    ).pipe(take(1)).subscribe(r => {
      this.addressForm.controls.zipCode.setValue(r);
      this.zipCodeOut.emit(r);
    });
  }

  openDdl(ddl: GiasDropDownTemplateSComponent | GiiasMultiselectTemplateSComponent): void {
    switch (ddl.giasFormControlName) {
      case 'country':
          ddl.loading = true;

          this.istatService.readCountries().pipe(
            take(1)
          ).subscribe({
            error: () => ddl.loading = false,
            next: r => this.countries$.next(r),
            complete: () => ddl.loading = false
          });
        break;
      case 'province':
        ddl.loading = true;

        this.istatService.readProvinces(this.addressForm.controls.country.value.codice).pipe(
          take(1)
        ).subscribe({
          error: () => ddl.loading = false,
          next: r => this.province$.next(r),
          complete: () => ddl.loading = false
        });
        break;
      case 'city':
        ddl.loading = true;

        this.istatService.readCities(this.addressForm.controls.province.value.Istat_Prov).pipe(
          take(1)
        ).subscribe({
          error: () => ddl.loading = false,
          next: r => {
            this.cities$.next(r)
          },
          complete: () => ddl.loading = false
        });
        break;
      default:
        ddl.listItems = [];
        break;
    }
  }

  geolocate() {
    let geocoder: google.maps.Geocoder = new google.maps.Geocoder();
    const formValue = this.addressForm.getRawValue();

    let addr: string = `${formValue.street}, ${formValue.fraction}, ${formValue.zipCode}, ${formValue.city.descrizione}, ${formValue.province.Provincia_Des}, ${formValue.country.descrizione}`;

    geocoder.geocode(
      {'address': addr},
      function(results, status) {
        if (status === 'OK') {
          const lat = results[0].geometry.location.lat();
          const lng = results[0].geometry.location.lng();
          this.addressForm.controls.lat.setValue(lat);
          this.addressForm.controls.lng.setValue(lng);
        } else {
          let messaggio: string = this.transloco.translate('GeolocalizzazioneNonAvvenuta');
          let messaggioErroreGeolocalizza: string = `<strong>${messaggio}</strong>`;
          this.giasDialogService.baseError(
            this.transloco.translate('Geolocalizzazione'),
            messaggioErroreGeolocalizza,
            false
          );
        }
      }.bind(this)
    );
  }

  private resetProvince(): void {
    const country: CodiciNazioniISO3166 = this.addressForm.controls.country.value;
    const prefix: string = country.codice == 'KE' ? country.codice : '';

    let province: Provincia = new Provincia();
    province.Istat_Prov = `${prefix}000`;
    this.addressForm.controls.province.setValue(province, {emitEvent: false});
    this.provinceOut.emit(province);
  }

  private resetCity(): void {
    const country: CodiciNazioniISO3166 = this.addressForm.controls.country.value;
    const prefix: string = country.codice == 'KE' ? country.codice : '';

    let province: Provincia = new Provincia();
    province.Istat_Prov = `${prefix}000`;

    let city: Comune = new Comune();
    city.codice = `${prefix}000`;
    city.provincia = province;
    this.addressForm.controls.city.setValue(city, {emitEvent: false});
    this.cityOut.emit(city);
  }
}
