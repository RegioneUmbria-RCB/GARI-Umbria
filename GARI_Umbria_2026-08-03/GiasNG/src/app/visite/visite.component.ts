import { Component,  ViewChild } from "@angular/core";
import { FiltersVisiteComponent } from "./filters-visite/filters-visite.component";
import { skip } from "rxjs";
import { FiltersServiceVisite } from "./filters-visite/filters-visite.service";

@Component({
    standalone: false,
    selector: 'app-visite',
    templateUrl: './visite.component.html',
    styleUrls: ['./visite.component.scss'],
    providers: []
  })
  export class VisiteComponent {

    isDisabled: boolean = false;
    showGrid: boolean = true;

    @ViewChild(FiltersVisiteComponent) child: FiltersVisiteComponent;

    constructor(private filters: FiltersServiceVisite,) {
      this.filters.subscribeToFiltersChange().pipe(skip(1)).subscribe((val) => {
        this.reloadGrid();
      });
    }

    reloadGrid() {
      this.showGrid = false;
      setTimeout(() => this.showGrid = true, 0);
    }

    updateDisabled(newValue: boolean) {
      this.isDisabled = newValue;
      this.child.updateDisabled(newValue);
    }

  }
