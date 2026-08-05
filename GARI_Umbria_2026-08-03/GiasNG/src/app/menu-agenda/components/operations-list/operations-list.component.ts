import { Component, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { faStar } from '@fortawesome/free-solid-svg-icons';
import { TranslocoService } from '@jsverse/transloco';
import { DrawerComponent, PanelBarComponent, PanelBarItemModel } from '@progress/kendo-angular-layout';
import { distinct, filterBy } from '@progress/kendo-data-query';
import { Tipo_Ricetta } from 'app/Model/attivita/Attivita';
import {  ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { BussinessMenuAgendaService } from '../../shared_services/bussiness-logic.service';
import { MenuAgendaDataStore } from '../../shared_services/menu-agenda-datastore.service';
import { OperationsService } from './operations.service';
import {RibaltamentoTypes, TabTypes} from '../utils';
import { ConvertToPanelItems, Operation } from './utils';
import { Gias2010Redirector } from '../grid-qdc/Gias2010Redirector.service';
import { ObjParametriAgenda } from 'gias-ui-kit';

@Component({
    standalone: false,
    selector: 'operations-list',
    templateUrl: './operations-list.component.html',
    styleUrls: ['./operations-list.component.css'],
    encapsulation: ViewEncapsulation.None,
})
export class OperationsListComponent implements OnInit {

    @ViewChild('panelBar') panelBar: PanelBarComponent;

    ObjParametriAgenda: ObjParametriAgenda;
    source: PanelBarItemModel[] = new Array<PanelBarItemModel>();
    items: PanelBarItemModel[] = new Array<PanelBarItemModel>();
    signal: Subject<void> = new Subject();
    showFilter = true;
    searchDisabled = false;
    filterText = '';

    kdrawerZIndex: number = 1;


    faStar = faStar;

    constructor(private operations: OperationsService,
        private agenda: ObjParametriAgendaService,
        private dataStore: MenuAgendaDataStore,
        private redirector: Gias2010Redirector,
        private transloco: TranslocoService,
        private bussiness: BussinessMenuAgendaService) {

        this.ObjParametriAgenda = this.agenda.getObjParamValue();
        this.agenda.currentObjParametriAgenda.subscribe(obj => this.ObjParametriAgenda = obj);
    }

    ngOnInit(): void {
        this.dataStore.initializeOperations(this.signal);

        this.operations.operationsSubject.pipe(takeUntil(this.signal))
            .subscribe((data: Operation[]) => {
                this.source = ConvertToPanelItems(data);
                this.items = JSON.parse(JSON.stringify(this.source));
            });

        this.operations.drawerZIndex.pipe(takeUntil(this.signal))
            .subscribe((number: number) => {
                this.kdrawerZIndex = number;
            });
    }

    onInput(e: any) {
        this.filterText = e.target.value;

        const fillterBy = filterBy(this.source, {
            operator: 'contains',
            field: 'title',
            value: this.filterText,
        });

        this.items = distinct(
            [
                ...fillterBy,
            ],
            'id'
        ) as PanelBarItemModel[];
    }

    public stateChange(data: Array<PanelBarItemModel>): boolean {
        const focused: PanelBarItemModel = data.filter(
            (item) => item.focused === true
        )[0];

        let TipoRicetta = 0;

        switch (this.dataStore.selectedTabstrip.value) {
            case TabTypes.Ricette:
            case TabTypes.Brogliaccio:
                TipoRicetta = Tipo_Ricetta.Standard_Destinazioni;
                break;
        }

        this.redirector.redirectToQdCfromMenuAgenda(0, 0,
            this.ObjParametriAgenda.Piva, this.ObjParametriAgenda.RagSoc,
            + focused.id, focused.title, TipoRicetta,
            Enum_DBTypeOperation.Write,this.dataStore.selectedTabstrip.value,new Date(),
            RibaltamentoTypes.Nessuno,this.ObjParametriAgenda.IdSezione);

        return false;
    }

    @ViewChild('drawer') drawer: DrawerComponent;
    labelPreferiti: string;
    public toggle() {
        this.labelPreferiti = this.computeLabelPreferiti();
        this.drawer.toggle();
    }


    computeLabelPreferiti() {
        let label = this.transloco.translate('opPreferite', {});
        let append = null;
        switch (this.dataStore.selectedTabstrip.value) {
            case TabTypes.QuadernoDiCampagna:
                break;
            case TabTypes.Ricette:
                append = this.transloco.translate('OpLabelRicette', {});
                label += " (" + append + ")";
                break;
            case TabTypes.Brogliaccio:
                append = this.transloco.translate('Brogliaccio', {});
                label += " (" + append + ")";
                break;
        }
        return label;
    }
}


