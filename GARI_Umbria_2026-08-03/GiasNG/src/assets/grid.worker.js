/* Funzioni che tolgono gli elementi dupplicati delle righe.*/

// ! Tipo: string.
function distinctPrimitive(rows) {
  const data = [...new Set(rows)];
  return data;
}

// ! Tipo: date.
function distinctDates(rows) {
  const dates = rows.filter((value, index, self) =>
    index === self.findIndex((t) => (
      t.getTime() === value.getTime()
    ))
  );
  return dates;
}

// ! Tipo: dropdown list. Toglie gli elementi dupplicati dalla sorgente.
function distinctDropdown(rows, col) {
  const result = [...new Set(rows)];
  // const noSelection = { id: '', name: ''};


  // Get unique elements of the result.
  const items = result.map(item => {
    const key = item['id'];                  // < dato per scontato che esiste 'id'
    return [key, item];
  });
  const map = new Map(items);
  const unique = [...map.values()];
  return [...unique];
}

function distinctMultiDropdown(rows, col) {
  const result = [...new Set(rows)];
  // const noSelection = { id: '', name: ''};
  // Get unique elements of the result.
  const items = result.map(item => {
    if (typeof (item['id']) === 'number') {
      const key = item.id;
      return [key, item];
    } else if (Array.isArray(item.id)) {
      const key = item.id.map(i => i.toString()).reduce((acc, i) => acc + "_" + i);
      const mock = { id: key, name: item.name.reduce((acc, i) => acc + " | " + i) };
      return [key, mock];
    }
    return null;
  });
  const map = new Map(items);
  const unique = [...map.values()];
  return [...unique];
}

// ! Ascolta a diversi eventi emessi in grid-worker.service.ts.
addEventListener('message', ({ data }) => {
  const rows = data.rows;
  const fieldName = data.field;

  if (data.type == null) {
    // ignore this message, some other worker could have sent it
    return;
  }

  let risp;
  switch (data.type) {
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
    case 'webpackOk':
    case 'webpackClose':
      // skip event about webpack
      return;
    default:
      throw Error('Data type not recognized: ' + data.type);
  }

  postMessage({ rows: risp, field: fieldName });
});