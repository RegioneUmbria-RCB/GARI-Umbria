import { CodiciNazioniISO3166 } from '../../metaschema/CodiciNazioniISO3166';
import { Istat } from '../../metaschema/Istat';
import {LatLng} from '../../GIS/Utility';

export class Indirizzo {
    codice: number;
    via: string;
    frazione: string;
    istatComune: Istat;
    cap: string;
    stato: CodiciNazioniISO3166;
    note: string;
    flag_cancellazione: boolean;
    geolocation: Required<LatLng> = {lat: 0, lng: 0, isValid: false};
}
export class IndirizzoDefault {
    CAP: string;
    Cod_Indirizzo: string;
    Comune: string;
    Indirizzo: string;
    IndirizzoCompleto: string;
    Localita: string;
    Provincia: string;
    Provincia_Sigla: string;
    Stato: string
}
