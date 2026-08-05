import { Injectable } from '@angular/core';
import {BehaviorSubject, Observable, Subject} from 'rxjs';
import {
  ReadDettaglioAziendale
} from 'app/Service/RequisitiStabilimento/requisiti-stabilimento.service';

@Injectable()
export class VisualizzaDettagliService {

  private readDettaglioAziendale = new ReadDettaglioAziendale();

  private objReadDettaglioAziendale = new BehaviorSubject(this.readDettaglioAziendale);
  currentReadDettaglioAziendale: Observable<ReadDettaglioAziendale> = this.objReadDettaglioAziendale.asObservable();

  private reloadGrid = new Subject<boolean>();

  public get requisitiPayload$(): ReadDettaglioAziendale {
    return this.objReadDettaglioAziendale.getValue();
  }


  public nextRequisitiPayload(value: ReadDettaglioAziendale): void {
    this.objReadDettaglioAziendale.next(value);
  }

  public get reloadGrid$(): Observable<any> {
    return this.reloadGrid.asObservable();
  }

  public nextReloadGrid(): void {
    this.reloadGrid.next(true);
  }
}
