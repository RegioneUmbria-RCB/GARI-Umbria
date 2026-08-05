import {
  Component, forwardRef,
  Input,
  OnDestroy,
  OnInit,
  Output,
  ViewEncapsulation
} from '@angular/core';
import {
  AddEvent,
  CancelEvent,
  EditEvent, ListViewComponent,
  RemoveEvent,
  SaveEvent
} from '@progress/kendo-angular-listview';
import {BehaviorSubject, ReplaySubject, Subject, takeUntil} from 'rxjs';
import {faClose, faTrashCan} from '@fortawesome/free-solid-svg-icons';
import {NG_VALUE_ACCESSOR} from '@angular/forms';

@Component({
  standalone: false,
  selector: 'app-listview',
  templateUrl: './listview.component.html',
  styleUrls: ['./listview.component.css'],
  encapsulation: ViewEncapsulation.None,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ListviewComponent),
      multi: true
    }
  ]
})
export class ListviewComponent implements OnInit, OnDestroy {
  /** The ListView is as high as its content and as wide as the available space.
   * To enable scrolling, use a fixed height that is less than the height of the ListView content. */
  @Input() height$: ReplaySubject<number> = new ReplaySubject<number>();
  @Input() loading$: ReplaySubject<boolean> = new ReplaySubject<boolean>();
  @Input() data$: BehaviorSubject<IListViewItem[]> = new BehaviorSubject<IListViewItem[]>([]);
  @Input() removeDataItem$: Subject<{ index: number, deleteCount: number; }>;
  @Input() resetStates$: Subject<boolean> = new Subject();
  @Input() showListHeader: (...args) => boolean = () => false;
  @Input() showAddBtn: (...args) => boolean = () => false;
  @Input() showEditBtn: (...args) => boolean = () => false;
  @Input() showSaveBtn: (...args) => boolean = () => false;
  @Input() showCancelBtn: (...args) => boolean = () => false;
  @Input() showRemoveBtn: (...args) => boolean = () => true;
  @Input() showDeleteBtn: (...args) => boolean = () => false;

  @Input() removeBtnTitle: string = 'giasgrid.RemoveRowFromList';
  @Input() deleteBtnTitle: string = 'giasgrid.Elimina';

  @Output('add') addEvent$: Subject<AddEvent> = new Subject<AddEvent>();
  @Output('edit') editEvent$: Subject<EditEvent> = new Subject<EditEvent>();
  @Output('save') saveEvent$: Subject<SaveEvent> = new Subject<SaveEvent>();
  @Output('cancel') cancelEvent$: Subject<CancelEvent> = new Subject<CancelEvent>();
  @Output('remove') removeEvent$: Subject<RemoveEvent> = new Subject<RemoveEvent>();
  @Output('delete') deleteEvent$: Subject<{ item: IListViewItem, itemIndex: number; }> = new Subject<{
    item: IListViewItem,
    itemIndex: number;
  }>();

  protected readonly faTrashCan = faTrashCan;
  protected readonly faClose = faClose;

  private destroy$: Subject<void> = new Subject();
  private editedRowIndex: number;

  constructor() {  }

  ngOnInit(): void {
    this.removeDataItem$.pipe(takeUntil(this.destroy$)).subscribe(e => {
      this.removeDataItem(e.index, e.deleteCount);
    });

    this.resetStates$.pipe(takeUntil(this.destroy$)).subscribe(state => {
      this.resetDataState(state);
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  addHandler(event: AddEvent): void {
    throw new Error('Method not ye implemented');
  }

  editHandler(event: EditEvent): void {
    throw new Error('Method not yet implemented');
  }

  saveHandler(event: SaveEvent): void {
    throw new Error('Method not yet implemented');
  }

  cancelHandler(event: CancelEvent): void {
    throw new Error('Method not yet implemented');
  }

  removeHandler(event: RemoveEvent): void {
    this.removeDataItem$.next({index: event.itemIndex, deleteCount: 1});
  }

  deleteHandler(dataItem: IListViewItem, index: number): void {
    this.deleteEvent$.next({item: dataItem, itemIndex: index});
  }

  setItemStateStyle(item: IListViewItem, index: number): string {
    // edit freely the style for warning and success, there was no time to do it well (as usual)
    let style: string;
    if (item.error) {
      style = 'background-color: #F8EEED !important'
    } else if (item.warning) {
      style = index % 2 === 0 ? 'background-color: #F0F0F0 !important': ''
    } else if (item.success) {
      style = index % 2 === 0 ? 'background-color: #F0F0F0 !important': ''
    } else {
      style = index % 2 === 0 ? 'background-color: #F0F0F0 !important': ''
    }
    return style;
  }

  private removeDataItem(index: number, deleteCount: number): void {
    this.data$.value.splice(index, deleteCount);
    this.data$.next([...this.data$.value]);
  }

  private closeEditor(sender: ListViewComponent, itemIndex: number = this.editedRowIndex): void {
    sender.closeItem(itemIndex);
    this.editedRowIndex = undefined;
  }

  private resetDataState(state: boolean = false): void {
    this.data$.next(this.data$.getValue().map(r => {
      r.error = state;
      r.success = state;
      r.warning = state;

      return r;
    }));
  }
}

export interface IListViewItem {
  description: string;
  key: string | number;
  error?: boolean;
  success?: boolean;
  warning?: boolean;
}
