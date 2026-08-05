import { Component, EventEmitter, OnInit, Output } from "@angular/core";
import { faAnglesLeft, faClose, faEraser } from "@fortawesome/free-solid-svg-icons";
import { TreeAziendeService } from "app/filtro-ricerca/service/tree-aziende.service";
import { Subject, debounceTime } from "rxjs";

@Component({
    standalone: false,
    selector: 'app-tree-aziende-filters',
    templateUrl: './tree-aziende-filters.component.html',
    styleUrls: ['./tree-aziende-filters.component.scss']
})
export class TreeAziendeFilters implements OnInit {

    faFilter = faAnglesLeft;
    faClear = faEraser;
    faClose = faClose;
    isButtonFilterDisabled: boolean;
    isButtonReloadTreeDisabled: boolean;

    @Output() onCancelTree = new EventEmitter<boolean>();
    @Output() onExpandTree = new EventEmitter<boolean>();

    private filterSubject = new Subject<string>();

    constructor(public treeService: TreeAziendeService) {
        this.isButtonReloadTreeDisabled = true;
        this.isButtonFilterDisabled = false;
    }

    ngOnInit() {
        this.filterSubject.pipe(debounceTime(300)).subscribe(searchTerm => {
          this.filterNodes(searchTerm);
        });
    }

    onValueChange(searchTerm: string) {
        this.filterSubject.next(searchTerm); // Emette l'evento di filtro
    }

    riportaResultGrigliaSuAlbero() {
        if (this.treeService.arrPivaFilter.length > 0) {
            this.treeService.filteredNodes = this.treeService.filterTreeDataByPivaArray(this.treeService.allNodes, this.treeService.arrPivaFilter);
            this.isButtonFilterDisabled = true;
            this.isButtonReloadTreeDisabled = false;
        }
    }

    resettaAlbero() {
        this.treeService.filteredNodes = this.treeService.allNodes;
        this.isButtonFilterDisabled = false;
        this.isButtonReloadTreeDisabled = true;
    }

    filterNodes(event: any) {
        this.treeService.filteredNodes = this.treeService.filterTreeData(this.treeService.allNodes, event);
        if (!event)
            this.onExpandTree.emit(false);
        else
            this.onExpandTree.emit(true);
    }

    onCancel() {
        this.onCancelTree.emit(true);
    }
}
