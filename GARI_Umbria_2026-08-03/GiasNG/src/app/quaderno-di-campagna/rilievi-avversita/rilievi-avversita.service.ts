import { Injectable } from '@angular/core';
import { Disciplinare, Specie } from 'app/Service/api.service';
import { BehaviorSubject, Observable, Subject } from 'rxjs';

@Injectable()
export class RilieviAvversitaService {
  private specieSubject = new BehaviorSubject<Specie | null>(null);
  private disciplinareSubject = new BehaviorSubject<Disciplinare | null>(null);
  private avversitaSubject = new BehaviorSubject<Avversita | null>(null);
  private reloadSubject = new Subject<void>();

  get specie$(): Observable<Specie | null> {
    return this.specieSubject.asObservable();
  }

  public nextSpecie(specie: Specie | null): void {
    this.specieSubject.next(specie);
  }

  get disciplinare$(): Observable<Disciplinare | null> {
    return this.disciplinareSubject.asObservable();
  }

  public nextDisciplinare(disciplinare: Disciplinare | null): void {
    this.disciplinareSubject.next(disciplinare);
  }

  get avversita$(): Observable<Avversita | null> {
    return this.avversitaSubject.asObservable();
  }

  public nextAvversita(avversita: Avversita | null): void {
    this.avversitaSubject.next(avversita);
  }

  get reload$(): Observable<void> {
    return this.reloadSubject.asObservable();
  }

  public nextReload(): void {
    this.reloadSubject.next();
  }
}

export interface Avversita {
  cod: string;
  des: string;
  soglia: string;
  MxAV_Cod: string;
}