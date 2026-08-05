import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { FormControl, FormGroup } from "@angular/forms";
import { enum_TipoControllo } from 'gias-ui-kit';
import { BaseCodeDescr } from "../../../../Model/baseClass/baseCodeDescr";
import { enum_LayoutFormatiStampaDoc } from "../../impostazioni.model";
import { TranslocoService } from "@jsverse/transloco";
import { map, Subject, take, takeUntil } from "rxjs";
import { enum_Impostazioni_Utenti } from "../../../../Model/Impostazioni_Utenti.enum";
import { AjaxAgronicaAPIService } from "../../../../Service/ajax-agronica.api.service";

@Component({
  standalone: false,
  selector: 'impostazione-formati-stampa',
  templateUrl: './formati-stampa.component.html',
  styleUrls: ['./formati-stampa.component.css']
})
export class FormatiStampaComponent implements OnInit, OnDestroy {
  @Input() formGroup: FormGroup;

  protected readonly TIPO_CONTROLLO = enum_TipoControllo.MENU_DISCESA;
  protected readonly listItems = [
    new BaseCodeDescr(enum_LayoutFormatiStampaDoc.STANDARD, this.transloco.translate('prof.LayoutStampeStandard')),
    new BaseCodeDescr(enum_LayoutFormatiStampaDoc.PESO, this.transloco.translate('prof.LayoutStampePeso')),
    new BaseCodeDescr(enum_LayoutFormatiStampaDoc.RISCONTRO_TOTALE, this.transloco.translate('prof.LayoutStampeRiscontratoTotale')),
    new BaseCodeDescr(enum_LayoutFormatiStampaDoc.RISCONTRO, this.transloco.translate('prof.LayoutStampeRiscontrato')),
    new BaseCodeDescr(enum_LayoutFormatiStampaDoc.PREZZO, this.transloco.translate('prof.LayoutStampePrezzo')),
  ];
  protected form = new FormGroup({
    valoreCorrente: new FormControl(0)
  });

  private signal = new Subject();

  constructor(private transloco: TranslocoService,
    private apiService: AjaxAgronicaAPIService
  ) { }

  private get impostazioneCod(): number {
    return this.formGroup.get('guida').value.Impostazione_Cod;
  }

  ngOnInit(): void {
    console.log(this.formGroup);
    this.apiService.ajaxAPIPost<any, BaseCodeDescr[]>("/MetaschemaNG/CaricaDatiDDL", {
      User: '', Impostazione_Cod: this.impostazioneCod
    }).pipe(take(1), map(R => R.RispostaStringa))
      .subscribe(valImp => {
        this.parseValue(valImp);
        this.handleEvents();
      });
  }

  ngOnDestroy(): void {
    this.signal.next(true);
    this.signal.complete();
  }

  private handleEvents(): void {
    this.form.valueChanges.pipe(takeUntil(this.signal))
      .subscribe(ch => this.patchValue(ch.valoreCorrente));
  }

  private parseValue(items: BaseCodeDescr[]) {
    // const peso = +items.find(v => v.codice === enum_Impostazioni_Utenti.SuperUser_LayOut_Peso_DDT)?.descrizione ?? 0;
    // const prezzo = +items.find(v => v.codice === enum_Impostazioni_Utenti.SuperUser_LayOut_Prezzo_DDT)?.descrizione ?? 0;
    // const riscontro = +items.find(v => v.codice === enum_Impostazioni_Utenti.SuperUser_LayOut_Riscontrato_DDT)?.descrizione ?? 0;
    const peso = +items.find(v => v.codice === enum_Impostazioni_Utenti.SuperUser_LayOut_Peso_DDT)?.descrizione;
    const prezzo = +items.find(v => v.codice === enum_Impostazioni_Utenti.SuperUser_LayOut_Prezzo_DDT)?.descrizione;
    const riscontro = +items.find(v => v.codice === enum_Impostazioni_Utenti.SuperUser_LayOut_Riscontrato_DDT)?.descrizione;
    if (peso && !prezzo && !riscontro) {
      this.form.get("valoreCorrente").patchValue(enum_LayoutFormatiStampaDoc.PESO);
    } else if (!peso && !prezzo && riscontro === 2) {
      this.form.get("valoreCorrente").patchValue(enum_LayoutFormatiStampaDoc.RISCONTRO_TOTALE);
    } else if (!peso && !prezzo && riscontro === 1) {
      this.form.get("valoreCorrente").patchValue(enum_LayoutFormatiStampaDoc.RISCONTRO);
    } else if (!peso && prezzo && !riscontro) {
      this.form.get("valoreCorrente").patchValue(enum_LayoutFormatiStampaDoc.PREZZO);
    } else {
      this.form.get("valoreCorrente").patchValue(enum_LayoutFormatiStampaDoc.STANDARD);
    }
    this.formGroup.get("valoreCorrente").patchValue(peso + "|" + prezzo + "|" + riscontro);
  }

  private patchValue(value: number) {
    const control = this.formGroup.get("valori");
    const items = control.value;
    let setting;
    switch (value) {
      case enum_LayoutFormatiStampaDoc.PESO: //100
        setting = items.find(v => v.codice == enum_Impostazioni_Utenti.SuperUser_LayOut_Peso_DDT);
        setting.valore = '1';
        setting = items.find(v => v.codice == enum_Impostazioni_Utenti.SuperUser_LayOut_Prezzo_DDT);
        setting.valore = '0';
        setting = items.find(v => v.codice == enum_Impostazioni_Utenti.SuperUser_LayOut_Riscontrato_DDT);
        setting.valore = '0';
        this.formGroup.get("valoreCorrente").patchValue("1|0|0");
        break;
      case enum_LayoutFormatiStampaDoc.RISCONTRO_TOTALE: //002
        setting = items.find(v => v.codice == enum_Impostazioni_Utenti.SuperUser_LayOut_Peso_DDT);
        setting.valore = '0';
        setting = items.find(v => v.codice == enum_Impostazioni_Utenti.SuperUser_LayOut_Prezzo_DDT);
        setting.valore = '0';
        setting = items.find(v => v.codice == enum_Impostazioni_Utenti.SuperUser_LayOut_Riscontrato_DDT);
        setting.valore = '2';
        this.formGroup.get("valoreCorrente").patchValue("0|0|2");
        break;
      case enum_LayoutFormatiStampaDoc.RISCONTRO: //001
        setting = items.find(v => v.codice == enum_Impostazioni_Utenti.SuperUser_LayOut_Peso_DDT);
        setting.valore = '0';
        setting = items.find(v => v.codice == enum_Impostazioni_Utenti.SuperUser_LayOut_Prezzo_DDT);
        setting.valore = '0';
        setting = items.find(v => v.codice == enum_Impostazioni_Utenti.SuperUser_LayOut_Riscontrato_DDT);
        setting.valore = '1';
        this.formGroup.get("valoreCorrente").patchValue("0|0|1");
        break;
      case enum_LayoutFormatiStampaDoc.PREZZO: //001
        setting = items.find(v => v.codice == enum_Impostazioni_Utenti.SuperUser_LayOut_Peso_DDT);
        setting.valore = '0';
        setting = items.find(v => v.codice == enum_Impostazioni_Utenti.SuperUser_LayOut_Prezzo_DDT);
        setting.valore = '1';
        setting = items.find(v => v.codice == enum_Impostazioni_Utenti.SuperUser_LayOut_Riscontrato_DDT);
        setting.valore = '0';
        this.formGroup.get("valoreCorrente").patchValue("0|0|1");
        break;
      default: //000
        setting = items.find(v => v.codice == enum_Impostazioni_Utenti.SuperUser_LayOut_Peso_DDT);
        setting.valore = '0';
        setting = items.find(v => v.codice == enum_Impostazioni_Utenti.SuperUser_LayOut_Prezzo_DDT);
        setting.valore = '0';
        setting = items.find(v => v.codice == enum_Impostazioni_Utenti.SuperUser_LayOut_Riscontrato_DDT);
        setting.valore = '0';
        this.formGroup.get("valoreCorrente").patchValue("0|0|0");
    }
    control.patchValue(items);
    this.formGroup.markAsTouched();
  }
}
