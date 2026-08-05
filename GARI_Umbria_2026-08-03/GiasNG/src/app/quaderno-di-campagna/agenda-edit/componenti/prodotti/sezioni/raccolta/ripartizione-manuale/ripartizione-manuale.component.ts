import { Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormControl, FormGroup } from '@angular/forms';
import { TranslocoService } from '@jsverse/transloco';
import { Fabbricato } from 'app/Model/anagrafiche/Fabbricato';
import { Tipo_Raccolta } from 'app/Model/attivita/Attivita';
import { DettaglioRaccolta } from 'app/Model/attivita/dettagli/DettaglioRaccolta';
import { QuantitaSuImpianto } from 'app/Model/attivita/dettagli/QuantitaSuImpianto';
import { RilevamentoDiMagazzino } from 'app/Model/attivita/RilevamentoDiMagazzino';
import { Prodotto } from 'app/Model/attivita/risorse/Prodotto';
import { UnitaDiMisura } from 'app/Model/metaschema/UnitaDiMisura';
import { QdCRaccoltaService } from 'app/quaderno-di-campagna/agenda-edit/service/prodotti/raccolta.service';
import { MasterService } from 'app/Service/master.service';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { GiasDropDownTemplateSComponent } from 'gias-ui-kit';
import { GiasDropDownTemplateService } from 'gias-ui-kit';
import { isEqual } from 'lodash';
import { RaccoltaDataShareService } from '../data-share.service';
import { enum_Generazione_Lotto_Raccolta, OpzioniRaccolta } from '../opzioni-raccolta/opzioni-raccolta.model';
import {QdCService} from "../../../../../service/qdc.service";
import {filter, from, map, Observable, Subject, takeUntil, tap} from "rxjs";

@Component({
  standalone: false,
  selector: 'app-ripartizione-manuale',
  templateUrl: './ripartizione-manuale.component.html',
  styleUrls: ['./ripartizione-manuale.component.css'],
  providers: [GiasDropDownTemplateService, RaccoltaDataShareService]
})
export class RipartizioneManualeComponent implements OnInit, OnDestroy {
  @ViewChild('grid') grid;
  @Input() Prodotto: FormControl<DettaglioRaccolta>;
  @Input() index: number;

  public form: FormGroup;
  public carichiAttivi = true;
  public isProductSelected = false;
  public isStorageSelected = false;

  private _signal$ = new Subject<void>();

  public get isInInfoMode() {
    return this.qdcservice.Sola_Lettura_QdCForm();
  }

  constructor(
    private translocoService: TranslocoService,
    private raccoltaService: QdCRaccoltaService,
    private dataShare: RaccoltaDataShareService,
    private master: MasterService,
    private qdcservice: QdCService
  ) {
    this.carichiAttivi = this.raccoltaService.Raccolta_Con_Carico_Magazzino;
  }

  ngOnInit(): void {
    this.dataShare.Prodotto = this.Prodotto;
    this.initForm();

    this.handleVisibility();

    this.raccoltaService.loadEndEvent.pipe(
      takeUntil(this._signal$),
      filter(end => !!end)
    ).subscribe(end => this.manageMagazziniVisibility());
    this.handleFormChanges();
  }

  ngOnDestroy(): void {
    this._signal$.next();
    this._signal$.complete();
  }

  /** Removes the current product section. */
  public onRemove() {
    this.raccoltaService.removeDettaglioRaccolta(this.index);
  }

  /** Adds a new product section at the head of the list. */
  public onAdd() {
    this.raccoltaService.addNewRaccoltaAt(0);
  }

  public onOpenDdl(ddlEl: GiasDropDownTemplateSComponent) {
    ddlEl.loading = true;
    switch (ddlEl.id) {
      case 'prodotto':
        this.loadProducts().subscribe(products => ddlEl.listItems = products);
        break;
      case 'udm':
        this.raccoltaService.leggiUDM().subscribe(udms => ddlEl.listItems = udms);
        break;
      case 'magazzino':
        this.loadMagazzini().subscribe(items => ddlEl.listItems = items);
        break;
    }
    ddlEl.loading = false;
  }

  private loadProducts(): Observable<Prodotto[]> {
    let newItem: Prodotto;
    return from(this.raccoltaService.leggiProdotti())
      .pipe(map(listItems => {
        if (!listItems.length) {
          newItem = new Prodotto(-1, this.translocoService.translate('qdc.ProdottoDaSpecie'));
          listItems.push(newItem);
        }
        newItem = new Prodotto(0, this.translocoService.translate('qdc.NessunaSelezione'));
        listItems.unshift(newItem);
        return listItems;
      }));
  }

  private loadMagazzini(): Observable<Fabbricato[]> {
    const newItem: Fabbricato = new Fabbricato({
      codice: 0, centroAziendalePK: {
        codice: 0,
        partitaIva: this.master.objP_server.PivaSuperUser
      }
    });
    newItem.descrizione = this.translocoService.translate('qdc.NessunaSelezione');
    return this.raccoltaService.leggiMagazzini().pipe(map(listItems => {
      listItems.unshift(newItem);
      return listItems;
    }));
  }

  /** Set the property `carichiAttivi` according to the settings.
   * If the setting is disabled, the storage details won't be shown
   * even if the harvest was previously registered with some of them.
   * See also: {@link QdCRaccoltaService.Raccolta_Con_Carico_Magazzino}
   */
  private manageMagazziniVisibility() {
    this.carichiAttivi = this.raccoltaService.Raccolta_Con_Carico_Magazzino;
  }

  /** Initializes the form used for the repartition and loads the values for the ddls. */
  private initForm(): void {
    this.form = new FormGroup({
      Prodotto: new FormControl(this.Prodotto.value.prodotto),
      UM: new FormControl(this.Prodotto.value.unitaDiMisura),
      Qta_Tot: new FormControl(this.getQta()),
      Magazzino: new FormControl(this.dataShare.Magazzino),
    });
    this.loadUDMDescription();
    this.loadProductDescription();
    this.loadMagazzinoDescription();
    this.sumQuantities();
    this.dataShare.formRipartizione.next(this.form);
  }

  private handleFormChanges() {
    let old = this.form.value;
    this.form.valueChanges.pipe(
      takeUntil(this._signal$),
      filter(formValue => !isEqual(old, formValue)),
      tap(formValue => old = formValue)
    ).subscribe(formValue => {
      let raccolta = this.Prodotto.value as DettaglioRaccolta;
      this.updateFormvalueInRaccolta(raccolta);
      this.handleVisibility();
      this.handleTipoRaccolta();
      this.updateMovimentazioni(raccolta);
      this.updateQuantitaSuImpianti(raccolta);
      this.Prodotto.patchValue(raccolta);
      this.raccoltaService.stopEditing();
      if (formValue.Prodotto.codice === 0) {
        this.form.get('Qta_Tot').patchValue(0);
      }
    });
  }

  private updateFormvalueInRaccolta(R: DettaglioRaccolta) {
    R.unitaDiMisura = this.form.value.UM;
    R.prodotto = this.form.value.Prodotto;
    if (!R.prodotto.codice) {
      this.form.get('Magazzino').patchValue(null);
      R.MagazziniMovimentazioni = [];
      R.quantitaTotaleReale = 0;
      R.QuantitaSuImpianti.forEach(q => q.Qta = 0);
      this.isStorageSelected = false;
      this.isProductSelected = false;
      return;
    }
    R.quantitaTotaleReale = this.form.value.Qta_Tot;
  }

  private updateMovimentazioni(prd: DettaglioRaccolta) {
    if (this.isStorageSelected && this.carichiAttivi
      && (!prd.MagazziniMovimentazioni || !prd.MagazziniMovimentazioni.length)
    ) {
      let mov = new RilevamentoDiMagazzino();
      prd.MagazziniMovimentazioni = [mov];
    }
    if (!prd.MagazziniMovimentazioni) return prd;

    prd.MagazziniMovimentazioni.forEach((mov: RilevamentoDiMagazzino) => {
      mov.Magazzino = this.form.value.Magazzino;
      mov.Prodotto = this.form.value.Prodotto;
      mov.udm = this.form.value.UM;
      mov.Qta = this.form.value.Qta_Tot;
    });
    return prd;
  }

  private updateQuantitaSuImpianti(prd: DettaglioRaccolta) {
    if (!!prd.QuantitaSuImpianti) {
      prd.QuantitaSuImpianti.forEach((q: QuantitaSuImpianto) => q.Magazzino = this.form.value.Magazzino);
    }
    return prd;
  }

  private handleTipoRaccolta() {
    let prodotto = this.Prodotto.value as DettaglioRaccolta;

    prodotto.TipoRaccolta = Tipo_Raccolta.Fast;
    if (this.isProductSelected) {
      prodotto.TipoRaccolta = Tipo_Raccolta.Leggera;
    }
    if (this.isProductSelected && this.isStorageSelected && this.carichiAttivi) {
      prodotto.TipoRaccolta = Tipo_Raccolta.Leggera_Con_Dettagli_Magazzino;
    }

    this.Prodotto.patchValue(prodotto);
  }

  private loadUDMDescription() {
    let UDM: UnitaDiMisura = this.Prodotto.value.unitaDiMisura;
    if (UDM) {
      this.raccoltaService.leggiUDM().pipe(
        map((list: UnitaDiMisura[]) => {
          const um = list.find(item => item.codice === UDM.codice);
          return !um ? new UnitaDiMisura(0, '') : um;
        }),
        tap((um: UnitaDiMisura) => {
          UDM.descrizione = um.descrizione;
          this.form.get('UM').patchValue(UDM);
        })
      ).subscribe(() => {
        let prodotto = this.Prodotto.value;
        this.Prodotto.patchValue(prodotto);
      });
    }
  }

  private loadProductDescription() {
    let prodotto: Prodotto = this.Prodotto.value.prodotto;
    if (!prodotto) return;
    if (!prodotto.codice) {
      prodotto.descrizione = this.translocoService.translate("NessunaSelezione");
      this.form.get('Prodotto').patchValue(prodotto);
      return;
    }
    this.raccoltaService.leggiProdotti().then((list) => {
      let p = list.find(item => item.codice === prodotto.codice);
      prodotto.descrizione = p.descrizione;
      this.form.get('Prodotto').patchValue(prodotto);
    });
  }

  private loadMagazzinoDescription() {
    let D = this.raccoltaService.sezione_prodotto.get('ProdottiRaccolti')
      .value.at(this.index) as DettaglioRaccolta;

    if (!D.MagazziniMovimentazioni || !D.MagazziniMovimentazioni[0]
      || !D.MagazziniMovimentazioni[0].Magazzino?.primaryKey?.codice) return;

    let chiaveMagazzino = D.MagazziniMovimentazioni[0].Magazzino.primaryKey.codice
      + "_" + D.MagazziniMovimentazioni[0].Magazzino.primaryKey.centroAziendalePK.codice
      + "_" + D.MagazziniMovimentazioni[0].Magazzino.primaryKey.centroAziendalePK.partitaIva;

    this.raccoltaService.leggiMagazzini().subscribe(list => {
      let mag = list.find(m => m['chiaveMagazzino'] === chiaveMagazzino);
      this.form.get('Magazzino').patchValue(mag);
    });
  }

  private showQtaColumn() {
    let cols = this.dataShare.gridColumns;
    cols.find(col => col.field === 'Qta').hidden = !this.isProductSelected;
  }

  private showLottoColumn() {
    let cols = this.dataShare.gridColumns;
    const hideLotto = !this.isStorageSelected || !this.carichiAttivi
      || this.raccoltaService.isRecipe || !this.raccoltaService.lottiAttivi;

    cols.find(c => c.field === 'Lotto').editable = this.raccoltaService.isLottoFromProg || this.raccoltaService.isLottoManuale;
    cols.find(c => c.field === 'Lotto').hidden = hideLotto;
    cols.find(c => c.field === 'Progetto_Cod').hidden = hideLotto || !this.raccoltaService.isLottoFromProg;
    cols.find(c => c.field === 'Progetto_Des').hidden = hideLotto || !this.raccoltaService.isLottoFromProg;
  }

  private handleVisibility() {
    this.isProductSelected = !!this.form.value.Prodotto && !!this.form.value.Prodotto?.codice;
    this.isStorageSelected = !!this.form.value.Magazzino && !!this.form.value.Magazzino?.primaryKey?.codice;

    this.raccoltaService.sezione_prodotto
      .get('Magazzino_del_Prodotto_Selezionato')
      .patchValue(this.isStorageSelected);

    this.showQtaColumn();
    if (this.raccoltaService.lottiAttivi) {
      this.showLottoColumn();
    }
  }

  /**
   * Calculates the total quantity of the harvested product.
   * @returns the summed quantity of the harvested product.
   * @private
   */
  private getQta(): number {
    if (!!this.Prodotto.value.quantitaTotaleReale)
      return this.Prodotto.value.quantitaTotaleReale;

    if (this.Prodotto.value.QuantitaSuImpianti?.length > 0) {
      return this.Prodotto.value.QuantitaSuImpianti.map(q => q.Qta)
        .reduce((acc, qta) => acc + qta);
    }
    return 0;
  }

  /**
   * Updates the total quantity of the harvested product in the parent form.
   * @private
   */
  private sumQuantities(): void {
    let raccolta = this.Prodotto.value;
    if (this.raccoltaService.sezione_prodotto.get('Opzioni_Raccolta').get('Ripartizione').pristine)
      return;

    let qtaTot = raccolta.QuantitaSuImpianti.map(q => q.Qta)
      .reduce((acc, qta) => acc + qta);

    this.Prodotto.patchValue(raccolta);
    this.form.get('Qta_Tot').patchValue(qtaTot);
  }

}
