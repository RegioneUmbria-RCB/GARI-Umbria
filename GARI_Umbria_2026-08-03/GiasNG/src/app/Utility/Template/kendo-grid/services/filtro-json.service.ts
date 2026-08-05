import { Injectable } from "@angular/core";

@Injectable({ providedIn: 'root' })
export class FiltroJSONService {

    public isFirstApplyView: boolean = true;

    public applyLoadCacheFirstTime: boolean = true;

}