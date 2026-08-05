import { Component, Input, OnDestroy, OnInit, Output, TemplateRef, ViewChild, } from '@angular/core';
import { DrawerComponent } from "@progress/kendo-angular-layout";
import { Subject } from "rxjs";
import { FormControl, FormGroup } from "@angular/forms";
import { faSearch, faSliders, faXmark } from '@fortawesome/free-solid-svg-icons';
import { enum_TipoControllo } from '../utils/models';
export interface giasGenericFormControl<T> {
  label: FormControl<String>;
  value: FormControl<T | T[]>;
  listItems: FormControl<T[]>;
  fieldType: FormControl<enum_TipoControllo>;
  tooltip: FormControl<String>;
}

export class FilterFieldFormGroupFactory {
  constructor() { }

  public create<T>(
    label?: string, value?: T | Array<T>, filedType?: enum_TipoControllo,
    listItems?: Array<T>, tooltip?: string
  ): FormGroup<giasGenericFormControl<T>> {
    return new FormGroup<giasGenericFormControl<T>>({
      label: new FormControl(label ?? ''),
      value: new FormControl(value ?? null),
      listItems: new FormControl(listItems ?? []),
      fieldType: new FormControl(filedType ?? 0),
      tooltip: new FormControl(tooltip ?? '')
    });
  }
}

/**
 *
 * @UsageNotes
 * To correctly render the component, please add the sequent style
 * ```
 * { position: absolute; z-index: 1000; right: 0px !important; }
 * ```
 */
@Component({
  standalone: false,
  selector: 'gias-side-filters-template',
  templateUrl: './gias-side-filters-template.component.html',
  styleUrls: ['./gias-side-filters-template.component.css']
})
export class GiasSideFiltersTemplateComponent implements OnInit, OnDestroy {
  @ViewChild('drawer') drawer: DrawerComponent;
  /**
   * Template containing the elements to show inside the side page. Must be passed as a `<ng-template>`.
   * @Example
   * In the parent component's html:
   * ```
   *  <app-side-filters-template [filtersTemplate]="filtersForm"></app-side-filters-template>
   *  ...
   *  <ng-template #filtersForm>
   *    ...
   *  </ng-template>
   * ```
   */
  @Input() filtersTemplate: TemplateRef<any> | null;
  /**
   * FormGroup containing the controls to show in the side page.
   * The controls should be of type {@link giasGenericFormControl} and can be created with the class
   * {@link FilterFieldFormGroupFactory}.
   * This method is still a bit unstable. The use of `filtersTemplate` is advised.
   * @beta
   */
  @Input() filtersFormGroup: FormGroup | null = null;
  /**
   * Emits when clicking on the search button.
   */
  @Output() search = new Subject<void>();
  /**
   * Defines if the filters should be applied on the press of the search button.
   */
  public shouldApply = true;
  protected drawerWidth: number;
  protected displayButton = true;
  protected signal: Subject<void> = new Subject<void>();
  protected faSearch = faSearch;
  protected faClose = faXmark;
  protected faFilters = faSliders;

  constructor() { }

  ngOnInit(): void { }

  ngOnDestroy(): void {
    this.search.complete();
    this.signal.next();
    this.signal.complete();
  }

  public getFiltersFields() {
    const ctrls = [];
    for (let c in this.filtersFormGroup.controls) {
      ctrls.push(this.filtersFormGroup.get(c));
    }
    return ctrls;
  }

  public onOpenFilters() {
    this.displayButton = false;
    this.drawer.toggle();
  }

  public onCancel() {
    this.displayButton = true;
    this.shouldApply = false;
    if (!!this.drawer.expanded) {
      this.drawer.toggle();
    }
  }

  public applyFilters() {
    if (!this.shouldApply) return;
    // if (!!this.filtersFormGroup) {
    this.search.next();
    // }
    this.centerVisual();
    if (!!this.drawer.expanded) {
      this.drawer.toggle();
    }
  }

  public centerVisual(): void {
    document.documentElement.scrollTop = 0;
  }

}
