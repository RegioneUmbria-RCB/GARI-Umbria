import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { HttpAction } from 'gias-kendo-grid';
import { KendoGridRow } from 'gias-kendo-grid';
import { IndirizziAppezzamentoDataService, IndirizzoAssociatoChiave } from './indirizzi-data.service';
import { Indirizzo } from '../../../../Model/anagrafiche/addresses/Indirizzo';
import { CodiciNazioniISO3166 } from 'app/Model/metaschema/CodiciNazioniISO3166';
import { Istat } from 'app/Model/metaschema/Istat';

const defaultIndex = -1;
const itemIndex = (item: IndirizzoAssociatoChiave, data: IndirizzoAssociatoChiave[]): number => {
  for (let idx = 0; idx < data.length; idx++) {
    if (data[idx].chiave === item.chiave) {
      return idx;
    }
  }
  return defaultIndex;
};

@Injectable()
export class IndirizziAppezzamentoService {
  private indirizzi: IndirizzoAssociatoChiave[];

  constructor(
    private indirizziAppezzamentoDataService: IndirizziAppezzamentoDataService
  ) {  }

  public perform(actionType: HttpAction, row: any): Observable<KendoGridRow[]> {
    this.updateTable(actionType, row);
    return of([]);
  }

  private updateTable(action: HttpAction, row: any) {
    if (this.indirizzi == undefined) {
      this.indirizzi = this.indirizziAppezzamentoDataService.indirizziAppezzamento;
    }

    let ind = this.mapToIndirizzoAssociatoChiave(row);

    switch(action) {
      case HttpAction.CREATE:
        if (this.indirizzi != undefined && this.indirizzi.length != 0){
          const chiave = Math.max.apply(Math, this.indirizzi.map(function (o) {
            return o.chiave;
          }));
          ind.chiave = chiave + 1;
          ind.indirizzo.codice = 0;
          ind.tipo_Indirizzo = 1;
        } else {
          ind.chiave = 0;
          ind.indirizzo.codice = 0;
          ind.tipo_Indirizzo = 1;
        }
        this.indirizzi.push(ind);
        break;
      case HttpAction.UPDATE:
        const index = itemIndex(ind, this.indirizzi);
        this.indirizzi.splice(index, 1, ind);
        break;
      case HttpAction.REMOVE:
        const _index = itemIndex(ind, this.indirizzi);
        this.indirizzi.splice(_index, 1);
        break;
    }
    this.indirizziAppezzamentoDataService.indirizziAppezzamento = this.indirizzi;
  }

  private mapToIndirizzoAssociatoChiave(row: any): IndirizzoAssociatoChiave {
    let ind: IndirizzoAssociatoChiave = new IndirizzoAssociatoChiave();
    ind.indirizzo = new Indirizzo();
    ind.indirizzo.istatComune = new Istat();
    ind.indirizzo.stato = new CodiciNazioniISO3166(row.codice_stato, row.descrizione_stato, '', '', 0);

    ind.indirizzo.cap = row.cap;
    ind.indirizzo.codice = row.codice;
    ind.indirizzo.frazione = row.frazione;
    ind.indirizzo.istatComune.com = row.com;
    ind.indirizzo.istatComune.localita = row.COMUNE;
    ind.indirizzo.istatComune.prov = row.prov;
    ind.indirizzo.istatComune.comuni_prov = row.PROVINCIA;
    ind.indirizzo.note = row.note;
    ind.indirizzo.via = row.via;
    ind.tipo_Indirizzo = row.tipo_indirizzo;

    ind.chiave = row.chiave;

    return ind;
  }

}
