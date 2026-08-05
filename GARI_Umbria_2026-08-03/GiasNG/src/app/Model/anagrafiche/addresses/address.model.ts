import {IndirizzoAssociato} from './IndirizzoAssociato';
import {Indirizzo} from './Indirizzo';
import {Istat} from '../../metaschema/Istat';
import {CodiciNazioniISO3166} from '../../metaschema/CodiciNazioniISO3166';
import {Comune, Provincia} from '../../MetaschemaModel';
import {LatLng} from '../../GIS/Utility';

export class AddressModel {
  get id(): number {
    return this._id;
  }

  set id(value: number) {
    this._id = value;
  }

  get type(): number {
    return this._type;
  }

  set type(value: number) {
    this._type = value;
  }
  get zipCode(): string {
    return this._zipCode;
  }

  set zipCode(value: string) {
    this._zipCode = value;
  }

  get street(): string {
    return this._street;
  }

  set street(value: string) {
    this._street = value;
  }

  get fraction(): string {
    return this._fraction;
  }

  set fraction(value: string) {
    this._fraction = value;
  }

  get country(): CodiciNazioniISO3166 {
    return this._country;
  }

  set country(value: CodiciNazioniISO3166) {
    this._country = value;
  }

  get province(): Provincia {
    return this._province;
  }

  set province(value: Provincia) {
    this._province = value;
  }

  get city(): Comune {
    return this._city;
  }

  set city(value: Comune) {
    this._city = value;
  }
  get note(): string {
    return this._note;
  }

  set note(value: string) {
    this._note = value;
  }

  get geolocation(): LatLng {
    return this._geolocation;
  }

  set gelocation(value: Required<LatLng>) {
    this._geolocation = value;
  }

  private _id: number;
  private _note: string;
  private _zipCode: string;
  private _street: string;
  private _fraction: string;
  private _country: CodiciNazioniISO3166;
  private _province: Provincia;
  private _city: Comune;
  private _type: number;
  private _geolocation: Required<LatLng>;

  constructor(
    code?: number,
    note?: string,
    zipCode?: string,
    street?: string,
    fraction?: string,
    country?: CodiciNazioniISO3166,
    province?: Provincia,
    city?: Comune,
    type?: number,
    geolocation?: Required<LatLng>
  ) {
    this._id = code ?? 0;
    this._note = note ?? '';
    this._zipCode = zipCode ?? '';
    this._street = street ?? '';
    this._fraction = fraction ?? '';
    this._country = country ?? new CodiciNazioniISO3166('', '', '', '', 0);
    this._province = province ?? new Provincia();
    this._city = city ?? new Comune();
    this._type = type ?? 0;
    this._geolocation = geolocation ?? {lat: 0, lng: 0, isValid: false};

    this._city.provincia = this._province;
  }

  static fromIndirizzoAssociato(addr: IndirizzoAssociato): AddressModel {
    let note: string = addr.indirizzo.note;
    let zipCode: string = addr.indirizzo.cap;
    let street: string = addr.indirizzo.via;
    let country: CodiciNazioniISO3166 = addr.indirizzo.stato;

    let city: Comune = new Comune();
    city.codice = addr.indirizzo.istatComune.com;
    city.descrizione = addr.indirizzo.istatComune.localita;

    let province: Provincia = new Provincia();
    province.Istat_Prov = addr.indirizzo.istatComune.prov;
    province.Provincia_Des = addr.indirizzo.istatComune.comuni_prov;
    province.regione_cod = addr.indirizzo.istatComune.reg;

    let type: number = addr.tipo_Indirizzo;

    return new AddressModel(addr.indirizzo.codice, note, zipCode, street, addr.indirizzo.frazione, country, province, city, type, addr.indirizzo.geolocation);
  }

  mapToIndirizzoAsociato(): IndirizzoAssociato {
    let result: IndirizzoAssociato = new IndirizzoAssociato();
    let address: Indirizzo = new Indirizzo();
    let istat: Istat = new Istat();

    address.codice = this._id;
    address.note = this._note;
    address.cap = this._zipCode;
    address.via = this._street;
    address.stato = this._country;
    address.geolocation = this._geolocation;
    address.frazione = this._fraction;

    istat.cap = this._zipCode;
    istat.com = this._city.codice;
    istat.prov = this._province.Istat_Prov;
    istat.comuni_prov = this._province.Provincia_Des;
    istat.localita = this._city.descrizione;
    istat.reg = this._province.regione_cod;
    istat.codiceBelfiore = '';

    address.istatComune = istat;

    result.indirizzo = address;
    result.tipo_Indirizzo = this._type;
    result.flag_cancellazione = false;
    return result;
  }
}
