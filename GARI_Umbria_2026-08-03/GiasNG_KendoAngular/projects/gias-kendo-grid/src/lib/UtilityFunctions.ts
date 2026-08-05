import { Renderer2 } from '@angular/core';
import { AbstractControl, FormArray } from '@angular/forms';

export module UtilityFunctions {

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

    export function forbiddenDdlValidator(control: AbstractControl) {
        if (control.value.codice == undefined || control.value.codice == -1 || control.value.codice == 0) {
            return { 'ddlNotSet': true };
        }
        return null;
    }
}

