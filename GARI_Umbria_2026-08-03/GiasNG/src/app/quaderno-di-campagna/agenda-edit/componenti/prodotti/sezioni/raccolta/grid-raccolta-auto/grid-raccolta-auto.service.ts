import {Injectable, Injector, Renderer2} from '@angular/core';
import {FormArray, FormControl, FormGroup} from '@angular/forms';
import {TranslocoService} from '@jsverse/transloco';
import {BaseCodeDescr} from 'app/Model/baseClass/baseCodeDescr';
import {CommandsColumnSettings, ExcelSettings} from 'gias-kendo-grid';
import {
    DropdownListItem,
    DropdownListWithForm,
    EditingMode,
    KendoGridColumn,
    KendoServerResult,
    LoaderType, NumericSettings,
    RendererGridEvent
} from 'gias-kendo-grid';
import {AbstractGridConfigService, HttpAction} from 'gias-kendo-grid';
import {filter, from, lastValueFrom, map, Observable, of, switchMap, take, takeUntil, tap} from 'rxjs';
import {GridRaccoltaModel, GridRaccoltaObject} from '../raccolta.model';
import {QdCRaccoltaService} from '../../../../../service/prodotti/raccolta.service';
import {QdCService} from '../../../../../service/qdc.service';
import {TRASFORMATI_VEGETALI} from 'app/Model/CostantiPersonalizzate';
import {DettaglioRaccolta} from 'app/Model/attivita/dettagli/DettaglioRaccolta';
import {QuantitaSuImpianto} from 'app/Model/attivita/dettagli/QuantitaSuImpianto';
import {UnitaDiMisura} from 'app/Model/metaschema/UnitaDiMisura';
import {Prodotto} from 'app/Model/attivita/risorse/Prodotto';
import {RilevamentoDiMagazzino} from 'app/Model/attivita/RilevamentoDiMagazzino';
import {Fabbricato} from 'app/Model/anagrafiche/Fabbricato';
import {MasterService} from 'app/Service/master.service';
import {Tipo_Raccolta} from 'app/Model/attivita/Attivita';
import {enum_Generazione_Lotto_Raccolta} from '../opzioni-raccolta/opzioni-raccolta.model';
import {enum_UnitaMisura} from 'app/Model/TipiEnumerativi';
import {EsercizioCDC} from '../../../../../../../Model/attivita/centri_di_costo/EsercizioCDC';
import {isInteger} from 'lodash';


export class GridRaccoltaAutoServerResult extends KendoServerResult{
    constructor(model, columns, rows) {
        super(model, columns, rows);
    }
}

@Injectable({ providedIn: 'root' })
export class GridRaccoltaAutoConfigService extends AbstractGridConfigService<GridRaccoltaAutoServerResult> {
    editingMode: EditingMode = EditingMode.IN_LINE;
    loader: LoaderType = LoaderType.SERVICE;
    rowId = 'Op_Cod';
    gridId = 'GridRaccolta';

    private carichiAttivi = true;
    private lottoManuale = false;


    private isProductSelected = false;
    private isStorageSelected = false;

    kendoRows: GridRaccoltaObject[] = [];
    kendoModel = new GridRaccoltaModel();
    kendoColumns: KendoGridColumn[] = [
        new KendoGridColumn({ field:'Prodotto_Cod', title: this.transloco.translate('qdc.ProdottoRaccolto')},{ editable: this.qdcservice.abilitaGrid }),
        new KendoGridColumn({ field:'UM_Cod', title: this.transloco.translate('qdc.um')},{ editable: this.qdcservice.abilitaGrid, hidden:true }),
        new KendoGridColumn({ field:'Qta', title: this.transloco.translate('qdc.qta')},{ editable: this.qdcservice.abilitaGrid, hidden:true, numeric: new NumericSettings({
                defaultValue: 0, format: 'n4', min: 0, step: 0.0001, decimals: 4
            })}),
        new KendoGridColumn({ field:'Progetto_Cod', title: this.transloco.translate('Esercizio')},{ editable: false, hidden:false }),
      new KendoGridColumn({field: 'Progetto_Des', title: this.transloco.translate('LottoImpianto')}, {
        editable: false,
        hidden: false
      }),
        new KendoGridColumn({ field:'Lotto', title: this.transloco.translate('LottoDiAccettazione')},{ editable: false, hidden:true }),
        new KendoGridColumn({ field:'Magazzino_Cod', title: this.transloco.translate('qdc.Magazzino')},{ editable: this.qdcservice.abilitaGrid, hidden:true }),
    ];

    constructor(injector: Injector,
        private renderer: Renderer2,
        transloco: TranslocoService, // necessario per traduzioni
        private qdcservice: QdCService,
        private raccoltaService: QdCRaccoltaService,
        private masterService: MasterService,
    ) {
        super(injector);
        this.handleCustomizatons();
        this.handleEvents();

        this.handleOptionsChange();
        this.handleColumnsVisibility();
    }


    private get eserciziCDC(): Array<EsercizioCDC> {
        return this.qdcservice.GetEserciziCDCSelezionatiModel();
    }

	override applyRendererRules(opts: RendererGridEvent): void {
		let visibleRows: [] = this.gridPublicService.gridElRef.nativeElement.querySelectorAll('tbody tr');
        let rows: [] = this.gridPublicService.gridComp.data['data'];

        this.disableLotForPrdNoMag(this.renderer, visibleRows, rows);
	}

    read(options?: any): Observable<GridRaccoltaAutoServerResult> {
        return from(this.loadRows().then( rows => {
            console.log('reload', rows)
            this.kendoRows = rows;
            this.handleColumnsVisibility();
            return  new GridRaccoltaAutoServerResult(this.kendoModel, this.kendoColumns, this.kendoRows);
        }));
    }

    perform(actionType: HttpAction, items: any): Observable<any[]> {
        if (actionType === HttpAction.CREATE) {
            this.raccoltaService.stopEditing();
        } else if (actionType === HttpAction.UPDATE) {
            this.raccoltaService.stopEditing();
        }
        console.log(actionType, items);
        return null;
    }

    private async loadRows(): Promise<GridRaccoltaObject[]> {
      if (this.kendoRows.length) {
        // Le righe sono già valorizzate, probabilmente stiamo
        // aggiornando la vista della griglia
        // (16/04/2024): le righe in gridPublicService sembra essere vuoto
        // return this.gridPublicService.giasGridComponent.rows as GridRaccoltaObject[];
        return this.kendoRows;
      }
      return this.getRowsFromDettaglioRaccolta();
    }

    private async getRowsFromDettaglioRaccolta(): Promise<GridRaccoltaObject[]> {
        let rows: GridRaccoltaObject[] = [];

        let prodotti = this.raccoltaService.sezione_prodotto
                            .get('ProdottiRaccolti') as FormArray;

        if (prodotti.length === 1 && (!prodotti.value[0].prodotto || prodotti.value[0].prodotto.codice === 0)) {
            this.raccoltaService.clear();
        }

        for (let prodotto of prodotti.controls) {
            let row = new GridRaccoltaObject();

            row.Prodotto_Cod = prodotto.value.prodotto?.codice;
            row.Prodotto_Des = await this.findProdottoDescr(prodotto.value.prodotto?.codice);

            row.Qta = isInteger(prodotto.value.quantitaTotaleReale) ?
                prodotto.value.quantitaTotaleReale : Math.round(prodotto.value.quantitaTotaleReale * 100)/100;

            let mov = prodotto.value.MagazziniMovimentazioni?.at(0);

            if (mov) {
                row.Lotto = mov.Lotto;

                if (mov.Magazzino) {
                    row.Magazzino_Cod = mov.Magazzino.primaryKey.codice + "_"
                        + mov.Magazzino.primaryKey.centroAziendalePK.codice + "_"
                        + mov.Magazzino.primaryKey.centroAziendalePK.partitaIva;
                    let mag = await lastValueFrom(this.findMagazzino(
                        row.Magazzino_Cod,
                        mov.Magazzino.descrizione,
                    ));
                    row.Magazzino_Des = mag.descrizione;
                }
            }

            row.UM_Cod = prodotto.value.unitaDiMisura?.codice;
            row.UM_Des = prodotto.value.unitaDiMisura?.descrizione;

            row.Op_Cod = prodotto.value.codice
                            ? prodotto.value.codice
                            : this.raccoltaService.newProgressivo;

            this.setRiferimentoEsercizio(row);

            rows.push(row);

            let p = prodotto.value;
            p.codice = prodotto.value.codice ? prodotto.value.codice : row.Op_Cod;
            prodotto.patchValue(p);
        }

        return rows;
    }

    private setRiferimentoEsercizio(item: GridRaccoltaObject) {
        item.Progetto_Cod = this.eserciziCDC.map(cdc => cdc.esercizio.codice.toString())
            .reduce((a, b) => a + "|" + b);
        item.Progetto_Des = this.eserciziCDC.map(cdc => cdc.esercizio.descrizione.toString())
            .reduce((a, b) => a + "|" + b);
    }

    ///////////////////////////////////////////////////////////////////////////

    private handleEvents() {
        this.newRow();
        this.handleImpiantiSelectionChange();

        this.handleFormChanges();
        this.handleEdit();
    }

    private handleEdit() {
      this.gridPublicService.changeDetected.pipe(
        takeUntil(this.signal),
        filter((ch: any) => ch.rowIndex >= 0 && (ch['action'] === 'cellClose' || ch['action'] === 'save')),
        tap(ch => {
          if (ch['action'] === 'save' || ch['action'] === 'cancel')
            this.raccoltaService.stopEditing();
        }),
        switchMap((ch: any) => {
          let rowIndex = ch.rowIndex;
          let item = ch.dataItem;
          this.setRiferimentoEsercizio(item);
          if (ch['action'] === 'save') {
            item = ch['formGroup'].value;
            // Copy form values in row
            ch.dataItem.Prodotto_Cod = item.Prodotto_Cod;
            ch.dataItem.Prodotto_Des = item.Prodotto_Des;
            ch.dataItem.Magazzino_Cod = item.Magazzino_Cod;
            ch.dataItem.Magazzino_Des = item.Magazzino_Des;
            ch.dataItem.UM_Cod = item.UM_Cod;
            ch.dataItem.UM_Des = item.UM_Des;
            ch.dataItem.Qta = item.Qta;
            ch.dataItem.Lotto = item.Lotto;
          }
          return this.updateItem(rowIndex, item);
        })
      ).subscribe(() => {
        this.handleColumnsVisibility();
        setTimeout(() => this.gridPublicService.refresh(true), 100);
      });
    }

    private handleFormChanges() {
        this.gridPublicService.formGroup.GiasSubscribe((form: FormGroup) => {
            if (!form) return;
            this.raccoltaService.startEditing();
            form.valueChanges.GiasSubscribe(change => {
                // Controlli sulla singola riga in modifica
                if (!this.isProductSelected && change.Prodotto_Cod)
                    this.isProductSelected = true;

                if (!change.Progetto_Des || !change.Progetto_Cod) {
                    this.setRiferimentoEsercizio(change);
                }

                if (!this.isStorageSelected && change.Magazzino_Cod)
                    this.isStorageSelected = true;

                // Deseleziono il prodotto o il magazzino
                if (this.isStorageSelected && !change.Magazzino_Cod
                    || this.isProductSelected && !change.Prodotto_Cod)
                    this.handleColumnsVisibility();

                this.updateVisibility();
                this.toggleFields();
            });
        });
    }

    private updateDettaglioProdotto(control: FormControl, change: GridRaccoltaObject): Observable<void> {
        let raccolta = control.value as DettaglioRaccolta;
        let prodotto = this.setProdotto(raccolta, change);
        let um = this.setUDM(raccolta, change);
        this.ripartizionaQuantita(raccolta, change);
        return this.setStorage(raccolta, change, prodotto, um).pipe(map(() => {
            raccolta.quantitaTotaleReale = change.Qta;
            this.setTipoRaccolta(raccolta);
            this.handleColumnsVisibility();
            control.patchValue(raccolta);
        }));
    }

    /** Imposta il magazzino a un dettaglio raccolta
     *
     * @private
     * @return `Observable<boolean>` contenente true se il magazzino è stato valorizzato (carichi attivi), false altrimenti.
     */
    private setStorage(raccolta: DettaglioRaccolta, change: GridRaccoltaObject, prodotto: Prodotto, um: UnitaDiMisura): Observable<boolean> {
        if (this.carichiAttivi) {
            const cod2Find = change.Prodotto_Cod ? change.Magazzino_Cod : '0';
            const desc2Find = change.Prodotto_Cod ? change.Magazzino_Des : '';
            return this.findMagazzino(cod2Find, desc2Find)
              .pipe(tap(magazzino => {
                  if (magazzino) {
                      if (!raccolta.MagazziniMovimentazioni)
                          raccolta.MagazziniMovimentazioni = [];
                      if (!raccolta.MagazziniMovimentazioni.length)
                          raccolta.MagazziniMovimentazioni.push(
                            new RilevamentoDiMagazzino()
                          );
                  }
                  if (raccolta.MagazziniMovimentazioni) {
                      raccolta.MagazziniMovimentazioni.forEach(mov => {
                          mov.Magazzino = magazzino;
                          mov.Prodotto = prodotto;
                          mov.udm = um;
                          mov.Lotto = change.Lotto;
                          mov.Qta = change.Qta;
                          mov.QtaTot = change.Qta;
                      });
                  }
                  raccolta.QuantitaSuImpianti.forEach((q: QuantitaSuImpianto) => {
                      q.Lotto = change.Lotto;
                      q.Magazzino = magazzino;
                  });
              }), map(() => true));
        } else return of(false);
    }

    private ripartizionaQuantita(raccolta: DettaglioRaccolta, change: GridRaccoltaObject) {
        const supTot = raccolta.QuantitaSuImpianti.map(q => q.esercizioCDC.superficieTrattata)
                                                  .reduce((a, b) => a + b);
        const ratio = (e: QuantitaSuImpianto) => e.esercizioCDC.superficieTrattata / supTot;
        raccolta.QuantitaSuImpianti.forEach(suImpianto => suImpianto.Qta = change.Qta * ratio(suImpianto));
    }

    private setProdotto(r: DettaglioRaccolta, c: GridRaccoltaObject): Prodotto {
        let prodotto = new Prodotto(0);

        if(!!c) {
            prodotto.codice = c.Prodotto_Cod ? c.Prodotto_Cod : 0;
            prodotto.descrizione = c.Prodotto_Des ? c.Prodotto_Des : '';
        }

        prodotto.elemCod = TRASFORMATI_VEGETALI;

        r.prodotto = prodotto;
        return prodotto;
    }

    private setUDM(r: DettaglioRaccolta, c: GridRaccoltaObject): UnitaDiMisura {
        let um = new UnitaDiMisura(enum_UnitaMisura.KG, '');

        if (!!c) {
            um.codice = c.UM_Cod ? c.UM_Cod : um.codice;
            um.descrizione = c.UM_Des ? c.UM_Des : um.descrizione;
        }

        r.unitaDiMisura = um;

        return um;
    }

    private setTipoRaccolta(r: DettaglioRaccolta) {
        r.TipoRaccolta = Tipo_Raccolta.Fast;
        if (r.prodotto.codice) {
            r.TipoRaccolta = Tipo_Raccolta.Leggera;
        }
        if (r.MagazziniMovimentazioni && r.MagazziniMovimentazioni.length
            && !!r.MagazziniMovimentazioni[0].Magazzino.primaryKey.codice) {
            r.TipoRaccolta = Tipo_Raccolta.Leggera_Con_Dettagli_Magazzino;
        }
    }

    private updateItem(rowIndex: number, item: GridRaccoltaObject) {
        const i = rowIndex;
        if (i === undefined || i < 0) return of(null);
        const control = (this.raccoltaService.sezione_prodotto
            .get('ProdottiRaccolti') as FormArray).controls
            .find(c => c.value.codice === item.Op_Cod) as FormControl;
        if (!control) return of(null);
        return this.updateDettaglioProdotto(control, item);
    }

    private handleImpiantiSelectionChange() {
        this.qdcservice.GridImpiantiHttpService.impiantiLoaded$
          .pipe(takeUntil(this.signal))
          .subscribe(() => this.refresh());
        this.qdcservice.SelezioneEsercizi.GiasSubscribe(es => {
            if (!es || !this.raccoltaService.sezione_prodotto.get('ProdottiRaccolti').value
                || !this.raccoltaService.sezione_prodotto.get('ProdottiRaccolti').value.at(0)) {
                return;
            }
            let esercizi = es.value;
            let current = this.raccoltaService.sezione_prodotto
                            .get('ProdottiRaccolti').value.at(0)
                            .QuantitaSuImpianti
                            .map((q: QuantitaSuImpianto) => q.esercizioCDC.esercizio.codice);

            let Es_Cod = esercizi.map(e => e.Progetto_Cod) as Array<number>;
            let selected = Es_Cod.filter(a => !current.includes(a));
            let unselected = current.filter(a => !Es_Cod.includes(a));

            this.raccoltaService.updateFastHarvestTemplate(selected, unselected);

            for (let p of (this.raccoltaService.sezione_prodotto
                            .get('ProdottiRaccolti') as FormArray).controls) {
                let prodotto = p.value as DettaglioRaccolta;

                this.addImpiantiFromCod(prodotto, selected);
                this.removeImpiantiFromCod(prodotto, unselected);
                let tmp = new GridRaccoltaObject();
                tmp.Qta = prodotto.quantitaTotaleReale;
                this.ripartizionaQuantita(prodotto, tmp);

                p.patchValue(prodotto);
                let ch = new GridRaccoltaObject();
                ch.Qta = prodotto.quantitaTotaleReale;
            }
            this.kendoRows.forEach(row => this.setRiferimentoEsercizio(row));
        });
    }

    private addImpiantiFromCod(prodotto: DettaglioRaccolta, impianti: number[]) {
        impianti.forEach(s => {
            let newData = new QuantitaSuImpianto();
            let template = prodotto.QuantitaSuImpianti.find(q => q.Lotto);
            newData.Qta = 0;
            newData.esercizioCDC = this.eserciziCDC
                .find(e => e.esercizio.codice === s);
            if (template) {
                newData.Lotto = template.Lotto;
                newData.Magazzino = template.Magazzino;
            }
            prodotto.QuantitaSuImpianti.push(newData);
        });
    }

    private removeImpiantiFromCod(prodotto: DettaglioRaccolta, impianti: number[]) {
        impianti.forEach(u => {
            let index = prodotto.QuantitaSuImpianti.findIndex(q =>
                q.esercizioCDC.esercizio.codice === u
            );
            prodotto.QuantitaSuImpianti.splice(index,1);
        });
    }

    private newRow() {
        this.gridPublicService.changeDetected.pipe(
            takeUntil(this.signal),
            filter(ev => ev['action']),
            switchMap((ev: any) => {
                switch(ev['action']) {
                    case 'save':
                        if (ev.rowIndex >= 0)
                            return of(null);
                        ev.dataItem.Op_Cod = this.raccoltaService.newProgressivo;
                        this.kendoRows.push(ev.dataItem);
                        return this.addRaccoltaProdotto(ev.dataItem)
                          .pipe(tap(() => this.refresh()));
                    case 'remove':
                        this.removeRaccoltaProdotto(ev.dataItem);
                        const index = this.kendoRows.findIndex(
                          r => r.Op_Cod === ev.dataItem.Op_Cod
                        );
                        if (index > -1) this.kendoRows.splice(index, 1);
                        break;
                }
                return of(null);
          })).subscribe(() => this.handleColumnsVisibility());
    }

    private removeRaccoltaProdotto(row: GridRaccoltaObject) {
        let prodotti = this.raccoltaService.sezione_prodotto
                           .get('ProdottiRaccolti') as FormArray;

        let index = prodotti.controls.findIndex(
            (ctrl: FormControl<DettaglioRaccolta>) =>
                ctrl.value.codice === row.Op_Cod
        );

        if (index < 0) return;

        prodotti.removeAt(index);
    }

    /** Adds a FormControl to this.raccolta.sezione_prodotto */
    private addRaccoltaProdotto(row: GridRaccoltaObject) {
        let array = this.raccoltaService.sezione_prodotto
                        .get('ProdottiRaccolti') as FormArray;
        let raccolta = new DettaglioRaccolta();
        raccolta.codice = row.Op_Cod;
        raccolta.quantitaTotaleReale = 0;
        raccolta.QuantitaSuImpianti = this.eserciziCDC.map( cdc => {
            let ri = new QuantitaSuImpianto();
            ri.Qta = 0;
            ri.esercizioCDC = cdc;
            ri.Lotto = row.Lotto;
           // ri.Magazzino = this.fabbricatiService.getBlankFabbricato();
            return ri;
        });
        raccolta.MagazziniMovimentazioni = [];
        if (this.carichiAttivi) {
            raccolta.MagazziniMovimentazioni.push(new RilevamentoDiMagazzino());
        }
        raccolta.dataIngresso = this.raccoltaService.harvestDate;
        let control = new FormControl(raccolta);
        return this.updateDettaglioProdotto(control, row)
          .pipe(tap(() => array.push(control)));
    }

    ///////////////////////////////////////////////////////////////////////////

    private handleCustomizatons() {
        this.cmdColumn = new CommandsColumnSettings({
            editBtn: this.qdcservice.abilitaGrid,
            infoBtn: false,
            removeBtn: this.qdcservice.abilitaGrid,
            onDisableInfoBtn: () => false,
        });
        this.behavior.excelSettings = new ExcelSettings({enabled: false});
        this.groups.groupable.enabled = false;
        this.views.enabled = false;
        this.pagination.navigable = true;

        this.toolbar.newItem = this.qdcservice.abilitaGrid;
        this.raccoltaService.loadEndEvent.GiasSubscribe(end => {
            if (!end) return;

            this.manageCarichiEnabled();

            this.isStorageSelected = false;
            this.kendoRows.forEach( row => {
                if (!!row.Magazzino_Cod) {
                    this.isStorageSelected = true;
                }
            });

            if (!this.isStorageSelected || !this.carichiAttivi) return;

            this.kendoColumns.find(s => s.field === 'Lotto').hidden = !this.carichiAttivi || this.raccoltaService.isRecipe || !this.raccoltaService.lottiAttivi;
            this.kendoColumns.find(s => s.field === 'Lotto').includeInChooser = this.carichiAttivi && !this.raccoltaService.isRecipe;
            this.kendoColumns.find(s => s.field === 'Magazzino_Cod').hidden = !this.carichiAttivi;
            this.kendoColumns.find(s => s.field === 'Magazzino_Cod').includeInChooser = this.carichiAttivi;
        });

        this.handleDDL();
    }

    private manageCarichiEnabled() {
        this.carichiAttivi = this.raccoltaService.Raccolta_Con_Carico_Magazzino;

        (this.raccoltaService.sezione_prodotto
            .get('ProdottiRaccolti').value as DettaglioRaccolta[])
            .forEach(raccolta => {

                if (raccolta.MagazziniMovimentazioni?.length) {
                    this.carichiAttivi = true;
                }

            });
    }

    private handleDDL() {
        this.handleUDM();
        this.handleProdottoDDL();
        this.handleMagazzinoDDL();
    }

    private handleUDM() {
        let col = this.kendoColumns.find(c => c.field === 'UM_Cod');
        let data: DropdownListItem[] = [];
        this.raccoltaService.leggiUDM().subscribe(R =>
            R.forEach((item: BaseCodeDescr) => data.push(new DropdownListItem(item.codice, item.descrizione)))
        );
        col.ddl = new DropdownListWithForm('id', 'UM_Cod', 'name', data);
        col.ddl.loadOnEdit = false;
        col.ddl.valuePrimitive =  true;
        col.ddl.descriptionField = 'UM_Des';
    }

    private handleProdottoDDL() {
        let col = this.kendoColumns.find(c => c.field === 'Prodotto_Cod');
        let data: DropdownListItem[] = [];

        this.raccoltaService.leggiProdotti().then(res => {
            res.forEach((item: BaseCodeDescr) => data.push(new DropdownListItem(
                    item.codice, item.descrizione
            )));

            if (!data.length) {
                data.push(new DropdownListItem(
                    -1, this.transloco.translate('qdc.ProdottoDaSpecie')
                ));
            }
            data.unshift(new DropdownListItem(
                0, this.transloco.translate('qdc.NessunaSelezione')
            ));
        });

        col.ddl = new DropdownListWithForm('id', 'Prodotto_Cod', 'name', data);
        col.ddl.loadOnEdit = false;
        col.ddl.valuePrimitive =  true;
        col.ddl.descriptionField = 'Prodotto_Des';
    }

    private async findProdottoDescr(codice: number): Promise<string> {
        if (!codice){
          //return new Promise<string>((resolve, reject) => resolve(''));
          return Promise.resolve('');
        }


        let prodotti = await this.raccoltaService.leggiProdotti();

        let p = prodotti.find(pr => pr.codice === codice);

        return !!p ? p.descrizione : '';
    }

    private findMagazzino(codice: string, descrizione: string) {
        return this.raccoltaService.leggiMagazzini().pipe(map(magazzini => {
            let mag = magazzini.find((m: Fabbricato) => m['chiaveMagazzino'] === codice);
            if (mag) return mag;
            else return new Fabbricato({
                codice: 0,
                centroAziendalePK: {
                    codice: 0,
                    partitaIva: this.masterService.objP_server.PivaSuperUser
                }
            });
        }));
    }

    private handleMagazzinoDDL() {
        let col = this.kendoColumns.find(c => c.field === 'Magazzino_Cod');
        let data: DropdownListItem[] = [];

        this.raccoltaService.leggiMagazzini().pipe(take(1)).subscribe( res => {
            res.forEach((item) => data.push(new DropdownListItem(
                item['chiaveMagazzino'],
                item.descrizione,
                item
            )));
            data.unshift(new DropdownListItem(0, this.transloco.translate('qdc.NessunaSelezione')));
        });

        col.ddl = new DropdownListWithForm('id', 'Magazzino_Cod', 'name', data);
        col.ddl.loadOnEdit = false;
        col.ddl.valuePrimitive =  true;
        col.ddl.descriptionField = 'Magazzino_Des';
    }

    ///////////////////////////////////////////////////////////////////////////

    private refresh() {
        let newGrid = new GridRaccoltaAutoServerResult(
            this.kendoModel, this.kendoColumns, this.kendoRows
        );

        this.gridPublicService.refresh(false, newGrid);
    }

    private handleColumnsVisibility() {
        this.isProductSelected = false;
        this.isStorageSelected = false;

        this.raccoltaService.sezione_prodotto.get('ProdottiRaccolti').value
        .forEach(row => {
            if (row.prodotto?.codice) {
                this.isProductSelected = true;
            }
            if (this.carichiAttivi && row.MagazziniMovimentazioni?.at(0)?.Magazzino?.primaryKey?.codice) {
                this.isStorageSelected = true;
            }
        });
        this.updateVisibility();
        this.toggleFields();
    }

    private updateVisibility() {
        // Mostro/Nascondo le colonne dipendenti dal prodotto
        this.kendoColumns.find(s => s.field === 'Qta').hidden = !this.isProductSelected;
        this.kendoColumns.find(s => s.field === 'UM_Cod').hidden = !this.isProductSelected;
        this.kendoColumns.find(s => s.field === 'Magazzino_Cod').hidden = !(this.isProductSelected && this.carichiAttivi);
        this.kendoColumns.find(s => s.field === 'Lotto').hidden = !(this.isProductSelected && this.raccoltaService.lottiAttivi)
            || this.raccoltaService.isRecipe;
        this.kendoColumns.find(s => s.field === 'Progetto_Des').hidden = !(this.isProductSelected
            && this.isStorageSelected && this.raccoltaService.isLottoFromProg && this.raccoltaService.lottiAttivi);
        this.kendoColumns.find(s => s.field === 'Progetto_Cod').hidden = !(this.isProductSelected
            && this.isStorageSelected && this.raccoltaService.isLottoFromProg && this.raccoltaService.lottiAttivi);
    }

    private toggleFields() {
        let optGenLotto = this.raccoltaService.sezione_prodotto
                              .get("Opzioni_Raccolta")
                              .get("GenerazioneLotto") as FormControl;
        if (this.isStorageSelected && this.raccoltaService.isNewRaccolta) optGenLotto.enable();
        else optGenLotto.disable();

        this.kendoColumns.find(s => s.field === 'Lotto').editable = (
            this.isStorageSelected
            && (this.raccoltaService.isLottoManuale || this.raccoltaService.isLottoFromProg)
            && !this.raccoltaService.isRecipe
        );
    }

    private handleOptionsChange() {
        let optGenLotto = this.raccoltaService.sezione_prodotto
                              .get("Opzioni_Raccolta")
                              .get("GenerazioneLotto") as FormControl;

        this.lottoManuale = (optGenLotto.value === enum_Generazione_Lotto_Raccolta.MANUALE);

        optGenLotto.valueChanges.GiasSubscribe( optGen => {
            this.lottoManuale = optGen === enum_Generazione_Lotto_Raccolta.MANUALE;
            let lottoFromProgetto = optGen === enum_Generazione_Lotto_Raccolta.DA_PROGETTO_COD;

            this.kendoColumns.find(col => col.field === 'Progetto_Des').hidden =
                !(lottoFromProgetto && this.isStorageSelected && this.raccoltaService.lottiAttivi);
            this.kendoColumns.find(col => col.field === 'Progetto_Cod').hidden =
                !(lottoFromProgetto && this.isStorageSelected && this.raccoltaService.lottiAttivi);
            this.kendoColumns.find(s => s.field === 'Lotto').editable = (
                this.raccoltaService.isLottoEditable(optGen) && this.isStorageSelected
            );

            if (this.raccoltaService.isLottoEditable(optGen)) {
                this.kendoRows.forEach(r => {
                    let dettagli = this.raccoltaService.sezione_prodotto
                                         .get('ProdottiRaccolti').value;
                    let curr: DettaglioRaccolta =  dettagli.find(d => d.codice === r.Op_Cod);

                    if (!curr) return;

                    r.Lotto = curr.QuantitaSuImpianti[0].Lotto;
                });
            } else {
                this.kendoRows.forEach(r => {
                    r.Lotto = this.transloco.translate('GeneratoAutomaticamente');
                });
            }
        });

    }

    // TODO(Anny):
    /**
     * Disabilita il campo lotto per le righe in cui è stato selezionato il
     * prodotto ma non il magazzino.
     * Si presuppone che la colonna Lotto sia visibile per via di un altra riga
     * provvista di magazzino selezionato.
     */
    private disableLotForPrdNoMag(renderer: any, domElems: any[], rows: any[]) {
        rows.forEach((dataItem: any, index: number) => {
            let currDomRow = domElems[index];

            if (!dataItem.Magazzino_Cod) {
                let currDom = domElems[index];
                let cell = currDom.children[4];

            }
        });
    }

}
