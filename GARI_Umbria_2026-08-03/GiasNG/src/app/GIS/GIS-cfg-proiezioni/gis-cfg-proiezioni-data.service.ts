import { Injectable } from '@angular/core';
import {BehaviorSubject, Observable} from 'rxjs';
import {ConfigurazioneProiezione} from '../../Service/api.service';

@Injectable({
  providedIn: 'root'
})
export class GISCfgProiezioniDataService {
  private _selectedConfiguration$: BehaviorSubject<ConfigurazioneProiezione> = new BehaviorSubject<ConfigurazioneProiezione>(null);
  private _openConfigurationEditDialog$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(null);
  private _openPermissionDialog$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(null);
  private _openCfgDialog$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(null);
  constructor() { }

  public get selectedConfiguration(): ConfigurazioneProiezione {
    return this._selectedConfiguration$.getValue();
  }

  public set selectedConfiguration(cfg: ConfigurazioneProiezione) {
    this._selectedConfiguration$.next(cfg);
  }

  public get openConfigurationEditDialog(): boolean {
    return this._openConfigurationEditDialog$.getValue();
  }

  public set openConfigurationEditDialog(value: boolean) {
    this._openConfigurationEditDialog$.next(value);
  }

  public get openPermissionDialog(): boolean {
    return this._openPermissionDialog$.getValue();
  }

  public set openPermissionDialog(value: boolean) {
    this._openPermissionDialog$.next(value);
  }

  public get openCfgDialog(): boolean {
    return this._openCfgDialog$.getValue();
  }

  public set openCfgDialog(value: boolean) {
    this._openCfgDialog$.next(value);
  }
}
