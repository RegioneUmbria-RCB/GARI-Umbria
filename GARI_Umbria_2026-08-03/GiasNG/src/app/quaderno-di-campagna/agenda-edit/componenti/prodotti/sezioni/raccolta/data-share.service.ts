import { Injectable } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { TranslocoService } from "@jsverse/transloco";
import { Fabbricato } from "app/Model/anagrafiche/Fabbricato";
import { DettaglioRaccolta } from "app/Model/attivita/dettagli/DettaglioRaccolta";
import { QdCRaccoltaService } from "app/quaderno-di-campagna/agenda-edit/service/prodotti/raccolta.service";
import {KendoGridColumn, NumericSettings} from 'gias-kendo-grid';
import {BehaviorSubject} from 'rxjs';


@Injectable()
export class RaccoltaDataShareService {

    public formRipartizione: BehaviorSubject<FormGroup> = new BehaviorSubject<FormGroup>(null);
    public Prodotto: FormControl<DettaglioRaccolta>;

    //private floatRegex = /[+-]?([0-9]*[.])?[0-9]+/;

    public gridColumns = [
        new KendoGridColumn({ field:'APPEZ_Des', title: this.transloco.translate('qdc.Appezzamento')},{ editable: false }),
        new KendoGridColumn({ field:'Qta', title: this.transloco.translate('qdc.qta')},{ editable: true, hidden:true, numeric: new NumericSettings({
                defaultValue: 0, format: 'n4', min: 0, step: 0.0001, decimals: 4
            })}),
        new KendoGridColumn({ field:'Progetto_Cod', title: this.transloco.translate('Esercizio')},{ editable: false, hidden:true }),
      new KendoGridColumn({field: 'Progetto_Des', title: this.transloco.translate('LottoImpianto')}, {
        editable: false,
        hidden: true
      }),
        new KendoGridColumn({ field:'Lotto', title: this.transloco.translate('LottoDiAccettazione')},{ editable: true, hidden: true }),
        //new KendoGridColumn({ field:'Magazzino_Cod', title: this.transloco.translate('qdc.Magazzino')},{ editable: true }),
    ];

    constructor(
        private transloco: TranslocoService,
        private raccoltaService: QdCRaccoltaService
    ) {}

    public get Magazzino(): Fabbricato {
        let mov = this.Prodotto.value.MagazziniMovimentazioni;

        if (!! mov && mov.length > 0) {
            return mov[0].Magazzino;
        }

        return undefined;
    }
    public set Magazzino(magazzino: Fabbricato) {
        let prodotto = this.Prodotto.value as DettaglioRaccolta;
        let mov = prodotto.MagazziniMovimentazioni;

        if (!mov || !mov.length) return;

        // if (mov[0].Magazzino.primaryKey.codice === magazzino.primaryKey.codice) return;
        // mov[0].Magazzino = magazzino;

        mov.forEach(m => m.Magazzino = magazzino);

        this.Prodotto.patchValue(prodotto);
    }

}
