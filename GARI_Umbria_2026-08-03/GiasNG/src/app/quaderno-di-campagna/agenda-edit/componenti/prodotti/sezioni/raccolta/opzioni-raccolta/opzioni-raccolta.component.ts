import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { TranslocoService } from '@jsverse/transloco';
import { BaseCodeDescr } from 'app/Model/baseClass/baseCodeDescr';
import { QdCRaccoltaService } from 'app/quaderno-di-campagna/agenda-edit/service/prodotti/raccolta.service';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { Subject, takeUntil } from 'rxjs';
import {
  enum_Generazione_Lotto_Raccolta,
  enum_Modalita_Raccolta,
  enum_Opzioni_Raccolta_Aggiornamento_Anagrafica,
  enum_Opzioni_Raccolta_Aggiornamento_Anagrafica as enum_Chiusura,
  enum_Ripartizione_Raccolta,
  OpzioniRaccolta
} from './opzioni-raccolta.model';
import {
  enum_SEMINA_TIPO,
  enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO
} from "../../../../../../../Model/TipiEnumerativi";

@Component({
  standalone: false,
  selector: 'app-opzioni-raccolta',
  templateUrl: './opzioni-raccolta.component.html',
  styleUrls: ['./opzioni-raccolta.component.css'],
  providers: [GiasDropDownTemplateService]
})
export class OpzioniRaccoltaComponent implements OnInit, OnDestroy {

	public form: FormGroup;
  signal: Subject<void> = new Subject<void>();

	ListChiusura: Array<BaseCodeDescr> = [
		new BaseCodeDescr(0, this.transloco.translate('qdc.LasciaImpiantiAttivi')),
		new BaseCodeDescr(1, this.transloco.translate('qdc.ChiusuraEsercizi')),
		new BaseCodeDescr(4, this.transloco.translate('qdc.ChiusuraAperturaEsercizi')),
		new BaseCodeDescr(2, this.transloco.translate('qdc.ChiusuraImpiantiEsercizi')),
		new BaseCodeDescr(5, this.transloco.translate('qdc.ChiusuraAperturaImpiantiEsercizi')),
		new BaseCodeDescr(3, this.transloco.translate('qdc.ChiusuraAppezzamentiImpiantiEsercizi')),
	];
	ListRipartizione: Array<BaseCodeDescr> = [
		new BaseCodeDescr(0, this.transloco.translate('qdc.RaccoltaAutomaticaSuperficie')),
		new BaseCodeDescr(1, this.transloco.translate('qdc.RaccoltaAutomaticaPiante')),
		new BaseCodeDescr(2, this.transloco.translate('qdc.Manuale')),
	];
	ListModalita: Array<BaseCodeDescr> = [
		new BaseCodeDescr(0, this.transloco.translate('qdc.Meccanica')),
		new BaseCodeDescr(1, this.transloco.translate('qdc.Manuale')),
	];
	ListLotto: Array<BaseCodeDescr> = [
		new BaseCodeDescr(0, this.transloco.translate('qdc.Manuale')),
		new BaseCodeDescr(1, this.transloco.translate('qdc.LottoDaData')),
		new BaseCodeDescr(2, this.transloco.translate('qdc.LottoUnivoco')),
		new BaseCodeDescr(3, this.transloco.translate('qdc.LottoDaEsercizio')),
	];

	constructor(private fb: FormBuilder,
		private transloco: TranslocoService,
		private raccoltaService: QdCRaccoltaService,
	) {
		this.initForm();
		this.disableControls();
	}

  public MostraOpzioniRaccolta(): boolean{
    let mostra:boolean = false;

    if(this.isNewRaccolta && !this.isImpostazione_Utente_con_Vincolo){
      mostra = true;
    }

    return mostra;
  }

  public get isImpostazione_Utente_con_Vincolo():boolean{
    return this.raccoltaService.qdcService.Opzione_Raccolta === enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.LASCIARE_IMPIANTI_ATTIVI_VINCOLO ||
            this.raccoltaService.qdcService.Opzione_Raccolta === enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_ESERCIZI_VINCOLO ||
            this.raccoltaService.qdcService.Opzione_Raccolta === enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_APRI_ESERCIZI_VINCOLO ||
            this.raccoltaService.qdcService.Opzione_Raccolta === enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_IMPIANTI_ESERCIZI_VINCOLO ||
            this.raccoltaService.qdcService.Opzione_Raccolta === enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_APRI_IMPIANTI_ESERCIZI_VINCOLO ||
            this.raccoltaService.qdcService.Opzione_Raccolta === enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_APPEZZAMENTI_IMPIANTI_ESERCIZI_VINCOLO
  }

	public get isNewRaccolta(): boolean {
		return this.raccoltaService.isNewRaccolta;
	}
  public get isNotRecipe(): boolean {
      return !this.raccoltaService.isRecipe;
  }
	public get lottiAttivi(): boolean {
		return this.raccoltaService.lottiAttivi;
	}
	public get carichiAttivi(): boolean {
		return this.raccoltaService.Raccolta_Con_Carico_Magazzino;
	}

	ngOnInit(): void {
		this.handleEvents();
	}

	ngOnDestroy(): void {
		this.signal.next();
    this.signal.complete();
	}

	private initForm() {
		this.form = this.raccoltaService.sezione_prodotto
						        .get('Opzioni_Raccolta') as FormGroup;
		this.form.get('Chiusura').patchValue(this.getDefault_Chiusura());
		this.form.addControl(
      'dataIngresso',
      new FormControl(this.raccoltaService.qdcService.TestataForm.get('Data')?.value)
    );

    if (this.isNewRaccolta) {
      this.form.get('GenerazioneLotto').patchValue(enum_Generazione_Lotto_Raccolta.DA_PROGETTO_COD);
      this.form.get('dataIngresso').disable();
    } else {
			this.loadOptionsData();
		}
    if (this.raccoltaService.isRecipe) {
      this.setRecipeOptions();
      return;
    }
		if (this.form.get('Ripartizione').value !== '')
      return;

		this.form.get('Ripartizione').patchValue(enum_Ripartizione_Raccolta.MANUALE);
		this.form.get('Modalita').patchValue(enum_Modalita_Raccolta.MECCANICA);
	}

  private loadOptionsData() {
    this.form.get('dataIngresso').patchValue(
      this.raccoltaService.sezione_prodotto.value.ProdottiRaccolti[0].dataIngresso
    );
    if (this.form.get('GenerazioneLotto').value !== enum_Generazione_Lotto_Raccolta.DA_PROGETTO_COD) {
      /* Generazione lotto aperta di default come MANUALE */
      this.form.get('GenerazioneLotto').patchValue(enum_Generazione_Lotto_Raccolta.MANUALE);
      this.form.get('GenerazioneLotto').disable();
    }
  }

  private setRecipeOptions() {
    this.form.get('Ripartizione').patchValue(enum_Ripartizione_Raccolta.AUTO_SUPERFICIE);
    this.form.get('GenerazioneLotto').patchValue(enum_Generazione_Lotto_Raccolta.DA_PROGETTO_COD);
    this.form.get('Modalita').patchValue(enum_Modalita_Raccolta.MECCANICA);
  }

	private handleEvents() {
		this.handleDateChange();
    this.raccoltaService.sezione_prodotto.valueChanges.subscribe(change => {
      if (this.form.get('dataIngresso').disabled && this.raccoltaService.isStorageSelected) {
          this.form.get('dataIngresso').enable();
      } else if (this.form.get('dataIngresso').enabled && !this.raccoltaService.isStorageSelected) {
          this.form.get('dataIngresso').disable();
      }
    });
	}

	private handleDateChange() {
    if (this.isNewRaccolta) {
      this.raccoltaService.qdcService.TestataForm.get('Data')?.valueChanges
        .pipe(takeUntil(this.signal))
        .subscribe(dataOp => {
          if (this.form.get('dataIngresso').pristine)
            this.form.get('dataIngresso').patchValue(dataOp);
        });
    }
    this.form.get('dataIngresso').valueChanges.GiasSubscribe(newDate => {
      let prodotti = this.raccoltaService.sezione_prodotto
                .get('ProdottiRaccolti') as FormArray;
      for (let ctrl of prodotti.controls) {
        let prodotto = ctrl.value;
        prodotto.dataIngresso = newDate;
        ctrl.patchValue(prodotto);
      }
    });
	}

	private disableControls() {
		if (!this.isNewRaccolta) this.form.get('dataIngresso').disable();

		this.form.get('GenerazioneLotto').disable();
		this.raccoltaService.sezione_prodotto.get('ProdottiRaccolti')
		.valueChanges.GiasSubscribe(prodotti => {
			let canChangeLotto = prodotti.map(dettaglio =>
				dettaglio.MagazziniMovimentazioni?.some(movimentazione =>
					movimentazione.Magazzino?.primaryKey?.codice
				)
			).some(isMagazzinoSelected => isMagazzinoSelected);

			if (canChangeLotto && this.form.get('GenerazioneLotto').disabled && this.raccoltaService.isNewRaccolta) {
				this.form.get('GenerazioneLotto').enable();
			} else if (!canChangeLotto && this.form.get('GenerazioneLotto').enabled) {
        // Riapplico il default e disabilito l'input
				this.form.get('GenerazioneLotto').patchValue(enum_Generazione_Lotto_Raccolta.DA_PROGETTO_COD);
				this.form.get('GenerazioneLotto').disable();
			}
		});

	}

  getDefault_Chiusura(){

    let default_chiusura = enum_Opzioni_Raccolta_Aggiornamento_Anagrafica.LASCIA_ATTIVI;

    switch (this.raccoltaService.qdcService.Opzione_Raccolta) {
      case enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.LASCIARE_IMPIANTI_ATTIVI_DEFAULT:
      case enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.LASCIARE_IMPIANTI_ATTIVI_VINCOLO:
        default_chiusura = enum_Opzioni_Raccolta_Aggiornamento_Anagrafica.LASCIA_ATTIVI;
        break;
      case enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_ESERCIZI_DEFAULT:
      case enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_ESERCIZI_VINCOLO:
        default_chiusura = enum_Opzioni_Raccolta_Aggiornamento_Anagrafica.CHIUDI_ESERCIZI;
        break;
      case enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_APRI_ESERCIZI_DEFAULT:
      case enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_APRI_ESERCIZI_VINCOLO:
        default_chiusura = enum_Opzioni_Raccolta_Aggiornamento_Anagrafica.CHIUDI_APRI_ESERCIZI;
        break;
      case enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_IMPIANTI_ESERCIZI_DEFAULT:
      case enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_IMPIANTI_ESERCIZI_VINCOLO:
        default_chiusura = enum_Opzioni_Raccolta_Aggiornamento_Anagrafica.CHIUDI_IMPIANTI_ESERCIZI;
        break;
      case enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_APRI_IMPIANTI_ESERCIZI_DEFAULT:
      case enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_APRI_IMPIANTI_ESERCIZI_VINCOLO:
        default_chiusura = enum_Opzioni_Raccolta_Aggiornamento_Anagrafica.CHIUDI_APRI_IMPIANTI_ESERCIZI;
        break;
      case enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_APPEZZAMENTI_IMPIANTI_ESERCIZI_DEFAULT:
      case enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_APPEZZAMENTI_IMPIANTI_ESERCIZI_VINCOLO:
        default_chiusura = enum_Opzioni_Raccolta_Aggiornamento_Anagrafica.CHIUDI_APPEZZAMENTI_IMPIANTI_ESERCIZI;
        break;
    }

    return default_chiusura;

  }

}
