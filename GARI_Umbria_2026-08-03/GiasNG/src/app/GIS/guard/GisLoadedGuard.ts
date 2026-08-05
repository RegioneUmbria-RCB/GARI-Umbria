import { Inject, Injectable, OnDestroy } from '@angular/core';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { TranslocoService, TRANSLOCO_SCOPE } from '@jsverse/transloco';
import { Subscription } from 'rxjs';

@Injectable()
export class GisLoadedGuard  {

    Subs = new Subscription();

    constructor(private translocoService: TranslocoService,
                @Inject(TRANSLOCO_SCOPE) private scope){}

    async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> {
        return new Promise<boolean>(async (resolve, reject) => {
             this.Subs.add(this.translocoService.selectTranslate('Albero', {}, this.scope).subscribe(e => {
                 resolve(true);
              }));
        });
    }
}
