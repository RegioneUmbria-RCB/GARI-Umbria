import { Injectable } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { AGRODATAFINE, AGRODATAINIZIO } from '../Model/CostantiPersonalizzate';
import {Observable, ReplaySubject, Subject} from 'rxjs';
import { ReadRequisitiStabilimento } from 'app/Service/RequisitiStabilimento/requisiti-stabilimento.service';

@Injectable()
export class RequisitiStabilimentoService {

  private requisitiPayloadSubject = new ReplaySubject<ReadRequisitiStabilimento>(1);
  private resultSubject = new ReplaySubject<any[]>(1);
  private contractsSubject = new ReplaySubject<any[]>(1);

  private reloadGrid = new Subject<boolean>();

  constructor(private fb: FormBuilder) { }

  public getFiltriForm(): FormGroup {
    return this.fb.group({
      validita: this.fb.group({
        inizio: [AGRODATAINIZIO],
        fine: [AGRODATAFINE]
      }),
      specie: new FormControl({ codice: 0, descrizione: '' }),
      prodotto: new FormControl({ codice: 0, descrizione: '' }),
      assegnazione: [false],
      budget: new FormControl({ Id_Budget: 0, Nome_Budget: '', Sa_Cod: 0 }),
      cooperativa: new FormControl({ Cod_RisUm: 0, Rag_Soc_Completa: '', CodContatto: '' })
    });
  }

  public get requisitiPayload$(): Observable<ReadRequisitiStabilimento> {
    return this.requisitiPayloadSubject.asObservable();
  }

  public nextRequisitiPayload(value: ReadRequisitiStabilimento): void {
    this.requisitiPayloadSubject.next(value);
  }

  public get result$(): Observable<any[]> {
    return this.resultSubject.asObservable();
  }

  public get reloadGrid$(): Observable<any> {
    return this.reloadGrid.asObservable();
  }

  public nextResult(value: any[]): void {
    this.resultSubject.next(value);
  }

  public get contracts$(): Observable<any[]> {
    return this.contractsSubject.asObservable();
  }

  public nextContracts(value: any[]): void {
    this.contractsSubject.next(value);
  }

  public nextReloadGrid(): void {
    this.reloadGrid.next(true);
  }
}

export interface RequisitiStabilimentoModel {
  PIVA: string;
  rag_soc: string;
  cuaa: string;
  Piva_Padre: string;
  Rag_Soc_Padre: string;
  GruppoRaccolta_Cod: number;
  GruppoRaccolta_Des: string;
  Mat_Cod: number;
  Mat_Des: string;
  Sup: number;
  Superficie: number;
  ResaPrevista: number;
}

export interface RequisitiStabilimentoContrattoModel {
  Piva: string;
  Contratto_Nome: string;
  Azienda: string;
  Contratto_Cod: number;
  Fase_Cod: number;
  Cod_Risum: number;
  Rag_Soc: string;
  Mat_Cod: number;
  Mat_Des: string;
  Superficie: number;
  QtaPrevista: number;
  ResaPrevista: number;
  Validita_Inizio: string;
  Validita_Fine: string;
}
