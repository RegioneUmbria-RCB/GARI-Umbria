import { animate, style, transition, trigger } from "@angular/animations";
import { Component, OnDestroy, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { faBars, faGripLinesVertical, faSearch, faSearchPlus, faStar } from "@fortawesome/free-solid-svg-icons";
import { Enum_SiteRedirector } from "app/Model/siti.enum";
import { GestioneRichiesteService } from "app/Service/gestione-richieste.service";
import { GiasDialogService } from "app/Service/gias-dialog.service";
import { GiasMessageService } from "app/Service/gias-message.service";
import { MasterService } from "app/Service/master.service";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { from, Subject, take, takeUntil } from "rxjs";
import { LinkMenu, MenuContestualeService, MenuContestualeSettings } from "./menu-contestuale.service";
import { TranslocoService } from '@jsverse/transloco';

@Component({
    standalone: false,
    selector: 'gias-menu-contestuale',
    templateUrl: './menu-contestuale.component.html',
    styleUrls: ['./menu-contestuale.component.scss'],
    animations: [
        trigger( 'inOutAnimation',
            [
                transition(':enter',
                    [
                        style({ opacity: 0 }), animate('0.3s ease-out', style({ opacity: 1 }))
                    ]
                ),
                transition(':leave',
                    [
                        style({ opacity: 1 }), animate('0.3s ease-in',  style({ opacity: 0 }))
                    ]
                )
            ]
        )
    ]

})
/** header component*/
export class MenuContestualeComponent implements OnInit, OnDestroy {
    public menuContestualeSetting: MenuContestualeSettings;
    public loadComplete: boolean;
    public signal$: Subject<void> = new Subject();

    public faSearch = faSearch;
    public faSearchPlus = faSearchPlus;
    public faStar = faStar;
    public faBars = faBars;
    public faGripLinesVertical = faGripLinesVertical;

    public bookmarks = [];
    public contextualMenu = [];

    showImpreseFilter = false;

    constructor(
        private menuContestualeService: MenuContestualeService,
        private masterService: MasterService,
        private gestioneRichiesteService: GestioneRichiesteService,
        private giasMessageService: GiasMessageService,
        private objParametriAgendaService: ObjParametriAgendaService,
        private giasDialogService: GiasDialogService,
        private router: Router,
        private translocoService: TranslocoService) {

    }

    ngOnInit() {
        this.masterService.initialLoadCompleteSource.pipe(
            takeUntil(this.signal$),
        ).subscribe((val) => {
            this.loadComplete = val;
        })

        this.menuContestualeService.currentMenuContestualeSettings.pipe(takeUntil(this.signal$)).subscribe(
            (obj) => {
                this.menuContestualeSetting = obj;
                if (!obj.background_color) {
                    obj.background_color = '#052747';
                }
                if (!obj.color) {
                    obj.color = 'white';
                }
            }
        )
    }

    leggiPreferiti(event: any) {
        this.bookmarks = [];
        this.menuContestualeService.LeggiPreferiti().subscribe((val) => {
            this.bookmarks = val;
        })
    }

    leggiMenuContestuale(event: any) {
        this.contextualMenu = [];
        this.menuContestualeService.LeggiMenu().subscribe((val) => {
            this.contextualMenu = val;
        })
    }

    toggleImpreseFilter() {
        this.showImpreseFilter = !this.showImpreseFilter;
    }

    itemSelected(linkMenu: LinkMenu) {
        const idSezione = linkMenu.idSezione;
        const redirectUrl = linkMenu.redirectUrl;
        const sitoRichiesto = linkMenu.sitoRichiesto;
        const paginaRichiesta = linkMenu.paginaRichiesta;
        const aziendaRichiesta = linkMenu.richiedeAziendaSelezionata;
        const objPAgenda = this.objParametriAgendaService.getObjParamValue();
        if (aziendaRichiesta > 0 && objPAgenda.Piva == '') {
            this.giasDialogService.alertMessage(this.translocoService.translate('SelezionareImpresa'));
            return;
        }
        if (redirectUrl !== '') {
            from(this.gestioneRichiesteService.gestioneRedirect()).pipe(
                take(1)
            ).subscribe(
                (val: string) => {
                    const href = val + '&IDSezione=' + idSezione;
                    window.location.href = href
                }
            )
        } else {
            from(this.gestioneRichiesteService.gestionePassaggioAltroSito(linkMenu.sitoRichiesto, linkMenu.paginaRichiesta)).pipe(
                take(1)
            ).subscribe(
                (val: string) => {
                    if (val.indexOf("http") >= 0) {
                        window.location.href = val;
                    } else {
                        this.router.navigate([val]);
                    }
                },
                err => {
                    this.giasMessageService.errorMessage(err)
                });
        }

    }

    filtrinoImprese() {
        this.masterService.getCurrentPageAsValue();
        this.gestioneRichiesteService.goToFiltrino(Enum_SiteRedirector.GiasNG, this.masterService.getCurrentPageAsValue()).then((link) => {
            window.location.href = link;
        })
    }

    ngOnDestroy(): void {
        this.signal$.next();
        this.signal$.complete();
    }

}
