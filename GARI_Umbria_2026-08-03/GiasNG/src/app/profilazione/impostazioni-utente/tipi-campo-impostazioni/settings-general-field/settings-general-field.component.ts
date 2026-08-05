import { Component, Input, OnInit } from '@angular/core';
import { FormGroup, FormGroupDirective } from '@angular/forms';
import { enum_Impostazioni_Utenti } from '../../../../Model/Impostazioni_Utenti.enum';
import { ProfilazioneDataShareService } from "../../../services/profilazione-data-share.service";
import { ImpostazioniFormService } from "../../../services/impostazioni/impostazioni-form.service";
import { skip } from "rxjs/operators";
import { map } from "rxjs";

@Component({
  standalone: false,
  selector: 'app-settings-general-field',
  templateUrl: './settings-general-field.component.html',
  styleUrls: ['./settings-general-field.component.css']
})
export class GeneralFieldComponent implements OnInit {
  public readonly enum_Impostazioni = [
    enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE,
    enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE_APP,
    enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI,
    enum_Impostazioni_Utenti.FiltroDataScadenzaFarmaco,
    enum_Impostazioni_Utenti.UTENTE_InizioFineAnnataAgraria,
    enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_VARIETA,
    enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_SPECIE_VEGETALI,
    enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI,
    enum_Impostazioni_Utenti.UTENTE_COD_PROXY,
    enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_DPI,
    enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_UDM_DEFAULT,
    enum_Impostazioni_Utenti.SUPERUSER_GESTIONE_MAGAZZINO_ABILITATA,
    enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_IMPOSTAZIONI,
    enum_Impostazioni_Utenti.SUPERUSER_SETUP_GIS,
    enum_Impostazioni_Utenti.SuperUser_FiltroSQL_MateriePrime,
    enum_Impostazioni_Utenti.SuperUser_LayOut_Peso_DDT
  ];

  @Input() giasFormControlName: string;
  @Input() label: string = '';
  @Input() value: any;

  public form: FormGroup;
  public readonly hiddenSettings: number[] = [
    enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_DPI
  ];

  constructor(
    private rootFormGroup: FormGroupDirective,
    private settingsFromService: ImpostazioniFormService,
    private datashare: ProfilazioneDataShareService
  ) {
  }

  get Impostazione_Cod(): number | enum_Impostazioni_Utenti {
    return this.form?.value?.guida?.Impostazione_Cod || 0;
  }

  get isHiddenSetting(): boolean {
    return this.hiddenSettings.includes(this.Impostazione_Cod);
  }

  
  protected get isAnnataAgraria(): boolean {
    return this.Impostazione_Cod === enum_Impostazioni_Utenti.UTENTE_InizioFineAnnataAgraria;
  }

  protected get isFiltroVarietaSpecie(): boolean {
    return this.Impostazione_Cod === enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_VARIETA ||
      this.Impostazione_Cod === enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_SPECIE_VEGETALI;
  }

  protected get isFiltroOperazioni(): boolean {
    return this.Impostazione_Cod === enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI;
  }

  protected get isProxy(): boolean {
    return this.Impostazione_Cod === enum_Impostazioni_Utenti.UTENTE_COD_PROXY;
  }

  protected get isStorageCategoryGrid(): boolean {
    return [
      enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_UDM_DEFAULT,
      enum_Impostazioni_Utenti.SUPERUSER_GESTIONE_MAGAZZINO_ABILITATA,
      enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI,
      enum_Impostazioni_Utenti.FiltroDataScadenzaFarmaco,
      enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE,
      enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE_APP,
    ].includes(this.Impostazione_Cod);
  }

  protected get isApp(): boolean {
    return this.Impostazione_Cod === enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_IMPOSTAZIONI;
  }

  protected get isSetupGIS(): boolean {
    return this.Impostazione_Cod === enum_Impostazioni_Utenti.SUPERUSER_SETUP_GIS;
  }

  protected get isFiltroSQLMateriePrime(): boolean {
    return this.Impostazione_Cod === enum_Impostazioni_Utenti.SuperUser_FiltroSQL_MateriePrime;
  }

  protected get isFormatiStampe(): boolean {
    return this.Impostazione_Cod === enum_Impostazioni_Utenti.SuperUser_LayOut_Peso_DDT;
  }

  protected get isHandled(): boolean {
    return this.enum_Impostazioni.includes(this.Impostazione_Cod);
  }


  ngOnInit(): void {
    this.form = this.rootFormGroup?.form;
    if (this.isHiddenSetting) {
      this.handleHidden();
    }
  }

  /** Ricarica il componente interno responsabile della visualizzazione dell'impostazione.
   * @param applyValue Se specificato, viene applicato come valore corrente del form corrispondente nell'oggetto `formsMap` di {@link ProfilazioneDataShareService}
   * @UsageNotes
   * Necessario gestire il ricaricamento anche nei singoli componenti specializzati.
   * La chiamata a questa funzione fa emettere un nuovo valore a `reloadSetting$: Subject<string>`,
   * contenuto in {@link ProfilazioneDataShareService}. Il nuovo valore è una stringa riportante il
   * nome del componente specializzato da aggiornare.
   *
   * Nel caso in cui l'aggiornamento del componente specializzato non sia ancora stato gestito
   * viene emessa una stringa vuota.
   *
   * Componenti per ora è gestiti:
   * - {@link SettingsCategorieMagazzinoComponent}
   */
  public reloadComponent(applyValue?: any): void {
    switch (this.Impostazione_Cod) {
      case enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_UDM_DEFAULT:
      case enum_Impostazioni_Utenti.SUPERUSER_GESTIONE_MAGAZZINO_ABILITATA:
      case enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE:
      case enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI:
      case enum_Impostazioni_Utenti.FiltroDataScadenzaFarmaco:
        if (applyValue && this.datashare.formsMap.has(this.Impostazione_Cod)) {
          this.datashare.formsMap.get(this.Impostazione_Cod)
            .get("valoreCorrente").patchValue(applyValue);
        }
        this.datashare.reloadSetting$.next({
          setting: this.Impostazione_Cod,
          component: 'SettingsCategorieMagazzinoComponent'
        });
        break;
      default:
        this.datashare.reloadSetting$.next({ setting: this.Impostazione_Cod, component: '' });
    }
  }

  private handleHidden(): void {
    if (this.Impostazione_Cod === enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_DPI) {
      this.settingsFromService.findImpostazioneForm(enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_DPI_PREDEFINITO)
        .get("valoreCorrente").valueChanges.pipe(
          map(dpi => this.form.get("valoreCorrente").patchValue(dpi !== "0" ? 1 : 0)),
          skip(1) // Salto il primo valore => caricamento impostazioni, l'utente non ha ancora toccato nulla
        ).subscribe(() => {
          console.log("default dpi changed", this.form.value);
          this.form.get("valoreCorrente").markAsTouched();
        });
    }
  }

}
