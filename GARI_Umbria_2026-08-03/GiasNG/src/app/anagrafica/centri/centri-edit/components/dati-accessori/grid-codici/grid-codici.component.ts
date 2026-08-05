import { Component, Inject } from '@angular/core';
import { CodiciAnagrafeValoriChiave } from 'app/Model/anagrafiche/CodiciAnagrafeValori';
import { GRID_HTTP_TOKEN, KendoServerResult } from 'gias-kendo-grid';
import { AbstractGridConfigService } from 'gias-kendo-grid';
import { GridPublicService } from 'gias-kendo-grid';
import { generateGridProviders } from 'gias-kendo-grid';
import { map, Subject, take } from 'rxjs';
import { EditCentroStore } from '../../../services/centri-store.service';
import { CodiciAnagraficiConfigService } from '../../../services/grids/codici-anagrafici.service';
import { CodiciLoaded } from '../../../utils';



@Component({
  standalone: false,
  selector: 'grid-centri-codici',
  templateUrl: './grid-codici.component.html',
  styleUrls: ['./grid-codici.component.scss'],
  providers: [...generateGridProviders(CodiciAnagraficiConfigService, CentriCodiciGridComponent)]
})
export class CentriCodiciGridComponent {

    constructor(
        private store: EditCentroStore,
        private codiciPubService: GridPublicService,
        @Inject(GRID_HTTP_TOKEN) public conf: AbstractGridConfigService<KendoServerResult>
    ) {
        let gridService = conf as CodiciAnagraficiConfigService;
        this.store.setCodiciGridPubService(codiciPubService);

        store.codiciDataReady.pipe(take(1)).subscribe(s => {
            (gridService.codiciReady as Subject<CodiciLoaded>).next({ codici: (s.codici as CodiciAnagrafeValoriChiave[]), ddl: s.ddl });
        });

    }

}
