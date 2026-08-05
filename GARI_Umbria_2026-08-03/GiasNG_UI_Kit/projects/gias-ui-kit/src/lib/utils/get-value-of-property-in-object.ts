export function getValueOfPropertyInObject(input: any, property: string): any {
  if (input !== null && input !== undefined) {
    const properties: Array<string> = property.split(".");

    for (let i = 0; i < properties.length; i++) {
      if (input === null || input === undefined)
        break;

      input = input[properties[i]];
    }
  }

  return input;
}