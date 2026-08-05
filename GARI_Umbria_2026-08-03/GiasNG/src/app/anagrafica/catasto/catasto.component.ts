import { Component, ElementRef, Inject, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CatastoCentroAziendale } from 'app/Model/anagrafiche/CatastoCentroAziendale';
import { ParticelleCatastali } from 'app/Model/anagrafiche/ParticelleCatastali';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { CATASTO_SERVICE_TOKEN, CatastoFactoryService } from 'app/Service/ServiceFactory/catasto.factory.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { EditingMode, KendoGridColumn } from 'gias-kendo-grid';
import { GridPublicService } from 'gias-kendo-grid';
import { Subject, Subscription } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { CatastoGridEventsService } from './catasto-grid-events.service';
import { CatastoHttpService } from './catasto-grid.service';
import { generateGridProvidersAnagrafica } from 'app/Utility/Template/kendo-grid/services/providers';
import { ObjParametriAgenda } from 'gias-ui-kit';

@Component({
    standalone: false,
    selector: 'app-catasto',
    templateUrl: './catasto.component.html',
    styleUrls: ['./catasto.component.scss'],
    providers: [
        ...generateGridProvidersAnagrafica(CatastoHttpService, CatastoComponent), CatastoGridEventsService
    ],
    encapsulation: ViewEncapsulation.None
})
export class CatastoComponent implements OnInit, OnDestroy {
    @ViewChild('grid') grid: ElementRef<HTMLInputElement>;
    editingMode: EditingMode = EditingMode.IN_LINE;

    objParametriAgenda: ObjParametriAgenda;
    // kendo_model: KendoCatastoModel;
    kendo_columns: Array<KendoGridColumn> = new Array<KendoGridColumn>();
    kendo_rows: [];
    public signal$: Subject<void> = new Subject();
    changeDetectedSub = new Subscription();

    public permessoEdit: boolean

    constructor(private route: ActivatedRoute,
        private gridPublicService: GridPublicService,
        private objParametriService: ObjParametriAgendaService,
        private router: Router,
        @Inject(CATASTO_SERVICE_TOKEN) private catastoService: CatastoFactoryService,
        private permessiUtenteService: PermessiUtenteService,
        private catastoGridEventsService: CatastoGridEventsService) { }


    ngOnDestroy(): void {
        this.signal$.next();
        this.signal$.complete();
    }

    ngOnInit(): void {
        this.permessoEdit = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Anagrafica_ParticellaCatastale, 2);
        this.objParametriService.currentObjParametriAgenda.pipe(takeUntil(this.signal$))
            .subscribe((data: ObjParametriAgenda) => {
                this.objParametriAgenda = data;
            }
            );

        this.changeDetectedSub = this.gridPublicService.changeDetected.subscribe((event: any) => {
            if (event?.action === 'edit') {
                this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Update;
            }
            if (event?.action === 'remove') {
                this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Delete;
                this.catastoGridEventsService.addCatastoSelezionato(event?.dataItem, "");
            }
            if (event?.action === 'info') {
                this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Read;
            }
            if (event?.action === 'add') {
                this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
            }
        });
    }


    ParticellaSelezionata(event: any) {
        event.selectedRows.forEach(row => {
            this.catastoGridEventsService.addCatastoSelezionato(row.dataItem, "");
        });
        event.deselectedRows.forEach(row => {
            this.catastoGridEventsService.removeCatastoSelezionato(row.dataItem);
        });
    }

    public impresaSelezionata() {
        return this.objParametriAgenda.Piva != null && this.objParametriAgenda.Piva !== '';
    }


    // onTemplateBtnClick($event, dataItem) {
    //     const catastoEdit = this.catastoService.getParticellaEdit();
    //     catastoEdit.centro = { partitaIva: dataItem.Piva, codice: dataItem.Sa_Cod };
    //     catastoEdit.particella = new ParticelleCatastali();
    //     catastoEdit.particella.primaryKey =
    //     {
    //         Prov: dataItem.Prov,
    //         Com: dataItem.Com,
    //         Sezione: dataItem.SEZIONE,
    //         Foglio: dataItem.FOGLIO,
    //         Numero: dataItem.NUMERO,
    //         Subalterno: dataItem.SUBALTERNO
    //     };
    //     this.catastoService.changeParticellaEdit(catastoEdit);
    //     this.router.navigate(['/Anagrafica/Catasto/Catasto-Edit']);
    // }
    //}

    onNuovo() {
        let catastoEdit = new CatastoCentroAziendale()
        this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
        catastoEdit.centro = { partitaIva: this.objParametriAgenda.Piva, codice: 0 };
        catastoEdit.particella = new ParticelleCatastali();
        catastoEdit.particella.primaryKey =
        {
            Prov: '000',
            Com: '000',
            Sezione: '',
            Foglio: 0,
            Numero: 0,
            Subalterno: ''
        };
        this.objParametriService.changeObjParametriAgenda(this.objParametriAgenda);
        this.catastoService.changeParticellaEdit(catastoEdit);
        this.router.navigate(['Catasto-Edit'], { relativeTo: this.route });
    }

    onInvestimentoCatastale($event: any, dataItem: any) {
        this.catastoGridEventsService.onInvestimentoCatastale(dataItem);
    }
}
