import { Renderer2 } from '@angular/core';
import { AbstractControl, FormArray, FormControl, FormGroup, ValidatorFn } from '@angular/forms';
import { enum_TipoOperazioneDB } from 'app/Model/TipiEnumerativi';
import { Cultivar } from '../Model/MetaschemaModel';
import { GiasDropDownTemplateSComponent, GiiasMultiselectTemplateSComponent } from 'gias-ui-kit';
import { GiasMultiColumnComboboxTemplateComponent } from 'gias-ui-kit';
import { Observable, take, tap } from "rxjs";
import { GiasDropDownTemplateComponent } from 'gias-ui-kit';

export module UtilityFunctions {
  // export function Stringa_Decodifica(testo: string, chiave: string): string {
  //  if (!testo || testo == "") {
  //    return ""
  //  }

  //  testo = Agronica_Url_Decode(testo)

  //  var cript = false;

  //  var strEncrypted = "";
  //  var i = 0;
  //  var A1: number;
  //  var A2: number;

  // }

  // export function Agronica_Url_Decode(strInput: string, separatore: string = "G"): string {
  //  if (!separatore || separatore == "") {
  //    separatore = "G";
  //  }
  //  var i: number;
  //  var carattere: string;
  //  var cod_ascii_16: number;
  //  var cod_hex: string;
  //  var strOutput: string;
  //  var vettore: string[];

  //  strOutput = ""

  //  vettore = strInput.split(separatore);

  //  vettore.forEach((value, index) => {
  //    cod_hex = value;
  //    cod_ascii_16 = Buffer.from(cod_hex, 'hex')
  //    carattere = String.fromCharCode(cod_ascii_16)
  //    strOutput = strOutput + carattere;
  //  })

  //  return strOutput;
  // }

  //Permette di nascondere o meno degli elementi Html
  export function setStyle(renderer: any, parent: any, selector: string, name: string, value: string) {
    let elem = parent.querySelector(selector);
    if (elem) {
      renderer.setStyle(elem, name, value);
    }
  }
  export function setStyleAll(renderer: any, parent: any, selector: string, name: string, value: string) {
    let elems = parent.querySelectorAll(selector);
    if (elems) {
      for (let elem of elems) {
        renderer.setStyle(elem, name, value);
      }
    }
  }

  export function addClass(renderer: Renderer2, parent: any, selector: string, name: string) {
    let elem = parent.querySelector(selector);
    if (elem) {
      renderer.addClass(elem, name);
    }
  }

  export function hideElem(renderer: any, parent: any, selector: string) {
    UtilityFunctions.setStyle(renderer, parent, selector, 'display', 'none');
  }

  export function showElem(renderer: any, parent: any, selector: string) {
    UtilityFunctions.setStyle(renderer, parent, selector, 'display', 'block');
  }

  export function trovaCultivarAltre(cultivar: Cultivar[]): number {

    let res = 0;
    const culQuery = cultivar.find((el) => {

      if (el.cul_des.trim().toLocaleLowerCase() == 'altre') {
        return el;
      }

    });

    if (culQuery !== undefined) {
      res = culQuery.cul_cod;
    }
    return res;
  }

  export async function loadDropDownItems(ddl: GiasDropDownTemplateSComponent | GiasDropDownTemplateComponent | GiasMultiColumnComboboxTemplateComponent, fn: Promise<any>) {
    ddl.loading = true;
    ddl.listItems = [];
    ddl.listItems = await fn;
    ddl.loading = false;
  }

  export function loadDropDownItemsObs(ddl: GiasDropDownTemplateSComponent | GiasDropDownTemplateComponent | GiasMultiColumnComboboxTemplateComponent, fn: Observable<any>) {
    ddl.loading = true;
    ddl.listItems = [];
    fn.pipe(
      take(1),
      tap(items => ddl.listItems = items)
    ).subscribe(() => ddl.loading = false)
  }

  export async function loadDropDownMultiSelectItemsOnlyWhenUndefined(ddl: GiasDropDownTemplateSComponent | GiasDropDownTemplateComponent | GiiasMultiselectTemplateSComponent | GiasMultiColumnComboboxTemplateComponent, fn: any) {

    if (ddl.listItems === undefined ||
      ddl.listItems === null) {

      const listItems = await fn();

      ddl.loading = true;
      ddl.listItems = [];
      ddl.listItems = listItems;
      ddl.loading = false;
    }

  }

  export function loadDropDownMultiSelectItemsOnlyWhenUndefinedObs(ddl: GiasDropDownTemplateSComponent | GiasDropDownTemplateComponent | GiiasMultiselectTemplateSComponent | GiasMultiColumnComboboxTemplateComponent, fn: Observable<any>) {

    if ((ddl.listItems === undefined ||
      ddl.listItems === null) && fn) {

      ddl.loading = true;
      ddl.listItems = [];

      fn.pipe(take(1)).subscribe(listItems => {
        ddl.listItems = listItems;
        ddl.loading = false;
      });
    }

  }

  export function compareObjects(o, p) {
    let i,
      keysO = Object.keys(o).sort((a, b) => a.localeCompare(b)),
      keysP = Object.keys(p).sort((a, b) => a.localeCompare(b));
    if (keysO.length !== keysP.length) {
      return false;
    }// not the same nr of keys
    if (keysO.join('') !== keysP.join('')) {
      return false;
    }// different keys
    for (i = 0; i < keysO.length; ++i) {
      if (o[keysO[i]] instanceof Array) {
        if (!(p[keysO[i]] instanceof Array)) {
          return false;
        }
        // if (compareObjects(o[keysO[i]], p[keysO[i]] === false) return false
        // would work, too, and perhaps is a better fit, still, this is easy, too
        if (p[keysO[i]].sort().join('') !== o[keysO[i]].sort().join('')) {
          return false;
        }
      } else if (o[keysO[i]] instanceof Date) {
        if (!(p[keysO[i]] instanceof Date)) {
          return false;
        }
        if (('' + o[keysO[i]]) !== ('' + p[keysO[i]])) {
          return false;
        }
      } else if (o[keysO[i]] instanceof Function) {
        if (!(p[keysO[i]] instanceof Function)) {
          return false;
        }
        // ignore functions, or check them regardless?
      } else if (o[keysO[i]] instanceof Object) {
        if (!(p[keysO[i]] instanceof Object)) {
          return false;
        }
        if (o[keysO[i]] === o) {// self reference?
          if (p[keysO[i]] !== p) {
            return false;
          }
        } else if (compareObjects(o[keysO[i]], p[keysO[i]]) === false) {
          return false;
        }// WARNING: does not deal with circular refs other than ^^
      }
      if (o[keysO[i]] !== p[keysO[i]])// change !== to != for loose comparison
      {
        return false;
      }// not the same value
    }
    return true;
  }

  export function clearFormArray(formArray: FormArray) {
    while (formArray.length !== 0) {
      formArray.removeAt(0);
    }
  }

  export function ManageFormArrayKendoGrid(formArray: FormArray, data: any, TipoOperazioneDB: enum_TipoOperazioneDB, index: number, emitEvent: boolean = true, Validator: ValidatorFn[] = []) {

    switch (TipoOperazioneDB) {
      case enum_TipoOperazioneDB.Scrittura:

        if (typeof data !== 'object')
          throw new Error("data deve essere un'oggetto");

        let formGroup = new FormGroup({}, Validator);

        for (let key of Object.keys(data)) {

          formGroup.addControl(key, new FormControl(data[key]));
        }

        if (index < 0) {
          formArray.push(formGroup);
        } else {
          formArray.insert(index, formGroup);
        }

        break;

      case enum_TipoOperazioneDB.Modifica:

        if (index < 0)
          throw new Error("index non può essere negativo");

        formArray.at(index).patchValue(data, { emitEvent: emitEvent });
        break;

      case enum_TipoOperazioneDB.Cancellazione:

        if (index < 0)
          throw new Error("index non può essere negativo");

        formArray.removeAt(index);
        break;
    }

  }

  export function distinctArrayValues(array: any[], fields: string[]): any[] {
    const retArray = new Array<any>();
    array.forEach((arrayEl) => {
      let arrayApp = { ...retArray };
      fields.forEach((field) => {
        arrayApp = arrayApp.filter((el) => {
          return el[field] == arrayEl[field];
        });
      });
      if (arrayApp.length == 0) {
        retArray.push(arrayEl);
      }
    });
    return [];
  }

  export function GetListofPropertyinArrayObject(input_array: Array<any>, property: string): Array<any> {

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

  export function GetValueofPropertyinObject(input: any, property: string): any {

    if (input !== null && input !== undefined) {
      let properties: Array<string> = property.split(".");

      for (let i = 0; i < properties.length; i++) {

        if (input === null || input === undefined)
          break;

        input = input[properties[i]];

      }
    }

    return input;

  }

  export function recursiveParentName(c: AbstractControl): string {
    if (c?.parent) {
      let f = Object.keys(c.parent.controls).find(name => c === c.parent.get(name))
      return this.recursiveParentName(c.parent) + '.' + f;
    } else {
      return '';
    }
  }

  export function forbiddenDdlValidator(control: AbstractControl) {
    if (control.value.codice == undefined || control.value.codice == -1 || control.value.codice == 0) {
      return { 'ddlNotSet': true };
    }
    return null;
  }

  export function getPrimitiveValueDDL(value) {
    if (typeof (value) == 'object')
      if (value.hasOwnProperty('id'))
        return value['id'];
      else if (value.hasOwnProperty('codice'))
        return value['codice'];

    return value;
  }

  export function getPrimitiveValue(obj: any, field: string) {
    if (typeof (obj) === 'object')
      return obj[field];
    return obj;
  }
}
export interface CodiceDescrizione {
  codice: number;
  descrizione: string;
}
