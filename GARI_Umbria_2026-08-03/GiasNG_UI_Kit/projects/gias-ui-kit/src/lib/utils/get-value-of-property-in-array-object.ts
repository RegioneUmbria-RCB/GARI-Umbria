export function getListOfPropertyInArrayObject(input_array: Array<any>, property: string): Array<any> {

  let output_array: Array<any> = input_array;

  if (output_array !== null && output_array !== undefined) {
    let properties: Array<string> = property.split(".");

    for (let i = 0; i < properties.length; i++) {

      if (output_array === null || output_array === undefined)
        break;

      output_array = output_array.map(x => x[properties[i]]);

    }
  }

  return output_array;
}