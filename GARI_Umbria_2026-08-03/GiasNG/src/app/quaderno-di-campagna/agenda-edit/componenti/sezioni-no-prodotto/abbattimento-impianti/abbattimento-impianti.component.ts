import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup } from "@angular/forms";
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { QdCService } from "../../../service/qdc.service";
import { BaseCodeDescr } from "../../../../../Model/baseClass/baseCodeDescr";
import { TranslocoService } from "@jsverse/transloco";
import {
  enum_Opzioni_Raccolta_Aggiornamento_Anagrafica
} from "../../prodotti/sezioni/raccolta/opzioni-raccolta/opzioni-raccolta.model";
import { enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO } from "../../../../../Model/TipiEnumerativi";
import { Subject, takeUntil } from "rxjs";

@Component({
  standalone: false,
  selector: 'app-abbattimento-impianti',
  templateUrl: './abbattimento-impianti.component.html',
  styleUrls: ['./abbattimento-impianti.component.css'],
  providers: [GiasDropDownTemplateService]
})
export class AbbattimentoImpiantiComponent implements OnInit, OnDestroy {

  protected options: FormGroup;
  protected closingOptions: Array<BaseCodeDescr> = [
    // new BaseCodeDescr(0, this.transloco.translate('qdc.LasciaImpiantiAttivi')),
    // new BaseCodeDescr(1, this.transloco.translate('qdc.ChiusuraEsercizi')),
    // new BaseCodeDescr(4, this.transloco.translate('qdc.ChiusuraAperturaEsercizi')),
    new BaseCodeDescr(enum_Opzioni_Raccolta_Aggiornamento_Anagrafica.CHIUDI_IMPIANTI_ESERCIZI, this.transloco.translate('qdc.ChiusuraImpiantiEsercizi')),
    new BaseCodeDescr(enum_Opzioni_Raccolta_Aggiornamento_Anagrafica.CHIUDI_APRI_IMPIANTI_ESERCIZI, this.transloco.translate('qdc.ChiusuraAperturaImpiantiEsercizi')),
    new BaseCodeDescr(enum_Opzioni_Raccolta_Aggiornamento_Anagrafica.CHIUDI_APPEZZAMENTI_IMPIANTI_ESERCIZI, this.transloco.translate('qdc.ChiusuraAppezzamentiImpiantiEsercizi')),
  ];

  private $signal: Subject<void> = new Subject<void>();

  constructor(
    private qdcService: QdCService,
    private transloco: TranslocoService,
  ) { }

  ngOnInit(): void {
    this.initOptions();
    this.onOptionsChange();
    this.onTargetedSurfaceChange();
  }

  ngOnDestroy(): void {
    this.$signal.next();
    this.$signal.complete();
  }

  private initOptions(): void {
    this.options = new FormGroup({});
    this.options.addControl('closing', new FormControl(this.getClosingDefault()));
  }

  private onOptionsChange() {
    this.blockChangeIfConstrain();
    this.options.get('closing').valueChanges
      .pipe(takeUntil(this.$signal))
      .subscribe(change => this.qdcService.Opzione_Raccolta = change);
  }

  private onTargetedSurfaceChange() {
    this.qdcService.SuperficiForm.get('Sup_Trattata').valueChanges
      .pipe(takeUntil(this.$signal))
      .subscribe((next) => {
        console.log(next, this.isTargetingEntireSurface(next));
      });
  }

  private getClosingDefault() {
    let closingDefault = enum_Opzioni_Raccolta_Aggiornamento_Anagrafica.CHIUDI_APPEZZAMENTI_IMPIANTI_ESERCIZI;
    switch (this.qdcService.Opzione_Abbattimento) {
      case enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.LASCIARE_IMPIANTI_ATTIVI_DEFAULT:
      case enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.LASCIARE_IMPIANTI_ATTIVI_VINCOLO:
        closingDefault = enum_Opzioni_Raccolta_Aggiornamento_Anagrafica.LASCIA_ATTIVI;
        break;
      case enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_ESERCIZI_DEFAULT:
      case enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_ESERCIZI_VINCOLO:
        closingDefault = enum_Opzioni_Raccolta_Aggiornamento_Anagrafica.CHIUDI_ESERCIZI;
        break;
      case enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_APRI_ESERCIZI_DEFAULT:
      case enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_APRI_ESERCIZI_VINCOLO:
        closingDefault = enum_Opzioni_Raccolta_Aggiornamento_Anagrafica.CHIUDI_APRI_ESERCIZI;
        break;
      case enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_IMPIANTI_ESERCIZI_DEFAULT:
      case enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_IMPIANTI_ESERCIZI_VINCOLO:
        closingDefault = enum_Opzioni_Raccolta_Aggiornamento_Anagrafica.CHIUDI_IMPIANTI_ESERCIZI;
        break;
      case enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_APRI_IMPIANTI_ESERCIZI_DEFAULT:
      case enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_APRI_IMPIANTI_ESERCIZI_VINCOLO:
        closingDefault = enum_Opzioni_Raccolta_Aggiornamento_Anagrafica.CHIUDI_APRI_IMPIANTI_ESERCIZI;
        break;
      case enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_APPEZZAMENTI_IMPIANTI_ESERCIZI_DEFAULT:
      case enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_APPEZZAMENTI_IMPIANTI_ESERCIZI_VINCOLO:
        closingDefault = enum_Opzioni_Raccolta_Aggiornamento_Anagrafica.CHIUDI_APPEZZAMENTI_IMPIANTI_ESERCIZI;
        break;
    }
    return closingDefault;
  }

  private blockChangeIfConstrain() {
    const constrained = [
      enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.LASCIARE_IMPIANTI_ATTIVI_VINCOLO,
      enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_ESERCIZI_VINCOLO,
      enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_APRI_ESERCIZI_VINCOLO,
      enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_IMPIANTI_ESERCIZI_VINCOLO,
      enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_APRI_IMPIANTI_ESERCIZI_VINCOLO,
      enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_APPEZZAMENTI_IMPIANTI_ESERCIZI_VINCOLO
    ];
    if (constrained.includes(this.qdcService.Opzione_Abbattimento)) {
      this.options.get('closing').disable();
    }
  }

  private isTargetingEntireSurface(targetedSurface?: number): boolean {
    const total = this.qdcService.superficieImpiantiSelezionati;
    const target = targetedSurface ?? this.qdcService.SuperficiForm.value.Sup_Trattata;
    return total === target;
  }
}
