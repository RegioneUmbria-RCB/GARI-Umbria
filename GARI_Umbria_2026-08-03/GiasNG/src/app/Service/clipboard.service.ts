import {Injectable} from '@angular/core';
import {Clipboard} from '@angular/cdk/clipboard';

@Injectable({
    providedIn: 'root'
})
export class ClipboardService {
    constructor(
        private clipboard: Clipboard
    ) {  }

    public copyStringToCliboard(s: string): boolean {
        return this.clipboard.copy(s);
    }

    public copyObjAsJsonToClipboard(obj: any): boolean {
        return this.clipboard.copy(JSON.stringify(obj))
    }

}
