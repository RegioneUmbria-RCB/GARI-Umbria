import { Injectable } from '@angular/core';
import { EditEvent } from '@progress/kendo-angular-grid';
import {  KendoGridColumn } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { GridPublicService } from 'gias-kendo-grid';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { inspect } from 'util'


function getTime(date: Date) {
  return date != null ? date.getTime() : 0;
}

const comparer = (a,b) => {
  if(a instanceof Date && b instanceof Date) {
    return getTime(a) - getTime(b);
  }

  if(typeof(a) == 'object' && typeof(b) == 'object')
  {
    return a?.name?.localeCompare(b?.name);
  }

  if(typeof(a) == 'number' && typeof(b) == 'number')
  {
    return a - b;
  }
  return a?.localeCompare(b);
}

@Injectable()
export class GridWorkerService {
  public resultsStore = {};
  public workLoad = new Subject<void>();
  public workerInst: Worker;
  public alreadySubed = false;

  constructor(public grid: GridPublicService) { }


  // ! Used to refresh worker data once a row modification has been made.
  public refreshWorker() {

    if(this.grid.changeDetected) {
      this.grid.changeDetected.pipe(takeUntil(this.grid.signal$))
        .subscribe((event: EditEvent) => {
          this.resultsStore = {};   // < force recalculation.
        });
      this.alreadySubed = true;
    }
  }

  // ! Elimina gli elementi dupplicati arrivati come righe della griglia.
  runTaskDistinct(
    rows: any[],
    field: string,
    type: CELL_TYPES,
    col: KendoGridColumn) {
    if (typeof Worker !== 'undefined') {
      if(!this.workerInst) {
        this.workerInst = new Worker(new URL('../grid.worker',import.meta.url));
        this.workerInst.onmessage = ({ data }) => {
          this.resultsStore[data.field] = data.rows.sort((a, b) => comparer(a, b));
          this.workLoad.next();
        };
        this.workerInst.onerror = this.onError;
      }
      const msg = {type: type, rows: rows, field: field,
        col: JSON.parse(JSON.stringify(col, getCircularReplacer()))};

      this.workerInst.postMessage(msg);
    } else {
      let risp;
      switch(type) {
        case 'string':
          risp = this.distinctPrimitive(rows);
          break;
        case 'dropdownlist':
          risp = this.distinctDropdown(rows, col);
          break;
        case 'multi_dropdownlist':
          risp = this.distinctDropdown(rows, col);
          break;
        case 'date':
          risp = this.distinctDates(rows);
          break;
        default:
          throw Error('Data type not recognized: ' + type);
      }

      this.resultsStore[field] = risp;
    }
  }

  // ! Funzioni che gestiscono il caso in cui il worker non è abilitato.
  distinctPrimitive(rows): any[] {
    const data = [...new Set(rows)];
    return data;
  }

  distinctDropdown(rows, col): any[] {
    const result = [];
    const ddl = col.ddl.data;
    const noSelection = { id: '', name: ''};
    rows.forEach(row => {
      const found = ddl.find(item => item.id === row.id);

      if(found) {
        result.push({ id: found.id, name: found.name});
      } else {
        result.push(noSelection);
      }
    });

    // Get unique elements of the result.
    const items: any[] = result.map(item => {
      const key = item['id'];               // < dato per scontato che esiste 'id'
      return [key, item];
    });
    const map: any = new Map(items);
    const unique = [...map.values()];
    return unique;
  }

  distinctDates(rows) {
    const data = rows.arr.filter((value, index, self) =>
        index === self.findIndex((t) => (
          t.place === value.place && t.name === value.name
        ))
    );
    return data;
  }

  // ! Stampa errore.
  onError(e) {
    const msg = [
      'ERROR: Line ', e.lineno, ' in ', e.filename, ': ', e.message
    ].join('');
    console.log(msg);
  }

  // Usually called due to a new data set being read in the grid.
  resetStore()
  {
    this.resultsStore = {};
  }
}

const getCircularReplacer = () => {
  const seen = new WeakSet();
  return (key, value) => {
    if (typeof value === "object" && value !== null) {
      if (seen.has(value)) {
        return;
      }
      seen.add(value);
    }
    return value;
  };
};
