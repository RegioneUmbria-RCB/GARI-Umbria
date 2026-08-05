import { Injectable } from '@angular/core';
import { Widget_Configuration } from 'app/Service/api.service';
import { BehaviorSubject, Observable, Subject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class WidgetConfigService {
  public $widgetsBlocked = new BehaviorSubject<boolean>(false);

  private configurationSource = new Subject<Widget_Configuration[]>();
  private presetSource = new Subject<void>();

  changeConfiguration(configs: Widget_Configuration[]): void {
    this.configurationSource.next(configs);
  }

  onConfigurationChange(): Observable<Widget_Configuration[]> {
    return this.configurationSource.asObservable();
  }

  saveWidgetPreset(): void {
    this.presetSource.next();
  }

  onSaveWidgetPreset(): Observable<void> {
    return this.presetSource.asObservable();
  }
}

