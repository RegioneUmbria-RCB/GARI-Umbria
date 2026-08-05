import {
  AfterContentInit,
  Component,
  ContentChildren,
  Input,
  OnDestroy,
  OnInit,
  QueryList,
  TemplateRef
} from '@angular/core';
import {FormArray, FormBuilder, FormControl, FormGroup} from '@angular/forms';
import {map, Subject, take} from 'rxjs';
import {ImpostazioniFormService} from '../../services/impostazioni/impostazioni-form.service';
import {SezioneImpostazioni} from '../../models/Impostazioni/SezioneImpostazioni.model';
import {TabStripTabComponent} from '@progress/kendo-angular-layout';

@Component({
  standalone: false,
  selector: 'app-form-impostazioni',
  templateUrl: './form-impostazioni.component.html',
  styleUrls: ['./form-impostazioni.component.css']
})
export class FormImpostazioniComponent implements OnInit, AfterContentInit, OnDestroy {
  @Input('disable') disable = false;
  @ContentChildren(TabStripTabComponent) extraTabs: QueryList<TabStripTabComponent>;

  public settingsForm: FormGroup;
  public editSectionsForm = new FormGroup({
    sezione: new FormControl('')
  })
  protected projectedTabs: { title: string; content: TemplateRef<any> }[] = [];
  private signal = new Subject();
  /** Contiene le sezioni e sottosezioni del form.
   * La chiave è composta da: codiceSezione_codiceSottosezione_codiceLivello
   */
  private _mapSezioni = new Map<string, FormGroup>();

  constructor(
    private fb: FormBuilder,
    private impostazioniService: ImpostazioniFormService,
  ) {
  }

  public get MASTER_EDIT(): boolean {
    return this.impostazioniService.MASTER_EDIT || false;
  }

  public get sezioniArray(): FormArray {
    return this.settingsForm.get('sezioni') as FormArray;
  }

  private get settingsFormValue() {
    return this.settingsForm.value;
  }

  ngOnInit(): void {
    this.loadSettings();
  }

  ngAfterContentInit() {
    this.projectedTabs = this.extraTabs?.map(tab => ({
      title: tab.title,
      content: tab['_tabContent'].toArray()[0].templateRef
    })) ?? [];
  }

  ngOnDestroy(): void {
    this.signal.next(true);
    this.signal.complete();
  }

  public loadSettings() {
    this.initForm()
    this.impostazioniService.datashare.formsMap.clear();
    this.caricaImpostazoni().subscribe(() => {
      this.impostazioniService.settingsForm = this.settingsForm;
      if (this.disable) {
        this.settingsForm.disable()
      }
    })
  }

  private initForm() {
    this.settingsForm = this.fb.group({
      /** username utente a cui sono riferite le impostazioni */
      username: this.fb.array(this.impostazioniService.utentiSelezionati),
      sezioni: this.fb.array([])
    })
    this._mapSezioni = new Map<string, FormGroup>();
  }

  private getSectionForm(codice: number): FormGroup {
    return this._mapSezioni.get(codice.toString());
  }

  private getSubsectionForm(sottosezioneCod: number, sezioneCod: number): FormGroup {
    return this._mapSezioni.get(sezioneCod + '_' + sottosezioneCod);
  }

  private getLevelForm(codice: number, sottosezioneCod: number, sezioneCod: number): FormGroup {
    return this._mapSezioni.get(sezioneCod + '_' + sottosezioneCod + '_' + codice);
  }

  /**
   * Carica la struttura del form impostazioni
   * @private
   */
  private caricaImpostazoni() {
    return this.impostazioniService.leggiSezioniImpostazioni().pipe(take(1), map(impostazioni => {
      this.parseSettings(impostazioni);
    }))
  }

  ///////////////////////////////////////////////////////////////////////////////////////

  private parseSettings(toParse: any[]) {
    for (let imp of toParse) {
      let sez = new SezioneImpostazioni(imp.Sezione_Cod, imp.Sezione_Des)
      let sub = new SezioneImpostazioni(
        imp.SottoSezione_Cod,
        imp.SottoSezione_Des
          ? imp.SottoSezione_Des
          : "" //this.transloco.translate("Categoria") + " " + imp.SottoSezione_Cod
      )
      sub.espandibile = imp.SottoSezione_Espandibile === 1;
      let lvl = new SezioneImpostazioni(imp.Livello_Cod, imp.Livello_Des)
      this.addLevelIfNotPresent(sez, sub, lvl);
      this.addSettingToForm(imp)
    }
  }

  private addSectionIfNotPresent(section: SezioneImpostazioni) {
    let found = this.getSectionForm(section.codice)
    if (!found) {
      const sectionForm = section.toForm();
      (this.settingsForm.get('sezioni') as FormArray).push(sectionForm)
      this._mapSezioni.set(section.codice.toString(), sectionForm);
    }
  }

  private addSubsectionIfNotPresent(section: SezioneImpostazioni, subsection: SezioneImpostazioni) {
    const found = this.getSubsectionForm(subsection.codice, section.codice);
    if (!found) {
      const subsectionForm = subsection.toForm();
      this.addSectionIfNotPresent(section);
      const sec = this.getSectionForm(section.codice);
      (sec.get('children') as FormArray).push(subsectionForm);
      this._mapSezioni.set(section.codice + '_' + subsection.codice, subsectionForm);
    }
  }

  private addLevelIfNotPresent(section: SezioneImpostazioni, subsection: SezioneImpostazioni, level: SezioneImpostazioni) {
    const found = this.getLevelForm(level.codice, subsection.codice, section.codice);
    if (!found) {
      const levelForm = level.toForm();
      this.addSubsectionIfNotPresent(section, subsection);
      const sub = this.getSubsectionForm(subsection.codice, section.codice);
      (sub.get('children') as FormArray).push(level.toForm());
      this._mapSezioni.set(section.codice + '_' + subsection.codice + '_' + level.codice, levelForm);
    }
  }

  private addSettingToForm(setting: any) {
    let level = this.getLevelForm(setting.Livello_Cod, setting.SottoSezione_Cod, setting.Sezione_Cod);
    if (level) {
      const form = new FormGroup({
        guida: new FormControl(setting),
        valori: new FormControl(''),
        valoreCorrente: new FormControl(setting.Valore_Default)
      });
      level = ((((this.settingsForm.get('sezioni') as FormArray).controls.find(s => s.value.codice === setting.Sezione_Cod)
        .get('children') as FormArray).controls.find(s => s.value.codice === setting.SottoSezione_Cod)
        .get('children') as FormArray).controls.find(s => s.value.codice === setting.Livello_Cod) as FormGroup);
      (level.get('impostazioni') as FormArray).push(form);
      this._mapSezioni.set(setting.Sezione_Cod + '_' + setting.SottoSezione_Cod + '_' + setting.Livello_Cod, level);
    }
  }

}
