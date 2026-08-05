import { Component, ElementRef, Inject, ViewChild, OnDestroy } from '@angular/core';
import { ImpreseService } from 'app/Service/Anagrafica/imprese.service';
import { LOADING_TOKEN, LoadingService } from 'gias-ui-kit';
import { ErroreGias } from 'app/Service/master.service';
import { ObjParametriAgenda } from 'gias-ui-kit';
import { Subject, takeUntil, tap } from 'rxjs';
import { TestAnagraficaService } from '../test-anagrafica.service';

@Component({
  standalone: false,
  selector: 'app-test-anagrafica-imprese',
  templateUrl: './test-anagrafica-imprese.component.html',
  styleUrls: ['./test-anagrafica-imprese.component.css'],
  providers: [{ provide: LOADING_TOKEN, useClass: LoadingService }]
})
export class TestAnagraficaImpreseComponent implements OnDestroy {

    @ViewChild('component', { static: false }) component: ElementRef;
    public erroriGias: ErroreGias[];
    private signal$: Subject<void> = new Subject();

    constructor(public testAnagraficaService: TestAnagraficaService,
        public impreseService: ImpreseService,
        @Inject(LOADING_TOKEN) private loadingService: LoadingService) { }

    ngOnDestroy(): void {
        this.signal$.next();
        this.signal$.complete();
    }

    testLettura() {
        this.loadingService.set_isLoading({ isLoading: true, component: this.component });
        var objAgenda = new ObjParametriAgenda();
        objAgenda.Piva = this.testAnagraficaService.PivaSelezionata
        this.impreseService.testLettura(objAgenda).pipe(
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
        this.impreseService.testScrittura(objAgenda).pipe(
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
