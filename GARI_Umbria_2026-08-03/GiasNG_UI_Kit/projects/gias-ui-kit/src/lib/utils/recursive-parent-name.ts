import { AbstractControl } from "@angular/forms";

export function recursiveParentName(c: AbstractControl): string {
  if (c?.parent) {
    let f = Object.keys(c.parent.controls).find(name => c === c.parent.get(name))
    return recursiveParentName(c.parent) + '.' + f;
  } else {
    return '';
  }
}