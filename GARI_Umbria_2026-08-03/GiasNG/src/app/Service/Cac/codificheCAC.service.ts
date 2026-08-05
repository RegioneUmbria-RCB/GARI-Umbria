import { Injectable } from "@angular/core";
import { mergeMap as _observableMergeMap, catchError as _observableCatch } from 'rxjs/operators';
import { throwError as _observableThrow, of as _observableOf } from 'rxjs';
import { Enum_DBTypeOperation } from "gias-ui-kit";
import { CacCodiciSistema } from "app/Model/metaschema/CacCodiciSistema";
import { CacStatusCodMapping } from "app/cac-codifiche/cac-codifiche.component.model";

@Injectable({
  providedIn: 'root'
})

export class CacCodificheService {

  public CacValues:CacCodiciSistema[] = [
    { codice: CacStatusCodMapping.Agea.toString(), descrizione: 'Agea' },
    { codice: CacStatusCodMapping.Artea.toString(), descrizione: 'Artea' },
    { codice: CacStatusCodMapping.CAI.toString(), descrizione: 'CAI' },
    { codice: CacStatusCodMapping.Demetra.toString(), descrizione: 'Demetra' },
    { codice: CacStatusCodMapping.Enogis.toString(), descrizione: 'Enogis' },
    { codice: CacStatusCodMapping.Gias.toString(), descrizione: 'Gias' },
    { codice: CacStatusCodMapping.Smarttractors.toString(), descrizione: 'Smarttractors' }
  ]
    constructor() {}
  
}

export class ObjParametriCAC {
    TipoOperazioneDB: Enum_DBTypeOperation;
    ID?: number;
    Sistema_Cod?: number;
    Codice_Esterno?: string;
    Descrizione_Esterno?: string;
    Tabella_Gias?: string;
    Codice_Gias?: string;
    Data_Creazione?: Date;
    Data_Modifica?: Date;
    Username_Creazione?: string;
    Username_Modifica?: string;
    Validita_Inizio?: Date;
    Validita_Fine?: Date;
    Sistema?: string;
}
