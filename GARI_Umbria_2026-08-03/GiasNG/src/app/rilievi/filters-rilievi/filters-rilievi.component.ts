import { Component, ViewChild } from "@angular/core";
import { faEraser, faMagnifyingGlass, faSliders, faXmark } from "@fortawesome/free-solid-svg-icons";
import { DrawerComponent } from "@progress/kendo-angular-layout";
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { GiasMultiSelectTemplateService } from 'gias-ui-kit';
import { FiltersRilieviService } from "./service/filters-rilievi.service";
import { Subscription } from "rxjs";
import { TranslocoService } from "@jsverse/transloco";
import { Impresa } from "app/Model/anagrafiche/Impresa";
import { Specie } from "app/Model/metaschema/utilizzi/Specie";
import { CentroAziendale } from 'app/Model/anagrafiche/CentroAziendale';
import { BaseCodeDescr } from "app/Model/baseClass/baseCodeDescr";

@Component({
    standalone: false,
    selector: 'app-filters-rilievi',
    templateUrl: './filters-rilievi.component.html',
    styleUrls: ['./filters-rilievi.component.scss'],
    providers: [GiasDropDownTemplateService, GiasMultiSelectTemplateService],
})
export class FiltersRilieviComponent {

    displayButton: boolean = true;
    drawerWidth: number;

    shouldApply: boolean = true;
    // showCentriAziendali: boolean = false;

    @ViewChild('drawer') drawer: DrawerComponent;

    private ddlSub: Subscription = new Subscription();

    defaultAzienda: Impresa;
    defaultSpecie: Specie;
    defaultCentro: CentroAziendale;
    defaultRilievo: BaseCodeDescr;

    faFilters = faSliders;
    faClose = faXmark;
    faSearch = faMagnifyingGlass;
    faTrash = faEraser;

    constructor(
        public filtersService: FiltersRilieviService,
        private ddlService: GiasDropDownTemplateService,
        private translocoService: TranslocoService
    ) {

        this.defaultAzienda = new Impresa();
        this.defaultAzienda.partitaIva = "-1";
        this.defaultAzienda.ragioneSociale = this.translocoService.translate("TutteLeAziende");

        this.defaultSpecie = new Specie(-1);
        this.defaultSpecie.descrizione = this.translocoService.translate("TutteLeSpecie");

        this.defaultCentro = CentroAziendale.getEmptyCentroAziendale("-1");
        this.defaultCentro.nome = this.translocoService.translate("TuttiICentriAziendali");

        this.defaultRilievo = new BaseCodeDescr(-1, this.translocoService.translate("TuttiIRilievi"));

    }

    onCancel() {
        this.displayButton = true;
        this.shouldApply = false;

        if (!!this.drawer.expanded) {
            this.drawer.toggle();
        }
    }

    onOpenFilters() {
        this.displayButton = false;
        this.drawer.toggle();
    }

    applyFilters() {
        this.filtersService.filters.next(this.filtersService.FiltersRilieviForm.getRawValue());

        if (!!this.drawer.expanded) {
            this.drawer.toggle();
        }
    }

}