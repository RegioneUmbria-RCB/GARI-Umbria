import { Injectable } from '@angular/core';
import {AgriculturalItemBaseService} from '../agricultural-item-base.service';
import {KendoGridRow} from 'gias-kendo-grid';
import {AgriculturalItem} from '../agricultural-item.model';
import {IntervalloTemporale} from '../../../../Model/anagrafiche/IntervalloTemporale';
import {UtilizzoTerreno} from '../../../../Model/metaschema/utilizzi/UtilizzoTerreno';
import {Varieta} from '../../../../Model/metaschema/utilizzi/Varieta';
import {Specie} from '../../../../Model/metaschema/utilizzi/Specie';
import {DestinazioneUso} from '../../../../Model/metaschema/utilizzi/DestinazioneUso';

@Injectable()
export abstract class AgriculturalExerciseBaseService extends AgriculturalItemBaseService {
  setListViewRows(): void {
    this.loading$.next(true);
    this.listViewRows$.next(this._checkedRows.map(this.mapToListViewItems.bind(this)));
    this.loading$.next(false);
  }

  protected mapToListViewItems(row: KendoGridRow): AgriculturalItem {
    return new AgriculturalItem(
      `${row['PIVA']}_${row['Progetto_Cod']}`,
      this.extractDescription(row),
      row['PIVA'],
      row['SA_COD'],
      row['APPEZZA'],
      row['id_Reg'],
      row['Progetto_Cod'],
      new IntervalloTemporale(row['Validita_Inizio'], row['Validita_Fine']),
      this.extractUseOfLand(row),
      false,
      false,
      false
    );
  }

  private extractUseOfLand(row: KendoGridRow): UtilizzoTerreno {
    if (row['Veg_Cod'] !== 0 && row['Destinazione_Uso_Cod'] === 0) {
      let uol = new Varieta();
      uol.codice = row['cul_cod'];
      uol.descrizione = row['cul_des'];
      uol.specie = new Specie(row['veg_cod']);
      uol.specie.descrizione = row['veg_des'];

      return uol;
    } else {
      let uol: DestinazioneUso = new DestinazioneUso();
      uol.codice = row['Destinazione_Uso_Cod'];
      uol.descrizione = row['Destinazione_Uso_Des'];

      return uol;
    }
  }
}
