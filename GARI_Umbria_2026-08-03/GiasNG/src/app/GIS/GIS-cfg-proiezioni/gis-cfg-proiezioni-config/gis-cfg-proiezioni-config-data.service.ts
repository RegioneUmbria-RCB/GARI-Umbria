import { Injectable } from '@angular/core';
import {BehaviorSubject} from 'rxjs';
import {ConfigurazioneProiezione} from '../../../Service/api.service';

@Injectable()
export class GISCfgProiezioniConfigDataService {
  private _cfg$: BehaviorSubject<string> = new BehaviorSubject<string>('');
  constructor() { }

  public get cfg(): string {
    return this._cfg$.getValue();
  }

  public set cfg(cfg: string) {
    this._cfg$.next(cfg);
  }
}
