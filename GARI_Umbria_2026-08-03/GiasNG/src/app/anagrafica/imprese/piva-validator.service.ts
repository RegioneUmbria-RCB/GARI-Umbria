import { Inject, Injectable } from '@angular/core';
import {AbstractControl, AsyncValidator, ValidationErrors} from '@angular/forms';
import { ImpreseFactoryService, IMPRESE_SERVICE_TOKEN } from 'app/Service/ServiceFactory/imprese.factory.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import {debounceTime, first, map, Observable, of, switchMap} from 'rxjs';
import {catchError} from 'rxjs/operators';
import {PIVA_LENGTH} from '../../Model/CostantiPersonalizzate';

@Injectable()
export class PivaValidatorService implements AsyncValidator {

  constructor(
    @Inject(IMPRESE_SERVICE_TOKEN) private impreseService: ImpreseFactoryService,
    private objParametriAgendaService: ObjParametriAgendaService
  ) {  }

  validate(control: AbstractControl): Observable<ValidationErrors | null> {
    return of(control.value).pipe(
      debounceTime(1000),
      switchMap((piva: string) => {
        const operationType = this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB;

        if (piva == undefined || piva.length === 0 || operationType == Enum_DBTypeOperation.Update) {
          return of(null);
        } else if (piva.length !== PIVA_LENGTH) {
          return of({invalidPiva: true});
        } else {
          return this.impreseService.controlloPresenzaPiva(piva);
        }
      }),
      map((resp: string | { invalidPiva: boolean } | { piva: boolean } | null) => {
        if (resp != undefined && resp.toString() != '') {
          return {invalidPiva: true};
        } else {
          return null;
        }
      }),
      catchError(() => of(null)),
      first()
    );
  }
}
