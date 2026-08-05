import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class MappaMultiAziendaService {

    private mappaProntaSubject = new Subject<number>();

    mappaProntaObservable: Observable<number> = this.mappaProntaSubject.asObservable();

    signalMappaPronta() {
        this.mappaProntaSubject.next(0);
    } 

}