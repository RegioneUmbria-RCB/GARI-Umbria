import { Injectable } from '@angular/core';
import {AgriculturalItemBaseService} from '../agricultural-item-base.service';
import {KendoGridRow} from 'gias-kendo-grid';
import {AgriculturalItem} from '../agricultural-item.model';
import {IntervalloTemporale} from '../../../../Model/anagrafiche/IntervalloTemporale';
import {AGRODATAINIZIO} from '../../../../Model/CostantiPersonalizzate';

@Injectable()
export abstract class AgriculturalPlotBaseService extends AgriculturalItemBaseService {
  private _dateFilter: Date = AGRODATAINIZIO;
  private plantValidityFilter = (row: KendoGridRow) => {
    const actualPlantValidity: IntervalloTemporale = new IntervalloTemporale(row['Validita_Inizio_Impianto'], row['Validita_Fine_Impianto']);
    return (this._dateFilter.toISOString() == AGRODATAINIZIO.toISOString() || actualPlantValidity.contains(this._dateFilter));
  };

  set dateFilter(date: Date) {
    this._dateFilter = date;
  }

  setListViewRows(): void {
    this.loading$.next(true);
    this.listViewRows$.next(this.getUniquePlotsFromRows().filter(this.plantValidityFilter).map(this.mapToListViewItems.bind(this)));
    this.loading$.next(false);
  }

  protected getUniquePlotsFromRows() {
    let a: Map<string, KendoGridRow> = new Map<string, KendoGridRow>();

    this._checkedRows.forEach(r => {
      const key: string = JSON.stringify({
        codice: r['APPEZZA'],
        centroAziendalePK: {
          codice: r['SA_COD'],
          partitaIva: r['PIVA']
        }
      });

      a.set(key, r);
    });

    return Array.from(a.values());
  }

  protected mapToListViewItems(row: KendoGridRow): AgriculturalItem {
    return new AgriculturalItem(
      `${row['PIVA']}_${row['SA_COD']}_${row['APPEZZA']}`,
      this.extractDescription(row),
      row['PIVA'],
      row['SA_COD'],
      row['APPEZZA'],
      0,
      0,
      new IntervalloTemporale(),
      null,
      false,
      false,
      false
    );
  }
}
