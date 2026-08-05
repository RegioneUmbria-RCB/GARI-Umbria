import { Component, EventEmitter, Output } from "@angular/core";
import { TreeContainerService } from 'app/Utility/Template/kendo-tree/services/tree-container.service';
import { ProvideAnagraficaTreeDeps } from 'app/Utility/Template/kendo-tree/utility/providers';
import { enum_TreeContext } from "app/Utility/Template/kendo-tree/enum/tree-context";
import { SharedDataService } from "app/GIS/services/shared-data.service";
import { LayerService } from "app/GIS/services/layer.service";

@Component({
    standalone: false,
    selector: 'tree-window',
    templateUrl: './tree-window.component.html',
    styleUrls: ['./tree-window.component.css'],
    providers: [...ProvideAnagraficaTreeDeps()]
})

export class TreeWindowComponent {
    @Output() operationEvent = new EventEmitter<string>();

    opened: boolean = true;

    readonly treeContextGis = enum_TreeContext.Gis;

    constructor(
        private treeContainerService: TreeContainerService,
        private sharedDataService: SharedDataService,
        private layerService: LayerService,
     ) {

     }

    public toggle(isOpened: boolean): void {
      let a = 0; //Commento per funzione vuota SonarQube
    }

    expandPanel() {
        this.treeContainerService.expander.next(!this.treeContainerService.expander.value);
    }

    testSetLayerItemGrouping() {

        const idLayer = <string> $("#idLayer").val();
        console.log("idLayer=" + idLayer);

        const raggruppaLayer = $("#raggruppaLayer").is(":checked");
        console.log("raggruppaLayer=" + raggruppaLayer);

        let datiLayer = this.sharedDataService.getTipologiaLayerById(idLayer);

        datiLayer.RaggruppaDescrizioneAssociata = raggruppaLayer ? "1" : "0";

        this.layerService.LayerItemGrouping.next([datiLayer,raggruppaLayer]);
    }

}
