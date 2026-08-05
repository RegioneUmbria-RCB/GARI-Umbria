import { Injectable } from '@angular/core';
import { TipologiaLayer } from 'app/Service/api.service';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable()
export class GISLayerAdvancedSettingsWindowService {
  private layerSelected = new BehaviorSubject<TipologiaLayer | null>(null);

  public selectLayer(layer: TipologiaLayer | null): void {
    this.layerSelected.next(layer);
  }

  public get layerSelected$(): Observable<TipologiaLayer | null> {
    return this.layerSelected.asObservable();
  }

  public get currentLayerSelected(): TipologiaLayer | null {
    return this.layerSelected.value;
  }
}