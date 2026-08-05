import { ChangeDetectionStrategy, ChangeDetectorRef, Component, NgZone, OnInit } from '@angular/core';
import { faHome, faIndustry, faObjectGroup } from '@fortawesome/free-solid-svg-icons';
import { BreadCrumbItem } from '@progress/kendo-angular-navigation';
import { BreadCrumb, removeNullValues } from './breadcrumbs.models';
import { BreadcrumbsService } from './breadcrumbs.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';

@Component({
    standalone: false,
    selector: 'gias-breadcrumbs',
    templateUrl: './breadcrumbs.component.html',
    styleUrls: ['./breadcrumbs.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class GiasBreadcrumbsComponent implements OnInit {
    items: BreadCrumbItem[] = [];
    faImpresa = faIndustry;
    faCentri = faHome;
    faCampi = faObjectGroup;

    constructor(
        public breadcrumbsService: BreadcrumbsService,
        public ngZone: NgZone,
        public ref: ChangeDetectorRef,
        public objParametriAgendaService: ObjParametriAgendaService) {
    }


    get shownBreadCrumbs() {
        const breadcrumbs = this.breadcrumbsService.items.map((crumb: BreadCrumb) => crumb.item);
        return removeNullValues([
            this.impresaPlaceholder?.item,
            ...breadcrumbs
        ]);
    }

    get impresaPlaceholder(): BreadCrumb {
        return this.breadcrumbsService.getSelectedImpresa();
    }

    get breadCrumbs(): BreadCrumb[] {
        return this.breadcrumbsService.items;
    }

    set breadCrumbs(items: BreadCrumb[]) {
        this.breadcrumbsService.items = items;
    }

    ngOnInit(): void {
        this.breadcrumbsService.componentInit(this.ref);
    }

    public onItemClick(selected: BreadCrumbItem): void {
        this.breadcrumbsService.discardGridFilters(selected);
    }
}
