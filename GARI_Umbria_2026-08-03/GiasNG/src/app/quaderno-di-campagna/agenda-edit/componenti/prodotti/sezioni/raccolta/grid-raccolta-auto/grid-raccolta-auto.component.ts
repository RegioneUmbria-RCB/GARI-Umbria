import { Component, DoCheck, Input, ViewChild } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { enum_UnitaMisura } from 'app/Model/TipiEnumerativi';
import { QdCRaccoltaService } from 'app/quaderno-di-campagna/agenda-edit/service/prodotti/raccolta.service';
import { generateGridProviders } from 'gias-kendo-grid';
import { GridRaccoltaAutoConfigService } from './grid-raccolta-auto.service';

@Component({
  standalone: false,
  selector: 'app-grid-raccolta-auto',
  templateUrl: './grid-raccolta-auto.component.html',
  styleUrls: ['./grid-raccolta-auto.component.css'],
  providers: [
    ...generateGridProviders(GridRaccoltaAutoConfigService, GridRaccoltaAutoComponent)
  ]
})
export class GridRaccoltaAutoComponent implements DoCheck {

  @ViewChild('grid') grid: any;

  @Input() Prodotto;

  constructor(
    private raccoltaService: QdCRaccoltaService,
    private transloco: TranslocoService
  ) { }


  /** Valorizza Op_Cod e UDM degli elementi appena inseriti:
   * Op_Cod è usato nella griglia come identificatore della riga,
   * UDM impostata a chilogrammi come default.
   */
  ngDoCheck(): void {
    if(this.grid?.rows?.length > 0) {
      this.grid.rows.forEach(row => {
        if (row.Op_Cod === undefined || row.Op_Cod === null) {
          row.Op_Cod = this.raccoltaService.progressivo;
        }

        if (row.Qta === '') row.Qta = 0;
        if (row.Prodotto_Cod === null) row.Prodotto_Cod = 0;
        if (!row.Progetto_Des) {
            row.Progetto_Des = this.raccoltaService.qdcService.GetEserciziCDCSelezionatiModel()
                .map(e => e.esercizio.codice.toString())
                .reduce((acc, e) => acc + '|' + e);
        }
        if (row.Prodotto_Des === '') {
          row.Prodotto_Des = this.grid.translocoService
                                 .translate('qdc.NessunaSelezione');
        }
        if (!row.Magazzino_Cod) row.Magazzino_Cod = 0;
        if (!row.UM_Cod) row.UM_Cod = enum_UnitaMisura.KG;
        if (!row.UM_Des) row.UM_Des = this.transloco.translate("Chilogrammi");
      });

      this.grid?.refresh();
    }
  }


}
