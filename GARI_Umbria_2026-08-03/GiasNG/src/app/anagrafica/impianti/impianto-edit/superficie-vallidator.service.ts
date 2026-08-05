import { Inject, Injectable } from '@angular/core';
import { AbstractControl, AsyncValidator, FormGroup, ValidationErrors } from '@angular/forms';
import { TranslocoService } from '@jsverse/transloco';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { ImpiantiFactoryService, IMPIANTI_SERVICE_TOKEN } from 'app/Service/ServiceFactory/impianti.factory.service';
import { Observable, of, switchMap } from 'rxjs';


@Injectable()
export class SuperficieValidatorService implements AsyncValidator{

    private cambiaSuperficie: boolean = false;

    constructor(@Inject(IMPIANTI_SERVICE_TOKEN) private appezzamentiService: ImpiantiFactoryService,
        private ImpiantoEditForm: FormGroup,
        private giasDialogService: GiasDialogService,
        private transloco: TranslocoService)
    {  }

    validate(control: AbstractControl): Observable<ValidationErrors> {
        if (this.ImpiantoEditForm.value.primaryKey.codice == 0 || !control.touched) {
            return of(null);
        }
        return this.appezzamentiService.verifica_Superficie(this.ImpiantoEditForm.value, control.value).pipe(
            switchMap((res1) => {
                // if(this.cambiaSuperficie) {
                //     return of(null);
                // } else if(res1.RispostaStringa.toString() != ''){
                //     // return this.giasDialogService.dialogMessageObs_Result(
                //     //     '',
                //     //     res1.RispostaStringa.toString(),
                //     //     [
                //     //         {text: this.transloco.translate('Conferma'), primary: true, returnObj: true},
                //     //         {text: this.transloco.translate('Annulla'), returnObj: false}
                //     //     ],
                //     //     undefined,
                //     //     undefined,
                //     //     e => e instanceof DialogCloseResult
                //     // );
                //     return of(null);
                // } else {
                //     return of(null);
                // }
              return of(null);
            }),
            switchMap((res2) => {
                if (!res2) {
                    return of(null);
                }
                if (!res2['returnObj'] && res2['returnObj'] != undefined && res2[0] == null) {
                    return of([{ 'superficie': false }]);
                } else if (res2['returnObj'] && res2[0] == null) {
                    this.cambiaSuperficie = true;
                    return of(null);
                } else {
                    return of(null);
                }
            })
        );

    }

    registerOnValidatorChange?(fn: () => void): void {
        throw new Error('Method not implemented.');
    }

}
