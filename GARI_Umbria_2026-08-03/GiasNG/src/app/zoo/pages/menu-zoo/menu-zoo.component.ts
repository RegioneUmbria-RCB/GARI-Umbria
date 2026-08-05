import {Component, DestroyRef, inject, Input, OnInit, ViewEncapsulation} from '@angular/core';
import {Subject} from "rxjs";
import {ViewportScroller} from "@angular/common";
import {MenuContestualeService} from "../../../Master/menu-contestuale/menu-contestuale.service";
import {ZooOperationsFilters} from "../../models/zoo-operations-filters.model";
import {takeUntilDestroyed} from "@angular/core/rxjs-interop";

@Component({
  standalone: false,
  selector: 'zoo-menu',
  templateUrl: './menu-zoo.component.html',
  styleUrls: ['./menu-zoo.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class MenuZooComponent implements OnInit {
  @Input() filters$: Subject<ZooOperationsFilters>;
  protected isEditingFavorites = false;
  private destroyRef = inject(DestroyRef);

  constructor(
    private menu: MenuContestualeService,
    private scroller: ViewportScroller,
  ) {
    this.menu.currentMenuContestualeSettings
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => {
        // this.dataStore.resetGridData();
        // this.applyFilters()
      });

    this.scroller.setOffset([0, 50]);
  }

  ngOnInit(): void {
  }

}
