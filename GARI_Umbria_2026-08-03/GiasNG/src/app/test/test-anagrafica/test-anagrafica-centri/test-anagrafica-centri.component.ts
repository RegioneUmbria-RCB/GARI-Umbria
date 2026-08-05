import { Component, ElementRef, Inject, ViewChild, OnDestroy } from '@angular/core';
import { CentriAziendaliService } from 'app/Service/Anagrafica/centri.service';
import { LOADING_TOKEN, LoadingService } from 'gias-ui-kit';
import { ErroreGias } from 'app/Service/master.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { Subject, takeUntil, tap } from 'rxjs';
import { TestAnagraficaService } from '../test-anagrafica.service';

@Component({
  standalone: false,
  selector: 'app-test-anagrafica-centri',
  templateUrl: './test-anagrafica-centri.component.html',
  styleUrls: ['./test-anagrafica-centri.component.css'],
  providers: [{ provide: LOADING_TOKEN, useClass: LoadingService }]
})
export class TestAnagraficaCentriComponent implements OnDestroy {

    @ViewChild('component', { static: false }) component: ElementRef;
    public erroriGias: ErroreGias[];
    private signal$: Subject<void> = new Subject();

    constructor(public testAnagraficaService: TestAnagraficaService,
        public centriService: CentriAziendaliService,
        @Inject(LOADING_TOKEN) private loadingService: LoadingService) { }

    ngOnDestroy(): void {
        this.signal$.next();
        this.signal$.complete();
    }

    testLettura() {
        this.loadingService.set_isLoading({ isLoading: true, component: this.component });
        var objAgenda = new ObjParametriAgenda();
        objAgenda.Piva = this.testAnagraficaService.PivaSelezionata
        this.centriService.testLettura(objAgenda).pipe(
            takeUntil(this.signal$),
            tap((r) => {
                if (!r.RispostaStringa) {
                    this.erroriGias = r.ErroriGias;
                } else {
                    this.erroriGias = [];
                }
                this.loadingService.set_isLoading({ isLoading: false, component: this.component });
            })
        ).subscribe();
    }

    testScrittura() {
        this.loadingService.set_isLoading({ isLoading: true, component: this.component });
        var objAgenda = new ObjParametriAgenda();
        objAgenda.Piva = this.testAnagraficaService.PivaSelezionata
        this.centriService.testScrittura(objAgenda).pipe(
            takeUntil(this.signal$),
            tap((r) => {
                if (!r.RispostaOK) {
                    this.erroriGias = r.ErroriGias;
                } else {
                    this.erroriGias = [];
                }
                this.loadingService.set_isLoading({ isLoading: false, component: this.component });
            })
        ).subscribe();
    }

}
