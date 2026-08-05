import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { BaseCodeDescrStr } from 'app/Model/baseClass/baseCodeDescrStr';
import { ImpreseService } from 'app/Service/Anagrafica/imprese.service';
import { GiasDropDownTemplateSComponent, GiasDropDownTemplateService } from 'gias-ui-kit';
import { map, Subscription, take } from 'rxjs';
import { TestAnagraficaService } from './test-anagrafica.service';

@Component({
  standalone: false,
  selector: 'app-test-anagrafica',
  templateUrl: './test-anagrafica.component.html',
    styleUrls: ['./test-anagrafica.component.css'],
  providers:[GiasDropDownTemplateService]
})
export class TestAnagraficaComponent implements OnInit {
    impreseTestForm: FormGroup = this.fb.group({
        impresa: { codice: '', descrizione: ''}
    });

    sub = new Subscription();

    constructor(public impreseService: ImpreseService,
                public testAnagraficaService: TestAnagraficaService,
                public fb: FormBuilder) { }

    ngOnInit(): void {
        this.sub = (this.impreseTestForm.valueChanges.subscribe(
            (val) => {
                this.testAnagraficaService.PivaSelezionata = val.impresa.codice;
            }
        ))
  }

    openDdl(ddlEl: GiasDropDownTemplateSComponent) {
        ddlEl.loading = true;
        this.impreseService.leggiImprese().pipe(
            take(1),
            map((result) => {
                let imprese: Array<BaseCodeDescrStr> = []

                imprese = result.kendo_rows.map((impRow) => {
                    return {
                        codice: (<any>impRow).piva,
                        descrizione: (<any>impRow).piva + ' ' + (<any>impRow).rag_soc,
                    }
                });
                imprese.splice(0, 0, {
                    codice: '',
                    descrizione: 'TUTTE'
                });
                ddlEl.listItems = imprese;
                ddlEl.loading = false;
            })
        ).subscribe();
    }

}
