/// <reference lib="webworker" />

/* Funzioni che tolgono gli elementi dupplicati delle righe.*/

// ! Tipo: string.
function distinctPrimitive(rows): any[] {
  const data = [...<any>new Set(rows)];
  return data;
}

// ! Tipo: date.
function distinctDates(rows): any[] {
  const dates = rows.filter((value, index, self) =>
      index === self.findIndex((t) => (
        t.getTime() === value.getTime()
      ))
  );
  return dates;
}

// ! Tipo: dropdown list. Toglie gli elementi dupplicati dalla sorgente.
function distinctDropdown(rows, col): any[] {
  const result = [...<any>new Set(rows)];
  // const noSelection = { id: '', name: ''};


  // Get unique elements of the result.
  const items: any[] = result.map(item => {
    const key = item['id'];                  // < dato per scontato che esiste 'id'
    return [key, item];
  });
  const map: any = new Map(items);
  const unique = [...map.values()];
  return [...unique];
}

function distinctMultiDropdown(rows, col): any[] {
  const result = [...<any>new Set(rows)];
  // const noSelection = { id: '', name: ''};
  // Get unique elements of the result.
  const items: any[] = result.map(item => {
    if (typeof(item['id']) === 'number') {
      const key = item.id;
      return [key, item];
    } else if (Array.isArray(item.id)) {
      const key = item.id.map(i => i.toString()).reduce((acc, i) => acc + "_" + i);
      const mock = {id: key, name: item.name.reduce((acc, i) => acc + " | " + i)};
      return [key, mock];
    }
  });
  const map: any = new Map(items);
  const unique = [...map.values()];
  return [...unique];
}

// ! Ascolta a diversi eventi emessi in grid-worker.service.ts.
addEventListener('message', ({ data }) => {
  const rows = data.rows;
  const fieldName = data.field;

  let risp;
  switch(data.type) {
    case 'string':
      risp = distinctPrimitive(rows);
      break;
    case 'dropdownlist':
      risp = distinctDropdown(rows, data.col);
      break;
    case 'multi_dropdownlist':
      risp = distinctMultiDropdown(rows, data.col);
      break;
    case 'date':
      risp = distinctDates(rows);
      break;
    case 'datetime':
      risp = distinctDates(rows);
      break;
    case 'number':
      risp = distinctPrimitive(rows);
      break;
    default:
      throw Error('Data type not recognized: ' + data.type);
  }

  postMessage({rows: risp, field: fieldName});
});
