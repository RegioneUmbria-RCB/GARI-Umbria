import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { TranslocoService } from '@jsverse/transloco';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';
import { QdCService } from '../service/qdc.service';
import {enum_ErroreGias_Tipo, ErroreGias, ErroreGias_Severity} from "../../../Service/master.service";

export class GridImpiantiValidator {

    constructor(private qdcservice: QdCService,
                private funzionicomuniservice: FunzioniComuniService,
                private translocoService: TranslocoService) { }


    //Replica il funzionamento della onEditKendoSup_Coinvolta della OperazioneBootstrapMaster
    onEditSupTrattata(row): ValidatorFn {

        return (control: AbstractControl): ValidationErrors => {

            let listErroriGias: ErroreGias[] = [];

            let percAbb:number = this.qdcservice.OttieniPercentualeAbbattimentoDiserboDisseccamento(null,null);

            if (percAbb > 0 && percAbb < 100)
            {

                if(row.Selected){
                    let supImpianto = row.Sup_Imp;
                    let supTrattata = control.value;
                    let supTrattabile = this.funzionicomuniservice.roundNumber(supImpianto * percAbb / 100 , this.qdcservice.get4DecimalNumericSettings().decimals);

                    if (supTrattabile < supTrattata){

                        row.Sup_Imp_help = supTrattabile;

                        control.setValue(supTrattabile);

                        listErroriGias.push(<ErroreGias>{
                            severity: ErroreGias_Severity.Warning,
                            tipo:  enum_ErroreGias_Tipo.Generico,
                            messaggio: this.translocoService.translate('qdc.AttenzioneMaxSuperficieProdotto', { PercAbb: percAbb })
                        });
                    }

                }

            }

            if(listErroriGias.length > 0)
                this.qdcservice.gestisci_ErroriGias(listErroriGias,true,true).then();

            return null;
        };
    }
}


