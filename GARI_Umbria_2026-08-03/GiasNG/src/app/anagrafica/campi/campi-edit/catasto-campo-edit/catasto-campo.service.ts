import { Injectable } from '@angular/core';
import { CatastoCampo } from 'app/Model/anagrafiche/CatastoCampo';
import {BehaviorSubject, Observable} from 'rxjs';

export class CatastoCampoId extends CatastoCampo{
  id: number;
}

@Injectable()
export class CatastoCampoService {

  getIndexOfCatastoCampoId(dataItem: any, catasto: CatastoCampoId[]): number{
    if (catasto != undefined && catasto.length > 0) {
      const Prov = dataItem.chiave.split('_')[0];
      const Comune = dataItem.chiave.split('_')[1];
      const Sezione = (dataItem.chiave.split('_')[2] == '' ? '0' : dataItem.chiave.split('_')[2]);
      const Foglio = parseInt(dataItem.chiave.split('_')[3]);
      const Numero = parseInt(dataItem.chiave.split('_')[4]);
      const Subalterno = (dataItem.chiave.split('_')[5] == '' ? '0' : dataItem.chiave.split('_')[5]);
      const chiaveParticella = Prov + '_' + Comune + '_' + Sezione + '_' + Foglio + '_' + Numero + '_' + Subalterno;

      for (let i = 0; i < catasto.length; i++) {
        let chiaveCatasto = catasto[i].particella.primaryKey['Prov'] + '_' +
          catasto[i].particella.primaryKey['Com'] + '_' +
          catasto[i].particella.primaryKey['Sezione'] + '_' +
          catasto[i].particella.primaryKey['Foglio'] + '_' +
          catasto[i].particella.primaryKey['Numero'] + '_' +
          catasto[i].particella.primaryKey['Subalterno'];

        if (chiaveCatasto == chiaveParticella) {
          return i;
        }
      }
    }

    return -1;
  }

}
