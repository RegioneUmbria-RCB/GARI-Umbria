
import { Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges } from '@angular/core';
import { faAlignJustify, faSave, faTrash, IconDefinition } from '@fortawesome/free-solid-svg-icons';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { DropdownListItem, GridCustomizations } from '../../models/grid.model';
import { GridCustomizationsService } from './grid-customizations.service';
import { InMemoryView, SelectionChangeEvent } from './model';

@Component({
  standalone: false,
  selector: 'gias-grid-customizations',
  templateUrl: './grid-customizations.component.html',
  providers: []
})
/** TODO: Razvan. Quando questa componente è attiva, la pagina della griglia è
 * renderizzata due volte.
 **/
export class GridCustomizationsComponent implements OnInit, OnDestroy, OnChanges {

  @Input() canApplyChanges = false;
  @Input() options: GridCustomizations;
  @Input() showSwitch = false;

  @Input() public set selectedDDLItem(value) {
    this._selectedDDLItem = value;
    //Tengo aggiornato anche il parametro nel servizio
    this.service.selectedDDLItem = value;
  }

  @Output() canApplyChangesChange = new EventEmitter<boolean>();
  @Output() applyView = new EventEmitter<InMemoryView>();

  faAlign: IconDefinition = faAlignJustify;
  faSave: IconDefinition = faSave;
  faTrash: IconDefinition = faTrash;
  switchValue: boolean = false;

  private $signal: Subject<void> = new Subject();
  private _selectedDDLItem: DropdownListItem;

  public get selectedDDLItem() {
    return this._selectedDDLItem;
  }

  constructor(public service: GridCustomizationsService) {
  }

  ngOnInit(): void {
    this.service.permessoPubblica = this.showSwitch;
    this.loadSubscriptions();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes.canApplyChanges.currentValue)
      this.forceApplyModifications();
  }

  ngOnDestroy(): void {
    this.service.persistView(this.selectedDDLItem);

    this.$signal.next();
    this.$signal.complete();
  }

  isDefaultSaveDisabled() {
    return this.service.conf.views.isSaveDisabledOnDefaultView && this.selectedDDLItem?.data?.Predefinita;
  }

  public forceApplyModifications(): void {
    const view: InMemoryView = this.service.getView(this.selectedDDLItem, true);

    if (view) {
      //this.applyView.emit(view);
      view.IsModified = true;
    }

    this.canApplyChanges = false;
    this.canApplyChangesChange.emit(this.canApplyChanges);
  }

  public applyOrCreateAndApplyView(ev: SelectionChangeEvent) {
    this.selectedDDLItem = ev.item as DropdownListItem;
    if (!ev.isNew) {
      this.canApplyChanges = false;
    }

    const view: InMemoryView = ev.isNew
      ? this.service.getView(ev.item as DropdownListItem, false)
      : this.service.getServerView(ev.item as DropdownListItem);

    this.switchValue = view.FlagPubblica;
    this.service.switchValue = this.switchValue;

    if (this.service.checkIfThereIsAForm()) {
      this.service.setFormFields(view);
      return;
    }

    this.applyView.emit(view);
  }

  public applyViewAndJSON(ev: SelectionChangeEvent) {
    const view: InMemoryView = this.service.getView(ev.item as DropdownListItem, false);
    //questa chiamata fa partire la ricarica della griglia
    this.service.setFormFields(view);
  }

  public onSave(event) {
    //this.service.saveAllViews();
    this.service.switchValue = this.switchValue;
    this.service.saveCurrentView(this.selectedDDLItem);
  }

  public onRemove(event) {
    this.service.deleteCustomization(this.selectedDDLItem);
  }

  private loadSubscriptions() {
    this.service.viewToApply
      .pipe(takeUntil(this.$signal))
      .subscribe((item: DropdownListItem) => {
        this.selectedDDLItem = item;
        if (!this.service.cacheViewWasApplied) {
          const ev = new SelectionChangeEvent({ item: item, isNew: false });
          this.applyOrCreateAndApplyView(ev);
        }
      });
  }
}
