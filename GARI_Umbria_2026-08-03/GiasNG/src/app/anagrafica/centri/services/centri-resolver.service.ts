import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CentriResolverResult, CentroKendoServerResult } from '../centri.models';
import {GiasIstatService} from '../../../Service/istat/gias-istat.service';


@Injectable({ providedIn: 'root' })
export class CentriResolver {
  constructor(private istatService: GiasIstatService) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot):
    Observable<CentriResolverResult> | Promise<CentriResolverResult> | CentriResolverResult {
    return null;
  }

  getMemoryData(centri: CentroKendoServerResult) {
    const provinci = this.istatService.getProvincie();
    const stati = this.istatService.getStati();

    return {
      centri: centri,
      provincie: provinci,
      stati: stati
    };
  }
}
