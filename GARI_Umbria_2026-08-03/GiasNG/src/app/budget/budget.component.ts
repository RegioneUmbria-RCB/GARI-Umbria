import { Component, Injector, NgZone, OnDestroy, OnInit } from '@angular/core';

import { AGRODATAINIZIO, SMARTPHONE_WIDTH } from 'app/Model/CostantiPersonalizzate';
import { Subject } from 'rxjs';
import { Router } from '@angular/router';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { FormBuilder } from '@angular/forms';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { GestioneRichiesteService } from 'app/Service/gestione-richieste.service';
import { TranslocoPipe, TranslocoService } from '@jsverse/transloco';
import { BudgetService } from 'app/Service/Budget/budget.service';
import {MenuContestualeService} from '../Master/menu-contestuale/menu-contestuale.service';
import {BudgetTestata} from 'app/Model/budget/budget.testata';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { DialogCloseResult } from '@progress/kendo-angular-dialog';
import { NavigationService } from 'app/Service/navigation.service';

@Component({
    standalone: false,
    selector: 'gias-budget',
    templateUrl: './budget.component.html',
    styleUrls: ['./budget.component.scss'],
})
/** Budget component*/
export class BudgetComponent implements OnInit, OnDestroy {
    public AGRODATA_INIZIO = AGRODATAINIZIO;
    public signal$: Subject<void> = new Subject();
    budgetName: string;
    budgetId: number;
    private piva: string;
    activeBudgetSubscription: any

    public windowHtml = window;
    SMARTPHONE_WIDTH = SMARTPHONE_WIDTH;

    constructor(
        private router: Router,
        private permessiUtenteService: PermessiUtenteService,
        private zone: NgZone,
        private fb: FormBuilder,
        private budgetService: BudgetService,
        private injector: Injector,
        private gestioneRichiesteService: GestioneRichiesteService,
        private transloco: TranslocoService,
        private translocopipe: TranslocoPipe,
        private menuContestualeService: MenuContestualeService,
        private giasDialogService: GiasDialogService,
        private parametriAgenda: ObjParametriAgendaService,
        private navigation: NavigationService
    ) {
        this.activeBudgetSubscription = this.budgetService.leggiBudgetAttivo().subscribe((activeBudget: BudgetTestata) => {
            this.budgetName = activeBudget.Nome_Budget;
            this.budgetId = activeBudget.Id_Budget
            this.piva = activeBudget.Piva;
            this.budgetService.changeBudget({ budgetId: activeBudget.Id_Budget, activeBudget: true, piva: activeBudget.Piva })
            if(this.budgetId == 0 || this.budgetId == undefined) {
                this.parametriAgenda.getObjParamValue().Piva ? this.messaggioErroreBudget(true) : this.messaggioErroreBudget(false);
            }
        });
    }

    WithoutTime(dateTime): Date {
        const date = new Date(dateTime.getTime());
        date.setHours(0, 0, 0, 0);
        return date;
    }

    ngOnDestroy(): void {
        this.budgetService.changeBudget({ budgetId: 0, activeBudget: false, piva: '' });
        this.signal$.next();
        this.signal$.complete();
    }

    ngOnInit(): void {
        this.menuContestualeService.changeMenuContestualeSettings({
            show: true,
            background_color: "#FF6699",
            color: 'white',
            title: this.transloco.translate('AnagraficheColturaliBudget'),
            search: true,
            bookmarks: true,
            contextualMenu: true,
            IDTipoSezione: 2,
            IDSezionePadre: 251
        })
    }

    private messaggioErroreBudget(pivaPresente: boolean): void{
        this.giasDialogService.dialogMessageObs_Result(
            '',
            pivaPresente ? this.transloco.translate('NonSonoPresentiBudgetAssegnati'): this.transloco.translate('UtenteNonSelezionato'),
            [
                {text: this.transloco.translate('Ok'), primary: true, returnObj: true}
            ],
            undefined,
            undefined,
            e => e instanceof DialogCloseResult
        ).subscribe(
            () => {
                this.navigation.home();
                return;
            }
        );
    }
}
