import { Injectable } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { BehaviorSubject, Observable, of } from 'rxjs';
import {CentroAziendale} from "../../../../Model/anagrafiche/CentroAziendale";

@Injectable()
export class IndirizziParentFormDataService{
  private _indirizziParentFormSource = new BehaviorSubject<FormGroup<any>>(null);

  private _centroSelezionato: CentroAziendale = new CentroAziendale({codice: 0, partitaIva: ''})
  private _centroSelezionatoSource = new BehaviorSubject(this._centroSelezionato);

  constructor() {  }

  get indirizziParentForm$(): Observable<FormGroup<any>> {
    return this._indirizziParentFormSource.asObservable();
  }

  get indirizziParentForm(): FormGroup {
    return this._indirizziParentFormSource.getValue();
  }

  set indirizziParentForm(parent: FormGroup) {
    this._indirizziParentFormSource.next(parent);
  }

  set centroSelezionato(centro: CentroAziendale) {
    this._centroSelezionatoSource.next(centro);
  }

  get centroSelezionato() {
    return this._centroSelezionatoSource.getValue();
  }
}
