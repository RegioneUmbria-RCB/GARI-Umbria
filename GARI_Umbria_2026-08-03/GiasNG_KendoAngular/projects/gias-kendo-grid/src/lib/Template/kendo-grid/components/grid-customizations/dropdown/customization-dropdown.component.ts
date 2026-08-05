import {
    Component,
    EventEmitter,
    Input,
    OnDestroy,
    OnInit,
    Output
} from '@angular/core';
import { Subject } from 'rxjs';
import { skip, takeUntil } from 'rxjs/operators';
import { DropdownList, DropdownListItem } from '../../../models/grid.model';
import { GridCustomizationsService } from '../grid-customizations.service';
import { DropdownChanged, SelectionChangeEvent as ApplyViewEvent } from '../model';
import { CustomizeDropdownService } from './customization-dropdown.service';
import { GridPublicService } from '../../../services/grid-public.service';
import { FiltroJSONService } from '../../../services/filtro-json.service';



@Component({
    standalone: false,
    selector: 'gias-customization-dropdown',
    templateUrl: './customization-dropdown.component.html',
    providers: []
})
export class CustomizationDropdownComponent implements OnInit, OnDestroy {
    private signal$ = new Subject<void>();


    /** Dati */
    dropdown: DropdownList;

    /** Campi di input/output */
    filterable = true;
    @Input() selected: DropdownListItem;;
    @Output() selectedChange = new EventEmitter<DropdownListItem>();
    @Output() applyView = new EventEmitter<ApplyViewEvent>();
    @Output() applyViewAndJSON = new EventEmitter<ApplyViewEvent>();

    // Elenco che contiene
    source: DropdownListItem[] = [];
    @Output() sourceChange = new EventEmitter<DropdownListItem[]>();

    constructor(
        private service: GridCustomizationsService,
        private dropdownSerivce: CustomizeDropdownService,
        private gridpublicService: GridPublicService,
        private filtroJSONService: FiltroJSONService) {
    }

    /** Initialization */
    public ngOnInit(): void {
        this.dropdownSerivce.pipe(skip(1), takeUntil(this.signal$))
            .subscribe((ddl: DropdownChanged) => {
                if (!ddl) {
                    // should never happen
                } else {
                    this.init(ddl);
                }
            });
    }

    private init(data: DropdownChanged) {

        this.dropdown = data.dropdown;
        this.source = data.dropdown.data.slice();

        this.selected = data.selected ?? this.dropdownSerivce.nextSelected(this.dropdown);
        this.selectedChange.emit(this.selected);

        const ev = new ApplyViewEvent({ item: this.selected, isNew: false });
        this.applyView.emit(ev);
    }


    public onNgModelChange(item: DropdownListItem) {
        this.selected = item;
        this.selectedChange.emit(item);
        const ev = new ApplyViewEvent({ item: item, isNew: false });
        this.applyView.emit(ev);
    }

    public handleFilter(value: string) {
        this.filter = value;

        this.dropdown.data = this.source.filter(
            (s) => s.name.toLowerCase().indexOf(value.toLowerCase()) !== -1
        );
    }

    public changeValue(newValue: DropdownListItem) {
        this.selected = newValue;
        this.filtroJSONService.applyLoadCacheFirstTime = true;
        this.selectedChange.emit(this.selected);
        const ev = new ApplyViewEvent({ item: this.selected, isNew: false });
        if (this.gridpublicService.thereIsAForm && !newValue.data.Predefinita)
            this.applyViewAndJSON.emit(ev);
        else
            this.applyView.emit(ev);
    }

    public filter: string;
    public addNew(): void {

        // if (FORBIDDEN_CHARS_FOR_NAMES.some(char => this.filter.includes(char))) {
        //     this.dropdownSerivce.showMessage('NomeContieneCaratteriNonConsentiti');
        //     return;
        // }

        const item: DropdownListItem = {
            id: this.dropdownSerivce.generateUID(),
            name: this.filter,
            data: {
                Predefinita: false,
                NomeUtente: this.service.username,
                GridId: this.service.CreateNewGridIdforView()
            }
        };

        this.selected = item;
        const ev = new ApplyViewEvent({ item: this.selected, isNew: true });
        this.applyView.emit(ev);

        this.service.saveCurrentViewObs(item)
            .subscribe(() => {
                const items = this.source.push(item) && this.source;
                const ddl = this.dropdownSerivce.getNewDDL(this.dropdown, items);
                this.dropdownSerivce.next({ dropdown: ddl, selected: this.selected });
                this.selectedChange.emit(this.selected);
                this.handleFilter(this.filter);
            });
    }

    ngOnDestroy(): void {
        this.signal$.next();
        this.signal$.complete();
    }

}
