import { Injectable } from '@angular/core';
import { TipologiaLayer } from 'app/Service/api.service';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable()
export class GISLayerPermissionsWindowService {
  private layerSelected = new BehaviorSubject<TipologiaLayer | null>(null);
  private remainingAuths = 0;

  public selectLayer(layer: TipologiaLayer | null): void {
    this.remainingAuths = 0;
    this.layerSelected.next(layer);
  }

  public addRemainingAuths(remainingAuths: number): void {
    this.remainingAuths += remainingAuths;
  }

  public get layerSelected$(): Observable<TipologiaLayer | null> {
    return this.layerSelected.asObservable();
  }

  public get currentLayerSelected(): TipologiaLayer | null {
    return this.layerSelected.value;
  }

  public get currentRemainingAuths(): number | null {
    return this.remainingAuths;
  }
}
