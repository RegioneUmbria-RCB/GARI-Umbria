import { DecimalPipe } from '@angular/common';
import { Inject, LOCALE_ID, Pipe, PipeTransform } from '@angular/core';
import { getListOfPropertyInArrayObject } from '../../utils/get-value-of-property-in-array-object';

@Pipe({
  standalone: false,
  name: 'join',
  pure: true
})
export class GiasJoinPipe implements PipeTransform {

  constructor(
    private decimalpipe: DecimalPipe,
    @Inject(LOCALE_ID) private locale_id: string
  ) { }

  transform(input: Array<any>, sep = ' , ', field = '', formatNumbertolocal: boolean = null, digitsInfo: string = null): string {

    let result = "";

    if (input) {
      if (field === "") {
        if (digitsInfo && formatNumbertolocal)
          input = this.convertNumberValueColumnCombobox(input, field, digitsInfo);

        result = input.join(sep);

      } else {

        if (digitsInfo && formatNumbertolocal) {
          input = this.convertNumberValueColumnCombobox(input, field, digitsInfo);
        } else {
          input = getListOfPropertyInArrayObject(input, field);
        }

        if (input !== undefined && input !== null)
          result = input.join(sep);
      }
    }

    return result;
  }

  private convertNumberValueColumnCombobox(input: Array<any>, field: string, digitsInfo: string) {
    let new_input = [];

    for (let x = 0; x < input.length; x++) {
      let value = "";

      if (field === "") {
        value = this.decimalpipe.transform(input[x], digitsInfo, this.locale_id);
      } else {
        value = this.decimalpipe.transform(input[x][field], digitsInfo, this.locale_id);
      }

      new_input.push(value);
    }

    return new_input;
  }
}
