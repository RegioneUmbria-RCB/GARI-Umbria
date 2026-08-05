import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from "@angular/router";
import { TranslocoService } from "@jsverse/transloco";
import { filter, map, Observable, tap } from "rxjs";

@Injectable({ providedIn: 'root' })
export class TranslocoLoadedGuard  {

    constructor(private translocoService: TranslocoService){}

    async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> {
        let risp = new Promise<boolean>(async (resolve, reject) => {
            this.translocoService.selectTranslate('').subscribe(e => {
                resolve(true);
            });
        });
        return risp;
    }

}
