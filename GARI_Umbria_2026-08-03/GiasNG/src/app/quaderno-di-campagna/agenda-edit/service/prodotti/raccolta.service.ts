import { Injectable } from "@angular/core";
import { FormArray, FormControl, FormGroup } from "@angular/forms";
import { TranslocoService } from "@jsverse/transloco";
import { CentroAziendale } from "app/Model/anagrafiche/CentroAziendale";
import { Fabbricato } from "app/Model/anagrafiche/Fabbricato";
import { Tipo_Raccolta } from "app/Model/attivita/Attivita";
import { DettaglioRaccolta } from "app/Model/attivita/dettagli/DettaglioRaccolta";
import { QuantitaSuImpianto } from "app/Model/attivita/dettagli/QuantitaSuImpianto";
import { RilevamentoDiMagazzino } from "app/Model/attivita/RilevamentoDiMagazzino";
import { Prodotto } from "app/Model/attivita/risorse/Prodotto";
import { RisorsaProdotto } from "app/Model/attivita/risorse/RisorsaProdotto";
import { TRASFORMATI_VEGETALI } from "app/Model/CostantiPersonalizzate";
import { UnitaDiMisura } from "app/Model/metaschema/UnitaDiMisura";
import { enum_LAVCOD, enum_UnitaMisura } from "app/Model/TipiEnumerativi";
import { CategorieMagazzinoService } from "app/profilazione/services/categorie-magazzino.service";
import { AjaxAgronicaService } from "app/Service/ajax-agronica.service";
import { CentriAziendaliService } from "app/Service/Anagrafica/centri.service";
import { FabbricatiService, LeggiMagazzini_QdC } from "app/Service/Anagrafica/fabbricati.service";
import { ProdottiService, LeggiProdotti } from "app/Service/Anagrafica/prodotti.service";
import { MasterService } from "app/Service/master.service";
import { LeggiUnitaDiMisura, UnitaDiMisuraService } from "app/Service/Metaschema/UnitaDiMisura.service";
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import {cloneDeep, isNumber} from 'lodash';
import {BehaviorSubject, lastValueFrom, map, Observable, of, take} from "rxjs";
import { enum_Generazione_Lotto_Raccolta } from "../../componenti/prodotti/sezioni/raccolta/opzioni-raccolta/opzioni-raccolta.model";
import { GridRaccoltaObject } from "../../componenti/prodotti/sezioni/raccolta/raccolta.model";
import { GridImpiantoSelezionatoModel } from "../../quaderno-di-campagna-form/quaderno-di-campagna-form.model";
import { QdCService } from "../qdc.service";
import {MovimentoDiMagazzino} from '../../../../Service/api.service';
import {BaseCodeDescr} from "../../../../Model/baseClass/baseCodeDescr";

const ID_NUOVA_ATTIVITA = '0';
const linkFabbricati = 'Anagrafica/Fabbricati.asmx/';

@Injectable()
export class QdCRaccoltaService {

    /**
     * Se attivo (Sì)(default): posso fare una raccolta inizialmente senza carichi con
     * la possibilità di aggiungerli poi in un secondo momento.
     * Se disattivo (No): non posso mai creare carichi di magazzino.
     * Valorizzata in base impostazioni:
     *   - Raccolta_Con_Carico_Magazzino: 1065
     *   - SUPERUSER_GESTIONE_MAGAZZINO_ABILITATA: 208
     */
    public Raccolta_Con_Carico_Magazzino: boolean;
    /** Valorizzata in base valore impostazione
     *  UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI: 181
     */
    public lottiAttivi: boolean;

    public sezione_prodotto: FormGroup;
    public Id_Attivita: string;
    public operationType: Enum_DBTypeOperation;

    public loadEndEvent: BehaviorSubject<boolean>  = new BehaviorSubject(false);
    public SaveEvent: BehaviorSubject<any> = new BehaviorSubject(undefined);

    private Array_UdM: BaseCodeDescr[];
    private _progressivoRighe = 0;

    constructor(
        public qdcService: QdCService,
        private prodottiService: ProdottiService,
        private fabbricatiService: FabbricatiService,
        private CatMagService: CategorieMagazzinoService,
    ) {
        this.initData();
    }

    public get isNewRaccolta(): boolean {
        return this.Id_Attivita === ID_NUOVA_ATTIVITA;
    }

    public get isRecipe(): boolean {
        return this.qdcService.TestataForm.value.Tipo === 2;
    }

    public get isLottoManuale():boolean {
        return this.sezione_prodotto
                   .get('Opzioni_Raccolta').get('GenerazioneLotto')
                   .value === enum_Generazione_Lotto_Raccolta.MANUALE;
    }

    public get isLottoFromProg(): boolean {
        return this.sezione_prodotto
            .get('Opzioni_Raccolta').get('GenerazioneLotto')
            .value === enum_Generazione_Lotto_Raccolta.DA_PROGETTO_COD;
    }

    public get newProgressivo(): number {
        this._progressivoRighe++;
        return cloneDeep(this._progressivoRighe);
    }

    public get progressivo(): number {
        return cloneDeep(this._progressivoRighe);
    }

    public get harvestedProductsCount(): number {
        return (this.sezione_prodotto.get('ProdottiRaccolti') as FormArray)
                    .controls.length;
    }

    public get harvestDate(): Date {
        return this.sezione_prodotto.get('Opzioni_Raccolta').get('dataIngresso').value
    }

    private get centroAziendale(): CentroAziendale {
        return this.qdcService.TestataForm.get("Centro_Aziendale").value;
    }

    public isLottoEditable(GenMode?: enum_Generazione_Lotto_Raccolta): boolean {
        if (GenMode === undefined) {
            return this.isLottoManuale || this.isLottoFromProg;
        }
        return GenMode === enum_Generazione_Lotto_Raccolta.MANUALE
            || GenMode === enum_Generazione_Lotto_Raccolta.DA_PROGETTO_COD;
    }

    public get isStorageSelected(): boolean {
        for (let product of this.sezione_prodotto.value.ProdottiRaccolti) {
            if (!product.MagazziniMovimentazioni) {
                continue;
            }
            for (let movement of product.MagazziniMovimentazioni) {
                if (movement.Magazzino && movement.Magazzino.primaryKey.codice !== 0) {
                    return true;
                }
            }
        }
        return false;
    }

    // Enable/disable the save button base on events
  /** Disable the save button while editing the harvest grid. */
    startEditing() {
        this.sezione_prodotto.get('isNotEditing_Raccolta').patchValue(false);
    }

  /** Enable the save button after editing the harvest grid. */
    stopEditing() {
        this.sezione_prodotto.get('isNotEditing_Raccolta').patchValue(true);
    }

    updateHarvestDate() {
        let hDate = this.harvestDate
        let prodotti = this.sezione_prodotto.get('ProdottiRaccolti') as FormArray
        prodotti.controls.forEach( (ctrl: FormControl<DettaglioRaccolta>) => {
            let raccolta = ctrl.value;
            raccolta.dataIngresso = hDate;
            ctrl.patchValue(raccolta)
        })
    }

    // ADD/REMOVE FROM SEZIONE PRODOTTO ////////////////////////////////////////

    public addDettaglioRaccolta(dettaglio: DettaglioRaccolta): FormControl<DettaglioRaccolta> {
        let array = this.sezione_prodotto
                        .get('ProdottiRaccolti') as FormArray;
        let control = new FormControl(dettaglio);
        array.push(control);
        return control;
    }

    public removeDettaglioRaccolta(index: number) {
        let array = this.sezione_prodotto
                        .get('ProdottiRaccolti') as FormArray;

        if (index < 0 || index > array.controls.length - 1) return null;
        if (array.controls.length - 1 === 0) {
            this.sezione_prodotto.get('Opzioni_Raccolta')
                .get('GenerazioneLotto').disable();
        }

        return array.controls.splice(index, 1);
    }

    public updateFastHarvestTemplate(ExSelected: number[], ExUnselected: GridRaccoltaObject[]) {
        let control = this.sezione_prodotto.get('FastHarvestTemplate');
        let template = control.value as DettaglioRaccolta;

        ExUnselected.forEach(un => {
            let index = template.QuantitaSuImpianti.findIndex(q =>
                q.esercizioCDC.esercizio.codice === un.Prodotto_Cod
            );

            template.QuantitaSuImpianti.splice(index, 1);
        });

        ExSelected.forEach(s => {
            let cdc = this.qdcService.GetEserciziCDCSelezionatiModel().find(e =>
                e.esercizio.codice === s
            );

            let qsi = new QuantitaSuImpianto();
            qsi.esercizioCDC = cdc;
            qsi.Qta = 0;

            template.QuantitaSuImpianti.push(qsi);
        });

        control.patchValue(template);
    }

    /** Push to the end by default */
    public addNewRaccoltaAt(index: number = this.harvestedProductsCount) {
        let dettaglio = this.createRaccolta();

        let array = this.sezione_prodotto
                        .get('ProdottiRaccolti') as FormArray;
        let control = new FormControl(dettaglio);

        array.insert(index, control);
    }

    /** Deletes all the current records */
    public clear() {
        (this.sezione_prodotto.get('ProdottiRaccolti') as FormArray).clear();
    }

    // DDL MANAGEMENT //////////////////////////////////////////////////////////

    public leggiMagazzini() {
        const LeggiMagazzini = <LeggiMagazzini_QdC>{
            lavorazione: this.sezione_prodotto.get("Operazione").value,
            impresa: this.qdcService.getImpresa_Model(),
            data: this.qdcService.TestataForm.get("Data").value
        };
        return this.fabbricatiService.Leggi_Magazzini(LeggiMagazzini)
          .pipe(take(1), map(Array_Magazzini => {
            Array_Magazzini.forEach(m => {
              m['chiaveMagazzino'] = m.primaryKey.codice + "_"
                + m.primaryKey.centroAziendalePK.codice + "_"
                + m.primaryKey.centroAziendalePK.partitaIva;
            });
            return Array_Magazzini;
          }));
    }

  public async leggiProdotti(): Promise<Prodotto[]> {
        let params = <LeggiProdotti>{
            specie: this.qdcService.GetSpeciefromUtilizzoTerreno(),
            impianti: this.qdcService.GetImpiantiSelezionatiModel()
        };

        return this.prodottiService.Leggi_Trasformati_Vegetali_Qdc(params)
        .then( res => {
            if (!res) return [];

            let list = res.map(item => new Prodotto(
                item.prodotto.codice,
                item.prodotto.descrizione + ' (' + item.codArticolo + ')'
            ));

            return list;
        });
    }

    public leggiUDM(): Observable<BaseCodeDescr[]> {
        if (this.Array_UdM) {
          return of(this.Array_UdM);
        }
        return this.CatMagService.leggiUnitaMisura(TRASFORMATI_VEGETALI)
          .pipe(take(1), map((x: BaseCodeDescr[]) => {
              this.Array_UdM = x.filter(udm => udm.codice !== enum_UnitaMisura.Litri && udm.codice !== enum_UnitaMisura.Confezioni)
              return this.Array_UdM;
          }));
    }

    // PRIVATE FUNCTIONS ///////////////////////////////////////////////////////

    private initData() {
        let sezioni = this.qdcService.QdCForm.get("Trattamento")
            .get("Sezioni_Prodotto") as FormArray;
        let codiciAttivita = this.qdcService.QdCForm.get("Trattamento")
                .get('Testata').get('Codici_Attivita').value;

        this.Id_Attivita = codiciAttivita.at(0).CodiceAttivita;
        this.operationType = this.qdcService.objParametriAgenda.TipoOperazioneDB;

        this.sezione_prodotto = sezioni.controls.find(c =>
            + c.value.Operazione.primaryKey.codice === enum_LAVCOD.RACCOLTA
        ) as FormGroup;
        this.explodeMagazziniMovimentazioni();
    }

    private createRaccolta(): DettaglioRaccolta {
        let raccolta = new DettaglioRaccolta();

        raccolta.dataIngresso = this.harvestDate
        raccolta.TipoRaccolta = Tipo_Raccolta.Fast;
        raccolta.quantitaTotaleReale = 0;
        raccolta.QuantitaSuImpianti = [];
        raccolta.prodotto = new Prodotto(0);
        raccolta.unitaDiMisura = new UnitaDiMisura(
                                    enum_UnitaMisura.KG, ''
                                );

        let esercizi = this.qdcService.GetEserciziCDCSelezionatiModel();
        raccolta.QuantitaSuImpianti = esercizi.map( cdc => {
            let suImpianto = new QuantitaSuImpianto();
            suImpianto.Qta = 0;
            suImpianto.esercizioCDC = cdc;
            return suImpianto;
        });
        raccolta.MagazziniMovimentazioni = [];

        if (this.Raccolta_Con_Carico_Magazzino)
            raccolta.MagazziniMovimentazioni.push(new RilevamentoDiMagazzino());

        return raccolta;
    }

    /**
     * Crea più DettagliRaccolta nel caso in cui un prodotto possedesse più movimentazioni su magazzini diversi.
     * @private
     */
    private explodeMagazziniMovimentazioni() {
        let exploded: DettaglioRaccolta[] = [];
        let prodotti = this.sezione_prodotto.get('ProdottiRaccolti') as FormArray;
        let copy: DettaglioRaccolta;
        this.reduceMagazzini(prodotti);

        for (let prd of prodotti.value) {
            if (!prd.MagazziniMovimentazioni || !prd.MagazziniMovimentazioni.length) {
                exploded.push(prd);
                continue;
            }

            for (let mov of prd.MagazziniMovimentazioni) {
                let mgz = mov.Magazzino as Fabbricato;
                let lotto = mov.Lotto;
                copy = cloneDeep(prd);

                copy.MagazziniMovimentazioni = [];
                copy.MagazziniMovimentazioni.push(mov);

                if (mgz) {
                    let filtered = copy.QuantitaSuImpianti.filter(
                        (qsi: QuantitaSuImpianto) =>
                        this.fabbricatiService.Equals(qsi.Magazzino, mgz) &&
                        qsi.Lotto === lotto
                    );

                    copy.QuantitaSuImpianti = filtered;
                }
                copy.quantitaTotaleReale = copy.QuantitaSuImpianti.map(q => q.Qta)
                                               .reduce((q1,q2) => q1 +q2)
                if (Math.abs(prd.quantitaTotaleReale - copy.quantitaTotaleReale) < 0.2) {
                    copy.quantitaTotaleReale = prd.quantitaTotaleReale;
                }
                exploded.push(copy);
            }
        }
        prodotti.clear();

        exploded.forEach(prd => prodotti.push(new FormControl(prd)));
    }

    /**
     * Riduce i magazzini aggregando quelli riferiti alla stessa coppia prodotto/magazzino.
     * @param prodotti FormArray de prodotti raccolti
     * @private
     */
    private reduceMagazzini(prodotti: FormArray) {
        let prodottiValue = prodotti.value as Array<DettaglioRaccolta>;
        prodotti.clear();
        for (let dettaglio of prodottiValue) {
            const iteratorMovimentazioni = this.groupMovimentazioniByProdottoMagazzino(dettaglio.MagazziniMovimentazioni);
            dettaglio.MagazziniMovimentazioni = [];
            let movimentazioni = iteratorMovimentazioni.next();
            while (!movimentazioni.done) {
                let movTotale = movimentazioni.value.reduce((a,b) => { a.Qta += b.Qta; return a; });
                dettaglio.MagazziniMovimentazioni.push(movTotale);
                movimentazioni = iteratorMovimentazioni.next();
            }
            prodotti.push(new FormControl(dettaglio));
        }
    }

    private groupMovimentazioniByProdottoMagazzino(movimentazioni: MovimentoDiMagazzino[]): IterableIterator<MovimentoDiMagazzino[]> {
        let movimentazioniProdottoXMagazzino = new Map<string, MovimentoDiMagazzino[]>();
        const getKeyProdottoXMagazzino = (m: MovimentoDiMagazzino) => m.Prodotto.codice + "_"
            + m.Magazzino.primaryKey.centroAziendalePK.partitaIva + "_"
            + m.Magazzino.primaryKey.centroAziendalePK.codice + "_"
            + m.Magazzino.primaryKey.codice;
        movimentazioni?.forEach(mov => {
            if (movimentazioniProdottoXMagazzino.has(getKeyProdottoXMagazzino(mov))) {
                movimentazioniProdottoXMagazzino.get(getKeyProdottoXMagazzino(mov)).push(mov);
            } else {
                movimentazioniProdottoXMagazzino.set(getKeyProdottoXMagazzino(mov), [mov]);
            }
        });
        return movimentazioniProdottoXMagazzino.values();
    }

}
