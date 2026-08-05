import { Inject, Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { TranslocoService, TRANSLOCO_SCOPE } from '@jsverse/transloco';

@Injectable()
export class BudgetLoadedGuard  {

    constructor(private translocoService: TranslocoService,
        @Inject(TRANSLOCO_SCOPE) private scope){}

    async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> {
        const risp = new Promise<boolean>(async (resolve, reject) => {
            this.translocoService.selectTranslate('CaricatoTutto', {}, this.scope).subscribe(e => {
                resolve(true);
            });
        });
        return risp;
    }

}
