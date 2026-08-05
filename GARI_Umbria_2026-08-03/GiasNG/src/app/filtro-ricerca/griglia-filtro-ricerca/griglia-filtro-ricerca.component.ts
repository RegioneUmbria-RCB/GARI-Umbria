import { Component, EventEmitter, Inject, Input, OnChanges, OnDestroy, Output, SimpleChanges } from "@angular/core";
import { GridFiltroRicercaHttpService } from "./service/grid-filtro-http.service";
import { generateGridProviders } from 'gias-kendo-grid';
import { GRID_HTTP_TOKEN } from 'gias-kendo-grid';
import { GridPublicService } from 'gias-kendo-grid';
import { BehaviorSubject, skip } from "rxjs";
import { FormFiltriPianoColturale } from "../utils";
import { faGears } from "@fortawesome/free-solid-svg-icons";

@Component({
    standalone: false,
    selector: 'app-griglia-filtro',
    templateUrl: './griglia-filtro-ricerca.component.html',
    styleUrls: ['./griglia-filtro-ricerca.component.scss'],
    providers: [
                ...generateGridProviders(GridFiltroRicercaHttpService,
                                        GrigliaFiltroRicercaComponent)
    ]
})
export class GrigliaFiltroRicercaComponent implements OnChanges, OnDestroy {

    @Output() showGrid = new EventEmitter<boolean>();
    @Output() dataItemClicked = new EventEmitter<any>();
    @Output() openDatiAggiuntivi = new EventEmitter<boolean>();

    @Input() aziendaSelected: any;

    changeView;
    filtroColture = new BehaviorSubject<FormFiltriPianoColturale>(null);

    faGears = faGears;

    constructor(@Inject(GRID_HTTP_TOKEN) private gridFiltroRicercaHttpService: GridFiltroRicercaHttpService,
                private gridpublicService: GridPublicService) {

                    this.changeView = this.gridFiltroRicercaHttpService.changeViewToApply.pipe(skip(1)).subscribe((val) => {
                        this.showGrid.emit(val);
                    });

                    this.filtroColture.pipe(skip(1)).subscribe((val) => {
                        this.gridFiltroRicercaHttpService.filtroColture = val;
                    });
    }

    ngOnChanges(changes: SimpleChanges) {
        if (changes['aziendaSelected']) {
            this.gridFiltroRicercaHttpService.selectAzienda(this.aziendaSelected);
        }
    }

    ngOnDestroy(): void {
        this.changeView.unsubscribe();
    }

    refreshGriglia() {
        this.gridpublicService.refresh(true);
    }

    hideSwitchPrivatePublicView() {
        return this.gridFiltroRicercaHttpService.getPermessoGridViste();
    }

    onApriDatiAggiuntivi() {
        this.openDatiAggiuntivi.emit(true);
    }

}
