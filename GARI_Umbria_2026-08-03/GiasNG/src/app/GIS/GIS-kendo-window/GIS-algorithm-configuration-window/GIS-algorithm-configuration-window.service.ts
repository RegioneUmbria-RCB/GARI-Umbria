import { Injectable } from "@angular/core";
import { Subject, Observable } from "rxjs";

@Injectable()
export class AlgorithmConfigurationWindowService {
  private forceReloadSubject = new Subject<void>();

  get forceReload$(): Observable<void> {
    return this.forceReloadSubject.asObservable();
  }

  public forceReload(): void {
    this.forceReloadSubject.next()
  }
}
